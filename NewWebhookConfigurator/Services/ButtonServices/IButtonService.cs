using Telegram.Bot.Types;

namespace FastApiWebhook.Services.ButtonServices
{
    public interface IButtonService
    {
        Task ChooseLanguageButton(Message message);
        Task MovieSearchIdUz(Message message);
    }
}
