using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTableTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var hashTable = new Hashtable();
            hashTable.Add(1, "A");
            hashTable.Add(2, new object());
            hashTable.Add(3, 123);

            foreach (var key in hashTable.Keys)
            {
                Console.WriteLine($"{hashTable[key]} - {hashTable[key].GetType()}");
            }

            Console.ReadKey();
        }
    }
}
