using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace WordProcessing
{
    class Program
    {
        private const string _filename = "rhit1_utf8.txt";

        static void Main(string[] args)
        {
            Console.WriteLine($"{Environment.CurrentDirectory}");
            var relpath = @"..\..\..\..\..\input";
            var path = Path.Combine(relpath, _filename);
            var exists = File.Exists(path);
            Console.WriteLine($"Hello World! File Exists = {exists}");

            #region 1
            /*
            var all = File.ReadAllText(path);

            Console.WriteLine($"File contains {all.Length} characters");

            */
            #endregion

            #region 2
            /*
            var letters = all.Where(x => char.IsLetterOrDigit(x));
            Console.WriteLine($"File contains {letters.Count()} letters");
            
            */
            #endregion

            #region 3
            /*
            var punctuation = all.Where(x => !(char.IsLetterOrDigit(x) || char.IsWhiteSpace(x)));
            var punct = all.Where(x => char.IsPunctuation(x));

            Console.WriteLine($"File contains {punctuation.Count()} punctuation chars ({punct.Count()})");

            var pun = punctuation.Distinct().ToArray();
            Console.WriteLine($"All punctuation: \n{string.Join(' ', pun)}");
            */
            #endregion

            var words = FindWords(File.ReadAllText(path), true).Take(100);
            foreach(var word in words)
            {
                Console.Write(word + " ");
            }
        }

        public static IEnumerable<string> FindWords(string input, bool toLower = false)
        {
            if (string.IsNullOrEmpty(input)) return new List<string>();
            
            var sb = new StringBuilder();
            bool isWord = char.IsLetterOrDigit(input[0]);
            var result = new List<string>();
            foreach(var c in input)
            {
                if (char.IsLetterOrDigit(c)) //word
                {
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

            return result;
        }
    }
}
