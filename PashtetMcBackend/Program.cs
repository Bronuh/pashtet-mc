using BronuhMcBackend.Models;
using BronuhMcBackend.Models.Api;
using BronuhMcBackend.Utils;
using Common.Password;

namespace BronuhMcBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.AddSingleton<IPasswordHasher, Rfc2898PasswordHasher>();
        builder.Services.AddSingleton<IApiProvider, DefaultApiProvider>();
        
        var app = builder.Build();
        app.Services.GetService<IApiProvider>()?.Initialize();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.Urls.Add("http://*:80");

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}