using Contentful.Core;
using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;

namespace ContentfulApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var client = new ContentfulManagementClient(httpClient, "CFPAT-DNgNQ2uTdHjgUr1gf1t2bEs-Hml3_IGDPdsc-ptruf4", "vcqyye6xiw3a");

            var path = LocateFile("book1\\book1.txt");
            Console.WriteLine(path);

            int page = 1;
            string text = "";
            foreach (var str in System.IO.File.ReadAllLines(path))
            {
                if (str.Trim() == page.ToString())
                {
                    // create page
                    if (page > 1)
                        CreateEntry(client, page-1, text);
                    page++;
                    text = "";
                }
                else
                {
                    text += "\n" + str;
                }
            }
        }

        private static string LocateFile(string filename)
        {
            Debug.WriteLine($"Looking for {filename} from current directory: {Environment.CurrentDirectory}");

            // check current directory
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in current directory = {Environment.CurrentDirectory}");
                return path;
            }

            // check Visual Studio relative path
            var relpathVS = @"..\..\..\..\..\input";
            path = Path.Combine(relpathVS, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio relative path to input folder");
                return path;
            }

            //check Visual Studio Code relative path
            var relpathVSC = @"..\..\input";
            path = Path.Combine(relpathVSC, filename);
            if (System.IO.File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio Code relative path to input folder");
                return path;
            }
            throw new FileNotFoundException($"File {filename} was not found");
        }

        private static void CreateEntry(ContentfulManagementClient client, int page, string text)
        {
            var entry = new Entry<dynamic>();
            entry.SystemProperties = new SystemProperties();
            entry.SystemProperties.Id = $"pagen{page}";

            entry.Fields = new
            {
                Number = new Dictionary<string, int>()
                {
                    { "ru", page },
                },
                Text = new Dictionary<string, string>()
                {
                    { "ru", text }
                }
            };

            var newEntry = client.CreateOrUpdateEntry(entry, contentTypeId: "page", version: 1).Result;
            client.PublishEntry(newEntry.SystemProperties.Id, newEntry.SystemProperties.Version.Value);
            Console.WriteLine($"Entry created #{page}");
            Thread.Sleep(1000);
        }
    }
}
