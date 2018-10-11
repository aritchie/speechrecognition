using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace Plugin.SpeechRecognition
{
    public abstract class AbstractSpeechRecognizer : ISpeechRecognizer
    {
        protected Subject<bool> ListenSubject { get; } = new Subject<bool>();


        public IObservable<bool> WhenListeningStatusChanged() => this.ListenSubject;
        public virtual bool IsSupported { get; protected set; }


        public virtual IObservable<string> ListenForFirstKeyword(params string[] keywords)
            => Observable.Create<string>(ob => this.ContinuousDictation().Subscribe(x =>
            {
                var values = x.Split(' ');
                foreach (var value in values)
                {
                    foreach (var keyword in keywords)
                    {
                        if (value.Equals(keyword, StringComparison.OrdinalIgnoreCase))
                        {
                            ob.OnNext(value);
                            ob.OnCompleted();
                            break;
                        }
                    }
                }
            }));

        public abstract IObservable<string> ListenUntilPause();
        public abstract IObservable<string> ContinuousDictation();
        public abstract IObservable<SpeechRecognizerStatus> RequestPermission();
    }
}
