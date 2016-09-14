using Facebook;
using LockeScreen.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LockeScreen.Models
{
    public class FacebookAlbumHelper : ModelBase
    {
        private ObservableCollection<AlbumModel> _Albums;
        private string _AccessToken;

        public delegate void GetAlbumsEventHandler(ObservableCollection<AlbumModel> AlbumResult);
        public delegate void GetAlbumImagesEventHandler( ObservableCollection<ImageViewModel> AlbumResult);
        public delegate void GetImageEventHandler(ImageViewModel ImageResult);
        public delegate void GetBitmapImageCompletedHandler( BitmapImage Image);

        public event GetAlbumImagesEventHandler GetAlbumImagesCompleted;
        public event GetAlbumsEventHandler GetAlbumsCompleted;
        public event GetBitmapImageCompletedHandler GetBitmapImageCompleted;



        //Callbacks

        public delegate void ResultCallback();
        public delegate void TokenCallback(string Token);
        public delegate void AlbumCollectionResultCallback(ObservableCollection<AlbumModel> albums);
        public delegate void ImageCollectionResultCallback(ObservableCollection<ImageViewModel> Images);

        public delegate void AlbumCallbackResult(BitmapImage Image, string Name);
        public delegate void ImageCallbackResult(BitmapImage Image);


        public FacebookAlbumHelper()
        {

            _Albums = new ObservableCollection<AlbumModel>();

        }

        public FacebookAlbumHelper(string AccessToken)
        {
            _AccessToken = AccessToken;
            _Albums = new ObservableCollection<AlbumModel>();

        }

        public bool IsLoggedIn
        {
            get
            {
                if (_AccessToken != "")
                    return true;

                return false;
            }
        }


        //public void GetFacebookAlbums(string AccessToken)
        //{


        //    Dispatcher.BeginInvoke(() =>
        //    {

        //        var fb = new FacebookClient(AccessToken);

        //        //ObservableCollection<FacebookAlbum> Albums = new ObservableCollection<FacebookAlbum>();

        //        fb.GetCompleted += (o, e) =>
        //        {
        //            if (e.Error != null)
        //            {
        //                Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
        //                return;
        //            }
        //            //Dispatcher.BeginInvoke(() =>
        //            //{

        //            var result = (IDictionary<string, object>)e.GetResultData();

        //            var data = (IList<object>)result["data"];

        //            AlbumModel alb = null;
        //            JsonObject album = new JsonObject();


        //            foreach (var albm in data)
        //            {
        //                //Dispatcher().BeginInvoke(() => {



        //                album = (JsonObject)albm;


        //                string image_name = (string)album["name"];
        //                string album_id = (string)album["id"];
        //                string image_id = (string)album["cover_photo"];
                        

        //                this.GetBitmapImageCompleted += (sender, image_result) =>
        //                {
                                

        //                    this.GetAlbumImagesCompleted += (albSender, albResult) =>
        //                    {
        //                        alb = new AlbumModel();
        //                        alb.Name = image_name;
        //                        alb.AlbumID = album_id;
        //                        alb.Thumbnail = image_result;
        //                        alb.Type = AlbumModel.ALBUM_TYPE.FACEBOOK;
        //                        alb.ImageThumbnails = albResult;
                                
        //                    _Albums.Add(alb);

        //                        if (_Albums.Count == album.Count)
        //                        {

        //                            GetAlbumsCompleted(this, _Albums);
                                    
        //                        }
                                
        //                    };


        //                    GetAlbumPhotos(album_id, _AccessToken);

                            
        //                };


        //                GetBitmapImage(image_id, _AccessToken);
        //            }
        //        };


        //        fb.GetTaskAsync("me/albums");
        //    });
        //}




        public void GetFacebookAlbums(string AccessToken, AlbumCollectionResultCallback AlbumsResult)
        {



            var fb = new FacebookClient(AccessToken);

            //ObservableCollection<FacebookAlbum> Albums = new ObservableCollection<FacebookAlbum>();

            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {
                    Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                    return;
                }
                //Dispatcher.BeginInvoke(() =>
                //{
                var result = (IDictionary<string, object>)e.GetResultData();
                //var id = (string)result["data"];
                var data = (IList<object>)result["data"];

                AlbumModel alb = null;
                JsonObject album = new JsonObject();


                foreach (var albm in data)
                {
                    


                        album = (JsonObject)albm;


                        string image_name = (string)album["name"];
                        string album_id = (string)album["id"];
                        string image_id = (string)album["cover_photo"];


                        GetFacebookImage(image_id, AccessToken, (img) =>
                        {

                            GetAlbumPhotos(album_id, AccessToken, (imgCollection) =>
                            {


                                alb = new AlbumModel();
                                alb.Name = image_name;
                                alb.AlbumID = album_id;
                                alb.Thumbnail = img;
                                alb.Type = AlbumModel.ALBUM_TYPE.FACEBOOK;
                                alb.ImageThumbnails = imgCollection;

                                _Albums.Add(alb);
                                if (_Albums.Count == album.Count)
                                {
                                    //facebook_albums_loaded = true;
                                    AlbumsResult(_Albums);
                                }

                            });
                        });



                }
                //});
            };

            fb.GetTaskAsync("me/albums");


        }



        public void GetFacebookImages(string Token, ImageCollectionResultCallback result)
        {

        }

        public delegate void FacebookImageResult();

        public void GetFacebookImage(string ImageID, string AccessToken, ImageCallbackResult result)
        {

            FacebookClient fbClient = new FacebookClient(AccessToken);
            fbClient.GetCompleted += (sender, ev) =>
            {
               
           var result_1 = (IDictionary<string, object>)ev.GetResultData();
           //JsonObject JsonResult = (JsonObject)result_1["data"];


           var imgLst = (IList<object>)result_1["images"];
           string Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

           // Download album thumnail async

           ImageDownloader imgDownloader = new ImageDownloader();
           imgDownloader.DownloadImage(Thumbnail, (bitmap) =>
           {
               
                       result(bitmap);
                   
           });
            };
            fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken);

        }



        //private void GetBitmapImage(string ImageID, string AccessToken)
        //{

        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        FacebookClient fbClient = new FacebookClient(AccessToken);
        //        fbClient.GetCompleted += (sender, ev) =>
        //        {

        //            var result_1 = (IDictionary<string, object>)ev.GetResultData();
        //            //JsonObject JsonResult = (JsonObject)result_1["data"];


        //            var imgLst = (IList<object>)result_1["images"];
        //            string Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

        //            // Download album thumnail async
        //            ImageDownloader ImgDownloader = new ImageDownloader();

        //            ImgDownloader.FileDownloaded += (o, img) =>
        //            {
        //                GetBitmapImageCompleted(this, img);
        //            };

        //            ImgDownloader.DownloadImage(Thumbnail);
                   

        //        };
        //        fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken);
        //    });
        //}




        public void GetAlbumPhotos(string album_id, string accessToken, ImageCollectionResultCallback result)
        {

            var fb = new FacebookClient(accessToken);

            
                ObservableCollection<ImageViewModel> Images = new ObservableCollection<ImageViewModel>();
                ImageViewModel Image;
                fb.GetCompleted += (o, e) =>
                {
                    if (e.Error != null)
                    {
                        Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
                        return;
                    }

                    var result_1 = (IDictionary<string, object>)e.GetResultData();

                    //
                    var data = (IList<object>)result_1["data"];
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
                        ImageDownloader imgDownloader = new ImageDownloader();
                        imgDownloader.DownloadImage(thumbnail, (bitmap) =>
                        {
                            Image = new ImageViewModel();
                            Image.ImageID = id;
                            Image.Image = bitmap;

                            Images.Add(Image);

                            if (imgCounter == data.Count - 1)
                            {
                                //
                                result(Images);
                                //return;
                            }
                            imgCounter++;
                        });

                    }
                };

                fb.GetTaskAsync("https://graph.facebook.com/" + album_id + "/photos?access_token=" + accessToken);
           

        }
 

        //void GetAlbumPhotos(string album_id, string AccessToken)
        //{

        //    var fb = new FacebookClient(AccessToken);
        //    var Images = new ObservableCollection<ImageViewModel>();

        //    Dispatcher.BeginInvoke(() =>
        //    {
        //        Images = new ObservableCollection<ImageViewModel>();
        //        ImageViewModel Image;
        //        fb.GetCompleted += (o, e) =>
        //        {
        //            if (e.Error != null)
        //            {
        //                Dispatcher.BeginInvoke(() => MessageBox.Show(e.Error.Message));
        //                return;
        //            }

        //            var result_1 = (IDictionary<string, object>)e.GetResultData();

        //            //
        //            var data = (IList<object>)result_1["data"];
        //            int imgCounter = 0;
        //            foreach (var img in data)
        //            {
        //                //
        //                JsonObject image_info = (JsonObject)img;
        //                //

        //                string id;
        //                string thumbnail;

        //                id = Convert.ToString(image_info["id"]);

        //                var imgLst = (IList<object>)image_info["images"]; // List of Image sizes

        //                //scaled_image = Convert.ToString(((JsonObject)imgLst[1])["source"]);
        //                thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

        //                string image_id = id;



        //                ImageDownloader ImgDownloader = new ImageDownloader();

        //                ImgDownloader.FileDownloaded += (imgSender, bitmap) =>
        //                {
        //                    Image = new ImageViewModel();
        //                    Image.ImageID = id;
        //                    Image.Image = bitmap;

        //                    Images.Add(Image);

        //                    if (imgCounter == data.Count - 1)
        //                    {
        //                        //
        //                        GetAlbumImagesCompleted(this, Images);
        //                    }
        //                    imgCounter++;
        //                };

        //                ImgDownloader.DownloadImage(thumbnail);

        //            }
        //        };

        //        fb.GetTaskAsync("https://graph.facebook.com/" + album_id + "/photos?access_token=" + AccessToken);
        //    });

        //}


        internal void GetAlbums(string result)
        {
            
           // throw new NotImplementedException();
                GetFacebookAlbums(result, (o) =>
                {
                    GetAlbumsCompleted( o);
                });
           
        }
    }

}