using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

namespace CodeChallenge
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 6)
            {
                Console.WriteLine("Merging Catalog A with Catalog B...");
                var catalogManager = new InMemoryCatalogManager(args[0], args[1], args[2]);
                catalogManager.Merge(args[3], args[4], args[5]);
                catalogManager.ExportCatalog(args[6]);
            }

            Console.WriteLine("Enter to exit...");
            Console.ReadLine();
        }
    }
}
