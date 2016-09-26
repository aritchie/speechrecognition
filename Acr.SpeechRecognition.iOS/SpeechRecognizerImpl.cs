using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using Speech;
using UIKit;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public Task<bool> RequestPermission()
        {
            var state = SFSpeechRecognizer.AuthorizationStatus;
            if (state != SFSpeechRecognizerAuthorizationStatus.NotDetermined)
                return Task.FromResult(state == SFSpeechRecognizerAuthorizationStatus.Authorized);

            var tcs = new TaskCompletionSource<bool>();
            SFSpeechRecognizer.RequestAuthorization(status =>
            {
                var result = status == SFSpeechRecognizerAuthorizationStatus.Authorized;
                tcs.TrySetResult(result);
            });
            return tcs.Task;
        }


        IObservable<string> listenOb;
        public IObservable<string> Dictate()
        {

            this.listenOb = this.listenOb ?? Observable.Create<string>(ob =>
            {
                SFSpeechRecognitionTask task = null;
                NSError error = null;

                var audioEngine = new AVAudioEngine();
                var speechRecognizer = new SFSpeechRecognizer();
                var speechRequest = new SFSpeechAudioBufferRecognitionRequest
                {
                    ShouldReportPartialResults = true,
                    TaskHint = SFSpeechRecognitionTaskHint.Dictation
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
                        var words = result.BestTranscription.FormattedString.Split(' ');
                        foreach (var word in words)
                            ob.OnNext(word);

                        // I want this to be endless so user can dictate or listen for one word and cancel
                        //if (result.Final)
                        //    ob.OnCompleted();

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
            })
            .Publish()
            .RefCount();

            return this.listenOb;
        }


        public bool IsSupported => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);


        IObservable<string> Listen(Action<IObserver<string>, SFSpeechRecognitionResult> onResult)
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
                    TaskHint = SFSpeechRecognitionTaskHint.Dictation
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
                        onResult(ob, result);
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
    }
}