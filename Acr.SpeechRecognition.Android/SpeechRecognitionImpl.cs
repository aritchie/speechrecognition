using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Speech;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


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
            var results = await this.permissions.RequestPermissionsAsync(Permission.Microphone);
            return results[Permission.Microphone] == PermissionStatus.Granted;
        }


        IObservable<string> listenOb;
        public IObservable<string> Listen()
        {
            this.listenOb = this.listenOb ?? Observable.Create<string>(ob =>
            {
                var handler = new EventHandler<string>((sender, args) => ob.OnNext(args));
                var listener = new SpeechRecognitionListener();
                listener.SpeechDetected += handler;

                var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                //var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(GetTopActivity());
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
            })
            .Publish()
            .RefCount();

            return this.listenOb;
        }


        public bool IsSupported => Android.OS.Build.VERSION.SdkInt >= BuildVersionCodes.M;
    }
}