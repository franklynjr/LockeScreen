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
using Facebook;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using LockeScreen.ViewModels;
using System.Windows.Media.Imaging;
using LockeScreen.Models;


namespace LockeScreen.Models
{
    public class FacebookHelper
    {
        /* TODO: Change  GetAsync back to GetTaskAsync*/

        const string AppId = "139219706255772";
        const string extendedPermissions = "user_about_me,read_stream,publish_stream,user_photos";
        FacebookClient _fb = new FacebookClient();
        string AccessToken = "";
        WebBrowser web_browser1;

        protected Dispatcher Dispatcher
        {
            get { return Deployment.Current.Dispatcher; }
        }

        public delegate void AccessTokenReceivedHandler(string result);
        public delegate void AccessTokenRequestFailedHandler(string result);
        public delegate void DownloadFailHandler(object sender, EventArgs e);

        public event AccessTokenReceivedHandler AccessTokenReceived;
        public event AccessTokenRequestFailedHandler AccessTokenRequestFailed;
        public event DownloadFailHandler DownloadFailed;


        private ObservableCollection<AlbumPreview> _Albums;
        

        // Events
        public delegate void GetAlbumsEventHandler(ObservableCollection<AlbumPreview> AlbumResult);
        public delegate void GetAlbumImagesEventHandler(ObservableCollection<ImageViewModel> AlbumResult);
        public delegate void GetImageEventHandler(ImageViewModel ImageResult);
        public delegate void GetBitmapImageCompletedHandler(BitmapImage Image);

       // public event GetAlbumImagesEventHandler GetAlbumImagesCompleted;
        public event GetAlbumsEventHandler GetAlbumsCompleted;
        //public event GetBitmapImageCompletedHandler GetBitmapImageCompleted;



        //Callbacks

        public delegate void ResultCallback();
        public delegate void TokenCallback(string Token);



        public bool IsLoggedin
        {
            get;
            private set;

        }

        public bool Albumsloaded
        {
            get;
            private set;

        }


        public FacebookHelper()
        {

            web_browser1 = new WebBrowser();
            web_browser1.IsScriptEnabled = true;

            IsLoggedin = false;
            _Albums = new ObservableCollection<AlbumPreview>();
        }


        public void GetAccessToken()
        {

            Dispatcher.BeginInvoke(() =>
            {
                web_browser1 = new WebBrowser();
                web_browser1.IsScriptEnabled = true;

                IsLoggedin = false;

                //Load Operations
                var loginUrl = GetFacebookLoginUrl(AppId, extendedPermissions);
                web_browser1.NavigationFailed += (nav_sender, nav_event) =>
                {
                    GetAccessToken();
                };

                web_browser1.Navigated += (nav_o, nav_e) =>
                {
                    

                    FacebookOAuthResult oauthResult;
                    if (!_fb.TryParseOAuthCallbackUrl(nav_e.Uri, out oauthResult))
                    {

                        if (nav_e.Uri.AbsolutePath == "/connect/blank.html")
                            return;
                        else
                            AccessTokenRequestFailed("Login Failed");

                    }
                    if (!Equals(oauthResult, null))
                    {
                        if (oauthResult.IsSuccess)
                        {

                            AccessToken = oauthResult.AccessToken;
                            IsLoggedin = true;
                            AccessTokenReceived(AccessToken);
                            return;
                            //GetAlbums(accessToken);
                        }
                        else
                        {
                            // user cancelled
                            AccessTokenRequestFailed("Login Failed");
                        }
                    }
                    else
                        AccessTokenRequestFailed("Login Failed"); 
                };

                web_browser1.Navigate(loginUrl);
            });
        }


        private Uri GetFacebookLoginUrl(string AppId, string ExtendedPermissions)
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


        public void GetAlbums(string result)
        {

            // throw new NotImplementedException();
            GetFacebookAlbums(result, (o) =>
            {
                GetAlbumsCompleted(o);
            });

        }



        public void GetFacebookAlbums(string AccessToken, ResultCallbacks.AlbumCollectionResultCallback AlbumsResult)
        {


            var fb = new FacebookClient(AccessToken);

            //ObservableCollection<FacebookAlbum> Albums = new ObservableCollection<FacebookAlbum>();

            fb.GetCompleted += (o, e) =>
            {
                Dispatcher.BeginInvoke(() =>
                    {

                        if (e.Error != null)
                        {
                            if (fbAttemptCount < MAX_ATTEMPTS)
                            {
                                GetFacebookAlbums(AccessToken, (newRes) =>
                                {
                                    fbAttemptCount = 0;
                                    AlbumsResult(newRes);
                                });
                                return;
                            }
                        }



                        //Dispatcher.BeginInvoke(() =>
                        //{
                        var result = (IDictionary<string, object>)e.GetResultData();
                        //var id = (string)result["data"];
                        var data = (IList<object>)result["data"];

                        JsonObject album = new JsonObject();


                        foreach (var albm in data)
                        {


                        AlbumPreview alb = new AlbumPreview();

                            album = (JsonObject)albm;

                            try
                            {
                                string album_name = (string)album["name"];
                                string album_id = (string)album["id"];
                                string image_id = (string)album["cover_photo"];

                                alb.AlbumID = album_id;
                                alb.Name = album_name;
                                GetFacebookAlbumThumbnailImage(image_id, alb, AccessToken, (img) =>
                                {

                                    _Albums.Add(img);
                                    if (_Albums.Count == album.Count)
                                    {
                                        //facebook_albums_loaded = true;
                                        try { GetAlbumsCompleted(_Albums); }
                                        catch (NullReferenceException) { }
                                        catch { }
                                        AlbumsResult(_Albums);
                                    }
                                });


                            }
                            catch { }

                        }
                    });
                //});
            };

            fb.GetTaskAsync("me/albums");

        }


        public void GetFacebookImages(string album_id, ResultCallbacks.ImageCollectionResultCallback ImageResult)
        {
            //string image_id;

            AlbumPreview alb = new AlbumPreview();

                GetAlbumPhotos(album_id, AccessToken, (imgCollection) =>
                {

                    try
                    {

                        //alb = new AlbumPreview();
                        //alb.Name = image_name;
                        //alb.AlbumID = album_id;
                        //alb.Type = AlbumPreview.ALBUM_TYPE.FACEBOOK;
                        //alb.ImageThumbnails = imgCollection;
                        //alb.NextUrl = next;

                        //_Albums.Add(alb);
                        //if (_Albums.Count == album.Count)
                        //{
                        //    //facebook_albums_loaded = true;
                        //    try { GetAlbumsCompleted(_Albums); }
                        //    catch (NullReferenceException) { }
                        //    catch { }
                        //    AlbumsResult(_Albums);
                        //}

                    }
                    catch
                    {
                        if (DownloadFailed != null)
                        {
                            DownloadFailed(this, new EventArgs());
                        } 
                    }
                });


        }


        public void GetFacebookImage(string ImageID, string AccessToken, ResultCallbacks.ImageCallbackResult result)
        {

            FacebookClient fbClient = new FacebookClient(AccessToken);
            fbClient.GetCompleted += (sender, ev) =>
            {

                var result_1 = (IDictionary<string, object>)ev.GetResultData();
                //JsonObject JsonResult = (JsonObject)result_1["data"];


                var imgLst = (IList<object>)result_1["images"];
                string img = Convert.ToString(((JsonObject)imgLst[1])["source"]);

                // Download album thumnail async

                ImageDownloader imgDownloader = new ImageDownloader();
                imgDownloader.DownloadImage(img, (bitmap) =>
                {
                    //try
                    //{
                    //      GetBitmapImageCompleted(bitmap);
                    //}
                    //catch (NullReferenceException) { }
                    //catch { }

                    result(bitmap);

                });
            };
            string req = "https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken;
            fbClient.GetTaskAsync(req);

        }



        public delegate void FacebookImageResult();



        int fbAttemptCount = 0;
        const int MAX_ATTEMPTS = 3;

        public void GetFacebookThumbnailImage(string ImageID, string AccessToken,ResultCallbacks.ImageCallbackResult result)
        {
            try
            {

                FacebookClient fbClient = new FacebookClient(AccessToken);
                fbClient.GetCompleted += (sender, ev) =>
                {
                    if (ev.Cancelled == true || ev.Error != null)
                    {
                        if (fbAttemptCount < MAX_ATTEMPTS)
                        {
                            GetFacebookThumbnailImage(ImageID, AccessToken, (newRes) =>
                            {
                                fbAttemptCount = 0;
                                result(newRes);
                                return;
                            });
                        }
                    }
                    else
                    {
                        var result_1 = (IDictionary<string, object>)ev.GetResultData();
                        //JsonObject JsonResult = (JsonObject)result_1["data"];


                        var imgLst = (IList<object>)result_1["images"];
                        string Thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

                        // Download album thumnail async

                        ImageDownloader imgDownloader = new ImageDownloader();
                        imgDownloader.DownloadFailed += (failedSender, e) =>
                        {
                            if (fbAttemptCount < MAX_ATTEMPTS)
                            {
                                imgDownloader.DownloadImage(Thumbnail, (bitmap) =>
                                {
                                    //try
                                    //{
                                    //    GetBitmapImageCompleted(bitmap);
                                    //}
                                    //catch (NullReferenceException) { }
                                    //catch { }

                                    result(bitmap);

                                });
                            }
                        };



                        imgDownloader.DownloadImage(Thumbnail, (bitmap) =>
                        {
                            //try
                            //{
                            //    GetBitmapImageCompleted(bitmap);
                            //}
                            //catch (NullReferenceException) { }
                            //catch { }

                            result(bitmap);

                        });
                    }
                };
                fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken);
            }
            catch (WebException)
            {
                if (fbAttemptCount < 3)
                {
                    fbAttemptCount++;

                    GetFacebookThumbnailImage(ImageID, AccessToken, (res) =>
                    {
                        result(res);
                    });

                }
                else
                {
                    if (this.DownloadFailed != null)
                    {
                        this.DownloadFailed(this, new EventArgs());
                    }
                }
            }
            catch
            {
                if (fbAttemptCount > 3)
                {

                    if (this.DownloadFailed != null)
                    {
                        this.DownloadFailed(this, new EventArgs());
                    }
                }

            }
        }



        public void GetFacebookAlbumThumbnailImage(string ImageID, AlbumPreview alb, string AccessToken, ResultCallbacks.AlbumCallbackResult result)
        {
            try
            {

                FacebookClient fbClient = new FacebookClient(AccessToken);
                fbClient.GetCompleted += (sender, ev) =>
                {
                    if (ev.Cancelled == true || ev.Error != null)
                    {
                        if (fbAttemptCount < MAX_ATTEMPTS)
                        {
                            GetFacebookThumbnailImage(ImageID, AccessToken, (newRes) =>
                            {
                                fbAttemptCount = 0;
                                alb.Thumbnail = newRes;
                                result(alb);
                                return;
                            });
                        }
                    }
                    else
                    {
                        var result_1 = (IDictionary<string, object>)ev.GetResultData();
                        //JsonObject JsonResult = (JsonObject)result_1["data"];


                        var imgLst = (IList<object>)result_1["images"];
                        string Thumbnail = Convert.ToString(((JsonObject)imgLst[5])["source"]);

                        // Download album thumnail async

                        ImageDownloader imgDownloader = new ImageDownloader();
                        imgDownloader.DownloadFailed += (failedSender, e) =>
                        {
                            if (fbAttemptCount < MAX_ATTEMPTS)
                            {
                                imgDownloader.DownloadImage(Thumbnail, (bitmap) =>
                                {
                                    //try
                                    //{
                                    //    GetBitmapImageCompleted(bitmap);
                                    //}
                                    //catch (NullReferenceException) { }
                                    //catch { }
                                    alb.Thumbnail = bitmap;
                                    result(alb);

                                });
                            }
                        };



                        imgDownloader.DownloadImage(Thumbnail, (bitmap) =>
                        {
                            //try
                            //{
                            //    GetBitmapImageCompleted(bitmap);
                            //}
                            //catch (NullReferenceException) { }
                            //catch { }
                            alb.Thumbnail = bitmap;
                            result(alb);

                        });
                    }
                };
                fbClient.GetTaskAsync("https://graph.facebook.com/" + ImageID + "?access_token=" + AccessToken);
            }
            catch (WebException)
            {
                if (fbAttemptCount < 3)
                {
                    fbAttemptCount++;

                    GetFacebookThumbnailImage(ImageID, AccessToken, (res) =>
                    {
                        alb.Thumbnail = res;
                        result(alb);
                    });

                }
                else
                {
                    if (this.DownloadFailed != null)
                    {
                        this.DownloadFailed(this, new EventArgs());
                    }
                }
            }
            catch
            {
                if (fbAttemptCount > 3)
                {

                    if (this.DownloadFailed != null)
                    {
                        this.DownloadFailed(this, new EventArgs());
                    }
                }

            }
        }


        //public void GetNextFacebookPhotos(string url, string accessToken, ResultCallbacks.ImageCollectionResultCallback result)
        //{

        //    var fb = new FacebookClient(accessToken);


        //    ObservableCollection<ImageViewModel> Images = new ObservableCollection<ImageViewModel>();
        //    ImageViewModel Image;
        //    fb.GetCompleted += (o, e) =>
        //    {
        //        if (e.Error != null)
        //        {

        //            if (fbAttemptCount < MAX_ATTEMPTS)
        //            {
        //                GetAlbumPhotos(url, accessToken, (newRes) =>
        //                {
        //                    fbAttemptCount = 0;
        //                    result(newRes);
        //                });
        //                return;
        //            }
        //            return;
        //        }

        //        var result_1 = (IDictionary<string, object>)e.GetResultData();

        //        //
        //        var data = (IList<object>)result_1["data"];
        //        var paging = (JsonObject)result_1["paging"];

        //        int imgCounter = 0;

        //        var _nextList = new object();
        //        var nextList = "";
        //        if (((JsonObject)paging).TryGetValue("next", out _nextList))
        //            nextList = Convert.ToString(_nextList);


        //        foreach (var img in data)
        //        {
        //            //
        //            JsonObject image_info = (JsonObject)img;
        //            //

        //            string id;
        //            string thumbnail;
        //            //string scaled_image;

        //            id = Convert.ToString(image_info["id"]);

        //            var imgLst = (IList<object>)image_info["images"]; // List of Image sizes

        //            //scaled_image = Convert.ToString(((JsonObject)imgLst[1])["source"]);
        //            thumbnail = Convert.ToString(((JsonObject)imgLst[7])["source"]);

        //            string image_id = id;
        //            ImageDownloader imgDownloader = new ImageDownloader();
        //            imgDownloader.DownloadImage(thumbnail, (bitmap) =>
        //            {
        //                Image = new ImageViewModel();
        //                Image.ImageID = id;
        //                Image.Image = bitmap;

        //                Images.Add(Image);

        //                if (imgCounter == data.Count - 1)
        //                {
        //                    //
        //                    //try { GetAlbumImagesCompleted(Images); }
        //                    //catch (NullReferenceException) { }
        //                    //catch { }
        //                    result(Images);
        //                    //return;
        //                }
        //                imgCounter++;
        //            });

        //        }
        //    };


        //    fb.GetTaskAsync(url);


        //}


        public void GetNextFacebookPhotos(string url,ObservableCollection<ImageViewModel> Images, string accessToken, ResultCallbacks.ImageCollectionResultCallback result)
        {

            var fb = new FacebookClient(accessToken);


            ImageViewModel Image;
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {

                    if (fbAttemptCount < MAX_ATTEMPTS)
                    {
                        fbAttemptCount++;
                        GetNextFacebookPhotos(url, Images,accessToken, (newRes) =>
                        {
                            fbAttemptCount = 0;
                            result(newRes);
                        });
                        return;
                    }
                    return;
                }

                var result_1 = (IDictionary<string, object>)e.GetResultData();

                //
                var data = (IList<object>)result_1["data"];
                var paging = (JsonObject)result_1["paging"];

                int imgCounter = 0;

                var _nextList = new object();
                var nextList = "";
                if (((JsonObject)paging).TryGetValue("next", out _nextList))
                    nextList = Convert.ToString(_nextList);


                foreach (var img in data)
                {
                    //
                    JsonObject image_info = (JsonObject)img;
                    //

                    string id;
                    string thumbnail;
                    //string scaled_image;

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
                            //try { GetAlbumImagesCompleted(Images); }
                            //catch (NullReferenceException) { }
                            //catch { }
                            if (!(nextList == null || nextList == ""))
                                GetNextFacebookPhotos(nextList, Images, accessToken, (Images_1) =>
                                {

                                    result(Images_1);
                                        return;
                                });

                            else

                                result(Images);
                            //return;
                        }
                        imgCounter++;
                    });
                };

            };

            fb.GetTaskAsync(url);


        }
        
        


        public void GetAlbumPhotos(string album_id, string accessToken, ResultCallbacks.ImageCollectionResultCallback result)
        {

            var fb = new FacebookClient(accessToken);


            ObservableCollection<ImageViewModel> Images = new ObservableCollection<ImageViewModel>();
            ImageViewModel Image;
            fb.GetCompleted += (o, e) =>
            {
                if (e.Error != null)
                {

                    if (fbAttemptCount < MAX_ATTEMPTS)
                    {
                        fbAttemptCount++;
                        GetAlbumPhotos(album_id, accessToken, (newRes) =>
                        {
                            fbAttemptCount = 0;
                            result(newRes);
                        });
                        return;
                    }
                    return;
                }

                var result_1 = (IDictionary<string, object>)e.GetResultData();

                //
                var data = (IList<object>)result_1["data"];
                var paging = (JsonObject)result_1["paging"];

                int imgCounter = 0;

                // Vacation Albums Test

                if (album_id == "10150994682003686")
                    imgCounter = 0;
                else
                {
                }


                var _nextList = new object();
                var nextList = "";
                if (((JsonObject)paging).TryGetValue("next", out _nextList))
                    nextList = Convert.ToString(_nextList);

                foreach (var img in data)
                {
                    //
                    JsonObject image_info = (JsonObject)img;
                    //

                    string id;
                    string thumbnail;
                    //string scaled_image;

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
                            //try { GetAlbumImagesCompleted(Images); }
                            //catch (NullReferenceException) { }
                            //catch { }

                            if (!(nextList == "" || nextList == null))
                            {
                                GetNextFacebookPhotos(nextList, Images, accessToken, (imageCollectionResult) =>
                                {

                                    // 

                                    result(Images);
                                    return;
                                });
                            }
                            else
                                result(Images);
                            //return;
                        }
                        imgCounter++;
                    });

                }
            };


            fb.GetTaskAsync("https://graph.facebook.com/" + album_id + "/photos?access_token=" + accessToken);


        }

        public class FacebookLoginFailedException : Exception
        {
            public FacebookLoginFailedException()
            {
                _message = "Login Failed";
            }



            public FacebookLoginFailedException(string message)
            {
                _message = message;
            }

            private string _message;

            public override string Message
            {
                get
                {
                    return _message;
                }
                
            }

        }




    }
}
