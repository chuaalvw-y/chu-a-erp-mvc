// Copyright (c) 2026 Alvin Wilsen Chan Chua
// GitHub: chuaalvw-y
// Licensed under the Alvin Wilsen Chan Chua Proprietary Use-Only License.
// See LICENSE.txt in the project root for full license information.

namespace ChuA.ERP.Web.Mvc.Tests.Contracts;

public class ResultTests
{
    [Fact]
    public void Success_should_have_no_errors()
    {
        var sut = Result.Success();
        sut.IsSuccess.Should().BeTrue();
        sut.IsFailure.Should().BeFalse();
        sut.Errors.Should().BeEmpty();
    }

    [Fact]
    public void Failure_with_single_error_should_capture_it()
    {
        var sut = Result.Failure(new Error("test.code", "boom"));
        sut.IsFailure.Should().BeTrue();
        sut.Errors.Should().ContainSingle(e => e.Code == "test.code" && e.Message == "boom");
    }

    [Fact]
    public void Failure_should_drop_none_sentinel_errors()
    {
        var sut = Result.Failure(new[] { Error.None, new Error("x", "y") });
        sut.Errors.Should().ContainSingle(e => e.Code == "x");
    }

    [Fact]
    public void FromProblem_should_carry_status_and_problem()
    {
        var problem = new ApiErrorResponse { Status = 422, Title = "Unprocessable", Detail = "Bad shape", ErrorCode = "domain.rule" };
        var sut = Result.FromProblem(problem);
        sut.IsFailure.Should().BeTrue();
        sut.StatusCode.Should().Be(422);
        sut.Problem.Should().BeSameAs(problem);
        sut.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public void Generic_Success_should_expose_value()
    {
        var sut = Result<int>.Success(42);
        sut.IsSuccess.Should().BeTrue();
        sut.Value.Should().Be(42);
        sut.ValueOrDefault.Should().Be(42);
    }

    [Fact]
    public void Generic_Failure_should_throw_when_accessing_value()
    {
        var sut = Result<int>.Failure(new Error("x", "y"));
        var act = () => sut.Value;
        act.Should().Throw<InvalidOperationException>();
        sut.ValueOrDefault.Should().Be(default);
    }
}
