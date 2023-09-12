using MongoDB.Driver;
using System.ComponentModel;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace FastApiWebhook.Services.ButtonServices
{
    public class ButtonService : IButtonService
    {
        private readonly ITelegramBotClient botClient;
        private readonly IMongoDatabase _mongoDb;

        public ButtonService(ITelegramBotClient telegramBotClient, IMongoDatabase mongoDb)
        {
            botClient = telegramBotClient;
            _mongoDb = mongoDb;
        }

        public async Task ChooseLanguageButton(Message message)
        {
            long chatId = message.Chat.Id;
            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "🇷🇺 Ru ", "🇺🇿 Uz" },
            })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "Tilni tanlang:",
                replyMarkup: replyKeyboardMarkup);

        }

        public async Task MovieSearchIdUz(Message message)
        {
            long chatId = message.Chat.Id;

            ReplyKeyboardMarkup replyKeyboardMarkup = new(new[]
            {
                new KeyboardButton[] { "Kino nomi bo'yicha qidirish", "Call me ☎️" },
            })
            {
                ResizeKeyboard = true
            };

            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: "💃 Kino ID raqamini jo'nating va man kinoni topib beraman :",
                replyMarkup: replyKeyboardMarkup);
        }
    }
}
