using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace ChuA.ERP.Web.Mvc.Tests.Extensions;

public class ControllerExtensionsTests
{
    private static ITempDataDictionary NewTempData()
    {
        var http = new DefaultHttpContext();
        return new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
    }

    [Fact]
    public void AddToast_then_ConsumeToasts_should_return_added_items_and_clear()
    {
        var tempData = NewTempData();
        tempData.AddToast("Saved", ToastLevel.Success, "Success");

        var toasts = tempData.ConsumeToasts();
        toasts.Should().ContainSingle(t => t.Text == "Saved" && t.Level == ToastLevel.Success && t.Title == "Success");

        tempData.ConsumeToasts().Should().BeEmpty();
    }

    [Fact]
    public void AddResultErrors_should_copy_errors_into_modelstate()
    {
        var modelState = new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary();
        var result = Result.Failure(new[]
        {
            new Error("vendor.legalname.required", "Legal name is required", "LegalName"),
            new Error("vendor.invalid", "General error"),
        });

        modelState.AddResultErrors(result);

        modelState.IsValid.Should().BeFalse();
        modelState["LegalName"]!.Errors.Should().ContainSingle(e => e.ErrorMessage == "Legal name is required");
        modelState[""]!.Errors.Should().ContainSingle(e => e.ErrorMessage == "General error");
    }
}
