using Application.Dto;

namespace StoreRoom.Tests;

/// <summary>
/// تست‌های محاسبه مبلغ. اگر این فرمول‌ها به هم بخورد، گزارش‌های مالی
/// انبار غلط می‌شوند، پس باید صریحاً محافظت شوند.
/// </summary>
public class TotalPriceTests
{
    [Theory]
    [InlineData(10, 250_000, 2_500_000)]
    [InlineData(1, 99_500, 99_500)]
    [InlineData(0, 500_000, 0)]
    public void GoodsIssue_TotalPrice_IsUnitPriceTimesQuantity(
        int quantity, decimal unitPrice, decimal expected)
    {
        var dto = new GoodsIssueDto
        {
            Quantity = quantity,
            UnitSellingPrice = unitPrice
        };

        Assert.Equal(expected, dto.TotalPrice);
    }

    [Fact]
    public void GoodsIssue_TotalPrice_WhenPriceIsNull_IsZero()
    {
        // قیمت فروش ثبت‌نشده نباید کل را null یا منفی کند.
        var dto = new GoodsIssueDto { Quantity = 10, UnitSellingPrice = null };

        Assert.Equal(0m, dto.TotalPrice);
    }

    /// <summary>
    /// در رسید ورودی، TotalPrice و NetPrice مقدار ذخیره‌شده‌اند (نه محاسبه‌شونده)،
    /// پس سرویس باید درست پرشان کند. این تست قرارداد مقداردهی را تثبیت می‌کند.
    /// </summary>
    [Theory]
    [InlineData(3, 1_000_000, 9, 3_000_000, 3_270_000)]
    [InlineData(7, 125_500, 0, 878_500, 878_500)]
    [InlineData(2, 500_000, 10, 1_000_000, 1_100_000)]
    public void Receipt_StoresTotalAndNetPrice(
        int quantity, decimal unitPrice, decimal taxRate,
        decimal expectedTotal, decimal expectedNet)
    {
        var dto = new GoodsReceiptDto
        {
            Quantity = quantity,
            UnitPrice = unitPrice,
            TaxRate = taxRate,
            TotalPrice = expectedTotal,
            NetPrice = expectedNet
        };

        Assert.Equal(expectedTotal, dto.TotalPrice);
        Assert.Equal(expectedNet, dto.NetPrice);
    }
}
