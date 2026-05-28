// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models.Notifications;

/// <summary>
/// View model surfaced to the notification-center partials. Couples the recent items list
/// with the unread count so the dropdown header can render the badge without a second
/// pass over the store.
/// </summary>
public sealed class NotificationCenterViewModel
{
    /// <summary>The most-recent notifications, newest first.</summary>
    public IReadOnlyList<NotificationDto> Items { get; init; } = Array.Empty<NotificationDto>();

    /// <summary>Unread notification count for the badge.</summary>
    public int UnreadCount { get; init; }
}
