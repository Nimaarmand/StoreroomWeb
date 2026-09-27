using ReaLTaiizor.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace StoreRoom.Forms
{
    public partial class Form27 : MaterialForm
    {
        public Form27()
        {
            InitializeComponent();
        }

        private async void Form27_Load(object sender, EventArgs e)
        {
            label1.Text = "در حال بارگذاری کمی صبر کنید ....";
            for (int i = 0; i <= 100; i++)
            {
                cyberProgressBar1.Value = i;
                await Task.Delay(50);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
