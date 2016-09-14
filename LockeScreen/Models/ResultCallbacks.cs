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
using LockeScreen.Models;
using LockeScreen.ViewModels;
using System.Windows.Media.Imaging;

namespace LockeScreen.Models
{
    public class ResultCallbacks
    {


        public delegate void AlbumCollectionResultCallback(ObservableCollection<AlbumPreview> albums);
        public delegate void FlickrAlbumsCallback(FlickerAlbums albums);
        public delegate void ImageCollectionResultCallback(ObservableCollection<ImageViewModel> Images);

        public delegate void AlbumCallbackResult(AlbumPreview album);
        public delegate void ImageCallbackResult(BitmapImage Image);
        public delegate void SettingsLoadCallback(SettingsModel settings);

        public enum LoginResult
        {
            SUCCESS,
            FAILED
        }
    }
}
