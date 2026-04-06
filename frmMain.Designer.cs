namespace devkit2
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
            tabPageMyProjects = new TabPage();
            tabPageManualLaunch = new TabPage();
            tabPagePrograms = new TabPage();
            tabPageDocument = new TabPage();
            tabPageFileExplorer = new TabPage();
            tabControlContainer.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlContainer
            // 
            tabControlContainer.Alignment = TabAlignment.Bottom;
            tabControlContainer.Controls.Add(tabPageMyProjects);
            tabControlContainer.Controls.Add(tabPageFileExplorer);
            tabControlContainer.Controls.Add(tabPageManualLaunch);
            tabControlContainer.Controls.Add(tabPagePrograms);
            tabControlContainer.Controls.Add(tabPageDocument);
            tabControlContainer.Dock = DockStyle.Fill;
            tabControlContainer.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            tabControlContainer.Location = new Point(0, 0);
            tabControlContainer.Multiline = true;
            tabControlContainer.Name = "tabControlContainer";
            tabControlContainer.SelectedIndex = 0;
            tabControlContainer.Size = new Size(896, 497);
            tabControlContainer.TabIndex = 0;
            tabControlContainer.Selected += tabControlContainer_Selected;
            // 
            // tabPageMyProjects
            // 
            tabPageMyProjects.Location = new Point(4, 4);
            tabPageMyProjects.Name = "tabPageMyProjects";
            tabPageMyProjects.Size = new Size(888, 463);
            tabPageMyProjects.TabIndex = 3;
            tabPageMyProjects.Text = "My Projects";
            tabPageMyProjects.UseVisualStyleBackColor = true;
            // 
            // tabPageManualLaunch
            // 
            tabPageManualLaunch.Location = new Point(4, 4);
            tabPageManualLaunch.Name = "tabPageManualLaunch";
            tabPageManualLaunch.Padding = new Padding(3);
            tabPageManualLaunch.Size = new Size(888, 463);
            tabPageManualLaunch.TabIndex = 0;
            tabPageManualLaunch.Text = "Manual Launch";
            tabPageManualLaunch.UseVisualStyleBackColor = true;
            // 
            // tabPagePrograms
            // 
            tabPagePrograms.Location = new Point(4, 4);
            tabPagePrograms.Name = "tabPagePrograms";
            tabPagePrograms.Padding = new Padding(3);
            tabPagePrograms.Size = new Size(888, 463);
            tabPagePrograms.TabIndex = 1;
            tabPagePrograms.Text = "Programs";
            tabPagePrograms.UseVisualStyleBackColor = true;
            // 
            // tabPageDocument
            // 
            tabPageDocument.Location = new Point(4, 4);
            tabPageDocument.Name = "tabPageDocument";
            tabPageDocument.Size = new Size(888, 463);
            tabPageDocument.TabIndex = 2;
            tabPageDocument.Text = "Document";
            tabPageDocument.UseVisualStyleBackColor = true;
            // 
            // tabPageFileExplorer
            // 
            tabPageFileExplorer.Location = new Point(4, 4);
            tabPageFileExplorer.Name = "tabPageFileExplorer";
            tabPageFileExplorer.Size = new Size(888, 463);
            tabPageFileExplorer.TabIndex = 4;
            tabPageFileExplorer.Text = "File Explorer";
            tabPageFileExplorer.UseVisualStyleBackColor = true;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(896, 497);
            Controls.Add(tabControlContainer);
            Name = "frmMain";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DevKit2";
            FormClosing += MainForm_FormClosing;
            Load += frmMain_Load;
            tabControlContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlContainer;
        private TabPage tabPageManualLaunch;
        private TabPage tabPagePrograms;
        private TabPage tabPageDocument;
        private TabPage tabPageMyProjects;
        private TabPage tabPageFileExplorer;
    }
}
