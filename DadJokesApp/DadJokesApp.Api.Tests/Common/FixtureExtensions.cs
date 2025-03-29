using AutoFixture;

namespace DadJokesApp.Api.Tests.Common;

public static class FixtureExtensions
{
    public static string GenerateRandomUri(this IFixture fixture) 
        => $"https://{fixture.Create<string>()}.com";
}
