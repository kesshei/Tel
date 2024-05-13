CHCP 65001
@echo off
color 0e
@echo ==================================
@echo 提醒：请右键本文件，用管理员方式打开。
@echo ==================================
@echo Start Install TelServer

sc create TelServer binPath=%~dp0\TelServer.exe start= auto 
sc description TelServer "Tel"
Net Start TelServer
pause
