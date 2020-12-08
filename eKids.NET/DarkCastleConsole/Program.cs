using Contentful.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace DarkCastleConsole
{
    class Program
    {
        static public readonly Dictionary<int, string> Gamebook = new Dictionary<int, string>();

        static void Main(string[] args)
        {
            var path = LocateFile("book1\\book1.txt");
            int page = 1;
            string text = "";
            foreach (var str in System.IO.File.ReadAllLines(path))
            {
                if (str.Trim() == page.ToString())
                {
                    // create page
                    if (page > 1)
                        Gamebook[page - 1] = text;
                    page++;
                    text = "";
                }
                else
                {
                    text += "\n" + str;
                }
            }
            // preparations complete.

            

            page = 1;
            // Game Cycle
            while (true)
            {
                try
                {
                    // clear 
                    Console.Clear();

                    // draw
                    
                    // Local file
                    var pagetext = Gamebook[page];

                    // Contentful backend
                    //var pagetext = GetPage(page);
                    PrintText(pagetext);

                    PrintBackpack();

                    // input

                    Console.WriteLine();
                    var current = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Номер страницы > ");
                    string input = Console.ReadLine();
                    Console.ForegroundColor = current;

                    // check for exit
                    if (input == "q")
                        break;

                    var newpage = int.Parse(input);
                    if (newpage > 0 && newpage < 617)
                        page = newpage;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }
            }

        }

        static string GetPage(int page)
        {
            var httpClient = new HttpClient();
            var client = new ContentfulClient(httpClient, "FGKyGKBu15Kz4ytUSIum2SLDQfv6hbMsc3gl_n0D-wM", "0EFgNbaHNldoyVEX1CTxkpKFhiHn1esLcLYiI6M2mZc", "vcqyye6xiw3a");

            var entry = client.GetEntry<dynamic>("pagen" + page).Result;

            return entry.text;
        }

        static void PrintBackpack(int startlen = 100)
        {
            int top = Console.CursorTop;
            int left = Console.CursorLeft;
            var current = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Blue;

            // ################ РЮКЗАК #################
            int line = 1;
            Console.CursorTop = line;
            Console.CursorLeft = startlen + 5;
            Console.Write("################ РЮКЗАК #################");
            line++;
            for (int i = 0; i < 10; i++)
            {
                Console.CursorTop = line++;
                Console.CursorLeft = startlen + 5;
                Console.Write("#                                       #");
            }
            // #                                       #
            // #                                       #
            // #                                       #
            // ...
            Console.CursorTop = line;
            Console.CursorLeft = startlen + 5;
            Console.Write("#########################################");
            // #########################################

            Console.CursorTop = top;
            Console.CursorLeft = left;
            Console.ForegroundColor = current;
        }

        static void PrintText(string text, int maxlen = 100)
        {
            var reader = new StringReader(text);
            while (true)
            {
                var txt = reader.ReadLine();
                if (txt == null) break;
                while (txt.Length > maxlen)
                {
                    for (int i = maxlen - 1; i > 0; i--)
                    {
                        if (!char.IsLetterOrDigit(txt[i]))
                        {
                            var sub = txt.Substring(0, i + 1);
                            Console.WriteLine(sub);
                            txt = txt.Substring(i + 1);
                            break;
                        }
                    }
                }
                Console.WriteLine(txt);
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
    }
}
