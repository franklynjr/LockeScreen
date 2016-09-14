using LockeScreen.Models;
using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LockeScreen.Common
{
    class ThumbnailGenerator:ModelBase
    {
        public ThumbnailGenerator()
        {
        }

        public void LoadThumbnail(string source, ResultCallbacks.ImageCallbackResult thumbnailImage)
        {
            //
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(source, System.IO.FileMode.Open, IsolatedStorageFile.GetUserStoreForApplication()))
                    {

                        bitmap.CreateOptions = BitmapCreateOptions.BackgroundCreation;
                        bitmap.DecodePixelHeight = 150;
                        bitmap.DecodePixelWidth = 150;

                        //WriteableBitmap wbmp = new WriteableBitmap(bitmap);
                        //wbmp.LoadJpeg(fs);
                        //wbmp.

                        bitmap.SetSource(fs);
                        thumbnailImage(bitmap);

                        //bitmap.UriSource =imgUri;
                    }
                }
                catch { }

            });
        }


        public void LoadThumbnail(Uri source, ResultCallbacks.ImageCallbackResult thumbnailImage)
        {
            //
            BitmapImage bitmap = new BitmapImage();
            bitmap.CreateOptions = BitmapCreateOptions.DelayCreation;

            bitmap.DecodePixelHeight = 150;
            bitmap.DecodePixelWidth = 150;

            bitmap.ImageOpened += (o, e) =>
            {

                thumbnailImage(bitmap);
            };
            bitmap.UriSource = source;
        }



    }
}
