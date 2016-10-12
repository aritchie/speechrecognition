@echo off
copy *.nupkg C:\users\allan.ritchie\dropbox\nuget\ /y
nuget push Acr.SpeechDialogs*.nupkg -Source https://www.nuget.org/api/v2/package
nuget push Acr.SpeechRecognition*.nupkg -Source https://www.nuget.org/api/v2/package
pause