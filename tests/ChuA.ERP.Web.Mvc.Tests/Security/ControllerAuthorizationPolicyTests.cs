// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Reflection;
using Microsoft.AspNetCore.Authorization;

namespace ChuA.ERP.Web.Mvc.Tests.Security;

public class ControllerAuthorizationPolicyTests
{
    [Theory]
    [InlineData(typeof(VendorsController), "Edit", AuthorizationPolicies.VendorUpdate)]
    [InlineData(typeof(VendorsController), "Delete", AuthorizationPolicies.VendorDelete)]
    [InlineData(typeof(VendorsController), "DeleteConfirmed", AuthorizationPolicies.VendorDelete)]
    [InlineData(typeof(CustomersController), "Edit", AuthorizationPolicies.CustomerUpdate)]
    [InlineData(typeof(CustomersController), "Delete", AuthorizationPolicies.CustomerDelete)]
    [InlineData(typeof(CustomersController), "DeleteConfirmed", AuthorizationPolicies.CustomerDelete)]
    [InlineData(typeof(BillsController), "Edit", AuthorizationPolicies.BillUpdate)]
    [InlineData(typeof(BillsController), "Delete", AuthorizationPolicies.BillDelete)]
    [InlineData(typeof(BillsController), "DeleteConfirmed", AuthorizationPolicies.BillDelete)]
    [InlineData(typeof(InvoicesController), "Edit", AuthorizationPolicies.InvoiceUpdate)]
    [InlineData(typeof(InvoicesController), "Delete", AuthorizationPolicies.InvoiceDelete)]
    [InlineData(typeof(InvoicesController), "DeleteConfirmed", AuthorizationPolicies.InvoiceDelete)]
    [InlineData(typeof(ChartOfAccountsController), "Edit", AuthorizationPolicies.ChartOfAccountUpdate)]
    [InlineData(typeof(ChartOfAccountsController), "Delete", AuthorizationPolicies.ChartOfAccountDelete)]
    [InlineData(typeof(ChartOfAccountsController), "DeleteConfirmed", AuthorizationPolicies.ChartOfAccountDelete)]
    [InlineData(typeof(InventoryController), "Edit", AuthorizationPolicies.InventoryUpdate)]
    [InlineData(typeof(InventoryController), "Delete", AuthorizationPolicies.InventoryDelete)]
    [InlineData(typeof(InventoryController), "DeleteConfirmed", AuthorizationPolicies.InventoryDelete)]
    [InlineData(typeof(PurchaseOrdersController), "Edit", AuthorizationPolicies.PurchaseOrderUpdate)]
    [InlineData(typeof(PurchaseOrdersController), "Delete", AuthorizationPolicies.PurchaseOrderDelete)]
    [InlineData(typeof(PurchaseOrdersController), "DeleteConfirmed", AuthorizationPolicies.PurchaseOrderDelete)]
    [InlineData(typeof(SalesOrdersController), "Edit", AuthorizationPolicies.SalesOrderUpdate)]
    [InlineData(typeof(SalesOrdersController), "Delete", AuthorizationPolicies.SalesOrderDelete)]
    [InlineData(typeof(SalesOrdersController), "DeleteConfirmed", AuthorizationPolicies.SalesOrderDelete)]
    [InlineData(typeof(JournalEntriesController), "Create", AuthorizationPolicies.JournalEntryCreate)]
    [InlineData(typeof(JournalEntriesController), "Edit", AuthorizationPolicies.JournalEntryUpdate)]
    [InlineData(typeof(JournalEntriesController), "Delete", AuthorizationPolicies.JournalEntryDelete)]
    [InlineData(typeof(JournalEntriesController), "DeleteConfirmed", AuthorizationPolicies.JournalEntryDelete)]
    [InlineData(typeof(WorkflowController), "Reassign", AuthorizationPolicies.WorkflowApprovalReassign)]
    [InlineData(typeof(WorkflowDefinitionsController), "Create", AuthorizationPolicies.WorkflowDefinitionCreate)]
    [InlineData(typeof(WorkflowDefinitionsController), "AddStep", AuthorizationPolicies.WorkflowDefinitionUpdate)]
    [InlineData(typeof(WorkflowDefinitionsController), "RemoveStep", AuthorizationPolicies.WorkflowDefinitionUpdate)]
    [InlineData(typeof(WorkflowDefinitionsController), "AddApprover", AuthorizationPolicies.WorkflowDefinitionUpdate)]
    [InlineData(typeof(WorkflowDefinitionsController), "RemoveApprover", AuthorizationPolicies.WorkflowDefinitionUpdate)]
    [InlineData(typeof(WorkflowDefinitionsController), "Publish", AuthorizationPolicies.WorkflowDefinitionPublish)]
    [InlineData(typeof(WorkflowDefinitionsController), "Retire", AuthorizationPolicies.WorkflowDefinitionRetire)]
    [InlineData(typeof(WorkflowDefinitionsController), "Clone", AuthorizationPolicies.WorkflowDefinitionCreate)]
    [InlineData(typeof(WorkflowConfigurationsController), "Create", AuthorizationPolicies.WorkflowConfigManage)]
    [InlineData(typeof(WorkflowConfigurationsController), "Edit", AuthorizationPolicies.WorkflowConfigManage)]
    [InlineData(typeof(WorkflowConfigurationsController), "Activate", AuthorizationPolicies.WorkflowConfigManage)]
    [InlineData(typeof(WorkflowConfigurationsController), "Deactivate", AuthorizationPolicies.WorkflowConfigManage)]
    [InlineData(typeof(WorkflowInstancesController), "Cancel", AuthorizationPolicies.WorkflowInstanceCancel)]
    public void Mutating_actions_should_use_specific_policies(Type controllerType, string methodName, string expectedPolicy)
    {
        var methods = controllerType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(method => method.Name == methodName)
            .ToArray();

        methods.Should().NotBeEmpty();
        var policies = methods
            .SelectMany(method => method.GetCustomAttributes<AuthorizeAttribute>())
            .Where(attribute => attribute.Policy is not null)
            .Select(attribute => attribute.Policy)
            .Distinct()
            .ToArray();

        policies.Should().Contain(expectedPolicy);
    }
}
