using DadJokesApp.Api.Extensions;
using DadJokesApp.Api.Externals.icanhazdadjoke;
using Microsoft.AspNetCore.Mvc;

namespace DadJokesApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JokesController 
(
    IHttpClientFactory httpClientFactory,
    ILogger<JokesController> logger
) : ControllerBase
{
    [HttpGet("random")]
    public async Task<ActionResult<string>> GetRandomJoke()
    {
        var client = httpClientFactory.CreateCanHazDadJokeHttpClient();

        var response = await client.GetAsync("/");
        
        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Joke API responded with {StatusCode}", response.StatusCode);
            return JokeServiceError();
        }

        var joke = await response.Content.ReadFromJsonAsync<RandomDadJokeResult>();

        if (joke == null || string.IsNullOrWhiteSpace(joke.Joke))
        {
            logger.LogError("Failed to deserialize joke response: {response}", response);
            return JokeServiceError();
        }
    
        return Ok(joke.Joke);
    }


    private ActionResult JokeServiceError() 
        => Problem
        (
            detail: "Unable to fetch a joke at this time.",
            title: "Joke API Error",
            statusCode: 500,
            instance: HttpContext.Request.Path
        );
}