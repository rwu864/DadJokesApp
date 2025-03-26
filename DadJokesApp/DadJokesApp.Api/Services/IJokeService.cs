using DadJokesApp.Api.Common;

namespace DadJokesApp.Api.Services;

public interface IJokeService
{
    Task<ServiceResult<string>> GetRandomJokeAsync();
}
