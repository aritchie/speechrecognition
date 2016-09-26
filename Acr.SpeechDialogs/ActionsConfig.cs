using System;
using System.Collections;
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
        public bool SpeakQuestion { get; set; }
        public bool SpeakAvailableAnswers { get; set; }

        IDictionary<string, Action> Actions { get; set; } = new Dictionary<string, Action>();
    }
}
