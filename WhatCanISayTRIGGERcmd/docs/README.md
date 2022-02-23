# What Can I Say Package

Package making use of [TRIGGERcmd](https://www.triggercmd.com/) that allows for remote execution of apps for Google Home. 
In particular generates a string detailing TRIGGERcmd commands
 that can be run on a device that can be passed back to a Google Nest device
 for enunciation there.
  
## Requirements
Needs .NET 5 installed on the device where it runs.

## When fully setup:
- On a Google Nest, can ask a device,if set up there __"Hey Google, start Say on Pi"__
- Or __"Hey Google, start Detail on Pi"__
- Will tell you what voice commands you can say on a device.
- Works on PC (uses cast)
- On RPi forwards through Relay command on PC.

## Usage

Create a .NET Console app add add this Nuget Packet.  Code

```
using static WhatCanISayTRIGGERcmd.WhatCanISayLib;

namespace WhatCanISayApp
{
	class Program
	{
		static void Main(string[] args)
		{
			Program_Main.Main(args);
		}
	}
}
```

## Running Console App

Normally used as TRIGGETcmd command to interogate the local commands.json file.

- If run with no parameters, interogates local commands.json, default mode voices
- If run with detail/s,voices/s runs in that mode.
  - Voice/s just lists the voice commnds
  - Detail/s list exactly what to say and can give a description.
- Relay is used when called remotely where commands need conversion to newline.
  - Remote device passes csv string over TRIGGERcmd
  - The remote device does the interogation but does not do the comma to newline conversion.
  - Actually Tilde for commas and undercore for space is relayed.
  - Use remotevoices or remotedetail on the remote device (eg RPi)
- Requires PC running cast to forward message

## Full equirements
Normally it would be used in unison with TRIGGERcmd. cast is also required on a PC.

- See setup on https://github.com/djaus2/TRIGGERcmdRPi




