using Microsoft.AspNetCore.Mvc;
using FastApiWebhook.Filters;
using FastApiWebhook.Services;
using Telegram.Bot.Types;

namespace FastApiWebhook.Controllers;

public class BotController : ControllerBase
{
    [HttpPost]
    [ValidateTelegramBot]
    public async Task<IActionResult> Post(
        [FromBody] Update update,
        [FromServices] UpdateHandlers handleUpdateService,
        CancellationToken cancellationToken)
    {
        await handleUpdateService.HandleUpdateAsync(update, cancellationToken);
        return Ok();
    }
}
