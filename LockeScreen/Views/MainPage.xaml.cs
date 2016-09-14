using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using LockeScreen.ViewModels;
using Microsoft.Phone.Scheduler;
using LockeScreen.Common;
using LockeScreen.Models;
using Coding4Fun.Toolkit.Controls;
using System.Diagnostics;
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;

namespace LockeScreen
{
    public partial class MainPage : PhoneApplicationPage
    {
        private AlbumListViewModel albumsViewModel;
        object isFirstLoad;
        object updatePanorama;
        object backgroundChanged;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            
            // Set the data context of the listbox control to the sample data
            albumsViewModel = new AlbumListViewModel();
            this.DataContext = albumsViewModel;

            
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            if(!LockScreenHelper.IsBackgroundImageProvider)
                SetLockScreen();


            ReloadFacebook();
            ReloadFlickr();
            Shared.LoadSettings();


            // Change the background if an image is selected
            if (LockScreenHelper.IsDefaultWallpaper && LockScreenHelper.IsBackgroundImageProvider)
            {
                LockScreenHelper.SetNextRandomBackground();
            }

            if (LockScreenHelper.IsBackgroundImageProvider && (!Shared.ApplicationData.TryGetValue(KEYS.FIRST_LOAD, out isFirstLoad) ||  Shared.ApplicationData.TryGetValue(KEYS.BACKGROUND_CHANGED, out backgroundChanged)))
            {
                UpdateBackground();
                Shared.ApplicationData.Remove(KEYS.BACKGROUND_CHANGED);
                
            }

            if (!Shared.ApplicationData.TryGetValue(KEYS.FIRST_LOAD, out isFirstLoad) && Shared.Settings.DynamicLockScreen)
            {
                LockScreenHelper.StartPeriodicAgent();
            }

            Shared.ApplicationData[KEYS.FIRST_LOAD] = false;


            if (Shared.ApplicationData.TryGetValue(SettingsModel.SettingsKeys.UPDATE_PANORAMA, out updatePanorama))
            {
                if ((bool)updatePanorama)
                    Shared.Settings.Load((sc) =>
                    {
                        UpdatePanorama();
                    });

                // set to null
                Shared.ApplicationData.Remove(SettingsModel.SettingsKeys.UPDATE_PANORAMA);
            }

            LoadNotificationText();

            //Reload for quick set
            //albumsViewModel.ReloadSelectedImagesAsync();


        }

        private async void SetLockScreen()
        {
            await Windows.Phone.System.UserProfile.LockScreenManager.RequestAccessAsync();
        }

        private void UpdateBackground()
        {

            IsolatedStorageFile appStorage = IsolatedStorageFile.GetUserStoreForApplication();

            Storyboard sbFadeIn = new Storyboard();


            sbFadeIn.Completed += (o, _e) =>
            {
                try
                {
                    BitmapImage __CurrentImage = new BitmapImage();
                    string abs = LockScreenHelper.GetCurrent().AbsolutePath.TrimStart("Local/".ToCharArray());
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(abs, System.IO.FileMode.Open, appStorage))
                    {
                        __CurrentImage.SetSource(fs);

                        ImageBrush imageBrush = new ImageBrush();
                        imageBrush.ImageSource = __CurrentImage;

                        this.pItems.Background = imageBrush;
                        Storyboard sbFadeOut = new Storyboard();

                        FadeInOut(this.pItems.Background, sbFadeOut, false);


                        appStorage.Dispose();
                    }
                }
                catch { }
            };

            FadeInOut(this.pItems.Background, sbFadeIn, true);


        }


        private void FadeInOut(DependencyObject target, Storyboard sb, bool isFadeIn)
        {
            Duration d = new Duration(TimeSpan.FromSeconds(1));
            DoubleAnimation daFade = new DoubleAnimation();
            daFade.Duration = d;
            if (isFadeIn)
            {
                daFade.From = 1.00;
                daFade.To = 0.00;
            }
            else
            {
                daFade.From = 0.00;
                daFade.To = 1.00;
            }

            sb.Duration = d;
            sb.Children.Add(daFade);
            Storyboard.SetTarget(daFade, target);
            Storyboard.SetTargetProperty(daFade, new PropertyPath("Opacity"));

            sb.Begin();
        }

        private void UpdatePanorama()
        {
            if (pItems != null)
            {
                if (((Panorama)pItems).Items != null || pItems.Items.Count > 0)
                {
                    foreach (PanoramaItem pi in pItems.Items)
                    {
                        if (pi.Visibility == System.Windows.Visibility.Visible)
                        {
                            pItems.DefaultItem = pi;
                            break;
                        }
                    }
                }
            }
        }


        private void ReloadFlickr()
        {
            object loadFlickr = false;
            if (Shared.ApplicationData.TryGetValue(KEYS.RELOAD_FLICKR_KEY, out loadFlickr))
                if ((bool)loadFlickr)
                {
                    albumsViewModel.FlickrAlbumsLoaded = false;
                    albumsViewModel.FlickrUserIsLoggedIn = true;
                    albumsViewModel.ReloadFlickrAsync();
                    Shared.ApplicationData[KEYS.RELOAD_FLICKR_KEY] = false;
                }

        }

        //
        private void ReloadFacebook()
        {
            object LoadFacebook = new object();

            if (Shared.ApplicationData.TryGetValue(KEYS.RELOAD_FACEBOOK_KEY, out LoadFacebook))
            {
                albumsViewModel.FacebookAlbumsLoaded = false;
                albumsViewModel.FacebookUserIsLoggedIn = true;
                albumsViewModel.ReloadFacebook();
                Shared.ApplicationData.Remove(KEYS.RELOAD_FACEBOOK_KEY);
            }
        }


        //Go to Phone Image Selection Page
        private void lbxAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lbxAlbums.SelectedIndex != -1)
                this.NavigationService.Navigate(new Uri("/Views/PhoneImageSelector.xaml?indx=" + lbxAlbums.SelectedIndex.ToString(), UriKind.RelativeOrAbsolute));

            lbxAlbums.SelectedIndex = -1;
        }


        // Go to settings Page
        private void btnSettings_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }


        // Go to status Page
        private void btnStatus_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Status.xaml", UriKind.RelativeOrAbsolute));
        }


      
        

        private void fbLogin_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.Relative));
            
        }

        private void lbxFacebookAlbums_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {


            if (lbxFacebookAlbums.SelectedItem != null)
                this.NavigationService.Navigate(new Uri("/Views/FacebookImageSelector.xaml?selectedItem=" + ((AlbumPreview)lbxFacebookAlbums.SelectedItem).AlbumID + "&indx=" +
                    lbxFacebookAlbums.SelectedIndex.ToString(), UriKind.RelativeOrAbsolute));

            lbxFacebookAlbums.SelectedItem = null;

            //if (lbxFacebookAlbums.SelectedIndex != -1)
            //    this.NavigationService.Navigate(new Uri("/Views/FacebookImageSelector.xaml?indx=" + lbxFacebookAlbums.SelectedIndex.ToString(), UriKind.RelativeOrAbsolute));

            //lbxFacebookAlbums.SelectedIndex = -1;
        }

        private void btnFlkrLogin_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/Views/FlickrLoginPage.xaml", UriKind.RelativeOrAbsolute));

        }

        private void Flickr_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if(lbxFlickr.SelectedItem != null)
                this.NavigationService.Navigate(new Uri("/Views/FlickrImageSelector.xaml?selectedItem=" + ((AlbumPreview)lbxFlickr.SelectedItem).Name.ToString(), UriKind.RelativeOrAbsolute));
            lbxFlickr.SelectedItem = null;
        }


        private void Settings_click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FlickrImageSelector.xaml", UriKind.RelativeOrAbsolute));
        }


        private void appBarSettings_Click_1(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }

        private void Status_Click_1(object sender, EventArgs e)
        {

            NavigationService.Navigate(new Uri("/Views/Status.xaml", UriKind.RelativeOrAbsolute));
        }


        private void Panorama_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
        //    var item = e.AddedItems[0];
        //    switch (((PanoramaItem)item).Header.ToString())
        //    {
        //        case "quick set":
                   
        //                ToastPrompt toast = new ToastPrompt();
        //                Shared.ShowToast("Tap any image to set as lockscreen background");
                       
                    
        //            break;
        //    }
        }

        private void lbxQuickSet_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems.Count > 0)
                {
                    var image = e.AddedItems[0];
                    LockScreenHelper.SetFileAsLockscreenImage(((ImageViewModel)image).Uri);

                    Shared.ShowToast("Lockscreen image upadated.");
                }
            }
            catch { }
        }

        private async void btnGoToLockscreenSettings_Click_1(object sender, RoutedEventArgs e)
        {
            // Launch URI for the lock screen settings screen.
            var op = await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings-lock:"));

        }
        
        
        private void btnAdd_Click_1(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                //var bg = LockScreenHelper.GetCurrent();

                //foreach (ShellTile shellTile in ShellTile.ActiveTiles)
                //{
                //ShellTileSchedule sts = new ShellTileSchedule(shellTile);
                //sts.StartTime = DateTime.Now;
                //sts.Recurrence = UpdateRecurrence.Onetime;
                //sts.RemoteImageUri = bg;

                //sts.Start();
                //}

                ShellTile.ActiveTiles.First().Update(
                    new FlipTileData()
                    {
                        //Count = 99,
                        WideBackContent = lsText.Text,
                        //SmallBackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileSmall.png", UriKind.Relative),
                        //SmallBackgroundImage = bg
                       // WideBackgroundImage = LockScreenHelper.GetCurrent()
                        
                    });

                FlipTileData ftd = new FlipTileData();

                System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[KEYS.NOTE] = lsText.Text;

               
                Shared.ShowToast("notification updated");
                
                btnAdd.Focus();
                    
                //System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings[KEYS.NOTE] = albumsViewModel.Note;

                //object o = lsText.Text;
                //Shared.ApplicationData.Add(KEYS.NOTE, o);
                
            });
            
        }

        private void LoadNotificationText()
        {
            Dispatcher.BeginInvoke(() =>
            {
                object data = new object();
                if (System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings.TryGetValue(KEYS.NOTE, out data))
                    albumsViewModel.Note = (string)data;
            });
            
        }

        private void lsText_KeyUp_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btnAdd_Click_1(this, new RoutedEventArgs());
        }

        private void appBarNextBg_Click_1(object sender, EventArgs e)
        {
            if (LockScreenHelper.IsBackgroundImageProvider)
            {
                LockScreenHelper.SetNextRandomBackground();
                UpdateBackground();
            }
        }

        private void btnQuickset_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/QuickSet.xaml", UriKind.RelativeOrAbsolute));
        }



        

    }
}