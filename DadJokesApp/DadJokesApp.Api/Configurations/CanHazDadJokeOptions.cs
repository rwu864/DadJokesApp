namespace DadJokesApp.Api.Configurations
{
    public class CanHazDadJokeOptions
    {
        public static readonly string SectionName = nameof(CanHazDadJokeOptions);

        public string BaseAddress { get; set; } = string.Empty;

        public string UserAgent { get; set; } = string.Empty;
    }
}
