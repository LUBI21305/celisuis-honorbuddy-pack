namespace HighVoltz
{
    partial class MaterialListForm
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
            this.MaterialGridView = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.MaterialGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // MaterialGridView
            // 
            this.MaterialGridView.AllowUserToAddRows = false;
            this.MaterialGridView.AllowUserToResizeColumns = false;
            this.MaterialGridView.AllowUserToResizeRows = false;
            this.MaterialGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.MaterialGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.MaterialGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MaterialGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.MaterialGridView.Location = new System.Drawing.Point(0, 0);
            this.MaterialGridView.Name = "MaterialGridView";
            this.MaterialGridView.ReadOnly = true;
            this.MaterialGridView.RowHeadersVisible = false;
            this.MaterialGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MaterialGridView.Size = new System.Drawing.Size(493, 443);
            this.MaterialGridView.TabIndex = 0;
            // 
            // Column1
            // 
            this.Column1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Column1.FillWeight = 150F;
            this.Column1.HeaderText = "Name";
            this.Column1.MinimumWidth = 150;
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            // 
            // Column2
            // 
            this.Column2.FillWeight = 55F;
            this.Column2.HeaderText = "Required";
            this.Column2.MinimumWidth = 55;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 55;
            // 
            // Column3
            // 
            this.Column3.FillWeight = 55F;
            this.Column3.HeaderText = "Bags";
            this.Column3.MinimumWidth = 55;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 55;
            // 
            // Column4
            // 
            this.Column4.FillWeight = 55F;
            this.Column4.HeaderText = "Bank";
            this.Column4.MinimumWidth = 55;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 55;
            // 
            // Column5
            // 
            this.Column5.FillWeight = 65F;
            this.Column5.HeaderText = "Gbank/Alts";
            this.Column5.MinimumWidth = 65;
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Width = 65;
            // 
            // MaterialListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 443);
            this.Controls.Add(this.MaterialGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MaterialListForm";
            this.Text = "Material List";
            this.Load += new System.EventHandler(this.MaterialListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MaterialGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView MaterialGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
    }
}