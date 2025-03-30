using DadJokesApp.Api.Models;
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
        var result = await jokeService.GetRandomJokeAsync();

        if (!result.Success)
            return JokeServiceError();

        return Ok(result.Value);
    }

    [HttpGet("search")]
    public async Task<ActionResult<JokeSearchModel>> SearchJokes([FromQuery] string term)
    {
        var result = await jokeService.SearchJokesAsync(term);

        if (!result.Success)
            return JokeServiceError();

        return Ok(result.Value);
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