//using System;
//using System.Reactive.Linq;


//namespace Plugin.SpeechRecognition
//{
//    public static class Extensions
//    {
//        public static IObservable<bool> Confirm(this ISpeechRecognizer speech, string yes = "yes", string no = "no") => speech
//            .ListenForFirstKeyword(yes, no)
//            .Select(x => x.Equals(yes, StringComparison.CurrentCultureIgnoreCase));
//    }
//}
