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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;
namespace LockeScreen.ViewModels
{
    public class AlbumListViewModel : INotifyPropertyChanged
    {


        public event PropertyChangedEventHandler PropertyChanged;


        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        // Constructor
        public AlbumListViewModel()
        {
            this.Albums = new ObservableCollection<AlbumViewModel>();
        }



        //private ObservableCollection<AlbumViewModel> _Albums;


        public ObservableCollection<AlbumViewModel> Albums
        {
            get;
            private set;
        }


        public bool AlbumsLoaded
        {
            get;
            private set;
        }


        public void LoadImages()
        {
            MediaLibrary library = new MediaLibrary();
            var images = library.Pictures;


            // get Albums
            foreach (PictureAlbum pa in library.RootPictureAlbum.Albums)
            {
                AlbumViewModel album_vm = new AlbumViewModel();
                BitmapImage thumbnail = new BitmapImage(); 
                
                
                album_vm.Name = pa.Name;

                // Set the first image as the thumbnail image
                if (pa.Pictures.Count > 1)
                {
                    thumbnail.SetSource(pa.Pictures[0].GetThumbnail());
                    album_vm.Thumbnail = thumbnail;
                }

                foreach (Picture pic in pa.Pictures)
                {
                    ImageViewModel image = new ImageViewModel();
                    image.ImageItem.SetSource(pic.GetImage());


                    album_vm.Images.Add(image);
                }
                
               Albums.Add(album_vm);

            }

            AlbumsLoaded = true;
        }

    }
}
