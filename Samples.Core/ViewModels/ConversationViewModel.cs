using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;
using ReactiveUI;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class ConversationViewModel : ReactiveObject
    {
        readonly ISpeechRecognizer speech;
        readonly ITextToSpeech tts;


        public ConversationViewModel()
        {
            this.speech = CrossSpeechRecognition.Current;
            this.tts = CrossTextToSpeech.Current;
            this.Start = ReactiveCommand.CreateFromTask(this.DoConversation);

            this.speech.WhenListeningStatusChanged().Subscribe(x => this.IsListening = x);
        }


        public ICommand Start { get; }
        public ObservableCollection<ListItemViewModel> Items { get; } = new ObservableCollection<ListItemViewModel>();


        bool listening;
        public bool IsListening
        {
            get => this.listening;
            private set => this.RaiseAndSetIfChanged(ref this.listening, value);
        }


        async Task DoConversation()
        {
            this.Items.Clear();

            using (var cancelSrc = new CancellationTokenSource())
            {
                await this.Computer("Please tell me your name");
                var name = await this.speech
                    .ContinuousDictation()
                    .Take(1)
                    .RunAsync(cancelSrc.Token);
                this.Add(name);

                await this.Computer($"Hello {name}.  Are you male or female?");
                var sex = await this.speech.ListenForFirstKeyword("male", "female");
                this.Add(sex);

                var next = sex.Equals("male", StringComparison.CurrentCultureIgnoreCase) ? "Yo dude" : "Sup";
                await this.Computer(next);

                await this.Computer("Tell me something interesting about yourself");
                next = await this.speech.ListenUntilPause();
                this.Add(next);

                await this.Computer("Interesting");
            }
        }


        async Task Computer(string speak)
        {
            this.Add(speak, true);
            await this.tts.Speak(speak);
        }


        void Add(string msg, bool bot = false) => Device.BeginInvokeOnMainThread(() => this.Items.Add(new ListItemViewModel
        {
            Text = msg,
            From = bot ? "Computer" : "You"
        }));
    }
}
