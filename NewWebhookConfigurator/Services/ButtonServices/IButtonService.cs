using Telegram.Bot.Types;

namespace FastApiWebhook.Services.ButtonServices
{
    public interface IButtonService
    {
        Task ChooseLanguageButton(Message message);
        Task MainMenuUz(Message message = null, CallbackQuery callbackQuery = null);
    }
}
