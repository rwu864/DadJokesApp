namespace DadJokesApp.Api.Models;

public class JokeSearchItemModel
{
    public string Text { get; set; } = string.Empty;
    public string HighlightedText { get; set; } = string.Empty;
    public int WordCount { get; set; }
}
