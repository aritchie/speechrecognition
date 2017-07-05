using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Plugin.SpeechRecognition;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DictationViewModel
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

            this.ToggleListen = new Command(async () =>
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
        public string ListenText { get; set; } = "Start Dictation";
        public string Text { get; set; }
    }
}
