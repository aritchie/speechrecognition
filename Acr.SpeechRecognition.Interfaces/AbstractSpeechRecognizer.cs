using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;


namespace Acr.SpeechRecognition
{
    public abstract class AbstractSpeechRecognizer : ISpeechRecognizer
    {
        readonly IPermissions permissions;


        protected AbstractSpeechRecognizer(IPermissions permissions)
        {
            this.permissions = permissions ?? CrossPermissions.Current;
        }


        protected Subject<bool> ListenSubject { get; } = new Subject<bool>();
        protected abstract bool IsSupported { get; }

        public abstract IObservable<string> ListenUntilPause();
        public abstract IObservable<string> ContinuousDictation();

        public virtual IObservable<string> ListenForFirstKeyword(params string[] keywords)
        {
            return Observable.Create<string>(ob =>
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
        }


        public virtual async Task<bool> RequestPermission()
        {
            var result = await this.permissions.RequestPermissionsAsync(Permission.Speech);
            return result[Permission.Speech] == PermissionStatus.Granted;
        }


        public virtual SpeechRecognizerStatus Status
        {
            get
            {
                if (!this.IsSupported)
                    return SpeechRecognizerStatus.NotSupported;

                var status = this.permissions.CheckPermissionStatusAsync(Permission.Speech).Result;
                switch (status)
                {
                    case PermissionStatus.Disabled:
                        return SpeechRecognizerStatus.Disabled;

                    case PermissionStatus.Denied:
                        return SpeechRecognizerStatus.PermissionDenied;

                    case PermissionStatus.Granted:
                        return SpeechRecognizerStatus.Available;

                    default:
                        return SpeechRecognizerStatus.NotSupported;
                }
            }
        }
        public IObservable<bool> WhenListeningStatusChanged()
        {
            return this.ListenSubject;
        }
    }
}
