using Microsoft.Win32;
using StoreRoom.License;
using System;
using System.IO;
using System.Text;

namespace StoreRoom.Utilities
{
    public static class LicenseManager
    {
        private const int TrialDays = 30;
        private const string RegistryPath = @"Software\StoreRoom";
        private const string RegistryKey = "LicenseData";
        private const string LicenseFileName = "license.txt";
        private static readonly byte[] _key = Encoding.UTF8.GetBytes("StoreRoom2024!");

        // ========== رمزنگاری ساده (XOR) ==========
        private static string Encrypt(string plainText)
        {
            byte[] data = Encoding.UTF8.GetBytes(plainText);
            for (int i = 0; i < data.Length; i++)
                data[i] ^= _key[i % _key.Length];
            return Convert.ToBase64String(data);
        }

        private static string Decrypt(string cipherText)
        {
            byte[] data = Convert.FromBase64String(cipherText);
            for (int i = 0; i < data.Length; i++)
                data[i] ^= _key[i % _key.Length];
            return Encoding.UTF8.GetString(data);
        }

        // ========== اعتبارسنجی اصلی ==========
        public static LicenseInfo ValidateLicense()
        {
            var fileLicense = LoadFromFile();
            if (fileLicense != null && fileLicense.IsValid())
                return fileLicense;

            var regLicense = LoadFromRegistry();
            if (regLicense != null && regLicense.IsValid())
                return regLicense;

            var trial = GetTrialLicense();
            if (trial.IsValid())
                return trial;

            return null;
        }

        // ========== تولید لایسنس (با تعداد روز دلخواه) ==========
        public static string GenerateLicenseFile(int days, string machineCode = null)
        {
            if (string.IsNullOrEmpty(machineCode))
                machineCode = HardwareHelper.GetMachineCode();

            LicenseType type = days == 0 ? LicenseType.Lifetime : (LicenseType)days;

            var license = new LicenseInfo
            {
                MachineCode = machineCode,
                Type = type,
                InstallDate = DateTime.Today,
                ExpiryDate = days == 0 ? null : (DateTime?)DateTime.Today.AddDays(days)
            };

            string rawData = $"{license.MachineCode}|{(int)license.Type}|{license.InstallDate.ToBinary()}";
            if (license.ExpiryDate.HasValue)
                rawData += $"|{license.ExpiryDate.Value.ToBinary()}";

            return Encrypt(rawData);
        }

        // ========== ذخیره در فایل ==========
        public static void SaveLicenseToFile(string licenseData)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicenseFileName);
            File.WriteAllText(path, licenseData);
        }

        // ========== ذخیره در رجیستری ==========
        public static void SaveLicenseToRegistry(string licenseData)
        {
            using (var key = Registry.CurrentUser.CreateSubKey(RegistryPath))
            {
                key?.SetValue(RegistryKey, licenseData);
            }
        }

        // ========== بارگذاری از فایل ==========
        private static LicenseInfo LoadFromFile()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LicenseFileName);
            if (!File.Exists(path)) return null;

            try
            {
                string encrypted = File.ReadAllText(path);
                return DecryptLicenseData(encrypted);
            }
            catch { return null; }
        }

        // ========== بارگذاری از رجیستری ==========
        private static LicenseInfo LoadFromRegistry()
        {
            using (var key = Registry.CurrentUser.OpenSubKey(RegistryPath))
            {
                if (key == null) return null;
                object value = key.GetValue(RegistryKey);
                if (value == null) return null;

                try
                {
                    return DecryptLicenseData(value.ToString());
                }
                catch { return null; }
            }
        }

        // ========== رمزگشایی داده‌های لایسنس ==========
        private static LicenseInfo DecryptLicenseData(string encrypted)
        {
            string rawData = Decrypt(encrypted);
            string[] parts = rawData.Split('|');

            if (parts.Length < 3) return null;

            var license = new LicenseInfo
            {
                MachineCode = parts[0],
                Type = (LicenseType)int.Parse(parts[1]),
                InstallDate = DateTime.FromBinary(long.Parse(parts[2]))
            };

            if (parts.Length >= 4 && !string.IsNullOrEmpty(parts[3]))
                license.ExpiryDate = DateTime.FromBinary(long.Parse(parts[3]));

            string currentMachine = HardwareHelper.GetMachineCode();
            if (license.MachineCode != currentMachine)
                return null;

            return license;
        }

        // ========== لایسنس آزمایشی (۳۰ روزه) ==========
        private static LicenseInfo GetTrialLicense()
        {
            string trialFilePath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StoreRoom", "trial.dat");

            DateTime installDate;
            if (!File.Exists(trialFilePath))
            {
                installDate = DateTime.Today;
                Directory.CreateDirectory(Path.GetDirectoryName(trialFilePath));
                File.WriteAllText(trialFilePath, installDate.ToBinary().ToString());
            }
            else
            {
                installDate = DateTime.FromBinary(long.Parse(File.ReadAllText(trialFilePath)));
            }

            return new LicenseInfo
            {
                MachineCode = HardwareHelper.GetMachineCode(),
                Type = LicenseType.Monthly,
                InstallDate = installDate,
                ExpiryDate = installDate.AddDays(TrialDays)
            };
        }
    }
}