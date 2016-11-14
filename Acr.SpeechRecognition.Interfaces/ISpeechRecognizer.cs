using System;
using System.Threading.Tasks;


namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        SpeechRecognizerStatus Status { get; }
        Task<bool> RequestPermission();

        /// <summary>
        /// Optimal observable for taking command (yes/no/maybe/go away/etc)
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        /// TODO: unrecognized words?
        IObservable<string> ListenForFirstKeyword(params string[] keywords);

        /// <summary>
        /// Optimal command for listening to a sentence.  Completes when user pauses
        /// </summary>
        /// <returns></returns>
        IObservable<string> ListenUntilPause();

        /// <summary>
        /// Continuous dictation.  Returns text as made available.  Dispose to stop dictation.
        /// </summary>
        /// <returns></returns>
        IObservable<string> ContinuousDictation();

        IObservable<bool> WhenListeningStatusChanged();
    }
}
