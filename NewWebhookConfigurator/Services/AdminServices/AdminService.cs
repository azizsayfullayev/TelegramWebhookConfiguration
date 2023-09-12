using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using FastApiWebhook.DbContexts;
using FastApiWebhook.Models;

namespace FastApiWebhook.Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ITelegramBotClient _botClient;



        public AdminService(ITelegramBotClient botClient, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _botClient = botClient;
        }

        public async Task UploadMovieData(Message message, long adminId)
        {

            var lastRow = _appDbContext.Movies.OrderByDescending(x => x.Id).FirstOrDefault();
            if (lastRow != null)
            {
                if (message.Text != null)
                {
                    if (message.Text.ToLower() == "/delete")
                    {
                        if (lastRow.VideoId == "")
                        {
                            _appDbContext.Movies.Remove(lastRow);
                            await _appDbContext.SaveChangesAsync();
                            await SendMessage(adminId, "O'chirildi, qaytib kinoni nomini jo'natishdan boshalang");
                            return;
                        }
                        await SendMessage(adminId, "To'g'ri nom qo'yib jo'nat");
                        return;

                    }
                    else if (message.Text.ToLower() == "/delete_movie")
                    {
                        _appDbContext.Movies.Remove(lastRow);
                        await _appDbContext.SaveChangesAsync();
                        await SendMessage(adminId, "O'chirildi, qaytib kinoni nomini jo'natishdan boshalang");
                        return;
                    }
                    if (lastRow.VideoId == "")
                    {
                        await SendMessage(adminId, "Kinoni nomi berilgan, qolgan malumotlarini jo'nating!");
                        return;
                    }

                    await AddName(message, adminId);
                    return;

                }
                else if (lastRow.VideoId == "")
                {
                    if (message.Video == null)
                    {
                        // Video has not
                        await SendMessage(adminId, "Talabga javob bermaydi tekshirib qaytib jo'nating!");
                        return;
                    }
                    if (message.Caption == "")
                    {
                        // Caption has not
                        await _botClient.SendTextMessageAsync(chatId: adminId, text: "Talabga javob bermaydi tekshirib qaytib jo'nating!");
                        return;
                    }
                    if (message.CaptionEntities == null)
                    {
                        // Caption entites has not
                        await _botClient.SendTextMessageAsync(chatId: adminId, text: "Talabga javob bermaydi tekshirib qaytib jo'nating!");
                        return;
                    }

                    lastRow.VideoId = message.Video.FileId;
                    lastRow.Description = message.Caption;

                    var captionEntities = JsonConvert.SerializeObject(message.CaptionEntities);
                    lastRow.DescriptionEntities = captionEntities;

                    string messageToAdmin = $"Kino malumotlari Muvaffaqiyatli qo'shildi:\n Kino nomi: " + lastRow.Title + "\nKino ID: " + lastRow.Id + "\n\nKeyingi kinoni nomini jo'nating yoki /delete_movie orqali kinoni o'chirib tashang.";
                    await _botClient.SendTextMessageAsync(chatId: adminId, text: messageToAdmin);

                    await _appDbContext.SaveChangesAsync();
                    return;
                }
                await SendMessage(adminId, "Kinoni qo'sholmadim qandaydir xatolik bor");
            }
            else if (message.Text != null)
            {
                await AddName(message, adminId);
                return;
            }




        }

        private async Task SendMessage(long chatId, string message)
        {

            await _botClient.SendTextMessageAsync(chatId, message);

        }
        private async Task AddName(Message message, long adminId)
        {

            var newMovie = new Movie();
            newMovie.Title = message.Text;
            newMovie.AdminId = adminId;

            await SendMessage(adminId, "Kino nomi muvaffaqiyatli qo'shildi, endi qolgan malumotlarni jo'nating!\nAgar xatolik ketgan bo'lsa: /delete ni bosing.");
            await _appDbContext.AddAsync(newMovie);
            await _appDbContext.SaveChangesAsync();
            return;
        }
    }
}
