using Telegram.Bot.Types;

namespace FastApiWebhook.Services.ButtonServices.BotKeyboards
{
    public interface IInlineQueries
    {
        Task BackToMainMenu(CallbackQuery callbackQuery);
        Task EchoInline(CallbackQuery callbackQuery);
    }
}
