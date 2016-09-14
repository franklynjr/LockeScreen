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
using LockeScreen.Common;
using Microsoft.Xna.Framework.Media;
using System.Threading;
using System.Collections.Generic;
using LockeScreen.Helpers;

namespace LockeScreen.Models
{
    public class AlbumPreview
    {



        public enum ALBUM_TYPE
        {
            PHONE,
            FACEBOOK,
            FLICKR,
            INSTAGRAM
        }

        



       
        public AlbumPreview()
        {
            ImageThumbnails = new ObservableCollection<ImageViewModel>();
            Thumbnail = new BitmapImage();
        }


        public ALBUM_TYPE Type
        {
            get;
            set;
        }


        public ObservableCollection<ImageViewModel> ImageThumbnails
        {
            get;
            set;
        }


        public bool ImageThumbnailsLoaded
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }


        public string AlbumID
        {
            get;
            set;
        }


        public BitmapImage Thumbnail
        {
            get;
            set;
        }


        public string GroupName
        {
            get;
            set;
        }


      

       



        public object NextUrl { get; set; }

    }
}
