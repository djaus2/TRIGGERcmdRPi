using System.IO;
using Newtonsoft;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;

/// <summary>
/// If run with no parameters, interogates local comms.json, default mode detail
/// If run with detail/s,voices/s runs in that mode.
/// - Voice/s just lists the voice commnds
/// - Detail/s list exactly what to say and can give description.
/// Relay is when called remotely where commas need conversion to newline.
/// - Remote device passes csv string over TRIGGERcmd
/// - The remote device does the interogation but does not do the comma to newline conversion.
/// </summary>
namespace WhatCanISay
{
	class Program
	{
		private const string _home = "/home/pi";
		private const string _TRIGGERcmdData = ".TRIGGERcmdData";
		private const string _commands = "commands.json";
		private const string _tempfile = "saythis.txt";
		private const string intro =  "These are the commands you can say ";
		enum Mode { simple, detail, relay, remotesimple,  remotedetail};

		private static class OperatingSystem
		{
			public static bool IsWindows() =>
				RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

			public static bool IsMacOS() =>
				RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

			public static bool IsLinux() =>
				RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		}

		static void Main(string[] args)
		{
			//C:\Users\DavidJones\\.TRIGGERcmdData
			string home;
			string TRIGGERcmdData = "";
			string commands;

			int i = args.Length;

			Mode mode = Mode.detail;

			if (args.Length != 0)
			{
				switch (args[0].ToLower())
				{
					case "voice":
					case "voices":
						mode = Mode.simple;
						break;
					case "detail":
					case "details":
						mode = Mode.detail;
						break;
					case "remotedetail":
					case "remotedetails":
						mode= Mode.remotedetail
						break;
					case "renotevoice":
					case "remotevoices":
						mode = Mode.remotesimple;
						break;
					default:
						// If relaying from Pi then csv string is passed.
						mode = Mode.relay;
						break;
				}

			}

			if (OperatingSystem.IsWindows())
			{
				home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				TRIGGERcmdData = home + "\\" + _TRIGGERcmdData + "\\";
			}
			else if (OperatingSystem.IsLinux())
			{
				home = "/home/pi";
				TRIGGERcmdData = home + "/" + _TRIGGERcmdData + "/";
				Console.WriteLine(TRIGGERcmdData);
			}
			commands = TRIGGERcmdData + _commands;
			
			Console.WriteLine(commands);
			string whatToSay = "";

			if (mode == Mode.relay)
			{
				whatToSay = args[0];
			}
			else
			{
				using (StreamReader reader = File.OpenText(commands))
				{
					dynamic commandz = (dynamic)JToken.ReadFrom(new JsonTextReader(reader));
					whatToSay = Iterate(commandz, mode);
				}
			}

			Console.WriteLine(whatToSay);
			WriteT2S(whatToSay, mode);
		}

		public static void WriteT2S(string txt, Mode mode)
		{

			string tmp = "/tmp/saythis.txt";
			if (OperatingSystem.IsWindows())
			{
				tmp = $"c:\\temp\\{_tempfile}";
			}
			else if (OperatingSystem.IsLinux())
			{
				tmp = $"/tmp/{_tempfile}";
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
			if ((mode == Mode.detail) ||  (mode == Mode.simple))
            { 
				string[] lines = what.Split(',');
				File.WriteAllLines(tmp,lines);
			}
			else
				File.WriteAllText(txt);
			return;
		}
		private static string Iterate(dynamic variable, Mode mode)
		{
			string whatToSay="";
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
							else if (property.Name.ToString() == "offcommand")
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
								whatToSay = $"{intro}, ";
							if ((mode == Mode.detail) || (mode == remotedetail))
							{
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
							if ((mode == Mode.detail) || (mode == remotedetail))
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
	}
}
