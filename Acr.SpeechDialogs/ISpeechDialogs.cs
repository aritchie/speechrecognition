using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.SpeechDialogs
{
    public interface ISpeechDialogs
    {
        void Actions(IDictionary<string, Action> actions);
        Task<bool> Confirm(string question);
        Task<string> Prompt(string question);
    }
}
