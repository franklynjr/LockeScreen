using LockeScreen.Common;
using LockeScreen.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LockeScreen.ViewModels
{
    class QuickSetViewModel:ModelBase
    {

        private ObservableCollection<ImageViewModel> _SelectedImages;

        public QuickSetViewModel()
        {
            _SelectedImages = new ObservableCollection<ImageViewModel>();
            Thread SelectedImagesThread = new Thread(new ThreadStart(GetSelectedImagesAync));
            SelectedImagesThread.Start();

        }

        public ObservableCollection<ImageViewModel> SelectedImages
        {
            get { return _SelectedImages; }
            set
            {

                if (_SelectedImages != value)
                {
                    _SelectedImages = value;
                    NotifyPropertyChanged("SelectedImages");
                }
            }
        }




        public void ReloadSelectedImagesAsync()
        {
            SelectedImagesLoaded = false;
            Thread SelectedImages = new Thread(new ThreadStart(GetSelectedImagesAync));
            SelectedImages.Start();
        }




        private void GetSelectedImagesAync()
        {
            Dispatcher.BeginInvoke(() =>
            {
                using (IsolatedStorageFile appStore = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    ImageViewModel image;
                    ObservableCollection<ImageViewModel> images = new ObservableCollection<ImageViewModel>();

                    List<string> Files = FileOperation.GetAllFiles("*jpg", appStore);
                    ThumbnailGenerator thmb = new ThumbnailGenerator();

                    if (Files.Count == 0)
                        SelectedImagesLoaded = true;


                    foreach (string file in Files)
                    {
                        thmb.LoadThumbnail(file, (bmpImage) =>
                        {
                            image = new ImageViewModel();
                            image.Image = bmpImage;
                            image.Url = file;
                            images.Add(image);

                            if (images.Count == Files.Count)
                            {
                                //
                                SelectedImages = images;
                                SelectedImagesLoaded = true;
                            }
                        });
                    }
                }
            });

        }


    }
}
