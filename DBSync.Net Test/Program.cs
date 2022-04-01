using DBSync.Net;

using System;

namespace DBSync.Net_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var conf = DatabaseConfig.Get;
            DBSyncGlobal.GlobalConnectionString = new DBSyncConnectionString(conf.Server, conf.Port, conf.Uid, conf.Pwd, conf.Database);

            DBSyncGlobal.GlobalEncryptionKey = "LarsMagZüge";

            var x = new DBSyncTable<TestyModel>("testy");
            
            x.Init();

            x.Add(new TestyModel()
            {
                DanielTest = "",
                Encrypted = "geheim1234",
                Text = "testy"
            });

            var o = x.Find(y => y.Text == "testy");

            o.Text = "tee";

            x.Modify(o);

            Console.WriteLine("Finished");
            Console.ReadLine();
        }
    }
}
