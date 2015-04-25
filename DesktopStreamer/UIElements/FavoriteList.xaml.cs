using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace DesktopStreamer
{
    #region Handlers

    public delegate void BtnAddHandler();
    public delegate void FavDropHandler(string link);
    public delegate void RemoveHandler(Favorite fav);
    public delegate void SelectionChangedHandler(Favorite fav); 
    #endregion

    public partial class FavoriteList : UserControl, INotifyPropertyChanged
    {
        #region Events

        public event BtnAddHandler onAddClick;
        public event FavDropHandler onFavDrop;
        public event RemoveHandler onRemoveClick;
        public event SelectionChangedHandler onSelectionChanged;
        public event PropertyChangedEventHandler PropertyChanged; 
        #endregion

        #region Properties

        private ObservableCollection<Favorite> favorites;
        public ObservableCollection<Favorite> Favorites
        {
            get { return favorites; }
            set { favorites = value; }
        }

        public Dispatcher UiDispatcher
        {
            get { return Dispatcher.CurrentDispatcher; }
        }

        private int updateIndex;
        public int UpdateIndex
        {
            get { return updateIndex; }
            set { updateIndex = value; if (updateIndex >= Favorites.Count) updateIndex = 0; }
        } 
        #endregion
        
        public FavoriteList()
        {
            InitializeComponent();
            favorites = new ObservableCollection<Favorite>();
            favList.ItemsSource = favorites;
            Console.WriteLine("list " + UiDispatcher.Thread.ManagedThreadId);
        }

        public void AddNewFavorite(Favorite fav)
        {
            Favorites.Add(fav);
            if (fav.ListPosition == -1) fav.ListPosition = Favorites.Count - 1;
            Favorites.OrderBy(b => b.ListPosition);
            NotifyProperyChanged("favorites");
        }

        //public void InitFavorites(List<Favorite> favorites)
        //{
        //    if(favorites == null) return ;
        //    foreach (Favorite fav in favorites) AddNewFavorite(fav);
        //}

        public List<Favorite> GetFavoriteList()
        {
            return new List<Favorite>(Favorites);
        }

        #region Event handling

        private void NotifyProperyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void btnAddClick(object sender, RoutedEventArgs e)
        {
            if (onAddClick != null) onAddClick();
        }

        private void btnUpClick(object sender, RoutedEventArgs e)
        {
            if(favList.SelectedIndex > 0) Favorites.Move(favList.SelectedIndex, favList.SelectedIndex - 1);
        }

        private void btnDownClick(object sender, RoutedEventArgs e)
        {
            if (favList.SelectedIndex < Favorites.Count - 1) Favorites.Move(favList.SelectedIndex, favList.SelectedIndex + 1);
        }

        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            if (favList.SelectedIndex < 0) return;
            Favorite fav = Favorites[favList.SelectedIndex];
            Favorites.RemoveAt(favList.SelectedIndex);
            if (onRemoveClick != null) onRemoveClick(fav);
        }

        private void favList_PreviewDrop(object sender, DragEventArgs e)
        {
            if (onFavDrop != null) onFavDrop(e.Data.GetData(typeof(string)) as string);
        }

        private void favList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favList.SelectedIndex >= 0) if (onSelectionChanged != null) onSelectionChanged(Favorites[favList.SelectedIndex]);
        }
        #endregion 
    }
}
