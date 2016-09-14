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
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using LockeScreen.ViewModels;

namespace LockeScreen.Models
{
    public class AlbumModel: INotifyPropertyChanged
    {
        string _AlbumName;
        string _AlbumID;

        BitmapImage _Thumbnail;
        ALBUM_TYPE _Type;


        public enum ALBUM_TYPE
        {
            PHONE,
            FACEBOOK,
            FLICKR,
            INSTAGRAM
        }

        ObservableCollection<ImageViewModel> _ImageThumbnails;

        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Constructor
        public AlbumModel()
        {
            _ImageThumbnails = new ObservableCollection<ImageViewModel>();
            _AlbumName = String.Empty;
            _Thumbnail = new BitmapImage();
        }


        public ALBUM_TYPE Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                    _Type = value;
            }
        }


        public ObservableCollection<ImageViewModel> ImageThumbnails
        {
            get { return _ImageThumbnails; }
            set
            {
                if (_ImageThumbnails != value)
                    _ImageThumbnails = value;
            }
        }


        public string Name
        {
            get { return _AlbumName; }
            set
            {
                if (_AlbumName != value)
                    _AlbumName = value;
            }
        }
        public string AlbumID
        {
            get { return _AlbumID; }
            set
            {
                if (_AlbumID != value)
                    _AlbumID = value;
            }
        }


        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
            set
            {
                if (_Thumbnail != value)
                    _Thumbnail = value;
            }
        }




        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[_AlbumName] = this;
            IsolatedStorageSettings.ApplicationSettings.Save();
        }

        public bool Load()
        {
            AlbumModel se = new AlbumModel(); ;
            if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<AlbumModel>(_AlbumName, out se))
            {
                this._AlbumName = se._AlbumName;
                this._Type = se._Type;
                this.ImageThumbnails = se._ImageThumbnails;
                if (se._ImageThumbnails.Count > 0)
                    return true;
            }

            return false;
        }


    }
}
