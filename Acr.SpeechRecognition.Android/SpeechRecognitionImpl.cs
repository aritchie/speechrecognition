using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Speech;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : Java.Lang.Object, ISpeechRecognizer, IRecognitionListener
    {
        TaskCompletionSource<string> tcs;


        public Task<string> Listen(CancellationToken? cancelToken = null)
        {
            cancelToken?.Register(() =>
            {
                try
                {
                    this.tcs?.TrySetCanceled();
                }
                catch
                {
                }
            });
            var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);

            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            intent.PutExtra(RecognizerIntent.ExtraCallingPackage, Application.Context.PackageName);
            //intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.VoiceCommandsDesc));
            intent.PutExtra(RecognizerIntent.ExtraLanguageModel, Application.Context.Resources.Configuration.Locale.Language);
            //intent.PutExtra(RecognizerIntent.ExtraMaxResults, 5);
            speechRecognizer.SetRecognitionListener(this);
            speechRecognizer.StartListening(intent);

            return this.tcs.Task;
        }



        public void OnBeginningOfSpeech()
        {
        }


        public void OnBufferReceived(byte[] buffer)
        {
        }


        public void OnEndOfSpeech()
        {
        }


        public void OnError([GeneratedEnum] SpeechRecognizerError error)
        {
        }


        public void OnEvent(int eventType, Bundle @params)
        {
        }


        public void OnPartialResults(Bundle partialResults)
        {
        }


        public void OnReadyForSpeech(Bundle @params)
        {
        }

        public void OnResults(Bundle results)
        {
            var matches = results.GetStringArrayList(Android.Speech.SpeechRecognizer.ResultsRecognition);
            if (matches != null)
                this.tcs?.TrySetResult(String.Join(" ", matches));
        }


        public void OnRmsChanged(float rmsdB)
        {
        }
    }
}
/*
 string rec = Android.Content.PM.PackageManager.FeatureMicrophone;
if (rec != "android.hardware.microphone")
     */