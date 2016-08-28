using System;
using Acr.SpeechRecognition;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Pages
{
    public partial class DictationPage : ContentPage
    {
        public DictationPage()
        {
            InitializeComponent();
            this.BindingContext = new DictationViewModel(SpeechRecognizer.Instance);
        }
    }
}
