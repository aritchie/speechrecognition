using System;
using Android.OS;
using Android.Speech;
using Debug = System.Diagnostics.Debug;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognitionListener : Java.Lang.Object, IRecognitionListener
    {
        public event EventHandler<string> SpeechDetected;


        public void OnBeginningOfSpeech()
        {
            Debug.WriteLine("Beginning of Speech");
        }


        public void OnBufferReceived(byte[] buffer)
        {
        }


        public void OnEndOfSpeech()
        {
            Debug.WriteLine("End of Speech");
        }


        public void OnError(SpeechRecognizerError error)
        {
            Debug.WriteLine("Error: " + error);
        }


        public void OnEvent(int eventType, Bundle @params)
        {
        }


        public void OnPartialResults(Bundle partialResults)
        {
        }


        public void OnReadyForSpeech(Bundle @params)
        {
            Debug.WriteLine("Ready for Speech");
        }


        public void OnResults(Bundle results)
        {
            Debug.WriteLine("Speech Results");
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