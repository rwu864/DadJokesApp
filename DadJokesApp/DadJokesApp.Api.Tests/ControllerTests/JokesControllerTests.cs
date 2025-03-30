using AutoFixture;
using DadJokesApp.Api.Common;
using DadJokesApp.Api.Controllers;
using DadJokesApp.Api.Models;
using DadJokesApp.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DadJokesApp.Api.Tests.ControllerTests;

public class JokesControllerTests
{
    private readonly IFixture _fixture;

    public JokesControllerTests()
    {
        _fixture = new Fixture();
    }

    #region GetRandomJoke

    [Fact]
    public async Task GetRandomJoke_ReturnsOk_WhenJokeServiceSucceeds()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var expectedJoke = _fixture.Create<string>();
        A.CallTo(() => fakeService.GetRandomJokeAsync())
            .Returns(ServiceResult<string>.Ok(expectedJoke));

        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext(); 

        // Act
        var result = await controller.GetRandomJoke();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedJoke, okResult.Value);
    }

    [Fact]
    public async Task GetRandomJoke_CallsServiceOnce_WhenInvoked()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        A.CallTo(() => fakeService.GetRandomJokeAsync())
            .Returns(ServiceResult<string>.Ok(string.Empty));

        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        await controller.GetRandomJoke();

        // Assert
        A.CallTo(() => fakeService.GetRandomJokeAsync())
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task GetRandomJoke_ReturnsProblem_WhenJokeServiceFails()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        A.CallTo(() => fakeService.GetRandomJokeAsync())
            .Returns(ServiceResult<string>.Fail());

        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.GetRandomJoke();

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, problemResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
        Assert.Equal("Unable to fetch a joke at this time.", problemDetails.Detail);
    }

    #endregion GetRandomJoke

    #region SearchJokes

    [Fact]
    public async Task SearchJokes_ReturnsBadRequest_WhenTermIsEmpty()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.SearchJokes(string.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Invalid search parameter", problemDetails.Title);
        Assert.Equal("Search term cannot be empty or whitespace", problemDetails.Detail);
    }

    [Fact]
    public async Task SearchJokes_ReturnsBadRequest_WhenTermIsWhitespace()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.SearchJokes(string.Empty);

        // Assert
        var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
        Assert.Equal(400, badRequestResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(badRequestResult.Value);
        Assert.Equal("Invalid search parameter", problemDetails.Title);
        Assert.Equal("Search term cannot be empty or whitespace", problemDetails.Detail);
    }

    [Fact]
    public async Task SearchJokes_ReturnsOk_WhenJokeServiceSucceeds()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var expectedResult = _fixture.Create<JokeSearchModel>();
        A.CallTo(() => fakeService.SearchJokesAsync(A<string>._))
            .Returns(ServiceResult<JokeSearchModel>.Ok(expectedResult));
        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.SearchJokes(_fixture.Create<string>());

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Equal(200, okResult.StatusCode);
        Assert.Equal(expectedResult, okResult.Value);
    }

    [Fact]
    public async Task SearchJokes_CallsServiceOnce_WhenInvoked()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var searchTerm = _fixture.Create<string>();
        A.CallTo(() => fakeService.SearchJokesAsync(A<string>._))
            .Returns(ServiceResult<JokeSearchModel>.Ok(_fixture.Create<JokeSearchModel>()));
        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        await controller.SearchJokes(searchTerm);

        // Assert
        A.CallTo(() => fakeService.SearchJokesAsync(searchTerm))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task SearchJokes_ReturnsProblem_WhenJokeServiceFails()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        A.CallTo(() => fakeService.SearchJokesAsync(A<string>._))
            .Returns(ServiceResult<JokeSearchModel>.Fail());
        var controller = new JokesController(fakeService);
        controller.ControllerContext.HttpContext = new DefaultHttpContext();

        // Act
        var result = await controller.SearchJokes(_fixture.Create<string>());

        // Assert
        var problemResult = Assert.IsType<ObjectResult>(result.Result);
        Assert.Equal(500, problemResult.StatusCode);
        var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
        Assert.Equal("Unable to fetch a joke at this time.", problemDetails.Detail);
    }

    #endregion
}
