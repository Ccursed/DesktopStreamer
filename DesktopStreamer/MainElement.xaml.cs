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

    /// <summary>
    /// Interaktionslogik für MainElement.xaml
    /// </summary>
    public partial class MainElement : UserControl
    {
        public event BtnWatchHandler onButtonWatch;

        public string srcLink
        {
            get { return txtLink.Text; }
            set { txtLink.Text = value; }
        }

        public MainElement()
        {
            InitializeComponent();
        }

        private void btnWatchClick(object sender, RoutedEventArgs e)
        {
            if(onButtonWatch != null)
                onButtonWatch(sender, e, srcLink);
        }

        private void txtLink_PreviewDrop(object sender, DragEventArgs e)
        {
            srcLink = "";
            base.OnPreviewDrop(e);
        }

        private void txtLink_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) btnWatchClick(sender, e);
        }
    }
}
