using System.Text.Json.Serialization;

namespace Enterprise.Application.Common.Models;

/// <summary>
/// Represents the result of an operation with success status and error details.
/// </summary>
public class Result
{
    public bool IsSuccess { get; init; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; init; }
    public IReadOnlyCollection<string> Errors { get; init; } = Array.Empty<string>();

    public Result()
    {
    }

    protected Result(bool isSuccess, string? error, IEnumerable<string>? errors = null)
    {
        IsSuccess = isSuccess;
        Error = error;
        var errorList = errors?.ToList() ?? (error != null ? new List<string> { error } : new List<string>());
        Errors = errorList.AsReadOnly();
    }

    public static Result Success() => new(true, null);
    public static Result Failure(string error) => new(false, error);
    public static Result Failure(IEnumerable<string> errors) => new(false, errors.FirstOrDefault(), errors);
}

/// <summary>
/// Represents the result of an operation returning a value of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of the returned value.</typeparam>
public class Result<T> : Result
{
    public T? Value { get; init; }

    public Result()
    {
    }

    protected Result(bool isSuccess, T? value, string? error, IEnumerable<string>? errors = null)
        : base(isSuccess, error, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public new static Result<T> Failure(string error) => new(false, default, error);
    public new static Result<T> Failure(IEnumerable<string> errors) => new(false, default, errors.FirstOrDefault(), errors);
}
