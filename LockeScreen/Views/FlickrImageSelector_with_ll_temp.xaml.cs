using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LockeScreen.Models;
using LockeScreen.ViewModels;
using System.Collections.ObjectModel;

namespace LockeScreen.Views
{
    public partial class FlickrImageSelector : PhoneApplicationPage
    {
        string AlbumName;
        ImageListViewModel images;

        SelectedImages _SelectedFlickrImages;

        public FlickrImageSelector()
        {
            InitializeComponent();
            images = new ImageListViewModel();
            
            _SelectedFlickrImages = new SelectedImages(AlbumPreview.ALBUM_TYPE.FLICKR);
            _SelectedFlickrImages.AlbumType = AlbumPreview.ALBUM_TYPE.FLICKR;

            this.Loaded += FlickrImageSelector_Loaded;
        }

        void FlickrImageSelector_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            AlbumName = this.NavigationContext.QueryString["selectedItem"];
            //images.FlickrGroupedImageCollection = LoadAlbumAsync(AlbumName);
            images.Images = LoadAlbumAsync(AlbumName);
            this.DataContext = images;

            foreach (ImageViewModel li in lbxThumbnails.Items)
            {
                // 
                if (_SelectedFlickrImages.SelectedImageNames.Contains(li.ImageID))
                {
                    lbxThumbnails.SelectedItems.Add(li); // Select the image
                }
            }


        }

        private void lbxThumbnails_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            foreach (ImageViewModel ivm in e.AddedItems)
            {

                if (!_SelectedFlickrImages.SelectedImageNames.Contains(ivm.ImageID))
                {

                    _SelectedFlickrImages.SelectedImageNames.Add(ivm.ImageID);
                    _SelectedFlickrImages.SaveFlickrImage(ivm.ImageID);
                }

                if (_SelectedFlickrImages.RemovedImages.Contains(ivm.ImageID))
                {
                    _SelectedFlickrImages.RemovedImages.Remove(ivm.ImageID);



                }
            }

            foreach (ImageViewModel ivm in e.RemovedItems)
            {

                while (_SelectedFlickrImages.SelectedImageNames.Contains(ivm.ImageID))
                {
                    _SelectedFlickrImages.SelectedImageNames.Remove(ivm.ImageID);
                }

                _SelectedFlickrImages.RemovedImages.Add(ivm.ImageID);
            }

            _SelectedFlickrImages.DeleteFiles();
            _SelectedFlickrImages.Save();
        }

        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            lbxThumbnails.SelectAll();
        }

        private ObservableCollection<ImageViewModel> LoadAlbumAsync(string AlbumName)
        {
            foreach (AlbumPreview alb in AlbumsViewModel.FlickrAlbums)
            {
                //
                if (alb.Name == AlbumName)
                {
                    _SelectedFlickrImages.AlbumName = AlbumName;
                    _SelectedFlickrImages.Load();
                    return alb.ImageThumbnails;
                }
            }

            return new ObservableCollection<ImageViewModel>();
        }




    }
}