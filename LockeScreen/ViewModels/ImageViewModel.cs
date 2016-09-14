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
using System.ComponentModel;
using System.Windows.Media.Imaging;
using LockeScreen.Common;

namespace LockeScreen.ViewModels
{
    public class ImageViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public ImageViewModel()
        {
            _Image = new BitmapImage();
        }

        private BitmapImage _Image;
        private string _ImageID;
        private string _ImageUrl;
        private Uri _imageUri;


        public BitmapImage Image
        {
            get { return _Image; }
            set
            {
                if (_Image != value)
                {
                    _Image = value;
                    NotifyPropertyChanged("Image");
                }
            }
        }

        string _GroupName;
        public string GroupName
        {
            get { return _GroupName; }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    NotifyPropertyChanged("GroupName");
                }
            }
        }

        public string ImageID
        {
            get { return _ImageID; }
            set
            {
                if (_ImageID != value)
                {
                    _ImageID = value;
                    NotifyPropertyChanged("ImageID");
                }
            }
        }

        public string Url
        {
            get { return _ImageUrl; }
            set
            {
                if (_ImageUrl != value)
                {
                    _ImageUrl = value;
                    _imageUri = new System.Uri("ms-appdata:///Local/" + value, UriKind.RelativeOrAbsolute);
                    this.ImageID = FileOperation.ExtractImageId(value);
                    NotifyPropertyChanged("Url");
                }
            }
        }



        public Uri Uri
        {
            get { return _imageUri; }
            set
            {
                if (_imageUri != value)
                {
                    _imageUri = value;

                    _ImageUrl = value.AbsolutePath;
                    NotifyPropertyChanged("Uri");
                }
            }
        }
    }
}
