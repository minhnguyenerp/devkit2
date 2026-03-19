namespace devkit2.Applications
{
    partial class NginxProfile
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
            tableLayoutPanel1 = new TableLayoutPanel();
            tableLayoutPanel4 = new TableLayoutPanel();
            txtWebRootDirectory = new TextBox();
            btnBrowseWebRootDirectory = new Button();
            label2 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            txtInstanceDirectory = new TextBox();
            btnBrowseInstanceDirectory = new Button();
            label1 = new Label();
            lblPort = new Label();
            txtPort = new TextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnOK = new Button();
            btnCancel = new Button();
            btnClear = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 159F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 1, 1);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(lblPort, 0, 2);
            tableLayoutPanel1.Controls.Add(txtPort, 1, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 4);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(648, 169);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 77F));
            tableLayoutPanel4.Controls.Add(txtWebRootDirectory, 0, 0);
            tableLayoutPanel4.Controls.Add(btnBrowseWebRootDirectory, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(159, 39);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel4.Size = new Size(489, 39);
            tableLayoutPanel4.TabIndex = 16;
            // 
            // txtWebRootDirectory
            // 
            txtWebRootDirectory.BackColor = Color.White;
            txtWebRootDirectory.Dock = DockStyle.Fill;
            txtWebRootDirectory.Location = new Point(5, 6);
            txtWebRootDirectory.Margin = new Padding(5, 6, 5, 6);
            txtWebRootDirectory.Name = "txtWebRootDirectory";
            txtWebRootDirectory.Size = new Size(402, 29);
            txtWebRootDirectory.TabIndex = 0;
            // 
            // btnBrowseWebRootDirectory
            // 
            btnBrowseWebRootDirectory.Dock = DockStyle.Fill;
            btnBrowseWebRootDirectory.Location = new Point(416, 4);
            btnBrowseWebRootDirectory.Margin = new Padding(4);
            btnBrowseWebRootDirectory.Name = "btnBrowseWebRootDirectory";
            btnBrowseWebRootDirectory.Size = new Size(69, 31);
            btnBrowseWebRootDirectory.TabIndex = 1;
            btnBrowseWebRootDirectory.Text = "...";
            btnBrowseWebRootDirectory.UseVisualStyleBackColor = true;
            btnBrowseWebRootDirectory.Click += btnBrowseWebRootDirectory_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(5, 39);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(149, 39);
            label2.TabIndex = 15;
            label2.Text = "Web Root Directory";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 77F));
            tableLayoutPanel2.Controls.Add(txtInstanceDirectory, 0, 0);
            tableLayoutPanel2.Controls.Add(btnBrowseInstanceDirectory, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(159, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.Size = new Size(489, 39);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // txtInstanceDirectory
            // 
            txtInstanceDirectory.BackColor = Color.White;
            txtInstanceDirectory.Dock = DockStyle.Fill;
            txtInstanceDirectory.Location = new Point(5, 6);
            txtInstanceDirectory.Margin = new Padding(5, 6, 5, 6);
            txtInstanceDirectory.Name = "txtInstanceDirectory";
            txtInstanceDirectory.Size = new Size(402, 29);
            txtInstanceDirectory.TabIndex = 0;
            // 
            // btnBrowseInstanceDirectory
            // 
            btnBrowseInstanceDirectory.Dock = DockStyle.Fill;
            btnBrowseInstanceDirectory.Location = new Point(416, 4);
            btnBrowseInstanceDirectory.Margin = new Padding(4);
            btnBrowseInstanceDirectory.Name = "btnBrowseInstanceDirectory";
            btnBrowseInstanceDirectory.Size = new Size(69, 31);
            btnBrowseInstanceDirectory.TabIndex = 1;
            btnBrowseInstanceDirectory.Text = "...";
            btnBrowseInstanceDirectory.UseVisualStyleBackColor = true;
            btnBrowseInstanceDirectory.Click += btnBrowseInstanceDirectory_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(5, 0);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(149, 39);
            label1.TabIndex = 11;
            label1.Text = "Instance Directory";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Dock = DockStyle.Fill;
            lblPort.Location = new Point(3, 78);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(153, 39);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port";
            lblPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtPort
            // 
            txtPort.Dock = DockStyle.Fill;
            txtPort.Location = new Point(162, 81);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(483, 29);
            txtPort.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 3;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
            tableLayoutPanel3.Controls.Add(btnOK, 0, 0);
            tableLayoutPanel3.Controls.Add(btnCancel, 2, 0);
            tableLayoutPanel3.Controls.Add(btnClear, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Right;
            tableLayoutPanel3.Location = new Point(317, 122);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.Size = new Size(328, 44);
            tableLayoutPanel3.TabIndex = 10;
            // 
            // btnOK
            // 
            btnOK.Dock = DockStyle.Fill;
            btnOK.Location = new Point(3, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(103, 38);
            btnOK.TabIndex = 5;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Location = new Point(221, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(104, 38);
            btnCancel.TabIndex = 6;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // btnClear
            // 
            btnClear.Dock = DockStyle.Fill;
            btnClear.Location = new Point(112, 3);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(103, 38);
            btnClear.TabIndex = 7;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += btnClear_Click;
            // 
            // NginxProfile
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 169);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "NginxProfile";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Nginx Profile";
            Load += NginxProfile_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
            tableLayoutPanel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TextBox txtInstanceDirectory;
        private Button btnBrowseInstanceDirectory;
        private Label label1;
        private TextBox txtPort;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnOK;
        private Button btnCancel;
        private Button btnClear;
        private Label lblPort;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel4;
        private TextBox txtWebRootDirectory;
        private Button btnBrowseWebRootDirectory;
    }
}