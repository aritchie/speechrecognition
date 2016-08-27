using System;
using System.Threading;
using System.Threading.Tasks;
using WinSpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public async Task<string> Listen(CancellationToken? cancelToken = null)
        {
            using (var speech = new WinSpeechRecognizer())
            {
                cancelToken?.Register(async () =>
                {
                    try
                    {
                        await speech.StopRecognitionAsync();
                    }
                    catch { }
                });
                //var grammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "webSearch");
                //this.speech.UIOptions.AudiblePrompt = "Say what you want to search for...";
                //this.speech.UIOptions.ExampleText = @"Ex. &#39;weather for London&#39;";
                //this.speech.Constraints.Add(webSearchGrammar);
                await speech.CompileConstraintsAsync();
                var result = await speech.RecognizeAsync();
                //speech.StopRecognitionAsync()
                return result.Text;
            }

            //if (result.Confidence.HasFlag())
            //result.PhraseDuration
            //result.Status == SpeechRecognitionResultStatus.Unknown
        }
    }
}