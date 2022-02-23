using System;
using static WhatCanISayTRIGGERcmd.WhatCanISayLib;

/// <summary>
/// If run with no parameters, interogates local commands.json, default mode voices
/// If run with detail/s,voices/s runs in that mode.
/// - Voice/s just lists the voice commnds
/// - Detail/s list exactly what to say and can give description.
/// Relay is when called remotely where commas need conversion to newline.
/// - Remote device passes csv string over TRIGGERcmd
/// - The remote device does the interogation but does not do the comma to newline conversion.
/// - Use remotevoices or remotedetail on the remote device (eg RPi)
/// - Requires PC running cast to forward message
/// </summary>
namespace WhatCanISay
{
	class Program
	{
		static void Main(string[] args)
		{
			
			Program_Main.Main(args);
		}
	}
}

		
