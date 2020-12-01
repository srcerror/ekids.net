using Microsoft.Azure.Cosmos.Table;
using System;
using System.Threading;

namespace StorageApp
{
    public class VDIPerfData : TableEntity
    {
        public VDIPerfData(string user)
        {
            this.RowKey = DateTime.Now.ToFileTime().ToString();
            this.PartitionKey = user;
            this.Created = DateTime.Now;
        }

        public VDIPerfData() {}

        public string Hostname { get; set; }

        public string Latency { get; set; }

        public DateTime Created { get; set; }

        public string OS { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            // https://www.c-sharpcorner.com/article/azure-storage-tables/
            var account = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=ekidsstore;AccountKey=uxNH0KNnugoYsjgbNYJ9uY16vSYYwoVyHYyNcQbTdtNSz/h8aTWtt80mZZOYWOITGDe2ng2bEDsSbTXh2p7ZRg==;EndpointSuffix=core.windows.net");
            var cli = account.CreateCloudTableClient();
            var table = cli.GetTableReference("vdiperformance");
            table.CreateIfNotExists();

            for (int i = 0; i < 10; i++)
            {
                var entity = new VDIPerfData("Ivan Kirkorau")
                {
                    Hostname = Environment.MachineName,
                    Latency = (200 + i).ToString(),
                    OS = Environment.OSVersion.VersionString
                };
                var insert = TableOperation.Insert(entity);
                table.Execute(insert);
                Console.WriteLine($"Inserted {entity.PartitionKey} Latency={entity.Latency} at {entity.Created}");
                Thread.Sleep(500);
            }
            

        }
    }
}
