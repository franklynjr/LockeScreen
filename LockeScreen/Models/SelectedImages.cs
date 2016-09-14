using LockeScreen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.IsolatedStorage;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using Microsoft.Xna.Framework.Media;
using System.Windows.Controls;
using System.IO;
using System.Diagnostics;
using LockeScreen.Common;
using System.Net;

namespace LockeScreen.Models
{
    public class SelectedImages : ModelBase
    {
        private List<string> _SelectedImages;
        private List<int> _SelectedIndices;

        public List<string> RemovedImages;

        public List<int> RemovedIndices;

        private string _AlbumName;
        private string _SourceName;
        private AlbumPreview.ALBUM_TYPE _AlbumType;




        public SelectedImages()
        {
            _SelectedImages = new List<string>();
            _SelectedIndices = new List<int>();

            RemovedImages = new List<string>();
            RemovedIndices = new List<int>();

            _SourceName = String.Empty;
            _AlbumName = String.Empty;

        }


        public SelectedImages(AlbumPreview.ALBUM_TYPE AlbumType)
        {
            this.AlbumType = AlbumType;

            switch (AlbumType)
            {
                case LockeScreen.Models.AlbumPreview.ALBUM_TYPE.PHONE:
                    
                    SourceName = "Phone";
                    break;
                case AlbumPreview.ALBUM_TYPE.FLICKR:

                    SourceName = "Flickr";

                    break;
                case AlbumPreview.ALBUM_TYPE.FACEBOOK:

                       SourceName = "Facebook";
                  
                    //Save FB Images
                    //Save FlickrImages()

                    break;

                case AlbumPreview.ALBUM_TYPE.INSTAGRAM:
                    break;

                default:
                    SourceName = "UNKNOWN";
                    break;
            }

            _SelectedImages = new List<string>();
            RemovedImages = new List<string>();
            
            _AlbumName = String.Empty;

        }

        public AlbumPreview.ALBUM_TYPE AlbumType
        {
            get { return _AlbumType; }
            set
            {
                if (_AlbumType != value)
                    _AlbumType = value;
            }
        }


        public string SourceName
        {
            get { return _SourceName; }
            private set
            {
                if (_SourceName != value)
                    _SourceName = value;
            }
        }


        public string AlbumName
        {
            get { return _AlbumName; }
            set
            {
                if (_AlbumName != value)
                    _AlbumName = value;
            }
        }


        public List<string> SelectedImageNames
        {
            get { return _SelectedImages; }
            set
            {
                if (_SelectedImages != value)
                    _SelectedImages = value;
            }
        }


        public List<int> SelectedIndices
        {
            get { return _SelectedIndices; }
            set
            {
                if (_SelectedIndices != value)
                    _SelectedIndices = value;
            }
        }


        public void Save()
        {
            IsolatedStorageSettings.ApplicationSettings[_AlbumName] = this;
        }

        private void SaveFlickrImages(string SourceName)
        {
            //throw new NotImplementedException();
        }

        public void Load()
        {
            try
            {
                SelectedImages selected_images = new SelectedImages(AlbumType); ;
                if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<SelectedImages>(_AlbumName, out selected_images))
                {
                    this.SelectedImageNames = selected_images.SelectedImageNames;
                    this.SelectedIndices = selected_images.SelectedIndices;

                    this.AlbumName = selected_images.AlbumName;
                }
            }
            catch
            {
                Debug.WriteLine("Error Loading selected images.");
            }
        }


        

        public void SavePhoneImages()
        {
            MediaLibrary library = new MediaLibrary();
            var images = library.Pictures;
            string dir = "SavedImages/" + _SourceName;

            var albumCollection = library.RootPictureAlbum.Albums;

            using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
            {

                for (int i = 0; i < albumCollection.Count; i++)
                {

                    var pa = albumCollection[i];
                    AlbumPreview album_vm = new AlbumPreview();
                    BitmapImage image = new BitmapImage();

                    if (pa.Name == _AlbumName)
                    {

                       
                        Common.FileOperation.CreateDirectory(dir);

                        for (int x = 0; x < pa.Pictures.Count; x++)
                        {
                            try
                            {
                                var pic = pa.Pictures[x];

                                string concat_name = FileOperation.CustomFilename(pic);
                                string imageName = dir + "/" + concat_name + ".jpg";

                                if (_SelectedImages.Contains(concat_name) && !appStore.FileExists(imageName))
                                {
                                    FileOperation.SaveImage(imageName, pic.GetImage());
                                }
                            }
                            catch { }


                        }
                    }

                }


                string filename;
                List<string> ImagesToDelete = new List<string>();

                foreach (string file in RemovedImages)
                {
                    filename = dir + "/" + file;
                    FileOperation.DeleteFile(filename);
                }

                foreach (string image in ImagesToDelete)
                {
                    RemovedImages.Remove(image);
                }



            }
        }



        public void SaveFacebookImage(string imageId)
        {
            //
            string dir = "SavedImages/" + _SourceName;

            Common.FileOperation.CreateDirectory(dir);

            string image_name = dir + "/" + imageId + ".jpg";
            FacebookHelper fbHelper = new FacebookHelper();

            fbHelper.GetFacebookImage(imageId, (string)Shared.ApplicationData["AccessToken"], (image) =>
            {
                FileOperation.SaveImage(image_name, image);


            });

            //Delete Unchecked Images
            string filename;
            List<string> ImagesToDelete = new List<string>();

            foreach (string file in RemovedImages)
            {
                filename = dir + "/" + file + ".jpg";
                FileOperation.DeleteFile(filename);
            }

            foreach (string image in ImagesToDelete)
            {
                RemovedImages.Remove(image);
            }
        }


        public void SaveFlickrImage(string imageId)
        {
            //
            string dir = "SavedImages/" + _SourceName;

            Common.FileOperation.CreateDirectory(dir);

            string image_name = dir + "/" + imageId + ".jpg";
            FlickrHelper fkrHelper = new FlickrHelper();

            fkrHelper.GetFlickrImage(imageId, (image) =>
            {
                FileOperation.SaveImage(image_name, image);


            });

            //Delete Unchecked Images
            string filename;
            List<string> ImagesToDelete = new List<string>();

            foreach (string file in RemovedImages)
            {
                filename = dir + "/" + file + ".jpg";
                FileOperation.DeleteFile(filename);
            }

            foreach (string image in ImagesToDelete)
            {
                RemovedImages.Remove(image);
            }
        }





        public void DeleteFiles()
        {


            string filename;

            string dir = "SavedImages/" + _SourceName;
            List<string> ImagesToDelete = new List<string>();

            foreach (string file in RemovedImages)
            {
                filename = dir + "/" + file + ".jpg";
                FileOperation.DeleteFile(filename);
            }

            foreach (string image in ImagesToDelete)
            {
                RemovedImages.Remove(image);
            }
        }


        public void DeleteFile(string file)
        {


            string filename;

            string dir = "SavedImages/" + _SourceName;
            List<string> ImagesToDelete = new List<string>();


            filename = dir + "/" + file + ".jpg";
            FileOperation.DeleteFile(filename);


            foreach (string image in ImagesToDelete)
            {
                RemovedImages.Remove(image);
            }
        }

    }
}
