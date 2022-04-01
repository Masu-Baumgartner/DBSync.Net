using DBSync.Net.Attributes;
using DBSync.Net.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net_Test
{
    public class TestyModel : IDBSyncModel
    {
        [Key("id")]
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }

        [Key("text")]
        [Length(1025)]
        public string Text { get; set; }

        [Key("encrypted")]
        [Encrypt]
        public string Encrypted { get; set; }

        [Key("daniel_test")]
        public string DanielTest { get; set; }

        [Key("test")]
        public string Testy { get; set; } = "";
    }
}
