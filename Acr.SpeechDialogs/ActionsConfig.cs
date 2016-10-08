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


        public string Question { get; }
        public bool SpeakAnswers { get; set; }
        //public bool RepeatOnBadCommand { get; set; }
        //public int RepeatOnBadAttempts { get; set; }

        public IDictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();


        public ActionsConfig Add(string phrase, Action action)
        {
            this.Actions.Add(phrase, action);
            return this;
        }
    }
}
