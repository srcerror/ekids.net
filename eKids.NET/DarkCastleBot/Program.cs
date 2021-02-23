using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace DarkCastleBot
{
    public static class Program
    {
        private static TelegramBotClient Bot;
        public static Dictionary<long, int> Page = new Dictionary<long, int>();
        static public readonly Dictionary<int, string> Gamebook = new Dictionary<int, string>();

        public static async Task Main()
        {
#if USE_PROXY
            var Proxy = new WebProxy(Configuration.Proxy.Host, Configuration.Proxy.Port) { UseDefaultCredentials = true };
            Bot = new TelegramBotClient(Configuration.BotToken, webProxy: Proxy);
#else
            Bot = new TelegramBotClient(Configuration.BotToken);
#endif

            var me = await Bot.GetMeAsync();
            Console.Title = me.Username;

            Bot.OnMessage += DarkCastleOnMessageReceived; // BotOnMessageReceived;
            //Bot.OnMessageEdited += BotOnMessageReceived;
            Bot.OnCallbackQuery += BotOnCallbackQueryReceived;
            Bot.OnInlineQuery += BotOnInlineQueryReceived;
            Bot.OnInlineResultChosen += BotOnChosenInlineResultReceived;
            Bot.OnReceiveError += BotOnReceiveError;

            /// Load the book
            /// 
            var path = LocateFile("book1\\book1.txt");
            int page = 1;
            string text = "";
            foreach (var str in System.IO.File.ReadAllLines(path))
            {
                if (str.Trim() == page.ToString())
                {
                    // create page
                    if (page > 1)
                        Gamebook[page - 1] = text;
                    page++;
                    text = "";
                }
                else
                {
                    text += "\n" + str;
                }
            }
            // preparations complete

            Bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            Bot.StopReceiving();
        }

        private static string LocateFile(string filename)
        {
            Debug.WriteLine($"Looking for {filename} from current directory: {Environment.CurrentDirectory}");

            // check current directory
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in current directory = {Environment.CurrentDirectory}");
                return path;
            }

            // check Visual Studio relative path
            var relpathVS = @"..\..\..\..\..\input";
            path = Path.Combine(relpathVS, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio relative path to input folder");
                return path;
            }

            //check Visual Studio Code relative path
            var relpathVSC = @"..\..\input";
            path = Path.Combine(relpathVSC, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio Code relative path to input folder");
                return path;
            }
            throw new FileNotFoundException($"File {filename} was not found");
        }

        private static async void DarkCastleOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                // Send inline keyboard
                case "/start":
                    await Start(message);
                    break;

                default:
                    await Play(message);
                    break;
            }

        }

        static async Task Start(Message message)
        {
            var chatId = message.Chat.Id;
            Page[chatId] = 1;

            string text = Gamebook[Page[chatId]];

            await Bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: text,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }

        static async Task Play(Message message)
        {
            var input = message.Text.Trim();
            var chatId = message.Chat.Id;
            if (!Page.ContainsKey(chatId))
            {
                Page[chatId] = 1;
                input = "1";
            }

            int page;
            string text = "";
            if (input.StartsWith('/'))
                input = input.Substring(1);
            if (int.TryParse(input, out page) && Gamebook.ContainsKey(page))
            {
                text = Gamebook[page];
                Page[chatId] = page;
            }
            else
            {
                text = Gamebook[Page[chatId]] + "\n\n" + "Неверный ввод. Введите номер страницы";
            }

            await Bot.SendTextMessageAsync(
                   chatId: message.Chat.Id,
                   text: text,
                   replyMarkup: new ReplyKeyboardRemove()
               );
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                // Send inline keyboard
                case "/inline":
                    await SendInlineKeyboard(message);
                    break;

                // send custom keyboard
                case "/keyboard":
                    await SendReplyKeyboard(message);
                    break;

                // send a photo
                case "/photo":
                    await SendDocument(message);
                    break;

                // request location or contact
                case "/request":
                    await RequestContactAndLocation(message);
                    break;

                default:
                    await Usage(message);
                    break;
            }

            // Send inline keyboard
            // You can process responses in BotOnCallbackQueryReceived handler
            static async Task SendInlineKeyboard(Message message)
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                // Simulate longer running task
                await Task.Delay(500);

                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    // first row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("1.1", "11"),
                        InlineKeyboardButton.WithCallbackData("1.2", "12"),
                    },
                    // second row
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("2.1", "21"),
                        InlineKeyboardButton.WithCallbackData("2.2", "22"),
                    }
                });
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: inlineKeyboard
                );
            }

            static async Task SendReplyKeyboard(Message message)
            {
                var replyKeyboardMarkup = new ReplyKeyboardMarkup(
                    new KeyboardButton[][]
                    {
                        new KeyboardButton[] { "1.1", "1.2" },
                        new KeyboardButton[] { "2.1", "2.2" },
                    },
                    resizeKeyboard: true
                );

                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Choose",
                    replyMarkup: replyKeyboardMarkup

                );
            }

            static async Task SendDocument(Message message)
            {
                await Bot.SendChatActionAsync(message.Chat.Id, ChatAction.UploadPhoto);

                const string filePath = @"Files/tux.png";
                using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var fileName = filePath.Split(Path.DirectorySeparatorChar).Last();
                await Bot.SendPhotoAsync(
                    chatId: message.Chat.Id,
                    photo: new InputOnlineFile(fileStream, fileName),
                    caption: "Nice Picture"
                );
            }

            static async Task RequestContactAndLocation(Message message)
            {
                var RequestReplyKeyboard = new ReplyKeyboardMarkup(new[]
                {
                    KeyboardButton.WithRequestLocation("Location"),
                    KeyboardButton.WithRequestContact("Contact"),
                });
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "Who or Where are you?",
                    replyMarkup: RequestReplyKeyboard
                );
            }

            static async Task Usage(Message message)
            {
                string usage = $"ECHO: {message.Text} \n" +
                                        "Usage:\n" +
                                        "/inline   - send inline keyboard\n" +
                                        "/keyboard - send custom keyboard\n" +
                                        "/photo    - send a photo\n" +
                                        "/request  - request location or contact";
                await Bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        // Process Inline Keyboard callback data
        private static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            await Bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"Received {callbackQuery.Data}"
            );

            await Bot.SendTextMessageAsync(
                chatId: callbackQuery.Message.Chat.Id,
                text: $"Received {callbackQuery.Data}"
            );
        }

        #region Inline Mode

        private static async void BotOnInlineQueryReceived(object sender, InlineQueryEventArgs inlineQueryEventArgs)
        {
            Console.WriteLine($"Received inline query from: {inlineQueryEventArgs.InlineQuery.From.Id}");

            InlineQueryResultBase[] results = {
                // displayed result
                new InlineQueryResultArticle(
                    id: "3",
                    title: "TgBots",
                    inputMessageContent: new InputTextMessageContent(
                        "hello"
                    )
                )
            };
            await Bot.AnswerInlineQueryAsync(
                inlineQueryId: inlineQueryEventArgs.InlineQuery.Id,
                results: results,
                isPersonal: true,
                cacheTime: 0
            );
        }

        private static void BotOnChosenInlineResultReceived(object sender, ChosenInlineResultEventArgs chosenInlineResultEventArgs)
        {
            Console.WriteLine($"Received inline result: {chosenInlineResultEventArgs.ChosenInlineResult.ResultId}");
        }

        #endregion

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}
