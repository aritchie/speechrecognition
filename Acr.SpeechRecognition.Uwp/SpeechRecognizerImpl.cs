using System;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using WinSpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        public IObservable<string> Listen()
        {
            return Observable.Create<string>(async ob =>
            {
                var speech = new WinSpeechRecognizer();
                await speech.CompileConstraintsAsync();

                //var grammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "webSearch");
                //this.speech.UIOptions.AudiblePrompt = "Say what you want to search for...";
                //this.speech.UIOptions.ExampleText = @"Ex. &#39;weather for London&#39;";
                //this.speech.Constraints.Add(webSearchGrammar);

                var handler = new TypedEventHandler<SpeechContinuousRecognitionSession, SpeechContinuousRecognitionResultGeneratedEventArgs>(
                    (sender, args) => ob.OnNext(args.Result.Text)
                );

                speech.ContinuousRecognitionSession.ResultGenerated += handler;
                //speech.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(1)
                await speech.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);

                return async () =>
                {
                    await speech.ContinuousRecognitionSession.StopAsync();
                    speech.ContinuousRecognitionSession.ResultGenerated -= handler;
                    speech.Dispose();
                };
            });

            //if (result.Confidence.HasFlag())
            //result.PhraseDuration
            //result.Status == SpeechRecognitionResultStatus.Unknown
        }
    }
}