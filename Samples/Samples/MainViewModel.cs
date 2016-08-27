using System;
using System.Windows.Input;
using System.Threading.Tasks;
using Acr.SpeechRecognition;
using Plugin.Permissions.Abstractions;
using Plugin.TextToSpeech.Abstractions;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples
{
    [ImplementPropertyChanged]
    public class MainViewModel
    {
        readonly ITextToSpeech tts;


        public MainViewModel(IPermissions permissions, ITextToSpeech tts, ISpeechRecognizer speech)
        {
            this.tts = tts;

            this.Start = new Command(async () =>
            {
                var status = await permissions.RequestPermissionsAsync(Permission.Microphone);
                if (status[Permission.Microphone] != PermissionStatus.Granted)
                {
                    this.SetTextAndSpeak("Hey Dummy!  Ya you!  You didn't enable permissions for the microphone");
                    return;
                }
                var lol = false;
                this.SetTextAndSpeak("Hello, please tell me your name?");

                this.IsListening = true;
                var answer = await speech.Listen();
                this.IsListening = false;

                switch (answer.ToLower())
                {
                    case "allan":
                        this.SetTextAndSpeak("Hello Master. I am here to serve you.  Please let me speak with Chris.");
                        break;

                    case "jason":
                        this.SetTextAndSpeak("Jason.  Allan really wants to know who botched the BLE MGS service");
                        break;

                    case "james":
                        this.SetTextAndSpeak("James. You look like you need a beer and you need to acknowledge that this is seriously cool shit");
                        break;

                    case "chris":
                        this.SetTextAndSpeak("MOTHER FUCKER.  THIS IS OPEN SOURCE MISTER COMPANY MAN");
                        lol = true;
                        break;

                    default:
                        this.SetTextAndSpeak($"Hello {answer}");
                        break;
                }

                if (lol)
                {
                    await Task.Delay(2000);
                    for (var i = 0; i < 3; i++)
                    {
                        tts.Speak("HA HA HA HA HA HA");
                        await Task.Delay(2000);
                    }
                }
            });
        }


        void SetTextAndSpeak(string text)
        {
            this.Text = text;
            this.tts.Speak(text);
        }


        public ICommand Start { get; }
        public string Text { get; set; }
        public bool IsListening { get; set; }
    }
}
