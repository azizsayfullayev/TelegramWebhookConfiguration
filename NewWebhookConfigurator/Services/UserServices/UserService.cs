using FastApiWebhook.DbContexts;
using FastApiWebhook.Models;
using FastApiWebhook.Services.ButtonServices;
using FastApiWebhook.Services.ButtonServices.BotKeyboards;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace FastApiWebhook.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IMongoDatabase _mongoDb;
        private readonly IButtonService _buttonService;
        private readonly AppDbContext _appDbContext;
        private readonly ITelegramBotClient _botClient;

        public UserService(AppDbContext appDbContext,
            ITelegramBotClient botClient,
            IButtonService buttonService,
            IMongoDatabase mongoDb)
        {
            _mongoDb = mongoDb;
            _buttonService = buttonService;
            _appDbContext = appDbContext;
            _botClient = botClient;
        }

        public async Task EchoUser(Message message)
        {
            // Everthing starts from here
            try
            {
                if (message != null && message.Text != null)
                {
                    var xxx = JsonConvert.SerializeObject(message.Entities);
                    if (message.Text == "/start")
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        await _buttonService.ChooseLanguageButton(message);
                        await IsNewUser(message);
                        stopwatch.Stop();

                        // Get the elapsed time in various formats
                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, elapsedTime.TotalSeconds.ToString());
                    }
                    else if (message.Text == "🇺🇿 Uz")
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        await _buttonService.MainMenuUz(message);

                        TimeSpan elapsedTime = stopwatch.Elapsed;
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, elapsedTime.TotalSeconds.ToString());
                    }
                    else if (message.Text == "🇷🇺 Ru")
                    {

                    }
                    else if (message.Text == "ℹ️ Bot haqida")
                    {
                        string photoId = "AgACAgIAAxkBAAIBzGViL5mbEln-JlTRbgvnqgt_BXz-AAKR1zEbMuoQS6UfT2Bg-hmvAQADAgADcwADMwQ";
                        string caption = "🎬 Kino qidiruv boti: Kino ixlosmandlari uchun. ID yoki film nomini kiritish orqali minglab kinolarni bir zumda topishingiz mumkin. \r\n\r\nO'ziz xohlagan kinoni topa olmayapsizmi? Hech qisi yo'q, tashvishlarga o'rin yo'q! Shunchaki @movieshipgroup guruhimzga azo bo'ling va guruhdagi adminlardan shu kinoni to'pib berishlarini so'rang 🗣. Sizning kino qidiruvingiz uchun tayyorlandi!\r\n\r\n🔍 O'ziz xohlagan kinoni topa olmadingizmi? 🤔 Buni @movieshipgroup dan so'rang! 🌟\r\n\r\n⛵️ Bizning kanal : @movie_ship";
                        string captionEntitesText = "[\r\n    {\r\n        \"type\": \"bold\",\r\n        \"offset\": 48,\r\n        \"length\": 2\r\n    },\r\n    {\r\n        \"type\": \"bold\",\r\n        \"offset\": 61,\r\n        \"length\": 6\r\n    },\r\n    {\r\n        \"type\": \"mention\",\r\n        \"offset\": 227,\r\n        \"length\": 15\r\n    },\r\n    {\r\n        \"type\": \"mention\",\r\n        \"offset\": 432,\r\n        \"length\": 15\r\n    },\r\n    {\r\n        \"type\": \"mention\",\r\n        \"offset\": 484,\r\n        \"length\": 11\r\n    }\r\n]\r\n";
                        var captionEntitesJson = JsonConvert.DeserializeObject<List<MessageEntity>>(captionEntitesText);
                        var inlineKeyboard = InlineKeyboards.GetOrqagaKeyboard();
                        await _botClient.SendPhotoAsync(chatId: message.Chat.Id, photo: InputFile.FromFileId(photoId), caption: caption, captionEntities: captionEntitesJson,replyMarkup: inlineKeyboard);
                    }
                    else if (message.Text == "☎️ Bog'lanish")
                    {
                        string caption = "Savollar va takliflar bo'yicha  👉 @movieship_admin";
                        string captionEntitesText = "[\r\n    {\r\n        \"type\": \"mention\",\r\n        \"offset\": 35,\r\n        \"length\": 16\r\n    }\r\n]\r\n";
                        var captionEntitesJson = JsonConvert.DeserializeObject<List<MessageEntity>>(captionEntitesText);
                        var inlineKeyboard = InlineKeyboards.GetOrqagaKeyboard();
                        await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, text: caption, entities: captionEntitesJson, replyMarkup:inlineKeyboard);
                    }
                    else if (message.Text.All(char.IsDigit))
                    {
                        await SearchByIdAndSendMovie(message, long.Parse(message.Text));
                    }
                    else
                    {
                        // Finds movie with its name 
                        await SearchByNameAndSend(message, message.Text);
                    }


                }

            }
            catch (Exception e)
            {

            }


        }

        public async Task IsNewUser(Message message)
        {
            if (message != null)
            {
                var users = _appDbContext.Users.Where(x => x.ChatId == message.Chat.Id);
                var user = users.FirstOrDefault();

                if (user == null)
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "User topilmadi");

                    var newUser = new FastApiWebhook.Models.User();
                    newUser.ChatId = message.Chat.Id;
                    newUser.UserName = "";
                    if (message.Chat != null && message.Chat.Username != "")
                    {
                        newUser.UserName = message.Chat.Username;
                    }
                    newUser.FirstName = message.Chat.FirstName;

                    await _appDbContext.Users.AddAsync(newUser);
                    await _appDbContext.SaveChangesAsync();
                    return;
                }
            }

            return;
        }

        public async Task SearchByIdAndSendMovie(Message message, long id)
        {
            var movie = _appDbContext.Movies.Where(x => x.Id == id).FirstOrDefault();
            var chatId = message.Chat.Id;
            if (movie != null)
            {

                await SendMovie(message, movie);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Bunaqa 'ID' lik kino topilmadi. Boshqa ID jo'nating 🙂");
            }

        }
        public async Task SearchByNameAndSend(Message message, string name)
        {
            var movie = _appDbContext.Movies.Where(x => x.Title.Contains(name.ToLower())).FirstOrDefault();
            long chatId = message.Chat.Id;

            if (movie != null)
            {
                await SendMovie(message, movie);
            }
            else
            {
                await _botClient.SendTextMessageAsync(chatId, "Bunaqa nomli kino topilmadi. Boshqa nom bilan yoki ID bilan qidirib ko'ring 🙂");
            }

        }
        public async Task SendMovie(Message message, Movie movie)
        {
            long chatId = message.Chat.Id;
            var videoId = movie.VideoId;
            var caption = movie.Description;
            var captionEntities = JsonConvert.DeserializeObject<List<MessageEntity>>(movie.DescriptionEntities);

            var inlineKeyboard = InlineKeyboards.GetOrqagaKeyboard();

            await _botClient.SendVideoAsync(chatId: chatId,
                caption: caption,
                captionEntities: captionEntities,
                video: InputFile.FromFileId(videoId),
                replyMarkup: inlineKeyboard
                );
        }
    }
}
