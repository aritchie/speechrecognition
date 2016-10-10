using System;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using Plugin.Permissions.Abstractions;
using WinSpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        public SpeechRecognizerImpl(IPermissions permissions = null) : base(permissions)
        {
        }


        public override IObservable<string> Listen(bool completeOnEndOfSpeech)
        {
            return completeOnEndOfSpeech
                ? this.QuickPhrase()
                : this.ContinuousListen();
        }


        protected override bool IsSupported => true;


        IObservable<string> QuickPhrase()
        {
            return Observable.Create<string>(async ob =>
            {
                var speech = new WinSpeechRecognizer();
                await speech.CompileConstraintsAsync();
                this.ListenSubject.OnNext(true);
                var result = await speech.RecognizeAsync();
                var words = result.Text.Split(' ');
                foreach (var word in words)
                    ob.OnNext(word);

                ob.OnCompleted();
                return () =>
                {
                    this.ListenSubject.OnNext(false);
                    speech.Dispose();
                };
            });
        }


        IObservable<string> ContinuousListen()
        {
            return Observable.Create<string>(async ob =>
            {
                var speech = new WinSpeechRecognizer();
                await speech.CompileConstraintsAsync();

                //if (showUI)
                //{
                //    var grammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "webSearch");
                //    speech.UIOptions.AudiblePrompt = "Say what you want to search for...";
                //    speech.UIOptions.ExampleText = @"Ex. &#39;weather for London&#39;";
                //    speech.Constraints.Add(webSearchGrammar);
                //}
                var handler = new TypedEventHandler<SpeechContinuousRecognitionSession, SpeechContinuousRecognitionResultGeneratedEventArgs>((sender, args) =>
                {
                    var splits = args.Result.Text.Split(' ');
                    foreach (var split in splits)
                        ob.OnNext(split);
                });
                speech.ContinuousRecognitionSession.ResultGenerated += handler;
                //speech.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(1)
                await speech.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);
                this.ListenSubject.OnNext(true);

                return async () =>
                {
                    await speech.ContinuousRecognitionSession.StopAsync();
                    speech.ContinuousRecognitionSession.ResultGenerated -= handler;
                    speech.Dispose();
                    this.ListenSubject.OnNext(false);
                };
            });
        }
    }
}