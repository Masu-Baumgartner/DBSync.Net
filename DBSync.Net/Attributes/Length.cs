using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class Length : Attribute
    {
        readonly int lenght = -1;
        public Length(int lenght)
        {
            this.lenght = lenght;
        }

        internal int Value
        {
            get { return lenght; }
        }
    }
}
