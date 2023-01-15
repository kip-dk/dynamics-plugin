$echo off
call pack
call ..\Kipon.Xrm.Cmd\Bin\Debug\Kipon.Xrm.Cmd "deploy" "/config:kipon-tools.json"