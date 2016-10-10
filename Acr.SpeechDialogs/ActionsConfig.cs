using System;
using System.Collections.Generic;


namespace Acr.SpeechDialogs
{
    public class ActionsConfig
    {
        public ActionsConfig(string question)
        {
            this.Question = question;
        }


        public static bool DefaultSpeakChoices { get; set; } = true;
        public static bool DefaultShowDialog { get; set; } = true;

        /// <summary>
        /// The question to read to the user
        /// </summary>
        public string Question { get; }


        /// <summary>
        /// This will speak all available entered choices
        /// </summary>
        public bool SpeakChoices { get; set; } = DefaultSpeakChoices;


        /// <summary>
        /// This will display a UI dialog as well with all available choices
        /// </summary>
        public bool ShowDialog { get; set; } = DefaultShowDialog;
        //public bool RepeatOnBadCommand { get; set; }
        //public int RepeatOnBadAttempts { get; set; }

        public IDictionary<string, Action> Choices { get; set; } = new Dictionary<string, Action>();


        public ActionsConfig Choice(string phrase, Action action)
        {
            this.Choices.Add(phrase, action);
            return this;
        }


        public ActionsConfig SetShowDialog(bool show)
        {
            this.ShowDialog = show;
            return this;
        }


        public ActionsConfig SetSpeakChoices(bool speakAnswers)
        {
            this.SpeakChoices = speakAnswers;
            return this;
        }
    }
}
