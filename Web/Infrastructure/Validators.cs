using System.Text.RegularExpressions;

namespace Web.Infrastructure;

/// <summary>نتیجه اعتبارسنجی: موفق بود یا نه، و اگر نه چرا</summary>
public record ValidationResult(bool IsValid, string? Error = null, string? Warning = null)
{
    public static ValidationResult Ok() => new(true);
    public static ValidationResult Fail(string error) => new(false, error);
}

public interface IReceiptValidator
{
    ValidationResult Validate(
        string? invoiceNumber,
        string? invoiceDate,
        int quantity,
        decimal unitPrice,
        decimal taxRate,
        int supplierId,
        int? warehouseId,
        int availableStock);
}

public interface IIssueValidator
{
    ValidationResult Validate(
        string? unit,
        int quantity,
        Guid productId,
        int? warehouseId,
        int? customerId,
        int? availableStock);
}

public sealed class ReceiptValidator : IReceiptValidator
{
    public ValidationResult Validate(
        string? invoiceNumber,
        string? invoiceDate,
        int quantity,
        decimal unitPrice,
        decimal taxRate,
        int supplierId,
        int? warehouseId,
        int availableStock)
    {
        if (supplierId <= 0)
            return ValidationResult.Fail("تامین‌کننده را انتخاب کنید.");

        if (string.IsNullOrWhiteSpace(invoiceNumber))
            return ValidationResult.Fail("شماره فاکتور را وارد کنید.");

        if (invoiceNumber.Trim().Length > 50)
            return ValidationResult.Fail("شماره فاکتور نباید بیشتر از ۵۰ نویسه باشد.");

        if (string.IsNullOrWhiteSpace(invoiceDate))
            return ValidationResult.Fail("تاریخ فاکتور را وارد کنید.");

        if (quantity <= 0)
            return ValidationResult.Fail("تعداد باید بزرگ‌تر از صفر باشد.");

        if (unitPrice < 0)
            return ValidationResult.Fail("قیمت واحد نمی‌تواند منفی باشد.");

        if (taxRate is < 0 or > 100)
            return ValidationResult.Fail("درصد مالیات باید بین ۰ تا ۱۰۰ باشد.");

        if (availableStock < 0)
            return ValidationResult.Fail("موجودی فعلی انبار منفی است. ابتدا موجودی را اصلاح کنید.");

        // انبار اجباری نیست (در مدل int? است) اما نبودش یعنی موجودی در جایی
        // دیگر ثبت می‌شود؛ هشدار می‌دهیم ولی جلوی ثبت را نمی‌گیریم.
        string? warning = warehouseId is null
            ? "انبار انتخاب نشده است؛ موجودی انبار به‌روزرسانی نخواهد شد."
            : null;

        return new ValidationResult(true, null, warning);
    }
}

public sealed class IssueValidator : IIssueValidator
{
    public ValidationResult Validate(
        string? unit,
        int quantity,
        Guid productId,
        int? warehouseId,
        int? customerId,
        int? availableStock)
    {
        if (productId == Guid.Empty)
            return ValidationResult.Fail("کالا را انتخاب کنید.");

        if (string.IsNullOrWhiteSpace(unit))
            return ValidationResult.Fail("واحد شمارش را وارد کنید.");

        if (quantity <= 0)
            return ValidationResult.Fail("تعداد باید بزرگ‌تر از صفر باشد.");

        if (availableStock is null)
            return ValidationResult.Fail("موجودی کالا مشخص نیست.");

        if (availableStock.Value < 0)
            return ValidationResult.Fail("موجودی فعلی این کالا منفی است. ابتدا موجودی را اصلاح کنید.");

        if (quantity > availableStock.Value)
            return ValidationResult.Fail(
                $"موجودی کافی نیست. موجودی فعلی: {PersianNumberConverter.ToPersianDigits(availableStock.Value)}");

        string? warning = warehouseId is null
            ? "انبار انتخاب نشده است؛ موجودی انبار به‌روزرسانی نخواهد شد."
            : null;

        return new ValidationResult(true, null, warning);
    }
}
