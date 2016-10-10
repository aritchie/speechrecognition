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

            this.ToggleListen = new Command(async () =>
            {
                if (!speech.IsSupported)
                {
                    this.ListenText = "Your current device/OS is not supported";
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
                    this.ListenText = "Stop Dictation";
                    token = speech
                        .Listen()
                        //.Catch<string, Exception>(ex => Observable.Return(ex.ToString()))
                        .Subscribe(x => this.Text += " " + x);
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
