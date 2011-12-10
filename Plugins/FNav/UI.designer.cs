namespace fnav
{
    partial class UIForm1
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
            this.LocationList = new System.Windows.Forms.ComboBox();
            this.GoToLocation = new System.Windows.Forms.Button();
            this.AddCurrentLocation = new System.Windows.Forms.Button();
            this.NewLocationName = new System.Windows.Forms.TextBox();
            this.RemoveLocation = new System.Windows.Forms.Button();
            this.CategoryFilter = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.CategoryNew = new System.Windows.Forms.ComboBox();
            this.Subzone = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.useFlightPath = new System.Windows.Forms.CheckBox();
            this.ZoneFilter = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LocationList
            // 
            this.LocationList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.LocationList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.LocationList.FormattingEnabled = true;
            this.LocationList.Location = new System.Drawing.Point(6, 19);
            this.LocationList.Name = "LocationList";
            this.LocationList.Size = new System.Drawing.Size(416, 21);
            this.LocationList.Sorted = true;
            this.LocationList.TabIndex = 0;
            this.toolTip1.SetToolTip(this.LocationList, "The location you want to go to");
            this.LocationList.SelectedIndexChanged += new System.EventHandler(this.Locations_SelectedIndexChanged);
            // 
            // GoToLocation
            // 
            this.GoToLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.GoToLocation.Location = new System.Drawing.Point(300, 46);
            this.GoToLocation.Name = "GoToLocation";
            this.GoToLocation.Size = new System.Drawing.Size(122, 23);
            this.GoToLocation.TabIndex = 3;
            this.GoToLocation.Text = "Go To Location";
            this.toolTip1.SetToolTip(this.GoToLocation, "Make so Number One. Engage!");
            this.GoToLocation.UseVisualStyleBackColor = true;
            this.GoToLocation.Click += new System.EventHandler(this.GoToLocation_Click);
            this.GoToLocation.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GoToLocation_MouseDown);
            // 
            // AddCurrentLocation
            // 
            this.AddCurrentLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AddCurrentLocation.Location = new System.Drawing.Point(442, 45);
            this.AddCurrentLocation.Name = "AddCurrentLocation";
            this.AddCurrentLocation.Size = new System.Drawing.Size(122, 23);
            this.AddCurrentLocation.TabIndex = 3;
            this.AddCurrentLocation.Text = "Add Current Location";
            this.toolTip1.SetToolTip(this.AddCurrentLocation, "Add your current location to the database");
            this.AddCurrentLocation.UseVisualStyleBackColor = true;
            this.AddCurrentLocation.Click += new System.EventHandler(this.AddCurrentLocation_Click);
            // 
            // NewLocationName
            // 
            this.NewLocationName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.NewLocationName.Location = new System.Drawing.Point(148, 19);
            this.NewLocationName.Name = "NewLocationName";
            this.NewLocationName.Size = new System.Drawing.Size(416, 20);
            this.NewLocationName.TabIndex = 1;
            this.toolTip1.SetToolTip(this.NewLocationName, "The name of the location. \r\n\r\nNB: In most cases it is preferable to use the name " +
                    "provided by the Sub Zone Text button.\r\nWhen adding a Class Trainer simply add th" +
                    "e Class name; eg Druid.");
            // 
            // RemoveLocation
            // 
            this.RemoveLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RemoveLocation.Location = new System.Drawing.Point(172, 46);
            this.RemoveLocation.Name = "RemoveLocation";
            this.RemoveLocation.Size = new System.Drawing.Size(122, 23);
            this.RemoveLocation.TabIndex = 2;
            this.RemoveLocation.Text = "Remove Location";
            this.toolTip1.SetToolTip(this.RemoveLocation, "Remove the currently selected location from the database");
            this.RemoveLocation.UseVisualStyleBackColor = true;
            this.RemoveLocation.Click += new System.EventHandler(this.RemoveLocation_Click);
            // 
            // CategoryFilter
            // 
            this.CategoryFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CategoryFilter.FormattingEnabled = true;
            this.CategoryFilter.Items.AddRange(new object[] {
            "[All]",
            "Banks",
            "Class Trainers",
            "Flight Points",
            "Mailboxes",
            "Profession Trainers"});
            this.CategoryFilter.Location = new System.Drawing.Point(6, 19);
            this.CategoryFilter.Name = "CategoryFilter";
            this.CategoryFilter.Size = new System.Drawing.Size(118, 21);
            this.CategoryFilter.Sorted = true;
            this.CategoryFilter.TabIndex = 0;
            this.toolTip1.SetToolTip(this.CategoryFilter, "Filter the locations by Category");
            this.CategoryFilter.SelectedIndexChanged += new System.EventHandler(this.CategoryFilter_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.CategoryNew);
            this.groupBox1.Controls.Add(this.NewLocationName);
            this.groupBox1.Controls.Add(this.Subzone);
            this.groupBox1.Controls.Add(this.AddCurrentLocation);
            this.groupBox1.Location = new System.Drawing.Point(12, 97);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(570, 84);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Add new...";
            // 
            // CategoryNew
            // 
            this.CategoryNew.FormattingEnabled = true;
            this.CategoryNew.Items.AddRange(new object[] {
            "Auction House",
            "Banks",
            "Class Trainers",
            "Flight Paths",
            "Inns",
            "Mailboxes",
            "Misc",
            "Profession Trainers",
            "Quest Hubs",
            "Travel",
            "Vendors"});
            this.CategoryNew.Location = new System.Drawing.Point(6, 18);
            this.CategoryNew.Name = "CategoryNew";
            this.CategoryNew.Size = new System.Drawing.Size(118, 21);
            this.CategoryNew.Sorted = true;
            this.CategoryNew.TabIndex = 0;
            this.toolTip1.SetToolTip(this.CategoryNew, "Assign the new location to this category");
            // 
            // Subzone
            // 
            this.Subzone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Subzone.Location = new System.Drawing.Point(314, 45);
            this.Subzone.Name = "Subzone";
            this.Subzone.Size = new System.Drawing.Size(122, 23);
            this.Subzone.TabIndex = 2;
            this.Subzone.Text = "Subzone Text";
            this.toolTip1.SetToolTip(this.Subzone, "Show the current sub zone");
            this.Subzone.UseVisualStyleBackColor = true;
            this.Subzone.Click += new System.EventHandler(this.Subzone_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.useFlightPath);
            this.groupBox2.Controls.Add(this.LocationList);
            this.groupBox2.Controls.Add(this.GoToLocation);
            this.groupBox2.Controls.Add(this.RemoveLocation);
            this.groupBox2.Location = new System.Drawing.Point(154, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(428, 79);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Go to...";
            // 
            // useFlightPath
            // 
            this.useFlightPath.AutoSize = true;
            this.useFlightPath.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.useFlightPath.Location = new System.Drawing.Point(63, 50);
            this.useFlightPath.Name = "useFlightPath";
            this.useFlightPath.Size = new System.Drawing.Size(103, 17);
            this.useFlightPath.TabIndex = 1;
            this.useFlightPath.Text = "Use Flight Paths";
            this.toolTip1.SetToolTip(this.useFlightPath, "Toggle the Use Flight Paths setting in HB");
            this.useFlightPath.UseVisualStyleBackColor = true;
            this.useFlightPath.CheckedChanged += new System.EventHandler(this.useFlightPath_CheckedChanged);
            // 
            // ZoneFilter
            // 
            this.ZoneFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ZoneFilter.FormattingEnabled = true;
            this.ZoneFilter.Items.AddRange(new object[] {
            "[All]",
            "Banks",
            "Class Trainers",
            "Flight Points",
            "Mailboxes",
            "Profession Trainers"});
            this.ZoneFilter.Location = new System.Drawing.Point(6, 46);
            this.ZoneFilter.Name = "ZoneFilter";
            this.ZoneFilter.Size = new System.Drawing.Size(118, 21);
            this.ZoneFilter.Sorted = true;
            this.ZoneFilter.TabIndex = 1;
            this.toolTip1.SetToolTip(this.ZoneFilter, "Filter the locations by Zone");
            this.ZoneFilter.SelectedIndexChanged += new System.EventHandler(this.ZoneFilter_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.CategoryFilter);
            this.groupBox3.Controls.Add(this.ZoneFilter);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(136, 79);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Filter by...";
            // 
            // errorProvider1
            // 
            this.errorProvider1.BlinkRate = 350;
            this.errorProvider1.ContainerControl = this;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 188);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(594, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(96, 17);
            this.statusLabel.Text = "FNav by Fpsware";
            // 
            // toolTip1
            // 
            this.toolTip1.AutoPopDelay = 7000;
            this.toolTip1.InitialDelay = 500;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 100;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.toolTip1.ToolTipTitle = "FNav - The lazy mans taxi";
            // 
            // UIForm1
            // 
            this.AcceptButton = this.GoToLocation;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 210);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "UIForm1";
            this.Text = "FNav - The lazy mans taxi by Fpsware";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.UIForm1_FormClosing);
            this.Load += new System.EventHandler(this.UIForm1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox LocationList;
        private System.Windows.Forms.Button GoToLocation;
        private System.Windows.Forms.Button AddCurrentLocation;
        private System.Windows.Forms.TextBox NewLocationName;
        private System.Windows.Forms.Button RemoveLocation;
        private System.Windows.Forms.ComboBox CategoryFilter;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox CategoryNew;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox ZoneFilter;
        private System.Windows.Forms.Button Subzone;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox useFlightPath;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}