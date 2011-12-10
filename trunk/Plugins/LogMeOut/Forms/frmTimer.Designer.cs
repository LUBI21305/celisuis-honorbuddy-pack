namespace LogMeOut.Forms
{
    partial class frmTimer
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.labTimer = new System.Windows.Forms.Label();
            this.btnChangeState = new System.Windows.Forms.Button();
            this.timerRafraichissement = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Trebuchet MS", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(12, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Stop in";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // labTimer
            // 
            this.labTimer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.labTimer.Font = new System.Drawing.Font("Trebuchet MS", 18F, System.Drawing.FontStyle.Bold);
            this.labTimer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.labTimer.Location = new System.Drawing.Point(12, 33);
            this.labTimer.Name = "labTimer";
            this.labTimer.Size = new System.Drawing.Size(136, 23);
            this.labTimer.TabIndex = 1;
            this.labTimer.Text = "00:00:00";
            this.labTimer.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // btnChangeState
            // 
            this.btnChangeState.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnChangeState.Enabled = false;
            this.btnChangeState.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnChangeState.Location = new System.Drawing.Point(40, 65);
            this.btnChangeState.Name = "btnChangeState";
            this.btnChangeState.Size = new System.Drawing.Size(75, 23);
            this.btnChangeState.TabIndex = 2;
            this.btnChangeState.Text = "Pause";
            this.btnChangeState.UseVisualStyleBackColor = false;
            this.btnChangeState.Click += new System.EventHandler(this.btnChangeState_Click);
            // 
            // timerRafraichissement
            // 
            this.timerRafraichissement.Interval = 1000;
            this.timerRafraichissement.Tick += new System.EventHandler(this.timerRafraichissement_Tick);
            // 
            // frmTimer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(160, 100);
            this.Controls.Add(this.btnChangeState);
            this.Controls.Add(this.labTimer);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmTimer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LogMeOut! - Timer";
            this.TopMost = true;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labTimer;
        private System.Windows.Forms.Button btnChangeState;
        private System.Windows.Forms.Timer timerRafraichissement;
    }
}