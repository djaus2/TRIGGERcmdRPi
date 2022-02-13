 #!/bin/sh
# Place this in ~
# chmod +x cast.sh
file="/tmp/saythis.txt"

var6='Please wait while the sensor is read'
espeak -ven-au "$var6" 2>/dev/null

DNETCoreGPIO 15

if [[ -f $file ]];then
    echo "$file exists"
    var6="$(cat $file)"

else
    echo "$file doesn't exist"
    var6="Data file doesn't exist".
fi

espeak -ven-au "$var6" 2>/dev/null
