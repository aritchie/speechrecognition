using System;
using Acr.SpeechDialogs;
using Plugin.TextToSpeech;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Pages
{
    public partial class DialogsPage : ContentPage
    {
        public DialogsPage()
        {
            this.InitializeComponent();
            this.BindingContext = new DialogsViewModel(SpeechDialogs.Instance, CrossTextToSpeech.Current);
        }
    }
}
