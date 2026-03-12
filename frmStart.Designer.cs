namespace devkit2
{
    partial class frmStart
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
            colWorkingDirectory = new DataGridViewTextBoxColumn();
            colSelect = new DataGridViewComboBoxColumn();
            colEnv = new DataGridViewCheckBoxColumn();
            colStart = new DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewPrograms
            // 
            dataGridViewPrograms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPrograms.Columns.AddRange(new DataGridViewColumn[] { colNo, colProgram, colWorkingDirectory, colSelect, colEnv, colStart });
            dataGridViewPrograms.Dock = DockStyle.Fill;
            dataGridViewPrograms.Location = new Point(0, 0);
            dataGridViewPrograms.Name = "dataGridViewPrograms";
            dataGridViewPrograms.Size = new Size(869, 450);
            dataGridViewPrograms.TabIndex = 3;
            dataGridViewPrograms.CellClick += dataGridViewPrograms_CellClick;
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
            colProgram.Width = 120;
            // 
            // colWorkingDirectory
            // 
            colWorkingDirectory.HeaderText = "Working Directory";
            colWorkingDirectory.Name = "colWorkingDirectory";
            colWorkingDirectory.ReadOnly = true;
            colWorkingDirectory.Width = 350;
            // 
            // colSelect
            // 
            colSelect.HeaderText = "Select";
            colSelect.Name = "colSelect";
            colSelect.Width = 120;
            // 
            // colEnv
            // 
            colEnv.HeaderText = "Env";
            colEnv.Name = "colEnv";
            colEnv.Resizable = DataGridViewTriState.False;
            colEnv.Width = 60;
            // 
            // colStart
            // 
            colStart.HeaderText = "Start";
            colStart.Name = "colStart";
            colStart.ReadOnly = true;
            colStart.Resizable = DataGridViewTriState.True;
            colStart.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // frmStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(869, 450);
            Controls.Add(dataGridViewPrograms);
            Name = "frmStart";
            Text = "frmStart";
            Load += frmStart_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewPrograms;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colProgram;
        private DataGridViewTextBoxColumn colWorkingDirectory;
        private DataGridViewComboBoxColumn colSelect;
        private DataGridViewCheckBoxColumn colEnv;
        private DataGridViewButtonColumn colStart;
    }
}