 #!/bin/sh
# Place this in ~
# chmod +x cast.sh

# DNETCoreGPIO is .NET app (with param 14) that reads temperature and Humidty from DHT sensor 
# file is the data file that that app writes to
# targetpc is Windows PC where cast runs
# trigger is the TRIGGERcmd on the targetPC to be called via curl that runs cast
file="/tmp/saythis.txt"
targetpc='BIGMOMMA5'
trigger='Echo'

var1='{"computer":"'
var2=$targetpc
var3='","trigger":"'
var4=$trigger
var5='","params":"'
var6='Reading'
var7='"}'

var=$var1$var2$var3$var4$var5$var6$var7

# echo $var
 curl -X POST https://www.triggercmd.com/api/run/triggerSave \
 -H 'authorization: Bearer <insert token>' \
 -H 'content-type: application/json' \
 -d "$var"

# Need to wait for this to finish.
 sleep 4

DNETCoreGPIO 15


if [[ -f $file ]];then
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
-H 'authorization: Bearer <insert token>' \
-H 'content-type: application/json' \
-d "$var"