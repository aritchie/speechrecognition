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
                this.ListenSubject.OnNext(true);
                task = speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
                {
                    if (err != null)
                    {
                        ob.OnError(new Exception(err.LocalizedDescription));
                    }
                    else if (result.Final)
                    {
                        ob.OnNext(result.BestTranscription.FormattedString);
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
                this.ListenSubject.OnNext(false);
            };
        });
    }
}