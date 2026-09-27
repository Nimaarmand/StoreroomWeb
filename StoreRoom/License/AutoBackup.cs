using Microsoft.Data.SqlClient;
using System;
using System.IO;
using System.Timers;

namespace StoreRoom.Utilities
{
    public static class AutoBackup
    {
        private static System.Timers.Timer _timer;
        private static string _connectionString;
        private static string _backupFolder;
        private static int _retentionDays = 30;
        private static bool _isRunning = false;

        /// <summary>
        /// راه‌اندازی پشتیبان‌گیری خودکار
        /// </summary>
        /// <param name="connectionString">رشته اتصال به دیتابیس</param>
        /// <param name="backupFolder">مسیر ذخیره پشتیبان‌ها (در صورت null، از مسیر پیش‌فرض استفاده می‌شود)</param>
        public static void Start(string connectionString, string backupFolder = null)
        {
            _connectionString = connectionString;

            // اگر مسیر مشخص نشده، از مسیر پیش‌فرض استفاده کن
            if (string.IsNullOrEmpty(backupFolder))
            {
                // ✅ تغییر مسیر به درایو D (یا هر جای دیگر)
                _backupFolder = @"D:\StoreroomBackups";

                // اگر درایو D وجود ندارد، از درایو C استفاده کن
                if (!Directory.Exists(@"D:\"))
                {
                    _backupFolder = Path.Combine(
                        Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
                        "Storeroom", "Backups"
                    );
                }
            }
            else
            {
                _backupFolder = backupFolder;
            }

            // ایجاد پوشه پشتیبان (اگر وجود نداشته باشد)
            if (!Directory.Exists(_backupFolder))
                Directory.CreateDirectory(_backupFolder);

            // پشتیبان‌گیری اولیه
            PerformBackup();

            // تنظیم Timer برای اجرای هر ۲۴ ساعت
            _timer = new System.Timers.Timer(24 * 60 * 60 * 1000);
            _timer.Elapsed += OnTimerElapsed;
            _timer.AutoReset = true;
            _timer.Start();

            LogMessage($"پشتیبان‌گیری خودکار راه‌اندازی شد. مسیر: {_backupFolder}");
        }

        public static void Stop()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Dispose();
                _timer = null;
            }
            LogMessage("پشتیبان‌گیری خودکار متوقف شد.");
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            PerformBackup();
        }

        private static void PerformBackup()
        {
            if (_isRunning) return;
            _isRunning = true;

            try
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupFileName = $"StoreroomDB_{timestamp}.bak";
                string backupPath = Path.Combine(_backupFolder, backupFileName);

                var builder = new SqlConnectionStringBuilder(_connectionString);
                string databaseName = builder.InitialCatalog;

                string query = $"BACKUP DATABASE [{databaseName}] TO DISK = @backupPath " +
                               $"WITH FORMAT, INIT, NAME = 'Auto Backup', STATS = 10";

                using (var connection = new SqlConnection(_connectionString))
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@backupPath", backupPath);
                    connection.Open();
                    command.ExecuteNonQuery();
                }

                LogMessage($"پشتیبان جدید ایجاد شد: {backupFileName}");
                DeleteOldBackups();
            }
            catch (Exception ex)
            {
                LogMessage($"خطا در پشتیبان‌گیری: {ex.Message}");
            }
            finally
            {
                _isRunning = false;
            }
        }

        private static void DeleteOldBackups()
        {
            try
            {
                var files = Directory.GetFiles(_backupFolder, "StoreroomDB_*.bak");
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    if (fileInfo.CreationTime < DateTime.Now.AddDays(-_retentionDays))
                    {
                        File.Delete(file);
                        LogMessage($"پشتیبان قدیمی حذف شد: {fileInfo.Name}");
                    }
                }
            }
            catch (Exception ex)
            {
                LogMessage($"خطا در حذف پشتیبان‌های قدیمی: {ex.Message}");
            }
        }

        private static void LogMessage(string message)
        {
            try
            {
                string logFolder = Path.Combine(_backupFolder, "Logs");
                if (!Directory.Exists(logFolder))
                    Directory.CreateDirectory(logFolder);

                string logFile = Path.Combine(logFolder, $"Backup_{DateTime.Now:yyyyMMdd}.log");
                string logEntry = $"{DateTime.Now:HH:mm:ss} - {message}";
                File.AppendAllText(logFile, logEntry + Environment.NewLine);
            }
            catch { }
        }

        public static string GetBackupFolder()
        {
            return _backupFolder;
        }

        public static DateTime? GetLastBackupDate()
        {
            try
            {
                var files = Directory.GetFiles(_backupFolder, "StoreroomDB_*.bak");
                if (files.Length == 0) return null;

                var lastFile = files.OrderByDescending(f => f).First();
                return File.GetCreationTime(lastFile);
            }
            catch
            {
                return null;
            }
        }
    }
}