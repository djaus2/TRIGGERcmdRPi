using System;
using Newtonsoft;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;

/// <summary>
/// If run with no parameters, interogates local comms.json, default mode detail
/// If run with detail/s,voices/s runs in that mode.
/// - Voice/s just lists the voice commnds
/// - Detail/s list exactly what to say and can give description.
/// Relay is when called remotely where commas need conversion to newline.
/// - Remote device passes csv string over TRIGGERcmd
/// - The remote device does the interogation but does not do the comma to newline conversion.
/// Sample usage:
////////////////////////////////////////////////////////////////////////////////////////////
//// static void Main(string[] args)
//// {
////	WhatCanISayLib.Mode mode = WhatCanISayLib.Mode.simple;
////	string jsonCmds = WhatCanISayLib.TRIGGERcmds.GetCmdsPath(args, out mode);
////
////
////	if (mode == WhatCanISayLib.Mode.relay)
////	{
////		string whatToSay = jsonCmds;
////		WhatCanISayLib.Parse(mode, "", whatToSay);
////	}
////	else
////	{
////		string jsonCommandsPath = jsonCmds;
////		WhatCanISayLib.Parse(mode, jsonCommandsPath, "");
////	};
//// }
/////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>
namespace WhatCanISayTRIGGERcmd
{
	public static class OperatingSystem
	{
		public static bool IsWindows() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

		public static bool IsMacOS() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

		public static bool IsLinux() =>
			RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
	}


	public static class WhatCanISayLib
	{
		/// <summary>
		///  These can be customised:
		/// </summary>
		public static string TempFilename { get; set; } = "saythis.txt";
		public static string TRIGGERcmdDataFolderName { get; set; } = ".TRIGGERcmdData";
		public static string commandsJsonFilename { get; set; } = "commands.json";
		public static string IntroText { get; set; } = "These are the commands you can say ";

		/// <summary>
		/// These can be teh first parameter as a string to set teh operational mode
		/// </summary>
		public enum Mode { simple, detail, relay, remotesimple, remotedetail }

		// The TRIGGERcmdDataFolderName is assumed to be under this folder:
		private const string _home = "/home/pi";

		/// <summary>
		/// 1. Reads commands.json if not relay mode and geneedates what to say as csv string.
		/// 2. Then splits into lines based upon csv.
		/// 3. Writes to temp file.
		/// 4. Returns the path of that file.
		/// For relay mode, skip step 1. 
		/// </summary>
		/// <param name="mode">One of Mode type</param>
		/// <param name="commandsJsonPath">Path to commands.json on this system</param>
		/// <param name="whatToSay">Alt: What to say as csv string</param>
		/// <returns>Path to the temp file created.</returns>
		public static string Parse(Mode mode, string commandsJsonPath, string whatToSay)
		{


			Console.WriteLine(commandsJsonPath);


			if (mode != Mode.relay)
			{
				whatToSay = "";
				using (StreamReader reader = File.OpenText(commandsJsonPath))
				{
					dynamic commandz = (dynamic)JToken.ReadFrom(new JsonTextReader(reader));
					whatToSay = Iterate(commandz, mode);
				}
			}

			return WriteT2S(whatToSay, mode);
		}

		/// <summary>
		/// Write the temp file
		/// </summary>
		/// <param name="txt">text to be spoken</param>
		/// <param name="mode">One of the Mode types. If not remote then split into lines based upon csv</param>
		/// <returns></returns>
		private static string WriteT2S(string txt, Mode mode)
		{

			string tmp = "/tmp/saythis.txt";
			if (OperatingSystem.IsWindows())
			{
				tmp = $"c:\\temp\\{TempFilename}";
			}
			else if (OperatingSystem.IsLinux())
			{
				tmp = $"/tmp/{TempFilename}";
			}
			if (File.Exists(tmp))
			{
				// If file found, delete it    
				File.Delete(tmp);
				Console.WriteLine("File deleted.");
			}
			// Puting in newlines means there is a pause between.
			// But on RPi don't split here, That is done on PC.
			string what = txt;
			Console.WriteLine($"Writing: {what}");
			if ((mode == Mode.detail) || (mode == Mode.simple) || mode== Mode.relay)
			{
				string[] lines = what.Split(',');
				File.WriteAllLines(tmp, lines);
			}
			else
				File.WriteAllText(tmp, what);
			return tmp;
		}

		/// <summary>
		/// Iterate through commands and extract command properties.
		/// Form what to say string,in csv format.
		/// </summary>
		/// <param name="variable"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		private static string Iterate(dynamic variable, Mode mode)
		{
			string whatToSay = "";
			if (variable.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
			{
				Console.WriteLine("type is Array");
				foreach (var item in variable)
				{
					if (item.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
					{
						//string name="";
						string voice = "";
						string description = "";
						//string command="";
						string offcommand = "";
						bool allowParams = false;
						Console.WriteLine("type is Object");
						foreach (var property in item)
						{


							Console.WriteLine("property name: " + property.Name.ToString());
							Console.WriteLine("property value: " + property.Value.ToString());
							if (property.Name.ToString() == "voice")
							{
								voice = property.Value.ToString();
							}
							else if (property.Name.ToString() == "description")
							{
								description = property.Value.ToString();
								// Can add a "description" property to a command to be spoken, in Text Command Editor.
							}
							else if (property.Name.ToString() == "offCommand")
							{
								offcommand = property.Value.ToString();
							}
							else if (property.Name.ToString() == "allowParams")
							{
								var val = property.Value.ToString();
								if (val == "true")
									allowParams = true;
							}

						}
						if (!string.IsNullOrEmpty(voice))
						{
							if (whatToSay != "")
								whatToSay += ", ";
							else
								whatToSay = $"{IntroText}, ";
							if ((mode == Mode.detail) || (mode == Mode.remotedetail))
							{
								if (whatToSay == "")
									whatToSay = $"{IntroText}, ";
								else
									whatToSay += ", ";
								whatToSay += "Turn ";
								if ((!string.IsNullOrEmpty(offcommand)) && allowParams)
								{
									whatToSay += $" On ,or Off, ";
								}
								else
								{
									whatToSay += " On ";
								}
							}
							whatToSay += voice;
							if ((mode == Mode.detail) || (mode == Mode.remotedetail))
							{
								if (!string.IsNullOrEmpty(description))
								{
									whatToSay += $",Description,{description}";
								}
							}

						}
					}
				}
			}
			return whatToSay;
		}

		public static class TRIGGERcmds
		{
			/// <summary>
			/// Gets path to commands.json for TRIGGERcmds on PC or RPi
			/// </summary>
			/// <param name="args">Can be mode as string, Default: The string to say.</param>
			/// <param name="mode">Returned mode as an enum</param>
			/// <returns>The path to commands.json or the text to be spoken</returns>
			public static string GetCmdsPath(string[] args, out Mode mode)
			{
				//C:\Users\DavidJones\\.TRIGGERcmdData
				string home;
				string TRIGGERcmdData = "";
				string commandsJsonPath;

				int i = args.Length;

				mode = WhatCanISayLib.Mode.simple;
				string retValue = "";

				if (args.Length != 0)
				{
					switch (args[0].ToLower())
					{
						case "simple":
						case "voice":
						case "voices":
							mode = WhatCanISayLib.Mode.simple;
							break;
						case "detail":
						case "details":
							mode = WhatCanISayLib.Mode.detail;
							break;
						case "remotedetail":
						case "remotedetails":
							mode = WhatCanISayLib.Mode.remotedetail;
							break;
						case "remote":
						case "renotevoice":
						case "remotevoices":
							mode = WhatCanISayLib.Mode.remotesimple;
							break;
						default:
							// If relaying from Pi then csv string is passed.
							mode = WhatCanISayLib.Mode.relay;
							retValue = args[0];
							break;
					}

				}
				else
					mode = Mode.simple;

				if (mode == WhatCanISayLib.Mode.relay)
				{
					retValue = args[0];
				}
				else
				{
					if (OperatingSystem.IsWindows())
					{
						home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
						TRIGGERcmdData = home + "\\" + TRIGGERcmdDataFolderName + "\\";
					}
					else if (OperatingSystem.IsLinux())
					{
						home = "/home/pi";
						TRIGGERcmdData = home + "/" + TRIGGERcmdDataFolderName + "/";
						Console.WriteLine(TRIGGERcmdData);
					}
					commandsJsonPath = TRIGGERcmdData + commandsJsonFilename;
					retValue = commandsJsonPath;
				};
				return retValue;
			}
		}
	}
}
