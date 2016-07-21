using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleValueTypeTest
{
    class Program
    {
        delegate void delegate1(int a);
        delegate void delegate2(int a);

        static void Main(string[] args)
        {
            //var keyValuePair1 = new KeyValuePair<int, int>(1, 10);
            //var keyValuePair2 = new KeyValuePair<int, int>(1, 2);
            //var keyValuePair3 = new KeyValuePair<int, string>(1, "123");
            //var keyValuePair4 = new KeyValuePair<string, int>("123", 1);
            //Console.WriteLine(keyValuePair1.GetHashCode());
            //Console.WriteLine(keyValuePair2.GetHashCode());
            //Console.WriteLine(keyValuePair3.GetHashCode());
            //Console.WriteLine(keyValuePair4.GetHashCode());

            //delegate1 d1 = (ab)=>{ };
            //d1 += (ab) => { };
            //delegate1 d2 = (ab) => { };

            //Console.WriteLine(d1.GetHashCode());
            //Console.WriteLine(d2.GetHashCode());

            //var type = new { Name = "123", Test = "123" };

            var str = string.Empty;
            var strBuilder = new StringBuilder();

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                strBuilder.Append(i.ToString());
            }
            var end1 = sw.Elapsed;
            sw.Stop();

            sw.Start();
            for (int i = 0; i < 100000; i++)
            {
                str += i.ToString();
            }
            var end2 = sw.Elapsed;

            Console.WriteLine(end1);
            Console.WriteLine(end2);

            Console.ReadKey();
        }
    }
}
