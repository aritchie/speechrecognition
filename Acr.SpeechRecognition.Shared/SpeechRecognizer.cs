using System;


namespace Acr.SpeechRecognition
{
    public static class SpeechRecognizer
    {
        static readonly Lazy<ISpeechRecognizer> instanceInit = new Lazy<ISpeechRecognizer>(() =>
        {
#if PCL
            throw new ArgumentException("[Acr.SpeechRecognition] No platform plugin found.  Did you install the nuget package in your app project as well?");
#else
            return new SpeechRecognizerImpl();
#endif
        }, false);


        static ISpeechRecognizer customInstance;
        public static ISpeechRecognizer Instance
        {
            get { return customInstance ?? instanceInit.Value; }
            set { customInstance = value; }
        }
    }
}
