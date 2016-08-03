using System;

namespace PlexRequests.CI.Deployment.App
{
    public class ConsoleHelper
    {
        public static void WriteLine(string text)
        {
            Console.WriteLine($"{DateTime.Now.ToString("t")} {text}");
        }
    }
}