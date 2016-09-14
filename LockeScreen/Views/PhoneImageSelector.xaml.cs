using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using LockeScreen.ViewModels;
using LockeScreen.Models;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Globalization;

namespace LockeScreen.Views
{
    public partial class PhoneImageSelector : PhoneApplicationPage
    {
        // Image Grid ImageListViewModel
        ImageListViewModel _ImageListViewModel;

        // Save Selected Images
        SelectedImages _SelectedPhoneImages = new SelectedImages();


        //AlbumViewModel SelectedImages = new AlbumViewModel();

        public PhoneImageSelector():base()
        {
            InitializeComponent();
            _ImageListViewModel = new ImageListViewModel();
            _SelectedPhoneImages = new SelectedImages(AlbumPreview.ALBUM_TYPE.PHONE);
            this.Loaded += new RoutedEventHandler(PhoneImageSelector_Loaded);
        }

        void PhoneImageSelector_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Selected Album index
                int indx = int.Parse(NavigationContext.QueryString["indx"]);


                if (indx != -1)
                {
                    _ImageListViewModel.Images = AlbumsViewModel.PhoneAlbums[indx].ImageThumbnails;
                    _SelectedPhoneImages.AlbumName = AlbumsViewModel.PhoneAlbums[indx].Name;
                    _SelectedPhoneImages.Load();
                }

                DataContext = _ImageListViewModel;

                //Select all previously selected images
                foreach (ImageViewModel li in lbxThumbnails.Items)
                {
                    // 
                    if (_SelectedPhoneImages.SelectedImageNames.Contains(li.ImageID))
                    {
                        lbxThumbnails.SelectedItems.Add(li); // Select the image
                    }
                }

            }
            catch { }
        }




        private void ApplicationBarIconButton_Click_1(object sender, EventArgs e)
        {
            lbxThumbnails.SelectAll();
        }


        private void lbxThumbnails_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            foreach (ImageViewModel ivm in e.AddedItems)
            {

                if (!_SelectedPhoneImages.SelectedImageNames.Contains(ivm.ImageID)){

                    _SelectedPhoneImages.SelectedImageNames.Add(ivm.ImageID);
                    _SelectedPhoneImages.SavePhoneImages();
                    
                }

                if (_SelectedPhoneImages.RemovedImages.Contains(ivm.ImageID))
                {
                    _SelectedPhoneImages.RemovedImages.Remove(ivm.ImageID);
                }
            }

            foreach (ImageViewModel ivm in e.RemovedItems)
            {

                while (_SelectedPhoneImages.SelectedImageNames.Contains(ivm.ImageID))
                {
                    _SelectedPhoneImages.SelectedImageNames.Remove(ivm.ImageID);
                    //_SelectedPhoneImages.DeleteFile("");
                }

                _SelectedPhoneImages.RemovedImages.Add(ivm.ImageID);
            }
            _SelectedPhoneImages.DeleteFiles();
            _SelectedPhoneImages.Save();
        }

        private void PhoneApplicationPage_Unloaded_1(object sender, RoutedEventArgs e)
        {
            //Update Select Count
        }

    }
}