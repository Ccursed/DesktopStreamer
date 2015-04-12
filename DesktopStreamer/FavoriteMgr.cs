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
        #region Events

        #endregion

        #region Properties

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

        private readonly BitmapImage defaultLogo = new BitmapImage(new Uri(@"Resources/twitch_default.png", UriKind.Relative));

        private List<Favorite> lsToUpdate;
        public List<Favorite> LsToUpdate
        {
            get { return lsToUpdate; }
            set { lsToUpdate = value; }
        }

        private List<Favorite> lsUpdated;
        public List<Favorite> LsUpdated
        {
            get { return lsUpdated; }
            set { lsUpdated = value; }
        }

        private bool runUpdate = true;
        public bool RunUpdate
        {
            get { return runUpdate; }
            set { runUpdate = value; }
        }

        private Dispatcher dispatcher;
        #endregion

        public FavoriteMgr(FavoriteList favList, string favoriteDirectory, string favoriteLogoDirectory, Dispatcher dispatcher)
        {
            this.favoriteDirectory = favoriteDirectory;
            this.favoriteLogoDirectory = favoriteLogoDirectory;
            this.favList = favList;
            //this.FavoriteUpdated += FavoriteMgr_FavoriteUpdated;
            this.dispatcher = dispatcher;
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
                fav.SrcChannel = channel;
                fav.SrcStream = stream;
                fav.Id = channel.Id;
                fav.Url = normalizedUrl;
                fav.Title = channel.DisplayName;
                //fav.Description = channel.Status;
                fav.ListPosition = -1;
                fav.Logo = defaultLogo;

                if (fav.SrcStream == StreamObject.StreamNotFound) fav.State = Favorite.Status.OFFLINE;
                else if (fav.SrcStream == StreamObject.StreamError) fav.State = Favorite.Status.PENDING;
                else fav.State = Favorite.Status.ONLINE;

                ImageSource img = GetStoredLogo(fav.SrcChannel);
                if (img != defaultLogo) fav.Logo = img;
                else fav.Logo = DownloadLogo(fav.SrcChannel);

                return fav;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("FavoriteMgr.Favorite failed on [{0}]. Error: {1}", url, ex.Message));
            }
        }

        public void UpdateFavorite(Favorite fav, bool downloadLogo)
        {
            try
            {
                string appended = "", host = "", normalizedUrl = "";
                HostApi.NormalizeUrl(fav.Url, out host, out appended, out normalizedUrl);
                StreamObject stream = GetStreamObject(appended.Replace("/", ""));
                ChannelObject channel = GetChannelObject(appended.Replace("/", ""));

                fav.Id = channel.Id;
                fav.Description = channel.Status;
                fav.Title = channel.DisplayName;
                fav.SrcChannel = channel;
                fav.SrcStream = stream;

                if (downloadLogo) if (fav.Logo == null || fav.Logo == defaultLogo) DownloadLogo(channel);

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("UpdateFavorite failed. Error: {0}", ex.Message));
            }
        }

        public void UpdateFavorite(Favorite fav)
        {
            UpdateFavorite(fav, true);
        }

        public void ConnectFavorite(Favorite fav)
        {
            UpdateFavorite(fav, false);
            fav.Logo = GetStoredLogo(fav.SrcChannel);
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
            catch (FileNotFoundException)
            {
                return defaultLogo;
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
