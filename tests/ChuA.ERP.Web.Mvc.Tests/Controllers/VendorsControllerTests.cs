using ChuA.ERP.Web.Mvc.ViewModels.Vendors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Controllers;

public class VendorsControllerTests
{
    private static VendorsController BuildSut(Mock<IVendorsApiClient> vendors)
    {
        var ctrl = new VendorsController(vendors.Object);
        var http = new DefaultHttpContext();
        var tempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
        ctrl.TempData = tempData;
        ctrl.ControllerContext = new ControllerContext { HttpContext = http };
        // Url helper stub so breadcrumb Url.Action calls don't NPE
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/stub");
        ctrl.Url = urlHelper.Object;
        return ctrl;
    }

    private static VendorDto Sample(string code = "V-1") =>
        new(Guid.NewGuid(), Guid.NewGuid(), code, "Acme", "USD", 30, false);

    [Fact]
    public async Task Index_should_return_View_with_paged_list_on_success()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var data = Enumerable.Range(1, 30).Select(i => Sample($"V-{i}")).ToArray();
        vendors.Setup(v => v.ListAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<VendorDto>>.Success(PagedResult<VendorDto>.FromCollection(data, 2, 10)));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.Index(search: null, pageNumber: 2, pageSize: 10, sort: null, CancellationToken.None);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        var vm = view.Model.Should().BeOfType<VendorListViewModel>().Subject;
        vm.Page.PageNumber.Should().Be(2);
        vm.Page.PageSize.Should().Be(10);
        vm.Page.TotalCount.Should().Be(30);
        vm.Page.Items.Should().HaveCount(10);
    }

    [Fact]
    public async Task Index_should_return_empty_view_with_modelstate_errors_on_failure()
    {
        var vendors = new Mock<IVendorsApiClient>();
        vendors.Setup(v => v.ListAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<VendorDto>>.Failure(new Error("api.error", "boom")));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.Index(search: null, pageNumber: 1, pageSize: 25, sort: null, CancellationToken.None);

        result.Should().BeOfType<ViewResult>();
        ctrl.ModelState.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Create_POST_should_redirect_to_details_on_success()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var newVendor = Sample("V-9");
        vendors.Setup(v => v.CreateAsync(It.IsAny<CreateVendorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(newVendor));
        var ctrl = BuildSut(vendors);
        var model = new VendorFormViewModel { VendorCode = "V-9", LegalName = "Acme", DefaultCurrencyCode = "USD", PaymentTermsDays = 30 };

        var result = await ctrl.Create(model, CancellationToken.None);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(VendorsController.Details));
        redirect.RouteValues!["id"].Should().Be(newVendor.Id);
    }

    [Fact]
    public async Task Create_POST_should_redisplay_form_on_invalid_modelstate()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var ctrl = BuildSut(vendors);
        ctrl.ModelState.AddModelError("VendorCode", "Required");
        var model = new VendorFormViewModel();

        var result = await ctrl.Create(model, CancellationToken.None);

        var view = result.Should().BeOfType<ViewResult>().Subject;
        view.Model.Should().BeSameAs(model);
        vendors.Verify(v => v.CreateAsync(It.IsAny<CreateVendorRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteConfirmed_should_redirect_to_index_on_success()
    {
        var vendors = new Mock<IVendorsApiClient>();
        vendors.Setup(v => v.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(Result.Success());
        var ctrl = BuildSut(vendors);

        var result = await ctrl.DeleteConfirmed(Guid.NewGuid(), CancellationToken.None);

        var redirect = result.Should().BeOfType<RedirectToActionResult>().Subject;
        redirect.ActionName.Should().Be(nameof(VendorsController.Index));
    }
}
