using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LMPT.DB
{
    public class ConfigurationProvider
    {
        public ConfigurationProvider()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            var path = home + @"/Library/Application Support/liveme-pro-tools";
            RootDirectory = new DirectoryInfo(path);


            DbFile = FromRoot("lmpt.db");
            OldWatchedJsonFile = FromRoot("watched.json");
            OldBookmarksJsonFile = FromRoot("bookmarks.json");
            OldProfileJsonFile = FromRoot("profiles.json");
        }

        public DirectoryInfo RootDirectory { get; set; }
        public string DbFile { get; internal set; }
        public string OldWatchedJsonFile { get; internal set; }
        public string OldBookmarksJsonFile { get; internal set; }
        public string OldProfileJsonFile { get; set; }

        private string FromRoot(string relativeFile)
        {
            return Path.Combine(RootDirectory.ToString(), relativeFile);
        }
    }
}