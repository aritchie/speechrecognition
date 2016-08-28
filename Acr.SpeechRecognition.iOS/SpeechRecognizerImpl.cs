using System;
using System.Reactive.Linq;
using AVFoundation;
using Foundation;
using Speech;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public IObservable<string> Listen()
        {
            return Observable.Create<string>(ob =>
            {
                var speechRecognizer = new SFSpeechRecognizer();
                var speechRequest = new SFSpeechAudioBufferRecognitionRequest();
                var audioEngine = new AVAudioEngine();

                audioEngine.InputNode.InstallTapOnBus(
                    bus: 0,
                    bufferSize: 1024,
                    format: audioEngine.InputNode.GetBusOutputFormat(0),
                    tapBlock: (buffer, when) => speechRequest.Append(buffer)
                );

                NSError error = null;
                audioEngine.StartAndReturnError(out error);
                var task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
                {
                    ob.OnNext(result.BestTranscription.FormattedString);
                    if (result.Final)
                        ob.OnCompleted();
                });

                return () =>
                {
                    task.Cancel();
                    task.Dispose();
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
/*

    SFSpeechRecognizer.RequestAuthorization(status =>
    {
        if (status != SFSpeechRecognizerAuthorizationStatus.Authorized)
            return;

    });
*/