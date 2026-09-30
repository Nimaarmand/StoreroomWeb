using Web.Infrastructure;

namespace StoreRoom.Tests;

/// <summary>
/// تست‌های منطق اعتبارسنجی حواله خروجی.
/// تمرکز اصلی: موجودی نباید منفی شود. این مهم‌ترین قاعده کاری انبار است.
/// </summary>
public class IssueValidatorTests
{
    private static readonly Guid ProductId = Guid.NewGuid();

    private static ValidationResult Validate(
        string? unit = "عدد",
        int quantity = 5,
        Guid? productId = null,
        int? warehouseId = 1,
        int? customerId = 1,
        int? availableStock = 100)
        => new IssueValidator().Validate(
            unit, quantity, productId ?? ProductId, warehouseId, customerId, availableStock);

    [Fact]
    public void Validate_WithValidInput_ReturnsOk()
    {
        var result = Validate();

        Assert.True(result.IsValid);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Validate_WithoutProduct_Fails()
    {
        var result = Validate(productId: Guid.Empty);

        Assert.False(result.IsValid);
        Assert.Equal("کالا را انتخاب کنید.", result.Error);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    public void Validate_WithoutUnit_Fails(string? unit)
    {
        var result = Validate(unit: unit);

        Assert.False(result.IsValid);
        Assert.Equal("واحد شمارش را وارد کنید.", result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-3)]
    public void Validate_WithNonPositiveQuantity_Fails(int quantity)
    {
        var result = Validate(quantity: quantity);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WithUnknownStock_Fails()
    {
        // موجودی null یعنی کالا/انبار پیدا نشده؛ نباید حدس بزنیم.
        var result = Validate(availableStock: null);

        Assert.False(result.IsValid);
        Assert.Equal("موجودی کالا مشخص نیست.", result.Error);
    }

    [Fact]
    public void Validate_WithNegativeStock_Fails()
    {
        var result = Validate(availableStock: -1);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void Validate_WhenQuantityExceedsStock_Fails()
    {
        var result = Validate(quantity: 101, availableStock: 100);

        Assert.False(result.IsValid);
        Assert.Contains("موجودی کافی نیست", result.Error);
    }

    [Fact]
    public void Validate_QuantityEqualToStock_IsAccepted()
    {
        // حالت مرزی: برداشتن تمام موجودی مجاز است.
        var result = Validate(quantity: 100, availableStock: 100);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_StockErrorMessage_UsesPersianDigits()
    {
        var result = Validate(quantity: 200, availableStock: 123);

        Assert.False(result.IsValid);
        Assert.Contains("۱۲۳", result.Error);
    }

    [Fact]
    public void Validate_WithoutCustomer_IsAccepted()
    {
        // مشتری در حواله خروجی اجباری نیست.
        var result = Validate(customerId: null);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithoutWarehouse_ReturnsOkWithWarning()
    {
        var result = Validate(warehouseId: null);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Warning);
    }
}
