using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSync.Net
{
    public class DBSyncConnectionString
    {
        public DBSyncConnectionString()
        {

        }

        public DBSyncConnectionString(string server, int port, string username, string password, string database)
        {
            Server = server;
            Port = port;
            Username = username;
            Password = password;
            Database = database;
        }

        public string Server { get; set; }

        public DBSyncConnectionString SetServer(string value)
        {
            Server = value;
            return this;
        }

        public int Port { get; set; }

        public DBSyncConnectionString SetPort(int value)
        {
            Port = value;
            return this;
        }

        public string Username { get; set; }

        public DBSyncConnectionString SetUsername(string value)
        {
            Username = value;
            return this;
        }

        public string Password { get; set; }

        public DBSyncConnectionString SetPassword(string value)
        {
            Password = value;
            return this;
        }

        public string Database { get; set; }

        public DBSyncConnectionString SetDatabase(string value)
        {
            Database = value;
            return this;
        }

        public override string ToString()
        {
            return $"server={Server};" +
                $"port={Port};" +
                $"uid={Username};" +
                $"pwd={Password};" +
                $"database={Database}";
        }

        public static string ToString(DBSyncConnectionString c)
        {
            return c.ToString();
        }

        public static DBSyncConnectionString FromString(string c)
        {
            var parts = c.Split(';');
            DBSyncConnectionString current = new DBSyncConnectionString();
            foreach (var p in parts)
            {
                var name = p.Substring(0, p.IndexOf("="));
                var value = p.Remove(0, name.Length + 1);
                switch (name.ToLower())
                {
                    case "server":
                        current.Server = value;
                        break;
                    case "port":
                        current.Port = int.Parse(value);
                        break;
                    case "uid":
                        current.Username = value;
                        break;
                    case "pwd":
                        current.Password = value;
                        break;
                    case "database":
                        current.Database = value;
                        break;
                    default:
                        break;
                }
            }
            return current;
        }

        public static implicit operator string(DBSyncConnectionString c)
        {
            return c.ToString();
        }

        public static implicit operator DBSyncConnectionString(string c)
        {
            return FromString(c);
        }
    }
}