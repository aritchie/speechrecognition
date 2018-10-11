using System;


namespace Plugin.SpeechRecognition
{
    public static partial class CrossSpeechRecognition
    {
        static ISpeechRecognizer current;
        public static ISpeechRecognizer Current
        {
            get
            {
                if (current == null)
                    throw new ArgumentException("[Plugin.SpeechRecognition] No platform plugin found.  Did you install the nuget package in your app project as well?");

                return current;
            }
            set => current = value;
        }
    }
}
