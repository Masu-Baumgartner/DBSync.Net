using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net.Core
{
    public class ColumnCache
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string PropName { get; set; }
        public string PropType { get; set; }
        public int Length { get; set; }
        public bool AutoIncrement { get; set; }
        public bool PrimaryKey { get; set; }
        public bool Encrypt { get; set; }
    }
}
