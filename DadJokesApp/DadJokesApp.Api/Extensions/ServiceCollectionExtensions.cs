using DadJokesApp.Api.Configuration;

namespace DadJokesApp.Api.Extensions;
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCanHazDadJokeHttpClient
    (
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        CanHazDadJokeOptions options = new();
        configuration.GetSection(CanHazDadJokeOptions.SectionName).Bind(options);

        services.AddHttpClient(CanHazDadJokeOptions.SectionName, client =>
        {
            client.BaseAddress = new Uri(options.BaseAddress);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", options.UserAgent);
        });

        return services;
    }
}
