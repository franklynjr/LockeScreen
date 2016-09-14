using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.IO.IsolatedStorage;
using System.Windows.Media.Imaging;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace LockeScreen.Common
{
    public class FileOperation
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

        public static bool SaveImage(string ImageDestination, BitmapImage Image)
        {
            try
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {

                    IsolatedStorageFileStream fStream = new IsolatedStorageFileStream(ImageDestination, FileMode.Create, appStore);

                    WriteableBitmap wbmp = new WriteableBitmap(Image);

                    System.Windows.Media.Imaging.Extensions.SaveJpeg(wbmp, fStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);

                    wbmp.SaveJpeg(fStream, wbmp.PixelWidth, wbmp.PixelHeight, 0, 85);

                    fStream.Close();

                    Image = null;

                    return true;
                }

            }
            catch { }

            return false;
        }


        public static bool DeleteFile(string filename)
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
                    appStore.DeleteDirectory("*/" + filename);
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
                    catch
                    {
                        Debug.WriteLine("An Error occurred while attemping to delete file");
                    }
                }
            }
            catch { }

        }

        internal static string CustomFilename(string ImageName)
        {
            string Unique_prefix = DateTime.Now.Ticks.ToString();
            string modifiedName = ImageName.Substring(0, 6).Replace(' ', '_');
            string newName = "";
            try
            {
                if (ImageName.Length > 0)
                {
                    newName = Unique_prefix + "_" +modifiedName + ".jpg";
                    return newName;
                }
            }
            catch { }

            newName = Unique_prefix + ".jpg";
            return newName;
        }



        internal static string CustomFilename(Microsoft.Xna.Framework.Media.Picture pic)
        {
            string newName = "";
            string Unique_prefix = pic.Date.Ticks.ToString();
            try
            {
                string modifiedName = pic.Name.Substring(0, 6).Replace(' ', '_').Replace(':', '_').Replace('\\', '_');


                if (modifiedName.Length > 0)
                {
                    newName = Unique_prefix + "_" + modifiedName;
                    return newName;
                }
            }
            catch { }

            newName = Unique_prefix;
            return newName;
        }


        public static string ExtractImageName(string source)
        {
            try
            {
                return source.Split('/')[source.Length - 1];
            }
            catch
            {
                return "";
            }
        }


        public static string ExtractImageName(Uri source)
        {
            try
            {
                return source.AbsolutePath.Split('/')[source.AbsolutePath.Length - 1];
            }
            catch
            {
                return "";
            }
        }



        public static string ExtractImageId(string source)
        {
            try
            {
                string[] segs = source.Split('/');
                return segs[segs.Length - 1].TrimEnd(".jpg".ToCharArray());
            }
            catch
            {
                return "";
            }
        }


        public static string ExtractImageId(Uri source)
        {
            try
            {
                return source.AbsolutePath.Split('/')[source.AbsolutePath.Length - 1].TrimEnd(".jpg".ToCharArray());
            }
            catch
            {
                return "";
            }
        }



        // Method to retrieve all directories, recursively, within a store. 
        public static List<String> GetAllDirectories(string pattern, IsolatedStorageFile storeFile)
        {
            // Get the root of the search string. 
            string root = System.IO.Path.GetDirectoryName(pattern);

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


        public static void DeleteFiles(LockeScreen.Models.AlbumPreview.ALBUM_TYPE type)
        {
            string _SourceName = "";

            switch (type)
            {
                case Models.AlbumPreview.ALBUM_TYPE.FACEBOOK:
                    _SourceName = "Facebook";
                    break;
                case Models.AlbumPreview.ALBUM_TYPE.FLICKR:
                    _SourceName = "Flickr";
                    break;
                case Models.AlbumPreview.ALBUM_TYPE.PHONE:
                    _SourceName = "Phone";
                    break;
                case Models.AlbumPreview.ALBUM_TYPE.INSTAGRAM:
                    _SourceName = "Instagram";
                    break;
            }

            string filename;

            string dir = "SavedImages/" + _SourceName;
            List<string> ImagesToDelete = new List<string>();

                filename = dir + "*.jpg";

                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    ImagesToDelete = new List<string>(appStore.GetFileNames(dir+"/*"));


                    foreach (string s in ImagesToDelete)
                    {
                        filename = dir+ "/" + s;
                        DeleteFile(filename);
                    }
                }
        }


    }
}
