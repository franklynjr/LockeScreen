using LockeScreen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LockeScreen.ViewModels
{
    public class FlickerAlbums
    {
        public ObservableCollection<AlbumPreview> MyFlickrAlbums
        {
            set;
            get;
        }

        public ObservableCollection<AlbumPreview> PhotosetsFlickrAlbums
        {
            set;
            get;
        }
        public ObservableCollection<AlbumPreview> GalleryFlickrAlbums
        {
            set;
            get;
        }


    }
}
