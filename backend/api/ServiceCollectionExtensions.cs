using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using service.services;

namespace api;

public static class ServiceCollectionExtensions
{
    public static void AddJwtService(this IServiceCollection services)
    {
        services.AddSingleton<JwtOptions>(services =>
        {
            var configuration = services.GetRequiredService<IConfiguration>();
            var options = configuration.GetRequiredSection("JWT").Get<JwtOptions>()!;
    
            // If address isn't set in the config then we are likely running in development mode.
            // We will use the address of the server as *issuer* for JWT.
            if (string.IsNullOrEmpty(options?.Address))
            {
                var server = services.GetRequiredService<IServer>();
                var addresses = server.Features.Get<IServerAddressesFeature>()?.Addresses;
                options!.Address = addresses?.FirstOrDefault();
            }

            return options;
        });
        services.AddSingleton<JwtService>();
    }
    
    public static void AddAvatarBlobService(this IServiceCollection services)
    {
        services.AddSingleton<BlobService>(provider =>
        {
            // Get connection string from configuration (appsettings.json)
            //var connectionString = provider.GetService<IConfiguration>()!.GetConnectionString("AvatarStorage");
            var connectionString = Environment.GetEnvironmentVariable("avatarstorage");
            
            // The client knows how to talk to the service on Azure.
            var client = new BlobServiceClient(connectionString);
            // Return an instance of the service we just made.
            return new BlobService(client);
        });
    }
}