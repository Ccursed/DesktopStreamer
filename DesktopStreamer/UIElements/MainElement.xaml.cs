using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DesktopStreamer
{
    public delegate void BtnWatchHandler(object sender, RoutedEventArgs e, string link);
    public delegate void WatchUrlIndexHandler();

    /// <summary>
    /// Interaktionslogik für MainElement.xaml
    /// </summary>
    public partial class MainElement : UserControl
    {
        public event BtnWatchHandler onButtonWatchClicked;
        public event WatchUrlIndexHandler onWatchedIndexChanged;

        public string SrcLink
        {
            get { return txtLink.Text; }
            set { txtLink.Text = value; }
        }

        private List<string> watchedUrls;
        public List<string> WatchedUrls
        {
            get { return watchedUrls; }
            set { watchedUrls = value; }
        }

        private int watchedUrlsIndex = -1;
        public int WatchedUrlsIndex
        {
            get { return watchedUrlsIndex; }
            set { watchedUrlsIndex = value; if (onWatchedIndexChanged != null) onWatchedIndexChanged(); }
        }

        public MainElement()
        {
            InitializeComponent();
            watchedUrls = new List<string>();
            onWatchedIndexChanged += MainElement_onWatchedIndexChanged;
        }

        void MainElement_onWatchedIndexChanged()
        {
            SrcLink = WatchedUrls[watchedUrlsIndex];
        }

        private void btnWatchClick(object sender, RoutedEventArgs e)
        {
            if(onButtonWatchClicked != null) onButtonWatchClicked(sender, e, SrcLink);
            WatchedUrls.Add(SrcLink);
            if (WatchedUrlsIndex == -1) WatchedUrlsIndex = 0;
        }

        private void txtLink_PreviewDrop(object sender, DragEventArgs e)
        {
            SrcLink = string.Empty;
            base.OnPreviewDrop(e);
        }

        private void txtLink_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnWatchClick(sender, e);
            else if (e.Key == Key.Up || e.Key == Key.Down)
            {
                if(watchedUrlsIndex != -1)
                {
                    if (e.Key == Key.Up) if (WatchedUrlsIndex > 0) WatchedUrlsIndex--;
                    if (e.Key == Key.Down) if (WatchedUrlsIndex < WatchedUrls.Count - 1) WatchedUrlsIndex++;
                }
            }
        }
    }
}
