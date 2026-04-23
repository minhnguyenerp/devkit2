namespace devkit2
{
    partial class frmSSHKeys
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
            toolStrip1 = new ToolStrip();
            toolStripButtonImportRawKey = new ToolStripButton();
            toolStripSeparator2 = new ToolStripSeparator();
            toolStripButtonImport = new ToolStripButton();
            toolStripButtonExport = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButtonDelete = new ToolStripButton();
            toolStripButtonSettings = new ToolStripButton();
            tableLayoutPanel1 = new TableLayoutPanel();
            textBox1 = new TextBox();
            listViewKeys = new ListView();
            toolStrip1.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStrip1
            // 
            toolStrip1.ImageScalingSize = new Size(28, 28);
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonImportRawKey, toolStripSeparator2, toolStripButtonImport, toolStripButtonExport, toolStripSeparator1, toolStripButtonDelete, toolStripButtonSettings });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(722, 35);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonImportRawKey
            // 
            toolStripButtonImportRawKey.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonImportRawKey.Image = Properties.Resources.import;
            toolStripButtonImportRawKey.ImageTransparentColor = Color.Magenta;
            toolStripButtonImportRawKey.Name = "toolStripButtonImportRawKey";
            toolStripButtonImportRawKey.Size = new Size(32, 32);
            toolStripButtonImportRawKey.Text = "Import Raw Key";
            toolStripButtonImportRawKey.Click += toolStripButtonImportRawKey_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(6, 35);
            // 
            // toolStripButtonImport
            // 
            toolStripButtonImport.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonImport.Image = Properties.Resources.import1;
            toolStripButtonImport.ImageTransparentColor = Color.Magenta;
            toolStripButtonImport.Name = "toolStripButtonImport";
            toolStripButtonImport.Size = new Size(32, 32);
            toolStripButtonImport.Text = "Import";
            toolStripButtonImport.Click += toolStripButtonImport_Click;
            // 
            // toolStripButtonExport
            // 
            toolStripButtonExport.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonExport.Image = Properties.Resources.export1;
            toolStripButtonExport.ImageTransparentColor = Color.Magenta;
            toolStripButtonExport.Name = "toolStripButtonExport";
            toolStripButtonExport.Size = new Size(32, 32);
            toolStripButtonExport.Text = "Export";
            toolStripButtonExport.Click += toolStripButtonExport_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 35);
            // 
            // toolStripButtonDelete
            // 
            toolStripButtonDelete.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDelete.Image = Properties.Resources.trash;
            toolStripButtonDelete.ImageTransparentColor = Color.Magenta;
            toolStripButtonDelete.Name = "toolStripButtonDelete";
            toolStripButtonDelete.Size = new Size(32, 32);
            toolStripButtonDelete.Text = "Delete";
            toolStripButtonDelete.Click += toolStripButtonDelete_Click;
            // 
            // toolStripButtonSettings
            // 
            toolStripButtonSettings.Alignment = ToolStripItemAlignment.Right;
            toolStripButtonSettings.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonSettings.Image = Properties.Resources.cogwheel;
            toolStripButtonSettings.ImageTransparentColor = Color.Magenta;
            toolStripButtonSettings.Name = "toolStripButtonSettings";
            toolStripButtonSettings.Size = new Size(32, 32);
            toolStripButtonSettings.Text = "Settings";
            toolStripButtonSettings.Click += toolStripButtonSettings_Click;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.ColumnCount = 1;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tableLayoutPanel1.Controls.Add(textBox1, 0, 1);
            tableLayoutPanel1.Controls.Add(listViewKeys, 0, 0);
            tableLayoutPanel1.Dock = DockStyle.Fill;
            tableLayoutPanel1.Location = new Point(0, 35);
            tableLayoutPanel1.Margin = new Padding(4);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 2;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 60F));
            tableLayoutPanel1.Size = new Size(722, 415);
            tableLayoutPanel1.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Dock = DockStyle.Fill;
            textBox1.Location = new Point(4, 170);
            textBox1.Margin = new Padding(4);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ScrollBars = ScrollBars.Vertical;
            textBox1.Size = new Size(714, 241);
            textBox1.TabIndex = 0;
            // 
            // listViewKeys
            // 
            listViewKeys.Dock = DockStyle.Fill;
            listViewKeys.Location = new Point(4, 4);
            listViewKeys.Margin = new Padding(4);
            listViewKeys.Name = "listViewKeys";
            listViewKeys.Size = new Size(714, 158);
            listViewKeys.TabIndex = 1;
            listViewKeys.UseCompatibleStateImageBehavior = false;
            listViewKeys.View = View.Details;
            listViewKeys.SelectedIndexChanged += listViewKeys_SelectedIndexChanged;
            // 
            // frmSSHKeys
            // 
            AutoScaleDimensions = new SizeF(9F, 21F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(722, 450);
            Controls.Add(tableLayoutPanel1);
            Controls.Add(toolStrip1);
            Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Margin = new Padding(4);
            Name = "frmSSHKeys";
            StartPosition = FormStartPosition.CenterParent;
            Text = "SSH Keys";
            Load += frmSSHKeys_Load;
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private ToolStrip toolStrip1;
        private TableLayoutPanel tableLayoutPanel1;
        private TextBox textBox1;
        private ListView listViewKeys;
        private ToolStripButton toolStripButtonImport;
        private ToolStripButton toolStripButtonExport;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonDelete;
        private ToolStripButton toolStripButtonImportRawKey;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton toolStripButtonSettings;
    }
}