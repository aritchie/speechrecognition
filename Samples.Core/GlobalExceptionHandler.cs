using System;
using Acr.UserDialogs;
using ReactiveUI;


namespace Samples
{
    public class GlobalExceptionHandler : IObserver<Exception>
    {
        public static void Register() => RxApp.DefaultExceptionHandler = new GlobalExceptionHandler();
        public void OnCompleted() {}
        public void OnError(Exception error) {}


        public void OnNext(Exception value)
        {
            Console.WriteLine(value);
            UserDialogs.Instance.Alert(value.ToString());
        }
    }
}