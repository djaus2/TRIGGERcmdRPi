#!/bin/sh
# Place this in ~
# chmod +x cast.sh

# whatcanisay is .NET app that reads commands.com and gets teh voice for each command.
# will also get command descriptions if in the file.
# These are then writen to the file as lines. (lines means a pause with cast).
# file is the data file that that app writes to
# targetpc is Windows PC where cast runs
# trigger is the TRIGGERcmd on the targetPC to be called via curl that runs cast
file="/tmp/saythis.txt"
targetpc='DJsSufaceBook2V2'
trigger='WhatCanISayRelay'

var1='{"computer":"'
var2=$targetpc
var3='","trigger":"'
var4=$trigger
var5='","params":"'
var6='Getting what you can say.'
var7='"}'

# Need to set path to WhatCanISay or have in the WSL's path
/home/pi/Trigger/WhatCanISayApp/bin/Release/net5/WhatCanISay remotedetail

if [ -f $file ];then
    echo "$file exists"
    var6="$(cat $file)"

else
    echo "$file doesn't exist"
    var6="Data file doesn't exist".
fi

var=$var1$var2$var3$var4$var5$var6$var7
echo
echo $var
echo

curl -X POST https://www.triggercmd.com/api/run/triggerSave \
-H 'authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjYxZGE3Nzk3ZjgzMTFiMDAxYTNmNDEzZSIsImlhdCI6MTY0NTA2NzE3NX0.uGIYujCImI0i8z1ph4uYp85dRiy1Gw7tCI9BU6P5ZOY' \
-H 'content-type: application/json' \
-d "$var"