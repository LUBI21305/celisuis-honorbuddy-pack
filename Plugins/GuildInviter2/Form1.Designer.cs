namespace GuildInviter2
{
    partial class cfg
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.WhisperText = new System.Windows.Forms.TextBox();
            this.save = new System.Windows.Forms.Button();
            this.MinLevel = new System.Windows.Forms.NumericUpDown();
            this.MaxLevel = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.MinLevel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxLevel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(30, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "GuildInviter v2.0 Config";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 51);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Minimum level:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(13, 76);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Maximum Level:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 100);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "Whisper Text:";
            // 
            // WhisperText
            // 
            this.WhisperText.Location = new System.Drawing.Point(12, 119);
            this.WhisperText.MaxLength = 255;
            this.WhisperText.Multiline = true;
            this.WhisperText.Name = "WhisperText";
            this.WhisperText.Size = new System.Drawing.Size(260, 90);
            this.WhisperText.TabIndex = 4;
            // 
            // save
            // 
            this.save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.save.Location = new System.Drawing.Point(104, 223);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 5;
            this.save.Text = "save\'n\'close";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // MinLevel
            // 
            this.MinLevel.Location = new System.Drawing.Point(104, 51);
            this.MinLevel.Maximum = new decimal(new int[] {
            85,
            0,
            0,
            0});
            this.MinLevel.Name = "MinLevel";
            this.MinLevel.Size = new System.Drawing.Size(39, 20);
            this.MinLevel.TabIndex = 6;
            // 
            // MaxLevel
            // 
            this.MaxLevel.Location = new System.Drawing.Point(104, 74);
            this.MaxLevel.Maximum = new decimal(new int[] {
            85,
            0,
            0,
            0});
            this.MaxLevel.Name = "MaxLevel";
            this.MaxLevel.Size = new System.Drawing.Size(39, 20);
            this.MaxLevel.TabIndex = 7;
            // 
            // cfg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 254);
            this.Controls.Add(this.MaxLevel);
            this.Controls.Add(this.MinLevel);
            this.Controls.Add(this.save);
            this.Controls.Add(this.WhisperText);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "cfg";
            this.Text = "GuildInviter v2.0 Config";
            this.Load += new System.EventHandler(this.cfg_Load);
            ((System.ComponentModel.ISupportInitialize)(this.MinLevel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.MaxLevel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox WhisperText;
        private System.Windows.Forms.Button save;
        private System.Windows.Forms.NumericUpDown MinLevel;
        private System.Windows.Forms.NumericUpDown MaxLevel;
    }
}