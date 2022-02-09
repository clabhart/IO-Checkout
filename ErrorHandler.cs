using System;
using System.Globalization;
using System.IO;

namespace IOCheckoutTool
{
    internal class ErrorHandler : IDisposable
    {
        public string LogPath { get; set; }
        private readonly StreamWriter ErrorLog;
        private readonly DirectoryInfo Project;

        public ErrorHandler(DirectoryInfo directory)
        {
            Project = directory;
            if (File.GetLastWriteTime(Path.Combine(Project.FullName, "Error Log.txt")).Date.ToString("d", CultureInfo.CurrentCulture) != DateTime.Today.ToString("d", CultureInfo.CurrentCulture) || !File.Exists(Path.Combine(Project.FullName, "Error Log.txt")))
            {
                ErrorLog = new StreamWriter(Path.Combine(Project.FullName, "Error Log.txt"), append: true)
                {
                    AutoFlush = true
                };
                ErrorLog.WriteLine(string.Concat("===============================================================", DateTime.Today.ToString("d", CultureInfo.CurrentCulture), "================================================================="));
            }
            else
            {
                ErrorLog = new StreamWriter(Path.Combine(Project.FullName, "Error Log.txt"), append: true)
                {
                    AutoFlush = true
                };
            }
            LogPath = ((FileStream)ErrorLog.BaseStream).Name;
        }

        public void LogError(string error)
        {
            string logmessage = string.Concat(DateTime.Now, ": ", error);
            ErrorLog.WriteLine(logmessage);
        }

        public void Dispose()
        {
            ErrorLog.Close();
            ErrorLog.Dispose();
        }
    }
}