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

namespace DesktopStreamer
{
    public delegate void BtnAddHandler();
    public delegate void FavDropHandler(string link);
    public delegate void RemoveHandler(Favorite fav);
    public delegate void SelectionChangedHandler(Favorite fav);

    public partial class FavoriteList : UserControl, INotifyPropertyChanged
    {
        public event BtnAddHandler onAddClick;
        public event FavDropHandler onFavDrop;
        public event RemoveHandler onRemoveClick;
        public event SelectionChangedHandler onSelectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;


        public ObservableCollection<Favorite> favorites { get; private set; }
        
        public FavoriteList()
        {
            InitializeComponent();
            favorites = new ObservableCollection<Favorite>();
            favList.ItemsSource = favorites;
        }

        public void AddNewFavorite(Favorite fav)
        {
            favorites.Add(fav);
            NotifyProperyChanged("favorites");
        }

        public List<Favorite> GetFavoriteList()
        {
            return new List<Favorite>(favorites);
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
            if(favList.SelectedIndex > 0) favorites.Move(favList.SelectedIndex, favList.SelectedIndex - 1);
        }

        private void btnDownClick(object sender, RoutedEventArgs e)
        {
            if (favList.SelectedIndex < favorites.Count - 1) favorites.Move(favList.SelectedIndex, favList.SelectedIndex + 1);
        }

        private void btnRemoveClick(object sender, RoutedEventArgs e)
        {
            if (favList.SelectedIndex < 0) return;
            Favorite fav = favorites[favList.SelectedIndex];
            favorites.RemoveAt(favList.SelectedIndex);
            if (onRemoveClick != null) onRemoveClick(fav);
        }

        private void favList_PreviewDrop(object sender, DragEventArgs e)
        {
            if (onFavDrop != null) onFavDrop(e.Data.GetData(typeof(string)) as string);
        }

        private void favList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (favList.SelectedIndex >= 0) if (onSelectionChanged != null) onSelectionChanged(favorites[favList.SelectedIndex]);
        }
        #endregion 
    }
}
