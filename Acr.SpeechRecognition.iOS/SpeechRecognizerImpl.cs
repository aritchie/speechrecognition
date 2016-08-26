using System;
using System.Threading;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;


namespace Acr.SpeechRecognition
{
    public class SpeechRecognizerImpl : ISpeechRecognizer
    {
        readonly AVAudioEngine audioEngine = new AVAudioEngine();
        readonly SFSpeechRecognizer speechRecognizer = new SFSpeechRecognizer();


        public Task<string> Listen(CancellationToken? cancelToken = null)
        {
            var speechRequest = new SFSpeechAudioBufferRecognitionRequest();
            SFSpeechRecognitionTask currentSpeechTask = null;
            var tcs = new TaskCompletionSource<string>();
            cancelToken?.Register(() =>
            {
                currentSpeechTask.Cancel();
                tcs.SetCanceled();
            });

            this.audioEngine.InputNode.InstallTapOnBus(
                bus: 0,
                bufferSize: 1024,
                format: this.audioEngine.InputNode.GetBusOutputFormat(0),
                tapBlock: (buffer, when) => speechRequest.Append(buffer)
            );
            this.audioEngine.Prepare();

            NSError error;
            this.audioEngine.StartAndReturnError(out error);
            currentSpeechTask = this.speechRecognizer.GetRecognitionTask(speechRequest, (result, err) =>
            {
                tcs.SetResult(result.BestTranscription.FormattedString);
                this.audioEngine.Stop();
                speechRequest.EndAudio();
            });
            return tcs.Task;
        }
    }
}
/*

    SFSpeechRecognizer.RequestAuthorization(status =>
    {
        if (status != SFSpeechRecognizerAuthorizationStatus.Authorized)
            return;



        InvokeOnMainThread(() => Dictate.Enabled = true);
    });
*/