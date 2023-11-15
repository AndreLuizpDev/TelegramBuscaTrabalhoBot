using MySql.Data.MySqlClient;
using System.Configuration;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
class Program
{
    private static string connectionString = ConfigurationManager.AppSettings["TelegramBotConnection"];
    private static DatabaseService databaseService = new DatabaseService(connectionString);    
    private static TelegramBotClient botClient = new TelegramBotClient(ConfigurationManager.AppSettings["BotToken"]);
    private static CancellationTokenSource cts = new CancellationTokenSource();
    private static ReceiverOptions receiverOptions;

    static async Task Main(string[] args)
    {
        var dbConnection = new MySqlConnection(connectionString);
        databaseService = new DatabaseService(connectionString);
        databaseService.InitializeDatabase();

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
        "*/includeFreelancer* <Name>;<Experience>;<ContatoTelegram>;<ContatoEmail>;<ContatoTelefonico>;<Description> - Add a new user",
        "*/listFreelancers* - List all Freelancers"
    };

        return commands;
    }
    async static Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var factory = new ConnectionFactory() { Uri = new Uri(ConfigurationManager.AppSettings["CloudAMQPUrl"]) };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        if (update.Message is not { } message)
            return;

        var chatId = message.Chat.Id;
        var messageText = message.Text;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        if (messageText != null)
        {

            List<string> availableCommands = GetAvailableCommands();
            string response = "Available commands:\n" + string.Join("\n", availableCommands);

            if (messageText.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(chatId, "Welcome to our Job Search Bot! 🤖✨\r\nExplore opportunities, add your freelancer profile, and connect with other users. Use commands like \r\n/includeFreelancer or /listFreelancers to get started. 🚀💼\r\n");
            }
            else if (messageText.StartsWith("/includeFreelancer"))
            {
                var parts = messageText.Split(';', StringSplitOptions.RemoveEmptyEntries); // Split the message into parts
                if (parts.Length < 6)
                {
                    await botClient.SendTextMessageAsync(chatId, "Invalid format. Please use: /includeFreelancer <name>;<experience>;<ContatoTelegram>;<ContatoEmail>;<ContatoTelefonico>;<description>");
                    return;
                }

                var name = parts[0].Replace("/includeFreelancer ", "");
                var experience = int.Parse(parts[1]);
                var contatoTelegram = parts[2];
                var contatoEmail = parts[3];
                var contatoTelefonico = parts[4];
                var description = parts[5];

                databaseService.CreateUser(chatId, name, experience, contatoTelegram, contatoEmail, contatoTelefonico, description);
                await botClient.SendTextMessageAsync(chatId, "User created successfully.");

                var body = Encoding.UTF8.GetBytes($"{chatId};{name};{experience};{contatoTelegram};{contatoEmail};{contatoTelefonico};{description}");
                channel.BasicPublish(exchange: "",
                                     routingKey: "UserQueue",
                                     basicProperties: null,
                                     body: body);
            }
            else if (messageText.StartsWith("/listFreelancers"))
            {

                List<string> userList = databaseService.GetUsersList();

                // Enviar cada item da lista como uma mensagem separada
                foreach (var user in userList)
                {
                    await botClient.SendTextMessageAsync(chatId, user, parseMode: ParseMode.Markdown);
                    // Aguarde um curto período de tempo entre as mensagens para evitar limites do Telegram
                    await Task.Delay(500);
                }
            }
            else
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, response, parseMode: ParseMode.Markdown);
            }
        }

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body;
            var message = Encoding.UTF8.GetString(body.ToArray());
            Console.WriteLine("Received from CloudAMQP: {0}", message);
        };
        channel.BasicConsume(queue: "UserQueue",
                             autoAck: true,
                             consumer: consumer);

        await Task.Delay(1000);    
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