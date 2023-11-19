using System;
using System.Data.Common;
using MySql.Data.MySqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

public class DatabaseService
{
    private readonly MySqlConnection dbConnection;
    private string operationMessage;
    public DatabaseService(string connectionString)
    {
        dbConnection = new MySqlConnection(connectionString);
    }
    public string GetOperationMessage()
    {
        return operationMessage;
    }

    public void InitializeDatabase()
    {
        try
        {
            dbConnection.Open();
            if (!CheckIfTableExists("users"))
            {
                CreateUsersTable();
                Console.WriteLine("Users table created successfully.");
            }
            else
            {
                Console.WriteLine("Users table already exists.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing database: {ex.Message}");
        }
        finally
        {
            dbConnection.Close();
        }
    }

    private bool CheckIfTableExists(string tableName)
    {
        using (var cmd = new MySqlCommand($"SHOW TABLES LIKE '{tableName}'", dbConnection))
        {
            return cmd.ExecuteScalar() != null;
        }
    }

    private string CreateUsersTable()
    {
        string query = @"
        CREATE TABLE users (
            id INT AUTO_INCREMENT PRIMARY KEY,
            user_id INT NOT NULL,
            name VARCHAR(255) NOT NULL,
            experience INT,
            ContatoTelegram VARCHAR(255),
            ContatoEmail VARCHAR(255),
            ContatoTelefonico VARCHAR(20),
            description TEXT
        )";

        try
        {
            using (var cmd = new MySqlCommand(query, dbConnection))
            {
                cmd.ExecuteNonQuery();
                operationMessage = "Tabelas criadas com sucesso.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating table: {ex.Message}");
            operationMessage = "Erro ao criar tabelas.";
        }
        finally
        {
            dbConnection.Close();
        }
        return operationMessage;
    }
    public void CreateFreelancer(long chatId, string name, string stacks, decimal experienceTime, string portfolio, string contactTelegram, string contactEmail, string contactPhone, string otherContacts)
    {
        try
        {
            dbConnection.Open();
            string query = "INSERT INTO Freelancer (UserTelegramID, Name, Stacks, ExperienceTime, Portfolio, ContactTelegram, ContactEmail, ContactPhone, OtherContacts, Status, Verified, LastUpdate, RegistrationDate, InactiveDate) VALUES (@user_id, @name, @stacks, @experience, @portfolio, @contatoTelegram, @contatoEmail, @contatoTelefonico, @otherContacts, 1, 0, NOW(), NOW(), NULL)";

            using MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@user_id", chatId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@stacks", stacks);
            cmd.Parameters.AddWithValue("@experience", experienceTime);
            cmd.Parameters.AddWithValue("@portfolio", portfolio);
            cmd.Parameters.AddWithValue("@contatoTelegram", contactTelegram);
            cmd.Parameters.AddWithValue("@contatoEmail", contactEmail);
            cmd.Parameters.AddWithValue("@contatoTelefonico", contactPhone);
            cmd.Parameters.AddWithValue("@otherContacts", otherContacts);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating freelancer: {ex.Message}");
        }
        finally
        {
            dbConnection.Close();
        }
    }

    public List<string> GetUsersList()
    {
        List<string> userList = new List<string>();

        try
        {
            dbConnection.Open();
            string query = "SELECT * from Freelancer";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var Status = reader["Status"].ToString() == "True" ? "Ativo" : "Inativo";

                    string userEntry = $"*Nome: * {reader["Name"]}\n" +
                                       $"*Stacks: * {reader["Stacks"]}\n" +
                                       $"*Experiência: * {reader["ExperienceTime"]} anos.\n" +
                                       $"*Portfolio: * {reader["Portfolio"].ToString().Replace(".", "\\.")}\n" +
                                       $"*Contato Telegram: * [{reader["Name"]}]({reader["ContactTelegram"].ToString().Replace(".", "\\.")})\n" +
                                       $"*Contato Email: * ||{reader["ContactEmail"].ToString().Replace(".", "\\.")}||\n" +
                                       $"*Contato Telefônico: * ||{reader["ContactPhone"]}||\n" +
                                       $"*Outros Contatos: * ||{reader["OtherContacts"]}||\n" +
                                       $"*Status: * {Status}\n" +
                                       $"*LastUpdate: * {reader["LastUpdate"]}\n" +
                                       $"*RegistrationDate: * {reader["RegistrationDate"]}\n" +
                                       $"*InactiveDate: * {reader["InactiveDate"]}\n\n";


                    userList.Add(userEntry);
                }
            }

            return userList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            // Se ocorrer um erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return new List<string>();
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public object VerificarEstadoMenu(long telegramUserID, string Command)
    {
        try
        {
            dbConnection.Open();
            // Consultar o estado do usuário na tabela UserState
            string query = "SELECT * FROM UserState WHERE TelegramUserID = @UserID";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@UserID", telegramUserID);

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read() && !Command.StartsWith("/listFreelancers")) // Se o usuário existir na tabela
            {
                // Continue de onde parou, atualizando as informações de acordo com o choice
                var CurrentRegistration = reader["CurrentRegistration"];

                return CurrentRegistration;
            }
            else if (Command.StartsWith("/includeFreelancer")) // Se o usuário não existir na tabela
            {
                dbConnection.Close();
                dbConnection.Open();
                // Comece do 0 e insira um novo registro na tabela UserState com os valores iniciais
                string insertQuery = $"INSERT INTO UserState (TelegramUserID, CurrentRegistration, LastUpdate) VALUES (@UserID, '{Command}', NOW())";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, dbConnection);
                insertCommand.Parameters.AddWithValue("@UserID", telegramUserID);
                insertCommand.ExecuteNonQuery();

                return Command;
            }
            else
            {
                return Command;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            // Se ocorrer um erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return ex.Message;
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public object VerificarEstado(long telegramUserID)
    {
        try
        {
            dbConnection.Open();
            // Consultar o estado do usuário na tabela UserState
            string query = "SELECT * FROM UserState WHERE TelegramUserID = @UserID";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@UserID", telegramUserID);

            MySqlDataReader reader = cmd.ExecuteReader();



            if (reader.Read()) // Se o usuário existir na tabela
            {
                // Continue de onde parou, atualizando as informações de acordo com o choice
                int freelancerState = Convert.ToInt32(reader["FreelancerState"]);

                return freelancerState;
            }
            else // Se o usuário não existir na tabela
            {
                dbConnection.Close();
                dbConnection.Open();
                // Comece do 0 e insira um novo registro na tabela UserState com os valores iniciais
                string insertQuery = "INSERT INTO UserState (TelegramUserID, LastUpdate) VALUES (@UserID, NOW())";
                MySqlCommand insertCommand = new MySqlCommand(insertQuery, dbConnection);
                insertCommand.Parameters.AddWithValue("@UserID", telegramUserID);
                insertCommand.ExecuteNonQuery();

                return 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            // Se ocorrer um erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return ex.Message;
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public object AtualizarEstado(long telegramUserID, int novoEstado, string Coluna)
    {
        try
        {
            dbConnection.Open();
            // Consultar o estado do usuário na tabela UserState
            string updateQuery = $"UPDATE UserState SET {Coluna} = @novoEstado, LastUpdate = NOW() WHERE TelegramUserID = @UserID";
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, dbConnection);
            updateCommand.Parameters.AddWithValue("@novoEstado", novoEstado);
            updateCommand.Parameters.AddWithValue("@UserID", telegramUserID);
            updateCommand.ExecuteNonQuery();

            return "ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            // Se ocorrer um erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return ex.Message;
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public object AtualizarFreelancer(long telegramUserID, object Dado, string Coluna)
    {
        try
        {
            dbConnection.Open();
            // Consultar o estado do usuário na tabela UserState
            string updateQuery = $"UPDATE Freelancer SET {Coluna} = '{Dado}', LastUpdate = NOW() WHERE UserTelegramID = @UserID";
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, dbConnection);
            updateCommand.Parameters.AddWithValue("@UserID", telegramUserID);
            updateCommand.ExecuteNonQuery();

            return "ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            // Se ocorrer um erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return ex.Message;
        }
        finally
        {
            dbConnection.Close();
        }
    }// Função para adicionar barra invertida aos caracteres entre 1 e 126
    string AddBackslash(object data)
    {
        if (data != null)
        {
            string originalString = data.ToString();
            string result = "";
            foreach (char c in originalString)
            {
                if (c >= 1 && c <= 126)
                {
                    result += '\\' + c;
                }
                else
                {
                    result += c;
                }
            }
            return result;
        }
        return "";
    }
}
