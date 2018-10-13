using System;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Acr.UserDialogs;
using Plugin.SpeechRecognition;
using Xamarin.Essentials;


namespace Plugin.SpeechDialogs
{
    public class SpeechDialogs : ISpeechDialogs
    {
        readonly ISpeechRecognizer speech = CrossSpeechRecognition.Current;
        readonly IUserDialogs dialogs = UserDialogs.Instance;


        public IObservable<string> Choices(string question, string[] choices, bool speakChoices = false) => Observable.FromAsync(async ct =>
        {
            await TextToSpeech.SpeakAsync(question, ct);
            var dialogTask = this.dialogs.ActionSheetAsync(question, "Cancel", null, ct, choices);
            var speechTask = this.speech
                .ListenForFirstKeyword(choices)
                .ToTask(ct);

            if (speakChoices)
            {
                Task.Run(async () =>
                {
                    var i = 0;
                    while (!ct.IsCancellationRequested && i < choices.Length)
                    {
                        await TextToSpeech.SpeakAsync(choices[i], ct);
                        i++;
                    }
                }, ct);
            }

            var finishTask = await Task.WhenAny(dialogTask, speechTask);
            if (ct.IsCancellationRequested)
                return null;

            if (finishTask == dialogTask)
                return dialogTask.Result;

            if (finishTask == speechTask)
                return speechTask.Result;

            return null;
        });


        public IObservable<bool> Confirm(ConfirmConfig config) => Observable.FromAsync(async ct =>
        {
            TextToSpeech.SpeakAsync(config.Message, ct);
            var confirmTask = this.dialogs.ConfirmAsync(config, ct);
            var speechTask = this.speech
                .ListenForFirstKeyword(config.OkText, config.CancelText)
                .ToTask(ct);

            var finishTask = await Task.WhenAny(confirmTask, speechTask);
            if (ct.IsCancellationRequested)
                return false;

            if (finishTask == confirmTask)
                return confirmTask.Result;

            if (finishTask == speechTask)
                return speechTask.Result.Equals(config.OkText, StringComparison.OrdinalIgnoreCase);

            return false;
        });


        public IObservable<string> Question(PromptConfig config) => Observable.FromAsync<string>(async ct =>
        {
            TextToSpeech.SpeakAsync(config.Message, ct);

            var promptTask = this.dialogs.PromptAsync(config, ct);
            var speechTask = this.speech
                .ListenUntilPause()
                .ToTask(ct);

            var finishTask = await Task.WhenAny(promptTask, speechTask);
            if (ct.IsCancellationRequested)
                return null;

            if (finishTask == promptTask)
                return promptTask.Result.Text;

            if (finishTask == speechTask)
                return speechTask.Result;

            return null;
        });
    }
}
