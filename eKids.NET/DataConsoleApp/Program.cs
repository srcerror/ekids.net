using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;

namespace DataConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            //RSSdata();

            //AstronomyHAP();

            PuppySearch();
        }

        private static void PuppySearch()
        {
            //JSON + HttpClient

            HttpClient cli = new HttpClient();
            var content = new StringContent("{\"params\":{ \"query\":\"breed_id: '225'\",\"queryParser\":\"structured\",\"searchLogData\":{ \"breed_id\":\"225\"},\"size\":500,\"start\":0,\"sort\":\"search_sort_order asc\"},\"headers\":{ \"content-type\":\"application/json\"}}",
                Encoding.UTF8, "application/json");

            var result = cli.PostAsync("https://www.puppyspot.com/api/search?index=puppy", content).Result;

            var json = result.Content.ReadAsStringAsync().Result;
            var obj = JObject.Parse(json);
            //Console.WriteLine(obj.ToString());

            var hits = obj["result"]["hits"]["hit"] as JArray;
            //Console.WriteLine(hits);
            var puppies = hits.Select(p => new
            {
                Id = p["id"].ToString(),
                Description = p["fields"]["puppyDescription"][0].ToString(),
                Price = p["fields"]["price"][0].ToString()
            });

            foreach (var puppy in puppies)
            {
                Console.WriteLine($"Puppy #{puppy.Id}: (${puppy.Price})");
                Console.WriteLine($"{puppy.Description}");
                Console.WriteLine();
            }
        }

        private static void AstronomyHAP()
        {
            // Astronomy picture of the day: https://apod.nasa.gov/apod/astropix.html
            HtmlWeb html = new HtmlWeb();
            var dox = html.Load("https://apod.nasa.gov/apod/astropix.html");

            var image = dox.DocumentNode.Descendants("img").First();
            string relImage = image.GetAttributeValue("src", "No Image");
            var imageurl = new Uri(new Uri("https://apod.nasa.gov/apod/astropix.html"), relImage);
            Console.WriteLine(imageurl.AbsoluteUri);

            var relparent = image.ParentNode.GetAttributeValue("href", "no link");
            var fullimage = new Uri(new Uri("https://apod.nasa.gov/apod/astropix.html"), relparent);
            Console.WriteLine($"Full Image URL: {fullimage.AbsoluteUri}");

            var links = dox.DocumentNode.Descendants("a").ToList();
            Console.WriteLine($"Found {links.Count} nodes");

            foreach (var item in links)
            {
                Console.WriteLine($"Link = {item.GetAttributeValue("href", "No Link")} : {item.InnerText}");
            }

            var prev = links.Where(x => x.InnerText.Contains("&lt;")).First();
            var rel = prev.GetAttributeValue("href", "");
            var url = new Uri(new Uri("https://apod.nasa.gov/apod/astropix.html"), rel);

            Console.WriteLine($"Previous URL={url.AbsoluteUri}");
        }

        private static void RSSdata()
        {
            var dox = XDocument.Load("https://news.tut.by/rss/index.rss");

            //Console.WriteLine(dox.ToString());

            var items = dox.Descendants("item").Select((x, i) => new { Index = i, Title = x.Element("title").Value, Link = x.Element("link").Value }).ToList();
            var header = new List<string>() { "ID,Title,Link" };

            var strings = items.Select(x => $"{x.Index},\"{x.Title}\",{x.Link}");

            var merged = header.Concat(strings);

            File.WriteAllLines("news.csv", merged);

            //foreach (var item in items)
            //{
            //    var title = item.Element("title").Value;
            //    var link = item.Element("link").Value;

            //    Console.WriteLine($"{title},{link}");
            //}



            //foreach (var item in items)
            //{
            //    Console.WriteLine($"{item.Title} \n >> {item.Link}");
            //}
            //Console.WriteLine($"Total: {items.Count()}");
        }
    }
}
