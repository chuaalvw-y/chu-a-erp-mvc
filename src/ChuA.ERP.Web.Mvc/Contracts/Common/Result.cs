// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>
/// UI-safe equivalent of <c>ChuA.SharedKernel.Models.Result</c>. The MVC declares its own
/// copy so it does not need to project-reference the API; on the wire the API simply returns
/// data or an <see cref="ApiErrorResponse"/>, which the client converts into a Result here.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, IReadOnlyCollection<Error> errors)
    {
        if (isSuccess && errors.Count > 0)
        {
            throw new ArgumentException("Successful results cannot contain errors.", nameof(errors));
        }
        if (!isSuccess && errors.Count == 0)
        {
            throw new ArgumentException("Failed results must contain at least one error.", nameof(errors));
        }
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IReadOnlyCollection<Error> Errors { get; }

    /// <summary>HTTP status returned by the API on failure (0 for client-side failures).</summary>
    public int StatusCode { get; init; }

    /// <summary>The raw ProblemDetails payload, if any.</summary>
    public ApiErrorResponse? Problem { get; init; }

    public static Result Success() => new(true, Array.Empty<Error>());

    public static Result Failure(Error error) => Failure(new[] { error });

    public static Result Failure(IEnumerable<Error> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new Result(false, errors.Where(e => e != Error.None).ToArray());
    }

    /// <summary>Builds a Result from an API ProblemDetails payload.</summary>
    public static Result FromProblem(ApiErrorResponse problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        var errors = problem.ToErrors();
        if (errors.Count == 0)
        {
            errors = new[] { new Error(problem.ErrorCode ?? "api.error", problem.Detail ?? problem.Title ?? "Unknown error") };
        }
        return new Result(false, errors)
        {
            StatusCode = problem.Status ?? 0,
            Problem = problem,
        };
    }
}
