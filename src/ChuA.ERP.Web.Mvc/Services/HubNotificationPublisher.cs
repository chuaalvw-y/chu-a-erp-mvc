// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Hubs;
using ChuA.ERP.Web.Mvc.Models.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// Default <see cref="INotificationPublisher"/>: persists notifications to
/// <see cref="INotificationStore"/> and broadcasts SignalR events to the recipient's
/// <c>user-{userId}</c> group via <see cref="ChuaErpHub"/>.
/// </summary>
public sealed class HubNotificationPublisher : INotificationPublisher
{
    /// <summary>Client-side method name invoked when a new notification is persisted.</summary>
    public const string NotificationReceivedEvent = "notificationReceived";

    private readonly IHubContext<ChuaErpHub> _hub;
    private readonly INotificationStore _store;
    private readonly ILogger<HubNotificationPublisher> _logger;
    private readonly TimeProvider _timeProvider;

    /// <summary>Constructs the publisher with its injected dependencies.</summary>
    public HubNotificationPublisher(
        IHubContext<ChuaErpHub> hub,
        INotificationStore store,
        ILogger<HubNotificationPublisher> logger,
        TimeProvider? timeProvider = null)
    {
        _hub = hub;
        _store = store;
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <inheritdoc />
    public async Task<NotificationDto> PublishAsync(
        string userId,
        string title,
        string? body = null,
        NotificationLevel level = NotificationLevel.Info,
        string? link = null,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("A user id is required.", nameof(userId));
        }
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("A title is required.", nameof(title));
        }

        var notification = new NotificationDto
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Body = body,
            Level = level,
            Link = link,
            Category = category,
            CreatedUtc = _timeProvider.GetUtcNow().UtcDateTime,
        };

        _store.Add(notification);

        try
        {
            await _hub.Clients
                .Group(ChuaErpHub.UserGroup(userId))
                .SendAsync(NotificationReceivedEvent, notification, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Persistence already succeeded — don't fail the originating controller just
            // because the live push couldn't be delivered (the badge will pick it up on
            // the next poll/page load).
            _logger.LogWarning(ex,
                "Failed to broadcast notificationReceived to user {UserId}; notification {NotificationId} persisted",
                userId, notification.Id);
        }

        return notification;
    }

    /// <inheritdoc />
    public async Task BroadcastEventAsync(
        string userId,
        string eventName,
        object? payload = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) return;
        if (string.IsNullOrWhiteSpace(eventName))
        {
            throw new ArgumentException("An event name is required.", nameof(eventName));
        }

        try
        {
            await _hub.Clients
                .Group(ChuaErpHub.UserGroup(userId))
                .SendAsync(eventName, payload, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to broadcast {EventName} to user {UserId}",
                eventName, userId);
        }
    }
}
