using System;
using System.Threading;
using System.Threading.Tasks;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public Task<string> Listen(CancellationToken? cancelToken = null)
        {
            throw new NotImplementedException();
        }
    }
}
