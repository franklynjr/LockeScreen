using Facebook;
using LockeScreen.ViewModels;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LockeScreen.Models
{
    public class FacebookAlbumCollection : ModelBase
    {

        static string AppId = "139219706255772";
        static string extendedPermissions = "user_about_me,read_stream,publish_stream,user_photos";
        static FacebookClient _fb = new FacebookClient();
        static string AccessToken = "";


        public delegate void ResultCallback();
        public delegate void TokenCallback(string Token);
        public delegate void AlbumCollectionResultCallback(ObservableCollection<AlbumModel> albums);
        public delegate void ImageCollectionResultCallback(ObservableCollection<ImageViewModel> Images);

        public delegate void AlbumCallbackResult(BitmapImage Image, string Name);
        public delegate void ImageCallbackResult(BitmapImage Image);

        private bool facebook_albums_loaded = false;


        public FacebookAlbumCollection(string Token, ResultCallback callback)
        {
            alb_vm = new ObservableCollection<AlbumModel>();
          
            GetFacebookAlbums(Token, (o) => {
                callback();
            });
           
            //GetAccessToken(GetFacebookAlbums);

        }

        public FacebookAlbumCollection()
        {
            alb_vm = new ObservableCollection<AlbumModel>();
            GetAccessToken((t) =>
            {
                GetFacebookAlbums(t, (o) => {
               
                });
            });
        }

        public void GetAccessToken(TokenCallback callback)
        {
            Dispatcher.BeginInvoke(() =>
            {
                WebBrowser web_browser1 = new WebBrowser();
                web_browser1.IsScriptEnabled = true;


                //Load Operations
                var loginUrl = GetFacebookLoginUrl(AppId, extendedPermissions);

                
                web_browser1.Navigated += (nav_o, nav_e) =>
                                            {
                                                FacebookOAuthResult oauthResult;
                                                if (!_fb.TryParseOAuthCallbackUrl(nav_e.Uri, out oauthResult))
                                                {

                                                    return;
                                                }

                                                if (oauthResult.IsSuccess)
                                                {

                                                    AccessToken = oauthResult.AccessToken;
                                                    callback(AccessToken);
                                                    //GetAlbums(accessToken);
                                                }
                                                else
                                                {
                                                    // user cancelled
                                                    MessageBox.Show(oauthResult.ErrorDescription);
                                                }
                                            };
            
                web_browser1.Navigate(loginUrl);
            });



        }

        private static Uri GetFacebookLoginUrl(string AppId, string ExtendedPermissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = AppId;
            parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
            parameters["response_type"] = "token";
            parameters["display"] = "touch";

            // add the 'scope' only if we have extendedPermissions.
            if (!string.IsNullOrEmpty(extendedPermissions))
            {
                // A comma-delimited list of permissions
                parameters["scope"] = extendedPermissions;
            }

            return _fb.GetLoginUrl(parameters);
        }

        private ObservableCollection<AlbumModel> alb_vm;

        public ObservableCollection<AlbumModel> Albums
        {
            get { return alb_vm; }
            set
            {
                if (alb_vm != value)
                    alb_vm = value;
            }
        }

        public void GetFacebookAlbums(string AccessToken, AlbumCollectionResultCallback AlbumsResult)
        {


            Dispatcher.BeginInvoke(() =>
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
                            //Dispatcher().BeginInvoke(() => {
                   
                       

                        album = (JsonObject)albm;


                        string image_name = (string)album["name"];
                        string album_id = (string)album["id"];
                        string image_id = (string)album["cover_photo"];


                        GetFacebookImage(image_id, AccessToken, (img) =>
                        {
                            //
                             GetAlbumPhotos(album_id, AccessToken, (imgCollection) =>
                                 {

                                 alb = new AlbumModel();
                                 alb.Name = image_name;
                                 alb.AlbumID = album_id;
                                 alb.Thumbnail = img;
                                 alb.Type = AlbumModel.ALBUM_TYPE.FACEBOOK;
                                 alb.ImageThumbnails = imgCollection;
                                 
                                 Albums.Add(alb);
                                 if (Albums.Count == album.Count)
                                 {
                                     facebook_albums_loaded = true;
                                     AlbumsResult(Albums);
                                 }
                             });
                        });


                 
                    //});      

                    }
                //});
            };

            fb.GetTaskAsync("me/albums");
        
        });
        } 

        public void GetFacebookImages(string Token, ImageCollectionResultCallback result)
        {

        }

        public delegate void FacebookImageResult();

        public void GetFacebookImage(string ImageID, string AccessToken, ImageCallbackResult result)
        {
            Dispatcher.BeginInvoke(() =>
            {
                FacebookClient fbClient = new FacebookClient(AccessToken);
                fbClient.GetCompleted += (sender, ev) =>
                      {

                          var result_1 = (IDictionary<string, object>)ev.GetResultData();
                          //JsonObject JsonResult = (JsonObject)result_1["data"];


                          var imgLst = (IList<object>)result_1["images"];
                          string Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

                          // Download album thumnail async
                          LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(Thumbnail, (image) =>
                          {
                              result(image);
                          });

                      };
                fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken);
            });
        }


        private ObservableCollection<ImageViewModel> Images;
        public void GetAlbumPhotos(string album_id, string accessToken, ImageCollectionResultCallback result)
        {

            var fb = new FacebookClient(accessToken);

            Dispatcher.BeginInvoke(()=>{
                Images = new ObservableCollection<ImageViewModel>();
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
                        LockeScreen.ViewModels.AlbumsViewModel.ImageDownloader.DownloadImage(thumbnail, (bitmap) =>
                        {
                            Image = new ImageViewModel();
                            Image.ImageID = id;
                            Image.Image = bitmap;
                            
                            Images.Add(Image);

                            if (imgCounter == data.Count - 1)
                            {
                                //
                                result(Images);
                            }
                            imgCounter++;
                        });
                    
                }
            };

            fb.GetTaskAsync("https://graph.facebook.com/"+album_id + "/photos?access_token=" + accessToken);
            });

        }
 
        private void GetAlbumCover(string AccessToken, FacebookImageResult result)
        {
        }
    }
}
