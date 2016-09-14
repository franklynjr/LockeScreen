using Facebook;
using LockeScreen.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LockeScreen.Models
{
    public class FacebookAlbum
    {
        string ablum_id;
        string album_name;
        string album_thumbnail;
        ObservableCollection<FacebookImage> _ImageList;

        public FacebookAlbum()
        {
            _ImageList = new ObservableCollection<FacebookImage>();
        }

        public string ID
        {
            get { return ablum_id; }
            set
            {
                if (ablum_id != value)
                    ablum_id = value;
            }
        }

        public string AlbumName
        {
            get { return album_name; }
            set
            {
                if (album_name != value)
                    album_name = value;
            }
        }


        public string AlbumThumbnail
        {
            get { return album_thumbnail; }
            set
            {
                if (album_thumbnail != value)
                    album_thumbnail = value;
            }
        }


        public ObservableCollection<FacebookImage> Images
        {
            get { return _ImageList; }
            set
            {
                if (_ImageList != value)
                    _ImageList = value;
            }
        }


        public void GetAlbumPhotos(string accessToken)
        {

            var fb = new FacebookClient(accessToken);

            Dispatcher().BeginInvoke(()=>{

                ImageViewModel Image;
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher().BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }

                var result = (IDictionary<string, object>)e.GetResultData();
                
                //
                var data = (IList<object>)result["data"];
                int imgCounter = 0;
                foreach (var img in data)
                {
                    //
                    JsonObject image_info = (JsonObject)img;
                    //

                    string id;
                    string thumbnail;
                    string scaled_image;

                    id = Convert.ToString(image_info["id"]);

                    var imgLst = (IList<object>)image_info["images"]; // List of Image sizes
                   
                        //scaled_image = Convert.ToString(((JsonObject)imgLst[1])["source"]);
                        thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);
                        string image_id = id;
                        LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(thumbnail, (bitmap) =>
                        {
                            Image = new ImageViewModel();
                            Image.ImageID = id;
                            Image.Image = bitmap;

                            if (imgCounter == data.Count)
                            {
                                //
                                
                            }

                        });
                    
                }
            };

            fb.GetTaskAsync("https://graph.facebook.com/"+ablum_id + "/photos?access_token=" + accessToken);
            });

        }



        protected new Dispatcher Dispatcher()
        {

            return Deployment.Current.Dispatcher;
        }
       


    }
}
