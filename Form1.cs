using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace WorkList
{
    #region frmWorkList
    public partial class frmWorkList : Form
    {
        #region variable declaration
        /// <summary>
        /// command line arguments
        /// </summary>
        private string[] args;

        /// <summary>
        /// path to our file
        /// </summary>
        private string _path = null;

        int index = 0;

        private WorkListFile _file;

        /// <summary>
        /// did we make any changes after loading?
        /// </summary>
        private bool _changed = false;

        private bool _startup = true;

        #endregion

        #region constructors
        public frmWorkList()
        {
            InitializeComponent();
        }

        public frmWorkList(string[] args)
        {
            InitializeComponent();
            
            this.args = args;
            for (int i = 0; i < args.Length;i++ )
            {
                if (args[i].Length == 2 && (args[i][0] == '-' || args[i][0] == '/'))
                {
                    args[i] = args[i][1].ToString();
                }

                switch (args[i])
                {
                    case "m":
                        // minimize
                        this.WindowState = FormWindowState.Minimized;
                        this.ShowInTaskbar = false;
                        this.Hide();
                        notifyIcon1.Visible = true;
                        break;
                    case "f":
                        if (args.Length > i)
                        {
                            i++;
                            FileInfo _fi = new FileInfo(args[i]);
                            if (_fi.Exists)
                            {
                                _path = args[i];
                            }
                            else
                            {
                                MessageBox.Show("File not found.");
                            }
                        }
                        break;
                    default:
                        if (_path == null)
                        {
                            FileInfo _fi = new FileInfo(args[i]);
                            if (_fi.Exists)
                            {
                                _path = args[i];
                            }
                        }
                        break;
                }
            }

            // empty details view
            lblSectionContent.Text = "";
            lblNameContent.Text = "";
            lblDetailsContent.Text = "";
            // Debug override
            //if(_path == null)
            //    _path = Application.StartupPath + @"/Worklist.xml";
            _startup = false;
        }
        #endregion

        #region context menues
        private void lbTodo_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                index = this.lbTodo.IndexFromPoint(e.Location);
                lbTodo.SelectedIndex = index;
                setContextMenu(index, menuToDo);
                showDetails(WorkListFile.ItemType.New, this.lbTodo, index);
            }
            catch (ArgumentOutOfRangeException)
            {
                setContextMenu(ListBox.NoMatches, menuToDo);
            }
        }


        private void lbInProgress_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                index = this.lbInProgress.IndexFromPoint(e.Location);
                lbInProgress.SelectedIndex = index;
                setContextMenu(index, menuProgress);
                showDetails(WorkListFile.ItemType.InProgress, this.lbInProgress, index);
            }
            catch (ArgumentOutOfRangeException)
            {
                setContextMenu(ListBox.NoMatches, menuProgress);
            }
        }

        private void lbDone_MouseUp(object sender, MouseEventArgs e)
        {
            try
            {
                index = this.lbDone.IndexFromPoint(e.Location);
                lbDone.SelectedIndex = index;
                setContextMenu(index, menuDone);
                showDetails(WorkListFile.ItemType.Done,this.lbDone, index);
            }
            catch (ArgumentOutOfRangeException)
            {
                setContextMenu(ListBox.NoMatches, menuDone);
            }
        }

        private void showDetails(WorkListFile.ItemType type,ListBox listBox, int index)
        {
            switch (type)
            {
                case WorkListFile.ItemType.New:
                    lblSectionContent.Text = "New";
                    break;
                case WorkListFile.ItemType.InProgress:
                    lblSectionContent.Text = "In Progress";
                    break;
                case WorkListFile.ItemType.Done:
                    lblSectionContent.Text = "Done";
                    break;
            }
            ListEntry _tmp = (ListEntry)listBox.Items[index];
            lblNameContent.Text = _tmp.Name;
            lblDetailsContent.Text = _tmp.Value;
        }

        #region context menu adds
        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            addItem(lbTodo, WorkListFile.ItemType.New);
        }
        private void addToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            addItem(lbInProgress, WorkListFile.ItemType.InProgress);
        }
        private void addToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            addItem(lbDone, WorkListFile.ItemType.Done);
        }
        #endregion

        #region context menu moves
        private void todoToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.Done, lbDone, index, WorkListFile.ItemType.New, lbTodo);
            _changed = true;
        }

        private void inProgressToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.Done, lbDone, index, WorkListFile.ItemType.InProgress, lbInProgress);
            _changed = true;
        }

        private void inProgressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.New, lbTodo, index, WorkListFile.ItemType.InProgress, lbInProgress);
            _changed = true;
        }

        private void doneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.New, lbTodo, index, WorkListFile.ItemType.Done, lbDone);
            _changed = true;
        }

        private void toDoToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.InProgress, lbInProgress, index, WorkListFile.ItemType.New, lbTodo);
            _changed = true;
        }

        private void doneToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _file.Move(WorkListFile.ItemType.InProgress, lbInProgress, index, WorkListFile.ItemType.Done, lbDone);
            _changed = true;
        }
        #endregion


        #region context menu deletes

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // ToDo
            _file.Delete(WorkListFile.ItemType.New, lbTodo, index);
        }

        private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Done
            _file.Delete(WorkListFile.ItemType.Done, lbDone, index);
        }

        private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Progress
            _file.Delete(WorkListFile.ItemType.InProgress, lbInProgress, index);
        }


        #endregion

        #endregion

        #region Helper functions
        private void addItem(ListBox lb, WorkListFile.ItemType type)
        {
            string text = null;
            if (InputBox(Application.ProductName, "Add new item: ", ref text) == System.Windows.Forms.DialogResult.OK)
            {
                _file.Add(type, lb, text,null);
                _changed = true;
            }
        }

        private void editDetails(ListEntry le)
        {

            string _details = null;
            if (le.Value != null)
                _details = le.Value;
            
            if (InputBox(Application.ProductName, "Edit Details: ", ref _details) == System.Windows.Forms.DialogResult.OK)
            {
                le.Value = _details;
                _changed = true;
            }
        }

        private void setContextMenu(int index, ContextMenuStrip cont)
        {
            if (index != ListBox.NoMatches)
            {
                cont.Items[0].Enabled = true;
                cont.Items[1].Enabled = true;
            }
            else
            {
                // disable move/delete when no item is selecteds
                cont.Items[0].Enabled = false;
                cont.Items[1].Enabled = false;
            }
        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void frmWorkList_Load(object sender, EventArgs e)
        {
            if (_path != null)
            {
                _file = new WorkListFile(_path);
                _file.fillBoxes(lbTodo, lbInProgress, lbDone);
            }
            else
            {
                _file = new WorkListFile();
            }
        }

        #endregion

        #region menu strip
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _file.Save();
                // reset changed marker
                _changed = false;
            }
            catch (ApplicationException)
            {
                SaveFileDialog sdlg = new SaveFileDialog();
                sdlg.Filter = "WorkList XML files (*.xml)|*.xml";
                sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                sdlg.CheckFileExists = false;
                sdlg.CheckPathExists = true;
                sdlg.AddExtension = true;
                sdlg.FileName = Environment.UserName + ".xml";
                DialogResult res = sdlg.ShowDialog();
                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    _file.Save(sdlg.FileName);
                    // reset changed marker
                    _changed = false;
                }
            }
        }


        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sdlg = new SaveFileDialog();
            sdlg.Filter = "WorkList XML files (*.xml)|*.xml";
            sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            sdlg.CheckFileExists = false;
            sdlg.CheckPathExists = true;
            sdlg.AddExtension = true;
            sdlg.FileName = Environment.UserName + ".xml";
            DialogResult res = sdlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                _file.Save(sdlg.FileName);
                // reset changed marker
                _changed = false;
            }
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void frmWorkList_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_changed == true)
            {
                DialogResult dlg = MessageBox.Show("Do you want to save your changes?", Application.ProductName, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dlg == System.Windows.Forms.DialogResult.Yes)
                {
                    try
                    {
                        _file.Save();
                        // reset changed marker
                        _changed = false;
                    }
                    catch (ApplicationException)
                    {
                        SaveFileDialog sdlg = new SaveFileDialog();
                        sdlg.Filter = "WorkList XML files (*.xml)|*.xml";
                        sdlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                        sdlg.CheckFileExists = false;
                        sdlg.CheckPathExists = true;
                        sdlg.AddExtension = true;
                        sdlg.FileName = Environment.UserName + ".xml";
                        DialogResult res = sdlg.ShowDialog();
                        if (res == System.Windows.Forms.DialogResult.OK)
                        {
                            _file.Save(sdlg.FileName);
                            // reset changed marker
                            _changed = false;
                        }
                    }
                }
            }
        }
        #endregion

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog odlg = new OpenFileDialog();
            odlg.Filter = "WorkList XML files (*.xml)|*.xml";
            odlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            odlg.CheckFileExists = true;
            DialogResult res = odlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                _file = new WorkListFile(odlg.FileName);
                _file.fillBoxes(lbTodo, lbInProgress, lbDone);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAbout frm = new frmAbout();
            frm.ShowDialog();
        }

        private void editDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Done
            editDetails((ListEntry)lbDone.SelectedItem);
        }

        private void editDetailsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // ToDo
            editDetails((ListEntry)lbTodo.SelectedItem);
        }

        private void editDetailsToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // InProgress
            editDetails((ListEntry)lbInProgress.SelectedItem);
        }

        #region menuProgress sorting
        private void topToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // move to top
            if (lbInProgress.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.InProgress, "top", index,lbInProgress);
                _changed = true;
            }
        }

        private void oneUpToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // move one up
            if (lbInProgress.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.InProgress, "up", index, lbInProgress);
                _changed = true;
            }
        }

        private void oneDownToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // move one down
            if (lbInProgress.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.InProgress, "down", index, lbInProgress);
                _changed = true;
            }
        }

        private void bottomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // move to bottom
            if (lbInProgress.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.InProgress, "bottom", index, lbInProgress);
                _changed = true;
            }
        }
        #endregion

        #region menuToDo sorting
        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // move to top
            if (lbTodo.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.New, "top", index, lbTodo);
                _changed = true;
            }
        }

        private void oneUpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // move one up
            if (lbTodo.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.New, "up", index, lbTodo);
                _changed = true;
            }
        }

        private void oneDownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // move one down
            if (lbTodo.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.New, "down", index, lbTodo);
                _changed = true;
            }
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // move one up
            if (lbTodo.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.New, "bottom", index, lbTodo);
                _changed = true;
            }
        }
        #endregion

        #region menuDone sorting
        private void topToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // move to top
            if (lbDone.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.Done, "top", index, lbDone);
                _changed = true;
            }
        }

        private void oneUpToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // move one up
            if (lbDone.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.Done, "up", index, lbDone);
                _changed = true;
            }
        }

        private void oneDownToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // move one down
            if (lbDone.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.Done, "down", index, lbDone);
                _changed = true;
            }
        }

        private void bottomToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // move to bottom
            if (lbDone.SelectedIndex != ListBox.NoMatches)
            {
                _file.sort(WorkListFile.ItemType.Done, "bottom", index, lbDone);
                _changed = true;
            }
        }
        #endregion

        private void editToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // menuDone
            editItem(lbDone, WorkListFile.ItemType.Done, index);
        }

        private void editItem(ListBox lb, WorkListFile.ItemType type, int index)
        {
            string text = ((ListEntry)lb.Items[index]).Name;

            if (InputBox(Application.ProductName, "Edit item: ", ref text) == System.Windows.Forms.DialogResult.OK)
            {
                _file.Edit(type, lb, index, text);
                _changed = true;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // menuTodo
            editItem(lbTodo, WorkListFile.ItemType.New, index);
        }

        private void editToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // menuInProgress
            editItem(lbInProgress, WorkListFile.ItemType.InProgress, index);
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPrintChooser frm = new frmPrintChooser(ref _file);
            frm.ShowDialog();
        }

        private void frmWorkList_Resize(object sender, EventArgs e)
        {
            if (_startup == false)
            { 
                if (FormWindowState.Minimized == this.WindowState)
                {
                    notifyIcon1.Visible = true;
                    this.ShowInTaskbar = false;
                    this.Hide();
                }
                else
                {
                    notifyIcon1.Visible = false;
                }
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (FormWindowState.Minimized == this.WindowState)
            {
                this.WindowState = FormWindowState.Normal;
                this.ShowInTaskbar = true;
                this.Show();
                this.BringToFront();
            }
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmSettings frm = new frmSettings(_file);
            frm.ShowDialog();
        }
    }
    #endregion

    #region custom exception class
    [Serializable]
    public class WorkListException : Exception
    {
        public string ErrorMessage
        {
            get
            {
                return base.Message.ToString();
            }
        }

        public WorkListException(string errorMessage) 
            : base(errorMessage) {}

        public WorkListException(string errorMessage, Exception innerEx)
                   : base(errorMessage, innerEx) {}
    }
    #endregion
}
