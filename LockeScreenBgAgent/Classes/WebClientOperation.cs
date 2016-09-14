using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LockeScreenBgAgent.Classes
{
    public class WebClientOperation
    {
        BitmapImage bmp = new BitmapImage();

        public void DownloadImage(string imageUrl)
        {
            WebClient sc = new WebClient();
            sc.OpenReadCompleted += sc_OpenReadCompleted;
            
            sc.OpenReadAsync(new Uri(imageUrl));

        }

        void sc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            bmp.SetSource(e.Result);
            WriteableBitmap wbmp = new WriteableBitmap(bmp);
            //throw new NotImplementedException();
        }
    }
}
