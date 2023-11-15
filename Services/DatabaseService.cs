using System;
using System.Data.Common;
using MySql.Data.MySqlClient;

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
        return operationMessage;
    }
    public void CreateUser(long chatId, string name, int experience, string contatoTelegram, string contatoEmail, string contatoTelefonico, string description)
    {
        try
        {
            dbConnection.Open();
            string query = "INSERT INTO users (user_id, name, experience, ContatoTelegram, ContatoEmail, ContatoTelefonico, description) VALUES (@user_id, @name, @experience, @contatoTelegram, @contatoEmail, @contatoTelefonico, @description)";

            using MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@user_id", chatId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@experience", experience);
            cmd.Parameters.AddWithValue("@contatoTelegram", contatoTelegram);
            cmd.Parameters.AddWithValue("@contatoEmail", contatoEmail);
            cmd.Parameters.AddWithValue("@contatoTelefonico", contatoTelefonico);
            cmd.Parameters.AddWithValue("@description", description);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
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
            string query = "SELECT name, experience, ContatoTelegram, ContatoEmail, ContatoTelefonico, description FROM users";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    string userEntry = $"*Nome: * {reader["name"]}\n" +
                                       $"*Experiência: * {reader["experience"]}\n" +
                                       $"*Contato Telegram: * {reader["ContatoTelegram"]}\n" +
                                       $"*Contato Email: * {reader["ContatoEmail"]}\n" +
                                       $"*Contato Telefônico: * {reader["ContatoTelefonico"]}\n" +
                                       $"*Descrição: * {reader["description"]}\n\n";

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
}
