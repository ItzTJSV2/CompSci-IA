using Microsoft.AspNetCore.Identity;
using System.Security.Cryptography.X509Certificates;

namespace Comp_Sci_IA_Main_Proj_
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthentication().AddCookie("LoginCookieAuth", options =>
            {
                options.Cookie.Name = "LoginCookieAuth";
                options.LoginPath = "/Accounts/Login";
                options.AccessDeniedPath= "/Trips/NoAccess";
                options.ExpireTimeSpan= TimeSpan.FromSeconds(30);
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