using System.IO;
using Newtonsoft;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace WhatCanISay
{
	class Program
	{
		private const string _home = "/home/pi";
		private const string _TRIGGERcmdData = ".TRIGGERcmdData";
		private const string _commands = "commands.json";
		private const string _tempfile = "saythis.txt";
		

		// A gem from here: https://mariusschulz.com/blog/detecting-the-operating-system-in-net-core
		public static class OperatingSystem
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
			string home ;
			string TRIGGERcmdData="";
			string commands ;

			if (OperatingSystem.IsWindows())
			{
				home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
				TRIGGERcmdData = home + "\\" + _TRIGGERcmdData +"\\";
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
			using (StreamReader reader = File.OpenText(commands))
			{
				dynamic o = (dynamic)JToken.ReadFrom(new JsonTextReader(reader));
				whatToSay = Iterate(o);
			}

			Console.WriteLine(whatToSay);
			WriteT2S(whatToSay);
		}

		public static void WriteT2S(string txt)
		{

			string tmp = "/tmp/temperature.txt";
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
			string what = $"These are the commands you can say,{txt}";
			string[] lines = what.Split(',');
			File.WriteAllLines(tmp,lines);
			return;
		}
		public static string Iterate(dynamic variable)
		{
			string whatToSay="";
			if (variable.GetType() == typeof(Newtonsoft.Json.Linq.JArray))
			{
				Console.WriteLine("type is Array");
				foreach (var item in variable)
				{
					if (item.GetType() == typeof(Newtonsoft.Json.Linq.JObject))
					{
						Console.WriteLine("type is Object");
						foreach (var property in item)
						{
							string name="";
							string voice =""
							string description ="";
							string command="";
							string offcommand = "";							
							bool allowParams=false;
							
							Console.WriteLine("property name: " + property.Name.ToString());
							Console.WriteLine("property value: " + property.Value.ToString());
							if (property.Name.ToString() == "voice")
							{
								voice = property.Value.ToString();
								if (whatToSay != "")
									whatToSay += ",";
								whatToSay += $"{property.Value.ToString()}";
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
								if (val.toLOower() = "true")
									allowParams = true;
							}
							if (!string.IsNullOrEmpty(voice)
							{
								if (whatToSay != "")
									whatToSay += ",";
								whattosay += voice;
								if ((!string.IsNullOrEmpty(offcommand) && allowParams)
								{
									whatToSay += $",On or Off";
								}						
								if (!string.IsNullOrEmpty(description)
								{
									whatToSay += $"Description,{property.Value.ToString()}";
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
