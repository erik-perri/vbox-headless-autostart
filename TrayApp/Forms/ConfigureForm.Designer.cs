using System;
using System.Windows.Forms;

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigureForm));
            this.labelLogLevel = new System.Windows.Forms.Label();
            this.checkBoxStartWithWindows = new System.Windows.Forms.CheckBox();
            this.comboBoxLogLevel = new System.Windows.Forms.ComboBox();
            this.checkBoxKeepAwakeMenu = new System.Windows.Forms.CheckBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.checkBoxTrayIcon = new System.Windows.Forms.CheckBox();
            this.dataGridMachines = new TrayApp.Forms.ConfigureForm.DoubleBufferedDataGridView();
            this.columnEnabled = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnShowInMenu = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnAutoStart = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnAutoStop = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.columnSaveState = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBoxServiceConfiguration = new System.Windows.Forms.GroupBox();
            this.tabControlMachines = new System.Windows.Forms.TabControl();
            this.tabPageMachines = new System.Windows.Forms.TabPage();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.groupBoxLogging = new System.Windows.Forms.GroupBox();
            this.buttonExit = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMachines)).BeginInit();
            this.groupBoxServiceConfiguration.SuspendLayout();
            this.tabControlMachines.SuspendLayout();
            this.tabPageMachines.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            this.groupBoxLogging.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelLogLevel
            // 
            this.labelLogLevel.AutoSize = true;
            this.labelLogLevel.Location = new System.Drawing.Point(8, 24);
            this.labelLogLevel.Name = "labelLogLevel";
            this.labelLogLevel.Size = new System.Drawing.Size(54, 13);
            this.labelLogLevel.TabIndex = 8;
            this.labelLogLevel.Text = "Log Level";
            // 
            // checkBoxStartWithWindows
            // 
            this.checkBoxStartWithWindows.AutoSize = true;
            this.checkBoxStartWithWindows.Location = new System.Drawing.Point(12, 24);
            this.checkBoxStartWithWindows.Name = "checkBoxStartWithWindows";
            this.checkBoxStartWithWindows.Size = new System.Drawing.Size(239, 17);
            this.checkBoxStartWithWindows.TabIndex = 4;
            this.checkBoxStartWithWindows.Text = "Start automatically when I sign in to Windows";
            this.toolTip.SetToolTip(this.checkBoxStartWithWindows, "If enabled this application will start with Windows.  If this is disabled virtual" +
        " machines will not start with the host.");
            this.checkBoxStartWithWindows.UseVisualStyleBackColor = true;
            // 
            // comboBoxLogLevel
            // 
            this.comboBoxLogLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLogLevel.FormattingEnabled = true;
            this.comboBoxLogLevel.Items.AddRange(new object[] {
            "Trace",
            "Debug",
            "Information",
            "Warning",
            "Error",
            "Critical"});
            this.comboBoxLogLevel.Location = new System.Drawing.Point(73, 20);
            this.comboBoxLogLevel.Name = "comboBoxLogLevel";
            this.comboBoxLogLevel.Size = new System.Drawing.Size(97, 21);
            this.comboBoxLogLevel.TabIndex = 9;
            // 
            // checkBoxKeepAwakeMenu
            // 
            this.checkBoxKeepAwakeMenu.AutoSize = true;
            this.checkBoxKeepAwakeMenu.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            this.checkBoxKeepAwakeMenu.Location = new System.Drawing.Point(12, 70);
            this.checkBoxKeepAwakeMenu.Name = "checkBoxKeepAwakeMenu";
            this.checkBoxKeepAwakeMenu.Size = new System.Drawing.Size(210, 17);
            this.checkBoxKeepAwakeMenu.TabIndex = 6;
            this.checkBoxKeepAwakeMenu.Text = "Show keep host awake option in menu";
            this.checkBoxKeepAwakeMenu.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.toolTip.SetToolTip(this.checkBoxKeepAwakeMenu, "If enabled an option to keep the host from going to sleep will be added to the tr" +
        "ay menu.");
            this.checkBoxKeepAwakeMenu.UseVisualStyleBackColor = true;
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(377, 376);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(120, 23);
            this.buttonSave.TabIndex = 10;
            this.buttonSave.Text = "&Save Configuration";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.OnSave);
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(296, 376);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 11;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.OnCancel);
            // 
            // checkBoxTrayIcon
            // 
            this.checkBoxTrayIcon.AutoSize = true;
            this.checkBoxTrayIcon.Location = new System.Drawing.Point(12, 47);
            this.checkBoxTrayIcon.Name = "checkBoxTrayIcon";
            this.checkBoxTrayIcon.Size = new System.Drawing.Size(131, 17);
            this.checkBoxTrayIcon.TabIndex = 5;
            this.checkBoxTrayIcon.Text = "Show system tray icon";
            this.toolTip.SetToolTip(this.checkBoxTrayIcon, "Displays an icon in the system tray with a menu to control the virtual machines. " +
        " If this is disabled open the app a second time to adjust the configuration.");
            this.checkBoxTrayIcon.UseVisualStyleBackColor = true;
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
            this.columnEnabled,
            this.columnName,
            this.columnShowInMenu,
            this.columnAutoStart,
            this.columnAutoStop,
            this.columnSaveState});
            this.dataGridMachines.DoubleBuffered = true;
            this.dataGridMachines.Location = new System.Drawing.Point(6, 6);
            this.dataGridMachines.Name = "dataGridMachines";
            this.dataGridMachines.RowHeadersVisible = false;
            this.dataGridMachines.Size = new System.Drawing.Size(474, 324);
            this.dataGridMachines.TabIndex = 1;
            this.dataGridMachines.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.Machines_CellMouseLeave);
            this.dataGridMachines.CellMouseMove += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Machines_CellMouseMove);
            this.dataGridMachines.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.Machines_CellMouseUp);
            this.dataGridMachines.CellToolTipTextNeeded += new System.Windows.Forms.DataGridViewCellToolTipTextNeededEventHandler(this.Machines_CellToolTipTextNeeded);
            this.dataGridMachines.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.Machines_CellValueChanged);
            this.dataGridMachines.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.Machines_RowPostPaint);
            this.dataGridMachines.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.Machines_RowPrePaint);
            this.dataGridMachines.SelectionChanged += new System.EventHandler(this.Machines_SelectionChanged);
            // 
            // columnEnabled
            // 
            this.columnEnabled.DataPropertyName = "Enabled";
            this.columnEnabled.HeaderText = "Enabled";
            this.columnEnabled.Name = "columnEnabled";
            this.columnEnabled.Width = 60;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.columnName.DataPropertyName = "Name";
            this.columnName.HeaderText = "Name";
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
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
            this.columnAutoStart.Width = 70;
            // 
            // columnAutoStop
            // 
            this.columnAutoStop.DataPropertyName = "AutoStop";
            this.columnAutoStop.HeaderText = "Auto-Stop";
            this.columnAutoStop.Name = "columnAutoStop";
            this.columnAutoStop.Width = 70;
            // 
            // columnSaveState
            // 
            this.columnSaveState.DataPropertyName = "SaveState";
            this.columnSaveState.HeaderText = "Save State";
            this.columnSaveState.Name = "columnSaveState";
            this.columnSaveState.Width = 70;
            // 
            // groupBoxServiceConfiguration
            // 
            this.groupBoxServiceConfiguration.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxTrayIcon);
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxStartWithWindows);
            this.groupBoxServiceConfiguration.Controls.Add(this.checkBoxKeepAwakeMenu);
            this.groupBoxServiceConfiguration.Location = new System.Drawing.Point(6, 6);
            this.groupBoxServiceConfiguration.Name = "groupBoxServiceConfiguration";
            this.groupBoxServiceConfiguration.Size = new System.Drawing.Size(461, 98);
            this.groupBoxServiceConfiguration.TabIndex = 3;
            this.groupBoxServiceConfiguration.TabStop = false;
            this.groupBoxServiceConfiguration.Text = "Configuration";
            // 
            // tabControlMachines
            // 
            this.tabControlMachines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControlMachines.Controls.Add(this.tabPageMachines);
            this.tabControlMachines.Controls.Add(this.tabPageSettings);
            this.tabControlMachines.Location = new System.Drawing.Point(8, 8);
            this.tabControlMachines.Name = "tabControlMachines";
            this.tabControlMachines.SelectedIndex = 0;
            this.tabControlMachines.Size = new System.Drawing.Size(494, 362);
            this.tabControlMachines.TabIndex = 10;
            // 
            // tabPageMachines
            // 
            this.tabPageMachines.Controls.Add(this.dataGridMachines);
            this.tabPageMachines.Location = new System.Drawing.Point(4, 22);
            this.tabPageMachines.Name = "tabPageMachines";
            this.tabPageMachines.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMachines.Size = new System.Drawing.Size(486, 336);
            this.tabPageMachines.TabIndex = 0;
            this.tabPageMachines.Text = "Virtual Machines";
            this.tabPageMachines.UseVisualStyleBackColor = true;
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.groupBoxLogging);
            this.tabPageSettings.Controls.Add(this.groupBoxServiceConfiguration);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(486, 336);
            this.tabPageSettings.TabIndex = 1;
            this.tabPageSettings.Text = "Settings";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // groupBoxLogging
            // 
            this.groupBoxLogging.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxLogging.Controls.Add(this.labelLogLevel);
            this.groupBoxLogging.Controls.Add(this.comboBoxLogLevel);
            this.groupBoxLogging.Location = new System.Drawing.Point(7, 110);
            this.groupBoxLogging.Name = "groupBoxLogging";
            this.groupBoxLogging.Size = new System.Drawing.Size(460, 54);
            this.groupBoxLogging.TabIndex = 7;
            this.groupBoxLogging.TabStop = false;
            this.groupBoxLogging.Text = "Logging";
            // 
            // buttonExit
            // 
            this.buttonExit.Location = new System.Drawing.Point(13, 375);
            this.buttonExit.Name = "buttonExit";
            this.buttonExit.Size = new System.Drawing.Size(125, 23);
            this.buttonExit.TabIndex = 12;
            this.buttonExit.Text = "Stop monitoring && E&xit";
            this.buttonExit.UseVisualStyleBackColor = true;
            this.buttonExit.Click += new System.EventHandler(this.OnExit);
            // 
            // ConfigureForm
            // 
            this.AcceptButton = this.buttonSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(509, 411);
            this.Controls.Add(this.buttonExit);
            this.Controls.Add(this.tabControlMachines);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonSave);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(525, 450);
            this.Name = "ConfigureForm";
            this.Text = "Configure VBox Headless AutoStart";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridMachines)).EndInit();
            this.groupBoxServiceConfiguration.ResumeLayout(false);
            this.groupBoxServiceConfiguration.PerformLayout();
            this.tabControlMachines.ResumeLayout(false);
            this.tabPageMachines.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.groupBoxLogging.ResumeLayout(false);
            this.groupBoxLogging.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelLogLevel;
        private System.Windows.Forms.ComboBox comboBoxLogLevel;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.CheckBox checkBoxKeepAwakeMenu;
        private System.Windows.Forms.CheckBox checkBoxStartWithWindows;
        private System.Windows.Forms.ToolTip toolTip;
        private DoubleBufferedDataGridView dataGridMachines;
        private System.Windows.Forms.GroupBox groupBoxServiceConfiguration;
        private System.Windows.Forms.TabControl tabControlMachines;
        private System.Windows.Forms.TabPage tabPageMachines;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.GroupBox groupBoxLogging;
        private DataGridViewCheckBoxColumn columnEnabled;
        private DataGridViewTextBoxColumn columnName;
        private DataGridViewCheckBoxColumn columnShowInMenu;
        private DataGridViewCheckBoxColumn columnAutoStart;
        private DataGridViewCheckBoxColumn columnAutoStop;
        private DataGridViewCheckBoxColumn columnSaveState;
        private System.Windows.Forms.CheckBox checkBoxTrayIcon;
        private System.Windows.Forms.Button buttonExit;
    }
}