namespace devkit2
{
    partial class frmPrograms
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
            dataGridViewPrograms = new DataGridView();
            colNo = new DataGridViewTextBoxColumn();
            colProgram = new DataGridViewTextBoxColumn();
            colVersion = new DataGridViewTextBoxColumn();
            colInstalled = new DataGridViewCheckBoxColumn();
            colSelect = new DataGridViewComboBoxColumn();
            colAction = new DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewPrograms
            // 
            dataGridViewPrograms.BorderStyle = BorderStyle.None;
            dataGridViewPrograms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPrograms.Columns.AddRange(new DataGridViewColumn[] { colNo, colProgram, colVersion, colInstalled, colSelect, colAction });
            dataGridViewPrograms.Dock = DockStyle.Fill;
            dataGridViewPrograms.Location = new Point(0, 0);
            dataGridViewPrograms.Name = "dataGridViewPrograms";
            dataGridViewPrograms.Size = new Size(839, 317);
            dataGridViewPrograms.TabIndex = 2;
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
            // colInstalled
            // 
            colInstalled.HeaderText = "Installed";
            colInstalled.Name = "colInstalled";
            colInstalled.ReadOnly = true;
            colInstalled.Resizable = DataGridViewTriState.False;
            colInstalled.Width = 75;
            // 
            // colSelect
            // 
            colSelect.HeaderText = "Select";
            colSelect.Name = "colSelect";
            colSelect.Width = 120;
            // 
            // colAction
            // 
            colAction.HeaderText = "Action";
            colAction.Name = "colAction";
            colAction.ReadOnly = true;
            colAction.Resizable = DataGridViewTriState.False;
            colAction.Width = 80;
            // 
            // frmPrograms
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(839, 317);
            Controls.Add(dataGridViewPrograms);
            Name = "frmPrograms";
            Text = "frmPrograms";
            Load += frmPrograms_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewPrograms;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colProgram;
        private DataGridViewTextBoxColumn colVersion;
        private DataGridViewCheckBoxColumn colInstalled;
        private DataGridViewComboBoxColumn colSelect;
        private DataGridViewButtonColumn colAction;
    }
}