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
            tabPageStart = new TabPage();
            tabPagePrograms = new TabPage();
            tabPageDocument = new TabPage();
            tabControlContainer.SuspendLayout();
            SuspendLayout();
            // 
            // tabControlContainer
            // 
            tabControlContainer.Alignment = TabAlignment.Bottom;
            tabControlContainer.Controls.Add(tabPageStart);
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
            // 
            // tabPageStart
            // 
            tabPageStart.Location = new Point(4, 4);
            tabPageStart.Name = "tabPageStart";
            tabPageStart.Padding = new Padding(3);
            tabPageStart.Size = new Size(888, 463);
            tabPageStart.TabIndex = 0;
            tabPageStart.Text = "Start";
            tabPageStart.UseVisualStyleBackColor = true;
            // 
            // tabPagePrograms
            // 
            tabPagePrograms.Location = new Point(4, 4);
            tabPagePrograms.Name = "tabPagePrograms";
            tabPagePrograms.Padding = new Padding(3);
            tabPagePrograms.Size = new Size(835, 463);
            tabPagePrograms.TabIndex = 1;
            tabPagePrograms.Text = "Programs";
            tabPagePrograms.UseVisualStyleBackColor = true;
            // 
            // tabPageDocument
            // 
            tabPageDocument.Location = new Point(4, 4);
            tabPageDocument.Name = "tabPageDocument";
            tabPageDocument.Size = new Size(835, 463);
            tabPageDocument.TabIndex = 2;
            tabPageDocument.Text = "Document";
            tabPageDocument.UseVisualStyleBackColor = true;
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
            Load += frmMain_Load;
            tabControlContainer.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TabControl tabControlContainer;
        private TabPage tabPageStart;
        private TabPage tabPagePrograms;
        private TabPage tabPageDocument;
    }
}
