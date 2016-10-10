using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Speech;
using UIKit;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        readonly IPermissions permissions;


        public SpeechRecognizerImpl(IPermissions permissions = null)
        {
            this.permissions = permissions ?? CrossPermissions.Current;
        }


        public async Task<bool> RequestPermission()
        {
            var result = await this.permissions.RequestPermissionsAsync(Permission.Speech);
            return result[Permission.Speech] == PermissionStatus.Granted;
        }


        public IObservable<string> Listen(bool completeOnEndOfSpeech)
        {
            return Observable.Create<string>(ob =>
            {
                SFSpeechRecognitionTask task = null;
                NSError error = null;

                var audioEngine = new AVAudioEngine();
                var speechRecognizer = new SFSpeechRecognizer();
                var speechRequest = new SFSpeechAudioBufferRecognitionRequest
                {
                    ShouldReportPartialResults = true,
                    TaskHint = completeOnEndOfSpeech
                        ? SFSpeechRecognitionTaskHint.Search
                        : SFSpeechRecognitionTaskHint.Dictation
                };

                audioEngine.InputNode.InstallTapOnBus(
                    bus: 0,
                    bufferSize: 1024,
                    format: audioEngine.InputNode.GetBusOutputFormat(0),
                    tapBlock: (buffer, when) => speechRequest.Append(buffer)
                );
                audioEngine.StartAndReturnError(out error);
                if (error != null)
                {
                    ob.OnError(new Exception(error.LocalizedDescription));
                }
                else
                {
                    task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
                    {
                        if (err != null)
                        {
                            ob.OnError(new Exception(err.LocalizedDescription));
                            return;
                        }
                        if (result.Final)
                        {
                            var words = result.BestTranscription.FormattedString.Split(' ');
                            foreach (var word in words)
                                ob.OnNext(word);

                            if (completeOnEndOfSpeech)
                                ob.OnCompleted();
                        }
                    });
                }
                return () =>
                {
                    task?.Cancel();
                    task?.Dispose();
                    audioEngine.Stop();
                    audioEngine.Dispose();
                    speechRequest.EndAudio();
                    speechRequest.Dispose();
                    speechRecognizer.Dispose();
                };
            });
        }


        public bool IsSupported => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
    }
}