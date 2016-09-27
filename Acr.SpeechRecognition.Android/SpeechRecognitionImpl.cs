using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
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


        public IObservable<string> Dictate()
        {
            return this.CreateListener(null, (sr, words, ob) => 
            {
                sr.StopListening();
                sr.StartListening(this.CreateSpeechIntent(null));

                foreach (var word in words)
                    ob.OnNext(word);                
            });
        }


        public IObservable<string> Command(int maxWords) 
        {
            return this.CreateListener(maxWords, (sr, words, ob) => 
            {
                var cmd = String.Join(" ", words);
                ob.OnNext(cmd);
                ob.OnCompleted();
            });            
        }


        public bool IsSupported => Android.Speech.SpeechRecognizer.IsRecognitionAvailable(Application.Context);


        IObservable<string> CreateListener(int? maxWords, Action<Android.Speech.SpeechRecognizer, IList<string>, IObserver<string>> action)
        {
            return Observable.Create<string>(ob =>
            {
                var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);
                var listener = new SpeechRecognitionListener();

                listener.SpeechDetected = words => action(speechRecognizer, words, ob);
                speechRecognizer.SetRecognitionListener(listener);
                speechRecognizer.StartListening(this.CreateSpeechIntent(maxWords));

                return () =>
                {
                    listener.Error = null;
                    speechRecognizer.StopListening();
                    speechRecognizer.Dispose();
                };
            });
        }


        Intent CreateSpeechIntent(int? maxWords)
        {
            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraLanguagePreference, "en");
            intent.PutExtra(RecognizerIntent.ExtraLanguage, Java.Util.Locale.Default);
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, RecognizerIntent.LanguageModelFreeForm);
            intent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            intent.PutExtra(RecognizerIntent.ExtraPartialResults, true);
            if (maxWords != null)
                intent.PutExtra(RecognizerIntent.ExtraMaxResults, maxWords.Value);
            
            return intent;
        }
    }
}