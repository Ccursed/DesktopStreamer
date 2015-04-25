using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopStreamer
{
    class SettingsManager
    {
        #region Enums

        public enum MediaPlayers
        {
            Vlc, WindowsMediaPlayer
        };
        #endregion

        #region Properties

        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public Dictionary<string, MediaPlayer> dctMediaPlayer; 
        #endregion

        public SettingsManager()
        {
            dctMediaPlayer = new Dictionary<string, MediaPlayer>();
        }

        public void ScanForPlayers()
        {
            foreach (MediaPlayers player in Enum.GetValues(typeof(MediaPlayers)))
            {
                switch (player)
                {
                    case MediaPlayers.Vlc: ScanForVlc(); break;
                    case MediaPlayers.WindowsMediaPlayer: ScanForWMP(); break;
                    default: break;
                }
            }
        }

        private void ScanForVlc()
        {
            string path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\VIDEOLAN\VLC", "InstallDir", null);
            if (path == null) path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\VIDEOLAN\VLC", "InstallDir", null);
            if (path == null) return;
            MediaPlayer player = new MediaPlayer(MediaPlayers.Vlc.ToString(), path);
            dctMediaPlayer.Add(player.Name, player);
        }

        private void ScanForWMP()
        {

        }
    }
}
