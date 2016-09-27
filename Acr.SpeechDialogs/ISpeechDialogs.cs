using System;
using System.Threading.Tasks;


namespace Acr.SpeechDialogs
{
    public interface ISpeechDialogs
    {
        void Actions(ActionsConfig config);
        Task<bool> Confirm(string question, ConfirmOptions options);
        Task<string> Prompt(string question);
    }
}
