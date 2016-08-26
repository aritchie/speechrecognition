using System;
using System.Threading;
using System.Threading.Tasks;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public Task<string> Listen(CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
/*
bool permissionGained = await AudioCapturePermissions.RequestMicrophonePermission();
if (!permissionGained)
{
 //ask user to modify settings
} 
     */