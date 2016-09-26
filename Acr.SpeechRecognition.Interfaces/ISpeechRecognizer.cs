using System;
using System.Threading.Tasks;

namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        bool IsSupported { get; }
        Task<bool> RequestPermission();
        IObservable<string> Dictate();
        IObservable<string> ReceiveCommand();
    }
}
