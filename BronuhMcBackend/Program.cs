using BronuhMcBackend.Models;
using BronuhMcBackend.Utils;

namespace BronuhMcBackend;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddAuthorization();
        builder.Services.Configure<DirectorySettings>(builder.Configuration.GetSection("DirectorySettings"));
        builder.Services.AddSingleton<FilesystemContext>();
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