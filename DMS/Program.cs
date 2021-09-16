using System;
using System.Threading.Tasks;
using Dotmim.Sync;

namespace DMS
{
  internal static class Program
  {
    private const string RemoteConnectionString1 = "Server=.;Database=AdventureWorks;Trusted_Connection=True;"; //Kunde1 db
    private const string RemoteConnectionString2 = "Server=.;Database=AdventureWorks2;Trusted_Connection=True;"; //Kunde2 db
    private const string PrivateCloudConnectionString = "Server=.;Database=Client;Trusted_Connection=True;"; // Private cloud db

    private static async Task Main()
    {
      var syncAgent1 = new SyncAgentLocal().CreateAgent(RemoteConnectionString1, PrivateCloudConnectionString, "1");
      var syncAgent2 = new SyncAgentLocal().CreateAgent(RemoteConnectionString2, PrivateCloudConnectionString, "2");

      var progress = new SynchronousProgress<ProgressArgs>(args =>
        Console.WriteLine($"{args.PogressPercentageString}:\t{args.Source}:\t{args.Message}"));
      
      // Launch the sync process
      var s1 = await syncAgent1.SynchronizeAsync(progress);
      // Write results
      Console.WriteLine(s1);

      // Launch the sync process
      var s2 = await syncAgent2.SynchronizeAsync(progress);
      // Write results
      Console.WriteLine(s2);
    }
  }
}