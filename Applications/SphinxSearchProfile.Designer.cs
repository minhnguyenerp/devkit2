namespace devkit2.Applications
{
    partial class SphinxSearchProfile
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
            txtDataDirectory = new TextBox();
            btnBrowseDataDirectory = new Button();
            tableLayoutPanel2 = new TableLayoutPanel();
            txtConfigDirectory = new TextBox();
            btnBrowseConfigDirectory = new Button();
            label1 = new Label();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnOK = new Button();
            btnCancel = new Button();
            btnClear = new Button();
            txtPort = new TextBox();
            label2 = new Label();
            lblPort = new Label();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel3.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 147F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 1, 1);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 4);
            tableLayoutPanel1.Controls.Add(txtPort, 1, 2);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(lblPort, 0, 2);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(648, 169);
            tableLayoutPanel1.TabIndex = 30;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 77F));
            tableLayoutPanel4.Controls.Add(txtDataDirectory, 0, 0);
            tableLayoutPanel4.Controls.Add(btnBrowseDataDirectory, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(147, 35);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel4.Size = new Size(501, 35);
            tableLayoutPanel4.TabIndex = 16;
            // 
            // txtDataDirectory
            // 
            txtDataDirectory.BackColor = Color.White;
            txtDataDirectory.Dock = DockStyle.Fill;
            txtDataDirectory.Location = new Point(3, 3);
            txtDataDirectory.Name = "txtDataDirectory";
            txtDataDirectory.Size = new Size(418, 29);
            txtDataDirectory.TabIndex = 2;
            // 
            // btnBrowseDataDirectory
            // 
            btnBrowseDataDirectory.Dock = DockStyle.Fill;
            btnBrowseDataDirectory.Location = new Point(427, 3);
            btnBrowseDataDirectory.Name = "btnBrowseDataDirectory";
            btnBrowseDataDirectory.Size = new Size(71, 29);
            btnBrowseDataDirectory.TabIndex = 3;
            btnBrowseDataDirectory.Text = "...";
            btnBrowseDataDirectory.UseVisualStyleBackColor = true;
            btnBrowseDataDirectory.Click += btnBrowseDataDirectory_Click;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 77F));
            tableLayoutPanel2.Controls.Add(txtConfigDirectory, 0, 0);
            tableLayoutPanel2.Controls.Add(btnBrowseConfigDirectory, 1, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(147, 0);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel2.Size = new Size(501, 35);
            tableLayoutPanel2.TabIndex = 12;
            // 
            // txtConfigDirectory
            // 
            txtConfigDirectory.BackColor = Color.White;
            txtConfigDirectory.Dock = DockStyle.Fill;
            txtConfigDirectory.Location = new Point(3, 3);
            txtConfigDirectory.Name = "txtConfigDirectory";
            txtConfigDirectory.Size = new Size(418, 29);
            txtConfigDirectory.TabIndex = 0;
            // 
            // btnBrowseConfigDirectory
            // 
            btnBrowseConfigDirectory.Dock = DockStyle.Fill;
            btnBrowseConfigDirectory.Location = new Point(427, 3);
            btnBrowseConfigDirectory.Name = "btnBrowseConfigDirectory";
            btnBrowseConfigDirectory.Size = new Size(71, 29);
            btnBrowseConfigDirectory.TabIndex = 1;
            btnBrowseConfigDirectory.Text = "...";
            btnBrowseConfigDirectory.UseVisualStyleBackColor = true;
            btnBrowseConfigDirectory.Click += btnBrowseConfigDirectory_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(5, 0);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(137, 35);
            label1.TabIndex = 11;
            label1.Text = "Config Directory";
            label1.TextAlign = ContentAlignment.MiddleRight;
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
            // txtPort
            // 
            txtPort.Dock = DockStyle.Fill;
            txtPort.Location = new Point(150, 73);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(495, 29);
            txtPort.TabIndex = 4;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(5, 35);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(137, 35);
            label2.TabIndex = 15;
            label2.Text = "Data Directory";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // lblPort
            // 
            lblPort.AutoSize = true;
            lblPort.Dock = DockStyle.Fill;
            lblPort.Location = new Point(3, 70);
            lblPort.Name = "lblPort";
            lblPort.Size = new Size(141, 35);
            lblPort.TabIndex = 3;
            lblPort.Text = "Port";
            lblPort.TextAlign = ContentAlignment.MiddleRight;
            // 
            // SphinxSearchProfile
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 169);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "SphinxSearchProfile";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Apache Profile";
            Load += ApacheProfile_Load;
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
        private TextBox txtConfigDirectory;
        private Button btnBrowseConfigDirectory;
        private Label label1;
        private TextBox txtPort;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnOK;
        private Button btnCancel;
        private Button btnClear;
        private Label lblPort;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel4;
        private TextBox txtDataDirectory;
        private Button btnBrowseDataDirectory;
    }
}