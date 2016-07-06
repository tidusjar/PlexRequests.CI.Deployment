using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlexRequests.CI.Deployment.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var watcher = new Watcher(args[0]);
            
            Console.WriteLine("Watching");
            Console.ReadLine();
        }
    }
}
