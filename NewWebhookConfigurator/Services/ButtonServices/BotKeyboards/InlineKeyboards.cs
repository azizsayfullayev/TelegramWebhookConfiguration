using Telegram.Bot.Types.ReplyMarkups;

namespace FastApiWebhook.Services.ButtonServices.BotKeyboards
{
    public class InlineKeyboards
    {
        public static InlineKeyboardMarkup GetOrqagaKeyboard()
        {
            var inlineKeyboard = new InlineKeyboardMarkup(new[]
            {
        // Row 1
        new []
        {
            InlineKeyboardButton.WithCallbackData("Orqaga qaytish", "MAIN MENU")
        },
          });

            return inlineKeyboard;
        }
    }
}
