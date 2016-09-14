using Facebook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LockeScreen.Models
{
    public class FacebookImage
    {
        string id;
        string thumbnail;
        string image;

        BitmapImage _image;
        BitmapImage _thumbnail;

        public string ID
        {
            get { return id; }
            set
            {
                if (id != value)
                    id = value;
            }
        }

        public string Thumbnail
        {
            get { return thumbnail; }
            set
            {
                if (thumbnail != value)
                    thumbnail = value;
            }
        }

        public string Image
        {
            get { return image; }
            set
            {
                if (image != value)
                    image = value;
            }
        }


        public BitmapImage ThumbnailBitmap
        {
            get { return _thumbnail; }
            set
            {
                if (_thumbnail != value)
                    _thumbnail = value;
            }
        }

        public BitmapImage ImageBitmap
        {
            get { return _image; }
            set
            {
                if (_image != value)
                    _image = value;
            }
        }


        public void GetFacebookImage(string ImageId, string AccessToken)
        {
            FacebookClient fbClient = new FacebookClient(AccessToken);

            fbClient.GetCompleted += fbClient_GetCompleted;

            id = ImageId;
            fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageId + "?access_token=" + AccessToken);
        }


        private void fbClient_GetCompleted(object sender, FacebookApiEventArgs e)
        {

            var result = (IDictionary<string, object>)e.GetResultData();
            JsonObject JsonResult = (JsonObject)result["data"];


            var imgLst = (IList<object>)JsonResult["images"]; // List of Image sizes


            Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);
            image = Convert.ToString(((JsonObject)imgLst[1])["source"]);
            LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(Thumbnail, (img) => {ThumbnailBitmap=img; });
            LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(image, (img) => { ImageBitmap = img; });
         }



        public void GetFacebookImageThumbnail(string ImageId, string AccessToken)
        {
            FacebookClient fbClient = new FacebookClient(AccessToken);

            fbClient.GetCompleted += (sender, e) =>
        {

            var result = (IDictionary<string, object>)e.GetResultData();
            JsonObject JsonResult = (JsonObject)result["data"];


            var imgLst = (IList<object>)JsonResult["images"]; // List of Image sizes


            Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);
            //image = Convert.ToString(((JsonObject)imgLst[1])["source"]);
            LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(Thumbnail, (img) => { ThumbnailBitmap = img; });
            //ImageBitmap = LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(image);
        };

            id = ImageId;
            fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageId + "?access_token=" + AccessToken);
        }
    }
}
