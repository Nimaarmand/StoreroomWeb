using System;
using System.IO;
using System.Windows.Forms;

namespace StoreRoom.Utilities
{
    public static class Logger
    {
        private static string LogFolderPath
        {
            get
            {
                string appData = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "StoreRoom", "Logs");
                if (!Directory.Exists(appData))
                    Directory.CreateDirectory(appData);
                return appData;
            }
        }

        /// <summary>
        /// ثبت پیام در فایل لاگ
        /// </summary>
        public static void Log(string message, bool showMessageBox = false, string title = "خطا")
        {
            try
            {
                string logFile = Path.Combine(LogFolderPath, $"StoreRoom_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"{DateTime.Now:HH:mm:ss} - {message}";
                File.AppendAllText(logFile, logEntry + Environment.NewLine);

                if (showMessageBox)
                {
                    MessageBox.Show(message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                // اگر لاگ نوشته نشد، نادیده گرفته شود
            }
        }

        /// <summary>
        /// ثبت خطا با جزئیات کامل
        /// </summary>
        public static void LogError(Exception ex, string additionalInfo = "")
        {
            string message = $"❌ خطا: {ex.Message}\n" +
                             $"📍 محل: {ex.StackTrace}\n" +
                             $"📌 اطلاعات اضافی: {additionalInfo}\n" +
                             $"📅 زمان: {DateTime.Now}\n" +
                             $"━━━━━━━━━━━━━━━━━━━━━━━━━━━";
            Log(message, showMessageBox: false);
        }

        /// <summary>
        /// ثبت اطلاعات سیستم برای دیباگ (نسخه دات‌نت، سیستم‌عامل و ...)
        /// </summary>
        public static void LogSystemInfo()
        {
            try
            {
                string info = $"🖥️ سیستم‌عامل: {Environment.OSVersion}\n" +
                              $"🧠 پردازنده: {Environment.ProcessorCount} هسته\n" +
                              $"📂 مسیر اجرا: {AppDomain.CurrentDomain.BaseDirectory}\n" +
                              $"📅 تاریخ اجرا: {DateTime.Now}\n" +
                              $"━━━━━━━━━━━━━━━━━━━━━━━━━━━";
                Log(info);
            }
            catch { }
        }

        /// <summary>
        /// دریافت مسیر فایل لاگ امروز
        /// </summary>
        public static string GetTodayLogPath()
        {
            return Path.Combine(LogFolderPath, $"StoreRoom_{DateTime.Now:yyyyMMdd}.log");
        }

        /// <summary>
        /// باز کردن پوشه لاگ در File Explorer
        /// </summary>
        public static void OpenLogFolder()
        {
            try
            {
                System.Diagnostics.Process.Start("explorer.exe", LogFolderPath);
            }
            catch { }
        }
    }
}