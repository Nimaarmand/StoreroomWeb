namespace StoreRoom.Forms
{
    partial class ContactUs
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
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label4);
            panel1.Controls.Add(label3);
            panel1.Controls.Add(label2);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Fill;
            panel1.Location = new Point(3, 64);
            panel1.Name = "panel1";
            panel1.Size = new Size(983, 641);
            panel1.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(505, 56);
            label1.Name = "label1";
            label1.Size = new Size(431, 57);
            label1.TabIndex = 0;
            label1.Text = "از طریق راه های ارتباطی ذکر شده\r\n شما میتوانید نظرات و پیشنهادات خود\r\n را در راستای بهبود کارایی و عملکرد نرم افزار اعلام نمایید";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.ForeColor = Color.FromArgb(0, 192, 0);
            label2.Location = new Point(620, 157);
            label2.Name = "label2";
            label2.Size = new Size(307, 57);
            label2.TabIndex = 1;
            label2.Text = "اینستاگرام : nimaarmand_khoozestan\r\nایمیل : nimaarmand1379@gmail.com\r\nروبیکا : @user44174";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.ForeColor = Color.FromArgb(192, 192, 0);
            label3.Location = new Point(602, 256);
            label3.Name = "label3";
            label3.Size = new Size(325, 57);
            label3.TabIndex = 2;
            label3.Text = "جهت اطلاع از:\r\n بروز رسانی ها و اموزش کار با نرم افزار به \r\nپیج اینستاگرام مراجعه نمایید";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.Red;
            label4.Location = new Point(505, 372);
            label4.Name = "label4";
            label4.Size = new Size(424, 57);
            label4.TabIndex = 3;
            label4.Text = "توجه توجه ** \r\nحق نشر نرم افزار محفوظ می باشد \r\nهرگونه استفاده بدون اجازه و غیر مجاز پیگرد قانونی دارد";
            // 
            // ContactUs
            // 
            AutoScaleDimensions = new SizeF(10F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(990, 709);
            Controls.Add(panel1);
            Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4, 4, 4, 4);
            Name = "ContactUs";
            Padding = new Padding(3, 64, 4, 4);
            RightToLeft = RightToLeft.Yes;
            Text = "ارتباط ما";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private Label label3;
        private Label label2;
        private Label label4;
    }
}