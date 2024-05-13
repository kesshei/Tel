#!/bin/sh
FileName='fast.service'
systemctl stop $FileName
systemctl disable $FileName
echo systemctl status $FileName
