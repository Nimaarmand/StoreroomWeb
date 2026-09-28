using Microsoft.Extensions.DependencyInjection;
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
    public partial class Settings : MaterialForm
    {
        public Settings()
        {
            InitializeComponent();
            // تنظیم اولیه پنل
            panel1.Dock = DockStyle.Fill;
            panel1.AutoSize = false;
        }

        /// <summary>
        /// نمایش فرم درون پنل با اندازه دقیق
        /// </summary>
        private void ShowFormInPanel(Form form, bool fillPanel = true)
        {
            panel1.Controls.Clear();
            form.TopLevel = false;
            form.Visible = true;
            form.AutoSize = false;

            if (fillPanel)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.Dock = DockStyle.Fill;
                form.Size = panel1.Size;
            }
            else
            {
                form.FormBorderStyle = FormBorderStyle.Sizable;
                form.Dock = DockStyle.None;

                // اگر فرم از پنل بزرگ‌تر است، کوچکش کن
                if (form.Width > panel1.Width || form.Height > panel1.Height)
                {
                    form.Size = new Size(
                        Math.Min(form.Width, panel1.Width - 20),
                        Math.Min(form.Height, panel1.Height - 20)
                    );
                }

                // قرار دادن در مرکز پنل
                int x = (panel1.Width - form.Width) / 2;
                int y = (panel1.Height - form.Height) / 2;
                form.Location = new Point(Math.Max(0, x), Math.Max(0, y));

                // اگر اندازه فرم تغییر کرد، دوباره مرکز کن
                form.Resize += (s, args) =>
                {
                    int newX = (panel1.Width - form.Width) / 2;
                    int newY = (panel1.Height - form.Height) / 2;
                    form.Location = new Point(Math.Max(0, newX), Math.Max(0, newY));
                };
            }

            panel1.Controls.Add(form);
            form.Show();

            form.FormClosed += (s, args) =>
            {
                panel1.Controls.Remove(form);
                form.Dispose();
            };
        }

        // ========== دکمه‌های منو ==========

        // ثبت کاربر (با اندازه اصلی)
        private void hopeButton1_Click(object sender, EventArgs e)
        {
            var form13 = Program.ServiceProvider.GetRequiredService<Form13>();
            ShowFormInPanel(form13, fillPanel: false);
        }

        // لیست کاربران (پر کردن پنل)
        private void hopeButton14_Click(object sender, EventArgs e)
        {
            var form17 = Program.ServiceProvider.GetRequiredService<Form17>();
            ShowFormInPanel(form17, fillPanel: true);
        }

        // لیست نقش‌ها (پر کردن پنل)
        private void hopeButton3_Click(object sender, EventArgs e)
        {
            var form19 = Program.ServiceProvider.GetRequiredService<Form19>();
            ShowFormInPanel(form19, fillPanel: true);
        }

        // ثبت نقش (با اندازه اصلی)
        private void hopeButton2_Click(object sender, EventArgs e)
        {
            var form16 = Program.ServiceProvider.GetRequiredService<Form16>();
            ShowFormInPanel(form16, fillPanel: false);
        }

        private void Settings_Load(object sender, EventArgs e)
        {

        }
    }
}
