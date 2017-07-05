using System;
using System.Reactive.Linq;
using AVFoundation;
using Foundation;
using Plugin.Permissions.Abstractions;
using Speech;
using UIKit;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        public SpeechRecognizerImpl(IPermissions permissions = null)
        {
        }


        protected override bool IsSupported => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
        public override SpeechRecognizerStatus Status { get; }
        public override IObservable<string> ListenUntilPause()
        {
            throw new NotImplementedException();
        }


        public override IObservable<string> ContinuousDictation()
        {
            throw new NotImplementedException();
        }


        public override IObservable<bool> RequestPermission()
        {
            throw new NotImplementedException();
        }
    }
}

//public override IObservable<string> Listen(bool completeOnEndOfSpeech)
        //{
        //    return Observable.Create<string>(ob =>
        //    {
        //        SFSpeechRecognitionTask task = null;
        //        NSError error = null;

        //        var audioEngine = new AVAudioEngine();
        //        var speechRecognizer = new SFSpeechRecognizer();
        //        var speechRequest = new SFSpeechAudioBufferRecognitionRequest
        //        {
        //            ShouldReportPartialResults = true,
        //            TaskHint = completeOnEndOfSpeech
        //                ? SFSpeechRecognitionTaskHint.Search
        //                : SFSpeechRecognitionTaskHint.Dictation
        //        };

        //        audioEngine.InputNode.InstallTapOnBus(
        //            bus: 0,
        //            bufferSize: 1024,
        //            format: audioEngine.InputNode.GetBusOutputFormat(0),
        //            tapBlock: (buffer, when) => speechRequest.Append(buffer)
        //        );
        //        audioEngine.StartAndReturnError(out error);
        //        if (error != null)
        //        {
        //            ob.OnError(new Exception(error.LocalizedDescription));
        //        }
        //        else
        //        {
        //            this.ListenSubject.OnNext(true);
        //            task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
        //            {
        //                if (err != null)
        //                {
        //                    ob.OnError(new Exception(err.LocalizedDescription));
        //                    return;
        //                }
        //                var words = result.BestTranscription.FormattedString.Split(' ');
        //                foreach (var word in words)
        //                    ob.OnNext(word);

        //                if (result.Final && completeOnEndOfSpeech)
        //                    ob.OnCompleted();
        //            });
        //        }
        //        return () =>
        //        {
        //            task?.Cancel();
        //            task?.Dispose();
        //            audioEngine.Stop();
        //            audioEngine.Dispose();
        //            speechRequest.EndAudio();
        //            speechRequest.Dispose();
        //            speechRecognizer.Dispose();
        //            this.ListenSubject.OnNext(false);
        //        };
        //    });
        //}