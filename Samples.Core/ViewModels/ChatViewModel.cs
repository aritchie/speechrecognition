using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Plugin.SpeechDialogs;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class ChatViewModel
    {
        public ChatViewModel()
        {
            var tts = CrossTextToSpeech.Current;
            var speech = CrossSpeechRecognition.Current;
            var dialogs = new SpeechDialogs();

            speech.WhenListeningStatusChanged().Subscribe(x => this.IsListening = x);

            this.Start = new Command(async () =>
            {
                if (speech.Status != SpeechRecognizerStatus.Available)
                {
                    await tts.Speak("Problem with speech recognition engine - " + speech.Status);
                    return;
                }

                var granted = await speech.RequestPermission();
                if (!granted)
                {
                    await tts.Speak("Hey Dummy!  Ya you!  You didn't enable permissions for the microphone");
                    return;
                }
                var answer = await dialogs.Question("Hello, please tell me your name?");
                await tts.Speak($"Hello {answer}");
            });
        }


        public ICommand Start { get; }
        public string Text { get; set; }
        public bool IsListening { get; set; }
    }
}
