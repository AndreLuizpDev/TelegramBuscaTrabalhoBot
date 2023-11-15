using MySql.Data.MySqlClient;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

class Program
{
    private static DatabaseService databaseService;
    static async Task Main(string[] args)
    {
        var botClient = new TelegramBotClient(ConfigurationManager.AppSettings["BotToken"]);
        var connectionString = ConfigurationManager.AppSettings["TelegramBotConnection"];
        var dbConnection = new MySqlConnection(connectionString);

        databaseService = new DatabaseService(connectionString);
        databaseService.InitializeDatabase();

        using CancellationTokenSource cts = new();

        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = Array.Empty<UpdateType>()
        };

        botClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            pollingErrorHandler: HandlePollingErrorAsync,
            receiverOptions: receiverOptions,
            cancellationToken: cts.Token
        );

        var me = await botClient.GetMeAsync();

        Console.WriteLine($"Listening @{me.Username}");
        Console.WriteLine($"Connection @{dbConnection}");
        Console.ReadLine();

        cts.Cancel();
    }
    public static List<string> GetAvailableCommands()
    {
        List<string> commands = new List<string>
    {
        "*/start* - Start the bot",
        "*/includeFreelancer* <Name> <Experience> <Description> - Add a new user",
        "*/listFreelancers* - List all Freelancers"
    };

        return commands;
    }
    async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        if (update.Message is not { } message)
            return;

        var chatId = message.Chat.Id;
        var messageText = message.Text;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (messageText != null)
        {
            if (messageText.StartsWith("/start"))
            {
                List<string> availableCommands = GetAvailableCommands();
                string response = "Available commands:\n" + string.Join("\n", availableCommands);
                await botClient.SendTextMessageAsync(message.Chat.Id, response, parseMode: ParseMode.Markdown);
            }
            else if (messageText.StartsWith("/includeFreelancer"))
            {
                var parts = messageText.Split(' ', 4); // Split the message into parts
                if (parts.Length < 4)
                {
                    await botClient.SendTextMessageAsync(chatId, "Invalid format. Please use: /createUser <name> <experience> <description>");
                    return;
                }

                var name = parts[1];
                var experience = int.Parse(parts[2]);
                var description = parts[3];

                databaseService.CreateUser(name, experience, description);
                await botClient.SendTextMessageAsync(chatId, "User created successfully.");
            }
            else if (messageText.StartsWith("/listFreelancers"))
            {
                string userList = databaseService.GetUsersList();
                await botClient.SendTextMessageAsync(chatId, userList, parseMode: ParseMode.Markdown);
            }
        }
    }

    static Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
}
