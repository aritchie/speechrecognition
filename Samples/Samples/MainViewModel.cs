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
                    await this.SetTextAndSpeak("Hey Dummy!  Ya you!  You didn't enable permissions for the microphone", 4000);
                    return;
                }
                var lol = 0;
                await this.SetTextAndSpeak("Hello, please tell me your name?", 3000);

                this.IsListening = true;
                var answer = await speech.Listen();
                this.IsListening = false;

                switch (answer.ToLower())
                {
                    case "alan":
                    case "allan":
                        await this.SetTextAndSpeak("Hello Master. I am here to serve you.  Please let me speak with Chris.", 4000);
                        break;

                    case "jason":
                        await this.SetTextAndSpeak("Jason. Hopefully you had your morning coffee before this", 4000);
                        break;

                    case "james":
                        await this.SetTextAndSpeak("James. You look like you need a beer and you need to acknowledge that this is seriously cool shit", 5000);
                        break;

                    case "osama":
                        await this.SetTextAndSpeak("Osama.  You are a pimp", 3000);
                        break;

                    case "darren":
                        await this.SetTextAndSpeak("Darren.  Burrito or sandwitch today?", 3000);
                        break;

                    case "chris":
                        await this.SetTextAndSpeak("MOTHER FUCKER.  THIS IS OPEN SOURCE MISTER COMPANY MAN", 4000);
                        lol = 3;
                        break;

                    case "afshin":
                    case "anthony":
                        await this.SetTextAndSpeak("JAW KESH", 1000);
                        break;

                    case "on duty":
                        await this.SetTextAndSpeak("You said duty", 2000);
                        lol = 1;
                        break;

                    default:
                        await this.SetTextAndSpeak($"Hello {answer}", 2000);
                        break;
                }

                if (lol > 0)
                {
                    for (var i = 0; i < lol; i++)
                    {
                        tts.Speak("HA HA HA HA HA HA");
                        await Task.Delay(2000);
                    }
                }
            });
        }


        async Task SetTextAndSpeak(string text, int wait)
        {
            this.Text = text;
            this.tts.Speak(text);
            await Task.Delay(wait);
        }


        public ICommand Start { get; }
        public string Text { get; set; }
        public bool IsListening { get; set; }
    }
}
