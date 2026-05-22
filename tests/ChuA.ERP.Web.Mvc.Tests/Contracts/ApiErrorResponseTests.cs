// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Tests.Contracts;

public class ApiErrorResponseTests
{
    [Fact]
    public void ToErrors_should_flatten_validation_dictionary()
    {
        var sut = new ApiErrorResponse
        {
            Status = 400,
            Errors = new Dictionary<string, string[]>
            {
                ["LegalName"] = new[] { "vendor.legalname.required: Legal name is required." },
                ["_"] = new[] { "vendor.invalid: General problem" },
            }
        };
        var errs = sut.ToErrors();
        errs.Should().HaveCount(2);
        errs.Should().Contain(e => e.Target == "LegalName" && e.Code == "vendor.legalname.required");
        errs.Should().Contain(e => e.Target == null && e.Code == "vendor.invalid");
    }

    [Fact]
    public void ToErrors_should_fall_back_to_detail_when_dictionary_empty()
    {
        var sut = new ApiErrorResponse
        {
            Status = 500,
            ErrorCode = "api.error",
            Detail = "Unexpected failure",
        };
        var errs = sut.ToErrors();
        errs.Should().ContainSingle(e => e.Code == "api.error" && e.Message == "Unexpected failure");
    }
}
