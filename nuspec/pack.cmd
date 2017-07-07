@echo off
del *.nupkg
rem nuget pack Acr.SpeechRecognition.nuspec
nuget pack Plugin.SpeechRecognition.nuspec
rem nuget pack Plugin.SpeechDialogs.nuspec
pause