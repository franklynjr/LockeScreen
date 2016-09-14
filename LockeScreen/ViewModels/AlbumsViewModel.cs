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
using System.Collections.ObjectModel;
using LockeScreen.ViewModels;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using LockeScreen.Models;
using System.Collections.Generic;
using LockeScreen.Common;
using Facebook;
using System.Threading;

namespace LockeScreen.ViewModels
{
    public static class AlbumsViewModel
    {
        public static ObservableCollection<AlbumPreview> PhoneAlbums = new ObservableCollection<AlbumPreview>();

        public static ObservableCollection<AlbumPreview> FacebookAlbums = new ObservableCollection<AlbumPreview>();

        public static ObservableCollection<AlbumPreview> FlickrAlbums = new ObservableCollection<AlbumPreview>();

        static List<string> AlbumIgnoreList = new List<string>(new string[] {"Favorite Pictures" });



        // Constructor
        public static bool PhoneAlbumsLoaded
        {
            get;
            private set;
        }

        public static bool FaceAlbumsLoaded
        {
            get;
            private set;
        }


        public static void LoadPhoneImages()
        {

            Dispatcher().BeginInvoke(() =>
            {
                //LoadPhoneImages()
                MediaLibrary library = new MediaLibrary();
                //var images = library.Pictures;

                var albumCollection = library.RootPictureAlbum.Albums;
                BitmapImage bmp;
                ImageViewModel image_vm;
                BitmapImage thumbnail;

                    AlbumPreview album_vm;
                try
                {

                    // get Albums
                    for (int i = 0; i < albumCollection.Count; i++)
                    {
                        album_vm = new AlbumPreview();
                        var pa = albumCollection[i];
                        //MessageBox.Show(pa.Name);
                        if (!AlbumIgnoreList.Contains(pa.Name))
                        {
                            album_vm = new AlbumPreview();
                            thumbnail = new BitmapImage();


                            try
                            {


                                album_vm.Name = pa.Name;

                                // Set the first image as the thumbnail image
                                if (pa.Pictures.Count > 0)
                                {
                                    thumbnail.SetSource(pa.Pictures[0].GetThumbnail());
                                    album_vm.Thumbnail = thumbnail;
                                }

                                //Exception thre
                                for (int x = 0; x < pa.Pictures.Count; x++)
                                {
                                    //try
                                    //{
                                        var pic = pa.Pictures[x];
                                        
                                            image_vm = new ImageViewModel();
                                            bmp = new BitmapImage();

                                            bmp.SetSource(pic.GetThumbnail());
                                            image_vm.ImageID = FileOperation.CustomFilename(pic);

                                            image_vm.Image = bmp;
                                            album_vm.ImageThumbnails.Add(image_vm);
                                        
                                    //}
                                    //catch (Exception)
                                    //{
                                    //    string loopNumber = i.ToString() + x.ToString();
                                    //    MessageBox.Show("err code avm" + loopNumber);
                                    //}

                                }
                                PhoneAlbums.Add(album_vm);

                            }
                            catch (Exception ex1)
                            {
                                MessageBox.Show(ex1.Message + ex1.StackTrace);
                            }


                        }

                    }
                        PhoneAlbumsLoaded = true;

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ex.StackTrace);
                }



            });
        }




        

        //public delegate void Images();

        private static void GetImages()
        {

            
        }

        public static Dispatcher Dispatcher()
        {
            return Deployment.Current.Dispatcher;
        }

    }
}
