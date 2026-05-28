// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

using ChuA.ERP.Web.Mvc.Models.Notifications;

namespace ChuA.ERP.Web.Mvc.Tests.Reactive;

/// <summary>
/// Behavioural tests for <see cref="InMemoryNotificationStore"/>. Locks down the contract
/// that publishers and the topbar dropdown depend on.
/// </summary>
public class InMemoryNotificationStoreTests
{
    private static NotificationDto Sample(string userId, string title = "n", DateTime? when = null) => new()
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Title = title,
        CreatedUtc = when ?? DateTime.UtcNow,
    };

    [Fact]
    public void Add_then_GetRecent_returns_newest_first()
    {
        var store = new InMemoryNotificationStore();
        var n1 = Sample("u1", "first");
        var n2 = Sample("u1", "second");
        store.Add(n1);
        store.Add(n2);

        var recent = store.GetRecent("u1");

        recent.Should().HaveCount(2);
        recent[0].Title.Should().Be("second");
        recent[1].Title.Should().Be("first");
    }

    [Fact]
    public void Add_should_evict_oldest_when_over_cap()
    {
        var store = new InMemoryNotificationStore();
        for (var i = 0; i < InMemoryNotificationStore.MaxPerUser + 5; i++)
        {
            store.Add(Sample("u1", "n" + i));
        }

        var recent = store.GetRecent("u1", max: 100);

        recent.Should().HaveCount(InMemoryNotificationStore.MaxPerUser);
        recent[0].Title.Should().Be("n" + (InMemoryNotificationStore.MaxPerUser + 4));
        recent.Last().Title.Should().Be("n5"); // n0..n4 evicted
    }

    [Fact]
    public void Per_user_isolation_should_be_enforced()
    {
        var store = new InMemoryNotificationStore();
        store.Add(Sample("alice", "a"));
        store.Add(Sample("bob", "b"));

        store.GetRecent("alice").Should().ContainSingle(n => n.Title == "a");
        store.GetRecent("bob").Should().ContainSingle(n => n.Title == "b");
    }

    [Fact]
    public void CountUnread_should_drop_when_marked_read()
    {
        var store = new InMemoryNotificationStore();
        var n = Sample("u1");
        store.Add(n);
        store.CountUnread("u1").Should().Be(1);

        var updated = store.MarkRead("u1", n.Id, DateTime.UtcNow);

        updated.Should().NotBeNull();
        updated!.IsUnread.Should().BeFalse();
        store.CountUnread("u1").Should().Be(0);
    }

    [Fact]
    public void MarkRead_should_return_null_when_not_found()
    {
        var store = new InMemoryNotificationStore();
        store.Add(Sample("u1"));

        var updated = store.MarkRead("u1", Guid.NewGuid(), DateTime.UtcNow);

        updated.Should().BeNull();
    }

    [Fact]
    public void MarkRead_should_not_let_other_user_flip_state()
    {
        var store = new InMemoryNotificationStore();
        var n = Sample("alice");
        store.Add(n);

        var updated = store.MarkRead("bob", n.Id, DateTime.UtcNow);

        updated.Should().BeNull();
        store.CountUnread("alice").Should().Be(1);
    }

    [Fact]
    public void MarkAllRead_should_flip_only_unread_entries()
    {
        var store = new InMemoryNotificationStore();
        var read = Sample("u1");
        store.Add(read);
        store.MarkRead("u1", read.Id, DateTime.UtcNow);
        store.Add(Sample("u1"));
        store.Add(Sample("u1"));

        var flipped = store.MarkAllRead("u1", DateTime.UtcNow);

        flipped.Should().Be(2);
        store.CountUnread("u1").Should().Be(0);
    }
}
