using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Media.Imaging;

namespace LockeScreen.Models
{
    class ImageDownloader:ModelBase
    {


        
        public event FacebookHelper.GetBitmapImageCompletedHandler FileDownloaded;
        public event FacebookHelper.DownloadFailHandler DownloadFailed;
        public void DownloadImage(string imageUrl)
        {
            Dispatcher.BeginInvoke(() =>
            {
                try
                {
                    WebClient sc = new WebClient();
                    sc.OpenReadCompleted += (object sender, OpenReadCompletedEventArgs e) =>
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(e.Result);
                        FileDownloaded(bmp);
                    };
                    //

                    sc.OpenReadAsync(new Uri(imageUrl));
                }
                catch
                {

                    if (DownloadFailed != null)
                        DownloadFailed(this, new EventArgs());
                }
            });
        }

        public void DownloadImage(string imageUrl, ResultCallbacks.ImageCallbackResult imageResult)
        {
            Dispatcher.BeginInvoke(() =>
            {

                WebClient sc = new WebClient();
                sc.OpenReadCompleted += (object sender, OpenReadCompletedEventArgs e) =>
                {
                    try
                    {
                        BitmapImage bmp = new BitmapImage();
                        bmp.SetSource(e.Result);
                        imageResult(bmp);
                    }
                    catch
                    {
                        if (DownloadFailed != null)
                            DownloadFailed(this, new EventArgs());
                    }
                };
                //

                sc.OpenReadAsync(new Uri(imageUrl));

            });
        }

    }
}
