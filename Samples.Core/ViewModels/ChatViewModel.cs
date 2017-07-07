using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Plugin.SpeechDialogs;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using ReactiveUI;


namespace Samples.ViewModels
{
    public class ChatViewModel : ReactiveObject
    {
        public ChatViewModel()
        {
            var tts = CrossTextToSpeech.Current;
            var speech = CrossSpeechRecognition.Current;
            var dialogs = new SpeechDialogs();

            speech.WhenListeningStatusChanged().Subscribe(x => this.IsListening = x);

            this.Start = ReactiveCommand.CreateFromTask(async () =>
            {
                var answer = await dialogs.Question("Hello, please tell me your name?");
                await tts.Speak($"Hello {answer}");
            });
        }


        public ICommand Start { get; }


        string text;
        public string Text
        {
            get => this.text;
            set => this.RaiseAndSetIfChanged(ref this.text, value);
        }


        bool listening;
        public bool IsListening
        {
            get => this.listening;
            set => this.RaiseAndSetIfChanged(ref this.listening, value);
        }
    }
}
