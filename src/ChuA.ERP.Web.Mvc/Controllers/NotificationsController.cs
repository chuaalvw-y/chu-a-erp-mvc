// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using ChuA.ERP.Web.Mvc.Models.Notifications;
using ChuA.ERP.Web.Mvc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChuA.ERP.Web.Mvc.Controllers;

/// <summary>
/// Backs the topbar notification center. Every endpoint is scoped to the calling user;
/// the store never exposes another user's notifications regardless of the requested id.
/// </summary>
[Authorize]
public sealed class NotificationsController : Controller
{
    private readonly INotificationStore _store;
    private readonly ILogger<NotificationsController> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>Constructs the controller with its injected dependencies.</summary>
    public NotificationsController(
        INotificationStore store,
        ILogger<NotificationsController> logger,
        TimeProvider? timeProvider = null)
    {
        _store = store;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>Returns the dropdown's item-list partial — the part inside the dropdown menu.</summary>
    [HttpGet]
    public IActionResult Items()
    {
        return PartialView("_NotificationItems", BuildModel());
    }

    /// <summary>Returns just the unread count as JSON — used as a polling fallback target.</summary>
    [HttpGet]
    public IActionResult Count()
    {
        var userId = CurrentUserId;
        var count = string.IsNullOrWhiteSpace(userId) ? 0 : _store.CountUnread(userId);
        return Json(new { count });
    }

    /// <summary>Marks a single notification read. Antiforgery-protected POST.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkRead(Guid id)
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();
        var updated = _store.MarkRead(userId, id, _timeProvider.GetUtcNow().UtcDateTime);
        if (updated is null)
        {
            return NotFound();
        }
        return Json(new { id = updated.Id, readUtc = updated.ReadUtc, unreadCount = _store.CountUnread(userId) });
    }

    /// <summary>Marks every notification for the current user read. Antiforgery-protected POST.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAllRead()
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId)) return Forbid();
        var flipped = _store.MarkAllRead(userId, _timeProvider.GetUtcNow().UtcDateTime);
        _logger.LogDebug("Marked {Flipped} notifications read for {UserId}", flipped, userId);
        return Json(new { flipped, unreadCount = 0 });
    }

    private NotificationCenterViewModel BuildModel()
    {
        var userId = CurrentUserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new NotificationCenterViewModel();
        }
        return new NotificationCenterViewModel
        {
            Items = _store.GetRecent(userId),
            UnreadCount = _store.CountUnread(userId),
        };
    }

    private string? CurrentUserId =>
        User.FindFirstValue("sub")
        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? User.Identity?.Name;
}
