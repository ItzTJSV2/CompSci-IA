using Dapper;
using System.Data.SQLite;

namespace Temp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string Connection = @"Data Source=database.db";

            var con = new SQLiteConnection(Connection);
            con.Open();

            string Name = "Test";
            string SecName = "Pain";

            con.Execute($"INSERT INTO UsersTable(FirstName, SecondName) VALUES({Name}, {SecName})");
        }
    }
}