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
using LockeScreen.Common;
using Microsoft.Phone.Shell;

namespace LockeScreen.Views
{
    public partial class FacebookImageSelector : PhoneApplicationPage
    {
        // Image Grid ImageListViewModel
        static ImageListViewModel _ImageListViewModel;

        SelectedImages _SelectedFacebookImages;


        public FacebookImageSelector()
            : base()
        {
            InitializeComponent();
            _ImageListViewModel = new ImageListViewModel();
            _SelectedFacebookImages = new SelectedImages(AlbumPreview.ALBUM_TYPE.FACEBOOK);
            this.Loaded += new RoutedEventHandler(FacebookImageSelector_Loaded);
            this.DataContext = _ImageListViewModel;

        }

        //  Not used - Possible Future implementation
        //public static readonly DependencyProperty ListVerticalOffsetProperty = DependencyProperty.Register(
        //                          "ListVerticalOffset",
        //                          typeof(double),
        //                          typeof(FacebookImageSelector),
        //                          new PropertyMetadata(new PropertyChangedCallback(OnListVerticalOffsetChanged)));

        //private double _lastFetch; // Not used

        // Not used -  Possible Future implementation
        //private static void OnListVerticalOffsetChanged(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        //{
        //    //throw new NotImplementedException();
        //    FacebookImageSelector page = depObj as FacebookImageSelector;
        //    ScrollViewer viewer = page._listScrollViewer;

        //    if (viewer != null)
        //    {
        //        if (page._lastFetch < viewer.ScrollableHeight)
        //        {
        //            // Trigger within 1/4 the viewport.
        //            if (viewer.VerticalOffset >= (viewer.ScrollableHeight - viewer.ViewportHeight / 4))
        //            {
        //                page._lastFetch = viewer.ScrollableHeight;
        //                _ImageListViewModel.GetNextFacebookPage();
        //            }
        //        }
        //    }
        //}
        

         //Not used Possible Future implementation
        //public double ListVerticalOffset
        //{
        //    get { return (double)this.GetValue(ListVerticalOffsetProperty); }
        //    set { this.SetValue(ListVerticalOffsetProperty, value); }
        //}

        void FacebookImageSelector_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Selected Album index
                int indx = int.Parse(NavigationContext.QueryString["indx"]);


                if (indx != -1)
                {
                    _ImageListViewModel.Images = AlbumsViewModel.FacebookAlbums[indx].ImageThumbnails;
                    _SelectedFacebookImages.AlbumName = AlbumsViewModel.FacebookAlbums[indx].Name;
                    _SelectedFacebookImages.Load();
                }

                //DataContext = _ImageListViewModel;

                ////Select all previously selected images
                //foreach (ImageViewModel li in lbxThumbnails.Items)
                //{
                //    // 
                //    if (_SelectedFacebookImages.SelectedImageNames.Contains(li.ImageID))
                //    {
                //        lbxThumbnails.SelectedItems.Add(li); // Select the image
                //    }
                //}


                string AlbumID = this.NavigationContext.QueryString["selectedItem"];
                
                object LoggedInvalue = false;

                if (Shared.ApplicationData.TryGetValue(indx.ToString(), out LoggedInvalue))
                {
                    _ImageListViewModel.FacebookImagesLoaded = (bool)LoggedInvalue;
                }

                if (!_ImageListViewModel.FacebookImagesLoaded)
                {
                    _ImageListViewModel.LoadAlbumAsync(AlbumID, (images) =>
                    {
                        AlbumsViewModel.FacebookAlbums[indx].ImageThumbnailsLoaded = true;
                        AlbumsViewModel.FacebookAlbums[indx].ImageThumbnails = images;
                        _ImageListViewModel.Images = images;

                        Shared.ApplicationData[indx.ToString()] = _ImageListViewModel.FacebookImagesLoaded =  true;
                        
                        foreach (ImageViewModel li in lbxThumbnails.Items)
                        {
                            // 
                            if (_SelectedFacebookImages.SelectedImageNames.Contains(li.ImageID))
                            {
                                lbxThumbnails.SelectedItems.Add(li); // Select the image
                            }
                        }
                    });
                }
                else
                {

                    foreach (ImageViewModel li in lbxThumbnails.Items)
                    {
                        // 
                        if (_SelectedFacebookImages.SelectedImageNames.Contains(li.ImageID))
                        {
                            lbxThumbnails.SelectedItems.Add(li); // Select the image
                        }
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

                if (!_SelectedFacebookImages.SelectedImageNames.Contains(ivm.ImageID)){

                    _SelectedFacebookImages.SelectedImageNames.Add(ivm.ImageID);
                    _SelectedFacebookImages.SaveFacebookImage(ivm.ImageID);
                }

                if (_SelectedFacebookImages.RemovedImages.Contains(ivm.ImageID))
                {
                    _SelectedFacebookImages.RemovedImages.Remove(ivm.ImageID);

                   

                }
            }

            foreach (ImageViewModel ivm in e.RemovedItems)
            {

                while (_SelectedFacebookImages.SelectedImageNames.Contains(ivm.ImageID))
                {
                    _SelectedFacebookImages.SelectedImageNames.Remove(ivm.ImageID);
                }

                _SelectedFacebookImages.RemovedImages.Add(ivm.ImageID);
            }

            _SelectedFacebookImages.DeleteFiles();
            _SelectedFacebookImages.Save();
        }

        private void FacebookApplicationPage_Unloaded_1(object sender, RoutedEventArgs e)
        {
            //Update Select Count
        }


        private bool selectionEnabled = false;
        private void toggleSelection_Click_1(object sender, EventArgs e)
        {
            ApplicationBarIconButton toggleSelection_new = (ApplicationBarIconButton)ApplicationBar.Buttons[0];
            if (selectionEnabled)
            {
                toggleSelection_new.IconUri = new Uri("/Assets/ApplicationBar.Check.png", UriKind.RelativeOrAbsolute);
                toggleSelection_new.Text = "Start Selection";
                selectionEnabled = true;
                
            }
            else
            {toggleSelection_new.IconUri = new Uri("/Assets/transport.play.png", UriKind.RelativeOrAbsolute);
                toggleSelection_new.Text = "Viewer";
                selectionEnabled = false;
            }
        }

        //ScrollViewer _listScrollViewer;

        //private void ListScrollViewer_Loaded_1(object sender, RoutedEventArgs e)
        //{
        //    _listScrollViewer = sender as ScrollViewer;

        //    Binding binding = new Binding();
        //    binding.Source = _listScrollViewer;
        //    binding.Path = new PropertyPath("VerticalOffset");
        //    binding.Mode = BindingMode.OneWay;
        //    this.SetBinding(ListVerticalOffsetProperty, binding);
        //}

    }
}