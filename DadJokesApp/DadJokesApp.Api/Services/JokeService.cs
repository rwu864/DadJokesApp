
using DadJokesApp.Api.Common;
using DadJokesApp.Api.Extensions;
using DadJokesApp.Api.Externals.icanhazdadjoke;

namespace DadJokesApp.Api.Services;

public class JokeService : IJokeService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<JokeService> _logger;

    public JokeService
    (
        IHttpClientFactory httpClientFactory, 
        ILogger<JokeService> logger
    )
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<ServiceResult<string>> GetRandomJokeAsync()
    {
        var client = _httpClientFactory.CreateCanHazDadJokeHttpClient();

        var response = await client.GetAsync("/");

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Joke API responded with {StatusCode}", response.StatusCode);
            return ServiceResult<string>.Fail();
        }

        var joke = await response.Content.ReadFromJsonAsync<RandomDadJokeResult>();

        if (joke == null || string.IsNullOrWhiteSpace(joke.Joke))
        {
            _logger.LogError("Failed to deserialize joke response. Response: {Response}", response);
            return ServiceResult<string>.Fail();

        }

        return ServiceResult<string>.Ok(joke.Joke);
    }
}
