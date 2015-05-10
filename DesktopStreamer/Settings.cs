using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace DesktopStreamer
{
    [XmlRootAttribute("Settings", Namespace="ccursed.net")]
    public class Settings
    {
        private bool startOnDragIn;
        public bool StartOnDragIn
        {
            get { return startOnDragIn; }
            set { startOnDragIn = value; }
        }

        private bool allowForeignLinks;
        public bool AllowForeignLinks
        {
            get { return allowForeignLinks; }
            set { allowForeignLinks = value; }
        }

        private int delayTime;
        public int DelayTime
        {
            get { return delayTime; }
            set { delayTime = value; }
        }

        private List<MediaPlayer> players;
        public List<MediaPlayer> Players
        {
            get { return players; }
            set { players = value; }
        }

        #region Minor interaction logic

        #endregion
    }
}
