using System;
using System.Reactive.Linq;
using AVFoundation;
using Foundation;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Speech;
using UIKit;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        readonly IPermissions permissions;


        public SpeechRecognizerImpl(IPermissions permissions = null) => this.permissions = permissions ?? CrossPermissions.Current;


        public override bool IsSupported => UIDevice.CurrentDevice.CheckSystemVersion(10, 0);
        public override IObservable<string> ListenUntilPause() => this.Listen(true);
        public override IObservable<string> ContinuousDictation() => this.Listen(false);


        public override IObservable<SpeechRecognizerStatus> RequestPermission() => Observable.FromAsync(async ct =>
        {
            if (!this.IsSupported)
                return SpeechRecognizerStatus.NotSupported;

            var status = await this.permissions.RequestPermissionsAsync(Permission.Speech);
            switch (status[Permission.Speech])
            {
                case PermissionStatus.Restricted:
                case PermissionStatus.Denied:
                    return SpeechRecognizerStatus.PermissionDenied;

                case PermissionStatus.Unknown:
                    return SpeechRecognizerStatus.NotSupported;

                case PermissionStatus.Disabled:
                    return SpeechRecognizerStatus.Disabled;

                default:
                    return SpeechRecognizerStatus.Available;
            }
        });


        protected virtual IObservable<string> Listen(bool completeOnEndOfSpeech) => Observable.Create<string>(ob =>
        {
            var speechRecognizer = new SFSpeechRecognizer();
            if (!speechRecognizer.Available)
                throw new ArgumentException("Speech recognizer is not available");

            var speechRequest = new SFSpeechAudioBufferRecognitionRequest();
            var audioEngine = new AVAudioEngine();
            var format = audioEngine.InputNode.GetBusOutputFormat(0);

            if (!completeOnEndOfSpeech)
                speechRequest.TaskHint = SFSpeechRecognitionTaskHint.Dictation;

            audioEngine.InputNode.InstallTapOnBus(
                0,
                1024,
                format,
                (buffer, when) => speechRequest.Append(buffer)
            );
            audioEngine.Prepare();
            audioEngine.StartAndReturnError(out var error);

            if (error != null)
                throw new ArgumentException("Error starting audio engine - " + error.LocalizedDescription);

            this.ListenSubject.OnNext(true);

            var currentIndex = 0;
            var cancel = false;
            var task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
            {
                if (cancel)
                    return;

                if (err != null)
                {
                    ob.OnError(new Exception(err.LocalizedDescription));
                }
                else
                {
                    if (result.Final && completeOnEndOfSpeech)
                    {
                        currentIndex = 0;
                        ob.OnNext(result.BestTranscription.FormattedString);
                        ob.OnCompleted();
                    }
                    else
                    {
                        for (var i = currentIndex; i < result.BestTranscription.Segments.Length; i++)
                        {
                            var s = result.BestTranscription.Segments[i].Substring;
                            currentIndex++;
                            ob.OnNext(s);
                        }
                    }
                }
            });

            return () =>
            {
                cancel = true;
                task?.Cancel();
                task?.Dispose();
                audioEngine.Stop();
                audioEngine.InputNode?.RemoveTapOnBus(0);
                audioEngine.Dispose();
                speechRequest.EndAudio();
                speechRequest.Dispose();
                speechRecognizer.Dispose();
                this.ListenSubject.OnNext(false);
            };
        });
    }
}

//      protected virtual IObservable<string> Listen(bool completeOnEndOfSpeech) => Observable.Create<string>(ob =>
//      {
//    var speechRecognizer = new SFSpeechRecognizer();
//          var path = NSBundle.MainBundle.GetUrlForResource("plugin", "m4a");
//          var speechRequest = new SFSpeechUrlRecognitionRequest(path);

//  this.ListenSubject.OnNext(true);
//          var task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
//          {
//           if (err != null)
//           {
//               ob.OnError(new Exception(err.LocalizedDescription));
//           }
//           else if (result.Final)
//           {
//               ob.OnNext(result.BestTranscription.FormattedString);
//               if (completeOnEndOfSpeech)
//                   ob.OnCompleted();
//           }
//  });

//          return () =>
//          {
//        task?.Cancel();
//        task?.Dispose();
//        speechRequest.Dispose();
//        speechRecognizer.Dispose();
//        this.ListenSubject.OnNext(false);
//  };
//});