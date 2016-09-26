using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Acr.SpeechDialogs.Impl
{
    public class SpeechDialogsImpl : ISpeechDialogs
    {
        public void Actions(IDictionary<string, Action> actions)
        {
            throw new NotImplementedException();
        }


        public Task<bool> Confirm(string question)
        {
            throw new NotImplementedException();
        }


        public Task<string> Prompt(string question)
        {
            throw new NotImplementedException();
        }
    }
}
