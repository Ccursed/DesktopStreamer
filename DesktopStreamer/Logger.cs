using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DesktopStreamer
{
    public class Logger
    {
        public enum LogLevel
        {
            Info, Warning, Error
        };

        private readonly string curLogPath;

        public Logger()
        {
            curLogPath = FileMgr.LogDirectory + @"\" + DateTime.UtcNow.Ticks + @".log";
            File.Create(curLogPath);
        }

        public bool Write(LogLevel level, string msg)
        {
            try
            {
                File.AppendText(string.Format("[{0,7}] {1}", level.ToString(), msg));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void CleanUp()
        {
            if (string.IsNullOrEmpty(File.ReadAllText(curLogPath))) File.Delete(curLogPath);
        }
    }
}
