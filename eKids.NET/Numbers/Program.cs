using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Numbers
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Fibonacci!");
            var numbers = Fibonacci().Skip(49).Take(1);
            
            foreach(var num in numbers)
            {
                Console.WriteLine(num);
                //Console.ReadLine();
            }
        }

        public static IEnumerable<BigInteger> Fibonacci()
        {
            yield return 1;
            yield return 1;
            BigInteger f = 1;
            BigInteger s = 1;

            while(true)
            {
                yield return f + s;
                var t = f + s;
                s = f;
                f = t; 
            }
        }
    }
}
