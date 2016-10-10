using System;
using System.Collections.Generic;
using Acr.SpeechDialogs;
using Plugin.TextToSpeech.Abstractions;
using PropertyChanged;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    [ImplementPropertyChanged]
    public class DialogsViewModel
    {
        public DialogsViewModel(ISpeechDialogs dialogs, ITextToSpeech tts)
        {
            this.List = new List<ListItemViewModel>
            {
                new ListItemViewModel
                {
                    Text = "Actions",
                    Command = new Command(() => dialogs.Actions(new ActionsConfig("Choose your destiny")
                        .SetShowDialog(this.ShowDialogs)
                        .SetSpeakChoices(true)
                        .Choice("Fatality", () => tts.Speak("Flawless Victory"))
                        .Choice("Friendship", () => tts.Speak("Friendship"))
                        .Choice("Bability", () => tts.Speak("Cute"))
                    ))
                },
                new ListItemViewModel
                {
                    Text = "Confirm",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Confirm("Shutdown your phone?", "Yes", "No", this.ShowDialogs);
                        tts.Speak(result ? "Your phone will now self destruct" : "Too Bad");
                    })
                },
                new ListItemViewModel
                {
                    Text = "Prompt",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Prompt("Tell me your life story.... quickly!");
                        tts.Speak(result + " - BORING");
                    })
                }
            };
        }


        public IList<ListItemViewModel> List { get; set; }
        public bool ShowDialogs { get; set; } = true;
    }
}
