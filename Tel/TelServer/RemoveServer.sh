#!/bin/sh
FileName='telserver.service'
systemctl stop $FileName
systemctl disable $FileName
echo systemctl status $FileName
