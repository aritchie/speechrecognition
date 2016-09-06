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
            Debug.WriteLine("Buffer Received");
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
            Debug.WriteLine("OnEvent: " + eventType);
        }


        public void OnPartialResults(Bundle partialResults)
        {
            Debug.WriteLine("OnPartialResults");
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
                foreach (var match in matches)
                    this.SpeechDetected?.Invoke(this, match);

                    //            {
        //if ((results != null)
        //        && results.containsKey(SpeechRecognizer.RESULTS_RECOGNITION))
        //{
        //    List<String> heard =
        //            results.getStringArrayList(SpeechRecognizer.RESULTS_RECOGNITION);
        //    float[] scores =
        //            results.getFloatArray(SpeechRecognizer.CONFIDENCE_SCORES);
        //    receiveWhatWasHeard(heard, scores);
        //}
        }


        public void OnRmsChanged(float rmsdB)
        {
        }
    }
}