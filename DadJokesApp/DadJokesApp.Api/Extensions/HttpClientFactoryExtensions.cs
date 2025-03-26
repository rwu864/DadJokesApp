using DadJokesApp.Api.Configurations;

namespace DadJokesApp.Api.Extensions;

public static class HttpClientFactoryExtensions
{
    public static HttpClient CreateCanHazDadJokeHttpClient(this IHttpClientFactory factory) 
        => factory.CreateClient(CanHazDadJokeOptions.SectionName);
}
