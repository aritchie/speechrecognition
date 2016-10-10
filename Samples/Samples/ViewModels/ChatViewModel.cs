using System;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Threading.Tasks;
using Acr.SpeechDialogs;
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


        public ChatViewModel(ITextToSpeech tts, ISpeechRecognizer speech, ISpeechDialogs dialogs)
        {
            this.tts = tts;

            this.Start = new Command(async () =>
            {
                if (!speech.IsSupported)
                {
                    await tts.Speak("Your current device/OS is not supported", 0);
                    return;
                }

                var granted = await speech.RequestPermission();
                if (!granted)
                {
                    await tts.Speak("Hey Dummy!  Ya you!  You didn't enable permissions for the microphone", 4000);
                    return;
                }
                this.IsListening = true;
                var answer = await dialogs.Prompt("Hello, please tell me your name?");
                this.IsListening = false;
                await tts.Speak($"Hello {answer}");
            });
        }


        public ICommand Start { get; }
        public string Text { get; set; }
        public bool IsListening { get; set; }
    }
}
