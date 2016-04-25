using System;

namespace Mironiasty.Random.Rownloglerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var mtao = new ManyTaskAtOnce();
            mtao.Start(10, 4);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
    }
}
