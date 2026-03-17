namespace devkit2.Applications
{
    partial class CaddyProfile
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
            tableLayoutPanel2 = new TableLayoutPanel();
            txtInstanceDirectory = new TextBox();
            btnBrowseInstanceDirectory = new Button();
            label1 = new Label();
            txtPort = new TextBox();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnOK = new Button();
            btnCancel = new Button();
            btnClear = new Button();
            lblPort = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 151F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(txtPort, 1, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 3);
            tableLayoutPanel1.Controls.Add(lblPort, 0, 1);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 39F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel1.Size = new Size(648, 132);
            tableLayoutPanel1.TabIndex = 11;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 77F));
            tableLayoutPanel2.Controls.Add(txtInstanceDirectory, 0, 0);
            tableLayoutPanel2.Controls.Add(btnBrowseInstanceDirectory, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(151, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.Size = new Size(497, 39);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // txtInstanceDirectory
            // 
            txtInstanceDirectory.BackColor = Color.White;
            txtInstanceDirectory.Dock = DockStyle.Fill;
            txtInstanceDirectory.Location = new Point(5, 6);
            txtInstanceDirectory.Margin = new Padding(5, 6, 5, 6);
            txtInstanceDirectory.Name = "txtInstanceDirectory";
            txtInstanceDirectory.Size = new Size(410, 29);
            txtInstanceDirectory.TabIndex = 0;
            // 
            // btnBrowseInstanceDirectory
            // 
            btnBrowseInstanceDirectory.Dock = DockStyle.Fill;
            btnBrowseInstanceDirectory.Location = new Point(424, 4);
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
            label1.Size = new Size(141, 39);
            label1.TabIndex = 11;
            label1.Text = "Instance Directory";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // txtPort
            // 
            txtPort.Dock = DockStyle.Fill;
            txtPort.Location = new Point(154, 42);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(491, 29);
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
            tableLayoutPanel3.Location = new Point(317, 85);
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
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Dock = DockStyle.Fill;
            lblPort.Location = new Point(3, 39);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(145, 39);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port";
            lblPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // CaddyProfile
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 132);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "CaddyProfile";
            StartPosition = FormStartPosition.CenterParent;
            Text = "CaddyProfile";
            Load += CaddyProfile_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
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
    }
}