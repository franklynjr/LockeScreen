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

namespace LockeScreen
{
    public partial class CopyMainPage : PhoneApplicationPage
    {
        private AlbumListViewModel albVm;


        PeriodicTask lsPeriodicTask;
        string lsPeriodicTaskName = "LockeScreenBgAgent";


        // Constructor
        public CopyMainPage()
        {
            InitializeComponent();
            
            // Set the data context of the listbox control to the sample data
            albVm = new AlbumListViewModel();
            albVm.FacebookAlbums = albVm.PhoneAlbums;
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = albVm;
            object LoadFacebook = new object();

            if (Shared.ApplicationData.TryGetValue("ReloadFacebook", out LoadFacebook))
                if ((bool)LoadFacebook)
                {
                    albVm.FacebookAlbumsLoaded = false;
                    albVm.ReloadFacebook();
                    Shared.ApplicationData["ReloadFacebook"] = false;
                }

            albVm.ReloadStorageAsync();
            
            if(LockScreenHelper.IsDefaultWallpaper)
                LockScreenHelper.SetNextRandomBackground();
            
            StartPeriodicAgent();

        }

        private void lbxAlbums_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lbxAlbums.SelectedIndex != -1)
                this.NavigationService.Navigate(new Uri("/Views/PhoneImageSelector.xaml?indx=" + lbxAlbums.SelectedIndex.ToString(), UriKind.RelativeOrAbsolute));

            lbxAlbums.SelectedIndex = -1;
        }

        private void btnSettings_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/Settings.xaml", UriKind.RelativeOrAbsolute));
        }



        private void StartPeriodicAgent()
        {


            System.Diagnostics.Debug.WriteLine("Starting bgAgent....");
            // is old task running, remove it
            lsPeriodicTask = ScheduledActionService.Find(lsPeriodicTaskName) as PeriodicTask;
            
            if (lsPeriodicTask != null)
            {
                try
                {
                    ScheduledActionService.Remove(lsPeriodicTaskName);
                }
                catch (Exception)
                {
                }
            }

            // create a new task
            lsPeriodicTask = new PeriodicTask(lsPeriodicTaskName);
            // load description from localized strings
            lsPeriodicTask.Description = "Locke Screen - Lockscreen image provider.";
            // set expiration days
            //lsPeriodicTask.ExpirationTime = DateTime.Now.AddDays(14);
            try
            {
                // add this to scheduled action service
                ScheduledActionService.Add(lsPeriodicTask);
                // debug, so run in every 30 secs
#if(DEBUG_AGENT)
                ScheduledActionService.LaunchForTest(lsPeriodicTaskName, TimeSpan.FromSeconds(30));
                System.Diagnostics.Debug.WriteLine("Periodic task is started: " + lsPeriodicTaskName);
#endif


                //ScheduledActionService.LaunchForTest(lsPeriodicTaskName, TimeSpan.FromSeconds(30));
                
                System.Diagnostics.Debug.WriteLine("Periodic task " + lsPeriodicTaskName + " at " + DateTime.Now.TimeOfDay.ToString());

            }
            catch (InvalidOperationException exception)
            {
                if (exception.Message.Contains("BNS Error: The action is disabled"))
                {
                    // load error text from localized strings
                    MessageBox.Show("Background agents for this application have been disabled.");
                }
                if (exception.Message.Contains("BNS Error: The maximum number of ScheduledActions of this type have already been added."))
                {
                    // No user action required. The system prompts the user when the hard limit of periodic tasks has been reached.
                }
            }
            catch (SchedulerServiceException)
            {
                // No user action required.
            }
        }

        private void fbLogin_Click_1(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Views/FacebookLoginPage.xaml", UriKind.Relative));
            //MessageBox.Show("my visibility " + btnFbLogin.Visibility.ToString() + " my binding val " + albVm.UserIsLoggedIn);
            //MessageBox.Show("fbpbar visibility " + Fbpgb.Visibility.ToString());

            //Dispatcher.BeginInvoke(() =>
            //{
            //    MessageBox.Show("Facebook Album Count: " + albVm.FacebookAlbums.Count.ToString());
            //    MessageBox.Show("Phone Album Count: " + albVm.PhoneAlbums.Count.ToString());
            //});
        }

        private void lbxFacebookAlbums_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

            if (lbxFacebookAlbums.SelectedIndex != -1)
                this.NavigationService.Navigate(new Uri("/Views/FacebookImageSelector.xaml?indx=" + lbxFacebookAlbums.SelectedIndex.ToString(), UriKind.RelativeOrAbsolute));

            lbxFacebookAlbums.SelectedIndex = -1;
        }
    }
}