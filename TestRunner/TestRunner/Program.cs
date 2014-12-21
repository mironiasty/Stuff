using System;
using System.Reflection;

namespace TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            var runner = new Runner(Assembly.GetExecutingAssembly());

            var test = runner.RunTests();
            foreach (var result in test)
            {
                Console.WriteLine(result);
            }

            Console.ReadKey();
        }
    }
}
