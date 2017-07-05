using System;
using System.Reactive.Linq;
using Windows.Foundation;
using Windows.Media.SpeechRecognition;
using WinSpeechRecognizer = Windows.Media.SpeechRecognition.SpeechRecognizer;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {
        public SpeechRecognizerImpl(IPermissions permissions = null) : base(permissions)
        {
        }


        protected override bool IsSupported => true;
        public override IObservable<string> ListenUntilPause()
        {
            return Observable.Create<string>(async ob =>
            {
                var speech = new WinSpeechRecognizer();
                await speech.CompileConstraintsAsync();
                this.ListenSubject.OnNext(true);

                var handler = new TypedEventHandler<SpeechContinuousRecognitionSession, SpeechContinuousRecognitionResultGeneratedEventArgs>((sender, args) =>
                {
                    var words = args.Result.Text.Split(' ');
                    foreach (var word in words)
                        ob.OnNext(word);
                });
                speech.ContinuousRecognitionSession.ResultGenerated += handler;
                await speech.ContinuousRecognitionSession.StartAsync();

                return () =>
                {
                    speech.ContinuousRecognitionSession.StopAsync();
                    speech.ContinuousRecognitionSession.ResultGenerated -= handler;
                    this.ListenSubject.OnNext(false);
                    speech.Dispose();
                };
            });
        }


        public override IObservable<string> ContinuousDictation()
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

                return () =>
                {
                    this.ListenSubject.OnNext(false);
                    speech.Dispose();
                };
            });
        }
        //        //if (showUI)
        //        //{
        //        //    var grammar = new SpeechRecognitionTopicConstraint(SpeechRecognitionScenario.Dictation, "webSearch");
        //        //    speech.UIOptions.AudiblePrompt = "Say what you want to search for...";
        //        //    speech.UIOptions.ExampleText = @"Ex. &#39;weather for London&#39;";
        //        //    speech.Constraints.Add(webSearchGrammar);
        //        //}
        //        //speech.ContinuousRecognitionSession.AutoStopSilenceTimeout = TimeSpan.FromDays(1)
    }
}