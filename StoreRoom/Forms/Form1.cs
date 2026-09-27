using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ReaLTaiizor.Forms;
using StoreRoom.Forms;
using System;
using System.Linq;
using System.Windows.Forms;

namespace StoreRoom
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();
            panel1.Dock = DockStyle.Fill;
            panel1.AutoScroll = true;

            // ✅ تنظیم حداقل اندازه برای جلوگیری از کوچک‌تر شدن بیش از حد
            this.MinimumSize = new Size(700, 500);

            this.Load += Form1_Load;
            this.Resize += Form1_Resize;
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await SetPermissionsBasedOnRole();
            ArrangeButtons(); // چینش اولیه دکمه‌ها
        }

        // ============================================
        // ✅ رویداد Resize برای چینش مجدد دکمه‌ها
        // ============================================
        private void Form1_Resize(object sender, EventArgs e)
        {
            ArrangeButtons();
        }

        // ============================================
        // ✅ تابع چینش خودکار دکمه‌ها در دو ستون (اصلاح‌شده)
        // ============================================
        private void ArrangeButtons()
        {
            // دریافت همه دکمه‌های موجود در panel1 (فقط دکمه‌هایی که Visible هستن)
            var buttons = panel1.Controls.OfType<Button>()
                             .Where(b => b.Visible)
                             .OrderBy(b => b.Name) // مرتب‌سازی بر اساس نام
                             .ToList();

            if (buttons.Count == 0) return;

            int gap = 10;
            int leftMargin = 20;
            int topMargin = 20;
            int btnHeight = 50;
            int panelWidth = panel1.ClientSize.Width;

            // ✅ محاسبه عرض ستون‌ها به‌صورت درصدی (40% برای چپ، 60% برای راست)
            int leftColWidth = (int)(panelWidth * 0.4) - leftMargin - gap / 2;
            int rightColWidth = (int)(panelWidth * 0.6) - leftMargin - gap / 2 - 20;

            // حداقل عرض برای دکمه‌ها (جلوگیری از خیلی کوچک شدن)
            if (leftColWidth < 80) leftColWidth = 80;
            if (rightColWidth < 80) rightColWidth = 80;

            int rightColX = leftMargin + leftColWidth + gap;

            for (int i = 0; i < buttons.Count; i++)
            {
                int row = i / 2;
                int col = i % 2;

                int x, btnWidth;
                if (col == 0) // ستون چپ
                {
                    x = leftMargin;
                    btnWidth = leftColWidth;
                }
                else // ستون راست
                {
                    x = rightColX;
                    btnWidth = rightColWidth;
                }

                int y = topMargin + (row * (btnHeight + gap));

                buttons[i].Size = new Size(btnWidth, btnHeight);
                buttons[i].Location = new Point(x, y);
                buttons[i].Anchor = AnchorStyles.None; // غیرفعال کردن Anchor خودکار
            }

            // تنظیم MinimumSize پنل برای اسکرول
            int totalHeight = topMargin + ((buttons.Count + 1) / 2) * (btnHeight + gap);
            panel1.AutoScrollMinSize = new Size(0, totalHeight);
        }

        // ============================================
        // بخش تنظیمات دسترسی‌ها (همان کدهای قبلی)
        // ============================================
        private async System.Threading.Tasks.Task SetPermissionsBasedOnRole()
        {
            if (string.IsNullOrEmpty(Program.CurrentUserId))
                return;

            var userManager = Program.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await userManager.FindByIdAsync(Program.CurrentUserId);
            if (user == null) return;

            var roles = await userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                EnableAllButtons(true);
                ArrangeButtons(); // بعد از تغییر visibility، دوباره مرتب کن
                return;
            }

            EnableAllButtons(false);

            bool isManager = roles.Contains("Manager");
            bool isWarehouseKeeper = roles.Contains("WarehouseKeeper");
            bool isUser = roles.Contains("User");
            bool isAuditor = roles.Contains("Auditor");

            if (isManager)
            {
                SetButtonVisibility(hopeButton1, true);
                SetButtonVisibility(hopeButton5, true);
                SetButtonVisibility(hopeButton8, true);
                SetButtonVisibility(hopeButton3, true);
                SetButtonVisibility(hopeButton10, true);
                SetButtonVisibility(hopeButton4, true);
                SetButtonVisibility(hopeButton2, true);
                SetButtonVisibility(hopeButton6, true);
                SetButtonVisibility(hopeButton7, true);
                SetButtonVisibility(hopeButton9, true);
                SetButtonVisibility(hopeButton12, true);
                SetButtonVisibility(hopeButton11, true);
                SetButtonVisibility(hopeButton18, true);
                SetButtonVisibility(hopeButton17, true);
                SetButtonVisibility(hopeButton19, false);
                SetButtonVisibility(hopeButton20, false);
                ArrangeButtons();
                return;
            }

            if (isWarehouseKeeper)
            {
                SetButtonVisibility(hopeButton1, true);
                SetButtonVisibility(hopeButton3, true);
                SetButtonVisibility(hopeButton10, true);
                SetButtonVisibility(hopeButton2, true);
                SetButtonVisibility(hopeButton4, true);
                SetButtonVisibility(hopeButton9, true);
                SetButtonVisibility(hopeButton12, true);
                SetButtonVisibility(hopeButton11, true);
                SetButtonVisibility(hopeButton18, true);
                SetButtonVisibility(hopeButton17, true);
                SetButtonVisibility(hopeButton5, false);
                SetButtonVisibility(hopeButton8, false);
                SetButtonVisibility(hopeButton6, false);
                SetButtonVisibility(hopeButton7, false);
                SetButtonVisibility(hopeButton19, false);
                SetButtonVisibility(hopeButton20, false);
                ArrangeButtons();
                return;
            }

            if (isUser || isAuditor)
            {
                SetButtonVisibility(hopeButton4, true);
                SetButtonVisibility(hopeButton2, true);
                SetButtonVisibility(hopeButton6, true);
                SetButtonVisibility(hopeButton7, true);
                SetButtonVisibility(hopeButton9, true);
                SetButtonVisibility(hopeButton12, true);
                SetButtonVisibility(hopeButton11, true);
                SetButtonVisibility(hopeButton18, true);
                SetButtonVisibility(hopeButton17, true);
                SetButtonVisibility(hopeButton1, false);
                SetButtonVisibility(hopeButton5, false);
                SetButtonVisibility(hopeButton8, false);
                SetButtonVisibility(hopeButton3, false);
                SetButtonVisibility(hopeButton10, false);
                SetButtonVisibility(hopeButton19, false);
                SetButtonVisibility(hopeButton20, false);
                ArrangeButtons();
                return;
            }

            EnableAllButtons(false);
            ArrangeButtons();
        }

        private void SetButtonVisibility(Control btn, bool visible)
        {
            if (btn != null)
            {
                btn.Visible = visible;
                btn.Enabled = visible;
            }
        }

        private void EnableAllButtons(bool enabled)
        {
            var buttons = new Control[]
            {
            hopeButton1, hopeButton2, hopeButton3, hopeButton4,
            hopeButton5, hopeButton6, hopeButton7, hopeButton8,
            hopeButton9, hopeButton10, hopeButton11, hopeButton12,
            hopeButton17, hopeButton18, hopeButton19, hopeButton20
            };
            foreach (var btn in buttons)
                if (btn != null)
                {
                    btn.Visible = enabled;
                    btn.Enabled = enabled;
                }
        }

        // ============================================
        // بخش نمایش فرم‌ها در پنل (همان کدهای قبلی)
        // ============================================
        private void ShowFormInPanel(Form form, bool fillPanel = false)
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

                if (form.Width > panel1.Width || form.Height > panel1.Height)
                {
                    float widthRatio = (float)panel1.Width / form.Width;
                    float heightRatio = (float)panel1.Height / form.Height;
                    float ratio = Math.Min(widthRatio, heightRatio) * 0.85f;
                    form.Size = new Size(
                        (int)Math.Max(form.Width * ratio, 100),
                        (int)Math.Max(form.Height * ratio, 100)
                    );
                }

                CenterFormInPanel(form);
            }

            panel1.Controls.Add(form);
            form.Show();

            form.FormClosed += (s, args) =>
            {
                panel1.Controls.Remove(form);
                form.Dispose();
            };

            panel1.Resize -= Panel1_Resize;
            panel1.Resize += Panel1_Resize;

            void Panel1_Resize(object sender, EventArgs e)
            {
                if (!fillPanel && panel1.Controls.Count > 0 && panel1.Controls[0] is Form f)
                {
                    if (f.Width > panel1.Width || f.Height > panel1.Height)
                    {
                        float widthRatio = (float)panel1.Width / f.Width;
                        float heightRatio = (float)panel1.Height / f.Height;
                        float ratio = Math.Min(widthRatio, heightRatio) * 0.85f;
                        f.Size = new Size(
                            (int)Math.Max(f.Width * ratio, 100),
                            (int)Math.Max(f.Height * ratio, 100)
                        );
                    }
                    CenterFormInPanel(f);
                }
            }
        }

        private void CenterFormInPanel(Form form)
        {
            if (form == null || panel1 == null) return;

            if (form.Width > panel1.Width || form.Height > panel1.Height)
            {
                float widthRatio = (float)panel1.Width / form.Width;
                float heightRatio = (float)panel1.Height / form.Height;
                float ratio = Math.Min(widthRatio, heightRatio) * 0.85f;
                form.Size = new Size(
                    (int)Math.Max(form.Width * ratio, 100),
                    (int)Math.Max(form.Height * ratio, 100)
                );
            }

            int x = (panel1.Width - form.Width) / 2;
            int y = (panel1.Height - form.Height) / 2;
            form.Location = new Point(Math.Max(0, x), Math.Max(0, y));
        }

        // ============================================
        // رویدادهای کلیک دکمه‌ها (همان کدهای قبلی)
        // ============================================
        private void hopeButton1_Click(object sender, EventArgs e)
        {
            var form2 = Program.ServiceProvider.GetRequiredService<Form2>();
            ShowFormInPanel(form2, fillPanel: false);
        }

        private void hopeButton5_Click(object sender, EventArgs e)
        {
            var form3 = Program.ServiceProvider.GetRequiredService<Form3>();
            ShowFormInPanel(form3, fillPanel: false);
        }

        private void hopeButton8_Click(object sender, EventArgs e)
        {
            var form4 = Program.ServiceProvider.GetRequiredService<Form4>();
            ShowFormInPanel(form4, fillPanel: false);
        }

        private void hopeButton3_Click(object sender, EventArgs e)
        {
            var form5 = Program.ServiceProvider.GetRequiredService<Form5>();
            ShowFormInPanel(form5, fillPanel: true);
        }

        private void hopeButton10_Click(object sender, EventArgs e)
        {
            var form6 = Program.ServiceProvider.GetRequiredService<Form6>();
            ShowFormInPanel(form6, fillPanel: false);
        }

        private void hopeButton4_Click(object sender, EventArgs e)
        {
            var form7 = Program.ServiceProvider.GetRequiredService<Form7>();
            ShowFormInPanel(form7, fillPanel: true);
        }

        private void hopeButton2_Click(object sender, EventArgs e)
        {
            var form8 = Program.ServiceProvider.GetRequiredService<Form8>();
            ShowFormInPanel(form8, fillPanel: true);
        }

        private void hopeButton6_Click(object sender, EventArgs e)
        {
            var form9 = Program.ServiceProvider.GetRequiredService<Form9>();
            ShowFormInPanel(form9, fillPanel: true);
        }

        private void hopeButton7_Click(object sender, EventArgs e)
        {
            var form10 = Program.ServiceProvider.GetRequiredService<Form10>();
            ShowFormInPanel(form10, fillPanel: true);
        }

        private void hopeButton9_Click(object sender, EventArgs e)
        {
            var form11 = Program.ServiceProvider.GetRequiredService<Form11>();
            ShowFormInPanel(form11, fillPanel: true);
        }

        private void hopeButton14_Click(object sender, EventArgs e)
        {
            var form13 = Program.ServiceProvider.GetRequiredService<Form13>();
            ShowFormInPanel(form13, fillPanel: true);
        }

        private void hopeButton13_Click(object sender, EventArgs e)
        {
            var form17 = Program.ServiceProvider.GetRequiredService<Form17>();
            ShowFormInPanel(form17, fillPanel: false);
        }

        private void hopeButton15_Click(object sender, EventArgs e)
        {
            var form16 = Program.ServiceProvider.GetRequiredService<Form16>();
            ShowFormInPanel(form16, fillPanel: false);
        }

        private void hopeButton16_Click(object sender, EventArgs e)
        {
            var form19 = Program.ServiceProvider.GetRequiredService<Form19>();
            ShowFormInPanel(form19, fillPanel: false);
        }

        private void hopeButton12_Click(object sender, EventArgs e)
        {
            var form21 = Program.ServiceProvider.GetRequiredService<Form21>();
            ShowFormInPanel(form21, fillPanel: true);
        }

        private void hopeButton11_Click(object sender, EventArgs e)
        {
            var form22 = Program.ServiceProvider.GetRequiredService<Form22>();
            ShowFormInPanel(form22, fillPanel: true);
        }

        private void hopeButton18_Click(object sender, EventArgs e)
        {
            var form23 = Program.ServiceProvider.GetRequiredService<Form23>();
            ShowFormInPanel(form23, fillPanel: true);
        }

        private void hopeButton17_Click(object sender, EventArgs e)
        {
            var form24 = Program.ServiceProvider.GetRequiredService<Form24>();
            ShowFormInPanel(form24, fillPanel: true);
        }

        private void hopeButton19_Click(object sender, EventArgs e)
        {
            var form27 = Program.ServiceProvider.GetRequiredService<Settings>();
            ShowFormInPanel(form27, fillPanel: false);
        }

        private void hopeButton20_Click(object sender, EventArgs e)
        {
            var ContactUs = Program.ServiceProvider.GetRequiredService<ContactUs>();
            ShowFormInPanel(ContactUs, fillPanel: true);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
           
        }
    }
}