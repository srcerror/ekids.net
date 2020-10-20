using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using eKids;
using System.Diagnostics;

namespace WordProcessing
{
    class Program
    {
        private const string _filepath = "rhit1_utf8.txt";

        static void Main(string[] args)
        {
            //TestFindWords();

            // Anagramma lookup
            // Load Dictionary
            var path = LocateFile("russian.dic");
            // as words
            using (var file = new StreamReader(path, Encoding.UTF8))
            {
                var words = file.FindWords(true);
                // create invariant keys

                foreach (var word in words.Take(10))
                {
                    Console.WriteLine(word.ToInvariantKey());
                }

                var key = "апельсин".ToInvariantKey();
                foreach (var word in words)
                {
                    if (key == word.ToInvariantKey())
                        Console.WriteLine(word);
                }
            }



        }

        private static string LocateFile(string filename)
        {
            Debug.WriteLine($"Looking for {filename} from current directory: {Environment.CurrentDirectory}");

            // check current directory
            var path = Path.Combine(Environment.CurrentDirectory, filename);
            if (File.Exists(path))
            {
                Debug.WriteLine($"Found in current directory = {Environment.CurrentDirectory}");
                return path;
            }

            // check Visual Studio relative path
            var relpathVS = @"..\..\..\..\..\input";
            path = Path.Combine(relpathVS, filename);
            if (File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio relative path to input folder");
                return path;
            }

            //check Visual Studio Code relative path
            var relpathVSC = @"..\..\input";
            path = Path.Combine(relpathVSC, filename);
            if (File.Exists(path))
            {
                Debug.WriteLine($"Found in Visual Studio Code relative path to input folder");
                return path;
            }
            throw new FileNotFoundException($"File {filename} was not found");
        }

        private static void TestFindWords()
        {
            Console.WriteLine($"{Environment.CurrentDirectory}");
            var relpath = @"..\..\..\..\..\input";
            var path = Path.Combine(relpath, _filepath);
            var exists = File.Exists(path);

            if (!exists)
            {
                // VS Code running in the same directory
                relpath = @"..\..\input";
                path = Path.Combine(relpath, _filepath);
                exists = File.Exists(path);
            }
            Console.WriteLine($"File Exists = {exists}");

            var all = File.ReadAllText(path);
            var lines = File.ReadAllLines(path);
            Console.WriteLine($"File contains {all.Length} characters");

            // TestWordProcessing(all);

            var words = all.FindWords(toLower: true);
            var wordsAll = lines.FindWords(false);
            Console.WriteLine($"Количество найденных слов = {words.Distinct().Count()} ({wordsAll.Distinct().Count()})");
        }

        private static void TestWordProcessing(string all)
        {
            string pathd = @"c:\Code\ekids\repo\README.md";
            string path1 = @"c:\Code\ekids\repo\";
            string path2 = @"c:\Code\ekids\repo";
            string fname = "README.md";
            string path3 = Path.Combine(path1, fname);
            var b = File.Exists(path3);
            string path4 = Path.Combine(path2, fname);
            b = File.Exists(path4);
            string path5 = Path.Combine(pathd, fname);
            b = File.Exists(path5);



            // space count
            int count = 0;
            foreach (var ch in all)
            {
                if (ch == ' ')
                {
                    count++;
                }
            }
            Console.WriteLine($"Number of spaces == {count}");

            // LINQ. IEnumerable<char>
            // Filter: Where 
            // Lambda: t => t.isGood()

            // Count()

            count = all.Where(t => t == ' ').Count();
            Console.WriteLine($"Number of spaces == {count}");

            count = all.Count(t => t == ' ');

            var letters = all.Where(x => char.IsLetterOrDigit(x));
            Console.WriteLine($"File contains {letters.Count()} letters");

            var punctuation = all.Where(x => !(char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)));
            var punct = all.Where(x => char.IsPunctuation(x));

            Console.WriteLine($"File contains {punctuation.Count()} punctuation chars ({punct.Count()})");

            var pun = punctuation.Distinct().ToArray();
            Console.WriteLine($"All punctuation: \n{string.Join(' ', pun)}");

            var pf = new Dictionary<char, int>();
            foreach (var ch in punct)
            {
                if (pf.ContainsKey(ch))
                {
                    pf[ch] = pf[ch] + 1;
                }
                else
                {
                    pf[ch] = 1;
                }
            }
            foreach (var kv in pf)
            {
                Console.WriteLine($"[{kv.Key}] = {kv.Value}");
            }

            // LINQ
            // Sort: OrderBy, OrderByDescending
            foreach (var kv in pf.OrderByDescending(t => t.Value))
            {
                Console.WriteLine($"[{kv.Key}] = {kv.Value}");
            }

            Console.WriteLine("Words processing ------------------------------");

            var allwords = FindWords(all, true);

            var words = allwords.Where(t => t.Contains('-'));

            foreach (var word in words)
            {
                Console.Write(word + " ");
            }
            Console.WriteLine();

            var wfreq = new Dictionary<string, int>();
            foreach (var w in allwords)
            {
                if (wfreq.ContainsKey(w)) wfreq[w] += 1;
                else wfreq[w] = 1;
            }

            // LINQ Transformation: Select
            words = wfreq.OrderByDescending(kv => kv.Value).Select(kv => kv.Key).Where(w => w.Length > 0).Take(50);
            foreach (var word in words)
            {
                Console.WriteLine($"[{word}] = {wfreq[word]}");
            }

            // Longest word
            var longest = allwords.OrderByDescending(w => w.Length).Take(50);

            foreach (var word in longest)
            {
                Console.WriteLine($"{word}");
            }
        }

        public static IEnumerable<string> FindWords(string input, bool toLower = false)
        {
            if (string.IsNullOrEmpty(input)) return new List<string>();
            
            var sb = new StringBuilder();
            var result = new List<string>();
            foreach(var c in input)
            {
                if (char.IsLetterOrDigit(c)) //word
                {
                    sb.Append(c);
                }
                else if (c == '-' && sb.Length > 0)
                {
                    // привет-медвед 
                    sb.Append(c);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        // add word
                        var word = sb.ToString();
                        if (toLower) word = word.ToLower();
                        result.Add(word);
                        sb.Clear();
                    }
                }
            }
            if (sb.Length > 0)
            {
                var word = sb.ToString();
                if (toLower) word = word.ToLower();
                result.Add(word);
            }

            return result;
        }
        
        // yield return
        public static IEnumerable<string> FindWords2(string input, bool toLower = false)
        {
            var sb = new StringBuilder();
            foreach (var c in input)
            {
                if (char.IsLetterOrDigit(c)) //word
                {
                    sb.Append(c);
                }
                else if (c == '-' && sb.Length > 0)
                {
                    // привет-медвед 
                    sb.Append(c);
                }
                else
                {
                    if (sb.Length > 0)
                    {
                        // add word
                        var word = sb.ToString();
                        if (toLower) word = word.ToLower();
                        yield return word;
                        sb.Clear();
                    }
                }
            }
            if (sb.Length > 0)
            {
                var word = sb.ToString();
                if (toLower) word = word.ToLower();
                yield return word;
            }
        }
    }
}
