using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using DesktopStreamer.UIElements;

namespace DesktopStreamer
{
    class UtilsMgr
    {
        #region Properties

        private const string ccursedUrl = @"http://www.ccursed.net";

        private readonly static Logger logger = new Logger();
        public static Logger Logger
        {
            get { return UtilsMgr.logger; }
        }  
        #endregion

        public static string CheckVersion()
        {
            string retCode = string.Empty;
            try
            {
                WebClient wc = new WebClient();
                retCode = wc.DownloadString(@"http://www.ccursed.net/DesktopStreamerVersion.html");
                string currentVersion = Properties.Settings.Default.Version;
                if(retCode != currentVersion)
                {
                    Message("New Version!", "There is a new version available!\nCheck it out at", ccursedUrl);
                }
            }
            catch (Exception ex)
            {
                UtilsMgr.Log(Logger.LogLevel.Error, string.Format("CheckVersion failed. Error: {0}" + ex.Message));
                throw new Exception(string.Format("CheckVersion failed. Error: {0}" + ex.Message));
            }
            return retCode;
        }

        public static void Message(string title, string msg)
        {
            Message(title, msg, null);
        }

        public static void Message(string title, string msg, string link)
        {
            CcursedMessageBox msgbox = new CcursedMessageBox(title, msg, link);
            msgbox.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            msgbox.Show();
        }

        public static void Log(string msg)
        {
            Log(Logger.LogLevel.Warning, msg);
        }

        public static void Log(Logger.LogLevel level, string msg)
        {
            if(Logger != null)
            {
                if (!Logger.Write(level, msg))
                {
                    MessageBox.Show("Failed to write to log file. This shouldnt happen. Really.", "Log error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }
    }
}
