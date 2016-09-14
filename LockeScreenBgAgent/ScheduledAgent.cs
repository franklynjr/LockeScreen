//#define DEBUG_AGENT

using System.Diagnostics;
using System.Windows;
using Microsoft.Phone.Scheduler;
using Windows.Phone.System.UserProfile;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml;
using System.Xml.Serialization;

using System;
using LockeScreenBgAgent.Classes;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Phone.Shell;

namespace LockeScreenBgAgent
{
    public class ScheduledAgent : ScheduledTaskAgent
    {
        /// <remarks>
        /// ScheduledAgent constructor, initializes the UnhandledException handler
        /// </remarks>
        static ScheduledAgent()
        {

            // Subscribe to the managed exception handler
            Deployment.Current.Dispatcher.BeginInvoke(delegate
            {
                Application.Current.UnhandledException += UnhandledException;
            });
        }

        /// Code to execute on Unhandled Exceptions
        private static void UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
        }

        /// <summary>
        /// Agent that runs a scheduled task
        /// </summary>
        /// <param name="task">
        /// The invoked task
        /// </param>
        /// <remarks>
        /// This method is called when a periodic or resource intensive task is invoked
        /// </remarks>
        protected override void OnInvoke(ScheduledTask task)
        {
            LockScreenChange(GetNextRandomImage(), false);

            // If debugging is enabled, launch the agent again in one minute.
            // debug, so run in every 30 secs

#if(DEBUG_AGENT)

                ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
                System.Diagnostics.Debug.WriteLine("Periodic task is started again: " + task.Name);
#endif
                //try
                //{
                //    ScheduledActionService.LaunchForTest(task.Name, TimeSpan.FromSeconds(30));
                //    System.Diagnostics.Debug.WriteLine("Periodic task " + task.Name + " at " + DateTime.Now.TimeOfDay.ToString());
                //}
                //catch { }
            // Call NotifyComplete to let the system know the agent is done working.
            NotifyComplete();
        }







        private string GetNextRandomImage()
        {
            string imageName = string.Empty;
            if (LockScreenManager.IsProvidedByCurrentApplication)
            {
                var currentImage = LockScreen.GetImageUri();

                try
                {
                    using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                    {

                        LockScreenData lsData = new LockScreenData();

                        List<string> Images = GetAllFiles("*.jpg", appStore);

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

            }
            return imageName;

        }


        private void LockScreenChange(string filePathOfTheImage, bool isAppResource)
        {
            try{

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


                    
                    /*
                    foreach (ShellTile st in ShellTile.ActiveTiles)
                    {
                        
                        st.Update(new FlipTileData()
                        {
                            //SmallBackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileSmall.png", UriKind.Relative),
                            BackgroundImage = uri
                            //BackBackgroundImage = new Uri(@"Assets\Tiles\FlipCycleTileMedium.png", UriKind.Relative)
                        });
                        Debug.WriteLine("update tileno {0}",i.ToString());

                        i++;
                    }
                */


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
            
            }catch{}
        }

        private void SaveCurrent(LockScreenData lsd)
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

                fstream= new IsolatedStorageFileStream(filename, FileMode.OpenOrCreate, appStore);
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

        private LockScreenData Load()
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

                }
            }
            catch (Exception) { }
            finally
            {
                mutex.ReleaseMutex();
            }
                    return lsd;
        }



        // Method to retrieve all directories, recursively, within a store. 
        public static List<String> GetAllDirectories(string pattern, IsolatedStorageFile storeFile)
        {
            // Get the root of the search string. 
            string root = Path.GetDirectoryName(pattern);

            if (root != "")
            {
                root += "/";
            }

            // Retrieve directories.
            List<String> directoryList = new List<String>(storeFile.GetDirectoryNames(pattern));

            // Retrieve subdirectories of matches. 
            for (int i = 0, max = directoryList.Count; i < max; i++)
            {
                string directory = directoryList[i] + "/";
                List<String> more = GetAllDirectories(root + directory + "*", storeFile);

                // For each subdirectory found, add in the base path. 
                for (int j = 0; j < more.Count; j++)
                {
                    more[j] = directory + more[j];
                }

                // Insert the subdirectories into the list and 
                // update the counter and upper bound.
                directoryList.InsertRange(i + 1, more);
                i += more.Count;
                max += more.Count;
            }

            return directoryList;
        }



        public static List<String> GetAllFiles(string pattern, IsolatedStorageFile storeFile)
        {
            // Get the root and file portions of the search string. 
            string fileString = Path.GetFileName(pattern);

            List<String> fileList = new List<String>(storeFile.GetFileNames(pattern));

            // Loop through the subdirectories, collect matches, 
            // and make separators consistent. 
            foreach (string directory in GetAllDirectories("*", storeFile))
            {
                foreach (string file in storeFile.GetFileNames(directory + "/" + fileString))
                {
                    fileList.Add((directory + "/" + file));
                }
            }

            return fileList;
        } // End of GetFiles.
    }
}