using DBSync.Net.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net.Interfaces
{
    public interface IDBSyncModel
    {
        [Key("id")]
        public int Id { get; set; }
    }
}
