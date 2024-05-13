#!/bin/sh
ServerPath='/lib/systemd/system'
FileName='telclient.service'

chmod +x $FileName
chmod +x RemoveServer.sh

cp $FileName $ServerPath/$FileName

systemctl enable $FileName
systemctl start $FileName
systemctl status $FileName
echo 'init end'
