namespace devkit2
{
    partial class frmFileExplorer
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
            splitContainer1 = new SplitContainer();
            explorerPane1 = new ExplorerPane();
            explorerPane2 = new ExplorerPane();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(explorerPane1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(explorerPane2);
            splitContainer1.Size = new Size(800, 450);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 0;
            // 
            // explorerPane1
            // 
            explorerPane1.Dock = DockStyle.Fill;
            explorerPane1.Location = new Point(0, 0);
            explorerPane1.Name = "explorerPane1";
            explorerPane1.Size = new Size(266, 450);
            explorerPane1.TabIndex = 0;
            // 
            // explorerPane2
            // 
            explorerPane2.Dock = DockStyle.Fill;
            explorerPane2.Location = new Point(0, 0);
            explorerPane2.Name = "explorerPane2";
            explorerPane2.Size = new Size(530, 450);
            explorerPane2.TabIndex = 0;
            // 
            // frmFileExplorer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            KeyPreview = true;
            Name = "frmFileExplorer";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "frmFileExplorer";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer1;
        private ExplorerPane explorerPane1;
        private ExplorerPane explorerPane2;
    }
}