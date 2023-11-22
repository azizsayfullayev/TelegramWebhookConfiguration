using FastApiWebhook.Controllers;
using FastApiWebhook.DbContexts;
using FastApiWebhook.Services;
using FastApiWebhook.Services.AdminServices;
using FastApiWebhook.Services.ButtonServices;
using FastApiWebhook.Services.ButtonServices.BotKeyboards;
using FastApiWebhook.Services.UserServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Serilog;
using Telegram.Bot;


var builder = WebApplication.CreateBuilder(args);

// Setup Bot configuration
var botConfigurationSection = builder.Configuration.GetSection(BotConfiguration.Configuration);
builder.Services.Configure<BotConfiguration>(botConfigurationSection);

var botConfiguration = botConfigurationSection.Get<BotConfiguration>();

// Register named HttpClient to get benefits of IHttpClientFactory
// and consume it with ITelegramBotClient typed client.
// More read:
//  https://docs.microsoft.com/en-us/aspnet/core/fundamentals/http-requests#typed-clients
//  https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/use-httpclientfactory-to-implement-resilient-http-requests
builder.Services.AddHttpClient("telegram_bot_client")
                .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
                {
                    BotConfiguration? botConfig = sp.GetConfiguration<BotConfiguration>();
                    TelegramBotClientOptions options = new(botConfig.BotToken);
                    return new TelegramBotClient(options, httpClient);
                });

// Dummy business-logic service
builder.Services.AddScoped<UpdateHandlers>();

// There are several strategies for completing asynchronous tasks during startup.
// Some of them could be found in this article https://andrewlock.net/running-async-tasks-on-app-startup-in-asp-net-core-part-1/
// We are going to use IHostedService to add and later remove Webhook
builder.Services.AddHostedService<ConfigureWebhook>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IButtonService, ButtonService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IInlineQueries, InlineQueries>();
//Serilog
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
   loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

var connectionString = builder.Configuration.GetConnectionString("PostgresDevelopmentDb");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var connectionStringx = builder.Configuration.GetConnectionString("ConnectionStringsx.MongoDB");
var databaseName = builder.Configuration.GetSection("ConnectionStringsx.DatabaseName").Value;

// Register the MongoDB client and database with a scoped lifetime
builder.Services.AddScoped<IMongoClient>(provider => new MongoClient("mongodb+srv://testuser:BNtPhqLY0ZCkanoZ@firstcluster.g1cbdud.mongodb.net/?retryWrites=true&w=majority"));
builder.Services.AddScoped<IMongoDatabase>(provider =>
{
    var client = provider.GetRequiredService<IMongoClient>();
    return client.GetDatabase("MovieBot");
});
// The Telegram.Bot library heavily depends on Newtonsoft.Json library to deserialize
// incoming webhook updates and send serialized responses back.
// Read more about adding Newtonsoft.Json to ASP.NET Core pipeline:
//   https://docs.microsoft.com/en-us/aspnet/core/web-api/advanced/formatting?view=aspnetcore-6.0#add-newtonsoftjson-based-json-format-support
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

var app = builder.Build();
// Construct webhook route from the Route configuration parameter
// It is expected that BotController has single method accepting Update
app.MapBotWebhookRoute<BotController>(route: botConfiguration.Route);
app.MapControllers();
app.Run();

#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
public class BotConfiguration
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
{
    public static readonly string Configuration = "BotConfiguration";

    public string BotToken { get; init; } = default!;
    public string HostAddress { get; init; } = default!;
    public string Route { get; init; } = default!;
    public string SecretToken { get; init; } = default!;
}
