// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChuA.ERP.Web.Mvc.Contracts.Common;
using ChuA.ERP.Web.Mvc.Security;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.Extensions.Logging;

namespace ChuA.ERP.Web.Mvc.ApiClients;

/// <summary>
/// Base class for every strongly typed API client. Centralises:
///   - bearer token attachment via <see cref="ITokenAcquisitionService"/>
///   - X-Correlation-ID propagation
///   - ProblemDetails parsing into <see cref="ApiErrorResponse"/> / <see cref="Result{T}"/>
///   - JSON serialization options (PascalCase records, DateOnly, enum strings)
/// Concrete clients only need to call <see cref="SendAsync{T}"/> / <see cref="SendAsync"/>.
/// </summary>
public abstract class ApiClientBase
{
    /// <summary>Shared JSON options. Tolerant of casing so the API's PascalCase records bind.</summary>
    protected static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter(),
        },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    protected ApiClientBase(
        HttpClient httpClient,
        ITokenAcquisitionService tokenAcquisition,
        ICorrelationIdAccessor correlationIds,
        ILogger logger)
    {
        Http = httpClient;
        TokenAcquisition = tokenAcquisition;
        CorrelationIds = correlationIds;
        Logger = logger;
    }

    protected HttpClient Http { get; }
    protected ITokenAcquisitionService TokenAcquisition { get; }
    protected ICorrelationIdAccessor CorrelationIds { get; }
    protected ILogger Logger { get; }

    /// <summary>Sends a request and deserializes the body into <typeparamref name="T"/>.</summary>
    protected async Task<Result<T>> SendAsync<T>(
        HttpMethod method,
        string relativeUri,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var request = await BuildRequestAsync(method, relativeUri, body, cancellationToken).ConfigureAwait(false);
        try
        {
            using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Result<T>.FromProblem(await ReadProblemAsync(response, cancellationToken).ConfigureAwait(false));
            }

            if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
            {
                return Result<T>.Success(default!);
            }

            var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions, cancellationToken).ConfigureAwait(false);
            if (value is null)
            {
                return Result<T>.Failure(new Error("api.empty_response", "API returned an empty body."));
            }
            return Result<T>.Success(value);
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "HTTP failure calling {Method} {Uri}", method, relativeUri);
            return Result<T>.Failure(new Error("api.transport", ex.Message));
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogWarning(ex, "Timeout calling {Method} {Uri}", method, relativeUri);
            return Result<T>.Failure(new Error("api.timeout", "The API call timed out."));
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "JSON parse error calling {Method} {Uri}", method, relativeUri);
            return Result<T>.Failure(new Error("api.parse", "Could not parse API response."));
        }
    }

    /// <summary>
    /// Sends a GET request for a list endpoint. The current API returns arrays, while the
    /// server-side paging contract is expected to return <see cref="PagedResult{T}"/>.
    /// This method accepts both shapes so the MVC can move toward API-side paging without
    /// breaking against the existing API.
    /// </summary>
    protected async Task<Result<PagedResult<T>>> SendPagedAsync<T>(
        string relativeUri,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        using var request = await BuildRequestAsync(HttpMethod.Get, relativeUri, body: null, cancellationToken).ConfigureAwait(false);
        try
        {
            using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Result<PagedResult<T>>.FromProblem(await ReadProblemAsync(response, cancellationToken).ConfigureAwait(false));
            }

            if (response.StatusCode == HttpStatusCode.NoContent || response.Content.Headers.ContentLength == 0)
            {
                return Result<PagedResult<T>>.Success(PagedResult<T>.Empty(pageSize));
            }

            var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(json))
            {
                return Result<PagedResult<T>>.Success(PagedResult<T>.Empty(pageSize));
            }

            return Result<PagedResult<T>>.Success(ParsePagedResponse<T>(json, pageNumber, pageSize));
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "HTTP failure calling GET {Uri}", relativeUri);
            return Result<PagedResult<T>>.Failure(new Error("api.transport", ex.Message));
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogWarning(ex, "Timeout calling GET {Uri}", relativeUri);
            return Result<PagedResult<T>>.Failure(new Error("api.timeout", "The API call timed out."));
        }
        catch (JsonException ex)
        {
            Logger.LogError(ex, "JSON parse error calling GET {Uri}", relativeUri);
            return Result<PagedResult<T>>.Failure(new Error("api.parse", "Could not parse API response."));
        }
    }

    /// <summary>Sends a request expecting no response body.</summary>
    protected async Task<Result> SendAsync(
        HttpMethod method,
        string relativeUri,
        object? body = null,
        CancellationToken cancellationToken = default)
    {
        using var request = await BuildRequestAsync(method, relativeUri, body, cancellationToken).ConfigureAwait(false);
        try
        {
            using var response = await Http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
                .ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                return Result.FromProblem(await ReadProblemAsync(response, cancellationToken).ConfigureAwait(false));
            }
            return Result.Success();
        }
        catch (HttpRequestException ex)
        {
            Logger.LogWarning(ex, "HTTP failure calling {Method} {Uri}", method, relativeUri);
            return Result.Failure(new Error("api.transport", ex.Message));
        }
        catch (TaskCanceledException ex)
        {
            Logger.LogWarning(ex, "Timeout calling {Method} {Uri}", method, relativeUri);
            return Result.Failure(new Error("api.timeout", "The API call timed out."));
        }
    }

    private async Task<HttpRequestMessage> BuildRequestAsync(
        HttpMethod method,
        string relativeUri,
        object? body,
        CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(method, relativeUri);

        var token = await TokenAcquisition.GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
        if (!string.IsNullOrWhiteSpace(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        request.Headers.TryAddWithoutValidation(ApiHeaders.CorrelationId, CorrelationIds.GetOrCreate());

        if (body is not null)
        {
            var json = JsonSerializer.Serialize(body, body.GetType(), JsonOptions);
            request.Content = new StringContent(json, Encoding.UTF8, "application/json");
        }

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/problem+json"));
        return request;
    }

    private static async Task<ApiErrorResponse> ReadProblemAsync(HttpResponseMessage response, CancellationToken ct)
    {
        try
        {
            var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
            if (contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                var parsed = await response.Content.ReadFromJsonAsync<ApiErrorResponse>(JsonOptions, ct)
                    .ConfigureAwait(false);
                if (parsed is not null)
                {
                    parsed.Status ??= (int)response.StatusCode;
                    parsed.Title ??= response.ReasonPhrase;
                    return parsed;
                }
            }
        }
        catch
        {
            // fall through to fabricated problem below
        }

        string? body = null;
        try { body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false); }
        catch { /* ignore */ }

        return new ApiErrorResponse
        {
            Status = (int)response.StatusCode,
            Title = response.ReasonPhrase ?? "API error",
            Detail = string.IsNullOrWhiteSpace(body) ? response.ReasonPhrase : body,
            ErrorCode = "api.error",
        };
    }

    /// <summary>Helper to append a non-null query string parameter to a URI.</summary>
    protected static string AppendQuery(string baseUri, string name, object? value)
    {
        if (value is null) return baseUri;
        var text = value switch
        {
            string s => s,
            DateOnly d => d.ToString("yyyy-MM-dd"),
            DateTime dt => dt.ToString("o"),
            _ => value.ToString() ?? string.Empty,
        };
        if (string.IsNullOrWhiteSpace(text)) return baseUri;
        var separator = baseUri.Contains('?') ? '&' : '?';
        return $"{baseUri}{separator}{Uri.EscapeDataString(name)}={Uri.EscapeDataString(text)}";
    }

    /// <summary>Builds a "?a=1&amp;b=2" suffix from a sequence of (name, value) pairs.</summary>
    protected static string QueryString(params (string Name, object? Value)[] pairs)
    {
        var uri = string.Empty;
        foreach (var (name, value) in pairs)
        {
            uri = AppendQuery(uri, name, value);
        }
        return uri;
    }

    private static PagedResult<T> ParsePagedResponse<T>(string json, int pageNumber, int pageSize)
    {
        using var document = JsonDocument.Parse(json);
        if (document.RootElement.ValueKind == JsonValueKind.Array)
        {
            var items = JsonSerializer.Deserialize<T[]>(json, JsonOptions) ?? Array.Empty<T>();
            return PagedResult<T>.FromCollection(items, pageNumber, pageSize);
        }

        var page = JsonSerializer.Deserialize<PagedResult<T>>(json, JsonOptions);
        if (page is null)
        {
            throw new JsonException("Paged response was empty.");
        }

        return page;
    }
}
