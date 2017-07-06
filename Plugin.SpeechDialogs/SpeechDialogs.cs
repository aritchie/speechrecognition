using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;


namespace Plugin.SpeechDialogs
{
    public class SpeechDialogs : ISpeechDialogs
    {
        readonly ITextToSpeech tts = CrossTextToSpeech.Current;
        readonly ISpeechRecognizer speech = CrossSpeechRecognition.Current;
        readonly IUserDialogs dialogs = UserDialogs.Instance;


        public IObservable<string> Choices(string question, bool speakChoices = true, params string[] choices) => Observable.FromAsync(async ct =>
        {

            await this.tts.Speak(question, cancelToken: ct);

            return String.Empty;
        });


        public IObservable<bool> Confirm(string question, string yes = "yes", string no = "no") => Observable.FromAsync(async ct =>
        {
            this.tts.Speak(question, cancelToken: ct);
            var confirmTask = this.dialogs.ConfirmAsync(question, null, yes, no, ct);
            var speechTask = this.speech.ListenForFirstKeyword(yes, no).ToTask(ct);

            var finishTask = await Task.WhenAny(confirmTask, speechTask);
            if (ct.IsCancellationRequested)
                return false;

            if (finishTask == confirmTask)
                return confirmTask.Result;

            if (finishTask == speechTask)
                return speechTask.Result.Equals(yes, StringComparison.OrdinalIgnoreCase);

            return false;
        });


        public IObservable<string> Question(string question)
        {
            //this.speech.ListenForFirstKeyword(yes, no).Subscribe(x => )
            //await this.tts.Speak(question, cancelToken: ct);
            //return true;
            return Observable.Return(String.Empty);
        }
    }
}
