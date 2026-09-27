using StoreRoom.Utilities;
namespace StoreRoom.LicenseGenerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int days = (int)numDays.Value;
            string machineCode = txtMachineCode.Text.Trim();

            if (string.IsNullOrEmpty(machineCode))
            {
                MessageBox.Show("کد ماشین را وارد کنید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // تولید داده لایسنس
            string licenseData = LicenseManager.GenerateLicenseFile(days, machineCode);

            // باز کردن SaveFileDialog برای انتخاب مسیر ذخیره
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "License files (*.dat)|*.dat|All files (*.*)|*.*";
                saveFileDialog.FileName = "license.dat";
                saveFileDialog.DefaultExt = "dat";
                saveFileDialog.Title = "ذخیره فایل لایسنس";
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // ذخیره فایل در مسیر انتخاب شده
                    File.WriteAllText(saveFileDialog.FileName, licenseData);
                    MessageBox.Show(
                        $"✅ لایسنس با {days} روز با موفقیت ذخیره شد.\n\nمسیر فایل:\n{saveFileDialog.FileName}",
                        "موفق",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("عملیات ذخیره لغو شد.", "اطلاع", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
