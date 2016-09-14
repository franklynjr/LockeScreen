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
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace LockeScreen.Models
{
    public class ModelBase: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool phone_Loaded = false;

        private bool fb_loaded = false;
        private bool fb_Images_loaded = false;
        private bool fb_logged_in = false;

        private bool flk_loaded = false;
        private bool flkr_logged_in = false;

        private bool selected_loaded = false;
        
        public void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
            }
        }

        public bool PhoneAlbumsLoaded
        {
            get { return phone_Loaded; }
            set
            {
                if (phone_Loaded != value)
                {
                    phone_Loaded = value;
                    NotifyPropertyChanged("PhoneAlbumsLoaded");
                }
            }
        }

        public bool SelectedImagesLoaded
        {
            get { return selected_loaded; }
            set
            {
                if (selected_loaded != value)
                {
                    selected_loaded = value;
                    NotifyPropertyChanged("SelectedImagesLoaded");
                }
            }
        }


        public bool FacebookImagesLoaded
        {
            get { return fb_Images_loaded; }
            set
            {
                if (fb_Images_loaded != value)
                {
                    fb_Images_loaded = value;

                    NotifyPropertyChanged("FacebookImagesLoaded");
                }
            }
        }


        public bool FacebookAlbumsLoaded
        {
            get { return fb_loaded; }
            set
            {
                if (fb_loaded != value)
                {
                    fb_loaded = value;

                    NotifyPropertyChanged("FacebookAlbumsLoaded");
                }
            }
        }


        public bool FlickrAlbumsLoaded
        {
            get { return flk_loaded; }
            set
            {
                if (flk_loaded != value)
                {
                    flk_loaded = value;

                    NotifyPropertyChanged("FlickrAlbumsLoaded");
                }
            }
        }


        public bool FlickrUserIsLoggedIn
        {
            get { return flkr_logged_in; }
            set
            {
                if (flkr_logged_in != value)
                {
                    flkr_logged_in = value;
                    NotifyPropertyChanged("FlickrUserIsLoggedIn");
                }
            }
        }

        public bool FacebookUserIsLoggedIn
        {
            get { return fb_logged_in; }
            set
            {
                if (fb_logged_in != value)
                {
                    fb_logged_in = value;
                    NotifyPropertyChanged("FacebookUserIsLoggedIn");
                }
            }
        }



        protected static Dispatcher Dispatcher
        {
            get { return Deployment.Current.Dispatcher; }
        }

    }
}
