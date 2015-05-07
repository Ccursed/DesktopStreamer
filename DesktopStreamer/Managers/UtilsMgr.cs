using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DesktopStreamer
{
    class UtilsMgr
    {
        private readonly static Logger logger = new Logger();
        public static Logger Logger
        {
            get { return UtilsMgr.logger; }
        } 

        public static string CheckVersion()
        {
            string retCode = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                retCode = wc.DownloadString(@"http://www.ccursed.net/DesktopStreamerVersion.html");
            }
            catch (Exception ex)
            {
                UtilsMgr.Log(Logger.LogLevel.Error, string.Format("CheckVersion failed. Error: {0}" + ex.Message));
                throw new Exception(string.Format("CheckVersion failed. Error: {0}" + ex.Message));
            }
            return retCode;
        }

        public static void Log(string msg)
        {
            Log(Logger.LogLevel.Warning, msg);
        }

        public static void Log(Logger.LogLevel level, string msg)
        {
            if(!Logger.Write(level, msg))
            {
                MessageBox.Show("Failed to write to log file. This shouldnt happen. Really.", "Log error", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
