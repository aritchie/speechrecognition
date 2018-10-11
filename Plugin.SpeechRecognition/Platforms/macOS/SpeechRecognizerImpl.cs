using System;
using System.Reactive.Linq;
using AppKit;
using AVFoundation;
using Foundation;


namespace Plugin.SpeechRecognition
{
    public class SpeechRecognizerImpl : AbstractSpeechRecognizer
    {


        public override bool IsSupported => true;
        public override IObservable<string> ListenUntilPause() => this.Listen(true);
        public override IObservable<string> ContinuousDictation() => this.Listen(false);


        public override IObservable<SpeechRecognizerStatus> RequestPermission() => Observable.Return(SpeechRecognizerStatus.Available);


        protected virtual IObservable<string> Listen(bool completeOnEndOfSpeech) => Observable.Create<string>(ob =>
        {
            NSError error = null;

            var audioEngine = new AVAudioEngine();
            var speechRecognizer = new NSSpeechRecognizer();
            speechRecognizer.Delegate = new NSSpeechRecognizerDelegate();
            return () =>
            {
            };
        });
    }
}