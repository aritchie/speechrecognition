using System;
using System.Collections.Generic;
using Android.OS;
using Android.Speech;
using Debug = System.Diagnostics.Debug;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognitionListener : Java.Lang.Object, IRecognitionListener
    {
        public Action StartOfSpeech { get; set; }
        public Action EndOfSpeech { get; set; }
        public Action ReadyForSpeech { get; set; }
        public Action<SpeechRecognizerError> Error { get; set; }
        public Action<IList<string>> SpeechDetected { get; set; }
        public Action<float> RmsChanged { get; set; }


        public void OnBeginningOfSpeech()
        {
            Debug.WriteLine("Beginning of Speech");
            this.StartOfSpeech?.Invoke();
        }


        public void OnBufferReceived(byte[] buffer)
        {
            Debug.WriteLine("Buffer Received");
        }


        public void OnEndOfSpeech()
        {
            Debug.WriteLine("End of Speech");
            this.EndOfSpeech?.Invoke();
        }


        public void OnError(SpeechRecognizerError error)
        {
            Debug.WriteLine("Error: " + error);
            this.Error?.Invoke(error);
        }


        public void OnEvent(int eventType, Bundle @params)
        {
            Debug.WriteLine("OnEvent: " + eventType);
        }


        public void OnPartialResults(Bundle partialResults)
        {
            Debug.WriteLine("OnPartialResults");
            this.SendResults(partialResults);
        }


        public void OnReadyForSpeech(Bundle @params)
        {
            Debug.WriteLine("Ready for Speech");
            this.ReadyForSpeech?.Invoke();
        }


        public void OnResults(Bundle results)
        {
            Debug.WriteLine("Speech Results");
            this.SendResults(results);
        }


        public void OnRmsChanged(float rmsdB)
        {
            Debug.WriteLine("RMS Changed: " + rmsdB);
            this.RmsChanged?.Invoke(rmsdB);
        }


        void SendResults(Bundle bundle)
        {
            var matches = bundle.GetStringArrayList(Android.Speech.SpeechRecognizer.ResultsRecognition);
            if (matches == null)
            {
                Debug.WriteLine("Matches value is null in bundle");
            }
            else
            {
                Debug.WriteLine("Matches found: " + matches.Count);
                this.SpeechDetected?.Invoke(matches);
            }
        }
    }
}