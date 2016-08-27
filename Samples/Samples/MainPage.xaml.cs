using System;
using Acr.SpeechRecognition;
using Plugin.Permissions;
using Plugin.TextToSpeech;
using Xamarin.Forms;


namespace Samples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            this.BindingContext = new MainViewModel(
                CrossPermissions.Current,
                CrossTextToSpeech.Current,
                SpeechRecognizer.Instance
            );
        }
    }
}
