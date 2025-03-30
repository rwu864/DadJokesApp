using DadJokesApp.Api.Services;

namespace DadJokesApp.Api.Models;

public class JokeSearchModel
{
    public IEnumerable<JokeSearchItemModel> ShortJokes { get; set; } = [];
    public IEnumerable<JokeSearchItemModel> MediumJokes { get; set; } = [];
    public IEnumerable<JokeSearchItemModel> LongJokes { get; set; } = [];
}
