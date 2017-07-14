using System;


namespace Plugin.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        /// <summary>
        /// Returns true if platform is supported
        /// </summary>
        bool IsSupported { get; }


        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        IObservable<SpeechRecognizerStatus> RequestPermission();


        /// <summary>
        /// Optimal observable for taking command (yes/no/maybe/go away/etc)
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
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


        /// <summary>
        /// When listening status changes
        /// </summary>
        /// <returns></returns>
        IObservable<bool> WhenListeningStatusChanged();
    }
}
