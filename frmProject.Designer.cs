namespace devkit2
{
    partial class frmProject
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
            txtProjectName = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            tableLayoutPanel2 = new TableLayoutPanel();
            comboBoxProgram = new ComboBox();
            comboBoxVersion = new ComboBox();
            btnProfile = new Button();
            dataGridView1 = new DataGridView();
            colProgram = new DataGridViewTextBoxColumn();
            colVersion = new DataGridViewComboBoxColumn();
            colProfile = new DataGridViewTextBoxColumn();
            tableLayoutPanel3 = new TableLayoutPanel();
            btnOK = new Button();
            btnCancel = new Button();
            label4 = new Label();
            tableLayoutPanel4 = new TableLayoutPanel();
            txtProjectLocation = new TextBox();
            btnBrowse = new Button();
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel4.SuspendLayout();
            SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 127F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(txtProjectName, 1, 0);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 1, 1);
            tableLayoutPanel1.Controls.Add(dataGridView1, 1, 2);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel3, 1, 4);
            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(tableLayoutPanel4, 1, 3);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 0);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 5;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 50F));
            tableLayoutPanel1.Size = new Size(747, 411);
            tableLayoutPanel1.TabIndex = 0;
            // 
            // txtProjectName
            // 
            txtProjectName.Dock = DockStyle.Fill;
            txtProjectName.Location = new Point(131, 4);
            txtProjectName.Margin = new Padding(4);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.Size = new Size(612, 29);
            txtProjectName.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Dock = DockStyle.Fill;
            label1.Location = new Point(4, 0);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(119, 35);
            label1.TabIndex = 1;
            label1.Text = "Project Name";
            label1.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Dock = DockStyle.Fill;
            label2.Location = new Point(4, 35);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(119, 35);
            label2.TabIndex = 1;
            label2.Text = "Application";
            label2.TextAlign = ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Dock = DockStyle.Fill;
            label3.Location = new Point(4, 70);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(119, 256);
            label3.TabIndex = 1;
            label3.Text = "Environments";
            label3.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.ColumnCount = 3;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65.5677643F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34.43224F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));
            tableLayoutPanel2.Controls.Add(comboBoxProgram, 0, 0);
            tableLayoutPanel2.Controls.Add(comboBoxVersion, 1, 0);
            tableLayoutPanel2.Controls.Add(btnProfile, 2, 0);
            tableLayoutPanel2.Dock = DockStyle.Fill;
            tableLayoutPanel2.Location = new Point(127, 35);
            tableLayoutPanel2.Margin = new Padding(0);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.Size = new Size(620, 35);
            tableLayoutPanel2.TabIndex = 4;
            // 
            // comboBoxProgram
            // 
            comboBoxProgram.Dock = DockStyle.Fill;
            comboBoxProgram.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxProgram.FormattingEnabled = true;
            comboBoxProgram.Location = new Point(3, 3);
            comboBoxProgram.Name = "comboBoxProgram";
            comboBoxProgram.Size = new Size(367, 29);
            comboBoxProgram.TabIndex = 3;
            comboBoxProgram.SelectedIndexChanged += comboBoxProgram_SelectedIndexChanged;
            // 
            // comboBoxVersion
            // 
            comboBoxVersion.Dock = DockStyle.Fill;
            comboBoxVersion.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxVersion.FormattingEnabled = true;
            comboBoxVersion.Location = new Point(376, 3);
            comboBoxVersion.Name = "comboBoxVersion";
            comboBoxVersion.Size = new Size(190, 29);
            comboBoxVersion.TabIndex = 3;
            // 
            // btnProfile
            // 
            btnProfile.Dock = DockStyle.Fill;
            btnProfile.Location = new Point(572, 3);
            btnProfile.Name = "btnProfile";
            btnProfile.Size = new Size(45, 29);
            btnProfile.TabIndex = 4;
            btnProfile.Text = "...";
            btnProfile.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { colProgram, colVersion, colProfile });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(130, 73);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(614, 250);
            dataGridView1.TabIndex = 6;
            dataGridView1.CellClick += dataGridView1_CellClick;
            dataGridView1.CurrentCellDirtyStateChanged += dataGridView1_CurrentCellDirtyStateChanged;
            // 
            // colProgram
            // 
            colProgram.HeaderText = "Program";
            colProgram.Name = "colProgram";
            colProgram.ReadOnly = true;
            colProgram.Width = 200;
            // 
            // colVersion
            // 
            colVersion.HeaderText = "Version";
            colVersion.Name = "colVersion";
            colVersion.Width = 130;
            // 
            // colProfile
            // 
            colProfile.HeaderText = "Profile";
            colProfile.Name = "colProfile";
            colProfile.Width = 220;
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableLayoutPanel3.Controls.Add(btnOK, 0, 0);
            tableLayoutPanel3.Controls.Add(btnCancel, 1, 0);
            tableLayoutPanel3.Dock = DockStyle.Right;
            tableLayoutPanel3.Location = new Point(503, 364);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Size = new Size(241, 44);
            tableLayoutPanel3.TabIndex = 5;
            // 
            // btnOK
            // 
            btnOK.Dock = DockStyle.Fill;
            btnOK.Location = new Point(3, 3);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(114, 38);
            btnOK.TabIndex = 0;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Location = new Point(123, 3);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(115, 38);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Dock = DockStyle.Fill;
            label4.Location = new Point(4, 326);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(119, 35);
            label4.TabIndex = 1;
            label4.Text = "Location";
            label4.TextAlign = ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel4
            // 
            tableLayoutPanel4.ColumnCount = 2;
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 60F));
            tableLayoutPanel4.Controls.Add(txtProjectLocation, 0, 0);
            tableLayoutPanel4.Controls.Add(btnBrowse, 1, 0);
            tableLayoutPanel4.Dock = DockStyle.Fill;
            tableLayoutPanel4.Location = new Point(127, 326);
            tableLayoutPanel4.Margin = new Padding(0);
            tableLayoutPanel4.Name = "tableLayoutPanel4";
            tableLayoutPanel4.RowCount = 1;
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel4.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel4.Size = new Size(620, 35);
            tableLayoutPanel4.TabIndex = 7;
            // 
            // txtProjectLocation
            // 
            txtProjectLocation.BackColor = Color.White;
            txtProjectLocation.Dock = DockStyle.Fill;
            txtProjectLocation.Location = new Point(4, 4);
            txtProjectLocation.Margin = new Padding(4);
            txtProjectLocation.Name = "txtProjectLocation";
            txtProjectLocation.ReadOnly = true;
            txtProjectLocation.Size = new Size(552, 29);
            txtProjectLocation.TabIndex = 0;
            // 
            // btnBrowse
            // 
            btnBrowse.Dock = DockStyle.Fill;
            btnBrowse.Location = new Point(563, 3);
            btnBrowse.Name = "btnBrowse";
            btnBrowse.Size = new Size(54, 29);
            btnBrowse.TabIndex = 1;
            btnBrowse.Text = "...";
            btnBrowse.UseVisualStyleBackColor = true;
            btnBrowse.Click += btnBrowse_Click;
            // 
            // frmProject
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(747, 411);
            Controls.Add(tableLayoutPanel1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "frmProject";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Project";
            Load += frmProject_Load;
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel4.ResumeLayout(false);
            tableLayoutPanel4.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private TextBox txtProjectName;
        private Label label1;
        private Label label2;
        private Label label3;
        private TableLayoutPanel tableLayoutPanel2;
        private ComboBox comboBoxProgram;
        private ComboBox comboBoxVersion;
        private TableLayoutPanel tableLayoutPanel3;
        private Button btnOK;
        private Button btnCancel;
        private DataGridView dataGridView1;
        private Label label4;
        private TableLayoutPanel tableLayoutPanel4;
        private TextBox txtProjectLocation;
        private Button btnBrowse;
        private DataGridViewTextBoxColumn colProgram;
        private DataGridViewComboBoxColumn colVersion;
        private DataGridViewTextBoxColumn colProfile;
        private Button btnProfile;
    }
}