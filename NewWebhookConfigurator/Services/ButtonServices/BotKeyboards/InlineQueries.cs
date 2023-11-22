using Telegram.Bot;
using Telegram.Bot.Types;

namespace FastApiWebhook.Services.ButtonServices.BotKeyboards
{
    public class InlineQueries : IInlineQueries
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IButtonService _buttonService;

        public InlineQueries(ITelegramBotClient botClient, IButtonService buttonService)
        {
            _botClient = botClient;
            _buttonService = buttonService;
        }
        public async Task BackToMainMenu(CallbackQuery callbackQuery)
        {
            await _buttonService.MainMenuUz(callbackQuery: callbackQuery);


            await _botClient.AnswerCallbackQueryAsync(
                   callbackQueryId: callbackQuery.Id,
                   text: "Kinoni qidirish uchun 'ID' kiriting:");
        }

        public async Task EchoInline(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data != null)
            {
                string dataCallback = callbackQuery.Data;
                if (dataCallback == "MAIN MENU")
                {
                    await BackToMainMenu(callbackQuery);
                }
            }
        }
    }
}
