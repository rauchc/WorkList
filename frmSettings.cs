using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.IO;

namespace WorkList
{
    public partial class frmSettings : Form
    {
        private const string RUN_LOCATION = @"Software\Microsoft\Windows\CurrentVersion\Run";

        public frmSettings(WorkListFile _file)
        {
            InitializeComponent();
            if (_file != null && _file.Path != null)
            {
                txtFile.Text = _file.Path;
            }
        }

        private void frmSettings_Load(object sender, EventArgs e)
        {
            if (IsAutoStartEnabled("WorkList", System.Reflection.Assembly.GetExecutingAssembly().Location))
            {
                chkAutorun.Checked = true;
            }
        }

        /// <summary>
        /// Sets the autostart value for the assembly.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
        public static void SetAutoStart(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.SetValue(keyName, assemblyLocation);
        }

        /// <summary>
        /// Returns whether auto start is enabled.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        /// <param name="assemblyLocation">Assembly location (e.g. Assembly.GetExecutingAssembly().Location)</param>
        public static bool IsAutoStartEnabled(string keyName, string assemblyLocation)
        {
            RegistryKey key = Registry.CurrentUser.OpenSubKey(RUN_LOCATION);
            if (key == null)
                return false;

            string value = (string)key.GetValue(keyName);
            if (value == null)
                return false;

            return (value == assemblyLocation);
        }

        /// <summary>
        /// Unsets the autostart value for the assembly.
        /// </summary>
        /// <param name="keyName">Registry Key Name</param>
        public static void UnSetAutoStart(string keyName)
        {
            RegistryKey key = Registry.CurrentUser.CreateSubKey(RUN_LOCATION);
            key.DeleteValue(keyName);
        }

        private void chkAutorun_CheckedChanged(object sender, EventArgs e)
        {
            chkFile.Enabled = chkAutorun.Checked;
            chkMinimized.Enabled = chkAutorun.Checked;
        }

        private void chkFile_CheckedChanged(object sender, EventArgs e)
        {
            txtFile.Enabled = chkFile.Checked;
            btnBrowse.Enabled = chkFile.Checked;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            string _value = null;

            if (chkAutorun.Checked == true)
            {
                _value = System.Reflection.Assembly.GetExecutingAssembly().Location;

                if (chkFile.Checked == true && File.Exists(txtFile.Text))
                {
                    _value += " " + txtFile.Text;
                }

                if (chkMinimized.Checked == true)
                {
                    _value += " /m";
                }

                SetAutoStart("WorkList", _value);
            }
            else
            {
                UnSetAutoStart("WorkList");
            }
            this.Close();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog odlg = new OpenFileDialog();
            odlg.Filter = "WorkList XML files (*.xml)|*.xml";
            odlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            odlg.CheckFileExists = true;
            DialogResult res = odlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                txtFile.Text = odlg.FileName;
            }
        }
    }
}
