using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Windows.Input;
using Acr.UserDialogs;
using Microsoft.Bot.Connector.DirectLine;
using Plugin.SpeechRecognition;
using ReactiveUI;
using Xamarin.Forms;


namespace Samples.ViewModels
{
    public class BotViewModel : ReactiveObject
    {
        readonly ISpeechRecognizer speech = CrossSpeechRecognition.Current;
        readonly IUserDialogs dialogs = UserDialogs.Instance;
        DirectLineClient client;
        Conversation conversation;
        IDisposable listener;


        public BotViewModel()
        {
            this.Start = ReactiveCommand.CreateFromTask(async _ =>
            {
                if (String.IsNullOrWhiteSpace(App.MicrosoftBotSecretKey))
                {
                    this.dialogs.Alert("You have not set the Microsoft Bot secret key");
                    return;
                }

                try
                {
                    this.Conversation.Clear();
                    using (this.dialogs.Toast("Starting Bot Conversation", TimeSpan.MaxValue))
                    {
                        this.client = new DirectLineClient(App.MicrosoftBotSecretKey ?? "0");
                        this.conversation = await this.client.Conversations.StartConversationAsync();

                        this.Started = true;
                        this.StartListener();
                    }
                }
                catch (Exception ex)
                {
                    this.dialogs.Alert("Error starting conversation - " + ex);
                }
            },
            this.WhenAny(
                x => x.Started,
                x => !x.Value
            ));

            this.Stop = ReactiveCommand.CreateFromTask(async () =>
            {
                this.listener?.Dispose();
                try
                {
                    using (this.dialogs.Toast("Ending Bot Conversation", TimeSpan.MaxValue))
                    {
                        var activity = new Activity
                        {
                            Type = ActivityTypes.EndOfConversation
                        };
                        await this.client.Conversations.PostActivityAsync(this.conversation.ConversationId, activity);
                        this.conversation = null;
                    }
                }
                catch (Exception ex)
                {
                    this.dialogs.Alert("Error ending conversation - " + ex);
                }
            },
            this.WhenAny(
                x => x.Started,
                x => x.Value
            ));
        }


        void StartListener()
        {
            this.listener = Observable.StartAsync(async ct =>
            {
                while (!ct.IsCancellationRequested)
                {
                    var statement = await this.speech
                        .ListenUntilPause()
                        .Where(x => !String.IsNullOrWhiteSpace(x))
                        .ToTask(ct);

                    var activity = new Activity
                    {
                        Text = statement,
                        Type = ActivityTypes.Message
                    };
                    var response = await this.client.Conversations.PostActivityAsync(this.conversation.ConversationId, activity, ct);

                    this.PostMessage(true, statement);
                    this.PostMessage(false, "");
                }
            }).Subscribe();
        }


        void PostMessage(bool fromYou, string msg) => Device.BeginInvokeOnMainThread(() => this.Conversation.Add(new ListItemViewModel
        {
            From = fromYou ? "You" : "Bot",
            Text = msg
        }));



        bool started;
        public bool Started
        {
            get => this.started;
            private set => this.RaiseAndSetIfChanged(ref this.started, value);
        }


        public ObservableCollection<ListItemViewModel> Conversation { get; } = new ObservableCollection<ListItemViewModel>();

        public ICommand Start { get; }
        public ICommand Stop { get; }
    }
}
/*
Skip to content
This repository
Search
Pull requests
Issues
Marketplace
Gist
 @aritchie
 Sign out
 Watch 127
  Star 617
 Fork 723 Microsoft/BotBuilder-Samples
 Code  Issues 13  Pull requests 17  Projects 0  Wiki Insights
Branch: master Find file Copy pathBotBuilder-Samples/CSharp/core-DirectLineWebSockets/DirectLineClient/Program.cs
3bc4bfb  on Apr 5
@pcostantini pcostantini Use Token API to exchange the client's secret for a token
2 contributors @pcostantini @ejadib
RawBlameHistory
129 lines (109 sloc)  4.98 KB
namespace DirectLineSampleClient
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Bot.Connector.DirectLine;
    using Newtonsoft.Json;
    using System.Threading;
    using System.Text;
    using WebSocketSharp;

    public class Program
    {
        private static string directLineSecret = ConfigurationManager.AppSettings["DirectLineSecret"];
        private static string botId = ConfigurationManager.AppSettings["BotId"];

        // fromUser is the field that identifies which user is sending activities to the Direct Line service.
        // Because this value is created and sent within your Direct Line client, your bot should not
        // trust the value for any security-sensitive operations. Instead, have the user log in and
        // store any sign-in tokens against the Conversation or Private state fields. Those fields
        // are secured by the conversation ID, which is protected with a signature.
        private static string fromUser = "DirectLineSampleClientUser";

        public static void Main(string[] args)
        {
            StartBotConversation().Wait();
        }

        private static async Task StartBotConversation()
        {
            // Obtain a token using the Direct Line secret
            var tokenResponse = await new DirectLineClient(directLineSecret).Tokens.GenerateTokenForNewConversationAsync();

            // Use token to create conversation
            var directLineClient = new DirectLineClient(tokenResponse.Token);
            var conversation = await directLineClient.Conversations.StartConversationAsync();

            using (var webSocketClient = new WebSocket(conversation.StreamUrl))
            {
                webSocketClient.OnMessage += WebSocketClient_OnMessage;
                webSocketClient.Connect();

                while (true)
                {
                    string input = Console.ReadLine().Trim();

                    if (input.ToLower() == "exit")
                    {
                        break;
                    }
                    else
                    {
                        if (input.Length > 0)
                        {
                            Activity userMessage = new Activity
                            {
                                From = new ChannelAccount(fromUser),
                                Text = input,
                                Type = ActivityTypes.Message
                            };

                            await directLineClient.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                        }
                    }
                }
            }
        }

        private static void WebSocketClient_OnMessage(object sender, MessageEventArgs e)
        {
            // Occasionally, the Direct Line service sends an empty message as a liveness ping. Ignore these messages.
            if (string.IsNullOrWhiteSpace(e.Data))
            {
                return;
            }

            var activitySet = JsonConvert.DeserializeObject<ActivitySet>(e.Data);
            var activities = from x in activitySet.Activities
                             where x.From.Id == botId
                             select x;

            foreach (Activity activity in activities)
            {
                Console.WriteLine(activity.Text);

                if (activity.Attachments != null)
                {
                    foreach (Attachment attachment in activity.Attachments)
                    {
                        switch (attachment.ContentType)
                        {
                            case "application/vnd.microsoft.card.hero":
                                RenderHeroCard(attachment);
                                break;

                            case "image/png":
                                Console.WriteLine($"Opening the requested image '{attachment.ContentUrl}'");

                                Process.Start(attachment.ContentUrl);
                                break;
                        }
                    }
                }

                Console.Write("Command> ");
            }
        }

        private static void RenderHeroCard(Attachment attachment)
        {
            const int Width = 70;
            Func<string, string> contentLine = (content) => string.Format($"{{0, -{Width}}}", string.Format("{0," + ((Width + content.Length) / 2).ToString() + "}", content));

            var heroCard = JsonConvert.DeserializeObject<HeroCard>(attachment.Content.ToString());

            if (heroCard != null)
            {
                Console.WriteLine("/{0}", new string('*', Width + 1));
                Console.WriteLine("*{0}*", contentLine(heroCard.Title));
                Console.WriteLine("*{0}*", new string(' ', Width));
                Console.WriteLine("*{0}*", contentLine(heroCard.Text));
                Console.WriteLine("{0}/", new string('*', Width + 1));
            }
        }
    }
}
Contact GitHub API Training Shop Blog About
© 2017 GitHub, Inc. Terms Privacy Security Status Help
     */