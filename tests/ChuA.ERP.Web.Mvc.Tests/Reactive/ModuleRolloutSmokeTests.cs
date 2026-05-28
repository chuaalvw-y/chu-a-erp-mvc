// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ViewModels.ChartOfAccounts;
using ChuA.ERP.Web.Mvc.ViewModels.Customers;
using ChuA.ERP.Web.Mvc.ViewModels.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>
/// Wiring-only smoke tests for the Phase 3 module rollout (Customers, Inventory,
/// ChartOfAccounts). The detailed semantics of the partial-rendering pattern are already
/// covered by <see cref="VendorsControllerReactiveTests"/>; these tests just lock down
/// that each new controller's IndexPartial is wired to the correct view.
/// </summary>
public class ModuleRolloutSmokeTests
{
    private static T BuildController<T>(T controller) where T : Controller
    {
        var http = new DefaultHttpContext();
        controller.TempData = new TempDataDictionary(http, Mock.Of<ITempDataProvider>());
        controller.ControllerContext = new ControllerContext { HttpContext = http };
        var urlHelper = new Mock<IUrlHelper>();
        urlHelper.Setup(u => u.Action(It.IsAny<UrlActionContext>())).Returns("/stub");
        controller.Url = urlHelper.Object;
        return controller;
    }

    [Fact]
    public async Task Customers_IndexPartial_should_return_customer_rows_partial()
    {
        var customers = new Mock<ICustomersApiClient>();
        customers.Setup(c => c.ListAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<CustomerDto>>.Success(PagedResult<CustomerDto>.Empty()));
        var ctrl = BuildController(new CustomersController(customers.Object));

        var result = await ctrl.IndexPartial(null, 1, 25, null, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_CustomerRowsPartial");
        partial.Model.Should().BeOfType<CustomerListViewModel>();
    }

    [Fact]
    public async Task Inventory_IndexPartial_should_return_item_rows_partial()
    {
        var inventory = new Mock<IInventoryApiClient>();
        inventory.Setup(i => i.ListAsync(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<ItemDto>>.Success(PagedResult<ItemDto>.Empty()));
        var ctrl = BuildController(new InventoryController(inventory.Object));

        var result = await ctrl.IndexPartial(null, 1, 25, null, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_ItemRowsPartial");
        partial.Model.Should().BeOfType<InventoryListViewModel>();
    }

    [Fact]
    public async Task ChartOfAccounts_IndexPartial_should_pass_through_accountType_filter()
    {
        var coa = new Mock<IChartOfAccountsApiClient>();
        coa.Setup(c => c.ListAsync(It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PagedResult<ChartOfAccountDto>>.Success(PagedResult<ChartOfAccountDto>.Empty()));
        var ctrl = BuildController(new ChartOfAccountsController(coa.Object));

        var result = await ctrl.IndexPartial("Asset", "cash", 1, 25, null, CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_ChartOfAccountRowsPartial");
        var vm = partial.Model.Should().BeOfType<ChartOfAccountListViewModel>().Subject;
        vm.AccountType.Should().Be("Asset");
        vm.Search.Should().Be("cash");
    }

    [Fact]
    public void Customers_CreateModal_GET_should_return_empty_form()
    {
        var ctrl = BuildController(new CustomersController(new Mock<ICustomersApiClient>().Object));
        var result = ctrl.CreateModal();
        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_CustomerFormModal");
        partial.Model.Should().BeOfType<CustomerFormViewModel>().Which.IsEdit.Should().BeFalse();
    }

    [Fact]
    public void Inventory_CreateModal_GET_should_return_empty_form()
    {
        var ctrl = BuildController(new InventoryController(new Mock<IInventoryApiClient>().Object));
        var result = ctrl.CreateModal();
        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_ItemFormModal");
        partial.Model.Should().BeOfType<ItemFormViewModel>().Which.IsEdit.Should().BeFalse();
    }

    [Fact]
    public void ChartOfAccounts_CreateModal_GET_should_return_empty_form()
    {
        var ctrl = BuildController(new ChartOfAccountsController(new Mock<IChartOfAccountsApiClient>().Object));
        var result = ctrl.CreateModal();
        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_ChartOfAccountFormModal");
        partial.Model.Should().BeOfType<ChartOfAccountFormViewModel>().Which.IsEdit.Should().BeFalse();
    }
}
