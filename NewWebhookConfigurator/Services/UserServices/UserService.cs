using FastApiWebhook.DbContexts;
using FastApiWebhook.Services.ButtonServices;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using FastApiWebhook.Services.ButtonServices;
using FastApiWebhook.Services.ButtonServices.BotKeyboards;
using Microsoft.VisualBasic;
using FastApiWebhook.Models;
using System.Runtime.InteropServices;

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
            if (message != null && message.Text != null)
            {

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
        public async Task SearchByNameAndSend (Message message, string name)
        {
            var movie = _appDbContext.Movies.Where(x=>  x.Title.Contains(name.ToLower())).FirstOrDefault();
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
        public async Task SendMovie (Message message, Movie movie)
        {
            long chatId =  message.Chat.Id;
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
