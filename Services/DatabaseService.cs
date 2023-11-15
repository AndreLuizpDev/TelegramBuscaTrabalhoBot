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
                description TEXT
            )";
        
        using (var cmd = new MySqlCommand(query, dbConnection))
        {
            cmd.ExecuteNonQuery();
            operationMessage = "Tabelas criadas com sucesso.";
        }
        return operationMessage;
    }
    public string CreateUser(string name, int experience, string description)
    {
        try
        {
            dbConnection.Open();
            string query = "INSERT INTO users (name, experience, description) VALUES (@name, @experience, @description)";
            using MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@experience", experience);
            cmd.Parameters.AddWithValue("@description", description);
            cmd.ExecuteNonQuery();
            operationMessage = "Usuário cadastrado com sucesso!";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating user: {ex.Message}");
            operationMessage = "Erro ao criar tabela.";
        }
        finally
        {
            dbConnection.Close();
        }
        return operationMessage;
    }

    public string GetUsersList()
    {
        try
        {
            dbConnection.Open();
            string query = "SELECT name, experience, description FROM users";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                string userList = "*Relação de Freelancers:*\n\n";
                while (reader.Read())
                {
                    userList += $"*Nome: * {reader["name"]}\n" +
                                $"*Experiência: * {reader["experience"]}\n" +
                                $"*Descrição: * {reader["description"]}\n\n";

                }
                return userList;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
            return "An error occurred while retrieving the user list.";
        }
        finally
        {
            dbConnection.Close();
        }
    }
}
