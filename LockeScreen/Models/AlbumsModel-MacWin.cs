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

namespace LockeScreen.Models
{
    public static class AlbumsModel
    {
        public static ObservableCollection<AlbumViewModel> Albums = new ObservableCollection<AlbumViewModel>();


        // Constructor

        public static bool AlbumsLoaded
        {
            get;
            private set;
        }


        public static void LoadImages()
        {
            
            Dispatcher().BeginInvoke(() =>
            {

                MediaLibrary library = new MediaLibrary();
                var images = library.Pictures;

                var albumCollection = library.RootPictureAlbum.Albums;


                try
                {

                    // get Albums

                    for (int i = 0; i < albumCollection.Count; i++)
                    {
                        var pa = albumCollection[i];

                        AlbumViewModel album_vm = new AlbumViewModel();
                        BitmapImage thumbnail = new BitmapImage();

                        if (pa.Pictures.Count > 1)
                        {
                            thumbnail.SetSource(pa.Pictures[0].GetThumbnail());
                            album_vm.Thumbnail = thumbnail;
                        }

                        foreach (var pic in pa.Pictures)
                        {
                            ImageViewModel image = new ImageViewModel();
                            BitmapImage bmp = new BitmapImage();
                            bmp.SetSource(pic.GetThumbnail());

                            image.ImageItem = bmp;

                            image.ImageName = pic.Name;
                            album_vm.ImageThumbnails.Add(image);
                        }

                        Albums.Add(album_vm);




                        album_vm.Name = pa.Name;
                    }
                }
                catch { }

//                    foreach (PictureAlbum pa in albumCollection)
//                    {
//                        MessageBox.Show(pa.Name);
//try
//                        {
//                        AlbumViewModel album_vm = new AlbumViewModel();
//                        BitmapImage thumbnail = new BitmapImage();


                        
    

//                            album_vm.Name = pa.Name;

//                            // Set the first image as the thumbnail image
//                            if (pa.Pictures.Count > 1)
//                            {
//                                thumbnail.SetSource(pa.Pictures[0].GetThumbnail());
//                                album_vm.Thumbnail = thumbnail;
//                            }

//                            foreach (var pic in pa.Pictures)
//                            {
//                                ImageViewModel image = new ImageViewModel();
//                                BitmapImage bmp = new BitmapImage();
//                                bmp.SetSource(pic.GetThumbnail());

//                                image.ImageItem = bmp;
//                                image.ImageName = pic.Name;
//                                album_vm.ImageThumbnails.Add(image);
//                            }

//                            Albums.Add(album_vm);

//                        }
//                        catch (Exception ex1)
//                        {
//                            MessageBox.Show("2... " + ex1.Message + ex1.StackTrace);
//                        }


//                    }

//                }
//                catch (Exception ex)
//                {
//                    MessageBox.Show("3... 1"  + ex.Message + ex.StackTrace);
//                }

                

            });
        }

        public delegate void Images();

        private static void GetImages()
        {

            
        }

        public static Dispatcher Dispatcher()
        {
            return Deployment.Current.Dispatcher;
        }
    }
}
