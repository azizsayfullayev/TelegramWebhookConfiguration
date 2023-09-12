using System.Diagnostics;
using Telegram.Bot;
using Telegram.Bot.Types;
using FastApiWebhook.DbContexts;
using FastApiWebhook.Models;
using FastApiWebhook.Services.ButtonServices;
using FastApiWebhook.Services.UserServices;
using MongoDB.Driver;

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
                    await _buttonService.MovieSearchIdUz(message);

                    TimeSpan elapsedTime = stopwatch.Elapsed;
                    await _botClient.SendTextMessageAsync(chatId: message.Chat.Id, elapsedTime.TotalSeconds.ToString());
                }
                else if (message.Text == "🇷🇺 Ru")
                {

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
                    if (message.From != null && message.Chat.Username != "")
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

    }
}
