namespace TrayApp.Forms
{
    partial class ConfigureForm
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
            this.labelLogLevel = new System.Windows.Forms.Label();
            this.groupBoxServiceConfiguration = new System.Windows.Forms.GroupBox();
            this.checkBoxStartWithWindows = new System.Windows.Forms.CheckBox();
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.checkBoxKeepAwakeMenu = new System.Windows.Forms.CheckBox();
            this.Machines = new System.Windows.Forms.GroupBox();
            this.dataGridMachines = new TrayApp.Forms.ConfigureForm.DoubleBufferedDataGridView();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.columnShowInMenu = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnAutoStart = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnAutoStop = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnSaveState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBoxServiceConfiguration.SuspendLayout();
            this.Machines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMachines)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLogLevel
            // 
            this.labelLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelLogLevel.AutoSize = true;
            this.labelLogLevel.Location = new System.Drawing.Point(316, 26);
            this.labelLogLevel.Name = "labelLogLevel";
            this.labelLogLevel.Size = new System.Drawing.Size(54, 13);
            this.labelLogLevel.TabIndex = 6;
            this.labelLogLevel.Text = "Log Level";
            // 
            // groupBoxServiceConfiguration
            // 
            this.groupBoxServiceConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxStartWithWindows);
            this.groupBoxServiceConfiguration.Controls.Add(this.comboBoxLogLevel);
            this.groupBoxServiceConfiguration.Controls.Add(this.labelLogLevel);
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxKeepAwakeMenu);
            this.groupBoxServiceConfiguration.Location = new System.Drawing.Point(12, 303);
            this.groupBoxServiceConfiguration.Name = "groupBoxServiceConfiguration";
            this.groupBoxServiceConfiguration.Size = new System.Drawing.Size(485, 58);
            this.groupBoxServiceConfiguration.TabIndex = 3;
            this.groupBoxServiceConfiguration.TabStop = false;
            this.groupBoxServiceConfiguration.Text = "Configuration";
            // 
            // checkBoxKeepAwakeMenu
            // 
            this.checkBoxKeepAwakeMenu.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.checkBoxKeepAwakeMenu.AutoSize = true;
            this.checkBoxKeepAwakeMenu.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxKeepAwakeMenu.Location = new System.Drawing.Point(150, 24);
            this.checkBoxKeepAwakeMenu.Name = "checkBoxKeepAwakeMenu";
            this.checkBoxKeepAwakeMenu.Size = new System.Drawing.Size(147, 17);
            this.checkBoxKeepAwakeMenu.TabIndex = 5;
            this.checkBoxKeepAwakeMenu.Text = "Show keep awake option";
            this.checkBoxKeepAwakeMenu.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.toolTip.SetToolTip(this.checkBoxKeepAwakeMenu, "If enabled an option to keep the host from going to sleep will be added to the tr" +
        "ay menu.");
            this.checkBoxKeepAwakeMenu.UseVisualStyleBackColor = true;
            // 
            // comboBoxLogLevel
            // 
            this.comboBoxLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Items.AddRange(new object[] {
            "Trace",
            "Debug",
            "Information",
            "Warning",
            "Error",
            "Critical"});
            this.comboBoxLogLevel.Location = new System.Drawing.Point(376, 22);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(97, 21);
            this.comboBoxLogLevel.TabIndex = 7;
            // 
            // Machines
            // 
            this.Machines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Machines.Controls.Add(this.dataGridMachines);
            this.Machines.Location = new System.Drawing.Point(12, 12);
            this.Machines.Name = "Machines";
            this.Machines.Size = new System.Drawing.Size(485, 280);
            this.Machines.TabIndex = 0;
            this.Machines.TabStop = false;
            this.Machines.Text = "Virtual Machines";
            // 
            // dataGridMachines
            // 
            this.dataGridMachines.AllowUserToAddRows = false;
            this.dataGridMachines.AllowUserToDeleteRows = false;
            this.dataGridMachines.AllowUserToResizeRows = false;
            this.dataGridMachines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridMachines.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridMachines.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridMachines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridMachines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnShowInMenu,
            this.columnAutoStart,
            this.columnAutoStop,
            this.columnSaveState,
            this.columnName});
            this.dataGridMachines.DoubleBuffered = true;
            this.dataGridMachines.Location = new System.Drawing.Point(12, 25);
            this.dataGridMachines.Name = "dataGridMachines";
            this.dataGridMachines.RowHeadersVisible = false;
            this.dataGridMachines.Size = new System.Drawing.Size(462, 243);
            this.dataGridMachines.TabIndex = 1;
            this.dataGridMachines.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.Machines_CellMouseLeave);
            this.dataGridMachines.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Machines_CellMouseMove);
            this.dataGridMachines.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.Machines_CellToolTipTextNeeded);
            this.dataGridMachines.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.Machines_RowPostPaint);
            this.dataGridMachines.SelectionChanged += new System.EventHandler(this.Machines_SelectionChanged);
            // 
            // columnShowInMenu
            // 
            this.columnShowInMenu.DataPropertyName = "ShowMenu";
            this.columnShowInMenu.HeaderText = "Show in Menu";
            this.columnShowInMenu.Name = "columnShowInMenu";
            this.columnShowInMenu.Width = 95;
            // 
            // columnAutoStart
            // 
            this.columnAutoStart.DataPropertyName = "AutoStart";
            this.columnAutoStart.HeaderText = "Auto-Start";
            this.columnAutoStart.Name = "columnAutoStart";
            this.columnAutoStart.Width = 75;
            // 
            // columnAutoStop
            // 
            this.columnAutoStop.DataPropertyName = "AutoStop";
            this.columnAutoStop.HeaderText = "Auto-Stop";
            this.columnAutoStop.Name = "columnAutoStop";
            this.columnAutoStop.Width = 75;
            // 
            // columnSaveState
            // 
            this.columnSaveState.DataPropertyName = "SaveState";
            this.columnSaveState.HeaderText = "Save State";
            this.columnSaveState.Name = "columnSaveState";
            this.columnSaveState.Width = 75;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnName.DataPropertyName = "Name";
            this.columnName.HeaderText = "Name";
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(407, 373);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(90, 23);
            this.buttonSave.TabIndex = 8;
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSave);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 373);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 9;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancel);
            // 
            // checkBoxStartWithWindows
            // 
            this.checkBoxStartWithWindows.AutoSize = true;
            this.checkBoxStartWithWindows.Location = new System.Drawing.Point(12, 24);
            this.checkBoxStartWithWindows.Name = "checkBoxStartWithWindows";
            this.checkBoxStartWithWindows.Size = new System.Drawing.Size(117, 17);
            this.checkBoxStartWithWindows.TabIndex = 4;
            this.checkBoxStartWithWindows.Text = "Start with Windows";
            this.toolTip.SetToolTip(this.checkBoxStartWithWindows, "If enabled this application will start with Windows.  If this is disabled virtual" +
        " machines will not start with the host.");
            this.checkBoxStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // ConfigureForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(509, 411);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(525, 450);
            this.Name = "ConfigureForm";
            this.Text = "Configure";
            this.groupBoxServiceConfiguration.ResumeLayout(false);
            this.groupBoxServiceConfiguration.PerformLayout();
            this.Machines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMachines)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelLogLevel;
        private System.Windows.Forms.GroupBox groupBoxServiceConfiguration;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.GroupBox Machines;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnShowInMenu;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnAutoStart;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnAutoStop;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnSaveState;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.CheckBox checkBoxKeepAwakeMenu;
        private DoubleBufferedDataGridView dataGridMachines;
        private System.Windows.Forms.CheckBox checkBoxStartWithWindows;
        private System.Windows.Forms.ToolTip toolTip;
    }
}