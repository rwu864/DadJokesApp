using DadJokesApp.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DadJokesApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JokesController (IJokeService jokeService) : ControllerBase
{
    [HttpGet("random")]
    public async Task<ActionResult<string>> GetRandomJoke()
    {
        var serviceResult = await jokeService.GetRandomJokeAsync();

        if (!serviceResult.Success)
            return JokeServiceError();

        return Ok(serviceResult.Value);
    }


    private ActionResult JokeServiceError(string? errorMessage = "Unable to fetch a joke at this time.") 
        => Problem
        (
            detail: errorMessage,
            title: "Joke API Error",
            statusCode: 500,
            instance: HttpContext.Request.Path
        );
}