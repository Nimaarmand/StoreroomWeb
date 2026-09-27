namespace StoreRoom.Forms
{
    partial class FormMachineCode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            foreverButton1 = new ReaLTaiizor.Controls.ForeverButton();
            label4 = new Label();
            txtMachineCode = new ReaLTaiizor.Controls.TextBoxEdit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(foreverButton1);
            panel1.Controls.Add(label4);
            panel1.Controls.Add(txtMachineCode);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 64);
            panel1.Margin = new Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new Size(622, 363);
            panel1.TabIndex = 0;
            // 
            // foreverButton1
            // 
            foreverButton1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            foreverButton1.BackColor = Color.Transparent;
            foreverButton1.BaseColor = Color.FromArgb(0, 192, 192);
            foreverButton1.Font = new Font("Tahoma", 12F, FontStyle.Bold);
            foreverButton1.Location = new Point(134, 94);
            foreverButton1.Name = "foreverButton1";
            foreverButton1.Rounded = false;
            foreverButton1.Size = new Size(233, 70);
            foreverButton1.TabIndex = 44;
            foreverButton1.Text = "کپی کردن کد";
            foreverButton1.TextColor = Color.Red;
            foreverButton1.Click += foreverButton1_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(450, 4);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(96, 19);
            label4.TabIndex = 43;
            label4.Text = "کد ماشین :";
            // 
            // txtMachineCode
            // 
            txtMachineCode.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtMachineCode.BackColor = Color.Transparent;
            txtMachineCode.Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtMachineCode.ForeColor = Color.White;
            txtMachineCode.Image = null;
            txtMachineCode.Location = new Point(68, 4);
            txtMachineCode.Margin = new Padding(4);
            txtMachineCode.MaxLength = 32767;
            txtMachineCode.Multiline = false;
            txtMachineCode.Name = "txtMachineCode";
            txtMachineCode.ReadOnly = false;
            txtMachineCode.Size = new Size(373, 43);
            txtMachineCode.TabIndex = 42;
            txtMachineCode.TextAlignment = HorizontalAlignment.Left;
            txtMachineCode.UseSystemPasswordChar = false;
            // 
            // FormMachineCode
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(629, 431);
            Controls.Add(panel1);
            Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "FormMachineCode";
            Padding = new Padding(3, 64, 4, 4);
            RightToLeft = RightToLeft.Yes;
            Text = "دریافت کد";
            Load += FormMachineCode_Load_1;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label4;
        private ReaLTaiizor.Controls.TextBoxEdit txtMachineCode;
        private ReaLTaiizor.Controls.ForeverButton foreverButton1;
    }
}