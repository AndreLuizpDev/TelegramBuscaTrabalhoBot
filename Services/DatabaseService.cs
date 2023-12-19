using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Relational;
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
    public string CreateTablesFromSQLFile()
    {
        string operationMessage = string.Empty;

        try
        {
            dbConnection.Open();
            string filePath = "C:/repo/TelegramBuscaTrabalhoBot/Data/tabelas.sql"; // Substitua pelo caminho correto do seu arquivo
            string query = File.ReadAllText(filePath); // Lê todo o conteúdo do arquivo SQL
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            MySqlDataReader reader = cmd.ExecuteReader();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating tables: {ex.Message}");
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
                                       $"*Portfolio: * {reader["Portfolio"]}\n" +
                                       $"*Contato Telegram: * [{reader["Name"]}]({reader["ContactTelegram"]})\n" +
                                       $"*Contato Email: * ||{reader["ContactEmail"]}||\n" +
                                       $"*Contato Telefônico: * ||{reader["ContactPhone"]}||\n" +
                                       $"*Outros Contatos: * ||{reader["OtherContacts"]}||\n" +
                                       $"*Status: * {Status}\n" +
                                       $"*Última Atualização: * {reader["LastUpdate"]}\n" +
                                       $"*Data de Registro: * {reader["RegistrationDate"]}\n" +
                                       $"*Data Inativação: * {reader["InactiveDate"]}\n\n";

                    userEntry = AdicionarEscape(userEntry);

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
    public List<string> GetUsers(long chatId)
    {
        List<string> userList = new List<string>();
        long UserTelegram = chatId;

        try
        {
            dbConnection.Open();
            string query = $"SELECT * FROM Freelancer WHERE UserTelegramID = {UserTelegram}";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var Status = reader["Status"].ToString() == "True" ? "Ativo" : "Inativo";

                    string userEntry = $"*Nome: * {reader["Name"]}\n" +
                                       $"*Stacks: * {reader["Stacks"]}\n" +
                                       $"*Experiência: * {reader["ExperienceTime"]} anos.\n" +
                                       $"*Portfolio: * {reader["Portfolio"]}\n" +
                                       $"*Contato Telegram: * [{reader["Name"]}]({reader["ContactTelegram"]})\n" +
                                       $"*Contato Email: * ||{reader["ContactEmail"]}||\n" +
                                       $"*Contato Telefônico: * ||{reader["ContactPhone"]}||\n" +
                                       $"*Outros Contatos: * ||{reader["OtherContacts"]}||\n" +
                                       $"*Status: * {Status}\n" +
                                       $"*Última Atualização: * {reader["LastUpdate"]}\n" +
                                       $"*Data de Registr/o: * {reader["RegistrationDate"]}\n" +
                                       $"*Data Inativação: * {reader["InactiveDate"]}\n\n";

                    userEntry = AdicionarEscape(userEntry);

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
    public List<string> GetCompanyList()
    {
        List<string> companyList = new List<string>();

        try
        {
            dbConnection.Open();
            string query = "SELECT * FROM Company";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var Status = reader["Status"].ToString() == "True" ? "Ativo" : "Inativo";

                    string companyEntry = $"*Nome: * {reader["Name"]}\n" +
                                          $"*Estado: * {reader["State"]}\n" +
                                          $"*País: * {reader["Country"]}\n" +
                                          $"*Contato Telegram: * [{reader["Name"]}]({reader["ContactTelegram"]})\n" +
                                          $"*Contato Email: * ||{reader["ContactEmail"]}||\n" +
                                          $"*Contato Telefônico: * ||{reader["ContactPhone"]}||\n" +
                                          $"*Outros Contatos: * ||{reader["OtherContacts"]}||\n" +
                                          $"*Status: * {Status}\n" +
                                          $"*Última Atualização: * {reader["LastUpdate"]}\n" +
                                          $"*Data de Registro: * {reader["RegistrationDate"]}\n" +
                                          $"*Data de Inativação: * {reader["InactiveDate"]}\n\n";

                    companyEntry = AdicionarEscape(companyEntry);

                    companyList.Add(companyEntry);
                }
            }
            return companyList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving company list: {ex.Message}");
            // Em caso de erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
            return new List<string>();
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public List<string> GetCompany(long chatId)
    {
        List<string> companyList = new List<string>();
        long UserTelegram = chatId;

        try
        {
            dbConnection.Open();
            string query = $"SELECT * FROM Company WHERE UserTelegramID = {UserTelegram}";
            MySqlCommand cmd = new MySqlCommand(query, dbConnection);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    var Status = reader["Status"].ToString() == "True" ? "Ativo" : "Inativo";

                    string companyEntry = $"*Nome: * {reader["Name"]}\n" +
                                          $"*Estado: * {reader["State"]}\n" +
                                          $"*País: * {reader["Country"]}\n" +
                                          $"*Contato Telegram: * [{reader["Name"]}]({reader["ContactTelegram"]})\n" +
                                          $"*Contato Email: * ||{reader["ContactEmail"]}||\n" +
                                          $"*Contato Telefônico: * ||{reader["ContactPhone"]}||\n" +
                                          $"*Outros Contatos: * ||{reader["OtherContacts"]}||\n" +
                                          $"*Status: * {Status}\n" +
                                          $"*Última Atualização: * {reader["LastUpdate"]}\n" +
                                          $"*Data de Registro: * {reader["RegistrationDate"]}\n" +
                                          $"*Data de Inativação: * {reader["InactiveDate"]}\n\n";

                    companyEntry = AdicionarEscape(companyEntry);

                    companyList.Add(companyEntry);
                }
            }
            return companyList;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving company list: {ex.Message}");
            // Em caso de erro, você pode retornar uma lista vazia ou lidar com isso de outra maneira
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

            if (reader.Read()) // Se o usuário existir na tabela
            {
                // Continue de onde parou, atualizando as informações de acordo com o choice
                var CurrentRegistration = reader["CurrentRegistration"];

                if (CurrentRegistration == null || string.IsNullOrWhiteSpace(CurrentRegistration.ToString()))
                {
                    CurrentRegistration = Command;
                }

                return CurrentRegistration;
            }
            else if (Command.StartsWith("/include")) // Se o usuário não existir na tabela
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
    public object VerificarEstado(long telegramUserID, String Coluna)
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
                int userState = Convert.ToInt32(reader[Coluna]);

                return userState;
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
    public object AtualizarEstado(long telegramUserID, object novoEstado, string Coluna)
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
    }
    public void CreateCompany(long chatId, string name, string state, string country, string contactTelegram, string contactEmail, string contactPhone, string otherContacts)
    {
        try
        {
            dbConnection.Open();
            string query = "INSERT INTO Company (UserTelegramID, Name, State, Country, ContactTelegram, ContactEmail, ContactPhone, OtherContacts, Status, Verified, LastUpdate, RegistrationDate, InactiveDate) VALUES (@user_id, @name, @state, @country, @contactTelegram, @contactEmail, @contactPhone, @otherContacts, 1, 0, NOW(), NOW(), NULL)";

            using MySqlCommand cmd = new MySqlCommand(query, dbConnection);
            cmd.Parameters.AddWithValue("@user_id", chatId);
            cmd.Parameters.AddWithValue("@name", name);
            cmd.Parameters.AddWithValue("@state", state);
            cmd.Parameters.AddWithValue("@country", country);
            cmd.Parameters.AddWithValue("@contactTelegram", contactTelegram);
            cmd.Parameters.AddWithValue("@contactEmail", contactEmail);
            cmd.Parameters.AddWithValue("@contactPhone", contactPhone);
            cmd.Parameters.AddWithValue("@otherContacts", otherContacts);

            cmd.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating company: {ex.Message}");
        }
        finally
        {
            dbConnection.Close();
        }
    }
    public object AtualizarCompany(long telegramUserID, object data, string column)
    {
        try
        {
            dbConnection.Open();
            // Consulta para atualizar a tabela Company com base na coluna fornecida
            string updateQuery = $"UPDATE Company SET {column} = @Data, LastUpdate = NOW() WHERE UserTelegramID = @UserID";
            MySqlCommand updateCommand = new MySqlCommand(updateQuery, dbConnection);
            updateCommand.Parameters.AddWithValue("@Data", data);
            updateCommand.Parameters.AddWithValue("@UserID", telegramUserID);
            updateCommand.ExecuteNonQuery();

            return "ok";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating company data: {ex.Message}");
            // Em caso de erro, você pode retornar uma mensagem específica ou lidar com isso de outra forma
            return ex.Message;
        }
        finally
        {
            dbConnection.Close();
        }
    }
    static string AdicionarEscape(string input)
    {
        string pattern = @"[_>#+\-={}.!]";
        string replacement = @"\$0";
        string novaString = Regex.Replace(input, pattern, replacement);

        return novaString;
    }
    public void excluirCadastroIndividual(long telegramUserID)
    {
        try
        {
            dbConnection.Open();

            string deleteQuery = $"DELETE FROM Freelancer WHERE UserTelegramID = @UserID";
            MySqlCommand updateCommand = new MySqlCommand(deleteQuery, dbConnection);
            updateCommand.Parameters.AddWithValue("@UserID", telegramUserID);
            updateCommand.ExecuteNonQuery();

            deleteQuery = $"DELETE FROM UserState WHERE TelegramUserID = @UserID";
            updateCommand = new MySqlCommand(deleteQuery, dbConnection);
            updateCommand.Parameters.AddWithValue("@UserID", telegramUserID);
            updateCommand.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving user list: {ex.Message}");
        }
        finally
        {
            dbConnection.Close();
        }
    }
}
