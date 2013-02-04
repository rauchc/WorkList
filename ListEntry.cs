using System;
using System.Collections.Generic;
using System.Text;

namespace WorkList
{
    public class ListEntry
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string value;

        public string Value
        {
            get { return this.value; }
            set { this.value = value; }
        }

        public ListEntry()
        {
            name = null;
            value = null;
        }

        public ListEntry(string _n, string _v)
        {
            name = _n;
            value = _v;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
