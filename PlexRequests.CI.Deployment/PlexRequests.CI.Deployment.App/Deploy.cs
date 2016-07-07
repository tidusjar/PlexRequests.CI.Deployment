using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;

namespace PlexRequests.CI.Deployment.App
{
    public class Deploy
    {
        private const string DeploymentPath = @"C:\PlexRequests.Net\Deployment\";
        //private const string DeploymentPath = @"C:\Users\Jamie.Rees\Documents\Test\Deployment";
        private const string ZipName = "PlexRequests.zip";
        private string FullZipPath => Path.Combine(DeploymentPath, ZipName);
        public void DeployApp(string zipPath)
        {
            MoveFile(zipPath);
            KillAppProcess();
            Unzip();
            StartApplication();
            DeleteOldZip();
        }

        private void MoveFile(string zipPath)
        {
            Console.WriteLine("Moving File");
            Directory.CreateDirectory(DeploymentPath);
            File.Move(zipPath, FullZipPath);
        }
        private void Unzip()
        {
            Console.WriteLine("Starting to unzip");
            using (var archive = ZipFile.OpenRead(FullZipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    if(entry.FullName.Equals("Release/")) { continue; }

                    var fullname = string.Empty;
                    if (entry.FullName.Contains("Release/")) // Don't extract the release folder, we are already in there
                    {
                        fullname = entry.FullName.Replace("Release/", string.Empty);
                    }

                    var fullPath = Path.Combine(DeploymentPath, fullname);

                    if (string.IsNullOrEmpty(entry.Name))
                    {
                        Directory.CreateDirectory(fullPath);
                    }
                    else
                    {
                        entry.ExtractToFile(fullPath, true);
                        Console.WriteLine("Deployed {0}", entry.FullName);
                    }
                }
            }
        }

        private void KillAppProcess()
        {
            Console.WriteLine("Killing current PlexRequests process");
            var process = Process.GetProcessesByName("PlexRequests");
            foreach (var p in process)
            {
                try
                {
                    Console.WriteLine($"Killing {p.ProcessName}");
                    p.Kill();
                }
                catch (Win32Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }
        }

        private void StartApplication()
        {
            Console.WriteLine("Starting new version");
            var info = new ProcessStartInfo(Path.Combine(DeploymentPath, "PlexRequests.exe"))
            {
                Arguments = "-p 8099"
            };
            Process.Start(info);
        }

        private void DeleteOldZip()
        {
            Console.WriteLine("Deleting .zip");
            File.Delete(FullZipPath);
        }
    }
}