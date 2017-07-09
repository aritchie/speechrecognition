using System;
using Acr.UserDialogs;


namespace Plugin.SpeechDialogs
{
    public interface ISpeechDialogs
    {
        IObservable<bool> Confirm(ConfirmConfig config);
        IObservable<string> Choices(string question, string[] choices, bool speakChoices = false);
        IObservable<string> Question(PromptConfig config);
    }
}
