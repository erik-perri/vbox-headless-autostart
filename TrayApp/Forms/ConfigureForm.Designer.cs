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
            this.labelLogLevel = new System.Windows.Forms.Label();
            this.groupBoxServiceConfiguration = new System.Windows.Forms.GroupBox();
            this.checkBoxKeepAwakeMenu = new System.Windows.Forms.CheckBox();
            this.labelShutdownWaitLimit = new System.Windows.Forms.Label();
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.Machines = new System.Windows.Forms.GroupBox();
            this.dataGridMachines = new TrayApp.Forms.ConfigureForm.DoubleBufferedDataGridView();
            this.columnMonitored = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnAutoStart = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnSaveState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnUuid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.labelDescription = new System.Windows.Forms.Label();
            this.groupBoxServiceConfiguration.SuspendLayout();
            this.Machines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMachines)).BeginInit();
            this.SuspendLayout();
            // 
            // labelLogLevel
            // 
            this.labelLogLevel.AutoSize = true;
            this.labelLogLevel.Location = new System.Drawing.Point(82, 28);
            this.labelLogLevel.Name = "labelLogLevel";
            this.labelLogLevel.Size = new System.Drawing.Size(54, 13);
            this.labelLogLevel.TabIndex = 3;
            this.labelLogLevel.Text = "Log Level";
            // 
            // groupBoxServiceConfiguration
            // 
            this.groupBoxServiceConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxKeepAwakeMenu);
            this.groupBoxServiceConfiguration.Controls.Add(this.labelShutdownWaitLimit);
            this.groupBoxServiceConfiguration.Controls.Add(this.comboBoxLogLevel);
            this.groupBoxServiceConfiguration.Controls.Add(this.labelLogLevel);
            this.groupBoxServiceConfiguration.Location = new System.Drawing.Point(12, 219);
            this.groupBoxServiceConfiguration.Name = "groupBoxServiceConfiguration";
            this.groupBoxServiceConfiguration.Size = new System.Drawing.Size(485, 93);
            this.groupBoxServiceConfiguration.TabIndex = 2;
            this.groupBoxServiceConfiguration.TabStop = false;
            this.groupBoxServiceConfiguration.Text = "Configuration";
            // 
            // checkBoxKeepAwakeMenu
            // 
            this.checkBoxKeepAwakeMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxKeepAwakeMenu.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxKeepAwakeMenu.Location = new System.Drawing.Point(142, 58);
            this.checkBoxKeepAwakeMenu.Name = "checkBoxKeepAwakeMenu";
            this.checkBoxKeepAwakeMenu.Size = new System.Drawing.Size(332, 24);
            this.checkBoxKeepAwakeMenu.TabIndex = 6;
            this.checkBoxKeepAwakeMenu.Text = "This will show an option in the system tray menu to prevent the host from going t" +
    "o sleep.";
            this.checkBoxKeepAwakeMenu.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxKeepAwakeMenu.UseVisualStyleBackColor = true;
            // 
            // labelShutdownWaitLimit
            // 
            this.labelShutdownWaitLimit.AutoSize = true;
            this.labelShutdownWaitLimit.Location = new System.Drawing.Point(11, 59);
            this.labelShutdownWaitLimit.Name = "labelShutdownWaitLimit";
            this.labelShutdownWaitLimit.Size = new System.Drawing.Size(125, 13);
            this.labelShutdownWaitLimit.TabIndex = 5;
            this.labelShutdownWaitLimit.Text = "Show keep awake menu";
            // 
            // comboBoxLogLevel
            // 
            this.comboBoxLogLevel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Items.AddRange(new object[] {
            "Trace",
            "Debug",
            "Information",
            "Warning",
            "Error",
            "Critical"});
            this.comboBoxLogLevel.Location = new System.Drawing.Point(142, 24);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(331, 21);
            this.comboBoxLogLevel.TabIndex = 4;
            // 
            // Machines
            // 
            this.Machines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Machines.Controls.Add(this.labelDescription);
            this.Machines.Controls.Add(this.dataGridMachines);
            this.Machines.Location = new System.Drawing.Point(12, 12);
            this.Machines.Name = "Machines";
            this.Machines.Size = new System.Drawing.Size(485, 196);
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
            this.columnMonitored,
            this.columnAutoStart,
            this.columnSaveState,
            this.columnName,
            this.columnUuid});
            this.dataGridMachines.DoubleBuffered = true;
            this.dataGridMachines.Location = new System.Drawing.Point(12, 25);
            this.dataGridMachines.Name = "dataGridMachines";
            this.dataGridMachines.RowHeadersVisible = false;
            this.dataGridMachines.Size = new System.Drawing.Size(462, 124);
            this.dataGridMachines.TabIndex = 1;
            this.dataGridMachines.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.Machines_CellMouseLeave);
            this.dataGridMachines.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Machines_CellMouseMove);
            this.dataGridMachines.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Machines_CellMouseUp);
            this.dataGridMachines.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Machines_CellValueChanged);
            this.dataGridMachines.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.Machines_RowPostPaint);
            this.dataGridMachines.SelectionChanged += new System.EventHandler(this.Machines_SelectionChanged);
            // 
            // columnMonitored
            // 
            this.columnMonitored.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.columnMonitored.DataPropertyName = "Monitored";
            this.columnMonitored.HeaderText = "Monitored";
            this.columnMonitored.Name = "columnMonitored";
            this.columnMonitored.Width = 60;
            // 
            // columnAutoStart
            // 
            this.columnAutoStart.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.columnAutoStart.DataPropertyName = "AutoStart";
            this.columnAutoStart.HeaderText = "Auto-Start";
            this.columnAutoStart.Name = "columnAutoStart";
            this.columnAutoStart.Width = 60;
            // 
            // columnSaveState
            // 
            this.columnSaveState.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.columnSaveState.DataPropertyName = "SaveState";
            this.columnSaveState.HeaderText = "Save State";
            this.columnSaveState.Name = "columnSaveState";
            this.columnSaveState.Width = 66;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnName.DataPropertyName = "Name";
            this.columnName.HeaderText = "Name";
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            // 
            // columnUuid
            // 
            this.columnUuid.DataPropertyName = "Uuid";
            this.columnUuid.HeaderText = "UUID";
            this.columnUuid.Name = "columnUuid";
            this.columnUuid.ReadOnly = true;
            this.columnUuid.Width = 210;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(407, 323);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(90, 23);
            this.buttonSave.TabIndex = 7;
            this.buttonSave.Text = "Save Changes";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSave);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 323);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancel);
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.Location = new System.Drawing.Point(11, 157);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(462, 26);
            this.labelDescription.TabIndex = 9;
            this.labelDescription.Text = "Unmonitored virtual machines will not be started or stopped with the host machine" +
    " and will not show up in the system tray menu.";
            this.labelDescription.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // ConfigureForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(509, 361);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.Machines);
            this.Controls.Add(this.groupBoxServiceConfiguration);
            this.MinimumSize = new System.Drawing.Size(525, 400);
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
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnMonitored;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnAutoStart;
        private System.Windows.Forms.DataGridViewCheckBoxColumn columnSaveState;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnUuid;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.CheckBox checkBoxKeepAwakeMenu;
        private System.Windows.Forms.Label labelShutdownWaitLimit;
        private DoubleBufferedDataGridView dataGridMachines;
    }
}