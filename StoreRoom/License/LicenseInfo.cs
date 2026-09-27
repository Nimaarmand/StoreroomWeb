using StoreRoom.License;
using System;

namespace StoreRoom.Utilities
{
    public class LicenseInfo
    {
        public string MachineCode { get; set; }
        public LicenseType Type { get; set; }
        public DateTime InstallDate { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public bool IsValid()
        {
            if (Type == LicenseType.Lifetime)
                return true;

            if (ExpiryDate.HasValue && DateTime.Today > ExpiryDate.Value)
                return false;

            return true;
        }

        public int RemainingDays()
        {
            if (Type == LicenseType.Lifetime)
                return -1; // نامحدود

            if (!ExpiryDate.HasValue)
                return 0;

            int days = (ExpiryDate.Value - DateTime.Today).Days;
            return days < 0 ? 0 : days;
        }

        public string GetStatusText()
        {
            if (Type == LicenseType.Lifetime)
                return "مادام‌العمر (نامحدود)";

            int remaining = RemainingDays();
            return remaining > 0 ? $"{remaining} روز باقی‌مانده" : "منقضی شده";
        }
    }
}