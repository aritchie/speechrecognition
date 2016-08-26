using System;
using Xamarin.Forms;


namespace Samples
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }


        async void btnListen_OnClicked(object sender, EventArgs e)
        {
            btnListen.IsEnabled = false;
            try
            {
                var result = await SpeechRecognizer.Listen();
                this.lblValue.Text = result;
                this.lblValue.TextColor = Color.Green;
            }
            catch (Exception ex)
            {
                this.lblValue.Text = ex.ToString();
                this.lblValue.TextColor = Color.Red;
            }
            Device.BeginInvokeOnMainThread(() => btnListen.IsEnabled = true);
        }
    }
}
