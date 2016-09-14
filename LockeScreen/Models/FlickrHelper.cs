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
using System.IO.IsolatedStorage;
using FlickrNet;
using System.Collections.ObjectModel;
using LockeScreen.ViewModels;
using LockeScreen.Models;
using Microsoft.Phone.Controls;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using LockeScreen.Common;

namespace LockeScreen.Models
{
    public class FlickrHelper
    {
        string flickrToken = "";
        public bool loggedIn = false;
        Flickr flickr;
        private string User;
        private string UserId;

        public FlickrHelper()
        {
            // Check if access token was already stored
            InitWrapper();

        }

        public delegate void FlickrAlbumCollectionCallback(ObservableCollection<AlbumPreview> FlickrAlbums);
        public delegate void FlickrAlbumImagesCollectionCallback(ObservableCollection<ImageViewModel> FlickrAlbums);



        public delegate void LoginEventHandler(ResultCallbacks.LoginResult Result);
        public delegate void DownloadFailHandler(object sender, EventArgs e);

        public event LoginEventHandler LoginAttempted;
        public event DownloadFailHandler DownloadFailed;


        public Dispatcher Dispatcher
        {
            get { return Deployment.Current.Dispatcher; }
        }

        public void Login()
        {
            Dispatcher.BeginInvoke(() =>
            {
                if (!loggedIn)
                {
                    try
                    {
                        if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_TOKEN_KEY, out flickrToken))
                        {
                            try
                            {
                                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_USER_KEY, out User);
                                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_USER_ID_KEY, out UserId);
                            }
                            catch
                            {

                                loggedIn = false;
                                try
                                {
                                    LoginAttempted(ResultCallbacks.LoginResult.FAILED);
                                }
                                catch { }
                            }
                            flickr = new Flickr("d1dec6db23f8c2d433a03c050e458406", "75d66064ce412813", flickrToken);

                            flickr.AuthCheckTokenAsync((res) =>
                            {
                                if (res.Error != null)
                                {
                                    if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS - 2)
                                    {
                                        flickrRequestAttempts++;
                                        Login();
                                        flickrRequestAttempts = 0;
                                        return;
                                    }
                                }

                                if (res.Result.Permissions == AuthLevel.Write)
                                {
                                    IsolatedStorageSettings.ApplicationSettings[KEYS.FLICKR_USER_ID_KEY] = UserId = res.Result.User.UserId;
                                    loggedIn = true;

                                    if(LoginAttempted != null)
                                        LoginAttempted(ResultCallbacks.LoginResult.SUCCESS);
                                    
                                }
                            });
                        }
                        else { LoginAttempted(ResultCallbacks.LoginResult.FAILED); }
                    }
                    catch { }
                }
            });
        }


        private void InitWrapper()
        {

            Dispatcher.BeginInvoke(() =>
            {
                if (!loggedIn)
                {
                    try
                    {
                        if (IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_TOKEN_KEY, out flickrToken))
                        {
                            try
                            {
                                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_USER_KEY, out User);
                                IsolatedStorageSettings.ApplicationSettings.TryGetValue<string>(KEYS.FLICKR_USER_ID_KEY, out UserId);
                            }
                            catch
                            {

                                loggedIn = false;
                                
                            }
                            flickr = new Flickr("d1dec6db23f8c2d433a03c050e458406", "75d66064ce412813", flickrToken);

                            flickr.AuthCheckTokenAsync((res) =>
                            {
                                if (res.Result.Permissions == AuthLevel.Write)
                                {
                                    IsolatedStorageSettings.ApplicationSettings[KEYS.FLICKR_USER_ID_KEY] = UserId = res.Result.User.UserId;
                                    loggedIn = true;
                                    
                                }
                            });
                        }
                        else { LoginAttempted(ResultCallbacks.LoginResult.FAILED); }
                    }
                    catch { }
                }
            });

        }

        public void GetTaggedPhotos(string tag, FlickrAlbumImagesCollectionCallback flickrAlbumImagesCollectionCallback)
        {
            try
            {
                ObservableCollection<ImageViewModel> _Images = new ObservableCollection<ImageViewModel>();
                ImageViewModel image;

                PhotoSearchOptions pso = new PhotoSearchOptions("", tag);
                flickr.PhotosSearchAsync(pso, (res) =>
                {
                    PhotoCollection galPhotoCollection = res.Result;
                    if (res.Error == null)
                    {
                        foreach (GalleryPhoto photo in galPhotoCollection)
                        {
                            image = new ImageViewModel();

                            string ThumbnailUrl = photo.ThumbnailUrl;
                            string photoId = photo.PhotoId;


                            ImageDownloader imgDownloader = new ImageDownloader();
                            imgDownloader.DownloadImage(ThumbnailUrl, (imageBmp) =>
                            {
                                image.Image = imageBmp;
                                image.ImageID = photoId;

                                _Images.Add(image);
                                if (_Images.Count == galPhotoCollection.Count)
                                {
                                    flickrAlbumImagesCollectionCallback(_Images);
                                }
                            });
                        }
                    }
                    else
                    {
                        if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                        {
                            GetTaggedPhotos(tag, (newRes) =>
                                {
                                    flickrAlbumImagesCollectionCallback(newRes);
                                });
                        }
                    }
                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetTaggedPhotos(tag,(result) =>
                    {
                        flickrRequestAttempts = 0;
                        flickrAlbumImagesCollectionCallback(result);
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
            catch { }


        }


        public void MyPhotos(FlickrAlbumImagesCollectionCallback flickrAlbumImagesCollectionCallback)
        {
            ObservableCollection<ImageViewModel> _Images = new ObservableCollection<ImageViewModel>();
            ImageViewModel image;

            PhotoSearchOptions pso = new PhotoSearchOptions(UserId);

            flickr.PhotosSearchAsync(pso, (res) =>
            {
                PhotoCollection galPhotoCollection = res.Result;
                if (res.Error == null)
                {
                    foreach (Photo photo in galPhotoCollection)
                    {
                        image = new ImageViewModel();

                        string ThumbnailUrl = photo.ThumbnailUrl;
                        string photoId = photo.PhotoId;


                        ImageDownloader imgDownloader = new ImageDownloader();
                        imgDownloader.DownloadImage(ThumbnailUrl, (imageBmp) =>
                        {
                            image.Image = imageBmp;
                            image.ImageID = photoId;

                            _Images.Add(image);
                            if (_Images.Count == galPhotoCollection.Count)
                            {
                                flickrAlbumImagesCollectionCallback(_Images);
                            }
                        });
                    }
                }
                else
                {
                    if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                    {
                        flickrRequestAttempts++;
                        MyPhotos((resCb) =>
                        {
                            flickrRequestAttempts = 0;
                            flickrAlbumImagesCollectionCallback(resCb);
                        });
                    }
                }
            });
            
        }



        public void Search(string tag)
        {
            PhotoSearchOptions pso = new PhotoSearchOptions("", "flowers");
            //flickr.PhotosSearchAsync(pso, (res) => { 
            //    res.Result
            //});
        }
        public void GetPhotoSets(FlickrAlbumCollectionCallback flickrAlbumCollectionCallback)
        {
            try
            {
                ObservableCollection<AlbumPreview> _Albums = new ObservableCollection<AlbumPreview>();
                AlbumPreview _album = new AlbumPreview();

                flickr.PhotosetsGetListAsync((res) =>
                {
                    PhotosetCollection photosets = res.Result;
                    if (res.Error == null)
                    {
                        if (photosets.Count == 0)
                        {
                            //Do callback
                            flickrAlbumCollectionCallback(_Albums);
                        }
                        foreach (Photoset photo in photosets)
                        {
                            ImageDownloader imgDownloader = new ImageDownloader();
                            imgDownloader.DownloadImage(photo.PhotosetThumbnailUrl, (img) =>
                            {
                                _album = new AlbumPreview();
                                _album.Name = photo.Title;
                                _album.Thumbnail = img;
                                _album.GroupName = "Photosets";
                                _Albums.Add(_album);

                                if (_Albums.Count == photosets.Count)
                                {
                                    //Do callback
                                    flickrAlbumCollectionCallback(_Albums);
                                }

                            });

                        }
                    }
                    else
                    {
                        if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                        {
                            flickrRequestAttempts++;
                            GetPhotoSets((newRes) =>
                            {
                                flickrRequestAttempts = 0;
                                flickrAlbumCollectionCallback(newRes);
                            });
                        }
                    }
                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetPhotoSets((result) =>
                    {
                        flickrRequestAttempts = 0;
                        flickrAlbumCollectionCallback(result);
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
            catch { }
        }


        public void GetFavoriteImages(FlickrAlbumImagesCollectionCallback flickrAlbumImagesCollectionCallback)
        {
            ObservableCollection<ImageViewModel> _Images = new ObservableCollection<ImageViewModel>();
            ImageViewModel image;

            try
            {
                flickr.FavoritesGetListAsync((res) =>
                {
                    PhotoCollection galPhotoCollection = res.Result;

                    // try Again if there was an error
                    if (res.Error != null)
                    {
                        if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                        {
                            flickrRequestAttempts++;
                            GetFavoriteImages((newRes) =>
                            {
                                flickrRequestAttempts = 0;
                                flickrAlbumImagesCollectionCallback(newRes);
                                return;
                            });
                        }
                    }

                    foreach (Photo photo in galPhotoCollection)
                    {


                        string ThumbnailUrl = photo.ThumbnailUrl;
                        string photoId = photo.PhotoId;


                        ImageDownloader imgDownloader = new ImageDownloader();
                        imgDownloader.DownloadImage(ThumbnailUrl, (imageBmp) =>
                        {
                            image = new ImageViewModel();
                            image.Image = imageBmp;
                            image.ImageID = photoId;

                            _Images.Add(image);
                            if (_Images.Count == galPhotoCollection.Count)
                            {
                                flickrAlbumImagesCollectionCallback(_Images);
                            }
                        });
                    }
                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetFavoriteImages((result) =>
                    {
                        flickrRequestAttempts = 0;
                        flickrAlbumImagesCollectionCallback(result);
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
            catch { }
        }

        public void GetGalleryAlbums(FlickrAlbumCollectionCallback flickrAlbumCollectionCallback)
        {
            try
            {
                flickr.GalleriesGetListAsync((flickrRes) =>
                {
                    if (flickrRes.Error != null || flickrRes.HasError)
                    {
                        if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                        {
                            flickrRequestAttempts++;
                            GetGalleryAlbums((newRes) =>
                            {
                                flickrRequestAttempts = 0;
                                flickrAlbumCollectionCallback(newRes);
                                return;
                            });
                        }
                    }
                    else
                    {
                        GalleryCollection galleryCollection = flickrRes.Result;
                        ObservableCollection<AlbumPreview> _Albums = new ObservableCollection<AlbumPreview>();
                        AlbumPreview _album;

                        if (galleryCollection.Count == 0)
                        {

                            flickrAlbumCollectionCallback(_Albums);
                        }

                        foreach (Gallery gal in galleryCollection)
                        {


                            string albumId = gal.GalleryId;
                            string albumName = gal.Title;
                            string thumbnailUrl = gal.PrimaryPhotoThumbnailUrl;

                            ImageDownloader imgDownloader = new ImageDownloader();
                            imgDownloader.DownloadImage(thumbnailUrl, (imageBmp) =>
                            {

                                GetFlickrAlbumImages(albumId, (ImageRes) =>
                                {
                                    _album = new AlbumPreview();
                                    _album.GroupName = "Galleries";
                                    _album.AlbumID = albumId;
                                    _album.Name = albumName;
                                    _album.Thumbnail = imageBmp;
                                    _album.ImageThumbnails = ImageRes;

                                    _Albums.Add(_album);

                                    if (_Albums.Count == galleryCollection.Count)
                                    {
                                        flickrAlbumCollectionCallback(_Albums);
                                    }
                                });
                            });
                        }

                    }

                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetGalleryAlbums((result) =>
                    {
                        flickrRequestAttempts = 0;
                        flickrAlbumCollectionCallback(result);
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
            catch { }
        }



        public void GetMyFavoritesAlbum(FlickrAlbumCollectionCallback flickrAlbumCollectionCallback)
        {
            ObservableCollection<AlbumPreview> _Albums = new ObservableCollection<AlbumPreview>();
            AlbumPreview _Album = new AlbumPreview();

            GetFavoriteImages((images) =>
            {
                _Album.GroupName = "Favorites";
                _Album.Name = "Favorites";

                //Set the album thumbnail
                if (images.Count > 0)
                    _Album.Thumbnail = images[0].Image;


                _Album.ImageThumbnails = images;
                _Albums.Add(_Album);
                flickrAlbumCollectionCallback(_Albums);

            });
        }


        public void GetFlickrAlbumImages(string GalleryId, FlickrAlbumImagesCollectionCallback flickrAlbumImagesCollectionCallback)
        {
            try
            {
                ObservableCollection<ImageViewModel> _Images = new ObservableCollection<ImageViewModel>();
                ImageViewModel image;

                flickr.GalleriesGetPhotosAsync(GalleryId, (flickrRes) =>
                {
                    GalleryPhotoCollection galPhotoCollection = flickrRes.Result;
                    if (flickrRes.Error == null)
                    {
                        foreach (GalleryPhoto photo in galPhotoCollection)
                        {

                            string ThumbnailUrl = photo.ThumbnailUrl;
                            string photoId = photo.PhotoId;


                            ImageDownloader imgDownloader = new ImageDownloader();
                            imgDownloader.DownloadImage(ThumbnailUrl, (imageBmp) =>
                            {
                                image = new ImageViewModel();
                                image.Image = imageBmp;
                                image.ImageID = photoId;

                                _Images.Add(image);
                                if (_Images.Count == galPhotoCollection.Count)
                                {
                                    flickrAlbumImagesCollectionCallback(_Images);
                                }
                            });
                        }
                    }
                    else
                    {

                        if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                        {
                            flickrRequestAttempts++;
                            GetFlickrAlbumImages(GalleryId, (res) =>
                            {
                                flickrRequestAttempts = 0;
                                flickrAlbumImagesCollectionCallback(res);

                            });
                        }
                    }
                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetFlickrAlbumImages(GalleryId, (result) =>
                    {
                        flickrRequestAttempts = 0;
                        flickrAlbumImagesCollectionCallback(result);
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
            catch { }
        }


        


        public void GetMyPhotosAlbum(FlickrAlbumCollectionCallback flickrAlbumCollectionCallback)
        {
            ObservableCollection<AlbumPreview> _Albums = new ObservableCollection<AlbumPreview>();
            AlbumPreview _Album = new AlbumPreview();

            MyPhotos((images) =>
            {
                _Album.GroupName = "My Photos";
                _Album.Name = "All Photos";

                //Set the album thumbnail
                if (images.Count > 0)
                    _Album.Thumbnail = images[0].Image;


                _Album.ImageThumbnails = images;
                _Albums.Add(_Album);
                flickrAlbumCollectionCallback(_Albums);

            });
        }


        public void GetAllAlbums(FlickrAlbumCollectionCallback flickrAlbumCollectionCallback)
        {
            // ObservableCollection<AlbumPreview> _Albums = new ObservableCollection<AlbumPreview>();


            GetMyPhotosAlbum((_combined) =>
            {
                GetMyFavoritesAlbum((fAlbum) =>
                    {

                        foreach (AlbumPreview alb in fAlbum)
                        {
                            _combined.Add(alb);
                        }

                        GetGalleryAlbums((gAlbums) =>
                        {
                            foreach (AlbumPreview alb in gAlbums)
                            {
                                _combined.Add(alb);
                            }

                            GetPhotoSets((pAlbums) =>
                            {

                                foreach (AlbumPreview alb in pAlbums)
                                {
                                    _combined.Add(alb);
                                }

                                flickrAlbumCollectionCallback(_combined);
                            });
                        });
                    });
            });


        }
        int flickrRequestAttempts = 0;
        const int MAX_FLICKR_ATTEMPTS = 3;

        public void GetFlickrImage(string ImageId, ResultCallbacks.ImageCallbackResult ImageCallback)
        {
            //
            try
            {
                Dispatcher.BeginInvoke(() =>
                {
                    flickr.PhotosGetInfoAsync(ImageId, (imgInfo) =>
                    {
                        if (imgInfo.Error != null || imgInfo.HasError)
                        {
                            if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                            {
                                flickrRequestAttempts++;
                                GetFlickrImage(ImageId, (result) =>
                                {
                                    flickrRequestAttempts = 0;
                                    ImageCallback(result);
                                    return;
                                });
                            }
                        }
                        string url = imgInfo.Result.MediumUrl;

                        if (url != null)
                        {

                            ImageDownloader imgDownloader = new ImageDownloader();
                            imgDownloader.DownloadImage(url, (imgResult) =>
                            {
                                ImageCallback(imgResult);
                            });
                        }
                    });
                });
            }
            catch (WebException)
            {
                if (flickrRequestAttempts < MAX_FLICKR_ATTEMPTS)
                {
                    flickrRequestAttempts++;
                    GetFlickrImage(ImageId, (result) => {
                        flickrRequestAttempts = 0;
                        ImageCallback(result);
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
            catch { }

        }
    }
}
