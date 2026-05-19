using System.ComponentModel.DataAnnotations;
using ChuA.ERP.Web.Mvc.Contracts.Dtos;

namespace ChuA.ERP.Web.Mvc.ViewModels.Bills;

/// <summary>Strongly typed form model shared by Create and Edit screens for Bills.</summary>
public sealed class BillFormViewModel
{
    public Guid? Id { get; set; }

    [Required]
    [Display(Name = "Vendor")]
    public Guid VendorId { get; set; }

    [Required, StringLength(60)]
    [Display(Name = "Bill number")]
    public string BillNumber { get; set; } = string.Empty;

    [Required, DataType(DataType.Date)]
    [Display(Name = "Bill date")]
    public DateOnly BillDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    [Required, DataType(DataType.Date)]
    [Display(Name = "Due date")]
    public DateOnly DueDate { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));

    [Required, StringLength(3, MinimumLength = 3)]
    [Display(Name = "Currency")]
    public string CurrencyCode { get; set; } = "USD";

    [Display(Name = "Lines")]
    public List<BillLineFormViewModel> Lines { get; set; } = new() { new BillLineFormViewModel() };

    public IReadOnlyList<VendorDto> Vendors { get; set; } = Array.Empty<VendorDto>();

    public bool IsEdit => Id.HasValue;

    public static BillFormViewModel FromDto(BillDto dto) => new()
    {
        Id = dto.Id,
        VendorId = dto.VendorId,
        BillNumber = dto.BillNumber,
        BillDate = dto.BillDate,
        DueDate = dto.DueDate,
        CurrencyCode = dto.CurrencyCode,
        Lines = dto.Lines?.Select(l => new BillLineFormViewModel
        {
            Description = l.Description,
            QuantityValue = l.Quantity.Value,
            QuantityUnitOfMeasure = l.Quantity.UnitOfMeasure,
            UnitPriceAmount = l.UnitPrice.Amount,
            UnitPriceCurrencyCode = l.UnitPrice.CurrencyCode,
            ExpenseAccountId = l.ExpenseAccountId,
        }).ToList() ?? new List<BillLineFormViewModel> { new() },
    };

    private IReadOnlyList<CreateBillLineDto> ToLineDtos() =>
        Lines.Select(l => new CreateBillLineDto(
            l.Description,
            new QuantityDto(l.QuantityValue, l.QuantityUnitOfMeasure),
            new MoneyDto(l.UnitPriceAmount, l.UnitPriceCurrencyCode),
            l.ExpenseAccountId)).ToArray();

    public CreateBillRequest ToCreateRequest() =>
        new(VendorId, BillNumber, BillDate, DueDate, CurrencyCode, ToLineDtos());

    public UpdateBillRequest ToUpdateRequest() =>
        new(BillNumber, BillDate, DueDate, CurrencyCode, ToLineDtos());
}
