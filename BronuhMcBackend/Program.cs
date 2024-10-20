using BhCommon;
using BronuhMcBackend.Models;
using BronuhMcBackend.Utils;

namespace BronuhMcBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ILoggerFactory loggerFactory = LoggerFactory.Create(logBuilder => logBuilder.AddConsole());
        ILogger logger = loggerFactory.CreateLogger("DEFAULT");
        
        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.Configure<DirectorySettings>(builder.Configuration.GetSection("DirectorySettings"));
        builder.Services.AddSingleton<FilesystemContext>();
        builder.Services.AddSingleton<ILogger>(logger);
        builder.Services.AddSingleton<PasswordHasher>();
        var app = builder.Build();

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