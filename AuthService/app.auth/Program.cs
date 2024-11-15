
using app.auth.DataAccess;
using app.auth.Services;
using Microsoft.EntityFrameworkCore;
using System;

namespace app.auth
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddTransient<UserService>();

            builder.Services.AddDbContext<DbClientContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("LocalhostDbConnection")));

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHealthChecks();
            var app = builder.Build();

            app.MapHealthChecks("api/health");

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
