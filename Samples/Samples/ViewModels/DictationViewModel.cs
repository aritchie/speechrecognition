using System;
using System.Windows.Input;
using Acr.SpeechRecognition;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    [ImplementPropertyChanged]
    public class DictationViewModel
    {
        public DictationViewModel(ISpeechRecognizer speech)
        {
            IDisposable token = null;

            this.ToggleListen = new Command(() =>
            {
                if (token == null)
                {
                    this.ListenText = "Stop Dictation";
                    token = speech
                        .Listen()
                        .Subscribe(x => this.Text += x);
                }
                else
                {
                    this.ListenText = "Start Dictation";
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
