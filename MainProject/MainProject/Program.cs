using Comp_Sci_IA_Main_Proj_.Pages.Accounts;
using Comp_Sci_IA_Main_Proj_.Pages.Trips;
using System.Data.SQLite;

namespace MainProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Getting DB Directory
            string basePath = Path.GetDirectoryName(Environment.CurrentDirectory);
            string RealDir = Path.Combine(basePath, @"Mainproject", @"database", @"database.db");

            string Connection = @"Data Source=" + RealDir;
            // Attaching database and ensuring tables exist
            if (!File.Exists(RealDir))
            {
                SQLiteConnection.CreateFile(RealDir);
                using (var connection = new SQLiteConnection(Connection))
                {
                    connection.Open();

                    string createUsersTable = @"
                    CREATE TABLE IF NOT EXISTS UserTable (
                        AccountID INTEGER DEFAULT 1,
                        Username TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        PhoneNumber TEXT NOT NULL,
                        Password TEXT NOT NULL,
                        Trips TEXT,
                        PRIMARY KEY(AccountID));";
                    string createTripsTable = @"
                    CREATE TABLE IF NOT EXISTS TripsTable (
                        TripID INTEGER DEFAULT 1,
                        TripName TEXT NOT NULL,
                        TripStart TEXT NOT NULL,
                        TripStops TEXT,
                        TripEnd TEXT NOT NULL,
                        TripDate TEXT NOT NULL,
                        Members TEXT NOT NULL,
                        VideoDirecs TEXT,
                        PhotoDirecs TEXT,
                        PRIMARY KEY(TripID));";
                    using (var command = new SQLiteCommand(connection))
                    {
                        command.CommandText = createUsersTable;
                        command.ExecuteNonQuery();
                        command.CommandText = createTripsTable;
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine("Created a new DB.");
                }
            }
            Console.WriteLine("DB check successful.");

            // BUILDING REST OF WEB APP
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication().AddCookie("LoginCookieAuth", options =>
            {
                options.Cookie.Name = "LoginCookieAuth";
                options.LoginPath = "/Accounts/Login";
                options.AccessDeniedPath = "/Trips/NoAccess";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(30);
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("UserAccount", policy => policy
                    .RequireClaim("User")
                    .RequireClaim("LoggedIn"));

                options.AddPolicy("AdminAccount", policy => policy
                    .RequireClaim("Admin")
                    .RequireClaim("LoggedIn"));
            });

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}