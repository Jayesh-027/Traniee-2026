namespace HelpdeskSystem.Helpers;

public class Result<T>
{
    private readonly T? _value;
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string ErrorMessage { get; }

    private Result(bool isSuccess, T? value, string errorMessage)
    {
        IsSuccess = isSuccess;
        _value = value;
        ErrorMessage = errorMessage;
    }

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access value");

    public static Result<T> Success(T value)
    {
        return new Result<T>(true, value, string.Empty);
    }

    public static Result<T> Failure(string error)
    {
        return new Result<T>(false, default, error);
    }
}
