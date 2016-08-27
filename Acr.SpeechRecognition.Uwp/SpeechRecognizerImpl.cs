using System;
using System.Threading;
using System.Threading.Tasks;
using WinSpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        readonly WinSpeechRecognizer speech = new WinSpeechRecognizer();


        public async Task<string> Listen(CancellationToken? cancelToken = null)
        {
            var result = await this.speech.RecognizeAsync();
            return result.Text;
        }
    }
}