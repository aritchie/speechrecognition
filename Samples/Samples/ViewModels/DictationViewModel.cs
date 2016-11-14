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
            speech
                .WhenListeningStatusChanged()
                .Subscribe(x => this.ListenText = x
                    ? "Stop Listening"
                    : "Start Dictation"
                );

            this.ToggleListen = new Command(async () =>
            {
                if (speech.Status != SpeechRecognizerStatus.Available)
                {
                    this.ListenText = "Problem with speech recognition engine - " + speech.Status;
                    return;
                }

                var granted = await speech.RequestPermission();
                if (!granted)
                {
                    this.ListenText = "Invalid Permissions";
                    return;
                }
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
