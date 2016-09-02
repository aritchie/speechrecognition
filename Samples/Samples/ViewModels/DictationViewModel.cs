using System;
using System.Windows.Input;
using Acr.SpeechRecognition;
using Plugin.Permissions.Abstractions;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    [ImplementPropertyChanged]
    public class DictationViewModel
    {
        public DictationViewModel(ISpeechRecognizer speech, IPermissions permissions)
        {
            IDisposable token = null;

            this.ToggleListen = new Command(async () =>
            {
                var status = await permissions.RequestPermissionsAsync(Permission.Microphone);
                if (status[Permission.Microphone] != PermissionStatus.Granted)
                {
                    this.ListenText = "Invalid Permissions";
                    return;
                }
                if (token == null)
                {
                    this.ListenText = "Stop Dictation";
                    token = speech
                        .Listen()
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
