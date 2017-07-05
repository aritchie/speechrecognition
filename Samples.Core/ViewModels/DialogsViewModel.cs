using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Plugin.SpeechDialogs;
using Plugin.TextToSpeech;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DialogsViewModel
    {
        public DialogsViewModel()
        {
            var tts = CrossTextToSpeech.Current;
            var dialogs = new SpeechDialogs();

            this.List = new List<ListItemViewModel>
            {
                new ListItemViewModel
                {
                    Text = "Actions",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Choices(
                            "Choose your destiny",
                            this.ShowDialogs,
                            "Fatality", //, () => tts.Speak("Flawless Victory"))
                            "Friendship", // tts.Speak("Friendship"))
                            "Bability" //, () => tts.Speak("Cute"))
                        );

                    })
                },
                new ListItemViewModel
                {
                    Text = "Confirm",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Confirm("Shutdown your phone?", "Yes", "No");
                        await tts.Speak(result ? "Your phone will now self destruct" : "Too Bad");
                    })
                },
                new ListItemViewModel
                {
                    Text = "Prompt",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Question("Tell me your life story.... quickly!");
                        tts.Speak(result + " - BORING");
                    })
                }
            };
        }


        public IList<ListItemViewModel> List { get; set; }
        public bool ShowDialogs { get; set; } = true;
    }
}
