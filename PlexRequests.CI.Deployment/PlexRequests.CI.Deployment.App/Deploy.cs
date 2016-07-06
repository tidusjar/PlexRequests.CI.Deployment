using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace PlexRequests.CI.Deployment.App
{
    public class Deploy
    {
        //private const string DeploymentPath = @"C:\PlexRequests.Net\Deployment\";
        private const string DeploymentPath = @"C:\Users\Jamie.Rees\Documents\Test\Deployment";
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
            Directory.CreateDirectory(DeploymentPath);
            File.Move(zipPath, FullZipPath);
        }
        private void Unzip()
        {
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
            var process = Process.GetProcessesByName("PlexRequests.exe");
            process.FirstOrDefault()?.Kill();
        }

        private void StartApplication()
        {
            var info = new ProcessStartInfo(Path.Combine(DeploymentPath, "PlexRequests.exe"))
            {
                Arguments = "-p 8095"
            };
            Process.Start(info);
        }

        private void DeleteOldZip()
        {
            File.Delete(FullZipPath);
        }
    }
}