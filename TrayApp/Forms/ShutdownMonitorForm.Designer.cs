namespace TrayApp.Forms
{
    partial class ShutdownMonitorForm
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
            this.labelReasonForExisting = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelReasonForExisting
            // 
            this.labelReasonForExisting.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelReasonForExisting.Location = new System.Drawing.Point(0, 0);
            this.labelReasonForExisting.Name = "labelReasonForExisting";
            this.labelReasonForExisting.Size = new System.Drawing.Size(244, 61);
            this.labelReasonForExisting.TabIndex = 0;
            this.labelReasonForExisting.Text = "This form exists to listen for shutdown events.";
            this.labelReasonForExisting.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ShutdownMonitorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 61);
            this.ControlBox = false;
            this.Controls.Add(this.labelReasonForExisting);
            this.Icon = Properties.Resources.TrayIcon;
            this.Name = "ShutdownMonitorForm";
            this.ShowInTaskbar = false;
            this.Text = "VBox Headless AutoStart Shutdown Monitor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label labelReasonForExisting;
    }
}