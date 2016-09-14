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

namespace LockeScreen.ViewModels
{
    public class AlbumViewModel: INotifyPropertyChanged
    {
        string _AlbumName;
        BitmapImage _Thumbnail;
        ObservableCollection<ImageViewModel> _Images;

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
        public AlbumViewModel()
        {
            _Images = new ObservableCollection<ImageViewModel>();
            _AlbumName = String.Empty;
            _Thumbnail = new BitmapImage();
        }



        public ObservableCollection<ImageViewModel> Images
        {
            get { return _Images; }
            set
            {
                if (_Images != value)
                    _Images = value;
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


        public BitmapImage Thumbnail
        {
            get { return _Thumbnail; }
            set
            {
                if (_Thumbnail != value)
                    _Thumbnail = value;
            }
        }



    }
}
