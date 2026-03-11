namespace dekit2
{
    partial class frmMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControlContainer = new TabControl();
            tabPageStart = new TabPage();
            tabPagePrograms = new TabPage();
            dataGridViewPrograms = new DataGridView();
            colNo = new DataGridViewTextBoxColumn();
            colProgram = new DataGridViewTextBoxColumn();
            colVersion = new DataGridViewTextBoxColumn();
            colSelect = new DataGridViewComboBoxColumn();
            colInstall = new DataGridViewCheckBoxColumn();
            dataGridView1 = new DataGridView();
            dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
            dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
            dataGridViewComboBoxColumn1 = new DataGridViewComboBoxColumn();
            dataGridViewCheckBoxColumn1 = new DataGridViewCheckBoxColumn();
            tabControlContainer.SuspendLayout();
            tabPageStart.SuspendLayout();
            tabPagePrograms.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).BeginInit();
            SuspendLayout();
            // 
            // tabControlContainer
            // 
            tabControlContainer.Alignment = TabAlignment.Bottom;
            tabControlContainer.Controls.Add(tabPageStart);
            tabControlContainer.Controls.Add(tabPagePrograms);
            tabControlContainer.Dock = DockStyle.Fill;
            tabControlContainer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabControlContainer.Location = new Point(0, 0);
            tabControlContainer.Multiline = true;
            tabControlContainer.Name = "tabControlContainer";
            tabControlContainer.SelectedIndex = 0;
            tabControlContainer.Size = new Size(843, 497);
            tabControlContainer.TabIndex = 0;
            // 
            // tabPageStart
            // 
            tabPageStart.Controls.Add(dataGridView1);
            tabPageStart.Location = new Point(4, 4);
            tabPageStart.Name = "tabPageStart";
            tabPageStart.Padding = new Padding(3);
            tabPageStart.Size = new Size(835, 463);
            tabPageStart.TabIndex = 0;
            tabPageStart.Text = "Start";
            tabPageStart.UseVisualStyleBackColor = true;
            // 
            // tabPagePrograms
            // 
            tabPagePrograms.Controls.Add(dataGridViewPrograms);
            tabPagePrograms.Location = new Point(4, 4);
            tabPagePrograms.Name = "tabPagePrograms";
            tabPagePrograms.Padding = new Padding(3);
            tabPagePrograms.Size = new Size(835, 463);
            tabPagePrograms.TabIndex = 1;
            tabPagePrograms.Text = "Programs";
            tabPagePrograms.UseVisualStyleBackColor = true;
            // 
            // dataGridViewPrograms
            // 
            dataGridViewPrograms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPrograms.Columns.AddRange(new DataGridViewColumn[] { colNo, colProgram, colVersion, colSelect, colInstall });
            dataGridViewPrograms.Dock = DockStyle.Fill;
            dataGridViewPrograms.Location = new Point(3, 3);
            dataGridViewPrograms.Name = "dataGridViewPrograms";
            dataGridViewPrograms.Size = new Size(829, 457);
            dataGridViewPrograms.TabIndex = 0;
            dataGridViewPrograms.CellClick += dataGridViewPrograms_CellClick;
            dataGridViewPrograms.CellValueChanged += dataGridViewPrograms_CellValueChanged;
            dataGridViewPrograms.CurrentCellDirtyStateChanged += dataGridViewPrograms_CurrentCellDirtyStateChanged;
            // 
            // colNo
            // 
            colNo.HeaderText = "No.";
            colNo.Name = "colNo";
            colNo.ReadOnly = true;
            colNo.Resizable = DataGridViewTriState.False;
            colNo.Width = 50;
            // 
            // colProgram
            // 
            colProgram.HeaderText = "Program";
            colProgram.Name = "colProgram";
            colProgram.ReadOnly = true;
            colProgram.Width = 350;
            // 
            // colVersion
            // 
            colVersion.HeaderText = "Version";
            colVersion.Name = "colVersion";
            colVersion.ReadOnly = true;
            // 
            // colSelect
            // 
            colSelect.HeaderText = "Select";
            colSelect.Name = "colSelect";
            colSelect.Width = 120;
            // 
            // colInstall
            // 
            colInstall.HeaderText = "Install";
            colInstall.Name = "colInstall";
            colInstall.Resizable = DataGridViewTriState.False;
            colInstall.Width = 60;
            // 
            // dataGridView1
            // 
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView1.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewComboBoxColumn1, dataGridViewCheckBoxColumn1 });
            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.Location = new Point(3, 3);
            dataGridView1.Name = "dataGridView1";
            dataGridView1.Size = new Size(829, 457);
            dataGridView1.TabIndex = 1;
            // 
            // dataGridViewTextBoxColumn1
            // 
            dataGridViewTextBoxColumn1.HeaderText = "No.";
            dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            dataGridViewTextBoxColumn1.ReadOnly = true;
            dataGridViewTextBoxColumn1.Resizable = DataGridViewTriState.False;
            dataGridViewTextBoxColumn1.Width = 50;
            // 
            // dataGridViewTextBoxColumn2
            // 
            dataGridViewTextBoxColumn2.HeaderText = "Program";
            dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            dataGridViewTextBoxColumn2.ReadOnly = true;
            dataGridViewTextBoxColumn2.Width = 350;
            // 
            // dataGridViewTextBoxColumn3
            // 
            dataGridViewTextBoxColumn3.HeaderText = "Version";
            dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewComboBoxColumn1
            // 
            dataGridViewComboBoxColumn1.HeaderText = "Select";
            dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
            dataGridViewComboBoxColumn1.Width = 120;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            dataGridViewCheckBoxColumn1.HeaderText = "Install";
            dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            dataGridViewCheckBoxColumn1.Resizable = DataGridViewTriState.False;
            dataGridViewCheckBoxColumn1.Width = 60;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(843, 497);
            Controls.Add(tabControlContainer);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Minh Nguyen DevKit2";
            Load += frmMain_Load;
            tabControlContainer.ResumeLayout(false);
            tabPageStart.ResumeLayout(false);
            tabPagePrograms.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).EndInit();
            ((System.ComponentModel.ISupportInitialize)dataGridView1).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlContainer;
        private TabPage tabPageStart;
        private TabPage tabPagePrograms;
        private DataGridView dataGridViewPrograms;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colProgram;
        private DataGridViewTextBoxColumn colVersion;
        private DataGridViewComboBoxColumn colSelect;
        private DataGridViewCheckBoxColumn colInstall;
        private DataGridView dataGridView1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
        private DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
    }
}
