using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Timers;

using Timer = System.Timers.Timer;

namespace PlexRequests.CI.Deployment.App
{
    public class Watcher
    {
        private FileSystemWatcher FileWatcher { get; }
        private ConcurrentDictionary<string,DateTime> FilesFound { get; set; }
        private readonly object _lock = new object();
        private Timer Timer { get; set; }
        public Watcher(string pathToWatch)
        {
            FilesFound = new ConcurrentDictionary<string, DateTime>();
            if (!Directory.Exists(pathToWatch))
            { throw new ArgumentException("Path does not exist"); }

            FileWatcher = new FileSystemWatcher(pathToWatch)
            {
                EnableRaisingEvents = true,
            };
            HookupEvents();
            GC.KeepAlive(FileWatcher);
        }

        private void HookupEvents()
        {
            Timer = new Timer(TimeSpan.FromMinutes(5).TotalMilliseconds);
            Timer.Elapsed += TimerOnElapsed;
            FileWatcher.Created += FileWatcherCreated;
            FileWatcher.Changed += FileWatcherCreated;
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            lock (_lock)
            {
                FilesFound = new ConcurrentDictionary<string, DateTime>();
            }
        }

        private void FileWatcherCreated(object sender, FileSystemEventArgs e)
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
                FilesFound.TryAdd(name, DateTime.UtcNow);
                Console.WriteLine("Found PlexRequests.zip");
                Thread.Sleep(TimeSpan.FromMinutes(1));
                Console.WriteLine("Waiting for file to download");
                var d = new Deploy();
                d.DeployApp(e.FullPath);
            }
        }


    }
}