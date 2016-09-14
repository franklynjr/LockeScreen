using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LockeScreen.Models;
using System.Collections.ObjectModel;
using LockeScreen.Common;
using System.Threading;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace LockeScreen.ViewModels
{
    public class StorageInfoViewModel: ModelBase
    {

        public StorageInfoViewModel()
        {
            Thread StoreageThread = new Thread(new ThreadStart(GetStorageStatusAsync));
            StoreageThread.Start();
        }

        public ObservableCollection<StorageInfo> Storage
        {
            get;
            set;
        }


        public void ReloadStorageAsync()
        {
            Thread StoreageThread = new Thread(new ThreadStart(GetStorageStatusAsync));
            StoreageThread.Start();
        }

        public void GetStorageStatusAsync()
        {

            string[] AlbumDirs = new string[] { "Phone", "Facebook", "Flickr" };
            Dispatcher.BeginInvoke(() =>
            {
                Storage = new ObservableCollection<StorageInfo>();
                foreach (string s in AlbumDirs)
                {

                    StorageInfo storageInfo = new StorageInfo();
                    storageInfo.SourceName = s;
                    this.Storage.Add(storageInfo);
                }

                try
                {
                    using (IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication())
                    {
                        BitmapImage __CurrentImage = new BitmapImage();
                        __CurrentImage.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                        __CurrentImage.DecodePixelHeight = 400;
                        __CurrentImage.DecodePixelWidth = 340;
                        string abs = LockScreenHelper.GetCurrent().AbsolutePath.TrimStart("Local/".ToCharArray());
                        using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(abs, System.IO.FileMode.Open, appStorage))
                        {
                            __CurrentImage.SetSource(fs);

                            CurrentImage = __CurrentImage;
                        }
                    }
                }
                catch { }
            });
        }

        BitmapImage _CurrentImage;

        public BitmapImage CurrentImage
        {
            get { return _CurrentImage; }
            set
            {
                if (_CurrentImage != value)
                {
                    _CurrentImage = value;
                    NotifyPropertyChanged("CurrentImage");
                }
            }
        }


    }
}
