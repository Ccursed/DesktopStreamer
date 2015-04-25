using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;
using System.Threading.Tasks;
using StreamHostApi;
using System.ComponentModel;
using System.Windows.Media.Imaging;

namespace DesktopStreamer
{
    public delegate void DescriptionChangedHandler(string description);

    [Serializable]
    public class Favorite : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event DescriptionChangedHandler onDescriptionChanged;

        public enum Status
        {
            PENDING, ONLINE, OFFLINE, DOWNLOADING
        };

        #region Properties

        private long id;
        public long Id
        {
            get { return id; }
            set { id = value; }
        }

        private bool isDeleted;
        public bool IsDeleted
        {
            get { return isDeleted; }
            set { isDeleted = value; }
        }

        private string url;
        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private int listPosition;
        public int ListPosition
        {
            get { return listPosition; }
            set { listPosition = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            set { title = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("title")); }
        }

        [NonSerialized]
        private bool connected;
        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        [NonSerialized]
        private StreamObject srcStream;
        public StreamObject SrcStream
        {
            get { return srcStream; }
            set { srcStream = value; }
        }

        [NonSerialized]
        private ChannelObject srcChannel;
        public ChannelObject SrcChannel
        {
            get { return srcChannel; }
            set { srcChannel = value; }
        }

        [NonSerialized]
        private Status state;
        public Status State 
        {
            get { return state; } 
            set { state = value; updateStatus(); } 
        }

        [NonSerialized]
        private ImageSource logo;
        public ImageSource Logo 
        { 
            get { return logo; }
            set { logo = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("logo")); }
        }

        [NonSerialized]
        private string description;
        public string Description 
        {
            get { return description; }
            set { description = value; if(onDescriptionChanged != null) onDescriptionChanged(description); } 
        }

        [NonSerialized]
        private ImageSource statusImage;
        public ImageSource StatusImage 
        {
            get { return statusImage; }
            set { statusImage = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("statusImage")); } 
        }
        #endregion

        #region Minor interaction logics
        
        private void updateStatus()
        {
            switch (state)
            {
                case Status.PENDING: StatusImage = new BitmapImage(new Uri(@"../Resources/statusPending.png", UriKind.Relative)); break;
                case Status.ONLINE: StatusImage = new BitmapImage(new Uri(@"../Resources/statusOnline.png", UriKind.Relative)); break;
                case Status.OFFLINE: StatusImage = new BitmapImage(new Uri(@"../Resources/statusOffline.png", UriKind.Relative)); break;
                case Status.DOWNLOADING: StatusImage = new BitmapImage(new Uri(@"../Resources/statusDownload.png", UriKind.Relative)); break;
                default: StatusImage = new BitmapImage(new Uri(@"../Resources/statusOffline.png", UriKind.Relative)); break;
            }
        }
        #endregion
    }
}
