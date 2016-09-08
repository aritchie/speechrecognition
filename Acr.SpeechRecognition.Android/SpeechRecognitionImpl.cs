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
                var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                var listener = new SpeechRecognitionListener();

                listener.SpeechDetected = ob.OnNext;
                listener.Error = _ => 
                {
                    speechRecognizer.StopListening();
                    speechRecognizer.StartListening(this.CreateSpeechIntent());
                };
                speechRecognizer.SetRecognitionListener(listener);
                speechRecognizer.StartListening(this.CreateSpeechIntent());

                return () =>
                {
                    listener.Error = null;
                    speechRecognizer.StopListening();
                    speechRecognizer.Dispose();
                };
            })
            .Publish()
            .RefCount();

            return this.listenOb;
        }


        public bool IsSupported => Android.Speech.SpeechRecognizer.IsRecognitionAvailable(Application.Context);


        Intent CreateSpeechIntent()
        {
            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "en");
            intent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);

            return intent;
        }
    }
}