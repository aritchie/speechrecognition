using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Speech;
using Plugin.Permissions.Abstractions;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        public SpeechRecognizerImpl(IPermissions permissions = null) : base(permissions)
        {
        }


        //public override IObservable<string> Listen(bool completeOnEndOfSpeech)
        //{
        //    return Observable.Create<string>(ob =>
        //    {
        //        var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
        //        var listener = new SpeechRecognitionListener
        //        {
        //            ReadyForSpeech = () => this.ListenSubject.OnNext(true),
        //            SpeechDetected = words =>
        //            {
        //                foreach (var word in words)
        //                    ob.OnNext(word);
        //            },
        //            EndOfSpeech = () =>
        //            {
        //                if (completeOnEndOfSpeech)
        //                    ob.OnCompleted();
        //            }
        //        };

        //        listener.Error = _ =>
        //        {
        //            speechRecognizer.StopListening();
        //            speechRecognizer.StartListening(this.CreateSpeechIntent());
        //        };
        //        speechRecognizer.SetRecognitionListener(listener);
        //        speechRecognizer.StartListening(this.CreateSpeechIntent());

        //        return () =>
        //        {
        //            listener.Error = null;
        //            speechRecognizer.StopListening();
        //            speechRecognizer.Dispose();
        //            this.ListenSubject.OnNext(false);
        //        };
        //    });
        //}


        protected override bool IsSupported => Android.Speech.SpeechRecognizer.IsRecognitionAvailable(Application.Context);
        public override IObservable<string> ListenUntilPause()
        {
            throw new NotImplementedException();
        }

        public override IObservable<string> ContinuousDictation()
        {
            throw new NotImplementedException();
        }

        public override IObservable<string> ListenForFirstKeyword(params string[] keywords)
        {
            throw new NotImplementedException();
        }


        Intent CreateSpeechIntent()
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