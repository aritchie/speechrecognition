using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech.Abstractions;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    [ImplementPropertyChanged]
    public class ConversationViewModel
    {
        readonly ISpeechRecognizer speech;
        readonly ITextToSpeech tts;


        public ConversationViewModel(ISpeechRecognizer speech, ITextToSpeech tts)
        {
            this.speech = speech;
            this.tts = tts;
            this.Start = new Command(() => this.DoConversation());

            speech.WhenListeningStatusChanged().Subscribe(x => this.IsListening = x);
        }


        public ICommand Start { get; }
        public bool IsListening { get; set; }
        public ObservableCollection<ConversationItemViewModel> Items { get; } = new ObservableCollection<ConversationItemViewModel>();


        async Task DoConversation()
        {
            this.Items.Clear();

            using (var cancelSrc = new CancellationTokenSource())
            {
                await this.Computer("Please tell me your name");
                var name = await this.speech.ContinuousDictation().Take(1).RunAsync(cancelSrc.Token);
                this.Add(name);

                await this.Computer($"Hello {name}.  Are you male or female?");
                var sex = await this.speech.ListenForFirstKeyword("male", "female");
                this.Add(sex);

                var next = sex.Equals("male", StringComparison.CurrentCultureIgnoreCase) ? "Yo dude" : "";
                await this.Computer(next);

                await this.Computer("Tell me something interesting about yourself");
                next = await this.speech.ListenUntilPause();
                this.Add(next);

                await this.Computer("How incredibly uninteresting.  We're done - press the button again!");
            }
        }


        async Task Computer(string speak)
        {
            this.Items.Add(new ConversationItemViewModel
            {
                Text = speak,
                FromComputer = true
            });
            await this.tts.Speak(speak);
        }


        void Add(string value)
        {
            this.Items.Add(new ConversationItemViewModel
            {
                Text = value,
                FromComputer = false
            });
        }
    }
}
