using DadJokesApp.Api.Common;
using DadJokesApp.Api.Models;

namespace DadJokesApp.Api.Services;

public interface IJokeService
{
    Task<ServiceResult<string>> GetRandomJokeAsync();
    Task<ServiceResult<JokeSearchModel>> SearchJokesAsync(string term);
}
