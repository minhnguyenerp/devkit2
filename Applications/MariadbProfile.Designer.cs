namespace devkit2.Applications
{
    partial class MariadbProfile
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
            this.lblDataDir = new System.Windows.Forms.Label();
            this.txtDataDir = new System.Windows.Forms.TextBox();
            this.btnWorkingDirectoryBrowse = new System.Windows.Forms.Button();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDataDir
            // 
            this.lblDataDir.AutoSize = true;
            this.lblDataDir.Location = new System.Drawing.Point(12, 15);
            this.lblDataDir.Name = "lblDataDir";
            this.lblDataDir.Size = new System.Drawing.Size(57, 15);
            this.lblDataDir.TabIndex = 0;
            this.lblDataDir.Text = "Data Dir:";
            // 
            // txtDataDir
            // 
            this.txtDataDir.Location = new System.Drawing.Point(75, 12);
            this.txtDataDir.Name = "txtDataDir";
            this.txtDataDir.Size = new System.Drawing.Size(280, 23);
            this.txtDataDir.TabIndex = 1;
            // 
            // btnWorkingDirectoryBrowse
            // 
            this.btnWorkingDirectoryBrowse.Location = new System.Drawing.Point(361, 11);
            this.btnWorkingDirectoryBrowse.Name = "btnWorkingDirectoryBrowse";
            this.btnWorkingDirectoryBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnWorkingDirectoryBrowse.TabIndex = 2;
            this.btnWorkingDirectoryBrowse.Text = "Browse";
            this.btnWorkingDirectoryBrowse.UseVisualStyleBackColor = true;
            this.btnWorkingDirectoryBrowse.Click += new System.EventHandler(this.btnWorkingDirectoryBrowse_Click);
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(12, 50);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(33, 15);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port:";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(75, 47);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(100, 23);
            this.txtPort.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(280, 82);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(361, 82);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(199, 82);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 7;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // MariadbProfile
            // 
            this.ClientSize = new System.Drawing.Size(448, 117);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.btnWorkingDirectoryBrowse);
            this.Controls.Add(this.txtDataDir);
            this.Controls.Add(this.lblDataDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MariadbProfile";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "MariaDB Profile";
            this.Load += new System.EventHandler(this.MariadbProfile_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDataDir;
        private System.Windows.Forms.TextBox txtDataDir;
        private System.Windows.Forms.Button btnWorkingDirectoryBrowse;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClear;
    }
}
