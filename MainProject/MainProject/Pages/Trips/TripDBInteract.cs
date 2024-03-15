using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PathSegments;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using System.Security.Principal;
using NuGet.Protocol.Plugins;

namespace Comp_Sci_IA_Main_Proj_.Pages.Trips
{
    public class StringCreation
    {
        public string TripStartEnd(int lati, int longi)
        {
            // Lat/Long
            string FinalString = (lati + "/" + longi);
            return FinalString;
        }
        public string ExtendableString(params string[] Values)
        {
            // -x-x-x-
            string FinalString = string.Join("-", Values);
            return FinalString;
        }
        public string TripStops(string[] Latitudes, string[] Longitudes)
        {
            // Lat/Long|Lat/Long|Lat/Long
            string[] IndividualRecords = new string[Latitudes.Length];
            for (int i = 0; i < Latitudes.Length; i++)
            {
                string temporary = Latitudes[i] + "/" + Longitudes[i];
                IndividualRecords[i] = (temporary);
            }
            string FinalString = string.Join("|", IndividualRecords);
            return FinalString;
        }   
    }
    public class TripDBInteract
    {
        public static string parentDir = Path.GetDirectoryName(Environment.CurrentDirectory);
        public static string Dir = Path.Combine(parentDir, @"Mainproject", @"database", @"Database.db");
        public static string Gallery = Path.Combine(parentDir, @"Mainproject", @"wwwroot", @"Gallery");
        public static string ConnectionString = ("Data Source=" + Dir);

        public List<int> GetAccessable(string Username, string Search = "")
        {
            CheckDB UserDB = new CheckDB();
            List<int> Trips = new List<int>();

            if (Username.ToLower() != "admin")
            {
                List<string> Account = UserDB.getAccount(Username);
                string AccountTrips = Account[5];
                List<string> Temp = new List<string>();

                // GETTING ALL ACCESSABLE TRIPS
                if (AccountTrips != "" && AccountTrips != null)
                {
                    Temp = (AccountTrips.Split("-")).ToList();
                }
                for (int i = 0; i < Temp.Count; i++)
                {
                    Trips.Add(Int16.Parse(Temp[i]));
                }
                if (Search != "") // SEARCH VALUE EXISTS
                {
                    List<int> AllSimilarTrips = new List<int>();
                    string queryStr = "SELECT * FROM TripsTable WHERE TripName LIKE @param1";
                    using (var connection = new SQLiteConnection(ConnectionString))
                    {
                        connection.Open();
                        using (SQLiteCommand command = new SQLiteCommand(connection))
                        {
                            command.CommandText = queryStr;
                            command.Parameters.AddWithValue("@param1", ("%" + Search + "%"));

                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    AllSimilarTrips.Add(reader.GetInt32(0));
                                }
                            }
                        }
                    }
                    List<int> Tempo = new List<int>();
                    for (int i = 0; i < Trips.Count; i++)
                    {
                        if (!AllSimilarTrips.Contains(Trips[i]))
                        {
                            Tempo.Add(i);

                        }
                    }
                    for (int y = 0; y < Temp.Count; y++)
                    {
                        Trips.Remove(Tempo[y]);
                    }
                }

            } else if (Username.ToLower() == "admin")
            {
                string queryStr = "SELECT * FROM TripsTable";
                List<int> TripList = new List<int>();
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(queryStr, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                TripList.Add(reader.GetInt32(0));
                            }
                        }
                    }
                }
                for (int i = 0; i < TripList.Count; i++)
                {
                    Trips.Add(TripList[i]);
                }
            }
            return Trips;
        }
        public List<string> GetTrip(int TripID)
        {
            List<string> Trip = new List<string>();
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string queryStr = "SELECT * FROM TripsTable WHERE TripID = @param1";
                using (SQLiteCommand command = new SQLiteCommand(queryStr, connection))
                {
                    command.Parameters.AddWithValue("@param1", TripID);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Trip.Add(reader.GetInt32(0).ToString()); // ID
                            Trip.Add(reader.GetString(1));           // Name
                            Trip.Add(reader.GetString(2));           // Start
                            Trip.Add(reader.GetString(3));           // Stops
                            Trip.Add(reader.GetString(4));           // End
                            Trip.Add(reader.GetString(5));           // Date
                            Trip.Add(reader.GetString(6));           // Members
                            if (!reader.IsDBNull(7))
                            {
                                Trip.Add(reader.GetString(7));
                            }
                            if (!reader.IsDBNull(8))
                            {
                                Trip.Add(reader.GetString(8));
                            }
                        }
                    }
                }
                return Trip;
            }
        }
        public List<string> GetMemberIDString(int tripID)
        {
            List<string> Members = new List<string>();
            TripDBInteract DB = new TripDBInteract();
            CheckDB UserDB = new CheckDB();

            string MemberString = DB.GetTrip(tripID)[6];
            List<string> Temp = (MemberString.Split("-")).ToList();
            for (int i = 0; i < Temp.Count; i++)
            {
                Members.Add(Temp[i]);
            }
            return Members;
        }
        public List<string> GetMembersNames(int tripID)
        {
            List<string> MemberNames = new List<string>();
            List<int> Members = new List<int>();
            TripDBInteract DB = new TripDBInteract();
            CheckDB UserDB = new CheckDB();

            string MemberString = DB.GetTrip(tripID)[6];
            List<string> Temp = (MemberString.Split("-")).ToList();
            for (int i = 0; i < Temp.Count; i++)
            {
                Members.Add(int.Parse(Temp[i]));
            }
            for (int j = 0; j < Members.Count; j++)
            {
                MemberNames.Add(UserDB.GetName(Members[j]));
            }
            return MemberNames;
        }
        public string CreateTrip(string TName, string TStart, string TStops, string TEnd, string TDate, string Members)
        {
            string TripID = "-1";
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                string queryStr = "INSERT INTO TripsTable (TripName, TripStart, TripStops, TripEnd, TripDate, Members) VALUES (@param1, @param2, @param3, @param4, @param5, @param6); SELECT last_insert_rowid();";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = queryStr;
                    command.Parameters.AddWithValue("@param1", TName);
                    command.Parameters.AddWithValue("@param2", TStart);
                    command.Parameters.AddWithValue("@param3", TStops);
                    command.Parameters.AddWithValue("@param4", TEnd);
                    command.Parameters.AddWithValue("@param5", TDate);
                    command.Parameters.AddWithValue("@param6", Members);
                    TripID = command.ExecuteScalar().ToString();
                }
                queryStr  = "SELECT TripID FROM TripsTable WHERE TripName = @param1";
                connection.Close();
            }
            string NewPath = Path.Combine(Gallery, TripID);
            if (!File.Exists(NewPath))
            {
                Directory.CreateDirectory(Path.Combine(NewPath, "Videos"));
                Directory.CreateDirectory(Path.Combine(NewPath, "Photos"));
            }
            return TripID;
        }
        public void DeleteTrip(int TripID)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = $"DELETE FROM TripsTable WHERE TripID = @Id";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@Id", TripID);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }
        public void EditTrip(string TripID, string TName, string TStart, string TStops, string TEnd, string TDate, string Members)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();

                string query = $"UPDATE TripsTable SET TripName = @TName, TripStart  = @TStart, TripStops = @TStops, TripEnd = @TEnd, TripDate = @TDate, Members = @Members WHERE TripID = @TripID";
                using (var command = new SQLiteCommand(connection))
                {
                    command.CommandText = query;
                    command.Parameters.AddWithValue("@TripID", TripID);
                    command.Parameters.AddWithValue("@TName", TName);
                    command.Parameters.AddWithValue("@TStart", TStart);
                    command.Parameters.AddWithValue("@TStops", TStops);
                    command.Parameters.AddWithValue("@TEnd", TEnd);
                    command.Parameters.AddWithValue("@TDate", TDate);
                    command.Parameters.AddWithValue("@Members", Members);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
            }
        }

        public List<string> GetUserInformation(int AccountID)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                List<string> Account = new List<string>();
                string queryStr1 = "SELECT * FROM UserTable WHERE AccountID = @Id";
                using (SQLiteCommand command = new SQLiteCommand(queryStr1, connection))
                {
                    command.Parameters.AddWithValue("@Id", AccountID);

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
                            if (!reader.IsDBNull(5))
                            {
                                Account.Add(reader.GetString(5));
                            }
                            else
                            {
                                Account.Add("");
                            }
                        }
                    }
                }
                return Account;
            }
            return null;
        }

    }
}
