// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Models.Notifications;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Façade controllers use to emit notifications and reactive events. Combines two concerns
/// behind one well-known seam:
/// <list type="bullet">
///   <item>Persist a notification into <see cref="INotificationStore"/> so the dropdown
///   can paginate history.</item>
///   <item>Broadcast a <c>notificationReceived</c> SignalR event to the user's tabs so the
///   bell badge updates without a poll.</item>
/// </list>
/// Also exposes plain reactive-event broadcasts (no persistence) for cross-cutting page
/// signals like <c>workflowInboxChanged</c>.
/// </summary>
public interface INotificationPublisher
{
    /// <summary>
    /// Stores a notification for the current user and broadcasts <c>notificationReceived</c>
    /// to their connected tabs. Returns the persisted record.
    /// </summary>
    Task<NotificationDto> PublishAsync(
        string userId,
        string title,
        string? body = null,
        NotificationLevel level = NotificationLevel.Info,
        string? link = null,
        string? category = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a one-shot reactive event to the supplied user without writing to the
    /// notification store. Used for page-scoped signals like
    /// <c>workflowInboxChanged</c> where the UI re-fetches its partial rather than
    /// surfacing a notification.
    /// </summary>
    /// <param name="userId">Recipient user id (matches the SignalR group naming).</param>
    /// <param name="eventName">Client-side event name to invoke (e.g. <c>workflowInboxChanged</c>).</param>
    /// <param name="payload">Optional JSON-serialisable payload.</param>
    /// <param name="cancellationToken">Token observed during the underlying hub send.</param>
    Task BroadcastEventAsync(
        string userId,
        string eventName,
        object? payload = null,
        CancellationToken cancellationToken = default);
}
