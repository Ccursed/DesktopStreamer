using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;

namespace DesktopStreamer
{
    public delegate List<Favorite> DeserializationFinishedHandler();

    class FileMgr
    {
        #region Properties

        private readonly string newtonsoftPath;
        public string NewtonsoftPath
        {
            get { return newtonsoftPath; }
        }

        private readonly string hostApiPath;
        public string HostApiPath
        {
            get { return hostApiPath; }
        }

        private string playerPath;
        public string PlayerPath
        {
            get { return playerPath; }
            set { playerPath = value; }
        }

        private readonly string baseDirectory;
        private readonly string dllDirectory;

        private readonly string livestreamerDirectory;
        public string LivestreamerDirectory
        {
            get { return livestreamerDirectory; }
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

        private FilestructureStatus structureStatus = FilestructureStatus.Unchecked;
        public FilestructureStatus StructureStatus
        {
            get { return structureStatus; }
            set { structureStatus = value; }
        }

        private ValidationStatus validation = ValidationStatus.Unvalidated;
        public ValidationStatus Validation
        {
            get { return validation; }
            set { validation = value; }
        }
        #endregion

        #region Enums

        public enum FilestructureStatus
        {
            Complete, Partial, Nonexistant, Unchecked
        };

        public enum ValidationStatus
        {
            Validated, Unvalidated
        };
        #endregion

        public FileMgr()
        {
            baseDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\DesktopStreamer";
            dllDirectory = baseDirectory + @"\DLL";
            livestreamerDirectory = baseDirectory + @"\Livestreamer";
            favoriteDirectory = baseDirectory + @"\Favorites";
            favoriteLogoDirectory = FavoriteDirectory + @"\Logos";
            hostApiPath = dllDirectory + @"\StreamHostApi.dll";
            newtonsoftPath = dllDirectory + @"\NewtonsoftJson.dll";
            PlayerPath = SearchVlc();
            
        }

        #region Filestructure and Extraction

        public void ValidateFileStructure()
        {
            try
            {
                //Base Directory
                if (!Directory.Exists(baseDirectory)) Directory.CreateDirectory(baseDirectory);

                //DLL Directory
                if (!Directory.Exists(dllDirectory)) Directory.CreateDirectory(dllDirectory);
                if (!File.Exists(hostApiPath)) ExtractResource("StreamHostApi.dll", dllDirectory, Properties.Resources.StreamHostApi);
                if (!File.Exists(newtonsoftPath)) ExtractResource("NewtonsoftJson.dll", dllDirectory, Properties.Resources.NewtonsoftJson);
                
                //Livestreamer Directory and files
                if (!Directory.Exists(livestreamerDirectory)) Directory.CreateDirectory(livestreamerDirectory);
                if (!File.Exists(LivestreamerDirectory + @"\livestreamer.exe"))
                {
                    foreach (String file in Directory.GetFiles(LivestreamerDirectory)) File.Delete(file);
                    ExtractResource("livestreamer.zip", baseDirectory, Properties.Resources.livestreamer);
                    ZipFile.ExtractToDirectory(baseDirectory + @"\" + "livestreamer.zip", baseDirectory);
                    File.Delete(baseDirectory + @"\" + "livestreamer.zip");
                }

                //Favorite structure
                if (!Directory.Exists(favoriteDirectory)) Directory.CreateDirectory(favoriteDirectory);
                if (!Directory.Exists(FavoriteLogoDirectory)) Directory.CreateDirectory(FavoriteLogoDirectory);

                Validation = ValidationStatus.Validated;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("FileMgr.ValidateFileStructure failed. Error: {0}", ex.Message));
            }
        }

        private void ExtractResource(string name, string path, byte[] data)
        {
            try
            {
                File.WriteAllBytes(path + @"\" + name, data);
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("FileMgr.ExtractResource failed on ({0}), ({1}). Error: {2}", name, path, ex.Message));
            }
        }

        public void CleanFileStructure()
        {
            CleanFileStructure(false);
        }

        public void CleanFileStructure(bool cleanFav)
        {
            try
            {
                if (Directory.Exists(dllDirectory)) Directory.Delete(dllDirectory, true);
                if (Directory.Exists(livestreamerDirectory)) Directory.Delete(livestreamerDirectory, true);
                if (cleanFav) if (Directory.Exists(FavoriteLogoDirectory)) Directory.Delete(FavoriteLogoDirectory, true);
                if (cleanFav) if (Directory.Exists(favoriteDirectory)) Directory.Delete(favoriteDirectory, true);
                if (cleanFav) if (Directory.Exists(baseDirectory)) Directory.Delete(baseDirectory, true);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("FileMgr.Extract failed. Error: {0}", ex.Message));
            }
        }

        public string SearchVlc()
        {
            string path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\VIDEOLAN\VLC", "InstallDir", null);
            if (path == null) path = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\VIDEOLAN\VLC", "InstallDir", null);
            if (path == null) path = @"C:\Program Files\VideoLAN\VLC";
            //if (path == null) throw new KeyNotFoundException("Couldnt find vlc.");
            return path + @"\vlc.exe";
        }

        public Assembly LoadNewtonsoft()
        {
            try
            {
                if (!File.Exists(newtonsoftPath)) throw new Exception("LoadNewtonSoft failed. Error: File does not Exist");
                Assembly newtonsoft = Assembly.LoadFrom(newtonsoftPath);
                return newtonsoft;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("LoadNewtonsoft failed. Error: {0}", ex.Message));
            }
        }

        public Assembly LoadStreamHostApi()
        {
            try
            {
                if (!File.Exists(hostApiPath)) throw new Exception("LoadStreamHostApi failed. Error: File does not Exist");
                Assembly streamHostApi = Assembly.LoadFrom(HostApiPath);
                return streamHostApi;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("LoadStreamHostApi failed. Error: {0}", ex.Message));
            }
        }
        #endregion

        #region Serialization

        public void SerializeFavorite(Favorite fav)
        {
            try
            {
                string path = FavoriteDirectory + @"\" + fav.Id + @".fav";
                using (Stream stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    IFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, fav);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SerializeFavorite failed. Error: {0}", ex.Message));
            }
        }

        public void SerializeFavoriteList(List<Favorite> favorites)
        {
            try
            {
                foreach (Favorite fav in favorites) SerializeFavorite(fav);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("SerializeFavoriteList failed. Error: {0}", ex.Message));
            }
        }

        public List<Favorite> DeserializeFavorites()
        {
            List<Favorite> favorites = new List<Favorite>();
            try
            {
                string[] files = Directory.GetFiles(FavoriteDirectory).Where(b => b.EndsWith(@".fav")).ToArray();
                foreach (String file in files)
                {
                    using (Stream stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        IFormatter formatter = new BinaryFormatter();
                        Favorite fav = (Favorite)formatter.Deserialize(stream);
                        favorites.Add(fav);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DeserializeFavorites failed. Error: {0}", ex.Message));
            }

            return favorites;
        }

        public void DeleteFavorite(Favorite fav)
        {
            try
            {
                string path = FavoriteDirectory + @"\" + fav.Id + @".fav";
                if (File.Exists(path)) File.Delete(path);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("DeleteFavorite failed. Id: {0}. Error: {1}", fav.Id, ex.Message));
            }
        }
        #endregion 
    }
}
