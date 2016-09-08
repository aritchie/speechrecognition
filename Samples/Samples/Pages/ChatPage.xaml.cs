using System;
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
            InitializeComponent();
            this.BindingContext = new ChatViewModel(
                CrossTextToSpeech.Current,
                SpeechRecognizer.Instance
            );
        }
    }
}
