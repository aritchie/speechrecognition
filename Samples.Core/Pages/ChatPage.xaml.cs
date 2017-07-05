using System;
using Acr.SpeechDialogs;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Pages
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            this.InitializeComponent();
            this.BindingContext = new ChatViewModel(
                CrossTextToSpeech.Current,
                SpeechRecognizer.Instance,
                SpeechDialogs.Instance
            );
        }
    }
}
