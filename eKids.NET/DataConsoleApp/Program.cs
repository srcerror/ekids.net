using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace DataConsoleApp
{
    class Program
    {
        static void Main(string[] args)
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
