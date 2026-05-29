// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Models.Notifications;

/// <summary>
/// Transport + storage record for a single user-scoped notification surfaced in the
/// topbar notification center. Immutable by construction; the store always returns a new
/// instance when marking-read mutates state.
/// </summary>
public sealed record NotificationDto
{
    /// <summary>Stable unique identifier (server-generated GUID).</summary>
    public required Guid Id { get; init; }

    /// <summary>The user this notification belongs to (matches the SignalR <c>user-{id}</c> group).</summary>
    public required string UserId { get; init; }

    /// <summary>One-line headline shown in the dropdown.</summary>
    public required string Title { get; init; }

    /// <summary>Optional longer body. Plain-text only.</summary>
    public string? Body { get; init; }

    /// <summary>Severity / styling hint.</summary>
    public NotificationLevel Level { get; init; } = NotificationLevel.Info;

    /// <summary>UTC moment the notification was generated.</summary>
    public required DateTime CreatedUtc { get; init; }

    /// <summary>UTC moment the user marked the notification read, when set.</summary>
    public DateTime? ReadUtc { get; init; }

    /// <summary>Optional deep link / route the dropdown item navigates to when clicked.</summary>
    public string? Link { get; init; }

    /// <summary>Domain category — e.g. <c>workflow</c>, <c>vendor</c>. Used by the UI for icon selection.</summary>
    public string? Category { get; init; }

    /// <summary>True when the notification has not yet been marked-read.</summary>
    public bool IsUnread => ReadUtc is null;

    /// <summary>Returns a copy with <see cref="ReadUtc"/> populated.</summary>
    public NotificationDto MarkRead(DateTime whenUtc) => this with { ReadUtc = whenUtc };
}
