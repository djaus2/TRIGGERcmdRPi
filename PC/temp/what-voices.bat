@echo off
REM Used by WhatCanISay App on PC
REM Place in c:\temp
REM Add contents of commands.json to commands.json on PC in <user>\.TRIGGERcmdData, or via GUI/Text Command Editor
REM Build app dotnet build:  
REM dotnet build -c RELEASE WhatCanISay.csproj 
REM ... in WhatCanISay folder. Need path here or add to Path.
<Path to built app folder>\WhatCanISay
Rem eg;"C:\Users\USER\source\repos\djaus2\TRIGGERcmdRPi\WhatCanISayApp\bin\Release\net5\WhatCanISay"
cast -device "Nest"    -file c:\temp\saythis.txt 
REM NB echo command is used by RPi to forward its saythis.txt to Google Home
