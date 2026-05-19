namespace RpgManager.Application.Common;

public enum ServiceErrorType
{
    None,
    Validation,
    NotFound,
    Forbidden,
    Conflict
}

public sealed record ServiceResult<T>(
    bool Succeeded,
    T? Data,
    string? Error,
    ServiceErrorType ErrorType = ServiceErrorType.None)
{
    public static ServiceResult<T> Success(T data) => new(true, data, null);

    public static ServiceResult<T> Failure(
        string error,
        ServiceErrorType errorType = ServiceErrorType.Validation)
        => new(false, default, error, errorType);
}
