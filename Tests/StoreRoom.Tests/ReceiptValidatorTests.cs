using Web.Infrastructure;

namespace StoreRoom.Tests;

/// <summary>
/// تست‌های منطق اعتبارسنجی رسید ورودی.
/// هدف: تضمین اینکه ورودی نامعتبر پیش از رسیدن به دیتابیس رد می‌شود.
/// </summary>
public class ReceiptValidatorTests
{
    private readonly ReceiptValidator _validator = new();

    /// <summary>یک رسید کاملاً معتبر به‌عنوان پایه، تا هر تست فقط یک چیز را خراب کند.</summary>
    private static ValidationResult Validate(
        string? invoiceNumber = "INV-1001",
        string? invoiceDate = "1404/01/01",
        int quantity = 10,
        decimal unitPrice = 250_000,
        decimal taxRate = 9,
        int supplierId = 1,
        int? warehouseId = 1,
        int availableStock = 0)
        => new ReceiptValidator().Validate(
            invoiceNumber, invoiceDate, quantity, unitPrice, taxRate,
            supplierId, warehouseId, availableStock);

    [Fact]
    public void Validate_WithValidInput_ReturnsOk()
    {
        var result = Validate();

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Validate_WithoutSupplier_Fails()
    {
        var result = Validate(supplierId: 0);

        Assert.False(result.IsValid);
        Assert.Equal("تامین‌کننده را انتخاب کنید.", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Validate_WithoutInvoiceNumber_Fails(string? invoiceNumber)
    {
        var result = Validate(invoiceNumber: invoiceNumber);

        Assert.False(result.IsValid);
        Assert.Equal("شماره فاکتور را وارد کنید.", result.Error);
    }

    [Fact]
    public void Validate_WithInvoiceNumberLongerThan50_Fails()
    {
        var result = Validate(invoiceNumber: new string('A', 51));

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithInvoiceNumberExactly50_IsAccepted()
    {
        var result = Validate(invoiceNumber: new string('A', 50));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WithNonPositiveQuantity_Fails(int quantity)
    {
        var result = Validate(quantity: quantity);

        Assert.False(result.IsValid);
        Assert.Equal("تعداد باید بزرگ‌تر از صفر باشد.", result.Error);
    }

    [Fact]
    public void Validate_WithNegativeUnitPrice_Fails()
    {
        var result = Validate(unitPrice: -1);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithZeroUnitPrice_IsAccepted()
    {
        // کالای رایگان مجاز است، فقط قیمت منفی نه.
        var result = Validate(unitPrice: 0);

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Validate_WithTaxRateOutOfRange_Fails(decimal taxRate)
    {
        var result = Validate(taxRate: taxRate);

        Assert.False(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(9)]
    [InlineData(100)]
    public void Validate_WithTaxRateInRange_IsAccepted(decimal taxRate)
    {
        var result = Validate(taxRate: taxRate);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithNegativeStock_Fails()
    {
        // موجودی منفی یعنی داده پایه خراب است؛ نباید با آن رسید زد.
        var result = Validate(availableStock: -5);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithoutWarehouse_ReturnsOkWithWarning()
    {
        // انبار ناموجود مانع ثبت نیست، فقط هشدار می‌دهد.
        var result = Validate(warehouseId: null);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Warning);
    }
}
