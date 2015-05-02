using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using StreamHostApi;
using System.ComponentModel;
using System.Timers;
using System.Collections.ObjectModel;
using System.Configuration;

namespace DesktopStreamer
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Properties

        private FileMgr fileMgr;
        private FavoriteMgr favMgr;
        private List<LivestreamerWrapper> lsInstances;
        private SettingsManager settingsMgr;
        private bool expanded = true;
        private double expandHeight;
        private double collapseHeight = 83;
        private bool justExpanded = false;

        private Settings settings;
        public Settings Settings
        {
            get { return settings; }
            set { settings = value; }
        }
        #endregion

        #region Startup and Initialization

        public MainWindow()
        {
            InitializeComponent();
            expandHeight = Height;
            Inits();
            RegisterEvents();
            this.Left = 1200;

            fileMgr.ValidateFileStructure();
            LoadSerializedFavorites();
            settingsMgr.ScanForPlayers();
            FinishStartUp();
        }

        private void Inits()
        {
            fileMgr = new FileMgr();
            favMgr = new FavoriteMgr(favList, fileMgr.FavoriteDirectory, fileMgr.FavoriteLogoDirectory, Dispatcher);
            settingsMgr = new SettingsManager();
            LivestreamerWrapper.Init(fileMgr.LivestreamerDirectory);
            lsInstances = new List<LivestreamerWrapper>();
        }

        private void RegisterEvents()
        {
            this.Closing += MainWindow_Closing;
            AppDomain.CurrentDomain.AssemblyResolve += LoadLocalAssemblys;
            mainEle.onButtonWatchClicked += MainEle_WatchClickEvent;
            favList.onFavDrop += favList_onFavDrop;
            favList.onAddClick += favList_AddClick;
            favList.onRemoveClick += favList_RemoveClick;
            favList.onSelectionChanged += favList_onSelectionChanged;
        }

        private void FinishStartUp()
        {
            favMgr.UpdateTimer.Start();
        }

        private void LoadSerializedFavorites()
        {
            foreach (Favorite fav in fileMgr.DeserializeFavorites()) favMgr.AddFavorite(fav.Url);
        }
        #endregion

        private void AddFavorite(string link)
        {
            try
            {
                Favorite fav = favMgr.CreateFavorite(link);
                var exists = favList.Favorites.Where(b => b.Url == fav.Url).ToList();
                if(exists.Count == 0)
                {
                    fileMgr.SerializeFavorite(fav);
                    favList.AddNewFavorite(fav);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(ex.Message));
            }
        }

        private void RemoveFavorite(Favorite fav)
        {
            fileMgr.DeleteFavorite(fav);
        }

        #region Event handling

        #region UI

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnTray_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow wndSettings = new SettingsWindow(null); //ToDo
            wndSettings.Owner = this;
            wndSettings.Owner.IsEnabled = false;
            wndSettings.SetPlayerList(settingsMgr.dctMediaPlayer.Values);
            wndSettings.Show();
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            if (expanded)
            {
                expanded = !expanded;
                MinHeight = 83;
                Height = collapseHeight;
                favList.Visibility = Visibility.Collapsed;
                imgExpand.Source = new BitmapImage(new Uri("../Resources/arrow483.png", UriKind.Relative));
                ExpandArea.Height = new GridLength(0.0, GridUnitType.Star);
            }
            else
            {
                expanded = !expanded;
                MinHeight = 200;
                Height = expandHeight;
                favList.Visibility = Visibility.Visible;
                imgExpand.Source = new BitmapImage(new Uri("../Resources/arrow4832.png", UriKind.Relative));
                ExpandArea.Height = new GridLength(5.0, GridUnitType.Star);
            }

            justExpanded = true;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
        #endregion

        #region MainWindow

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (justExpanded)
            {
                justExpanded = false;
                return;
            }

            if (expanded) expandHeight = e.NewSize.Height;
            else collapseHeight = e.NewSize.Height;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (LivestreamerWrapper instance in lsInstances) instance.Close();
        } 
        #endregion

        #region FavoriteList

        private void favList_onFavDrop(string link)
        {
            AddFavorite(link);
        }

        private void favList_AddClick()
        {
            AddFavorite(mainEle.SrcLink);
        }

        private void favList_RemoveClick(Favorite fav)
        {
            RemoveFavorite(fav);
        }

        private void favList_onSelectionChanged(Favorite fav)
        {
            mainEle.SrcLink = fav.Url;
        }
        #endregion

        #region LivestreamerWrapper

        private void lsw_instanceFinished(LivestreamerWrapper instance)
        {

        }

        private void onInstanceChangedState(LivestreamerWrapper instance, LivestreamerWrapper.Status old, LivestreamerWrapper.Status now)
        {
            switch (now)
            {
                case LivestreamerWrapper.Status.Idle: break;
                case LivestreamerWrapper.Status.Starting: break;
                case LivestreamerWrapper.Status.Working: break;
                case LivestreamerWrapper.Status.Error: break;
                case LivestreamerWrapper.Status.Finished: instance.Close(); break;
                default: break;
            }
        }

        private void log_onAdd(string line)
        {
            Console.WriteLine(line);
        }
        #endregion

        private void MainEle_WatchClickEvent(object sender, RoutedEventArgs e, string link)
        {
            LivestreamerWrapper lsWrapper = LivestreamerWrapper.CreateInstance();
            lsWrapper.SetArguments(LivestreamerWrapper.CreateStartParameter(link, LivestreamerWrapper.Quality.Best, fileMgr.PlayerPath, null));
            lsWrapper.instanceChangedState += onInstanceChangedState;
            lsWrapper.Start();
            lsWrapper.log.onAdd += log_onAdd;
        }

        private Assembly LoadLocalAssemblys(object sender, ResolveEventArgs args)
        {
            try
            {
                if (args.Name.ToLower().StartsWith("streamhostapi")) return fileMgr.LoadStreamHostApi();
                else if (args.Name.ToLower().StartsWith("newtonsoft.json")) return fileMgr.LoadNewtonsoft();
                else return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Well, I couldnt build the necessary file structure. Good Luck in the future ! :) " + ex.Message);
                throw;
            }
        }
        #endregion 
    }
}
