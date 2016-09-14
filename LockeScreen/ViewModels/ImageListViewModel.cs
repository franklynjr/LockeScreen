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
using LockeScreen.Helpers;
using System.Collections.Generic;
using System.Linq;
using LockeScreen.Common;

namespace LockeScreen.ViewModels
{
    public class ImageListViewModel : ModelBase
    {
        ObservableCollection<ImageViewModel> _Images;
        public ObservableCollection<ImageViewModel> Images
        {
            get { return _Images; }
            set
            {
                if (_Images != value)
                {
                    _Images = value;
                    NotifyPropertyChanged("Images");
                }
            }
        }


        public List<KeyedList<string, ImageViewModel>> _FlickrGroupedImages;

        public List<KeyedList<string, ImageViewModel>> FlickrGroupedImages
        {
            get { return _FlickrGroupedImages; }
            set
            {
                if (_FlickrGroupedImages != value)
                {
                    _FlickrGroupedImages = value;
                    NotifyPropertyChanged("FlickrGroupedAlbums");
                }
            }
        }

        //ToDo Rename Flickr Grouped Albums
        public ObservableCollection<ImageViewModel> FlickrGroupedImageCollection
        {
            set
            {
                var FlickrImages = value;

                var groupedImages =
                    from image in FlickrImages
                    orderby image.GroupName
                    group image by image.GroupName into albumsByAlbumType
                    select new KeyedList<string, ImageViewModel>(albumsByAlbumType);

                FlickrGroupedImages = new List<KeyedList<string, ImageViewModel>>(groupedImages); ;
                //return new List<KeyedList<string, AlbumPreview>>(groupedAlbums);
            }
        }


        private string next_url;

        public string NextUrl
        {
            get { return next_url; }
            set
            {
                if (next_url != value)
                {
                    next_url = value;
                }
            }
        }

        private FacebookHelper FbHelper;
        public void LoadAlbumAsync(string AlbumID, ResultCallbacks.ImageCollectionResultCallback Images)
        {
            FbHelper = new FacebookHelper();

            FbHelper.AccessTokenRequestFailed += (message) =>
            {
                //LoginFailed();
            };

            FbHelper.AccessTokenReceived += (token) =>
            {
                FbHelper.GetAlbumPhotos(AlbumID, token, (result) => {

                    Images(result);
                });
            };
            FbHelper.GetAccessToken();
        }
    }
}
