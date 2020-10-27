using System;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace COVID_19_UK_Bot
{
    static class Program
    {
        private static TelegramBotClient _bot;

        public static async Task Main()
        {
            _bot = new TelegramBotClient(Configuration.BotToken);

            var me = await _bot.GetMeAsync();
            Console.Title = me.Username;

            _bot.OnMessage += BotOnMessageReceived;
            _bot.OnMessageEdited += BotOnMessageReceived;
            _bot.OnReceiveError += BotOnReceiveError;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Console.WriteLine($"Start listening for @{me.Username}");

            Console.ReadLine();
            _bot.StopReceiving();
        }

        private static async void BotOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var message = messageEventArgs.Message;
            if (message == null || message.Type != MessageType.Text)
                return;

            switch (message.Text.Split(' ').First())
            {
                case "/ukcovid@Covid19UkBot":
                case "/ukcovid":
                    await GetUkCovid19Info(message);
                    break;
                case "/queen":
                case "/queen@Covid19UkBot":
                    await GetQueenSpeech(message);
                    break;
                default:
                    await Usage(message);
                    break;
            }

            static async Task GetUkCovid19Info(Message message)
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: await CovidApi.AsyncGetMsgWithLatestDataByNation(),
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            static async Task GetQueenSpeech(Message message)
            {
                await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "🇬🇧 We will succeed and that success will belong to every one of us. --Elizabeth II\n" +
                          "https://www.youtube.com/watch?v=2klmuggOElE",
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            static async Task Usage(Message message)
            {
                const string usage = "Usage:\n" +
                                     "/ukcovid - Get 🇬🇧 COVID-19 Information\n" +
                                     "/queen - Get queen's speech";
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: usage,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Console.WriteLine("Received error: {0} — {1}",
                receiveErrorEventArgs.ApiRequestException.ErrorCode,
                receiveErrorEventArgs.ApiRequestException.Message
            );
        }
    }
}