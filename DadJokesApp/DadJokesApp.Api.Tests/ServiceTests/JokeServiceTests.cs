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

    [Fact]
    public async Task GetRandomJokeAsync_ReturnsJoke_WhenApiResponseIsSuccessful()
    {
        // Arrange
        var expectedJoke = _fixture.Create<string>();
        var mockHandler = new MockHttpMessageHandler(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new RandomDadJokeResult { Joke = expectedJoke })
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
            Content = JsonContent.Create(new { }) 
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
            Content = JsonContent.Create(new RandomDadJokeResult { Joke = string.Empty })
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
}