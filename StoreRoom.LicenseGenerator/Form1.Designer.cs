namespace StoreRoom.LicenseGenerator
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtMachineCode = new TextBox();
            numDays = new NumericUpDown();
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            saveFileDialog1 = new SaveFileDialog();
            ((System.ComponentModel.ISupportInitialize)numDays).BeginInit();
            SuspendLayout();
            // 
            // txtMachineCode
            // 
            txtMachineCode.Location = new Point(41, 45);
            txtMachineCode.Margin = new Padding(4);
            txtMachineCode.Multiline = true;
            txtMachineCode.Name = "txtMachineCode";
            txtMachineCode.Size = new Size(651, 55);
            txtMachineCode.TabIndex = 0;
            // 
            // numDays
            // 
            numDays.Location = new Point(207, 157);
            numDays.Margin = new Padding(4);
            numDays.Name = "numDays";
            numDays.Size = new Size(384, 27);
            numDays.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(247, 224);
            button1.Margin = new Padding(4);
            button1.Name = "button1";
            button1.Size = new Size(276, 80);
            button1.TabIndex = 2;
            button1.Text = "ساخت لایسنس";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(712, 59);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(83, 19);
            label1.TabIndex = 3;
            label1.Text = "لایسنس :";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(712, 157);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(122, 19);
            label2.TabIndex = 4;
            label2.Text = "زمان لایسنس :";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 19F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(875, 312);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(numDays);
            Controls.Add(txtMachineCode);
            Font = new Font("Tahoma", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "Form1";
            RightToLeft = RightToLeft.Yes;
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)numDays).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtMachineCode;
        private NumericUpDown numDays;
        private Button button1;
        private Label label1;
        private Label label2;
        private SaveFileDialog saveFileDialog1;
    }
}
