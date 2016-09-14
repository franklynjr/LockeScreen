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
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;

namespace LockeScreen.Common
{
    public class LsFileOperation
    {

        public static int SavedImagesCount(string SourceName)
        {
            int Count = 0;
            if (SourceName != "" || SourceName != null)
            {
                using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (isf.DirectoryExists("SavedImages/" + SourceName))
                    {
                        string[] files = isf.GetFileNames("SavedImages/" + SourceName + "/*");

                        return files.Length;
                    }
                }
            }

            return Count;
        }



        public static bool CreateDirectory(string Dir)
        {
            try
            {


                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    if (!appStore.DirectoryExists(Dir))
                    {
                        appStore.CreateDirectory(Dir);
                        return true;
                    }
                }
            }
            catch
            {
                return false;
            }


            return false;
        }


        public static bool SaveImage(string ImageDestination, Stream ImageStream)
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    BitmapImage image = new BitmapImage();
                    image.SetSource(ImageStream);
                    IsolatedStorageFileStream fStream = new IsolatedStorageFileStream(ImageDestination, FileMode.Create, appStore);

                    WriteableBitmap wbmp = new WriteableBitmap(image);

                    System.Windows.Media.Imaging.Extensions.SaveJpeg(wbmp, fStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);

                    wbmp.SaveJpeg(fStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);

                    fStream.Close();

                    image = null;

                    return true;
                }

            }
            catch { }

            return false;
        }


        public static bool DeleteImage(string filename)
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    if (appStore.FileExists(filename))
                    {
                        appStore.DeleteFile(filename);

                    }
                }
            }
            catch { }

            return false;
        }



        internal static void DeleteAll()
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    DeleteAllFiles();
                    appStore.Remove();
                }
            }
            catch { }

        }

        internal static void DeleteDirectory(string filename)
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                }
            }
            catch { }

        }

        internal static void DeleteAllFiles()
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    string[] files = appStore.GetFileNames();
                    try
                    {
                        foreach (string s in files)
                            appStore.DeleteFile(s);
                    }
                    catch {
                        Debug.WriteLine("An Error occurred while attemping to delete file");
                    }
                }
            }
            catch { }

        }

        
    }
}
