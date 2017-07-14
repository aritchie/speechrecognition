using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Speech;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        readonly IPermissions permissions;


        public SpeechRecognizerImpl(IPermissions permissions = null) => this.permissions = permissions ?? CrossPermissions.Current;


        public override bool IsSupported => SpeechRecognizer.IsRecognitionAvailable(Application.Context);
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
            var stop = false;
            var syncLock = new object();
            var speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
            var listener = new SpeechRecognitionListener();

            listener.ReadyForSpeech = () => this.ListenSubject.OnNext(true);
            listener.SpeechDetected = sentence =>
            {
                lock (syncLock)
                    ob.OnNext(sentence);
            };
            //listener.EndOfSpeech = () =>
            //{
            //    lock (syncLock)
            //    {
            //        if (completeOnEndOfSpeech)
            //        {
            //            stop = true;
            //            ob.OnCompleted();
            //        }
            //        else
            //        {
            //            speechRecognizer.Destroy();
            //            speechRecognizer = null;

            //            speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
            //            speechRecognizer.SetRecognitionListener(listener);
            //            speechRecognizer.StartListening(this.CreateSpeechIntent());
            //        }
            //    }
            //};
            listener.Error = ex =>
            {
                switch (ex)
                {
                    case SpeechRecognizerError.Client:
                    case SpeechRecognizerError.RecognizerBusy:
                    case SpeechRecognizerError.SpeechTimeout:
                        if (stop)
                            return;

                        speechRecognizer.Destroy();
                        speechRecognizer = null;

                        speechRecognizer = SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                        speechRecognizer.SetRecognitionListener(listener);
                        speechRecognizer.StartListening(this.CreateSpeechIntent());
                        break;

                        //        default:
                        //            ob.OnError(new Exception($"Could not start speech recognizer - ERROR: {ex}"));
                        //            break;
                }
            };

            speechRecognizer.SetRecognitionListener(listener);
            speechRecognizer.StartListening(this.CreateSpeechIntent());

            return () =>
            {
                this.ListenSubject.OnNext(false);
                listener.Error = null;
                speechRecognizer?.Destroy();
            };
        });


        protected virtual Intent CreateSpeechIntent()
        {
            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, Java.Util.Locale.Default);
            intent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);

            return intent;
        }
    }
}