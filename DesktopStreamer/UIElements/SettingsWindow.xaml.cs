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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DesktopStreamer
{
    public partial class SettingsWindow : Window
    {
        public ObservableCollection<MediaPlayer> obscMediaPlayers;
        
        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        public SettingsWindow(Settings settings)
        {
            InitializeComponent();
            obscMediaPlayers = new ObservableCollection<MediaPlayer>();
            lsPlayers.ItemsSource = obscMediaPlayers;
            this.Settings = settings;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            Owner.IsEnabled = true;
            base.OnClosing(e);
        }

        public void SetPlayerList(IEnumerable<MediaPlayer> cltPlayers)
        {
            foreach (MediaPlayer mp in cltPlayers) obscMediaPlayers.Add(mp);
        }

        private void slBuffer_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            tbBuffer.Text = slBuffer.Value.ToString();
        }

        private void btnLogFolder_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(FileMgr.LogDirectory);
        }
    }
}
