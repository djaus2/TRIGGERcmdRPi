echo off
REM Used by WhatCanISay App on PC
REM Place in c:\temp
REM Add contents of commands.json to commands.json on PC in <user>\.TRIGGERcmdData, or via GUI/Text Command Editor
REM Build app dotnet build:  
REM dotnet build -c RELEASE WhatCanISay.csproj 
REM ... in WhatCanISay folder. Need path here or add to Path.
Rem set PATH="%PATH%;"C:\Users\david\source\repos\djaus2\TRIGGERcmdRPi\WhatCanISayApp\bin\Release\net5"
C:\Users\david\source\repos\djaus2\TRIGGERcmdRPi\WhatCanISayApp\bin\Release\net5\WhatCanISay "%1" "%2"
cast -device "Nest"    -file c:\temp\saythis.txt 
REM NB echo command is used by RPi to forward its saythis.txt to Google Home
