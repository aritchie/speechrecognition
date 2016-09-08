using System;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech.Abstractions;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    [ImplementPropertyChanged]
    public class ChatViewModel
    {
        readonly ITextToSpeech tts;


        public ChatViewModel(ITextToSpeech tts, ISpeechRecognizer speech)
        {
            this.tts = tts;

            this.Start = new Command(async () =>
            {
                if (!speech.IsSupported)
                {
                    await this.SetTextAndSpeak("Your current device/OS is not supported", 0);
                    return;
                }
                    
                var granted = await speech.RequestPermission();
                if (!granted)
                {
                    await this.SetTextAndSpeak("Hey Dummy!  Ya you!  You didn't enable permissions for the microphone", 4000);
                    return;
                }
                var lol = 0;
                await this.SetTextAndSpeak("Hello, please tell me your name?", 2500);

                this.IsListening = true;
                var answer = await speech.Listen().Take(1);
                this.IsListening = false;
                await this.SetTextAndSpeak($"Hello {answer}", 2000);
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
