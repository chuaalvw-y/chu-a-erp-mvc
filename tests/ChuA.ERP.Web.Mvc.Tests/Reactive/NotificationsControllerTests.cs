// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Models.Notifications;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>
/// Behavioural tests for <see cref="NotificationsController"/>. Confirms partial-rendering,
/// JSON payload shape, and the per-user store scoping invariant.
/// </summary>
public class NotificationsControllerTests
{
    private static (NotificationsController ctrl, InMemoryNotificationStore store) BuildSut(string userId = "alice")
    {
        var store = new InMemoryNotificationStore();
        var ctrl = new NotificationsController(store, NullLogger<NotificationsController>.Instance);
        var http = new DefaultHttpContext
        {
            User = new ClaimsPrincipal(new ClaimsIdentity(
                [new Claim("sub", userId)],
                CookieAuthenticationDefaults.AuthenticationScheme))
        };
        ctrl.ControllerContext = new ControllerContext { HttpContext = http };
        return (ctrl, store);
    }

    [Fact]
    public void Items_should_return_PartialView_with_NotificationCenterViewModel()
    {
        var (ctrl, store) = BuildSut("u1");
        store.Add(new NotificationDto { Id = Guid.NewGuid(), UserId = "u1", Title = "hi", CreatedUtc = DateTime.UtcNow });

        var result = ctrl.Items();

        var partial = result.Should().BeOfType<PartialViewResult>().Subject;
        partial.ViewName.Should().Be("_NotificationItems");
        var vm = partial.Model.Should().BeOfType<NotificationCenterViewModel>().Subject;
        vm.Items.Should().HaveCount(1);
        vm.UnreadCount.Should().Be(1);
    }

    [Fact]
    public void Items_should_only_return_current_users_notifications()
    {
        var (ctrl, store) = BuildSut("alice");
        store.Add(new NotificationDto { Id = Guid.NewGuid(), UserId = "alice", Title = "for-alice", CreatedUtc = DateTime.UtcNow });
        store.Add(new NotificationDto { Id = Guid.NewGuid(), UserId = "bob", Title = "for-bob", CreatedUtc = DateTime.UtcNow });

        var partial = (PartialViewResult)ctrl.Items();
        var vm = (NotificationCenterViewModel)partial.Model!;

        vm.Items.Should().ContainSingle().Which.Title.Should().Be("for-alice");
    }

    [Fact]
    public void Count_should_return_unread_count_as_json()
    {
        var (ctrl, store) = BuildSut("u1");
        store.Add(new NotificationDto { Id = Guid.NewGuid(), UserId = "u1", Title = "n", CreatedUtc = DateTime.UtcNow });

        var json = ctrl.Count().Should().BeOfType<JsonResult>().Subject;

        var prop = json.Value!.GetType().GetProperty("count")!.GetValue(json.Value);
        prop.Should().Be(1);
    }

    [Fact]
    public void MarkRead_should_flip_unread_and_return_updated_count()
    {
        var (ctrl, store) = BuildSut("u1");
        var n = new NotificationDto { Id = Guid.NewGuid(), UserId = "u1", Title = "n", CreatedUtc = DateTime.UtcNow };
        store.Add(n);

        var result = ctrl.MarkRead(n.Id);

        var json = result.Should().BeOfType<JsonResult>().Subject;
        var unread = json.Value!.GetType().GetProperty("unreadCount")!.GetValue(json.Value);
        unread.Should().Be(0);
        store.CountUnread("u1").Should().Be(0);
    }

    [Fact]
    public void MarkRead_should_return_NotFound_for_unknown_id()
    {
        var (ctrl, _) = BuildSut();

        var result = ctrl.MarkRead(Guid.NewGuid());

        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public void MarkAllRead_should_flip_every_unread_for_caller()
    {
        var (ctrl, store) = BuildSut("u1");
        for (var i = 0; i < 3; i++)
        {
            store.Add(new NotificationDto { Id = Guid.NewGuid(), UserId = "u1", Title = "n" + i, CreatedUtc = DateTime.UtcNow });
        }

        var result = ctrl.MarkAllRead();

        var json = result.Should().BeOfType<JsonResult>().Subject;
        var flipped = json.Value!.GetType().GetProperty("flipped")!.GetValue(json.Value);
        flipped.Should().Be(3);
        store.CountUnread("u1").Should().Be(0);
    }
}
