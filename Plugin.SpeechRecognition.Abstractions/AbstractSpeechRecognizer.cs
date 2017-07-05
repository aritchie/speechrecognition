using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;


namespace Plugin.SpeechRecognition
{
    public abstract class AbstractSpeechRecognizer : ISpeechRecognizer
    {
        protected Subject<bool> ListenSubject { get; } = new Subject<bool>();
        protected abstract bool IsSupported { get; }
        public abstract SpeechRecognizerStatus Status { get; }

        public IObservable<bool> WhenListeningStatusChanged() => this.ListenSubject;
        public abstract IObservable<string> ListenUntilPause();
        public abstract IObservable<string> ContinuousDictation();
        public abstract IObservable<bool> RequestPermission();


        public virtual IObservable<string> ListenForFirstKeyword(params string[] keywords)
            => Observable.Create<string>(ob =>
                this.ContinuousDictation()
                    .Subscribe(word =>
                    {
                        if (keywords.Any(x => x.Equals(word, StringComparison.CurrentCultureIgnoreCase)))
                        {
                            ob.OnNext(word);
                            ob.OnCompleted();
                        }
                    })
            );


        //public virtual async Task<bool> RequestPermission()
        //{
        //    var result = await this.permissions.RequestPermissionsAsync(Permission.Speech);
        //    return result[Permission.Speech] == PermissionStatus.Granted;
        //}


        //public virtual SpeechRecognizerStatus Status
        //{
        //    get
        //    {
        //        if (!this.IsSupported)
        //            return SpeechRecognizerStatus.NotSupported;

        //        var status = this.permissions.CheckPermissionStatusAsync(Permission.Speech).Result;
        //        switch (status)
        //        {
        //            case PermissionStatus.Disabled:
        //                return SpeechRecognizerStatus.Disabled;

        //            case PermissionStatus.Denied:
        //                return SpeechRecognizerStatus.PermissionDenied;

        //            case PermissionStatus.Granted:
        //                return SpeechRecognizerStatus.Available;

        //            default:
        //                return SpeechRecognizerStatus.NotSupported;
        //        }
        //    }
        //}
    }
}
