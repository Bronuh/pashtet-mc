using BronuhMcBackend.Models;
using BronuhMcBackend.Models.Api;
using Common;
using Common.IO.Checksum;
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
        builder.Services.AddSingleton<IPasswordHasher>(DefaultServices.PasswordHasher);
        builder.Services.AddSingleton<IChecksumProvider>(DefaultServices.ChecksumProvider);
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