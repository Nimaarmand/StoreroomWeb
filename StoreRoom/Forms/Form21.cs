using Application.Dto;
using Application.Features.Implementation.GoodsReceipt_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using ReaLTaiizor.Forms;
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StoreRoom.Forms
{
    public partial class Form21 : MaterialForm
    {
        private readonly GoodsReceiptService _goodsReceiptService;
        private int _ReceiptId = 0;

        public Form21(GoodsReceiptService goodsReceiptService)
        {
            InitializeComponent();
            _goodsReceiptService = goodsReceiptService;
            this.Load += Form21_Load;
            // اتصال رویداد CellFormatting برای فرمت قیمت‌ها
            poisonDataGridView1.CellFormatting += poisonDataGridView1_CellFormatting;
        }

        private async void Form21_Load(object sender, EventArgs e)
        {
            textBoxEdit1.Text = "جستجو ...";
            CustomizeDataGridView();
            await LoadReceiptsAsync(showEmptyMessage: false);
        }


        private void textBoxEdit1_Enter_1(object sender, EventArgs e)
        {
            if (textBoxEdit1.Text == "جستجو ...")
                textBoxEdit1.Text = "";
        }
        private void textBoxEdit1_Leave_1(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxEdit1.Text))
                textBoxEdit1.Text = "جستجو ...";
        }
       

        // ========== بارگذاری داده‌ها ==========
        private async Task LoadReceiptsAsync(bool showEmptyMessage = false)
        {
            int count = (int)dungeonNumeric1.Value;
            if (count <= 0) count = 20;

            var receipts = await _goodsReceiptService.GetTopReceiptsWithDetailsAsync(count);
            poisonDataGridView1.DataSource = receipts.ToList();

            DgvPersian();
            CustomizeDataGridView();

            if (showEmptyMessage && !receipts.Any())
                MessageBox.Show("هیچ رسیدی یافت نشد.", "اطلاع", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ========== تنظیمات دیتاگرید (فارسی‌سازی) ==========
        private void DgvPersian()
        {
            foreach (DataGridViewColumn col in poisonDataGridView1.Columns)
            {
                col.Visible = false;
            }

            if (poisonDataGridView1.Columns.Contains("ProductName"))
            {
                poisonDataGridView1.Columns["ProductName"].Visible = true;
                poisonDataGridView1.Columns["ProductName"].HeaderText = "نام محصول";
                poisonDataGridView1.Columns["ProductName"].DisplayIndex = 0;
            }

            if (poisonDataGridView1.Columns.Contains("Description"))
            {
                poisonDataGridView1.Columns["Description"].Visible = true;
                poisonDataGridView1.Columns["Description"].HeaderText = "توضیحات";
                poisonDataGridView1.Columns["Description"].DisplayIndex = 1;
            }

            if (poisonDataGridView1.Columns.Contains("Quantity"))
            {
                poisonDataGridView1.Columns["Quantity"].Visible = true;
                poisonDataGridView1.Columns["Quantity"].HeaderText = "تعداد";
                poisonDataGridView1.Columns["Quantity"].DisplayIndex = 2;
            }

            if (poisonDataGridView1.Columns.Contains("Unit"))
            {
                poisonDataGridView1.Columns["Unit"].Visible = true;
                poisonDataGridView1.Columns["Unit"].HeaderText = "واحد";
                poisonDataGridView1.Columns["Unit"].DisplayIndex = 3;
            }

            if (poisonDataGridView1.Columns.Contains("UnitPrice"))
            {
                poisonDataGridView1.Columns["UnitPrice"].Visible = true;
                poisonDataGridView1.Columns["UnitPrice"].HeaderText = "قیمت";
                poisonDataGridView1.Columns["UnitPrice"].DisplayIndex = 4;
            }

            if (poisonDataGridView1.Columns.Contains("TotalPrice"))
            {
                poisonDataGridView1.Columns["TotalPrice"].Visible = true;
                poisonDataGridView1.Columns["TotalPrice"].HeaderText = "قیمت کل";
                poisonDataGridView1.Columns["TotalPrice"].DisplayIndex = 5;
            }

            if (poisonDataGridView1.Columns.Contains("ReceiptDate"))
            {
                poisonDataGridView1.Columns["ReceiptDate"].Visible = true;
                poisonDataGridView1.Columns["ReceiptDate"].HeaderText = "تاریخ ورود";
                poisonDataGridView1.Columns["ReceiptDate"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                poisonDataGridView1.Columns["ReceiptDate"].DisplayIndex = 6;
            }

            if (poisonDataGridView1.Columns.Contains("WarehouseName"))
            {
                poisonDataGridView1.Columns["WarehouseName"].Visible = true;
                poisonDataGridView1.Columns["WarehouseName"].HeaderText = "انبار";
                poisonDataGridView1.Columns["WarehouseName"].DisplayIndex = 7;
            }

            if (poisonDataGridView1.Columns.Contains("SupplierName"))
            {
                poisonDataGridView1.Columns["SupplierName"].Visible = true;
                poisonDataGridView1.Columns["SupplierName"].HeaderText = "تأمین‌کننده";
                poisonDataGridView1.Columns["SupplierName"].DisplayIndex = 8;
            }

            if (poisonDataGridView1.Columns.Contains("InvoiceNumber"))
            {
                poisonDataGridView1.Columns["InvoiceNumber"].Visible = true;
                poisonDataGridView1.Columns["InvoiceNumber"].HeaderText = "شماره فاکتور";
                poisonDataGridView1.Columns["InvoiceNumber"].DisplayIndex = 9;
            }

            if (poisonDataGridView1.Columns.Contains("StatusText"))
            {
                poisonDataGridView1.Columns["StatusText"].Visible = true;
                poisonDataGridView1.Columns["StatusText"].HeaderText = "وضعیت";
                poisonDataGridView1.Columns["StatusText"].DisplayIndex = 10;
            }

            if (poisonDataGridView1.Columns.Contains("UserFullName"))
            {
                poisonDataGridView1.Columns["UserFullName"].Visible = true;
                poisonDataGridView1.Columns["UserFullName"].HeaderText = "ثبت‌کننده";
                poisonDataGridView1.Columns["UserFullName"].DisplayIndex = 11;
            }
        }

        // ========== تنظیمات ظاهری دیتاگرید ==========
        private void CustomizeDataGridView()
        {
            if (poisonDataGridView1.Columns.Count == 0) return;
            poisonDataGridView1.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.DefaultCellStyle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            poisonDataGridView1.ReadOnly = true;
        }

        // ========== فرمت قیمت‌ها (سه‌رقم سه‌رقم) ==========
        private void poisonDataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value) return;

            if (poisonDataGridView1.Columns[e.ColumnIndex].Name == "UnitPrice" ||
                poisonDataGridView1.Columns[e.ColumnIndex].Name == "TotalPrice")
            {
                if (decimal.TryParse(e.Value.ToString(), out decimal price))
                {
                    e.Value = price.ToString("N0");
                    e.FormattingApplied = true;
                }
            }
        }

        // ========== دکمه نمایش ==========
        private async void foxButton1_Click(object sender, EventArgs e)
        {
            int count = (int)dungeonNumeric1.Value;
            if (count <= 0)
            {
                MessageBox.Show("تعداد باید بزرگتر از صفر باشد", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                await LoadReceiptsAsync(showEmptyMessage: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== جستجوی زنده (با متد جدید SearchReceipts) ==========
        private async void textBoxEdit1_TextChanged(object sender, EventArgs e)
        {
            string keyword = textBoxEdit1.Text.Trim();

            if (keyword == "جستجو ...")
                return;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadReceiptsAsync(false);
                return;
            }

            try
            {
                var searchResults = await _goodsReceiptService.SearchReceiptsAsync(keyword, (int)dungeonNumeric1.Value);
                poisonDataGridView1.DataSource = searchResults.ToList();
                DgvPersian();
                CustomizeDataGridView();

                if (!searchResults.Any())
                    MessageBox.Show("هیچ رسیدی با عبارت مورد نظر یافت نشد.", "نتیجه جستجو",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در جستجو: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== دریافت شناسه رسید انتخاب‌شده ==========
        private void poisonDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && poisonDataGridView1.Rows[e.RowIndex].Cells["ReceiptId"].Value != null)
            {
                _ReceiptId = (int)poisonDataGridView1.Rows[e.RowIndex].Cells["ReceiptId"].Value;
            }
        }

        // ========== تأیید رسید ==========
        private async void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (_ReceiptId == 0)
            {
                MessageBox.Show("لطفاً یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(Program.CurrentUserId))
            {
                MessageBox.Show("لطفاً ابتدا وارد سیستم شوید.", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var result = await _goodsReceiptService.ApproveReceiptAsync(_ReceiptId, Program.CurrentUserId);
                if (result.Success)
                {
                    MessageBox.Show(result.Message, "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadReceiptsAsync(false);
                }
                else
                {
                    MessageBox.Show(result.Message, "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در تأیید رسید: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== ویرایش رسید ==========
        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_ReceiptId == 0)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var editForm = new Form12(_goodsReceiptService, _ReceiptId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                await LoadReceiptsAsync(showEmptyMessage: false);
            }
        }

        // ========== حذف رسید ==========
        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (_ReceiptId == 0)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("آیا از حذف این رسید انبار اطمینان دارید؟", "تأیید حذف",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _goodsReceiptService.RemoveByIdAsync(_ReceiptId);
                    MessageBox.Show("رسید انبار با موفقیت حذف شد.", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadReceiptsAsync(showEmptyMessage: false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در حذف رسید: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

      
    }
}