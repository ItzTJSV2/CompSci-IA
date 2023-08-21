using Microsoft.Data.Sqlite;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.Data;
using System.Data.SqlClient;

namespace Comp_Sci_IA_Main_Proj_.Pages.Accounts
{
    public class CheckDB
    {
        public static string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
        public static string Dir = Path.Combine(parentDir, @"Comp Sci IA Main Proj", @"database", @"Database.db");
        public static string ConnectionString = ("Data Source=" + Dir);

        public int CheckExist(string Search)
        {
            Console.WriteLine(ConnectionString);
            using (var connection = new SqliteConnection(ConnectionString))
            {
                connection.Open();
                var command = connection.CreateCommand();

                string queryStr1 = "SELECT COUNT(*) FROM dbo.Users WHERE Username = @String";
                string queryStr2 = "SELECT COUNT(*) FROM dbo.Users WHERE Email = @String";

                command.CommandText = queryStr1;
                command.Parameters.Add("@String", SqliteType.Text).Value = Search;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int count = (int)command.ExecuteScalar();
                        connection.Close();
                        if (count > 0)
                        {
                            return 1;  // FOUND IN USERNAME
                        }
                    }
                }
                command.CommandText = queryStr2;
                command.Parameters.Add("@String", SqliteType.Text).Value = Search;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int count = (int)command.ExecuteScalar();
                        connection.Close();
                        if (count > 0)
                        {
                            return 2;  // FOUND IN EMAIL
                        }
                    }
                }
            }
            return 0; // VALUE EXISTS
        }

        public List<string> getAccount(string Input)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                List<string> Account = new List<string>();
                string queryStr1 = "SELECT * FROM dbo.UserTable WHERE Username = @String";
                string queryStr2 = "SELECT * FROM dbo.UserTable WHERE Email = @String";
                if (CheckExist(Input) == 1)
                {
                    using(SqlCommand command = new SqlCommand(queryStr1, connection))
                    {
                        command.Parameters.Add("@String", SqlDbType.NVarChar).Value = Input;

                        connection.Open();

                        using(SqlDataReader reader = command.ExecuteReader())
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
                } else if (CheckExist(Input) == 2)
                {
                    using (SqlCommand command = new SqlCommand(queryStr2, connection))
                    {
                        command.Parameters.Add("@String", SqlDbType.NVarChar).Value = Input;

                        connection.Open();

                        using (SqlDataReader reader = command.ExecuteReader())
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
                    Console.WriteLine("Something has gone very wrong...");
                    return null;
                }
            }
        }

    }
}
