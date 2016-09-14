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
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using LockeScreen.Models;
using System.Windows.Threading;
using System.Threading;
using LockeScreen.Common;
using Facebook;
using System.Collections.Generic;
using System.Linq;
using LockeScreen.Helpers;
using System.IO.IsolatedStorage;

namespace LockeScreen.ViewModels
{
    public class AlbumListViewModel : ModelBase
    {
        private ObservableCollection<AlbumPreview> _PhoneAlbums;
        private ObservableCollection<AlbumPreview> _FacebookAlbums;
        private ObservableCollection<AlbumPreview> _FlickrAlbums;
        private ObservableCollection<ImageViewModel> _SelectedImages;
        private BitmapImage _CurrentBackgroundImage;


        private string _note;



        public BitmapImage CurrentBackgroundImage
        {

            get { return _CurrentBackgroundImage; }
            set
            {
                if (_CurrentBackgroundImage != value)
                {
                    _CurrentBackgroundImage = value;
                    NotifyPropertyChanged("CurrentBackgroundImage");
                }
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {

                if (_note != value)
                {
                    _note = value;
                    NotifyPropertyChanged("Note");
                }
            }
        }

        public ObservableCollection<ImageViewModel> SelectedImages
        {
            get { return _SelectedImages; }
            set
            {

                if (_SelectedImages != value)
                {
                    _SelectedImages = value;
                    NotifyPropertyChanged("SelectedImages");
                }
            }
        }

        private ObservableCollection<StorageInfo> _Storage;


        private string access_token;


        public ObservableCollection<AlbumPreview> PhoneAlbums
        {
            get { return _PhoneAlbums; }
            set
            {
                
                if (_PhoneAlbums != value)
                {
                    _PhoneAlbums = value;
                    NotifyPropertyChanged("PhoneAlbums");
                }
            }
        }


        public ObservableCollection<AlbumPreview> FacebookAlbums
        {
            get { return _FacebookAlbums; }
            set
            {
                if (_FacebookAlbums != value)
                {
                    _FacebookAlbums = value;
                    NotifyPropertyChanged("FacebookAlbums");
                }
            }
        }

        public ObservableCollection<AlbumPreview> FlickrAlbums
        {
            get { return _FlickrAlbums; }
            set
            {
                if (_FlickrAlbums != value)
                {
                    _FlickrAlbums = value;
                    NotifyPropertyChanged("FlickrAlbums");
                }
            }
        }

        public List<KeyedList<string, AlbumPreview>> _FlickrGroupedAlbums;

        public List<KeyedList<string, AlbumPreview>> FlickrGroupedAlbums
        {
            get { return _FlickrGroupedAlbums;}
            set
            {
                if (_FlickrGroupedAlbums != value)
                {
                    _FlickrGroupedAlbums = value;
                    NotifyPropertyChanged("FlickrGroupedAlbums");
                }
            }
        }

        //ToDo Rename Flickr Grouped Albums
        public ObservableCollection<AlbumPreview> FlickrGroupedAlbumsCollection
        {
            set
            {
                var FlickrAlbums = value;

                var groupedAlbums =
                    from album in FlickrAlbums
                    orderby album.GroupName
                    group album by album.GroupName into albumsByAlbumType
                    select new KeyedList<string, AlbumPreview>(albumsByAlbumType);

                FlickrGroupedAlbums = new List<KeyedList<string, AlbumPreview>>(groupedAlbums); ;
                //return new List<KeyedList<string, AlbumPreview>>(groupedAlbums);
            }
        }


        //ToDo Rename Flickr Grouped Albums
        public List<KeyedList<string, AlbumPreview>> GroupedAlbums
        {
            get
            {
                var FlickrAlbums = AlbumsViewModel.FlickrAlbums;

                var groupedAlbums =
                    from album in FlickrAlbums
                    orderby album.GroupName
                    group album by album.GroupName into albumsByAlbumType
                    select new KeyedList<string, AlbumPreview>(albumsByAlbumType);

                return new List<KeyedList<string, AlbumPreview>>(groupedAlbums);
            }
        }

        //public delegate void LoginFailedHandler();
        //public  event LoginFailedHandler LoginFailed;

        public ObservableCollection<StorageInfo> Storage
        {
            get { return _Storage; }
            set
            {
                if (_Storage != value)
                {
                    _Storage = value;
                    NotifyPropertyChanged("Storage");
                }
            }
        }




        public string AccessToken
        {
            get { return access_token; }
            set
            {

                if (access_token != value)
                    access_token = value;

                NotifyPropertyChanged("AccessToken");

            }
        }

        // Constructor
        public AlbumListViewModel()
        {

            _PhoneAlbums = new ObservableCollection<AlbumPreview>();
            _FacebookAlbums = new ObservableCollection<AlbumPreview>();
            _FlickrAlbums = new ObservableCollection<AlbumPreview>();
            _SelectedImages = new ObservableCollection<ImageViewModel>();

            _Storage = new ObservableCollection<StorageInfo>();
            //_Settings = new SettingsModel();

            Thread albumsThread = new Thread(new ThreadStart(GetPhoneAlbumsAsync));
            Thread FacebookThread = new Thread(new ThreadStart(GetFacebookAlbumsAsync));
            Thread FlickrThread = new Thread(new ThreadStart(GetFlickrAlbumsAsync));
            _CurrentBackgroundImage = new BitmapImage();

            PhoneAlbumsLoaded = false;
           //GetFacebookAlbumsAsync();

            //Settings.Load();

            FlickrThread.Start();
           FacebookThread.Start();
           albumsThread.Start();
            

        }

        FacebookHelper FbHelper;
        private void GetFacebookAlbumsAsync()
        {

            Dispatcher.BeginInvoke(() =>
            {

                FbHelper = new FacebookHelper();
                FacebookUserIsLoggedIn = true;
                // Try Again If Download Fail
                FbHelper.DownloadFailed += (sender, e) =>
                {

                    //Retry
                    if (fbErrorCount < MaxAttempt)
                    {
                        fbErrorCount++;
                        GetFacebookAlbumsAsync();
                    }
                };

                FbHelper.AccessTokenRequestFailed += (message) =>
                {
                    //LoginFailed();
                    FacebookUserIsLoggedIn = false;
                    FacebookAlbumsLoaded = true;
                };

                FbHelper.AccessTokenReceived += (token) =>
                {

                    Shared.ApplicationData["AccessToken"] = token;
                    base.FacebookUserIsLoggedIn = true;

                    FbHelper.GetFacebookAlbums(token, (albums) =>
                    {
                        Dispatcher.BeginInvoke(() =>
                        {
                           
                            FacebookAlbums = AlbumsViewModel.FacebookAlbums = Sort(albums);
                            base.FacebookAlbumsLoaded = true;
                        });

                    });
                };

                FbHelper.GetAccessToken();
            });
        }



        private ObservableCollection<AlbumPreview> Sort(ObservableCollection<AlbumPreview> col)
        {
            ObservableCollection<AlbumPreview> _collection = new ObservableCollection<AlbumPreview>();

            var SortedDoc = from item in col
                           orderby item.Name
                           select item;

            foreach (AlbumPreview alb in SortedDoc)
            {
                _collection.Add(alb);
            }

            return _collection;

        }


        private int fbErrorCount = 0;       // facebook attempt count
        private int MaxAttempt = 3;         // Max attempt for any async call

        private void GetFlickrAlbumsAsync(){
            
            //throw new NotImplementedException();
            Dispatcher.BeginInvoke(() =>
                           {
                               FlickrHelper flickrHelper = new FlickrHelper();
                               flickrHelper.LoginAttempted += (lRes) =>
                               {
                                   if (lRes == ResultCallbacks.LoginResult.SUCCESS)
                                   {
                                       FlickrUserIsLoggedIn = true;
                                       flickrHelper.GetAllAlbums((albRes) =>
                                          {
                                              FlickrAlbumsLoaded = true;
                                              //FlickrGroupedAlbumsCollection = AlbumsViewModel.FlickrAlbums = albRes;
                                              FlickrAlbums = AlbumsViewModel.FlickrAlbums = albRes;
                                          });

                                   }
                                   else
                                   {
                                       FlickrAlbumsLoaded = true;
                                       FlickrUserIsLoggedIn = false;
                                   }
                               };

                               flickrHelper.Login();
                               FlickrAlbumsLoaded = false;
                           });
        }

        //private ObservableCollection<AlbumViewModel> _Albums;
        public void GetPhoneAlbumsAsync()
        {
            AlbumsViewModel.LoadPhoneImages();
           
                Dispatcher.BeginInvoke(() =>
                {
                    this.PhoneAlbums = AlbumsViewModel.PhoneAlbums;
                    this.PhoneAlbumsLoaded = true;

                });
        }



        public void ReloadFacebook()
        {
            _FacebookAlbums = new ObservableCollection<AlbumPreview>();

            Thread FacebookThread = new Thread(new ThreadStart(GetFacebookAlbumsAsync));

            //GetFacebookAlbumsAsync();
            FacebookThread.Start();
            //throw new NotImplementedException();
        }

        public void ReloadFlickrAsync()
        {

            Thread FlickrThread = new Thread(new ThreadStart(GetFlickrAlbumsAsync));
            FlickrThread.Start();
            //GetFacebookAlbumsAsync();   
        }

    }
}
