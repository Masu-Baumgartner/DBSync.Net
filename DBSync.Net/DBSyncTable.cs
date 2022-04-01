using DBSync.Net.Attributes;
using DBSync.Net.Core;
using DBSync.Net.Interfaces;


using MySql.Data.MySqlClient;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace DBSync.Net
{
    public class DBSyncTable<T> : IList<T>
    {
        private string Table { get; set; }
        public DBSyncConnectionString ConnectionString { get; set; } = null;
        public string EncryptionKey { get; set; } = "";
        public List<T> Data { get; set; }
        private List<ColumnCache> Cache { get; set; }

        public int Count
        {
            get
            {
                return Data.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                lock(Data)
                {
                    return Data[index];
                }
            }
            set
            {
                Modify(value);
            }
        }

        public void Modify(T value)
        {
            lock(Data)
            {
                var con = new MySqlConnection(ConnectionString);
                con.Open();
                var cmd = con.CreateCommand();

                foreach (var c in Cache)
                {
                    if (c.Encrypt)
                        cmd.Parameters.AddWithValue("@" + c.Name, DBSyncCrypt.Encrypt(EncryptionKey, GetProperty(value, c.PropName).ToString()));
                    else
                        cmd.Parameters.AddWithValue("@" + c.Name, GetProperty(value, c.PropName));
                }

                cmd.CommandText = GenerateUpdateTableSQL();
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                var s = Data.Find(x => ((IDBSyncModel)x).Id == ((IDBSyncModel)value).Id);
                Data.Remove(s);
                Data.Add(value);

                con.Close();
            }
        }

        public DBSyncTable(string table)
        {
            Table = table;

            Cache = new();

            var type = typeof(T);

            foreach (var prop in type.GetProperties())
            {
                ColumnCache c = new();

                c.PropName = prop.Name;
                c.PropType = prop.PropertyType.FullName;

                switch (prop.PropertyType.Name)
                {
                    case "Int32":
                        c.Type = "INT";
                        break;
                    case "String":
                        c.Type = "VARCHAR";
                        break;
                    default:
                        c.Type = "VARCHAR";
                        break; //TODO: Add bool
                }

                c.Length = -1;

                foreach (var attr in prop.GetCustomAttributes(true))
                {
                    if (attr is Key key)
                    {
                        c.Name = key.Value;
                    }

                    if (attr is Length l)
                    {
                        c.Length = l.Value;
                    }

                    if (attr is AutoIncrement)
                    {
                        c.AutoIncrement = true;
                    }

                    if (attr is PrimaryKey)
                    {
                        c.PrimaryKey = true;
                    }

                    if (attr is Encrypt)
                    {
                        c.Encrypt = true;

                        if (c.Length == -1)
                            c.Length = 4096;

                        c.Type = "VARCHAR";
                    }
                }

                if (c.Length == -1)
                {
                    if (c.Type == "INT")
                        c.Length = 255;
                    else
                        c.Length = 512;
                }

                Cache.Add(c);
            }
        }

        public void Init()
        {
            if (ConnectionString == null)
            {
                ConnectionString = DBSyncGlobal.GlobalConnectionString;
            }

            if (EncryptionKey == "")
                EncryptionKey = DBSyncGlobal.GlobalEncryptionKey;

            GenerateTable();
            ReloadData();
        }

        public string GenerateTableCreateSQL()
        {
            string sql = $"CREATE TABLE IF NOT EXISTS {Table} (";

            int i1 = 0;
            foreach (var c in Cache)
            {
                sql += $"`{c.Name}` {c.Type}({c.Length}) NOT NULL {(c.AutoIncrement ? "AUTO_INCREMENT" : "")}";

                if (i1 != Cache.Count - 1)
                    sql += " , ";

                i1++;
            }

            foreach (var c in Cache)
            {
                if (c.PrimaryKey)
                {
                    sql += $" , PRIMARY KEY (`{c.Name}`)";
                }
            }

            sql += " );";

            return sql;
        }
        public void GenerateTable()
        {
            var con = new MySqlConnection(ConnectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = GenerateTableCreateSQL();
            cmd.ExecuteNonQuery();
            con.Close();
        }

        public string GenerateSelectTableSQL()
        {
            string sql = $"SELECT * FROM {Table}";

            return sql;
        }
        public void ReloadData()
        {
            var con = new MySqlConnection(ConnectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = GenerateSelectTableSQL();
            var reader = cmd.ExecuteReader();

            Data = new List<T>();

            while (reader.Read())
            {
                var co = typeof(T).GetConstructors()[0].Invoke(new object[0]);

                foreach (var c in Cache)
                {
                    switch (c.Type)
                    {
                        case "INT":
                            SetProperty(co, c.PropName, reader.GetInt32(c.Name));
                            break;
                        case "VARCHAR":
                            if (c.Encrypt)
                            {
                                SetProperty(co, c.PropName,
                                    DBSyncCrypt.Decrypt(
                                        EncryptionKey,
                                        reader.GetString(c.Name)
                                    )
                                );
                            }
                            else
                            {
                                SetProperty(co, c.PropName, reader.GetString(c.Name));
                            }
                            break;
                        default:
                            SetProperty(co, c.PropName, reader.GetString(c.Name));
                            break;
                    }
                }

                Data.Add((T)co);
            }

            con.Close();
        }

        public string GenerateInsertTableSQL()
        {
            string sql = $"INSERT INTO {Table} (";

            int i = 0;
            foreach (var c in Cache)
            {
                if(!c.AutoIncrement)
                {
                    if (i != Cache.Count - 1)
                        sql += c.Name + ", ";
                    else
                        sql += c.Name;
                }

                i++;
            }

            sql += ") VALUES (";

            i = 0;
            foreach(var c in Cache)
            {
                if(!c.AutoIncrement)
                {
                    if (i != Cache.Count - 1)
                        sql += "@" + c.Name + ", ";
                    else
                        sql += "@" + c.Name;
                }

                i++;
            }

            sql += $");";

            return sql;
        }

        public string GenerateUpdateTableSQL()
        {
            string sql = $"UPDATE {Table} SET ";

            int i = 0;
            foreach(var c in Cache)
            {
                if (i != Cache.Count - 1)
                    sql += c.Name + " = @" + c.Name + ", ";
                else
                    sql += c.Name + " = @" + c.Name;

                i++;
            }

            return sql;
        }

        public int Insert(T value)
        {
            lock(Data)
            {
                var con = new MySqlConnection(ConnectionString);
                con.Open();
                var cmd = con.CreateCommand();

                foreach (var c in Cache)
                {
                    if (c.Encrypt)
                        cmd.Parameters.AddWithValue("@" + c.Name, DBSyncCrypt.Encrypt(EncryptionKey, GetProperty(value, c.PropName).ToString()));
                    else
                        cmd.Parameters.AddWithValue("@" + c.Name, GetProperty(value, c.PropName));
                }

                cmd.CommandText = GenerateInsertTableSQL();
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                var tav = (IDBSyncModel)value;
                var liid = (int)cmd.LastInsertedId;

                tav.Id = liid;
                Data.Add((T)tav);

                con.Close();

                return liid;
            }
        }
        public void Delete(T value)
        {
            var con = new MySqlConnection(ConnectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = $"DELETE FROM {Table} WHERE id={((IDBSyncModel)value).Id};";
            cmd.ExecuteNonQuery();
            con.Close();

            lock(Data)
                Data.Remove(value);
        }

        public void DeleteAll()
        {
            var con = new MySqlConnection(ConnectionString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = $"TRUNCATE TABLE {Table};";
            cmd.ExecuteNonQuery();
            con.Close();

            lock (Data)
                Data.Clear();
        }

        public void SetProperty(object target, string key, object value)
        {
            PropertyInfo prop = target.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                prop.SetValue(target, value, null);
            }
        }
        public object GetProperty(object target, string key)
        {
            PropertyInfo prop = target.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance);
            if (null != prop && prop.CanWrite)
            {
                return prop.GetValue(target, null);
            }
            else
                return null;
        }

        public int IndexOf(T item)
        {
            lock(Data)
            {
                return Data.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            Insert(item);
        }

        public void RemoveAt(int index)
        {
            throw new System.NotImplementedException();
        }

        public void Add(T item)
        {
            Insert(item);
        }

        public void Clear()
        {
            lock(Data)
            {
                DeleteAll();
            }
        }

        public bool Contains(T item)
        {
            lock(Data)
            {
                return Data.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new System.NotImplementedException();
        }

        public bool Remove(T item)
        {
            if (item == null)
                return false;

            lock(Data)
            {
                Delete(item);
                return true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock(Data)
            {
                return Data.GetEnumerator();
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock(Data)
            {
                return Data.GetEnumerator();
            }
        }

        public T Find(Predicate<T> match)
        {
            lock(Data)
            {
                return Data.Find(match);
            }
        }
    }
}
