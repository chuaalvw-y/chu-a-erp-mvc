// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Models.Notifications;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Per-user store of recent notifications backing the topbar notification center.
/// V1 implementation is in-memory and per-process (see <see cref="InMemoryNotificationStore"/>);
/// the abstraction exists so a DB-backed implementation can be swapped in later without
/// touching publishers/controllers.
/// </summary>
public interface INotificationStore
{
    /// <summary>Returns the most-recent notifications for the supplied user, newest first.</summary>
    IReadOnlyList<NotificationDto> GetRecent(string userId, int max = 25);

    /// <summary>Returns the unread count for the supplied user.</summary>
    int CountUnread(string userId);

    /// <summary>Persists the supplied notification. Implementations may evict older entries to keep the per-user cap.</summary>
    void Add(NotificationDto notification);

    /// <summary>Marks a single notification read. Returns the updated notification, or null when not found.</summary>
    NotificationDto? MarkRead(string userId, Guid notificationId, DateTime whenUtc);

    /// <summary>Marks every notification for the user read. Returns the count actually flipped.</summary>
    int MarkAllRead(string userId, DateTime whenUtc);
}
