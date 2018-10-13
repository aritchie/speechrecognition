using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.SpeechRecognition;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DictationViewModel : ReactiveObject
    {
        public DictationViewModel()
        {
            var speech = CrossSpeechRecognition.Current;
            var dialogs = UserDialogs.Instance;

            IDisposable token = null;
            speech
                .WhenListeningStatusChanged()
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => this.ListenText = x
                    ? "Stop Listening"
                    : "Start Dictation"
                );

            this.ToggleListen = ReactiveCommand.Create(()  =>
            {
                if (token == null)
                {
                    if (this.UseContinuous)
                    {
                        token = speech
                            .ContinuousDictation()
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(
                                x => this.Text += " " + x,
                                ex => dialogs.Alert(ex.ToString())
                            );
                    }
                    else
                    {
                        token = speech
                            .ListenUntilPause()
                            .ObserveOn(RxApp.MainThreadScheduler)
                            .Subscribe(
                                x => this.Text = x,
                                ex => dialogs.Alert(ex.ToString())
                            );
                    }
                }
                else
                {
                    token.Dispose();
                    token = null;
                }
            });
        }


        public ICommand ToggleListen { get; }
        [Reactive] public bool UseContinuous { get; set; } = true;
        [Reactive] public string ListenText { get; private set; } = "Start Listening";
        [Reactive] public string Text { get; private set; }
    }
}
