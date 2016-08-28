using System;
using Android.OS;
using Android.Speech;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognitionListener : Java.Lang.Object, IRecognitionListener
    {
        public event EventHandler<string> SpeechDetected;


        public void OnBeginningOfSpeech()
        {
        }


        public void OnBufferReceived(byte[] buffer)
        {
        }


        public void OnEndOfSpeech()
        {
        }


        public void OnError(SpeechRecognizerError error)
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
            {
                var result = String.Join(" ", matches);
                this.SpeechDetected?.Invoke(this, result);
            }
        }


        public void OnRmsChanged(float rmsdB)
        {
        }
    }
}