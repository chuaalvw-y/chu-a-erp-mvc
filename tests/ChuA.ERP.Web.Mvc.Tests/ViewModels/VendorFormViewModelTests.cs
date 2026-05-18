using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.ViewModels.Vendors;

namespace ChuA.ERP.Web.Mvc.Tests.ViewModels;

public class VendorFormViewModelTests
{
    [Fact]
    public void FromDto_should_copy_every_field()
    {
        var dto = new VendorDto(Guid.NewGuid(), Guid.NewGuid(), "V-100", "Acme", "USD", 30, true);

        var vm = VendorFormViewModel.FromDto(dto);

        vm.Id.Should().Be(dto.Id);
        vm.VendorCode.Should().Be("V-100");
        vm.LegalName.Should().Be("Acme");
        vm.DefaultCurrencyCode.Should().Be("USD");
        vm.PaymentTermsDays.Should().Be(30);
        vm.IsBlocked.Should().BeTrue();
        vm.IsEdit.Should().BeTrue();
    }

    [Fact]
    public void ToCreateRequest_should_omit_isblocked()
    {
        var vm = new VendorFormViewModel
        {
            VendorCode = "V-1",
            LegalName = "Acme",
            DefaultCurrencyCode = "USD",
            PaymentTermsDays = 30,
            IsBlocked = true, // ignored by create
        };

        var req = vm.ToCreateRequest();

        req.VendorCode.Should().Be("V-1");
        req.LegalName.Should().Be("Acme");
    }

    [Fact]
    public void Empty_model_should_fail_required_validation()
    {
        var vm = new VendorFormViewModel();
        var ctx = new ValidationContext(vm);
        var errors = new List<ValidationResult>();

        var ok = Validator.TryValidateObject(vm, ctx, errors, validateAllProperties: true);

        ok.Should().BeFalse();
        errors.Should().Contain(e => e.MemberNames.Contains(nameof(VendorFormViewModel.VendorCode)));
        errors.Should().Contain(e => e.MemberNames.Contains(nameof(VendorFormViewModel.LegalName)));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(366)]
    public void Payment_terms_out_of_range_should_fail(int days)
    {
        var vm = new VendorFormViewModel
        {
            VendorCode = "V-1",
            LegalName = "Acme",
            DefaultCurrencyCode = "USD",
            PaymentTermsDays = days,
        };
        var ctx = new ValidationContext(vm);
        var errors = new List<ValidationResult>();

        var ok = Validator.TryValidateObject(vm, ctx, errors, validateAllProperties: true);

        ok.Should().BeFalse();
        errors.Should().Contain(e => e.MemberNames.Contains(nameof(VendorFormViewModel.PaymentTermsDays)));
    }
}
