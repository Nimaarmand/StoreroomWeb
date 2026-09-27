using System;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace StoreRoom.Utilities
{
    public static class HardwareHelper
    {
        public static string GetMachineCode()
        {
            string cpu = GetProcessorId();
            string disk = GetVolumeSerial();
            string mac = GetMacAddress();
            string raw = $"{cpu}-{disk}-{mac}";
            using (var sha = SHA256.Create())
            {
                byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return Convert.ToBase64String(hash).Replace("/", "").Replace("+", "")[..16];
            }
        }

        private static string GetProcessorId()
        {
            try
            {
                return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER") ?? "UNKNOWN";
            }
            catch { return "UNKNOWN"; }
        }

        private static string GetVolumeSerial()
        {
            try
            {
                string drive = Path.GetPathRoot(Environment.SystemDirectory);
                var driveInfo = new DriveInfo(drive);
                return driveInfo.VolumeLabel ?? "UNKNOWN";
            }
            catch { return "UNKNOWN"; }
        }

        private static string GetMacAddress()
        {
            try
            {
                var nics = NetworkInterface.GetAllNetworkInterfaces()
                    .Where(n => n.OperationalStatus == OperationalStatus.Up &&
                                n.NetworkInterfaceType != NetworkInterfaceType.Loopback);
                foreach (var nic in nics)
                    return nic.GetPhysicalAddress().ToString();
            }
            catch { }
            return "UNKNOWN";
        }
    }
}