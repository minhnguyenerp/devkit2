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
            colEnvironment = new DataGridViewComboBoxColumn();
            colProfile = new DataGridViewTextBoxColumn();
            colStart = new DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).BeginInit();
            SuspendLayout();
            // 
            // dataGridViewPrograms
            // 
            dataGridViewPrograms.BorderStyle = BorderStyle.None;
            dataGridViewPrograms.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewPrograms.Columns.AddRange(new DataGridViewColumn[] { colNo, colProgram, colEnvironment, colProfile, colStart });
            dataGridViewPrograms.Dock = DockStyle.Fill;
            dataGridViewPrograms.Location = new Point(0, 0);
            dataGridViewPrograms.Name = "dataGridViewPrograms";
            dataGridViewPrograms.Size = new Size(869, 450);
            dataGridViewPrograms.TabIndex = 3;
            dataGridViewPrograms.CellClick += dataGridViewPrograms_CellClick;
            dataGridViewPrograms.CellDoubleClick += dataGridViewPrograms_CellDoubleClick;
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
            // colEnvironment
            // 
            colEnvironment.HeaderText = "Environment";
            colEnvironment.Name = "colEnvironment";
            colEnvironment.Width = 120;
            // 
            // colProfile
            // 
            colProfile.HeaderText = "Profile";
            colProfile.Name = "colProfile";
            colProfile.ReadOnly = true;
            colProfile.Width = 350;
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
            FormClosing += frmStart_FormClosing;
            Load += frmStart_Load;
            ((System.ComponentModel.ISupportInitialize)dataGridViewPrograms).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private DataGridView dataGridViewPrograms;
        private DataGridViewTextBoxColumn colNo;
        private DataGridViewTextBoxColumn colProgram;
        private DataGridViewComboBoxColumn colEnvironment;
        private DataGridViewTextBoxColumn colProfile;
        private DataGridViewButtonColumn colStart;
    }
}