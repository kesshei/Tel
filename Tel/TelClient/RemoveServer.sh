#!/bin/sh
FileName='telclient.service'
systemctl stop $FileName
systemctl disable $FileName
echo systemctl status $FileName
