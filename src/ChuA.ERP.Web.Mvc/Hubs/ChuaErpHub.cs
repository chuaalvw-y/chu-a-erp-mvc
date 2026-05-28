// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChuA.ERP.Web.Mvc.Hubs;

/// <summary>
/// Single MVC-hosted SignalR hub for ERP UX reactivity. Mirrors the Dashboard module's
/// <c>DashboardHub</c> pattern but is owned by the MVC shell so the bulk of reactive
/// notifications (workflow inbox updates, notification center, future module updates)
/// do not require an API-side hub.
///
/// <para>
/// On connect every cookie-authenticated user is added to <c>user-{userId}</c>, which is
/// the canonical broadcast group for per-user push (badge counts, "your row was saved",
/// etc.). Module-specific opt-in topic groups (<c>workflow-inbox-{userId}</c>,
/// <c>vendor-grid-{userId}</c>, …) are joined via <see cref="SubscribeToTopicAsync"/> so
/// pages only receive the events they care about.
/// </para>
///
/// <para>
/// V1 cross-user broadcast is limited to events the MVC itself triggers (the current
/// user's own actions). True cross-user push — e.g. "user X just assigned you an
/// approval" — requires the API to publish into a hub it owns, which is deliberately
/// deferred to a follow-up so this wave can ship without touching the API.
/// </para>
/// </summary>
[Authorize]
public sealed class ChuaErpHub : Hub
{
    /// <summary>Group prefix for a user's connected tabs/devices.</summary>
    public const string UserGroupPrefix = "user-";

    /// <summary>SignalR mount path. Kept here so MVC view scripts and Program.cs share a constant.</summary>
    public const string HubPath = "/hubs/erp";

    private readonly ILogger<ChuaErpHub> _logger;

    /// <summary>Constructs the hub with its injected dependencies.</summary>
    public ChuaErpHub(ILogger<ChuaErpHub> logger)
    {
        _logger = logger;
    }

    /// <inheritdoc />
    public override async Task OnConnectedAsync()
    {
        var userId = ResolveUserId(Context.User);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId)).ConfigureAwait(false);
            _logger.LogDebug(
                "Hub connection {ConnectionId} joined {Group}",
                Context.ConnectionId,
                UserGroup(userId));
        }
        await base.OnConnectedAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Client opt-in subscription to a free-form topic group — used by module pages to
    /// receive events scoped to that module/screen (e.g. <c>workflow-inbox</c>). The hub
    /// always namespaces the requested topic to the calling user, so two users cannot
    /// observe each other's per-page event streams.
    /// </summary>
    public async Task SubscribeToTopicAsync(string topic)
    {
        if (!TryNormalizeTopic(topic, out var group)) return;
        await Groups.AddToGroupAsync(Context.ConnectionId, group).ConfigureAwait(false);
        _logger.LogDebug(
            "Hub connection {ConnectionId} subscribed to {Group}",
            Context.ConnectionId,
            group);
    }

    /// <summary>Client opt-out from a topic group previously joined with <see cref="SubscribeToTopicAsync"/>.</summary>
    public async Task UnsubscribeFromTopicAsync(string topic)
    {
        if (!TryNormalizeTopic(topic, out var group)) return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, group).ConfigureAwait(false);
    }

    /// <summary>Computes the canonical user-scoped group name for the supplied user id.</summary>
    public static string UserGroup(string userId) => UserGroupPrefix + userId;

    /// <summary>
    /// Computes a user-namespaced topic group name. Returning <c>topic-{userId}</c> ensures a
    /// caller subscribing to "workflow-inbox" cannot accidentally receive another user's events.
    /// </summary>
    public static string TopicGroup(string topic, string userId)
        => $"{topic}-{userId}";

    private bool TryNormalizeTopic(string topic, out string group)
    {
        group = string.Empty;
        if (string.IsNullOrWhiteSpace(topic)) return false;
        var userId = ResolveUserId(Context.User);
        if (string.IsNullOrWhiteSpace(userId)) return false;
        group = TopicGroup(topic.Trim().ToLowerInvariant(), userId);
        return true;
    }

    /// <summary>
    /// Picks the most stable identifier on the supplied principal. Falls back across the
    /// usual OIDC identifier-style claims so a DevLogin user (no <c>sub</c>) still gets a
    /// stable per-user group.
    /// </summary>
    private static string? ResolveUserId(ClaimsPrincipal? principal)
    {
        if (principal is null) return null;
        return principal.FindFirstValue("sub")
            ?? principal.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? principal.FindFirstValue("oid")
            ?? principal.FindFirstValue("preferred_username")
            ?? principal.Identity?.Name;
    }
}
