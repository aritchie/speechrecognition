using System;
using System.Threading.Tasks;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech.Abstractions;
using System.Reactive.Linq;
using System.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using Acr.UserDialogs;


namespace Acr.SpeechDialogs.Impl
{
    public class SpeechDialogsImpl : ISpeechDialogs
    {
        readonly ISpeechRecognizer speech;
        readonly ITextToSpeech tts;
        readonly IUserDialogs dialogs;


        public SpeechDialogsImpl(ISpeechRecognizer sr, ITextToSpeech tts, IUserDialogs dialogs)
        {
            this.speech = sr;
            this.tts = tts;
            this.dialogs = dialogs;
        }


        public async void Actions(ActionsConfig config)
        {
            IDisposable dialog = null;
            IDisposable speech = null;
            var cancelSrc = new CancellationTokenSource();

            if (config.ShowDialog)
            {
                var dialogCfg = new ActionSheetConfig
                {
                    Title = config.Question
                };
                foreach (var choice in config.Choices)
                {
                    dialogCfg.Add(choice.Key, () =>
                    {
                        cancelSrc.Cancel();
                        speech.Dispose();
                        choice.Value?.Invoke();
                    });
                }
                dialog = this.dialogs.ActionSheet(dialogCfg);
            }

            // TODO: register for speech before TTS?  TTS may trigger this!
            this.speech
                .Listen()
                .Where(x => config.Choices.Keys.Any(y => y.Equals(x, StringComparison.CurrentCultureIgnoreCase)))
                .Take(1)
                .Subscribe(x =>
                {
                    dialog?.Dispose();
                    cancelSrc.Cancel();
                    config.Choices[x]?.Invoke();
                });


            await this.tts.Speak(config.Question, cancelSrc.Token);
            if (config.SpeakChoices)
            {
                foreach (var key in config.Choices.Keys)
                {
                    if (!cancelSrc.IsCancellationRequested)
                        await this.tts.Speak(key, cancelSrc.Token);
                }
            }
        }


        public async Task<bool> Confirm(string question, string positive, string negative, bool showDialog, CancellationToken? cancelToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            IDisposable dialog = null;
            IDisposable speech = null;
            this.tts.Speak(question);

            if (showDialog)
            {
                this.dialogs.Confirm(new ConfirmConfig
                {
                    Message = question,
                    OkText = positive,
                    CancelText = negative,
                    OnAction = dr =>
                    {
                        tcs.TrySetResult(dr);
                        speech.Dispose();
                    }
                });
            }

            speech = this.speech
                .Listen()
                .Where(x =>
                    x.Equals(positive, StringComparison.CurrentCultureIgnoreCase) ||
                    x.Equals(negative, StringComparison.CurrentCultureIgnoreCase)
                )
                .Take(1)
                .Subscribe(text =>
                {
                    var r = text.Equals(positive, StringComparison.CurrentCultureIgnoreCase);
                    dialog?.Dispose();
                    tcs.TrySetResult(r);
                });

            cancelToken?.Register(() =>
            {
                dialog?.Dispose();
                speech.Dispose();
                tcs.TrySetCanceled();
            });

            return await tcs.Task;
        }


        public async Task<string> Prompt(string question, CancellationToken? cancelToken)
        {
            this.tts.Speak(question);
            var result = await this.speech.Listen(true).ToTask(cancelToken ?? CancellationToken.None);
            return result;
        }
    }
}
