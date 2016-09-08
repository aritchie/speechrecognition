using System;
using System.Threading.Tasks;

namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        Task<bool> RequestPermission();
        IObservable<string> Listen();
        bool IsSupported { get; }
    }
}
