REM Used by WhatCanISay App on PC
REM Place in c:\temp
REM Add contents of commands.json to commands.json on PC in <user>\.TRIGGERcmdData, or via GUI/Text Command Editor
REM Build app dotnet build:  
REM dotnet build -c RELEASE WhatCanISay.csproj 
REM ... in WhatCanISay folder. Need path here or add to Path.
<Path to WhatCanISay folder>\bin\Release\net6.0\WhatCanISay
cast -device "Family Room speaker"    -file c:\temp\sayythis.txt 
REM NB echo command is used by RPi to forward its saythis.txt to Google Home
