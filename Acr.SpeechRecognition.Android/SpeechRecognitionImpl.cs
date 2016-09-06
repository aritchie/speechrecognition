using System;
using System.Reactive.Linq;
using Android.App;
using Android.Content;
using Android.Speech;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public static Func<Activity> GetTopActivity { get; set; }
        public IObservable<string> Listen()
        {
            return Observable.Create<string>(ob =>
            {
                var handler = new EventHandler<string>((sender, args) => ob.OnNext(args));
                var listener = new SpeechRecognitionListener();
                listener.SpeechDetected += handler;

                //var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(GetTopActivity());
                speechRecognizer.SetRecognitionListener(listener);

                //RecognizerIntent.GetVoiceDetailsIntent(Application.Context);
                var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
                intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "en");
                intent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
                intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
                intent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);

                intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
                //intent.PutExtra(RecognizerIntent.ExtraLanguageModel, Application.Context.Resources.Configuration.Locale.Language);
                intent.PutExtra(RecognizerIntent.ExtraPrompt, "Testing");
                //intent.PutExtra(RecognizerIntent.ExtraSpeechInputCompleteSilenceLengthMillis, 1500);
                //intent.PutExtra(RecognizerIntent.ExtraSpeechInputPossiblyCompleteSilenceLengthMillis, 1500);
                //intent.PutExtra(RecognizerIntent.ExtraSpeechInputMinimumLengthMillis, 15000);
                intent.PutExtra(RecognizerIntent.ExtraMaxResults, 3);

                speechRecognizer.StartListening(intent); // this may need to be called in speechRecognizer
                return () =>
                {
                    listener.SpeechDetected -= handler;
                    speechRecognizer.StopListening();
                    speechRecognizer.Dispose();
                };
            });
        }


        //public bool IsSupported => Android.OS.Build.VERSION.SdkInt >= 23;
        public bool IsSupported => true;
    }
}