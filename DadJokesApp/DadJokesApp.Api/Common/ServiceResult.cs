namespace DadJokesApp.Api.Common;

public class ServiceResult<T>
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public T? Value { get; init; }

    public static ServiceResult<T> Ok(T value) => new() { Success = true, Value = value };
    public static ServiceResult<T> Fail(string? errorMessage = null) => new() { Success = false, ErrorMessage = errorMessage };
}
