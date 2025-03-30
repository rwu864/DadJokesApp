using AutoFixture;

namespace DadJokesApp.Api.Tests.Common;

public static class FixtureExtensions
{
    public static string GenerateRandomUri(this IFixture fixture) 
        => $"https://{fixture.Create<string>()}.com";

    public static string GenerateRandomWords(this IFixture fixture, int wordCount)
    {
        if (wordCount <= 0)
            return string.Empty;

        IList<string> words = [];
        for (int i = 0; i < wordCount; i++)
        {
            words.Add(fixture.Create<string>());
        }

        return string.Join(" ", words);
    }
}
