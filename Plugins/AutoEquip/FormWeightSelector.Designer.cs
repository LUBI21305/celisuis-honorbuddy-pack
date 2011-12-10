namespace AutoEquip
{
    partial class FormWeightSelector
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
            this.lbxWeightSets = new System.Windows.Forms.ListBox();
            this.lblClass = new System.Windows.Forms.Label();
            this.lblSpec = new System.Windows.Forms.Label();
            this.lblMisc = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lbxWeights = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbxWeightSets
            // 
            this.lbxWeightSets.FormattingEnabled = true;
            this.lbxWeightSets.Location = new System.Drawing.Point(12, 12);
            this.lbxWeightSets.Name = "lbxWeightSets";
            this.lbxWeightSets.Size = new System.Drawing.Size(172, 225);
            this.lbxWeightSets.TabIndex = 0;
            this.lbxWeightSets.SelectedIndexChanged += new System.EventHandler(this.lbxWeightSets_SelectedIndexChanged);
            // 
            // lblClass
            // 
            this.lblClass.AutoSize = true;
            this.lblClass.Location = new System.Drawing.Point(190, 12);
            this.lblClass.Name = "lblClass";
            this.lblClass.Size = new System.Drawing.Size(38, 13);
            this.lblClass.TabIndex = 1;
            this.lblClass.Text = "Class: ";
            // 
            // lblSpec
            // 
            this.lblSpec.AutoSize = true;
            this.lblSpec.Location = new System.Drawing.Point(190, 35);
            this.lblSpec.Name = "lblSpec";
            this.lblSpec.Size = new System.Drawing.Size(38, 13);
            this.lblSpec.TabIndex = 2;
            this.lblSpec.Text = "Spec: ";
            // 
            // lblMisc
            // 
            this.lblMisc.AutoSize = true;
            this.lblMisc.Location = new System.Drawing.Point(190, 58);
            this.lblMisc.Name = "lblMisc";
            this.lblMisc.Size = new System.Drawing.Size(35, 13);
            this.lblMisc.TabIndex = 3;
            this.lblMisc.Text = "Misc: ";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 243);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(299, 243);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(193, 243);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lbxWeights
            // 
            this.lbxWeights.Cursor = System.Windows.Forms.Cursors.Default;
            this.lbxWeights.FormattingEnabled = true;
            this.lbxWeights.Location = new System.Drawing.Point(193, 103);
            this.lbxWeights.Name = "lbxWeights";
            this.lbxWeights.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.lbxWeights.Size = new System.Drawing.Size(181, 134);
            this.lbxWeights.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(190, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Weights:";
            // 
            // FormWeightSelector
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(383, 273);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbxWeights);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lblMisc);
            this.Controls.Add(this.lblSpec);
            this.Controls.Add(this.lblClass);
            this.Controls.Add(this.lbxWeightSets);
            this.Name = "FormWeightSelector";
            this.Text = "Select Weight Set";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbxWeightSets;
        private System.Windows.Forms.Label lblClass;
        private System.Windows.Forms.Label lblSpec;
        private System.Windows.Forms.Label lblMisc;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ListBox lbxWeights;
        private System.Windows.Forms.Label label1;
    }
}