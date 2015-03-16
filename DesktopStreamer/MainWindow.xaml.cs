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
        private bool expanded = true;
        private double expandHeight;
        private double collapseHeight = 83;
        private bool justExpanded = false;
        #endregion

        #region Startup and Initialization

        public MainWindow()
        {
            InitializeComponent();
            expandHeight = Height;
            Inits();
            RegisterEvents();
            fileMgr.ValidateFileStructure();
            LoadSerializedFavorites();
        }

        private void Inits()
        {
            fileMgr = new FileMgr();
            favMgr = new FavoriteMgr(favList, fileMgr.FavoriteDirectory, fileMgr.FavoriteLogoDirectory);
            LivestreamerWrapper.Init(fileMgr.LivestreamerDirectory);
            lsInstances = new List<LivestreamerWrapper>();
        }

        private void RegisterEvents()
        {
            this.Closing += MainWindow_Closing;
            AppDomain.CurrentDomain.AssemblyResolve += LoadLocalAssemblys;
            mainEle.onButtonWatch += MainEle_WatchClickEvent;
            favList.onFavDrop += favList_onFavDrop;
            favList.onAddClick += favList_AddClick;
            favList.onRemoveClick += favList_RemoveClick;
            favList.onSelectionChanged += favList_onSelectionChanged;
        }

        private async void LoadSerializedFavorites()
        {
            List<Favorite> favorites = null;

            await Task.Run(() => (favorites = fileMgr.DeserializeFavorites()));

            favMgr.InsertDeserializedFavorites(favorites);
        }
        #endregion

        private void AddFavorite(string link)
        {
            try
            {
                Favorite fav = favMgr.CreateFavorite(link);
                fileMgr.SerializeFavorite(fav);
                favList.AddNewFavorite(fav);
            }
            catch (Exception)
            {
                MessageBox.Show("Favorite could not be added!");
            }
        }

        private void RemoveFavorite(Favorite fav)
        {
            fileMgr.DeleteFavorite(fav);
        }

        #region Event handling

        private void MainEle_WatchClickEvent(object sender, RoutedEventArgs e, string link)
        {
            LivestreamerWrapper lsWrapper = LivestreamerWrapper.CreateInstance();
            lsWrapper.SetArguments(LivestreamerWrapper.CreateStartParameter(link, LivestreamerWrapper.Quality.Best, fileMgr.PlayerPath, null));
            MessageBox.Show(lsWrapper.lsInstance.StartInfo.Arguments);
            lsWrapper.instanceChangedState += onInstanceChangedState;
            lsWrapper.Start();
            lsWrapper.log.onAdd += log_onAdd;
        }

        private void lsw_instanceFinished(LivestreamerWrapper instance)
        {
            MessageBox.Show(instance.name);
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

            MessageBox.Show(now.ToString());
        }

        private void log_onAdd(string line)
        {
            Console.WriteLine(line);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (LivestreamerWrapper instance in lsInstances) instance.Close();
        }

        private void btnExpand_Click(object sender, RoutedEventArgs e)
        {
            if (expanded)
            {
                expanded = !expanded;
                MinHeight = 83;
                Height = collapseHeight;
                favList.Visibility = Visibility.Collapsed;
                ExpandArea.Height = new GridLength(0.0, GridUnitType.Star);
            }
            else
            {
                expanded = !expanded;
                MinHeight = 200;
                Height = expandHeight;
                favList.Visibility = Visibility.Visible;
                ExpandArea.Height = new GridLength(5.0, GridUnitType.Star);
            }

            justExpanded = true;
        }

        private void MainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (justExpanded)
            {
                justExpanded = false;
                return;
            }

            if(expanded) expandHeight = e.NewSize.Height;
            else collapseHeight = e.NewSize.Height;
        }

        private void favList_onFavDrop(string link)
        {
            AddFavorite(link);
        }

        private void favList_AddClick()
        {
            AddFavorite(mainEle.srcLink);
        }

        private void favList_RemoveClick(Favorite fav)
        {
            RemoveFavorite(fav);
        }

        private void favList_onSelectionChanged(Favorite fav)
        {
            mainEle.srcLink = fav.Url;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private Assembly LoadLocalAssemblys(object sender, ResolveEventArgs args)
        {
            try
            {
                if (fileMgr.Validation == FileMgr.ValidationStatus.Unvalidated) fileMgr.ValidateFileStructure();

                if (args.Name.ToLower().StartsWith("streamhostapi"))
                    return fileMgr.LoadStreamHostApi();
                else return fileMgr.LoadNewtonsoft();
            }
            catch (NullReferenceException)
            {
                MessageBox.Show("Not funny man!");
                throw;
            }
            catch (Exception)
            {
                MessageBox.Show("Well, I couldnt build the necessary file structure. Good Luck in the future ! :) ");
                //Close();
                throw;
            }
        }
        #endregion   
    }
}
