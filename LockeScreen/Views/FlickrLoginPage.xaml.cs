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
using FlickrNet;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Tasks;
using LockeScreen.Common;

namespace LockeScreen.Views
{
    public partial class FlickrLoginPage : PhoneApplicationPage
    {
        Flickr flickr;
        string frob;
        string url;
        



        public FlickrLoginPage()
        {
            Flickr.CacheDisabled = true;
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(FlickrLoginPage_Loaded);
        }


        void FlickrLoginPage_Loaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

        }

        private void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            flickr = new Flickr("d1dec6db23f8c2d433a03c050e458406", "75d66064ce412813");
            flickr.AuthGetFrobAsync((fResult) =>
           {
               frob = fResult.Result;
               
               url = flickr.AuthCalcUrlMobile(frob, AuthLevel.Read);
               webBrowser1.Navigated += (o, ne) =>
               {
                   //
                   flickr.AuthGetTokenAsync(frob, (tokenResult) =>
                   {
                       //
                       if (tokenResult != null)
                       {
                           //
                           if (!tokenResult.HasError)
                           {
                               try
                               {
                                   IsolatedStorageSettings.ApplicationSettings[KEYS.FLICKR_TOKEN_KEY] = tokenResult.Result.Token;
                                   IsolatedStorageSettings.ApplicationSettings[KEYS.FLICKR_USER_ID_KEY] = tokenResult.Result.User.UserId;
                                   IsolatedStorageSettings.ApplicationSettings[KEYS.FLICKR_USER_KEY] = tokenResult.Result.User.UserName;
                               }
                               catch { }
                               IsolatedStorageSettings.ApplicationSettings.Save();
                               if (NavigationService.CanGoBack)
                               {
                                   Shared.ApplicationData[KEYS.RELOAD_FLICKR_KEY] = true;
                                   NavigationService.GoBack();
                               }
                           }
                       }
                   });
               };


               webBrowser1.Navigate(new Uri(url));
           });
        }




    }
}