// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.ViewModels.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>Tests for the partial / count endpoints added to WorkflowController in Wave 1.</summary>
public class WorkflowControllerReactiveTests
{
    private static WorkflowController BuildSut(Mock<IWorkflowApiClient> workflow)
    {
        var publisher = new Mock<INotificationPublisher>();
        publisher.Setup(p => p.BroadcastEventAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object?>(), It.IsAny<CancellationToken>()))
                 .Returns(Task.CompletedTask);
        var ctrl = new WorkflowController(workflow.Object, publisher.Object, NullLogger<WorkflowController>.Instance);
        ctrl.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        return ctrl;
    }

    private static WorkflowApprovalDto SampleTask() =>
        new(
            Id: Guid.NewGuid(),
            WorkflowInstanceId: Guid.NewGuid(),
            StepNumber: 1,
            AssignedUserId: Guid.NewGuid(),
            Decision: "Pending",
            AssignedAt: DateTimeOffset.UtcNow,
            DueAt: null,
            DecidedAt: null,
            Comments: null,
            DelegatedToUserId: null,
            EscalatedFromUserId: null);

    [Fact]
    public async Task InboxPartial_should_return_partial_with_table_view_name_and_tasks()
    {
        var workflow = new Mock<IWorkflowApiClient>();
        workflow.Setup(w => w.ListTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WorkflowApprovalDto>>.Success(new[] { SampleTask(), SampleTask() }));
        var ctrl = BuildSut(workflow);

        var result = await ctrl.InboxPartial(CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_WorkflowInboxTable");
        var vm = partial.Model.Should().BeOfType<WorkflowListViewModel>().Subject;
        vm.Tasks.Should().HaveCount(2);
    }

    [Fact]
    public async Task InboxPartial_should_return_empty_partial_on_api_failure()
    {
        var workflow = new Mock<IWorkflowApiClient>();
        workflow.Setup(w => w.ListTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WorkflowApprovalDto>>.Failure(new Error("api.error", "boom")));
        var ctrl = BuildSut(workflow);

        var result = await ctrl.InboxPartial(CancellationToken.None);

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        var vm = partial.Model.Should().BeOfType<WorkflowListViewModel>().Subject;
        vm.Tasks.Should().BeEmpty();
    }

    [Fact]
    public async Task Count_should_return_json_with_count_property()
    {
        var workflow = new Mock<IWorkflowApiClient>();
        workflow.Setup(w => w.ListTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WorkflowApprovalDto>>.Success(new[] { SampleTask(), SampleTask(), SampleTask() }));
        var ctrl = BuildSut(workflow);

        var result = await ctrl.Count(CancellationToken.None);

        var json = result.Should().BeOfType<JsonResult>().Subject;
        var count = json.Value!.GetType().GetProperty("count")!.GetValue(json.Value);
        count.Should().Be(3);
    }

    [Fact]
    public async Task Count_should_return_zero_when_api_fails()
    {
        var workflow = new Mock<IWorkflowApiClient>();
        workflow.Setup(w => w.ListTasksAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<WorkflowApprovalDto>>.Failure(new Error("api.error", "boom")));
        var ctrl = BuildSut(workflow);

        var json = (JsonResult)await ctrl.Count(CancellationToken.None);
        var count = json.Value!.GetType().GetProperty("count")!.GetValue(json.Value);
        count.Should().Be(0);
    }
}
