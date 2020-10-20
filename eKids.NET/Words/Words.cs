using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

namespace eKids
{
    public static class WordsExtensions
    {
        public static IEnumerable<string> FindWords(this TextReader file, bool toLower = false)
        {
            var line = file.ReadLine();
            while (line != null)
            {
                foreach(var word in line.FindWords(toLower))
                {
                    yield return word;
                }

                line = file.ReadLine();
            }
        }

        public static IEnumerable<string> FindWords(this IEnumerable<char> input, bool toLower = false)
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

        public static IEnumerable<string> FindWords(this IEnumerable<string> input, bool toLower = false)
        {
            foreach (var str in input)
            {
                var words = str.FindWords();
                foreach (var word in words)
                    yield return word;
            }
        }

        private static IEnumerable<string> CheckCleanBuilder(this StringBuilder sb, bool toLower)
        {
            if (sb.Length > 0)
            {
                var other = sb.ToString();
                if (toLower) other = other.ToLower();
                 
                sb.Clear();
                yield return other;
            }
        }
        
        public static IEnumerable<string> FindTokens(this IEnumerable<char> input, bool toLower = false)
        {
            var sw = new StringBuilder();
            var ss = new StringBuilder();
            var sp = new StringBuilder();
            foreach (var c in input)
            {
                if (char.IsLetterOrDigit(c)) //word
                {
                    sw.Append(c);

                    var strings = ss.CheckCleanBuilder(toLower).Concat(sp.CheckCleanBuilder(toLower));
                    foreach (var s in strings)
                    {
                        yield return s;
                    }

                }
                else if (char.IsWhiteSpace(c)) // whitespace
                {
                    ss.Append(c);

                    var strings = sw.CheckCleanBuilder(toLower).Concat(sp.CheckCleanBuilder(toLower));
                    foreach (var s in strings)
                    {
                        yield return s;
                    }
                }
                else if (char.IsPunctuation(c)) // punctuation
                {
                    if (c == '-' && sw.Length > 0)
                    {
                        sw.Append(c);
                    }
                    else
                    {
                        sp.Append(c);

                        var strings = sw.CheckCleanBuilder(toLower).Concat(ss.CheckCleanBuilder(toLower));
                        foreach (var s in strings)
                        {
                            yield return s;
                        }
                    }    
                }
                else
                {
                    throw new ArgumentException("Unexpected character = " + c);
                }
            }

            if (sw.Length > 0)
            {
                var word = sw.ToString();
                if (toLower) word = word.ToLower();
                yield return word;
            }
            if (ss.Length > 0)
            {
                var word = ss.ToString();
                if (toLower) word = word.ToLower();
                yield return word;
            }
            if (sp.Length > 0)
            {
                var word = sp.ToString();
                if (toLower) word = word.ToLower();
                yield return word;
            }
        }

        public static string AsString(this IEnumerable<char> input)
        {
            return new string(input.ToArray());
        }

        public static string ToInvariantKey(this string input)
        {
            var clean = input.Where(x => char.IsLetterOrDigit(x));
            return clean.OrderBy(x => x).AsString();
        }
    }
}
