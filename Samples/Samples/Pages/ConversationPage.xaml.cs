using System;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Pages
{
    public partial class ConversationPage : ContentPage
    {
        public ConversationPage()
        {
            this.InitializeComponent();
            this.BindingContext = new ConversationViewModel(
                SpeechRecognizer.Instance,
                CrossTextToSpeech.Current
            );
        }
    }
}
