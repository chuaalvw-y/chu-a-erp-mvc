// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Models.Notifications;

namespace ChuA.ERP.Web.Mvc.Services;

/// <summary>
/// In-memory, per-process <see cref="INotificationStore"/>. Keeps the most-recent
/// <see cref="MaxPerUser"/> notifications per user; older entries are evicted on insert.
/// Thread-safe via a single per-user lock; suitable for V1 single-instance deployments.
/// </summary>
/// <remarks>
/// Scale-out (multiple MVC instances behind a load balancer) would require either a Redis
/// SignalR backplane or a DB-backed store — both deferred to a follow-up.
/// </remarks>
public sealed class InMemoryNotificationStore : INotificationStore
{
    /// <summary>Maximum notifications kept per user. Older entries are evicted FIFO.</summary>
    public const int MaxPerUser = 50;

    private readonly Dictionary<string, LinkedList<NotificationDto>> _byUser = new(StringComparer.OrdinalIgnoreCase);
    private readonly object _gate = new();

    /// <inheritdoc />
    public IReadOnlyList<NotificationDto> GetRecent(string userId, int max = 25)
    {
        if (string.IsNullOrWhiteSpace(userId) || max <= 0) return Array.Empty<NotificationDto>();
        lock (_gate)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return Array.Empty<NotificationDto>();
            return list.Take(max).ToArray();
        }
    }

    /// <inheritdoc />
    public int CountUnread(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return 0;
        lock (_gate)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return 0;
            return list.Count(n => n.IsUnread);
        }
    }

    /// <inheritdoc />
    public void Add(NotificationDto notification)
    {
        ArgumentNullException.ThrowIfNull(notification);
        if (string.IsNullOrWhiteSpace(notification.UserId)) return;
        lock (_gate)
        {
            if (!_byUser.TryGetValue(notification.UserId, out var list))
            {
                list = new LinkedList<NotificationDto>();
                _byUser[notification.UserId] = list;
            }
            list.AddFirst(notification);
            while (list.Count > MaxPerUser)
            {
                list.RemoveLast();
            }
        }
    }

    /// <inheritdoc />
    public NotificationDto? MarkRead(string userId, Guid notificationId, DateTime whenUtc)
    {
        if (string.IsNullOrWhiteSpace(userId) || notificationId == Guid.Empty) return null;
        lock (_gate)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return null;
            for (var node = list.First; node is not null; node = node.Next)
            {
                if (node.Value.Id == notificationId)
                {
                    if (!node.Value.IsUnread) return node.Value;
                    var updated = node.Value.MarkRead(whenUtc);
                    node.Value = updated;
                    return updated;
                }
            }
            return null;
        }
    }

    /// <inheritdoc />
    public int MarkAllRead(string userId, DateTime whenUtc)
    {
        if (string.IsNullOrWhiteSpace(userId)) return 0;
        lock (_gate)
        {
            if (!_byUser.TryGetValue(userId, out var list)) return 0;
            var flipped = 0;
            for (var node = list.First; node is not null; node = node.Next)
            {
                if (node.Value.IsUnread)
                {
                    node.Value = node.Value.MarkRead(whenUtc);
                    flipped++;
                }
            }
            return flipped;
        }
    }
}
