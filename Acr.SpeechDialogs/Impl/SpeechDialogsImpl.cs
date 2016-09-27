using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Acr.SpeechRecognition;
using Plugin.TextToSpeech.Abstractions;
using System.Reactive.Linq;
using System.Linq;

namespace Acr.SpeechDialogs.Impl
{
    public class SpeechDialogsImpl : ISpeechDialogs
    {
        readonly ISpeechRecognizer speech;
        readonly ITextToSpeech tts;


        public SpeechDialogsImpl(ISpeechRecognizer sr, ITextToSpeech tts)
        {
            this.speech = sr;
            this.tts = tts;
        }


        public void Actions(ActionsConfig config)
        {
            this.tts.Speak(config.Question);

            if (config.SpeakAnswers) 
            {
                foreach (var key in config.Actions.Keys)
                {
                    this.tts.Speak(key, true);
                }
            }

            this.speech
                .Dictate()
                .Where(x => x.Equals(x, StringComparison.CurrentCultureIgnoreCase))
                .Take(1)
                .Subscribe(x => config.Actions[x]?.Invoke());
        }


        public async Task<bool> Confirm(string question, ConfirmOptions options)
        {
            this.tts.Speak(question, false);
            var result = await this.speech
                .Dictate()
                .Where(x => x == "yes" || x == "no")
                .Take(1);
            
            return result == "yes";
        }


        public async Task<string> Prompt(string question)
        {
            this.tts.Speak(question);
            var result = await this.speech.Command();
            return result;
        }
    }
}
