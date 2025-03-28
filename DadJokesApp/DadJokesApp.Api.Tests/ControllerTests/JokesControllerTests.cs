using DadJokesApp.Api.Common;
using DadJokesApp.Api.Controllers;
using DadJokesApp.Api.Services;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DadJokesApp.Api.Tests.ControllerTests;

public class JokesControllerTests
{
    [Fact]
    public async Task GetRandomJoke_ReturnsOk_WhenJokeServiceSucceeds()
    {
        // Arrange
        var fakeService = A.Fake<IJokeService>();
        var expectedJoke = Guid.NewGuid().ToString();
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
}
