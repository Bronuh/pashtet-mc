#region

using Common;
using Common.IO.Checksum;
using Common.Password;
using Microsoft.AspNetCore.HttpOverrides;
using PashtetMcBackend.Models.Api;

#endregion

namespace PashtetMcBackend;

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
        
        // Configure Forwarded Headers options
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            
            // Adjust this setting to match the reverse proxy's IP range if it's static
            options.KnownProxies.Clear(); // Clear KnownProxies if not relevant or set if IP range is known
            options.KnownNetworks.Clear(); // Clear or set KnownNetworks based on the proxy network
        });
        
        var app = builder.Build();
        app.Services.GetService<IApiProvider>()?.Initialize();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        
        app.Urls.Add("http://*:80");

        app.UseAuthorization();
        
        // Apply Forwarded Headers Middleware
        app.UseForwardedHeaders();

        app.MapControllers();
        
        app.UseStatusCodePagesWithReExecute("/api/error/notfound");
        
        app.Run();
    }
}