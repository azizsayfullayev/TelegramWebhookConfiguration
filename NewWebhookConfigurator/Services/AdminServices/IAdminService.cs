using Telegram.Bot.Types;

namespace FastApiWebhook.Services.AdminServices
{
    public interface IAdminService
    {
        Task UploadMovieData(Message message, long adminId);
    }

}
