using System;
using Acr.SpeechDialogs.Impl;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech;


namespace Acr.SpeechDialogs
{
    public static class SpeechDialogs
    {
        static readonly Lazy<ISpeechDialogs> instanceLazy = new Lazy<ISpeechDialogs>(() =>
            new SpeechDialogsImpl(
                SpeechRecognizer.Instance,
                CrossTextToSpeech.Current,
                Acr.UserDialogs.UserDialogs.Instance
            )
        );


        static ISpeechDialogs instance;
        public static ISpeechDialogs Instance
        {
            get { return instance ?? instanceLazy.Value; }
            set { instance = value; }
        }
    }
}
