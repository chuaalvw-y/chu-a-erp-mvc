// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Text.Json.Serialization;

namespace ChuA.ERP.Web.Mvc.Contracts.Common;

/// <summary>
/// UI-side mirror of <c>ChuA.ERP.Api.Contracts.ApiErrorResponse</c>. The MVC parses this shape
/// out of every non-2xx HTTP response (the API always returns ProblemDetails on error).
/// </summary>
public sealed class ApiErrorResponse
{
    [JsonPropertyName("type")]
    public string? Type { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("status")]
    public int? Status { get; set; }

    [JsonPropertyName("detail")]
    public string? Detail { get; set; }

    [JsonPropertyName("instance")]
    public string? Instance { get; set; }

    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("correlationId")]
    public string? CorrelationId { get; set; }

    [JsonPropertyName("traceId")]
    public string? TraceId { get; set; }

    [JsonPropertyName("errors")]
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>Flattens the <see cref="Errors"/> map and ErrorCode/Detail into <see cref="Error"/> records.</summary>
    public IReadOnlyCollection<Error> ToErrors()
    {
        var list = new List<Error>();
        if (Errors is not null)
        {
            foreach (var (field, messages) in Errors)
            {
                var target = string.Equals(field, "_", StringComparison.Ordinal) ? null : field;
                foreach (var message in messages)
                {
                    var (code, text) = SplitCodeMessage(message);
                    list.Add(new Error(code ?? ErrorCode ?? "validation", text, target));
                }
            }
        }

        if (list.Count == 0 && !string.IsNullOrWhiteSpace(Detail))
        {
            list.Add(new Error(ErrorCode ?? "api.error", Detail!));
        }

        return list;
    }

    private static (string? Code, string Message) SplitCodeMessage(string raw)
    {
        var colon = raw.IndexOf(':');
        if (colon > 0 && colon < raw.Length - 1)
        {
            return (raw[..colon].Trim(), raw[(colon + 1)..].Trim());
        }
        return (null, raw);
    }
}
