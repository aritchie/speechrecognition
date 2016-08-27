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
        public Task<string> Listen(CancellationToken? cancelToken = null)
        {
            var speechRecognizer = Android.Speech.SpeechRecognizer.CreateSpeechRecognizer(Application.Context);

            var intent = new Intent(RecognizerIntent.ActionRecognizeSpeech);
            //intent.PutExtra(RecognizerIntent.ExtraCallingPackage, PackageName);
            //intent.PutExtra(RecognizerIntent.ExtraPrompt, GetString(Resource.String.VoiceCommandsDesc));
            //intent.PutExtra(RecognizerIntent.ExtraLanguageModel, this.Resources.Configuration.Locale.Language);
            //intent.PutExtra(RecognizerIntent.ExtraMaxResults, 5);
            speechRecognizer.SetRecognitionListener(this);
            speechRecognizer.StartListening(intent);

            return null;
        }



        public void OnBeginningOfSpeech()
        {
        }


        public void OnBufferReceived(byte[] buffer)
        {
        }


        public void OnEndOfSpeech()
        {
            // COMPLETE TASK HERE
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
            // TODO: complete here?
            //var matches = results.GetStringArrayList(SpeechRecognizer.ResultsRecognition);
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