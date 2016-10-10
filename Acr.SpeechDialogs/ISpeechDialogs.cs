using System;
using System.Threading;
using System.Threading.Tasks;


namespace Acr.SpeechDialogs
{
    public interface ISpeechDialogs
    {
        void Actions(ActionsConfig config);
        Task<bool> Confirm(string question, string positive, string negative, bool showDialog = true, CancellationToken? cancelToken = null);
        Task<string> Prompt(string question, CancellationToken? cancelToken = null);
    }
}
