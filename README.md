## DBSync.Net ##
###A c# library used to syncronice model lists with databases###

####Note:####
This library is still under development. If you use it for production you do this on your own risk

####Features:####

- One or more database server support
- Optional encryption for important data
- Only c# has to be used. No SQL needed
- In-Memory cache
- Threadsafe

####Future Features:####

- Store json objects
- Encrypt json objects
- Connection string builder

####Quick Start####

1) Create a model

```csharp
public class TestyModel : IDBSyncModel
{
   [Key("id")] // This is not optional
   [PrimaryKey]
   [AutoIncrement]
   public int Id { get; set; }

   [Key("text")]
   [Length(324)] // Custom max. length
   public string Text1 { get; set; } // The property name must not be the key name

   [Key("data")]
   [Encrypt] // This encrypts the value with the provided key which can be set in the table properties 
   public string Data { get; set; }
}
```

2) Initialize table

```csharp
var table = new DBSyncTable<TestyModel>("tablename");

table.ConnectionString = "Connection string here"
table.EncryptionKey = "Optional, encryption key";

table.Init(); // This creates the table in the db if its missing and loads data from it to the cache
```

3) Use it

Use can use it basicly like any other IList object.

```csharp
table.Add(new TestyModel()
{
   Text1 = "Hmmmm",
   Data = "Secret"
});

table.Clear();
```

####Additional things:####

You can configure a global connection string and encryption key which will be used if its not set before

```csharp
DBSyncGlobal.ConnectionString = "This will be used on every table if its not set for this table specificly";
DBSyncGlobal.EncryptionKey = "Same as above";
```

Examples can be found in the test project

####Licensing:####
See LICENSE file

####Contact:####


If you have any questions feel free to ask me via:

- Discord: masusniper#0001

- Mail: admin@endelon-hosting.de

- Mail: marcel.kbkm@gmail.com