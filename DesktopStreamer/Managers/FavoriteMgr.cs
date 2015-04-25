using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StreamHostApi;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Net.Cache;
using System.ComponentModel;
using System.Timers;
using System.Windows.Threading;
using System.Windows;
using System.Threading;

namespace DesktopStreamer
{
    class FavoriteMgr
    {
        #region Properties

        private readonly BitmapImage defaultLogo = new BitmapImage(new Uri(@"Resources/twitch_default.png", UriKind.Relative));

        private System.Timers.Timer updateTimer;
        public System.Timers.Timer UpdateTimer
        {
          get { return updateTimer; }
          set { updateTimer = value; }
        }

        private readonly string favoriteDirectory;
        public string FavoriteDirectory
        {
            get { return favoriteDirectory; }
        }

        private readonly string favoriteLogoDirectory;
        public string FavoriteLogoDirectory
        {
            get { return favoriteLogoDirectory; }
        }

        private readonly FavoriteList favList;
        public FavoriteList FavList
        {
            get { return favList; }
        }
        #endregion

        public FavoriteMgr(FavoriteList favList, string favoriteDirectory, string favoriteLogoDirectory, Dispatcher dispatcher)
        {
            this.favoriteDirectory = favoriteDirectory;
            this.favoriteLogoDirectory = favoriteLogoDirectory;
            this.favList = favList;
            this.updateTimer = new System.Timers.Timer();
            updateTimer.Interval = 5000;
            updateTimer.Elapsed += updateTimer_Elapsed;
        }

        private void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            FavList.UiDispatcher.Invoke(() =>
            {
                int index = FavList.UpdateIndex;

                if (index != -1)
                {
                    if(FavList.Favorites[index].Url != null) FavList.Favorites[index] = CreateFavorite(FavList.Favorites[index].Url);
                }
            });
        }

        public Favorite CreateFavorite(string url)
        {
            try
            {
                string appended = "", host = "", normalizedUrl = "";
                if (!HostApi.NormalizeUrl(url, out host, out appended, out normalizedUrl)) return null;
                Favorite fav = new Favorite();
                StreamObject stream = GetStreamObject(appended.Replace("/", ""));
                ChannelObject channel = GetChannelObject(appended.Replace("/", ""));
                if (!channel.ChannelExists()) throw new FavoriteCouldNotBeCreatedException();
                fav.SrcChannel = channel;
                fav.SrcStream = stream;
                fav.Id = channel.Id;
                fav.Url = normalizedUrl;
                fav.Title = channel.DisplayName;
                fav.Description = channel.Status;
                fav.ListPosition = -1;

                if (fav.SrcStream == StreamObject.StreamNotFound) fav.State = Favorite.Status.OFFLINE;
                else if (fav.SrcStream == StreamObject.StreamError) fav.State = Favorite.Status.PENDING;
                else fav.State = Favorite.Status.ONLINE;

                fav.Logo = GetLogo(channel);

                return fav;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("FavoriteMgr.Favorite failed on [{0}]. Error: {1}", url, ex.Message));
            }
        }

        public void AddFavorite(string url)
        {
            FavList.UiDispatcher.Invoke(() =>
            {
                Favorite fav = CreateFavorite(url);
                FavList.Favorites.Add(fav);
            });
        }

        public StreamObject GetStreamObject(string name)
        {
            return HostApi.GetStream(HostApi.BuildUrl(HostApi.Operatione.Stream, name));
        }

        public ChannelObject GetChannelObject(string name)
        {
            return HostApi.GetChannel(HostApi.BuildUrl(HostApi.Operatione.Channel, name));
        }

        private  Favorite.Status IsOnline(StreamObject stream)
        {
            if (stream.IsStreamFound()) return Favorite.Status.ONLINE;
            else return Favorite.Status.OFFLINE;
        }

        private ImageSource GetLogo(ChannelObject channel)
        {
            try
            {
                string path = GetLogoPath(channel);
                if(File.Exists(path))
                {
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                }
                else
                {
                    if (channel.Logo == null) return new BitmapImage(new Uri(@"..\Resources\twitch_default.png", UriKind.Relative));
                    System.Drawing.Image logo = HostApi.DownloadStreamLogo(channel.Logo);
                    logo.Save(path);
                    return new BitmapImage(new Uri(path, UriKind.Absolute));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetLogo failed with Url: {0} . Error: {1}", channel.Id, ex.Message));
            }
        }

        private string GetLogoPath(ChannelObject channel)
        {
            try
            {
                return favoriteLogoDirectory + @"\" + channel.Id.ToString() + @".png";
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetLogoPath failed. Error: {0}", ex.Message));
            }
        }
    }

    public class FavoriteCouldNotBeCreatedException : Exception
    {
    }
}
