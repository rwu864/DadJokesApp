namespace DadJokesApp.Api.Externals.icanhazdadjoke;

public class SearchDadJokeResponse
{
    public int CurrentPage { get; set; }
    public int Limit { get; set; }
    public int NextPage { get; set; }
    public int PreviousPage { get; set; }
    public List<SearchDadJokeResponseItem> Results { get; set; } = [];
    public string SearchTerm { get; set; } = string.Empty;
    public int Status { get; set; }
    public int TotalJokes { get; set; }
    public int TotalPages { get; set; }
}
