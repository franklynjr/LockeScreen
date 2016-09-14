//#define DEBUG_AGENT

using LockeScreenBgAgent.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Phone.System.UserProfile;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Scheduler;

namespace LockeScreen.Common
{
    class LockScreenHelper
    {

        static PeriodicTask lsPeriodicTask;
        static string lsPeriodicTaskName = "LockeScreenBgAgent";


        public static void SetNextRandomBackground( ) {
            LockScreenChange(GetNextRandomImage(), false);
        }
        public static void SetNextBackground() { }

        public static bool IsDefaultWallpaper
        {
            get
            {
                try
                {
                    if (LockScreen.GetImageUri().ToString().Contains("DefaultLockScreen.jpg"))
                        return true;
                    else
                        return false;
                }
                catch { }
                return false;
            }
        }


        private static string GetNextRandomImage()
        {
            var currentImage = LockScreen.GetImageUri();
            string imageName = string.Empty;

            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    LockScreenData lsData = new LockScreenData();

                    List<string> Images = FileOperation.GetAllFiles("*.jpg", appStore);

                    Random r = new Random();

                    int cnt = Images.Count;
                    if (cnt != 0)
                    {
                        lsData = Load();

                        if (lsData.CurrentImage == null)
                            lsData.CurrentImage = LockScreen.GetImageUri().AbsolutePath.TrimStart("local/".ToCharArray());

                        int nextIndex;
                        int safteyCounter = 0;
                        if (cnt == 1)
                        {
                            imageName = Images[0];
                            lsData.CurrentImage = imageName;
                            lsData.CurrentIndex = 0;
                        }
                        else
                        {



                            do
                            {

                                nextIndex = r.Next() % cnt;

                                //Set Lockscreen
                                if (safteyCounter > 20)
                                    break;

                                imageName = Images[nextIndex];
                                bool indRes = nextIndex == lsData.CurrentIndex;
                                bool nameRes = lsData.CurrentImage == imageName;
                                if (!(indRes && nameRes))
                                {

                                    lsData.CurrentImage = imageName;
                                    lsData.CurrentIndex = nextIndex;
                                    break;
                                }

                                safteyCounter++;
                            } while (nextIndex == lsData.CurrentIndex && lsData.CurrentImage == imageName);
                        }

                        SaveCurrent(lsData);
                    }
                    else
                    {
                        imageName = LockScreen.GetImageUri().AbsolutePath.TrimStart("local/".ToCharArray());
                    }
                }
            }
            catch { }

            return imageName;

        }


        private static  void SaveCurrent(LockScreenData lsd)
        {
            string filename = "lsData.xml";
            Mutex mutex = new Mutex(false, filename);
            try
            {
                mutex.WaitOne();
                //do something with the file....
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    XmlSerializer xmlData = new XmlSerializer(lsd.GetType());
                    IsolatedStorageFileStream fstream;

                    if (appStore.FileExists(filename))
                        appStore.DeleteFile(filename);

                    fstream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, appStore);
                    xmlData.Serialize(fstream, lsd);
                    fstream.Close();
                }

            }
            catch (Exception)
            {
                //handle exception
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        
        }


        // Start the perodic agent
        public static void StartPeriodicAgent()
        {


            System.Diagnostics.Debug.WriteLine("Starting bgAgent....");

            // if the old task is running, remove it
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
                    //MessageBox.Show("Background agents for this application have been disabled.");
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





        // Stop BgAgent
        public static void StopPeriodicAgent()
        {


            System.Diagnostics.Debug.WriteLine("Stopping bgAgent....");

            // if the old task is running, remove it
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
        }


        private static async void LockScreenChange(string filePathOfTheImage, bool isAppResource)
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                // If you're not the provider, this call will prompt the user for permission.
                // Calling RequestAccessAsync from a background agent is not allowed.
                await LockScreenManager.RequestAccessAsync();
            }

            // Only do further work if the access is granted.
            if (LockScreenManager.IsProvidedByCurrentApplication && filePathOfTheImage != string.Empty)
            {

                // ms-appdata points to the root of the local app data folder.
                // ms-appx points to the Local app install folder, to reference resources bundled in the XAP package
                var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                var uri = new Uri(schema + filePathOfTheImage, UriKind.Absolute);

                // Get the URI of the lock screen background image.
                var currentImage = LockScreen.GetImageUri();
                try
                {
                    // Set the lock screen background image.
                    LockScreen.SetImageUri(uri);
                    
                }
                catch
                {
                    Debug.WriteLine("Error setting " + filePathOfTheImage + " as background.");
                }


                System.Diagnostics.Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());

            }
            else
            {
                //MessageBox.Show("Background cant be updated as you clicked no!!");
            }
        }


        

        public static async void SetFileAsLockscreenImage(Uri AsoluteUri)
        {
            if (!LockScreenManager.IsProvidedByCurrentApplication)
            {
                // If you're not the provider, this call will prompt the user for permission.
                // Calling RequestAccessAsync from a background agent is not allowed.
                await LockScreenManager.RequestAccessAsync();
            }

            // Only do further work if the access is granted.
            if (AsoluteUri.IsAbsoluteUri)
            {

                // ms-appdata points to the root of the local app data folder.
                // ms-appx points to the Local app install folder, to reference resources bundled in the XAP package
                //var schema = isAppResource ? "ms-appx:///" : "ms-appdata:///Local/";
                //var uri = new Uri(schema + filePathOfTheImage, UriKind.Absolute);

                // Get the URI of the lock screen background image.
                var currentImage = LockScreen.GetImageUri();
                try
                {
                    // Set the lock screen background image.
                    LockScreen.SetImageUri(AsoluteUri);

                    
                }
                catch
                {
                    Debug.WriteLine("Error setting " + AsoluteUri.AbsolutePath + " as background.");
                }
                

                System.Diagnostics.Debug.WriteLine("The new lock screen background image is set to {0}", currentImage.ToString());

            }
            else
            {
                //MessageBox.Show("Background cant be updated as you clicked no!!");
            }
        }


        private static LockScreenData Load()
        {
            string filename = "lsData.xml";
            Mutex mutex = new Mutex(false, filename);
            LockScreenData lsd = new LockScreenData();
            try
            {
                mutex.WaitOne();

                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    IsolatedStorageFileStream fstream = new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, appStore);

                    try
                    {
                        XmlSerializer xmlData = new XmlSerializer(lsd.GetType());

                        lsd = (LockScreenData)xmlData.Deserialize(fstream);
                        fstream.Close();
                    }
                    catch { fstream.Close(); }

                    return lsd;
                }
            }
            catch (Exception) { }

            mutex.ReleaseMutex();

            return lsd;
        }





        public static Uri GetCurrent()
        {
            return LockScreen.GetImageUri();
        }

        public static bool IsBackgroundImageProvider { get { return LockScreenManager.IsProvidedByCurrentApplication; } }
    }
}

