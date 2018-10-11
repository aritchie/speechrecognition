using System;


namespace Plugin.SpeechRecognition
{
    public static partial class CrossSpeechRecognition
    {
        static CrossSpeechRecognition()
        {
            Current = new SpeechRecognizerImpl();
        }
    }
}
