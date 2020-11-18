using System;
using System.IO;

using DevExpress.Xpo;
using DevExpress.Xpo.DB;

using Xenial.Licensing.Tests.Domain;

using static Xenial.Tasty;

var connectionString = InMemoryDataStore.GetConnectionStringInMemory(true);
XpoDefault.DataLayer = XpoDefault.GetDataLayer(connectionString, AutoCreateOption.DatabaseAndSchema);
SQLiteConnectionProvider.Register();

var directory = Path.GetDirectoryName(typeof(TrialRequestCommandHandlerTests).Assembly.Location);

var databases = new[]
{
    ("InMemory", connectionString),
    ("Sqlite", SQLiteConnectionProvider.GetConnectionString(Path.Combine(directory, $"{Guid.NewGuid()}.db")))
};

foreach (var (name, cs) in databases)
{
    TrialRequestCommandHandlerTests.Tests(name, cs);
}

return await Run(args);
