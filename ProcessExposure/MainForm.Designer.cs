namespace ProcessExposure
{
    partial class MainForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.txtStartFile = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEndFile = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtExposureValue = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtIncrementValue = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnIncreaseValue = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(50, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "文件路径：";
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(232, 49);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.Size = new System.Drawing.Size(497, 31);
            this.txtLocation.TabIndex = 1;
            // 
            // txtStartFile
            // 
            this.txtStartFile.Location = new System.Drawing.Point(232, 133);
            this.txtStartFile.Name = "txtStartFile";
            this.txtStartFile.Size = new System.Drawing.Size(231, 31);
            this.txtStartFile.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(50, 133);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(138, 25);
            this.label2.TabIndex = 2;
            this.label2.Text = "初始文件名：";
            // 
            // txtEndFile
            // 
            this.txtEndFile.Location = new System.Drawing.Point(232, 209);
            this.txtEndFile.Name = "txtEndFile";
            this.txtEndFile.Size = new System.Drawing.Size(231, 31);
            this.txtEndFile.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 212);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(138, 25);
            this.label3.TabIndex = 4;
            this.label3.Text = "结束文件名：";
            // 
            // txtExposureValue
            // 
            this.txtExposureValue.Location = new System.Drawing.Point(177, 72);
            this.txtExposureValue.Name = "txtExposureValue";
            this.txtExposureValue.Size = new System.Drawing.Size(155, 31);
            this.txtExposureValue.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(17, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 25);
            this.label4.TabIndex = 6;
            this.label4.Text = "初始曝光值：";
            // 
            // txtIncrementValue
            // 
            this.txtIncrementValue.Location = new System.Drawing.Point(177, 156);
            this.txtIncrementValue.Name = "txtIncrementValue";
            this.txtIncrementValue.Size = new System.Drawing.Size(155, 31);
            this.txtIncrementValue.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(138, 25);
            this.label5.TabIndex = 8;
            this.label5.Text = "递增曝光值：";
            // 
            // btnIncreaseValue
            // 
            this.btnIncreaseValue.Location = new System.Drawing.Point(524, 75);
            this.btnIncreaseValue.Name = "btnIncreaseValue";
            this.btnIncreaseValue.Size = new System.Drawing.Size(175, 44);
            this.btnIncreaseValue.TabIndex = 11;
            this.btnIncreaseValue.Text = "执行";
            this.btnIncreaseValue.UseVisualStyleBackColor = true;
            this.btnIncreaseValue.Click += new System.EventHandler(this.btnIncreaseValue_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtIncrementValue);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnIncreaseValue);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txtExposureValue);
            this.groupBox1.Location = new System.Drawing.Point(55, 300);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(761, 202);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "设置曝光值";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(746, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 38);
            this.button1.TabIndex = 14;
            this.button1.Text = "浏览";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(873, 530);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtEndFile);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtStartFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(899, 780);
            this.Name = "MainForm";
            this.Text = "Process Exposure";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.TextBox txtStartFile;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEndFile;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtExposureValue;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtIncrementValue;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnIncreaseValue;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}

