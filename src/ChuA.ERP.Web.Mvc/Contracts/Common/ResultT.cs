namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>Generic version of <see cref="Result"/> that carries a value on success.</summary>
public sealed class Result<T> : Result
{
    private readonly T? _value;

    private Result(T value)
        : base(true, Array.Empty<Error>())
    {
        _value = value;
    }

    private Result(IReadOnlyCollection<Error> errors, int statusCode, ApiErrorResponse? problem)
        : base(false, errors)
    {
        StatusCode = statusCode;
        Problem = problem;
    }

    /// <summary>Wrapped value; throws if the result is a failure.</summary>
    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failed result.");

    /// <summary>Safe accessor for failure-tolerant call sites.</summary>
    public T? ValueOrDefault => _value;

    public new int StatusCode { get; init; }
    public new ApiErrorResponse? Problem { get; init; }

    public static Result<T> Success(T value) => new(value);

    public static new Result<T> Failure(Error error) => Failure(new[] { error });

    public static new Result<T> Failure(IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new Result<T>(errors.Where(e => e != Error.None).ToArray(), 0, null);
    }

    public static new Result<T> FromProblem(ApiErrorResponse problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        var errors = problem.ToErrors();
        if (errors.Count == 0)
        {
            errors = new[] { new Error(problem.ErrorCode ?? "api.error", problem.Detail ?? problem.Title ?? "Unknown error") };
        }
        return new Result<T>(errors, problem.Status ?? 0, problem);
    }
}
