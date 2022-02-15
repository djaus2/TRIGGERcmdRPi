using System;
using WhatCanISayTRIGGERcmd;

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

		static void Main(string[] args)
		{
			WhatCanISayLib.Mode mode = WhatCanISayLib.Mode.simple;
			string jsonCmds = WhatCanISayLib.TRIGGERcmds.GetCmdsPath(args, out mode);


			if (mode == WhatCanISayLib.Mode.relay)
			{
				string whatToSay = jsonCmds;
				WhatCanISayLib.Parse(mode, "", whatToSay);
			}
			else
			{
				string jsonCommandsPath = jsonCmds;
				WhatCanISayLib.Parse(mode, jsonCommandsPath, "");
			};
		}
	}
}

		
