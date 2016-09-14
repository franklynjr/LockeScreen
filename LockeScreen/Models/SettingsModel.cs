using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using LockeScreen.Common;
using System.Threading;
using LockeScreen.ViewModels;

namespace LockeScreen.Models
{
    public class SettingsModel:ModelBase
    {
        private bool _FacebookPanoramaEnabled;
        private bool _PhonePanoramaEnabled;
        private bool _DynamicLockScreen;
        private bool _FlickrPanoramaEnabled;
        private bool _SkyDrivPanoramaeEnabled;
        private bool _InstagramPanoramaEnabled;
        private bool _ToastEnabled;
        private bool _QuickSet;
        private bool _Note;





        public bool NotePanoramaEnabled
        {
            get { return _Note; }
            set
            {
                if (_Note != value)
                {
                    _Note = value;
                    NotifyPropertyChanged("NotePanoramaEnabled");
                }
            }
        }


        public bool FacebookPanoramaEnabled
        {
            get { return _FacebookPanoramaEnabled; }
            set
            {
                if (_FacebookPanoramaEnabled != value)
                {
                    _FacebookPanoramaEnabled = value;
                    NotifyPropertyChanged("FacebookPanoramaEnabled");
                }
            }
        }



        public bool FlickrPanoramaEnabled
        {
            get { return _FlickrPanoramaEnabled; }
            set
            {
                if (_FlickrPanoramaEnabled != value)
                {
                    _FlickrPanoramaEnabled = value;
                    NotifyPropertyChanged("FlickrPanoramaEnabled");
                }
            }
        }
        public bool SkyDrivPanoramaeEnabled
        {
            get { return _SkyDrivPanoramaeEnabled; }
            set
            {
                if (_SkyDrivPanoramaeEnabled != value)
                {
                    _SkyDrivPanoramaeEnabled = value;
                    NotifyPropertyChanged("SkyDrivPanoramaeEnabled");
                }
            }
        }
        public bool InstagramPanoramaEnabled
        {
            get { return _InstagramPanoramaEnabled; }
            set
            {
                if (_InstagramPanoramaEnabled != value)
                {
                    _InstagramPanoramaEnabled = value;
                    NotifyPropertyChanged("InstagramPanoramaEnabled");
                }
            }
        }

        public bool PhonePanoramaEnabled
        {
            get { return _PhonePanoramaEnabled; }
            set
            {
                if (_PhonePanoramaEnabled != value)
                {
                    _PhonePanoramaEnabled = value;
                    NotifyPropertyChanged("PhonePanoramaEnabled");
                }
            }
        }

        public bool ToastEnabled
        {
            get { return _ToastEnabled; }
            set
            {
                if (_ToastEnabled != value)
                {
                    _ToastEnabled = value;
                    NotifyPropertyChanged("ToastEnabled");
                }
            }
        }


        public bool QuickSetPanoramaEnabled
        {
            get { return _QuickSet; }
            set
            {
                if (_QuickSet != value)
                {
                    _QuickSet = value;
                    NotifyPropertyChanged("QuickSetPanoramaEnabled");
                }
            }
        }


        public bool DynamicLockScreen
        {
            get { return _DynamicLockScreen; }
            set
            {
                if (_DynamicLockScreen != value)
                {
                    _DynamicLockScreen = value;
                    NotifyPropertyChanged("DynamicLockScreen");
                }
            }
        }


        public static class SettingsKeys
        {
            public const string ALL_SETTINGS = "SettingsAll";

            // Isolated Settings
            public const string FACEBOOK_ENABLED = "SettingsFacebook";
            public const string FLICKR_ENABLED = "SettingsFlickr";
            public const string PHONE_ENABLED = "SettingsPhone";
            public const string QUICK_SET = "SettingsQuickSet";
            public const string NOTE_SETTINGS = "SettingsNote";
            public const string TOAST_ENABLED = "SettingsAll";
            public const string DYNAMIC_BACKGROUND = "DynamicBackground";
            public const string UPDATE_PANORAMA = "UPDATE_PANORAMA";

            //public const string ALL_SETTINGS = "SettingsAll";
            //public const string ALL_SETTINGS = "SettingsAll";
            //public const string ALL_SETTINGS = "SettingsAll";
            //public const string ALL_SETTINGS = "SettingsAll";
            //public const string ALL_SETTINGS = "SettingsAll";

        }



        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[SettingsKeys.ALL_SETTINGS] = this;

            IsolatedStorageSettings.ApplicationSettings.Save();
        }


        public void LoadAsync()
        {
            Thread loadControlsAsync = new Thread(new ThreadStart(Load));
            loadControlsAsync.Start();
        }


        public void Load()
        {
            Dispatcher.BeginInvoke(() =>
            {
                SettingsModel obj;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(SettingsKeys.ALL_SETTINGS, out obj))
                {


                    // Retrieve all settings
                    this.DynamicLockScreen = obj.DynamicLockScreen;
                    this.ToastEnabled = obj.ToastEnabled;
                    this.FlickrPanoramaEnabled = obj.FlickrPanoramaEnabled;
                    this.FacebookPanoramaEnabled = obj.FacebookPanoramaEnabled;
                    this.InstagramPanoramaEnabled = obj.InstagramPanoramaEnabled;
                    this.PhonePanoramaEnabled = obj.PhonePanoramaEnabled;
                    this.SkyDrivPanoramaeEnabled = obj.SkyDrivPanoramaeEnabled;
                    this.QuickSetPanoramaEnabled = obj.QuickSetPanoramaEnabled;
                    this.NotePanoramaEnabled = obj.NotePanoramaEnabled;

                }
                else
                {
                    LoadDesfaults();
                }
            });
        }

        public void Load(ResultCallbacks.SettingsLoadCallback sc)
        {
            Dispatcher.BeginInvoke(() =>
            {
                SettingsModel obj;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(SettingsKeys.ALL_SETTINGS, out obj))
                {


                    // Retrieve all settings
                    this.DynamicLockScreen = obj.DynamicLockScreen;
                    this.ToastEnabled = obj.ToastEnabled;
                    this.FlickrPanoramaEnabled = obj.FlickrPanoramaEnabled;
                    this.FacebookPanoramaEnabled = obj.FacebookPanoramaEnabled;
                    this.InstagramPanoramaEnabled = obj.InstagramPanoramaEnabled;
                    this.PhonePanoramaEnabled = obj.PhonePanoramaEnabled;
                    this.SkyDrivPanoramaeEnabled = obj.SkyDrivPanoramaeEnabled;
                    this.QuickSetPanoramaEnabled = obj.QuickSetPanoramaEnabled;
                    this.NotePanoramaEnabled = obj.NotePanoramaEnabled;
                    sc(this);

                }
                else
                {
                    LoadDesfaults();
                    sc(this);
                }
            });
        }


        public void LoadDesfaults()
        {
            
            this.PhonePanoramaEnabled = true;
            this.FacebookPanoramaEnabled = true;
            this.FlickrPanoramaEnabled = true;
            this.DynamicLockScreen = true;
            this.ToastEnabled = true;
            this.InstagramPanoramaEnabled = true;
            this.SkyDrivPanoramaeEnabled = true;
            this.QuickSetPanoramaEnabled = true;
            this.NotePanoramaEnabled = true;

        }



        public void Reset()
        {
            IsolatedStorageSettings.ApplicationSettings.Clear();
            FileOperation.DeleteAll();

            this.PhonePanoramaEnabled = true;
            this.FacebookPanoramaEnabled = true;
            this.FlickrPanoramaEnabled = true;
            this.DynamicLockScreen = true;
            this.ToastEnabled = true;
            this.InstagramPanoramaEnabled = true;
            this.SkyDrivPanoramaeEnabled = true;
            this.QuickSetPanoramaEnabled = true;

            IsolatedStorageSettings.ApplicationSettings[SettingsKeys.ALL_SETTINGS] = this;
        }
        public void ClearPhone()
        {
            foreach (AlbumPreview alb in AlbumsViewModel.PhoneAlbums)
            {
                IsolatedStorageSettings.ApplicationSettings[alb.Name] = null;
                
            }
            FileOperation.DeleteFiles(AlbumPreview.ALBUM_TYPE.PHONE);
        }

        public void ClearFacebook()
        {
            foreach (AlbumPreview alb in AlbumsViewModel.FacebookAlbums)
            {
                IsolatedStorageSettings.ApplicationSettings[alb.Name] = null;
            }
            FileOperation.DeleteFiles(AlbumPreview.ALBUM_TYPE.FACEBOOK);
        }


        public void ClearFlickr()
        {
            foreach (AlbumPreview alb in AlbumsViewModel.FlickrAlbums)
            {
                IsolatedStorageSettings.ApplicationSettings[alb.Name] = null;
            }
            FileOperation.DeleteFiles(AlbumPreview.ALBUM_TYPE.FLICKR);
        }


        public void ClearAll()
        {
            foreach (AlbumPreview alb in AlbumsViewModel.PhoneAlbums)
            {
                IsolatedStorageSettings.ApplicationSettings[alb.Name] = null;
            }
            FileOperation.DeleteAllFiles();
        }


    }


    //private List<SelectedImages> _SelectedPhoneImages;
    //private List<string> _PhoneAlbums;

    //private List<SelectedImages> _SelectedFacebookImages;
    //private List<string> _FacebookAlbums;

    //private List<SelectedImages> _SelectedFlickrPhoneImages;
    //private List<string> _FlickrAlbums;

}

