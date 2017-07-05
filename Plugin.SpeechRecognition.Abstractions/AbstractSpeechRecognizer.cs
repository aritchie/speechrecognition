using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Xml;


namespace Plugin.SpeechRecognition
{
    public abstract class AbstractSpeechRecognizer : ISpeechRecognizer
    {
        protected Subject<bool> ListenSubject { get; } = new Subject<bool>();


        public IObservable<bool> WhenListeningStatusChanged() => this.ListenSubject;
        public virtual bool IsSupported { get; protected set; }
        public virtual IObservable<string> ListenForFirstKeyword(params string[] keywords) => this.ContinuousDictation()
            .Where(x => keywords.Any(y => y.Equals(x, StringComparison.OrdinalIgnoreCase)))
            .Take(1);

        public abstract IObservable<string> ListenUntilPause();
        public abstract IObservable<string> ContinuousDictation();
        public abstract IObservable<SpeechRecognizerStatus> RequestPermission();
    }
}
