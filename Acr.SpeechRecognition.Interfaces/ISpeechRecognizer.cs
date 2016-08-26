using System;
using System.Threading;
using System.Threading.Tasks;


namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        Task<string> Listen(CancellationToken? cancelToken = null);
    }
}
