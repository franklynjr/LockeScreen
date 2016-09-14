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
using LockeScreen.Models;
using System.Collections.ObjectModel;

namespace Sample.Views
{
    public partial class FlickrAlbum : PhoneApplicationPage
    {
        public FlickrAlbum()
        {
            InitializeComponent();
            Albums = new ObservableCollection<AlbumPreview>();
            this.Loaded += new RoutedEventHandler(FlickrAlbum_Loaded);
        }
        ObservableCollection<AlbumPreview> Albums;
        void FlickrAlbum_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();
            Dispatcher.BeginInvoke(() =>
                           {
                               FlickrHelper flickrHelper = new FlickrHelper();
                               flickrHelper.LoginAttempted += (lRes) =>
                               {
                                   if (lRes == ResultCallbacks.LoginResult.SUCCESS)
                                   {
                                       flickrHelper.GetAllAlbums((albRes) =>
                                          {

                                              Albums = albRes;
                                          });

                                   }
                                   else
                                       NavigationService.Navigate(new Uri("/Views/FlickrLoginPage.xaml", UriKind.RelativeOrAbsolute));
                               };
                               flickrHelper.Login();
                           });
        }
    }
}