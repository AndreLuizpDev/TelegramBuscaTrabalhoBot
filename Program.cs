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
using System.Xml.Linq;

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

            var Command = databaseService.VerificarEstadoMenu(chatId, messageText).ToString();

            if (Command.StartsWith("/start"))
            {
                await botClient.SendTextMessageAsync(chatId, "Welcome to our Job Search Bot! 🤖✨\r\nExplore opportunities, add your freelancer profile, and connect with other users. Use commands like \r\n/includeFreelancer or /listFreelancers to get started. 🚀💼\r\n");
            }
            else if (Command.StartsWith("/includeFreelancer"))
            {
                var userState = databaseService.VerificarEstado(chatId);

                var name = ""; var stacks = ""; var portfolio = ""; var contactTelegram = "";
                var contactEmail = ""; var contactPhone = ""; var otherContacts = ""; decimal experienceTime = 0;
                switch (userState)
                {
                    case 1:
                        await botClient.SendTextMessageAsync(chatId, "Digite seu nome: ");
                        databaseService.AtualizarEstado(chatId, 2, "FreelancerState");
                        break;
                    case 2:
                        name = messageText;
                        databaseService.CreateFreelancer(chatId, name, stacks, experienceTime, portfolio, contactTelegram, contactEmail, contactPhone, otherContacts);
                        await botClient.SendTextMessageAsync(chatId, "Stacks (separadas por vírgula): ");
                        databaseService.AtualizarEstado(chatId, 3, "FreelancerState");
                        break;
                    case 3:
                        stacks = messageText;
                        databaseService.AtualizarFreelancer(chatId, stacks, "Stacks");
                        await botClient.SendTextMessageAsync(chatId, "Tempo de experiência: ");
                        databaseService.AtualizarEstado(chatId, 4, "FreelancerState");
                        break;
                    case 4:
                        if (decimal.TryParse(messageText, out experienceTime))
                        {
                            databaseService.AtualizarFreelancer(chatId, experienceTime, "ExperienceTime");
                            await botClient.SendTextMessageAsync(chatId, "Portfolio: ");
                            databaseService.AtualizarEstado(chatId, 5, "FreelancerState");
                        }
                        else
                        {
                            Console.WriteLine("Por favor, insira um número válido para o tempo de experiência.");
                            await botClient.SendTextMessageAsync(chatId, "Tempo de experiência: ");
                        }
                        break;
                    case 5:
                        portfolio = messageText;
                        databaseService.AtualizarFreelancer(chatId, portfolio, "Portfolio");
                        await botClient.SendTextMessageAsync(chatId, "Número de Contato (Formato: 5511999999999): ");
                        databaseService.AtualizarEstado(chatId, 6, "FreelancerState");
                        break;
                    case 6:
                        // Aqui você pode lidar com a lógica para capturar o telefone e definir a variável contactTelegram
                        contactTelegram = "t.me/+" + messageText;
                        databaseService.AtualizarFreelancer(chatId, contactTelegram, "ContactTelegram");
                        await botClient.SendTextMessageAsync(chatId, "Email de contato: ");
                        databaseService.AtualizarEstado(chatId, 7, "FreelancerState");
                        break;
                    case 7:
                        contactEmail = messageText;
                        databaseService.AtualizarFreelancer(chatId, contactEmail, "ContactEmail");
                        await botClient.SendTextMessageAsync(chatId, "Telefone de contato: ");
                        databaseService.AtualizarEstado(chatId, 8, "FreelancerState");
                        break;
                    case 8:
                        contactPhone = messageText;
                        databaseService.AtualizarFreelancer(chatId, contactPhone, "ContactPhone");
                        await botClient.SendTextMessageAsync(chatId, "Outros contatos: ");
                        databaseService.AtualizarEstado(chatId, 9, "FreelancerState");
                        break;
                    case 9:
                        otherContacts = messageText;
                        databaseService.AtualizarFreelancer(chatId, otherContacts, "OtherContacts");
                        // Aqui você tem todas as informações necessárias para criar o usuário ou realizar ação final
                        await botClient.SendTextMessageAsync(chatId, "Usuário criado com sucesso.");
                        databaseService.AtualizarEstado(chatId, 10, "FreelancerState");
                        break;
                    case 10:
                        await botClient.SendTextMessageAsync(chatId, "Welcome to our Job Search Bot! 🤖✨\r\nExplore opportunities, add your freelancer profile, and connect with other users. Use commands like \r\n/includeFreelancer or /listFreelancers to get started. 🚀💼\r\n");
                        break;
                }
            }
            else if (Command.StartsWith("/listFreelancers"))
            {

                List<string> userList = databaseService.GetUsersList();

                // Enviar cada item da lista como uma mensagem separada
                foreach (var user in userList)
                {
                    Console.WriteLine(user);
                    await botClient.SendTextMessageAsync(chatId, user, parseMode: ParseMode.MarkdownV2);
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