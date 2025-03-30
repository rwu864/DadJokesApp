using AutoFixture;
using DadJokesApp.Api.Externals.icanhazdadjoke;
using DadJokesApp.Api.Services;
using DadJokesApp.Api.Tests.Common;
using FakeItEasy;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;

namespace DadJokesApp.Api.Tests.ServiceTests;

public class JokeServiceTests
{
    private readonly IHttpClientFactory _fakeHttpClientFactory;
    private readonly ILogger<JokeService> _fakeLogger;
    private readonly IFixture _fixture;

    public JokeServiceTests()
    {
        _fakeLogger = A.Fake<ILogger<JokeService>>();
        _fakeHttpClientFactory = A.Fake<IHttpClientFactory>();
        _fixture = new Fixture();
    }

    #region GetRandomJokeAsync

    [Fact]
    public async Task GetRandomJokeAsync_ReturnsJoke_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedJoke = _fixture.Create<string>();
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new RandomDadJokeResponse { Joke = expectedJoke })
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://icanhazdadjoke.com")
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.GetRandomJokeAsync();

        // Assert
        Assert.True(result.Success);
        Assert.Equal(expectedJoke, result.Value);
    }

    [Fact]
    public async Task GetRandomJokeAsync_ReturnsFail_WhenApiResponseIsNotSuccessful()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.GetRandomJokeAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetRandomJokeAsync_ReturnsFail_WhenDeserializationFails()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{invalid_json}", System.Text.Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.GetRandomJokeAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task GetRandomJokeAsync_ReturnsFail_WhenJokePropertyIsEmpty()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new RandomDadJokeResponse { Joke = string.Empty })
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri("https://icanhazdadjoke.com")
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.GetRandomJokeAsync();

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    #endregion GetRandomJokeAsync

    #region Search

    [Fact]
    public async Task SearchJokesAsync_ReturnsFail_WhenTermIsEmpty()
    {
        // Arrange
        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(string.Empty);

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Search term must contain value", result.ErrorMessage);
    }

    [Fact]
    public async Task SearchJokesAsync_ReturnsFail_WhenTermIsWhitespace()
    {
        // Arrange
        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(" ");

        // Assert
        Assert.False(result.Success);
        Assert.Equal("Search term must contain value", result.ErrorMessage);
    }

    [Fact]
    public async Task SearchJokesAsync_ReturnsJokesInCorrectLists_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var searchTerm = _fixture.Create<string>();
        var mockResponse = new SearchDadJokeResponse
        {
            Results =
            [
                new() { Joke = _fixture.GenerateRandomWords(2) }, // should be a short joke
                new() { Joke = _fixture.GenerateRandomWords(11) },// should be a medium joke
                new() { Joke = _fixture.GenerateRandomWords(25) } // should be a long joke
            ],
            SearchTerm = searchTerm,
            Status = 200,
            TotalJokes = 3
        };

        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(mockResponse)
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(searchTerm);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value.ShortJokes);
        Assert.Single(result.Value.MediumJokes);
        Assert.Single(result.Value.LongJokes);
    }

    [Fact]
    public async Task SearchJokesAsync_HighlightsTermsCorrectly_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var searchTerm = _fixture.Create<string>();
        var mockResponse = new SearchDadJokeResponse
        {
            Results =
            [
                new() { Joke = _fixture.GenerateRandomWords(2) + searchTerm }, 
                new() { Joke = searchTerm + _fixture.GenerateRandomWords(11) },
                new() { Joke = _fixture.GenerateRandomWords(20) + searchTerm + _fixture.GenerateRandomWords(20) }
            ],
            SearchTerm = searchTerm,
            Status = 200,
            TotalJokes = 3
        };

        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(mockResponse)
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(searchTerm);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.Value);

        var expectedHighlight = $"<{searchTerm}>";

        Assert.Single(result.Value.ShortJokes);
        Assert.Contains(expectedHighlight, result.Value.ShortJokes.First().HighlightedText);

        Assert.Single(result.Value.MediumJokes);
        Assert.Contains(expectedHighlight, result.Value.MediumJokes.First().HighlightedText);

        Assert.Single(result.Value.LongJokes);
        Assert.Contains(expectedHighlight, result.Value.LongJokes.First().HighlightedText);
    }

    [Fact]
    public async Task SearchJokesAsync_ReturnsFail_WhenApiResponseIsNotSuccessful()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.InternalServerError));

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(_fixture.Create<string>());

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    [Fact]
    public async Task SearchJokesAsync_ReturnsFail_WhenDeserializationFails()
    {
        // Arrange
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("{invalid_json}", System.Text.Encoding.UTF8, "application/json")
        });

        var httpClient = new HttpClient(mockHandler)
        {
            BaseAddress = new Uri(_fixture.GenerateRandomUri())
        };

        A.CallTo(() => _fakeHttpClientFactory.CreateClient(A<string>._))
            .Returns(httpClient);

        var service = new JokeService(_fakeHttpClientFactory, _fakeLogger);

        // Act
        var result = await service.SearchJokesAsync(_fixture.Create<string>());

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.Value);
    }

    #endregion Search
}