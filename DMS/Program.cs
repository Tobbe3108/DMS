using System;
using System.Threading.Tasks;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;

namespace DMS
{
  internal static class Program
  {
    private const string RemoteConnectionString = "Server=.;Database=AdventureWorks;Trusted_Connection=True;"; //Kunde db
    private const string PrivateCloudConnectionString = "Server=.;Database=Client;Trusted_Connection=True;"; // Private cloud db

    private static async Task Main()
    {
      var tables = new[]
      {
        "ProductCategory", "ProductModel", "Product",
        "Address", "Customer", "CustomerAddress", "SalesOrderHeader", "SalesOrderDetail"
      };

      var syncSetup = new SyncSetup(tables);

      syncSetup.Tables["Customer"].Columns.AddRange(new[]
      {
        "CustomerID", "NameStyle", "FirstName", "LastName"
      });
      
      syncSetup.Tables["Address"].Columns.AddRange(new[]
      {
        "AddressID", "AddressLine1", "City", "PostalCode"
      });
      
      foreach (var table in tables)
      {
        syncSetup.Tables[table].SyncDirection = SyncDirection.DownloadOnly;
      }

      var syncOptions = new SyncOptions
      {
        UseBulkOperations = true,
        UseVerboseErrors = true
      };

      var remoteProvider = new SqlSyncChangeTrackingProvider(RemoteConnectionString);
      var privateCloudProvider = new SqlSyncChangeTrackingProvider(PrivateCloudConnectionString);

      var localOrchestrator = new LocalOrchestrator(privateCloudProvider, syncOptions, syncSetup);
      
      // Call when column filter changed
      // await localOrchestrator.DeprovisionAsync();
      
      var progress = new SynchronousProgress<ProgressArgs>(args =>
        Console.WriteLine($"{args.PogressPercentageString}:\t{args.Source}:\t{args.Message}"));
      
      var agent = new SyncAgent(privateCloudProvider, remoteProvider, syncOptions, syncSetup);
      
      // Launch the sync process
      var s1 = await agent.SynchronizeAsync(progress);
      // Write results
      Console.WriteLine(s1);
    }
  }
}