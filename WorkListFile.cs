using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;
using Microsoft.Win32;

namespace WorkList
{
    public class WorkListFile
    {
        private List<ListEntry> _new;

        internal List<ListEntry> ListNew
        {
            get { return _new; }
            set { _new = value; }
        }

        private List<ListEntry> _inProgress;

        internal List<ListEntry> ListInProgress
        {
            get { return _inProgress; }
            set { _inProgress = value; }
        }

        private List<ListEntry> _done;

        internal List<ListEntry> ListDone
        {
            get { return _done; }
            set { _done = value; }
        }

        private string _path = null;

        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }


        public enum ItemType
        {
            New=0,
            InProgress=1,
            Done=2
        }

        public WorkListFile()
        {
            _new = new List<ListEntry>();
            _inProgress = new List<ListEntry>();
            _done = new List<ListEntry>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">Worklist file to open</param>
        public WorkListFile(string path)
        {
            _path = path;
            try
            {
                XmlDocument _doc = new XmlDocument();
                _doc.Load(_path);

                if (_doc.SelectSingleNode("WorkList") != null)
                {

                    if(_doc.SelectSingleNode("//WorkList/New") != null)
                        _new = getItems("//WorkList/New/item", ref _doc);

                    if (_doc.SelectSingleNode("//WorkList/InProgress") != null)
                        _inProgress = getItems("//WorkList/InProgress/item", ref _doc);

                    if (_doc.SelectSingleNode("//WorkList/Done") != null)
                        _done = getItems("//WorkList/Done/item", ref _doc);
                }
            }
            catch (XmlException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private List<ListEntry> getItems(string xpath, ref XmlDocument _doc)
        {
            List<ListEntry> _l = new List<ListEntry>();
            if (_doc.SelectNodes(xpath) != null)
            {
                foreach(XmlNode _node in _doc.SelectNodes(xpath))
                {
                    //_l.Add(new ListEntry(_node.ChildNodes["text"].InnerText, _node.ChildNodes["details"].InnerText));
                    string _text = null;
                    string _details = null;
                    foreach (XmlNode _item in _node.ChildNodes)
                    {
                        switch (_item.Name)
                        {
                            case "text":
                                _text = _item.InnerText;
                                break;
                            case "details":
                                _details = _item.InnerText;
                                break;
                        }
                    }
                    if (_text != null)
                    {
                        _l.Add(new ListEntry(_text, _details));
                    }
                }
            }
            return _l;
        }


        public void Add(ItemType type,ListBox lb, string text, string details)
        {
            //lb.Items.Add(new ListEntry(text, details));
            switch (type)
            {
                case ItemType.New:
                    _new.Add(new ListEntry(text, details));
                    break;
                case ItemType.InProgress:
                    _inProgress.Add(new ListEntry(text, details));
                    break;
                case ItemType.Done:
                    _done.Add(new ListEntry(text, details));
                    break;
            }
            ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
        }

        public ListEntry Delete(ItemType type, ListBox lb, int index)
        {
            ListEntry _tmp = null;
            _tmp = (ListEntry)lb.Items[index];
            //lb.Items.RemoveAt(index);
            switch (type)
            {
                case ItemType.New:
                    _new.Remove(_tmp);
                    break;
                case ItemType.InProgress:
                    _inProgress.Remove(_tmp);
                    break;
                case ItemType.Done:
                    _done.Remove(_tmp);
                    break;
            }
            ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
            return _tmp;
        }

        public void Move(ItemType type, ListBox lb, int index,ItemType newType, ListBox newLB)
        {
            ListEntry _tmp = Delete(type, lb, index);
            ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
            Add(newType, newLB, _tmp.ToString(), _tmp.Value);
        }


        public void fillBoxes(ListBox lbNew, ListBox lbInProgress, ListBox lbDone)
        {
            fillBox(lbNew, ref _new);
            fillBox(lbInProgress, ref _inProgress);
            fillBox(lbDone, ref _done);
        }

        private void fillBox(ListBox lb, ref List<ListEntry> content)
        {
            lb.DataSource = content;
            lb.DisplayMember = "Name";
            lb.ValueMember = "Value";
            
        }

        public void Save()
        {
            if (isPathSet())
            {
                XmlNode _item;
                XmlNode _text;
                XmlNode _details;
                XmlDocument _doc = new XmlDocument();
                XmlNode rootNode = _doc.CreateElement("WorkList");
                _doc.AppendChild(rootNode);
                XmlNode XmlNew = _doc.CreateElement("New");

                // enter New elements
                foreach (ListEntry text in _new)
                {
                    _item = _doc.CreateElement("item");
                    _text = _doc.CreateElement("text");
                    _text.InnerText = text.ToString();
                    _item.AppendChild(_text);
                    _details = _doc.CreateElement("details");
                    _details.InnerText = text.Value;
                    _item.AppendChild(_details);
                    XmlNew.AppendChild(_item);
                }
                rootNode.AppendChild(XmlNew);

                // enter InProgress elements
                XmlNode xmlInProgress = _doc.CreateElement("InProgress");
                foreach (ListEntry text in _inProgress)
                {
                    _item = _doc.CreateElement("item");
                    _text = _doc.CreateElement("text");
                    _text.InnerText = text.ToString();
                    _item.AppendChild(_text);
                    _details = _doc.CreateElement("details");
                    _details.InnerText = text.Value;
                    _item.AppendChild(_details);
                    xmlInProgress.AppendChild(_item);
                }
                rootNode.AppendChild(xmlInProgress);

                // enter Done elements
                XmlNode xmlDone = _doc.CreateElement("Done");
                foreach (ListEntry text in _done)
                {
                    _item = _doc.CreateElement("item");
                    _text = _doc.CreateElement("text");
                    _text.InnerText = text.ToString();
                    _item.AppendChild(_text);
                    _details = _doc.CreateElement("details");
                    _details.InnerText = text.Value;
                    _item.AppendChild(_details);
                    xmlDone.AppendChild(_item);
                }
                rootNode.AppendChild(xmlDone);
                // save XML
                _doc.Save(_path);                
            }
            else
            {
                throw new ApplicationException("No Path has been set!");
            }
        }

        public void sort(ItemType type, string position, int index, ListBox lb)
        {
            ListEntry _tmp = null;
            switch (type)
            {
                case ItemType.InProgress:
                    _tmp = _inProgress[index];
                    switch (position)
                    {
                        case "top":
                            _inProgress.RemoveAt(index);
                            _inProgress.Insert(0, _tmp);
                            break;
                        case "up":
                            if ((index - 1) >= 0)
                            {
                                _inProgress.RemoveAt(index);
                                _inProgress.Insert(index - 1, _tmp);
                            }
                            break;
                        case "down":
                            _inProgress.RemoveAt(index);
                            _inProgress.Insert(index + 1, _tmp);
                            break;
                        case "bottom":
                            _inProgress.RemoveAt(index);
                            _inProgress.Insert(_inProgress.Count, _tmp);
                            break;
                    }
                    break;
                case ItemType.Done:
                    _tmp = _done[index];
                    switch (position)
                    {
                        case "top":
                            _done.RemoveAt(index);
                            _done.Insert(0, _tmp);
                            break;
                        case "up":
                            if ((index - 1) >= 0)
                            {
                                _done.RemoveAt(index);
                                _done.Insert(index - 1, _tmp);
                            }
                            break;
                        case "down":
                            _done.RemoveAt(index);
                            _done.Insert(index + 1, _tmp);
                            break;
                        case "bottom":
                            _done.RemoveAt(index);
                            _done.Insert(_done.Count, _tmp);
                            break;
                    }
                    break;
                case ItemType.New:
                    _tmp = _new[index];
                    switch (position)
                    {
                        case "top":
                            _new.RemoveAt(index);
                            _new.Insert(0, _tmp);
                            break;
                        case "up":
                            if ((index - 1) >= 0)
                            {
                                _new.RemoveAt(index);
                                _new.Insert(index - 1, _tmp);
                            }
                            break;
                        case "down":
                            if (index + 1 != _new.Count)
                            {
                                _new.RemoveAt(index);
                                _new.Insert(index + 1, _tmp);
                            }
                            break;
                        case "bottom":
                            _new.RemoveAt(index);
                            _new.Insert(_new.Count, _tmp);
                            break;
                    }
                    break;
            }
            ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
        }

        public void Edit(ItemType type, ListBox lb, int index, string text)
        {
            switch (type)
            {
                case ItemType.New:
                    _new[index].Name = text;
                    break;
                case ItemType.InProgress:
                    _inProgress[index].Name = text;
                    break;
                case ItemType.Done:
                    _done[index].Name = text;
                    break;
            }
            ((CurrencyManager)lb.BindingContext[lb.DataSource]).Refresh();
        }


        public void Save(string path)
        {
            _path = path;
            Save();
        }

        public bool isPathSet()
        {
            if (_path != null)
                return true;
            else
                return false;
        }


        public void Print(bool printNew, bool printInProgress, bool printDone)
        {
            // create new webbrowser control
            WebBrowser web = new WebBrowser();
            web.DocumentCompleted += new WebBrowserDocumentCompletedEventHandler(web_DocumentCompleted);
            
            //web.DocumentText = "";
            string text = null;
            text = "<!DOCTYPE HTML PUBLIC \"-//W3C//DTD HTML 4.01 Transitional//EN\"" +
       "\"http://www.w3.org/TR/html4/loose.dtd\">" +
"<html><head><title>WorkList</title>";
            text += "<style type=\"text/css\">" + Environment.NewLine + "<!--" + Environment.NewLine;
            text += "   h1 {color: #1F497D;font: Cambria 13pt}" + Environment.NewLine;
            text += "--></style>";
            text += "</head><body>";
            if (_new.Count > 0 && printNew == true)
            {
                text += "<h1>New</h1>";
                text += "<ul type=\"circle\">" + Environment.NewLine;
                foreach (ListEntry le in _new)
                {
                    if(!string.IsNullOrEmpty(le.Value))
                        text += "   <li>" + le.Name + " (" + le.Value + ")</li>" + Environment.NewLine;
                    else
                        text += "   <li>" + le.Name + "</li>" + Environment.NewLine;
                }
                text += "</ul>" + Environment.NewLine;
            }

            if (_inProgress.Count > 0 && printInProgress == true)
            {
                text += "<h1>In Progress</h1>";
                text += "<ul type=\"circle\">" + Environment.NewLine;
                foreach (ListEntry le in _inProgress)
                {
                    if (!string.IsNullOrEmpty(le.Value))
                        text += "   <li>" + le.Name + " (" + le.Value + ")</li>" + Environment.NewLine;
                    else
                        text += "   <li>" + le.Name + "</li>" + Environment.NewLine;
                }
                text += "</ul>" + Environment.NewLine;
            }

            if (_done.Count > 0 && printDone == true)
            {
                text += "<h1>Done</h1>";
                text += "<ul type=\"circle\">" + Environment.NewLine;
                foreach (ListEntry le in _done)
                {
                    if (!string.IsNullOrEmpty(le.Value))
                        text += "   <li>" + le.Name + " (" + le.Value + ")</li>" + Environment.NewLine;
                    else
                        text += "   <li>" + le.Name + "</li>" + Environment.NewLine;
                }
                text += "</ul>" + Environment.NewLine;
            }

            text += "</body></html>";

            web.DocumentText = text;
        }

        private void web_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            // replace header and footer
            string key = @"Software\Microsoft\Internet Explorer\PageSetup";
            
            // read current user settings for header and footer
            string _footer = Registry.CurrentUser.OpenSubKey(key).GetValue("footer").ToString();
            string _header = Registry.CurrentUser.OpenSubKey(key).GetValue("header").ToString();
            
            // set header and footer to empty string
            Registry.CurrentUser.OpenSubKey(key, true).SetValue("footer", "");
            Registry.CurrentUser.OpenSubKey(key, true).SetValue("header", "");
                        
            // print the document
            ((WebBrowser)sender).ShowPrintDialog();
            
            // Replace modified settings with original ones
            Registry.CurrentUser.OpenSubKey(key, true).SetValue("footer", _footer);
            Registry.CurrentUser.OpenSubKey(key, true).SetValue("header", _header);
            ((WebBrowser)sender).Dispose();
        }
    }
}
