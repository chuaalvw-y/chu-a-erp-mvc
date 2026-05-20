// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Tests.Contracts;

public class PagedResultTests
{
    [Fact]
    public void FromCollection_should_slice_correctly()
    {
        var items = Enumerable.Range(1, 25).ToArray();
        var sut = PagedResult<int>.FromCollection(items, pageNumber: 2, pageSize: 10);
        sut.PageNumber.Should().Be(2);
        sut.PageSize.Should().Be(10);
        sut.TotalCount.Should().Be(25);
        sut.TotalPages.Should().Be(3);
        sut.Items.Should().BeEquivalentTo(new[] { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 });
        sut.HasNextPage.Should().BeTrue();
        sut.HasPreviousPage.Should().BeTrue();
    }

    [Fact]
    public void Empty_should_return_zero_items()
    {
        var sut = PagedResult<string>.Empty();
        sut.Items.Should().BeEmpty();
        sut.TotalCount.Should().Be(0);
        sut.TotalPages.Should().Be(0);
        sut.HasNextPage.Should().BeFalse();
        sut.HasPreviousPage.Should().BeFalse();
    }
}
