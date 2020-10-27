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

            _bot.OnCallbackQuery += BotOnCallbackQueryReceived;

            _bot.StartReceiving(Array.Empty<UpdateType>());
            Log.i($"Start listening for @{me.Username}");

            Console.ReadLine();
            _bot.StopReceiving();
            Log.i("Stop receiving.");
        }

        static async void BotOnCallbackQueryReceived(object sender, CallbackQueryEventArgs callbackQueryEventArgs)
        {
            var callbackQuery = callbackQueryEventArgs.CallbackQuery;

            var message = callbackQuery.Message;

            await _bot.AnswerCallbackQueryAsync(
                callbackQueryId: callbackQuery.Id,
                text: $"🇬🇧 Your nation is {callbackQuery.Data}"
            );
            await _bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId,
                $"🇬🇧 Your nation is {callbackQuery.Data}. Waiting...");

            await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);

            await _bot.EditMessageTextAsync(
                message.Chat.Id,
                message.MessageId,
                await CovidApi.AsyncGetMsgWithLatestDataByNation(CovidApi.ConvertToLocation(callbackQuery.Data))
            );
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
                    await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                    await SendUkCovid19Info(message);
                    break;
                case "/queen":
                case "/queen@Covid19UkBot":
                    await _bot.SendChatActionAsync(message.Chat.Id, ChatAction.Typing);
                    await SendQueenSpeech(message);
                    break;
                case "/nation":
                case "/nation@Covid19UkBot":
                    await SendNationInlineKeyboard(message);
                    break;
                default:
                    await SendUsage(message);
                    break;
            }

            static async Task SendUkCovid19Info(Message message)
                => await SendMsg(
                    message: message,
                    text: await CovidApi.AsyncGetMsgWithLatestDataByNation()
                );

            static async Task SendQueenSpeech(Message message)
                => await SendMsg(message, Text.QUEEN_SPEECH_MSG);

            static async Task SendUsage(Message message)
                => await SendMsg(message, Text.USAGE);

            static async Task SendMsg(Message message, string text)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: text,
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            static async Task SendNationInlineKeyboard(Message message)
            {
                var inlineKeyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("England", "England"),
                        InlineKeyboardButton.WithCallbackData("Scotland", "Scotland"),
                    },
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData("Northern Ireland", "Northern Ireland"),
                        InlineKeyboardButton.WithCallbackData("Wales", "Wales"),
                    }
                });
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "🇬🇧 Choose you nation",
                    replyMarkup: inlineKeyboard
                );
            }
        }

        private static void BotOnReceiveError(object sender, ReceiveErrorEventArgs receiveErrorEventArgs)
        {
            Log.e($"{receiveErrorEventArgs.ApiRequestException.ErrorCode.ToString()}" +
                  " — " +
                  $"{receiveErrorEventArgs.ApiRequestException.Message}");
        }
    }
}