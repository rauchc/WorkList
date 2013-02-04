using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorkList
{
    public partial class frmPrintChooser : Form
    {
        private WorkListFile _file;

        public frmPrintChooser(ref WorkListFile file)
        {
            _file = file;
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            _file.Print(chkNew.Checked,chkInProgress.Checked,chkDone.Checked);
            this.Close();
        }
    }
}
