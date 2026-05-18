namespace ChuA.ERP.Web.Mvc.Security;

/// <summary>
/// HTTP header names exchanged with ChuA.ERP.Api. Mirrors
/// <c>ChuA.ERP.Api.Constants.ApiHeaders</c>.
/// </summary>
public static class ApiHeaders
{
    public const string CorrelationId = "X-Correlation-ID";
    public const string CompanyId = "X-Company-Id";
    public const string ApiVersion = "X-Api-Version";
    public const string ClientId = "X-Client-Id";
}
