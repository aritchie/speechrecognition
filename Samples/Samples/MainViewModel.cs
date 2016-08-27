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
        public MainViewModel(IPermissions permissions, ITextToSpeech tts, ISpeechRecognizer speech)
        {
            this.Start = new Command(async () =>
            {
                var status = await permissions.RequestPermissionsAsync(Permission.Microphone);
                if (status[Permission.Microphone] != PermissionStatus.Granted)
                {
                    this.Answer = "Hey Dummy!  Ya you!  You didn't enable permissions for the microphone";
                    tts.Speak(this.Answer);
                    return;
                }
                var lol = false;
                this.IsAnswerVisible = false;
                tts.Speak("Hello, please tell me your name?");
                var answer = await speech.Listen();

                switch (answer.ToLower())
                {
                    case "allan":
                        this.Answer = "Hello Master. I am here to serve you.  Please let me speak with Chris.";
                        break;

                    case "jason":
                        this.Answer = "Jason.  Allan really wants to know who botched the BLE MGS service";
                        break;

                    case "james":
                        this.Answer = "James. You look like you need a beer and you need to acknowledge that this is seriously cool shit";
                        break;

                    case "chris":
                        this.Answer = "MOTHER FUCKER.  THIS IS OPEN SOURCE MISTER COMPANY MAN";
                        lol = true;
                        break;

                    default:
                        this.Answer = $"Hello {answer}";
                        break;
                }

                tts.Speak(this.Answer);
                if (lol)
                {
                    await Task.Delay(2000);
                    for (var i = 0; i < 3; i++)
                    {
                        tts.Speak("HA HA HA HA HA HA");
                        await Task.Delay(2000);
                    }
                }
                this.IsAnswerVisible = true;
            });
        }


        public ICommand Start { get; }
        public bool IsAnswerVisible { get; set; }
        public string Answer { get; set; }
    }
}
