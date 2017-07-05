using System;
using System.Reactive.Linq;
using Plugin.SpeechRecognition;
using Plugin.TextToSpeech;
using Plugin.TextToSpeech.Abstractions;


namespace Plugin.SpeechDialogs
{
    public class SpeechDialogs : ISpeechDialogs
    {
        readonly ITextToSpeech tts = CrossTextToSpeech.Current;
        readonly ISpeechRecognizer speech = CrossSpeechRecognition.Current;


        public IObservable<string> Choices(string question, bool speakChoices = true, params string[] choices)
        {
            throw new NotImplementedException();
        }


        public IObservable<bool> Confirm(string question, string yes = "yes", string no = "no") => Observable.FromAsync(async ct =>
        {
            //this.speech.ListenForFirstKeyword(yes, no).Subscribe(x => )
            await this.tts.Speak(question, cancelToken: ct);
            return true;
        });


        public IObservable<string> Question(string question)
        {
            //this.speech.ListenForFirstKeyword(yes, no).Subscribe(x => )
            //await this.tts.Speak(question, cancelToken: ct);
            //return true;
            return Observable.Return(String.Empty);
        }
    }
}
