using System;
using Samples.ViewModels;
using Xamarin.Forms;


namespace Samples.Pages
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage()
        {
            this.InitializeComponent();
            this.BindingContext = new ChatViewModel();
        }
    }
}
