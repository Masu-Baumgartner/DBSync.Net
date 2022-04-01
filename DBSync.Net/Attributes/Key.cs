using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class Key : Attribute
    {
        readonly string value;
        public Key(string key)
        {
            value = key;
        }
        internal string Value
        {
            get { return value; }
        }
    }
}
