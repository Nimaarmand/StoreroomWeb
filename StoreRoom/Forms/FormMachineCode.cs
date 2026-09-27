using ReaLTaiizor.Forms;
using System;
using System.Windows.Forms;
using StoreRoom.Utilities;

namespace StoreRoom.Forms
{
    public partial class FormMachineCode : MaterialForm
    {
        public FormMachineCode()
        {
            InitializeComponent();
            this.Load += FormMachineCode_Load;
        }

        private void FormMachineCode_Load(object sender, EventArgs e)
        {
            string machineCode = HardwareHelper.GetMachineCode();
            txtMachineCode.Text = machineCode;
            txtMachineCode.ReadOnly = true;
        }

        // دکمه کپی
        private void foreverButton1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtMachineCode.Text))
            {
                Clipboard.SetText(txtMachineCode.Text);
                MessageBox.Show("کد ماشین در کلیپ‌بورد کپی شد.", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // دکمه بستن
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FormMachineCode_Load_1(object sender, EventArgs e)
        {

        }
    }
}