using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DesktopStreamer
{
    public class MediaPlayer
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public MediaPlayer() : this(string.Empty, string.Empty)
        {
        }

        public MediaPlayer(string name, string path)
        {
            this.Name = name;
            this.Path = path;
        }
    }
}
