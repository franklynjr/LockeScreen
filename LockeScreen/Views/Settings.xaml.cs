#define DEBUG_AGENT

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
using System.IO.IsolatedStorage;

using LockeScreen.Common;
using Windows.Phone.System.UserProfile;
using LockeScreen.ViewModels;
using System.Threading;
using LockeScreen.Models;

namespace LockeScreen.Views
{
    public partial class Settings  : PhoneApplicationPage
    {

        SettingsViewModel AppSettings;
        int source_count = 0;

        public Settings()
        {
            AppSettings = new SettingsViewModel();
            InitializeComponent();
            LoadSettingsAsync();
            this.DataContext = AppSettings;
            this.Loaded += Settings_Loaded;
            this.Unloaded += Settings_Unloaded;
            
        }

        void Settings_Unloaded(object sender, RoutedEventArgs e)
        {
            //throw new NotImplementedException();

            Shared.ApplicationData[SettingsModel.SettingsKeys.UPDATE_PANORAMA] = true;

            AppSettings.Settings.Save();
        }

        void Settings_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnReset_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Doing this will reset all customized settings for this application. Are you sure you want to perform this action? Select 'OK'  to continue.", "Reset LockeScreen", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                IsolatedStorageSettings.ApplicationSettings.Clear();
                FileOperation.DeleteAll();
            }
        }

        private void btnSetAs_Click_1(object sender, RoutedEventArgs e)
        {
            RequestDefault();
        }

        private async void RequestDefault()
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {

                await LockScreenManager.RequestAccessAsync();
                
            }
        }

        private void ToastOn_Checked_1(object sender, RoutedEventArgs e)
        {
            AppSettings.Settings.ToastEnabled = true;
        }

        private void ToastOff_Checked_1(object sender, RoutedEventArgs e)
        {

            AppSettings.Settings.ToastEnabled = false;
        }

        private void LoadSettingsAsync()
        {
            Thread loadControlsAsync = new Thread(new ThreadStart(LoadSettings));
            loadControlsAsync.Start();
        }


        private void LoadSettings()
        {
            AppSettings.Settings.Load();
            SelectToastButton();
        }

        private void SelectToastButton()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (AppSettings.Settings.ToastEnabled)
                    ToastOn.IsChecked = true;
                else
                    ToastOff.IsChecked = true;
            });
        }

        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            
                LockScreenHelper.StartPeriodicAgent();
            
                

        }

        private void CheckBox_Unchecked_1(object sender, RoutedEventArgs e)
        {
            LockScreenHelper.StopPeriodicAgent();
        }

        private void btnClearPhone_Click_1(object sender, RoutedEventArgs e)
        {
            if(MessageBox.Show("Remove the images you selected from your phone?","Clear Selected Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                AppSettings.Settings.ClearPhone();
        }

        private void btnCleaflickr_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove the images you selected from Flickr?", "Clear Selected Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                AppSettings.Settings.ClearFlickr();
        }

        private void btnClearfacebook_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove the images you selected from Facebook?", "Clear Selected Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                AppSettings.Settings.ClearFacebook();
        }

        private void btnClearAll_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Remove all selected images?", "Clear Selected Images", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            AppSettings.Settings.ClearAll();
        }

        private void btnCleaNote_Click_1(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Clear note?", "Clear note", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                var bg = LockScreenHelper.GetCurrent();
                Microsoft.Phone.Shell.ShellTile.ActiveTiles.First().Update(
                    new Microsoft.Phone.Shell.FlipTileData()
                    {
                        WideBackContent = ""
                    });
                Microsoft.Phone.Shell.FlipTileData ftd = new Microsoft.Phone.Shell.FlipTileData();
                IsolatedStorageSettings.ApplicationSettings.Remove(KEYS.NOTE); 
            }
        }

        private void phoneEnabled_Unchecked_1(object sender, RoutedEventArgs e)
        {
            if (source_count == 1)
            {
                MessageBox.Show("you must have at least one active source.");
                phoneEnabled.IsChecked = true;
                source_count--;
            }
            else
                source_count--;
        }

        private void flickrEnabled_Unchecked_1(object sender, RoutedEventArgs e)
        {

            if (source_count == 1)
            {
                MessageBox.Show("you must have at least one active source.");
                flickrEnabled.IsChecked = true;
                source_count--;
            }
            else
                source_count--;
        }

        private void facebookEnabled_Unchecked_1(object sender, RoutedEventArgs e)
        {

            if (source_count == 1)
            {
                MessageBox.Show("you must have at least one active source.");
                facebookEnabled.IsChecked = true;
                source_count--;
            }
            else
                source_count--;
        }

        private void flickrEnabled_Checked_1(object sender, RoutedEventArgs e)
        {
            source_count++;
        }

        private void phoneEnabled_Checked_1(object sender, RoutedEventArgs e)
        {
            source_count++;
        }

        private void facebookEnabled_Checked_1(object sender, RoutedEventArgs e)
        {
            source_count++;
        }
    }
}