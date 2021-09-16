using System;
using Dotmim.Sync;
using Dotmim.Sync.Enumerations;
using Dotmim.Sync.SqlServer;

namespace DMS
{
  public class SyncAgentLocal
  {
    public Dotmim.Sync.SyncAgent CreateAgent(string remoteConnectionString, string privateCloudConnectionString, string defaultValue)
    {
      var tables = new[]
      {
        "Address"
      };

      var syncSetup = new SyncSetup(tables);

      /*syncSetup.Tables["Customer"].Columns.AddRange(new[]
      {
        "CustomerID", "NameStyle", "FirstName", "LastName"
      });

      syncSetup.Tables["Address"].Columns.AddRange(new[]
      {
        "AddressID", "AddressLine1", "City", "PostalCode"
      });*/

      foreach (var table in tables)
      {
        syncSetup.Tables[table].SyncDirection = SyncDirection.DownloadOnly;
      }

      var syncOptions = new SyncOptions
      {
        UseBulkOperations = true,
        UseVerboseErrors = true
      };

      var remoteProvider = new SqlSyncChangeTrackingProvider(remoteConnectionString);
      var privateCloudProvider = new SqlSyncChangeTrackingProvider(privateCloudConnectionString);

      var localOrchestrator = new LocalOrchestrator(privateCloudProvider, syncOptions, syncSetup);
      localOrchestrator.OnTableChangesApplying(args =>
      {
        /*args.Changes.Columns.Add(new SyncColumn()
        {
          ColumnName = "tenantId",
          DataType = "int",
          AllowDBNull = false,
        });*/

        Console.WriteLine("TEST!");
        
        // foreach (var row in args.Changes.Rows)
        // {
        //   row["tenantId"] = defaultValue;
        // }
      });
      
      localOrchestrator.OnTableChangesApplied(args => { Console.WriteLine("Test"); });
      
      // Call when column filter changed
      localOrchestrator.DeprovisionAsync().Wait();

      return new SyncAgent(privateCloudProvider, remoteProvider, syncOptions, syncSetup);
    }
  }
}