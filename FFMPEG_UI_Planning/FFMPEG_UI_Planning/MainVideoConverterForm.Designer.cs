namespace FFMPEG_UI_Planning
{
    partial class MainVideoConverterForm
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
            this.lBDirectories = new System.Windows.Forms.ListBox();
            this.tbOutputText = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.tbPatientName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnAddDir = new System.Windows.Forms.Button();
            this.btnRemoveDir = new System.Windows.Forms.Button();
            this.btnStartConversion = new System.Windows.Forms.Button();
            this.datePicker = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.TBTestName = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbOutputFileName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tbTime = new System.Windows.Forms.TextBox();
            this.lblTime = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lbFiles = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lBDirectories
            // 
            this.lBDirectories.FormattingEnabled = true;
            this.lBDirectories.Location = new System.Drawing.Point(29, 34);
            this.lBDirectories.Name = "lBDirectories";
            this.lBDirectories.Size = new System.Drawing.Size(178, 212);
            this.lBDirectories.TabIndex = 0;
            this.lBDirectories.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // tbOutputText
            // 
            this.tbOutputText.Location = new System.Drawing.Point(110, 372);
            this.tbOutputText.Multiline = true;
            this.tbOutputText.Name = "tbOutputText";
            this.tbOutputText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutputText.Size = new System.Drawing.Size(681, 116);
            this.tbOutputText.TabIndex = 1;
            this.tbOutputText.Text = "This is where the output of the video conversion and the status will be displayed" +
    "\r\n";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(125, 343);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(641, 23);
            this.progressBar.TabIndex = 2;
            // 
            // tbPatientName
            // 
            this.tbPatientName.Location = new System.Drawing.Point(101, 37);
            this.tbPatientName.Name = "tbPatientName";
            this.tbPatientName.Size = new System.Drawing.Size(229, 20);
            this.tbPatientName.TabIndex = 3;
            this.tbPatientName.LostFocus += new System.EventHandler(this.tbPatientName_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Patient Name:";
            // 
            // btnAddDir
            // 
            this.btnAddDir.Location = new System.Drawing.Point(46, 266);
            this.btnAddDir.Name = "btnAddDir";
            this.btnAddDir.Size = new System.Drawing.Size(58, 23);
            this.btnAddDir.TabIndex = 5;
            this.btnAddDir.Text = "add";
            this.btnAddDir.UseVisualStyleBackColor = true;
            this.btnAddDir.Click += new System.EventHandler(this.btnAddDir_Click);
            // 
            // btnRemoveDir
            // 
            this.btnRemoveDir.Location = new System.Drawing.Point(125, 265);
            this.btnRemoveDir.Name = "btnRemoveDir";
            this.btnRemoveDir.Size = new System.Drawing.Size(62, 23);
            this.btnRemoveDir.TabIndex = 6;
            this.btnRemoveDir.Text = "remove";
            this.btnRemoveDir.UseVisualStyleBackColor = true;
            this.btnRemoveDir.Click += new System.EventHandler(this.btnRemoveDir_Click);
            // 
            // btnStartConversion
            // 
            this.btnStartConversion.BackColor = System.Drawing.Color.LightGreen;
            this.btnStartConversion.Location = new System.Drawing.Point(234, 278);
            this.btnStartConversion.Name = "btnStartConversion";
            this.btnStartConversion.Size = new System.Drawing.Size(453, 59);
            this.btnStartConversion.TabIndex = 7;
            this.btnStartConversion.Text = "START CONVERSION";
            this.btnStartConversion.UseVisualStyleBackColor = false;
            this.btnStartConversion.Click += new System.EventHandler(this.StartConversion_Click);
            // 
            // datePicker
            // 
            this.datePicker.CustomFormat = "MM/dd/yyyy";
            this.datePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.datePicker.Location = new System.Drawing.Point(101, 114);
            this.datePicker.Name = "datePicker";
            this.datePicker.Size = new System.Drawing.Size(229, 20);
            this.datePicker.TabIndex = 8;
            this.datePicker.Value = new System.DateTime(2017, 7, 3, 11, 7, 38, 0);
            this.datePicker.ValueChanged += new System.EventHandler(this.datePicker_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 114);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Date:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.TBTestName);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.tbOutputFileName);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.tbTime);
            this.groupBox1.Controls.Add(this.lblTime);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lbFiles);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.tbPatientName);
            this.groupBox1.Controls.Add(this.datePicker);
            this.groupBox1.Location = new System.Drawing.Point(234, 21);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 225);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Video Data Details";
            // 
            // TBTestName
            // 
            this.TBTestName.Location = new System.Drawing.Point(101, 73);
            this.TBTestName.Name = "TBTestName";
            this.TBTestName.Size = new System.Drawing.Size(229, 20);
            this.TBTestName.TabIndex = 17;
            this.TBTestName.TextChanged += new System.EventHandler(this.TBTestName_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 186);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 13);
            this.label6.TabIndex = 16;
            this.label6.Text = "Output File:";
            // 
            // tbOutputFileName
            // 
            this.tbOutputFileName.Location = new System.Drawing.Point(101, 186);
            this.tbOutputFileName.Name = "tbOutputFileName";
            this.tbOutputFileName.ReadOnly = true;
            this.tbOutputFileName.Size = new System.Drawing.Size(229, 20);
            this.tbOutputFileName.TabIndex = 15;
            this.tbOutputFileName.LostFocus += new System.EventHandler(this.tbOutputFileName_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(25, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Test Name:";
            // 
            // tbTime
            // 
            this.tbTime.Location = new System.Drawing.Point(101, 144);
            this.tbTime.Name = "tbTime";
            this.tbTime.ReadOnly = true;
            this.tbTime.Size = new System.Drawing.Size(229, 20);
            this.tbTime.TabIndex = 13;
            this.tbTime.TextChanged += new System.EventHandler(this.tbTime_TextChanged);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Location = new System.Drawing.Point(22, 147);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(33, 13);
            this.lblTime.TabIndex = 12;
            this.lblTime.Text = "Time:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(378, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(115, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Files To Be Converted:";
            // 
            // lbFiles
            // 
            this.lbFiles.FormattingEnabled = true;
            this.lbFiles.Location = new System.Drawing.Point(379, 37);
            this.lbFiles.Name = "lbFiles";
            this.lbFiles.Size = new System.Drawing.Size(153, 173);
            this.lbFiles.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(30, 11);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Directory Name:";
            // 
            // MainVideoConverterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(837, 504);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnStartConversion);
            this.Controls.Add(this.btnRemoveDir);
            this.Controls.Add(this.btnAddDir);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.tbOutputText);
            this.Controls.Add(this.lBDirectories);
            this.Name = "MainVideoConverterForm";
            this.Text = "Video Converter";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lBDirectories;
        private System.Windows.Forms.TextBox tbOutputText;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.TextBox tbPatientName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddDir;
        private System.Windows.Forms.Button btnRemoveDir;
        private System.Windows.Forms.Button btnStartConversion;
        private System.Windows.Forms.DateTimePicker datePicker;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListBox lbFiles;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.TextBox tbTime;
        private System.Windows.Forms.TextBox tbOutputFileName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TBTestName;
        private System.Windows.Forms.Label label6;
    }
}

