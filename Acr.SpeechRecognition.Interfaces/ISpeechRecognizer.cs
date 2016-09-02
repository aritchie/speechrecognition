using System;


namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        IObservable<string> Listen();
        bool IsSupported { get; }
    }
}
