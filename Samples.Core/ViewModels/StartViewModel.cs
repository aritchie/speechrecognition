using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Acr.UserDialogs;
using Plugin.SpeechRecognition;
using ReactiveUI;
using Samples.Pages;


namespace Samples.ViewModels
{
    public class StartViewModel : ReactiveObject
    {
        readonly ISpeechRecognizer speech = CrossSpeechRecognition.Current;
        readonly IUserDialogs dialogs = UserDialogs.Instance;

        public StartViewModel()
        {
            if (!this.speech.IsSupported)
                this.PermissionStatus = "SPEECH RECOGNITION NOT SUPPORTED";

            this.GotoSamples = ReactiveCommand.CreateFromTask(async () =>
            {
                try
                {
                    await App.Current.MainPage.Navigation.PushAsync(new MainPage());
                }
                catch (Exception ex)
                {
                    this.dialogs.Alert(ex.ToString());
                }
            },
            this.WhenAny(
                x => x.PermissionStatus,
                x => x.Value == SpeechRecognizerStatus.Available.ToString()
            ));

            this.RequestPermission = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!this.speech.IsSupported)
                    return;

                var result = await CrossSpeechRecognition.Current.RequestPermission();
                this.PermissionStatus = result.ToString();
            });
        }


        public ICommand RequestPermission { get;  }
        public ICommand GotoSamples { get; }


        string permissionStatus = "Unknown";
        public string PermissionStatus
        {
            get => this.permissionStatus;
            private set => this.RaiseAndSetIfChanged(ref this.permissionStatus, value);
        }
    }
}
