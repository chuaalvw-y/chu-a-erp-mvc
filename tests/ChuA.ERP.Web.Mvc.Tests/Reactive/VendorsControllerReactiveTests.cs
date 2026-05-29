// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ViewModels.Vendors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>
/// Tests for the reactive partial / modal endpoints added to VendorsController in Wave 1.
/// </summary>
public class VendorsControllerReactiveTests
{
    private static VendorsController BuildSut(Mock<IVendorsApiClient> vendors)
    {
        var ctrl = new VendorsController(vendors.Object);
        var http = new DefaultHttpContext();
        ctrl.TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
        ctrl.ControllerContext = new ControllerContext { HttpContext = http };
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/stub");
        ctrl.Url = urlHelper.Object;
        return ctrl;
    }

    private static VendorDto Sample(string code = "V-1") =>
        new(Guid.NewGuid(), Guid.NewGuid(), code, "Acme", "USD", 30, false);

    [Fact]
    public async Task IndexPartial_should_return_rows_partial_with_paged_model()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var data = Enumerable.Range(1, 5).Select(i => Sample("V-" + i)).ToArray();
        vendors.Setup(v => v.ListAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<VendorDto>>.Success(PagedResult<VendorDto>.FromCollection(data, 1, 25)));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.IndexPartial("acme", 1, 25, null, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_VendorRowsPartial");
        var vm = partial.Model.Should().BeOfType<VendorListViewModel>().Subject;
        vm.Page.Items.Should().HaveCount(5);
        vm.Search.Should().Be("acme");
    }

    [Fact]
    public async Task RowPartial_should_return_vendor_row_when_found()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var sample = Sample("V-9");
        vendors.Setup(v => v.GetAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(sample));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.RowPartial(sample.Id, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_VendorRow");
        partial.Model.Should().BeOfType<VendorDto>().Which.Id.Should().Be(sample.Id);
    }

    [Fact]
    public async Task RowPartial_should_return_NotFound_when_api_fails()
    {
        var vendors = new Mock<IVendorsApiClient>();
        vendors.Setup(v => v.GetAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Failure(new Error("not.found", "missing")));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.RowPartial(Guid.NewGuid(), CancellationToken.None);

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void CreateModal_GET_should_return_empty_form()
    {
        var ctrl = BuildSut(new Mock<IVendorsApiClient>());

        var result = ctrl.CreateModal();

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_VendorFormModal");
        var vm = partial.Model.Should().BeOfType<VendorFormViewModel>().Subject;
        vm.IsEdit.Should().BeFalse();
    }

    [Fact]
    public async Task CreateModal_POST_invalid_should_return_422_with_form()
    {
        var ctrl = BuildSut(new Mock<IVendorsApiClient>());
        ctrl.ModelState.AddModelError("VendorCode", "required");

        var result = await ctrl.CreateModal(new VendorFormViewModel(), CancellationToken.None);

        result.Should().BeOfType<PartialViewResult>();
        ctrl.Response.StatusCode.Should().Be(StatusCodes.Status422UnprocessableEntity);
    }

    [Fact]
    public async Task CreateModal_POST_success_should_return_row_with_marker_headers()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var created = Sample("V-10");
        vendors.Setup(v => v.CreateAsync(It.IsAny<CreateVendorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(created));
        var ctrl = BuildSut(vendors);
        var model = new VendorFormViewModel { VendorCode = "V-10", LegalName = "Acme", DefaultCurrencyCode = "USD", PaymentTermsDays = 30 };

        var result = await ctrl.CreateModal(model, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_VendorRow");
        partial.Model.Should().BeOfType<VendorDto>().Which.Id.Should().Be(created.Id);
        ctrl.Response.Headers["X-Chua-Row-Action"].ToString().Should().Be("create");
        ctrl.Response.Headers["X-Chua-Vendor-Id"].ToString().Should().Be(created.Id.ToString());
    }

    [Fact]
    public async Task EditModal_GET_should_populate_form_from_dto()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var sample = Sample("V-7");
        vendors.Setup(v => v.GetAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(sample));
        var ctrl = BuildSut(vendors);

        var result = await ctrl.EditModal(sample.Id, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        var vm = partial.Model.Should().BeOfType<VendorFormViewModel>().Subject;
        vm.IsEdit.Should().BeTrue();
        vm.VendorCode.Should().Be("V-7");
    }

    [Fact]
    public async Task EditModal_POST_success_should_return_row_with_update_header()
    {
        var vendors = new Mock<IVendorsApiClient>();
        var sample = Sample("V-7");
        vendors.Setup(v => v.UpdateAsync(sample.Id, It.IsAny<UpdateVendorRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(sample));
        vendors.Setup(v => v.GetAsync(sample.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<VendorDto>.Success(sample));
        var ctrl = BuildSut(vendors);
        var model = new VendorFormViewModel { VendorCode = "V-7", LegalName = "Acme", DefaultCurrencyCode = "USD", PaymentTermsDays = 30 };

        var result = await ctrl.EditModal(sample.Id, model, CancellationToken.None);

        result.Should().BeOfType<PartialViewResult>();
        ctrl.Response.Headers["X-Chua-Row-Action"].ToString().Should().Be("update");
    }
}
