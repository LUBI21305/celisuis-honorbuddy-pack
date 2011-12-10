namespace RoutesBuddy
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
            this.ImportBut = new System.Windows.Forms.Button();
            this.ProfileTypeCombo = new System.Windows.Forms.ComboBox();
            this.ExportBut = new System.Windows.Forms.Button();
            this.HeightNumeric = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.smoothCheck = new System.Windows.Forms.CheckBox();
            this.RouteList = new System.Windows.Forms.DataGridView();
            this.Routes = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DebugButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumeric)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RouteList)).BeginInit();
            this.SuspendLayout();
            // 
            // ImportBut
            // 
            this.ImportBut.Location = new System.Drawing.Point(11, 256);
            this.ImportBut.Name = "ImportBut";
            this.ImportBut.Size = new System.Drawing.Size(88, 23);
            this.ImportBut.TabIndex = 0;
            this.ImportBut.Text = "Import";
            this.ImportBut.UseVisualStyleBackColor = true;
            this.ImportBut.Click += new System.EventHandler(this.ImportBut_Click);
            // 
            // ProfileTypeCombo
            // 
            this.ProfileTypeCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProfileTypeCombo.Enabled = false;
            this.ProfileTypeCombo.FormattingEnabled = true;
            this.ProfileTypeCombo.Items.AddRange(new object[] {
            "GB",
            "HB"});
            this.ProfileTypeCombo.Location = new System.Drawing.Point(93, 308);
            this.ProfileTypeCombo.Name = "ProfileTypeCombo";
            this.ProfileTypeCombo.Size = new System.Drawing.Size(100, 21);
            this.ProfileTypeCombo.TabIndex = 2;
            // 
            // ExportBut
            // 
            this.ExportBut.Enabled = false;
            this.ExportBut.Location = new System.Drawing.Point(105, 256);
            this.ExportBut.Name = "ExportBut";
            this.ExportBut.Size = new System.Drawing.Size(88, 23);
            this.ExportBut.TabIndex = 3;
            this.ExportBut.Text = "Export";
            this.ExportBut.UseVisualStyleBackColor = true;
            this.ExportBut.Click += new System.EventHandler(this.ExportBut_Click);
            // 
            // HeightNumeric
            // 
            this.HeightNumeric.Enabled = false;
            this.HeightNumeric.Location = new System.Drawing.Point(12, 309);
            this.HeightNumeric.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.HeightNumeric.Minimum = new decimal(new int[] {
            200,
            0,
            0,
            -2147483648});
            this.HeightNumeric.Name = "HeightNumeric";
            this.HeightNumeric.Size = new System.Drawing.Size(64, 20);
            this.HeightNumeric.TabIndex = 4;
            this.HeightNumeric.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 293);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Height";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(130, 292);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Profile Type";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(12, 256);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(182, 23);
            this.progressBar.TabIndex = 7;
            this.progressBar.Visible = false;
            // 
            // smoothCheck
            // 
            this.smoothCheck.AutoSize = true;
            this.smoothCheck.Checked = true;
            this.smoothCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.smoothCheck.Location = new System.Drawing.Point(11, 335);
            this.smoothCheck.Name = "smoothCheck";
            this.smoothCheck.Size = new System.Drawing.Size(86, 17);
            this.smoothCheck.TabIndex = 8;
            this.smoothCheck.Text = "Smooth path";
            this.smoothCheck.UseVisualStyleBackColor = true;
            // 
            // RouteList
            // 
            this.RouteList.AllowUserToAddRows = false;
            this.RouteList.AllowUserToDeleteRows = false;
            this.RouteList.AllowUserToResizeColumns = false;
            this.RouteList.AllowUserToResizeRows = false;
            this.RouteList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.RouteList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Routes});
            this.RouteList.Location = new System.Drawing.Point(11, 12);
            this.RouteList.Name = "RouteList";
            this.RouteList.RowHeadersVisible = false;
            this.RouteList.RowHeadersWidth = 23;
            this.RouteList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.RouteList.Size = new System.Drawing.Size(182, 238);
            this.RouteList.TabIndex = 9;
            // 
            // Routes
            // 
            this.Routes.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Routes.HeaderText = "Routes";
            this.Routes.Name = "Routes";
            // 
            // DebugButton
            // 
            this.DebugButton.Enabled = false;
            this.DebugButton.Location = new System.Drawing.Point(93, 335);
            this.DebugButton.Name = "DebugButton";
            this.DebugButton.Size = new System.Drawing.Size(100, 21);
            this.DebugButton.TabIndex = 10;
            this.DebugButton.Text = "Debug";
            this.DebugButton.UseVisualStyleBackColor = true;
            this.DebugButton.Click += new System.EventHandler(this.DebugButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(205, 368);
            this.Controls.Add(this.DebugButton);
            this.Controls.Add(this.RouteList);
            this.Controls.Add(this.smoothCheck);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.HeightNumeric);
            this.Controls.Add(this.ExportBut);
            this.Controls.Add(this.ProfileTypeCombo);
            this.Controls.Add(this.ImportBut);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MainForm";
            this.Text = "RoutesBuddy";
            ((System.ComponentModel.ISupportInitialize)(this.HeightNumeric)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RouteList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ImportBut;
        private System.Windows.Forms.ComboBox ProfileTypeCombo;
        private System.Windows.Forms.Button ExportBut;
        private System.Windows.Forms.NumericUpDown HeightNumeric;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ProgressBar progressBar;
        public System.Windows.Forms.CheckBox smoothCheck;
        private System.Windows.Forms.DataGridView RouteList;
        private System.Windows.Forms.DataGridViewTextBoxColumn Routes;
        private System.Windows.Forms.Button DebugButton;

    }
}