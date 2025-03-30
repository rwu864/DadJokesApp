using DadJokesApp.Api.Common;
using DadJokesApp.Api.Extensions;
using DadJokesApp.Api.Externals.icanhazdadjoke;
using DadJokesApp.Api.Models;
using System.Text.RegularExpressions;

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

        var joke = await response.Content.ReadFromJsonAsync<RandomDadJokeResponse>();

        if (joke == null || string.IsNullOrWhiteSpace(joke.Joke))
        {
            _logger.LogError("Failed to deserialize joke response. Response: {Response}", response);
            return ServiceResult<string>.Fail();
        }

        return ServiceResult<string>.Ok(joke.Joke);
    }

    public async Task<ServiceResult<JokeSearchModel>> SearchJokesAsync(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return ServiceResult<JokeSearchModel>.Fail("Search term must contain value");

        var client = _httpClientFactory.CreateCanHazDadJokeHttpClient();

        var query = new Dictionary<string, string>
        {
            ["limit"] = "30",
            ["term"] = term
        };

        var queryString = string.Join("&", query.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));
        var url = $"/search?{queryString}";

        var response = await client.GetAsync(url);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Joke search API responded with {StatusCode}", response.StatusCode);
            return ServiceResult<JokeSearchModel>.Fail();
        }

        var searchResult = await response.Content.ReadFromJsonAsync<SearchDadJokeResponse>();

        if (searchResult == null)
        {
            _logger.LogError("Failed to deserialize joke search response. Response: {Response}", response);
            return ServiceResult<JokeSearchModel>.Fail();
        }

        IList<JokeSearchItemModel> shortJokes = [];
        IList<JokeSearchItemModel> mediumJokes = [];
        IList<JokeSearchItemModel> longJokes = [];

        foreach (var item in searchResult.Results)
        {
            var wordCount = item.Joke.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length;

            var highlightedText = HighlightSearchTerm(item.Joke, term);

            var model = new JokeSearchItemModel
            {
                Text = item.Joke,
                HighlightedText = highlightedText,
                WordCount = wordCount,
            };

            if (wordCount < 10)
                shortJokes.Add(model);
            else if (wordCount < 20)
                mediumJokes.Add(model);
            else
                longJokes.Add(model);
        }

        return ServiceResult<JokeSearchModel>.Ok(new JokeSearchModel
        {
            ShortJokes = shortJokes,
            MediumJokes = mediumJokes,
            LongJokes = longJokes
        });
    }
    private string HighlightSearchTerm(string text, string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return text;

        return Regex.Replace
        (
            text,
            Regex.Escape(term),
            m => $"<{m.Value}>",
            RegexOptions.IgnoreCase
        );
    }
}