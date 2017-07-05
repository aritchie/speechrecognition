using System;


namespace Plugin.SpeechRecognition
{
    public static class CrossSpeechRecognition
    {
        static ISpeechRecognizer current;
        public static ISpeechRecognizer Current
        {
            get
            {
#if BAIT
                if (current == null)
                    throw new ArgumentException("[Plugin.SpeechRecognition] No platform plugin found.  Did you install the nuget package in your app project as well?");
#else
                current = current ?? new SpeechRecognizerImpl();
#endif
                return current;
            }
            set => current = value;
        }
    }
}
