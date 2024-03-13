using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Security.Claims;

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class CheckDB
    {
        public static string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
        public static string Dir = Path.Combine(parentDir, @"Mainproject", @"database", @"Database.db");
        public static string ConnectionString = ("Data Source=" + Dir);


        public async void DelAccount(int AccId)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = $"DELETE FROM UserTable WHERE AccountID = @Id";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Id", AccId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public async void ChangePass(int AccId, string Password)
        {
            using (var connection = new SQLiteConnection(ConnectionString)) 
            {
                connection.Open();

                string query = $"UPDATE UserTable SET Password = @Pass WHERE AccountID = @Id";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Pass", Password);
                    command.Parameters.AddWithValue("@Id", AccId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public async void ChangeField(int AccId, string Username, string Email, string Phone)
        {
            using (var connection = new SQLiteConnection(ConnectionString)) 
            {
                connection.Open();
                
                string query = $"UPDATE UserTable SET Username = @Username, Email  = @Email, PhoneNumber = @Phone WHERE AccountID = @Id";
                using (var command = new SQLiteCommand(connection)) 
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Username", Username);
                    command.Parameters.AddWithValue("@Email", Email);
                    command.Parameters.AddWithValue("@Phone", Phone);
                    command.Parameters.AddWithValue("@Id", AccId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public int CheckExist(string Search)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string queryStr1 = "SELECT COUNT(*) FROM UserTable WHERE Username = @String COLLATE NOCASE";
                string queryStr2 = "SELECT COUNT(*) FROM UserTable WHERE Email = @String COLLATE NOCASE";
                int count = 0;
                // Username
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = queryStr1;
                    command.Parameters.AddWithValue("@String", Search);

                    count = Convert.ToInt32(command.ExecuteScalar());
                    connection.Close();
                }
                if (count > 0)
                {
                    return 1;  // FOUND IN USERNAME
                }
                connection.Open();
                // Email
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = queryStr2;
                    command.Parameters.AddWithValue("@String", Search);

                    count = Convert.ToInt32(command.ExecuteScalar());
                }
                connection.Close();
                if (count > 0)
                {
                    return 2;  // FOUND IN EMAIL
                }
            }
            return 0; // VALUE DOESN'T EXISTS
        }

        public string GetName (int accountID)
        {
            string userName = "";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string queryStr = "SELECT * FROM UserTable WHERE AccountID = @Id";
                using (SQLiteCommand command = new SQLiteCommand(queryStr, connection))
                {
                    command.Parameters.AddWithValue("@Id", accountID);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            userName = reader.GetString(1);
                        }
                    }
                }
            }

            return userName;
        }
        public string getUserTripString(string userName)
        {
            string tripString = "";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string queryStr = "SELECT * FROM UserTable WHERE Username = @UserName";
                using (SQLiteCommand command = new SQLiteCommand(queryStr, connection))
                {
                    command.Parameters.AddWithValue("@UserName", userName);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tripString = reader.GetString(5);
                        }
                    }
                }
            }

            return tripString;
        }
        public void changeUserTrips(string userName, string newStopString)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = $"UPDATE UserTable SET Trips = @TripString WHERE Username = @Username";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Username", userName);
                    command.Parameters.AddWithValue("@TripString", newStopString);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public List<string> getAccount(string Input)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                List<string> Account = new List<string>();
                string queryStr1 = "SELECT * FROM UserTable WHERE Username = @String";
                string queryStr2 = "SELECT * FROM UserTable WHERE Email = @String";
                if (CheckExist(Input) == 1)
                {
                    using(SQLiteCommand command = new SQLiteCommand(queryStr1, connection))
                    {
                        command.Parameters.AddWithValue("@String", Input);

                        connection.Open();

                        using(var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Account.Add(reader.GetInt32(0).ToString());
                                Account.Add(reader.GetString(1));
                                Account.Add(reader.GetString(2));
                                Account.Add(reader.GetString(3));
                                Account.Add(reader.GetString(4));
                                if (!reader.IsDBNull(5))
                                {
                                    Account.Add(reader.GetString(5));
                                } else
                                {
                                    Account.Add("");
                                }
                                
                            }
                        }
                    }
                    return Account;
                } else if (CheckExist(Input) == 2)
                {
                    using (SQLiteCommand command = new SQLiteCommand(queryStr2, connection))
                    {
                        command.Parameters.AddWithValue("@String", Input);

                        connection.Open();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Account.Add(reader.GetInt32(0).ToString());
                                Account.Add(reader.GetString(1));
                                Account.Add(reader.GetString(2));
                                Account.Add(reader.GetString(3));
                                Account.Add(reader.GetString(4));
                            }
                        }
                    }
                    return Account;
                } else
                {
                    return null;
                }
            }
        }

    }
}
