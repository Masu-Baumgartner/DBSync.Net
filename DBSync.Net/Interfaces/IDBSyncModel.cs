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
        int Id { get; set; }
    }
}
