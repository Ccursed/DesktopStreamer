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

namespace DesktopStreamer
{
    class FavoriteMgr
    {
        #region Properties
        
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

        public FavoriteMgr(FavoriteList favList, string favoriteDirectory, string favoriteLogoDirectory)
        {
            this.favoriteDirectory = favoriteDirectory;
            this.favoriteLogoDirectory = favoriteLogoDirectory;
            this.favList = favList;
        }

        public Favorite CreateFavorite(string url) //TODO die beiden Methoden mergen
        {
            try
            {
                string appended = "", host = "", normalizedUrl = "";
                if (!HostApi.NormalizeUrl(url, out host, out appended, out normalizedUrl)) return null;
                Favorite fav = new Favorite();
                StreamObject stream = GetStreamObject(appended.Replace("/", ""));
                ChannelObject channel = GetChannelObject(appended.Replace("/", ""));

                if (!channel.ChannelExists()) throw new FavoriteCouldNotBeCreatedException();

                fav.Url = normalizedUrl;
                fav.Description = channel.Status;
                fav.Title = channel.DisplayName;
                fav.SrcChannel = channel;
                fav.SrcStream = stream;

                if (fav.SrcStream == StreamObject.StreamNotFound) fav.State = Favorite.Status.OFFLINE;
                else if (fav.SrcStream == StreamObject.StreamError) fav.State = Favorite.Status.PENDING;
                else fav.State = Favorite.Status.ONLINE;

                fav.Logo = DownloadLogo(channel);
                return fav;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("FavoriteMgr.Favorite failed on [{0}]. Error: {1}", url, ex.Message));
            }
        }

        public void InsertDeserializedFavorites(List<Favorite> favorites) //TODO die beiden Methoden mergen
        {
            foreach(Favorite fav in favorites)
            {
                string appended = "", host = "", normalizedUrl = "";
                HostApi.NormalizeUrl(fav.Url, out host, out appended, out normalizedUrl);
                StreamObject stream = GetStreamObject(appended.Replace("/", ""));
                ChannelObject channel = GetChannelObject(appended.Replace("/", ""));

                fav.Description = channel.Status;
                fav.Title = channel.DisplayName;
                fav.SrcChannel = channel;
                fav.SrcStream = stream;

                if (fav.SrcStream == StreamObject.StreamNotFound) fav.State = Favorite.Status.OFFLINE;
                else if (fav.SrcStream == StreamObject.StreamError) fav.State = Favorite.Status.PENDING;
                else fav.State = Favorite.Status.ONLINE;

                fav.Logo = GetStoredLogo(channel);
                favList.favorites.Add(fav);
            }
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

        private ImageSource GetStoredLogo(ChannelObject channel)
        {
            try
            {
                return new BitmapImage(new Uri(GetLogoPath(channel), UriKind.Absolute));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetStoredLogo failed. Error: {0}", ex.Message));
            }
        }

        private ImageSource DownloadLogo(ChannelObject channel)
        {
            try
            {
                if (channel.Logo == null) return new BitmapImage(new Uri(@"Resources/twitch_default.png", UriKind.Relative));
                System.Drawing.Image logo = HostApi.DownloadStreamLogo(channel.Logo);
                string savePath = GetLogoPath(channel);
                logo.Save(savePath);
                return new BitmapImage(new Uri(savePath, UriKind.Absolute));
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DownloadLogo faled. Url: [{0}]. Error: {1}", channel.Logo, ex.Message));
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
