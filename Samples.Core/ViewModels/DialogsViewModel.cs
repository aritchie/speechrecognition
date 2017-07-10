using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Acr.UserDialogs;
using Plugin.SpeechDialogs;
using Plugin.TextToSpeech;
using ReactiveUI;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DialogsViewModel : ReactiveObject
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
                            new []
                            {
                                "Fatality",
                                "Friendship",
                                "Bability"
                            },
                            this.IsActionsSpoken
                        );
                        await tts.Speak(result);
                    })
                },
                new ListItemViewModel
                {
                    Text = "Confirm",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Confirm(new ConfirmConfig
                        {
                            Message = "Shutdown your phone?",
                            OkText = "Yes",
                            CancelText = "No"
                        });
                        await tts.Speak(result ? "Your phone will now self destruct" : "Too Bad");
                    })
                },
                new ListItemViewModel
                {
                    Text = "Prompt",
                    Command = new Command(async () =>
                    {
                        var result = await dialogs.Question(new PromptConfig
                        {
                            Message = "Tell me your life story.... quickly!"
                        });
                        await tts.Speak(result + " - BORING");
                    })
                }
            };
        }


        public IList<ListItemViewModel> List { get; set; }
        public bool IsActionsSpoken { get; set; } = true;
    }
}
