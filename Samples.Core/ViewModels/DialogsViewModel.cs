using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Acr.UserDialogs;
using Plugin.SpeechDialogs;
using ReactiveUI;
using Xamarin.Essentials;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class DialogsViewModel : ReactiveObject
    {
        public DialogsViewModel()
        {
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
                        await TextToSpeech.SpeakAsync(result);
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
                        await TextToSpeech.SpeakAsync(result ? "Your phone will now self destruct" : "Too Bad");
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
                        await TextToSpeech.SpeakAsync(result + " - BORING");
                    })
                }
            };
        }


        public IList<ListItemViewModel> List { get; set; }
        public bool IsActionsSpoken { get; set; } = true;
    }
}
