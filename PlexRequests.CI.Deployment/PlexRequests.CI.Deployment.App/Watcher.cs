using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;

namespace PlexRequests.CI.Deployment.App
{
    public class Watcher
    {
        private FileSystemWatcher FileWatcher { get; }
        private Dictionary<string,DateTime> FilesFound { get; set; }
        private readonly object _lock = new object();
        private Timer Timer { get; set; }
        public Watcher(string pathToWatch)
        {
            FilesFound = new Dictionary<string, DateTime>();
            if (!Directory.Exists(pathToWatch))
            { throw new ArgumentException("Path does not exist"); }

            FileWatcher = new FileSystemWatcher(pathToWatch)
            {
                EnableRaisingEvents = true,
            };
            HookupEvents();
        }

        private void HookupEvents()
        {
            Timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            Timer.Elapsed += TimerOnElapsed;
            FileWatcher.Created += FileWatcher_Created;
            FileWatcher.Changed += FileWatcher_Created;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_lock)
            {
                FilesFound = new Dictionary<string, DateTime>();
            }
        }

        private void FileWatcher_Created(object sender, FileSystemEventArgs e)
        {
            DateTime outDt;
            var name = e.Name;
            if (FilesFound.TryGetValue(name, out outDt))
            {
                // We have already processed
                return;
            }
            if (e.Name.Equals("PlexRequests.zip"))
            {
                Console.WriteLine("Found PlexRequests.zip");
                var d = new Deploy();
                d.DeployApp(e.FullPath);
            }
        }


    }
}