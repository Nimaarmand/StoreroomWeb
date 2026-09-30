using Web.Infrastructure;

namespace StoreRoom.Tests;

/// <summary>
/// تست‌های تبدیل رقم. کاربر فارسی تایپ می‌کند اما دیتابیس باید لاتین ذخیره کند؛
/// یک اشتباه اینجا یعنی جست‌وجو و گزارش‌گیری خراب.
/// </summary>
public class PersianNumberConverterTests
{
    [Theory]
    [InlineData("1404/01/15", "۱۴۰۴/۰۱/۱۵")]
    [InlineData("abc123", "abc۱۲۳")]
    [InlineData("0", "۰")]
    [InlineData("9876543210", "۹۸۷۶۵۴۳۲۱۰")]
    public void ToPersianDigits_ConvertsLatinDigits(string input, string expected)
    {
        Assert.Equal(expected, PersianNumberConverter.ToPersianDigits(input));
    }

    [Theory]
    [InlineData("۱۲۳")]
    [InlineData("١٢٣")]  // ارقام عربی
    public void ToEnglishDigits_ConvertsPersianAndArabicDigits(string input)
    {
        Assert.Equal("123", PersianNumberConverter.ToEnglishDigits(input));
    }

    [Fact]
    public void ToPersianDigits_WithNull_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, PersianNumberConverter.ToPersianDigits((string?)null));
    }

    [Fact]
    public void ToEnglishDigits_WithNull_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, PersianNumberConverter.ToEnglishDigits(null));
    }

    [Theory]
    [InlineData("۱٬۲۳۴٬۵۶۷", "1234567")]  // جداکننده هزارگان فارسی
    [InlineData("1,234,567", "1234567")]
    [InlineData("۱۲۳abc۴۵۶", "123456")]      // حروف وسط عدد
    [InlineData("  ۵۰  ", "50")]
    [InlineData("-۱۲", "-12")]
    [InlineData("۱۲۳٫۴۵", "123.45")]        // جداکننده اعشار فارسی
    [InlineData("۱۲۳٬۴۵۶٫۷۸", "123456.78")] // هر دو جداکننده با هم
    public void NormalizeNumericInput_StripsSeparatorsAndNonDigits(
        string input, string expected)
    {
        Assert.Equal(expected, PersianNumberConverter.NormalizeNumericInput(input));
    }

    [Fact]
    public void NormalizeNumericInput_PersianDecimalSeparator_StaysADecimalPoint()
    {
        // رگرسیون: «۱۲۳٫۴۵» نباید به «۱۲۳۴۵» تبدیل شود، وگرنه مبلغ ده‌برابر می‌شود.
        var normalized = PersianNumberConverter.NormalizeNumericInput("۱۲۳٫۴۵");

        Assert.Equal(123.45m, decimal.Parse(normalized,
            System.Globalization.CultureInfo.InvariantCulture));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void NormalizeNumericInput_WithBlank_ReturnsEmpty(string? input)
    {
        Assert.Equal(string.Empty, PersianNumberConverter.NormalizeNumericInput(input));
    }

    [Theory]
    [InlineData(1234.5, 2, "۱٬۲۳۴٫۵۰")]   // جداکننده هزارگان و اعشار، هر دو فارسی
    [InlineData(1234567, 0, "۱٬۲۳۴٬۵۶۷")]
    [InlineData(500, 0, "۵۰۰")]            // زیر ۱۰۰۰ جداکننده نمی‌گذارد
    public void ToPersianDigits_Decimal_UsesPersianSeparators(
        decimal value, int decimals, string expected)
    {
        // با Culture پیش‌فرض خروجی «۱,۲۳۴.۵۰» بود: ارقام فارسی با کاما و نقطه لاتین.
        Assert.Equal(expected, PersianNumberConverter.ToPersianDigits(value, decimals));
    }
}
