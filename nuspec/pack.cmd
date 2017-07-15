@echo off
del *.nupkg
nuget pack Plugin.SpeechRecognition.nuspec
rem nuget pack Plugin.SpeechDialogs.nuspec
pause