using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace DesktopStreamer.UIElements
{
    public partial class CcursedMessageBox : Window
    {
        public CcursedMessageBox(string title, string message) : this(title, message, null)
        {}

        public CcursedMessageBox(string title, string message, string link)
        {
            InitializeComponent();
            tbMessage.Text = message;
            tbTitle.Text = title;
            if (!string.IsNullOrEmpty(link))
            {
                hlLink.NavigateUri = new Uri(link);
                tbHyperInline.Text = link.Replace(@"http://", "");
            }
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void hlLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
