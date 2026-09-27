using Application.Features.Implementation.Customer_Service;
using Application.Features.Implementation.GoodsIssue_Service;
using Application.Features.Implementation.GoodsReceipt_Service;
using Application.Features.Implementation.Product_Service;
using Application.Features.Implementation.Supplier_Service;
using Application.Features.Implementation.Warehouse_Service;
using Domain.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ReaLTaiizor.Forms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace StoreRoom.Forms
{
    public partial class Form7 : MaterialForm
    {
        private readonly ProductService _productService;
        private Guid _productId;

        public Form7(ProductService productService)
        {
            InitializeComponent();
            _productService = productService;
        }

        private void Form7_Load(object sender, EventArgs e)
        {
            textBoxEdit1.Text = "جستجو ...";
            poisonDataGridView1.CellFormatting += poisonDataGridView1_CellFormatting;
        }

        private void textBoxEdit1_Enter(object sender, EventArgs e)
        {
            if (textBoxEdit1.Text == "جستجو ...")
                textBoxEdit1.Text = "";
        }

        // ========== تنظیمات دیتاگرید ==========
        private void DgvPersian()
        {
            if (poisonDataGridView1.Columns.Contains("ProductId"))
                poisonDataGridView1.Columns["ProductId"].Visible = false;
            if (poisonDataGridView1.Columns.Contains("CategoryId"))
                poisonDataGridView1.Columns["CategoryId"].Visible = false;
            if (poisonDataGridView1.Columns.Contains("IsActive"))
                poisonDataGridView1.Columns["IsActive"].Visible = false;
            if (poisonDataGridView1.Columns.Contains("Weight"))
                poisonDataGridView1.Columns["Weight"].Visible = false;
            if (poisonDataGridView1.Columns.Contains("Barcode"))
                poisonDataGridView1.Columns["Barcode"].Visible = false;

            poisonDataGridView1.Columns["Name"].HeaderText = "نام";
            poisonDataGridView1.Columns["Description"].HeaderText = "توضیحات";
            poisonDataGridView1.Columns["BaseUnit"].HeaderText = "واحد";
            if (poisonDataGridView1.Columns.Contains("Category"))
                poisonDataGridView1.Columns["Category"].HeaderText = "دسته بندی";
            poisonDataGridView1.Columns["Number"].HeaderText = "تعداد";
            poisonDataGridView1.Columns["MaxStockLevel"].HeaderText = "حداکثر";
            poisonDataGridView1.Columns["MinStockLevel"].HeaderText = "حداقل";
            poisonDataGridView1.Columns["ProductionDate"].HeaderText = "تاریخ تولید";
            poisonDataGridView1.Columns["ExpirationDate"].HeaderText = "تاریخ انقضا";
            poisonDataGridView1.Columns["CreatedAt"].HeaderText = "تاریخ ثبت";

            poisonDataGridView1.Columns["Name"].DisplayIndex = 0;
            poisonDataGridView1.Columns["Description"].DisplayIndex = 1;
            poisonDataGridView1.Columns["BaseUnit"].DisplayIndex = 2;
            poisonDataGridView1.Columns["Category"].DisplayIndex = 3;
            poisonDataGridView1.Columns["Number"].DisplayIndex = 4;
            poisonDataGridView1.Columns["MinStockLevel"].DisplayIndex = 5;
            poisonDataGridView1.Columns["MaxStockLevel"].DisplayIndex = 6;
            poisonDataGridView1.Columns["ProductionDate"].DisplayIndex = 7;
            poisonDataGridView1.Columns["ExpirationDate"].DisplayIndex = 8;
            poisonDataGridView1.Columns["CreatedAt"].DisplayIndex = 9;
        }

        private void CustomizeDataGridView()
        {
            poisonDataGridView1.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.DefaultCellStyle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 12, FontStyle.Bold);
            poisonDataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            poisonDataGridView1.ReadOnly = true;
        }

        // ========== بارگذاری داده‌ها ==========
        private async Task LoadProductsAsync(bool showEmptyMessage = false)
        {
            int count = (int)dungeonNumeric1.Value;
            if (count <= 0) count = 20;

            var products = await _productService.GetTopProductsAsync(count);
            poisonDataGridView1.DataSource = products.ToList();

            DgvPersian();
            CustomizeDataGridView();

            if (showEmptyMessage && !products.Any())
                MessageBox.Show("هیچ محصولی یافت نشد.", "اطلاع", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ========== رویداد دکمه نمایش ==========
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
                await LoadProductsAsync(showEmptyMessage: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ========== دریافت شناسه محصول انتخاب‌شده ==========
        private void poisonDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && poisonDataGridView1.Rows[e.RowIndex].Cells["ProductId"].Value != null)
            {
                _productId = (Guid)poisonDataGridView1.Rows[e.RowIndex].Cells["ProductId"].Value;
            }
        }

        // ========== منوی ویرایش ==========
        private async void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_productId == Guid.Empty)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var editForm = new Form5(_productService, _productId);
            if (editForm.ShowDialog() == DialogResult.OK)
            {
                await LoadProductsAsync(showEmptyMessage: false);
            }
        }

        // ========== جستجوی زنده) ==========

        private async void textBoxEdit1_TextChanged_1(object sender, EventArgs e)
        {
            string keyword = textBoxEdit1.Text.Trim();

            // اگر متن placeholder است، هیچ کاری نکن
            if (keyword == "جستجو ...")
                return;

            // اگر جستجو خالی شد، لیست کامل را نمایش بده
            if (string.IsNullOrWhiteSpace(keyword))
            {
                await LoadProductsAsync(showEmptyMessage: false);
                return;
            }

            try
            {
                var searchResults = await _productService.SearchByNameAsync(keyword);
                poisonDataGridView1.DataSource = searchResults.ToList();
                DgvPersian();
                CustomizeDataGridView();

                if (!searchResults.Any())
                    MessageBox.Show("هیچ محصولی با این نام یافت نشد.", "نتیجه جستجو",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطا در جستجو: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // ========== رویداد CellFormatting برای نمایش نام دسته‌بندی ==========
        private void poisonDataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.Value == null || e.Value == DBNull.Value)
            {
                if (poisonDataGridView1.Columns[e.ColumnIndex].Name == "Category")
                {
                    e.Value = "بدون دسته‌بندی";
                    e.FormattingApplied = true;
                }
                return;
            }

            if (poisonDataGridView1.Columns[e.ColumnIndex].Name == "Category" && e.Value is Category category)
            {
                e.Value = category.Name;
                e.FormattingApplied = true;
            }
        }

        // ==========  ثبت ورود کالا ==========
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (_productId == Guid.Empty)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var goodsReceiptService = Program.ServiceProvider.GetRequiredService<GoodsReceiptService>();
            var warehouseService = Program.ServiceProvider.GetRequiredService<WarehouseService>();
            var supplierService = Program.ServiceProvider.GetRequiredService<SupplierService>();
            var userManager = Program.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var createform = new Form12(goodsReceiptService, warehouseService, supplierService, userManager, _productId);
            createform.ShowDialog();
        }

        // ==========  ثبت خروج کالا ==========
        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (_productId == Guid.Empty)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var goodsIssueService = Program.ServiceProvider.GetRequiredService<GoodsIssueService>();
            var warehouseService = Program.ServiceProvider.GetRequiredService<WarehouseService>();
            var customerService = Program.ServiceProvider.GetRequiredService<CustomerService>();
            var userManager = Program.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var createform = new Form15(goodsIssueService, warehouseService, customerService, userManager, _productId);
            createform.ShowDialog();
        }

        // ========== حذف محصول ==========
        private async void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (_productId == Guid.Empty)
            {
                MessageBox.Show("لطفاً ابتدا یک سطر را انتخاب کنید.", "اخطار", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("آیا از حذف این محصول اطمینان دارید؟", "تأیید حذف",
                                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    await _productService.DeleteProductAsync(_productId);
                    MessageBox.Show("محصول با موفقیت حذف شد.", "موفق", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadProductsAsync(showEmptyMessage: false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"خطا در حذف محصول: {ex.Message}", "خطا", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

     
    }
}