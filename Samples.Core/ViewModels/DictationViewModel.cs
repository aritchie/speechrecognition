using System;
using System.Windows.Input;
using Plugin.SpeechRecognition;
using ReactiveUI;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DictationViewModel : ReactiveObject
    {
        public DictationViewModel()
        {
            var speech = CrossSpeechRecognition.Current;

            IDisposable token = null;
            speech
                .WhenListeningStatusChanged()
                .Subscribe(x => this.ListenText = x
                    ? "Stop Listening"
                    : "Start Dictation"
                );

            this.ToggleListen = new Command(() =>
            {
                if (token == null)
                {
                    token = speech
                        .ContinuousDictation()
                        //.Catch<string, Exception>(ex => Observable.Return(ex.ToString()))
                        .Subscribe(x => this.Text += " " + x);
                }
                else
                {
                    token.Dispose();
                    token = null;
                }
            });
        }


        public ICommand ToggleListen { get; }


        string listenText = "Start Dictation";
        public string ListenText
        {
            get => this.listenText;
            set => this.RaiseAndSetIfChanged(ref this.listenText, value);
        }


        string text;
        public string Text
        {
            get => this.text;
            set => this.RaiseAndSetIfChanged(ref this.text, value);
        }
    }
}
