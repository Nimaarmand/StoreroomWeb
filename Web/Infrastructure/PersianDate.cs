using System.Globalization;

namespace Web.Infrastructure;

/// <summary>
/// نمایش تاریخ‌های میلادی ذخیره‌شده در دیتابیس به شکل تاریخ شمسی.
/// دیتابیس همچنان تاریخ میلادی نگه می‌دارد؛ فقط لایه نمایش تغییر می‌کند.
/// </summary>
public static class PersianDate
{
    private static readonly string[] MonthNames =
    {
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };

    private static readonly string[] DayNames =
    {
        "شنبه", "یکشنبه", "دوشنبه", "سه‌شنبه", "چهارشنبه", "پنجشنبه", "جمعه"
    };

    /// <summary>تاریخ شمسی با ساعت</summary>
    public static string ToPersian(DateTime value)
        => $"{ToPersianDate(value)} ساعت {value:HH:mm}";

    /// <summary>تاریخ شمسی به همراه نام روز هفته</summary>
    public static string ToPersianFull(DateTime value)
        => $"{DayName(value.DayOfWeek)} {ToPersianDate(value)}";

    /// <summary>تاریخ شمسی به تنهایی</summary>
    public static string ToPersianDate(DateTime value)
    {
        var (year, month, day) = ToGregorian(value);
        return $"{day} {MonthNames[month - 1]} {year}";
    }

    /// <summary>نام فارسی روز هفته</summary>
    public static string DayName(DayOfWeek day)
        => DayNames[((int)day + 1) % 7];

    /// <summary>
    /// تبدیل تاریخ میلادی به (سال، ماه، روز) شمسی.
    /// الگوریتم استاندارد تبدیل تقویم جلالی.
    /// </summary>
    public static (int Year, int Month, int Day) ToGregorian(DateTime date)
    {
        int[] gregorianDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };

        int gy = date.Year;
        int gm = date.Month;
        int gd = date.Day;

        int jy = gy <= 1600 ? 0 : 979;
        gy -= gy <= 1600 ? 621 : 1600;

        int gy2 = (gm > 2) ? gy + 1 : gy;
        int days = 355666 + (365 * gy) + ((gy2 + 3) / 4) - ((gy2 + 99) / 100)
                   + ((gy2 + 399) / 400) + gd;

        for (int i = 0; i < (gm - 1); i++)
            days += gregorianDays[i];

        if (IsLeapJalali(jy))
            days += 1;

        jy += 33 * (days / 12053);
        days %= 12053;

        jy += 4 * (days / 1461);
        days %= 1461;

        if (days > 365)
        {
            jy += (days - 1) / 365;
            days = (days - 1) % 365;
        }

        int jm, jd;
        if (days < 186)
        {
            jm = 1 + days / 31;
            jd = 1 + days % 31;
        }
        else
        {
            jm = 7 + (days - 186) / 30;
            jd = 1 + (days - 186) % 30;
        }

        return (jy, jm, jd);

        static bool IsLeapJalali(int year) =>
            (((year - (year / 33) * 33) + 4) % 33) < 4;
    }

    /// <summary>تبدیل عدد لاتین به رقم فارسی برای نمایش</summary>
    public static string Digits(string? value) => PersianNumberConverter.ToPersianDigits(value);
}
