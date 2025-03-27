using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DadJokesApp.Web.Pages;

public class RandomJokeModel : PageModel
{
    private readonly HttpClient _httpClient;

    public RandomJokeModel(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("DadJokesApp.Api"); 
    }

    public string? Joke { get; set; }

    public async Task OnGetAsync()
    {
        Joke = await _httpClient.GetStringAsync("api/jokes/random");
    }
}