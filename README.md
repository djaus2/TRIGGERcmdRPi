# TRIGGERcmd Control of RPi

<hr/>

**NB: 26th Feb 2022_ Please use  [https://iothub.sportronics.com.au](https://iothub.sportronics.com.au) instead of ```https://davidjones.sportronics.com.au``` for a day or so. Thx**

<hr/>

**Latest:** _There is now a Nuget Package for the library. Additional Console app uses that rather than the library source._  
```Install-Package WhatCanISayTRIGGERcmd -Version 1.0.2```  
**PS** WhatCanISay app builds and runs OK on WSL: Windows Subsystem for Linux (Debian)  
**Nb: _Solution file for the app is now in the root folder._**
  
Trigger commands in .NET app running on Raspberry Pi from Google Nest with sensor values spoken on a Google Nest (Mini) via a PC.

# About

It was said that you can't get the temperature etc from a RPi, from a .Net app there, enunciated on a Google Nest as a Hey Google, request from the Nest. Well I've done it! It does though require an intervening Windows device to relay (cast) the text to the Nest.

# Requirements

- A Google Nest (I used a mini) with Home setup.
- RPi, I used a 3B+
- Install Debian OS on RPi
- [Install the .NET SDK or the .NET Runtime on Debian](https://docs.microsoft.com/en-us/dotnet/core/install/linux-debian)
- TRIGGERcmd installed on RPi [as per here](https://www.triggercmd.com/forum/topic/12/raspberry-pi-setup?_=1642757365671)
  - I found the setup script didn't work, but manual wasn't too difficult.
- TRIGGERcmd installed on PC. [Start here](https://www.triggercmd.com/en/)
  - Found you do need a subscription but its not costly
  - Follow instructions on the Instruction page
  - Get the token
- Cast installed on PC from [here](https://www.push2run.com/phpbb/viewtopic.php?t=1042)
- Insert the token in the curl command in Sensor_DHT1Wire.sh, Sensor_BME280.sh and what.sh on RPi
- Files as below installed copied to PC/RPi
- Clone [djaus2/DNETCoreGPIO](https://github.com/djaus2/DNETCoreGPIO) ro RPi and setup for .Net6 to build there or build on PC and deploy to RPi.
  - Add to PATH on RPi
- Make the RPi the default TRIGGERcmd compter.

## Running TRIGGERcmd agent

- On PC: The agent should auto start.
- On the RPi:in a command shell run ```triggercmdagent``` and enter the token when prompted.
  - Note trstart.sh and tr.sh commands as below

# Files

## PC
- .TRIGGERcmdData
  - commands.json
    - Add these via the TRIGGERcmds Text/GUI editor _(Hidden icons from Taskbar)._ after TRIGGERcmd installation on PC
    - **Echo** command takes a string as a parameter and calls cast to enunciate it on Google Nest
    - **WhatCanISay[Brief/Detail/Relay]** command interogates local commands.json and speaks voice commands via Google Nest as with Echo.
        - Command interogates local commands.json and forwards through PC (echo command there) 
          - ... to speak voice commands via Google Nest as with Echo.
        = Brief just lists the command voices
        - Detail:
          - If a command has a "description" property, that is also spoken 
          - If On/Off thenthat is explained.
        - Used by RPi to forward message through PC
- temp
  - Place contents in c:\temp
## RPi
- .TRIGGERcmdData
  - commands.json
    - Add the commands you want using Text editor to ~/.TRIGGERcmdData after TRIGGERcmd installation on RPi
      - **Sensor**: Calls Sensor_DHT1Wire.sh
        - Gets called from Nest with _Hey Google, start Sensor"_
        - Actions ```DNETCoreGPIO 14``` as above which writes/overwrites Temperature and Humidity string to /tmp/temperature.txt
        - String is read back in and curl command created to call echo on PC with the string as its parameter
        - The read temperature and humidity are enuncitaed on the Google Nest
      - **Local Sensor**: Calls speaklocal_DHT1Wire.sh
        - Gets called from Nest with _Hey Google, start local"_
        - As per Sensor but T2S is used to output directly to RPi Audio.
      - **BME280**: Calls Sensor_BME280.sh
        - Gets called from Nest with _Hey Google, start temp"_
        - Actions ```DNETCoreGPIO 15``` as above which writes/overwrites Temperature, Pressure and Humidity _(and Altitude which is erroneous)_ string to /tmp/temperature.txt
        - String is read back in and curl command created to call echo on PC with the string as its parameter
        - The read temperature and humidity are enuncitaed on the Google Nest
      - **BME280 Local**: Calls speaklocal_DHT1Wire.sh
        - Gets called from Nest with _Hey Google, start bosh"_
        - As per temp but T2S is used to output directly to RPi Audio.
      - **DNETMotorEnable**: ```DNETCoreGPIO 21 and 22``` as above
        - Gets called from Nest with _Hey Google, Start Motor_
          - Means enable motor
        - And _Hey Google, Stop Motor_
          - Means disable motor
      - **DNETMotorDirection**: ```DNETCoreGPIO 23 and 24``` as above
        - Gets called from Nest with _Hey Google, Start Spin_
          - Means forward if enabled.
        - And _Hey Google, Stop Spin_
          - Means reverse if enabled.
      - **DNETRelay** ```DNETCoreGPIO 11 and 12``` as above
        - Gets called from Nest with _Hey Google, Start Relay_
          - Means switch on relay.
        - And _Hey Google, Stop relay_
          - Means switch off relay.
      - **WhatCanISay[Brief/Detail]**
        - Command interogates local commands.json and forwards through PC (echo command there) 
          - ... to speak voice commands via Google Nest as with Echo.
        = Brief just lists the command voices
        - Detail:
          - If a command has a "description" property, that is also spoken 
          - If On/Off thenthat is explained.
      - There are other commands used by Azure IoT Hub __(later)__
- home_pi_
    - **Place following .sh files in ~ and chmod +x each.**
      - Sensor_DHT1Wire.sh
        - Place in ~ and chmod +x
        - Scripts all of the functionality for the **Sensor** command as below.
      - speaklocal_DHT1Wire.sh
        - Place in ~ and chmod +
        - As per Sensor_DHT1Wire.sh but Text to Speech is used directly on RPi
        - Ref: [How To Make Your Raspberry Pi Speak](https://www.dexterindustries.com/howto/make-your-raspberry-pi-speak/#:~:text=Make%20sure%20your%20Raspberry%20Pi%20is%20powered%20up,to%20convert%20text%20to%20speech%20on%20the%20speakers.)
         - ```sudo apt-get install espeak```
      - Sensor_BME280.sh/speaklocal_BME280.sh
        - As per DHT1Wire but using BME280 sensor
    - DNETCoreGPIO.sh
      - Calls DNETCoreGPIO app with integer parameter as below.
      - DNETCoreGPIO: Is .NET6.0 C# app
        - Repository on GitHub here: [djaus2/DNETCoreGPIO](https://github.com/djaus2/DNETCoreGPIO)
          - Clone with ```git clone github.com/djaus2/DNETCoreGPI``` in a folder on RPi in command shell
          - Build in folder with ```dotnet build -c RELEASE DNETCoreGPIO.csproj```
          - Need to add path to built DNETCoreGPIO  something like  ```projectdirectory/bin/Debug/Net6```
        - Called with various parameters by TRIGGERcmd on RPi:
          - 11.12 Turn relay on/off 
          - 14: Get Temperature and Humidity from DHT22 1 Wire mode
          - 15: Get Temperature, Presssure and Humidity from BME280 _(Altitude also but is erroneous)_.
          - 21: Motor Forward
          - 22: Motor Reverse
          - 23: Motor Enable
          - 24: Motor Disable
    - what-[voices/detail].sh
      - As per sensor_xx.sh scripts but calls whatcanisay app instead of DNETCoreGPIO app.
    - trstart.sh
      - Start TRIGGERcmd agent on RPi
      - Need user token
    - tr.sh
      - Restarts stopped TRIGGERcmd on RPi (token not required).     


 
## Apps
- WhatCanISay
  - Can ask a device,if set up there **"Hey Google, start Say"**.
    - Or **"Hey Google,start Detail"**.   
  - Will tell you what voice commands you can say on a device.
    - Second option tells you exactly what to say foreach command. 
  - Works on PC (uses cast)
  - On RPi forwards through Echo on PC. 2Do

**Coming:** Integrating with Azure IoT Hub
