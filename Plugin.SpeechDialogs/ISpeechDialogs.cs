using System;


namespace Plugin.SpeechDialogs
{
    public interface ISpeechDialogs
    {
        IObservable<bool> Confirm(string question, string yes = "yes", string no = "no");
        IObservable<string> Choices(string question, bool speakChoices = true, params string[] choices);
        IObservable<string> Question(string question);
    }
}
