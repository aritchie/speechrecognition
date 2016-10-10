using System;
using System.Threading.Tasks;


namespace Acr.SpeechRecognition
{
    public interface ISpeechRecognizer
    {
        SpeechRecognizerStatus Status { get; }
        Task<bool> RequestPermission();
        IObservable<string> Listen(bool completeOnEndOfSpeech = false);
        IObservable<bool> WhenListeningStatusChanged();
        //IObservable<string> Listen(bool completeOnEndOfSpeech = false, CultureInfo culture = null);
        //IList<CultureInfo> AvailableCultures { get; }
    }
}
