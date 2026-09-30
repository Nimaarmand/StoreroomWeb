namespace Web.Infrastructure;

/// <summary>
/// تبدیل ارقام فارسی ↔ لاتین.
/// این کلاس عمداً static است: تبدیل رقم وضعیت دارد و به هیچ وابستگی‌ای نیاز
/// ندارد، بنابراین ثبت آن در DI فقط پیچیدگی اضافه می‌کرد.
/// </summary>
public static class PersianNumberConverter
{
    private const string PersianDigits = "۰۱۲۳۴۵۶۷۸۹";
    private const string ArabicDigits = "٠١٢٣٤٥٦٧٨٩";

    /// <summary>جداکننده هزارگان فارسی/عربی (U+066C)</summary>
    private const char PersianThousandsSeparator = '٬';

    /// <summary>جداکننده اعشار فارسی/عربی (U+066B)</summary>
    private const char PersianDecimalSeparator = '٫';

    /// <summary>ویرگول عربی که کاربر فارسی تایپ می‌کند (U+060C)</summary>
    private const char ArabicComma = '،';

    public static string ToPersianDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var chars = value.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] >= '0' && chars[i] <= '9')
                chars[i] = PersianDigits[chars[i] - '0'];
            else
            {
                int a = ArabicDigits.IndexOf(chars[i]);
                if (a >= 0)
                    chars[i] = PersianDigits[a];
            }
        }
        return new string(chars);
    }

    /// <summary>
    /// قالب‌بندی عدد به رقم فارسی با جداکننده‌های فارسی.
    /// از Culture پیش‌فرض استفاده نمی‌شود چون خروجی آن «۱,۲۳۴.۵۰» است:
    /// ارقام فارسی با کاما و نقطه لاتین، که با متن فارسی هم‌خوان نیست.
    /// </summary>
    public static string ToPersianDigits(decimal value, int decimals = 0)
    {
        var formatted = value.ToString($"N{decimals}", System.Globalization.CultureInfo.InvariantCulture);

        var chars = formatted.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            if (chars[i] == ',')
                chars[i] = PersianThousandsSeparator;
            else if (chars[i] == '.')
                chars[i] = PersianDecimalSeparator;
        }

        return ToPersianDigits(new string(chars));
    }

    public static string ToPersianDigits(int value) => ToPersianDigits(value.ToString());

    /// <summary>تبدیل رقم‌های فارسی/عربی به لاتین، برای ذخیره در دیتابیس</summary>
    public static string ToEnglishDigits(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var chars = value.ToCharArray();
        for (int i = 0; i < chars.Length; i++)
        {
            int p = PersianDigits.IndexOf(chars[i]);
            if (p >= 0)
                chars[i] = (char)('0' + p);
            else
            {
                int a = ArabicDigits.IndexOf(chars[i]);
                if (a >= 0)
                    chars[i] = (char)('0' + a);
            }
        }
        return new string(chars);
    }

    /// <summary>
    /// پاک‌سازی ورودی عددی کاربر: ارقام فارسی به لاتین، حذف جداکننده هزارگان و
    /// تبدیل جداکننده اعشار فارسی به نقطه لاتین.
    /// برای فیلدهایی که کاربر ممکن است با ارقام فارسی یا کاما پر کند.
    /// </summary>
    public static string NormalizeNumericInput(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var latin = ToEnglishDigits(value);

        var builder = new System.Text.StringBuilder(latin.Length);
        foreach (char c in latin)
        {
            if (char.IsDigit(c) || c == '.' || c == '-')
            {
                builder.Append(c);
            }
            else if (c == PersianDecimalSeparator)
            {
                // جداکننده اعشار فارسی باید به نقطه تبدیل شود نه اینکه حذف شود،
                // وگرنه «۱۲۳٫۴۵» به «۱۲۳۴۵» تبدیل می‌شود و مبلغ ده‌برابر می‌شود.
                builder.Append('.');
            }
            else if (c == PersianThousandsSeparator || c == ArabicComma || c == ',')
            {
                // جداکننده هزارگان: فقط نمایشی است، باید حذف شود.
            }
        }
        return builder.ToString();
    }
}
