using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocGen
{
    class Program
    {
        static void Main(string[] args)
        {
            DocLoader loader = new DocLoader();
            loader.Load("DocGen");
            Generator generator = new Generator();
            generator.Generate("output", loader);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
