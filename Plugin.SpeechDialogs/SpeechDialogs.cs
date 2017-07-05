using System;


namespace Plugin.SpeechDialogs
{
    public class SpeechDialogs : ISpeechDialogs
    {
        public IObservable<string> Choices(string question, bool speakChoices = true, params string[] choices)
        {
            throw new NotImplementedException();
        }

        public IObservable<bool> Confirm(string question, string yes = "yes", string no = "no")
        {
            throw new NotImplementedException();
        }

        public IObservable<string> Question(string question)
        {
            throw new NotImplementedException();
        }
    }
}
