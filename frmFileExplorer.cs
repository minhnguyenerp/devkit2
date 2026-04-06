using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace devkit2
{
    public partial class frmFileExplorer : Form
    {
        public frmFileExplorer()
        {
            InitializeComponent();
            splitContainer1.SplitterDistance = this.ClientSize.Width / 2;
        }
    }
}
