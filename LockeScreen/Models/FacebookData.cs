using Facebook;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
namespace LockeScreen.Models
{
    public class FacebookData :PhoneApplicationPage
    {
        //
        private bool isLoggedIn = false;
        private JsonObject json = new JsonObject();
        private List<FacebookAlbum> _Album;
        private string _AccessToken;

        private bool _ImagesLoaded = false;

        WebBrowser webBrowser1;

        //
        public bool UserIsLoggedIn
        {
            get { return isLoggedIn; }
            set
            {
                if (isLoggedIn != value)
                {
                    isLoggedIn = value;
                }
            }
        }

        //
        public bool ImagesLoaded
        {
            get { return _ImagesLoaded; }
            set
            {
                if (_ImagesLoaded != value)
                {
                    _ImagesLoaded = value;
                }
            }
        }

        //
        public string AccessToken
        {
            get { return _AccessToken; }
            set
            {
                if (_AccessToken != value)
                {
                    _AccessToken = value;
                }
            }
        }

        //
        public List<FacebookAlbum> Albums
        {
            get { return _Album; }
            set
            {
                if (_Album != value)
                {
                    _Album = value;
                }
            }
        }

        //
        public FacebookData()
        {
            _Album = new List<FacebookAlbum>();
            webBrowser1 = new WebBrowser();
            webBrowser1.IsScriptEnabled = true;
            webBrowser1.Navigated += webBrowser1_Navigated;
            webBrowser1_Loaded(this, new RoutedEventArgs());
        }

        private const string AppId = "139219706255772";

        /// <summary>
        /// Extended permissions is a comma separated list of permissions to ask the user.
        /// </summary>
        /// <remarks>
        /// For extensive list of available extended permissions refer to 
        /// https://developers.facebook.com/docs/reference/api/permissions/
        /// </remarks>
        private const string ExtendedPermissions = "user_about_me,read_stream,publish_stream,user_photos";

        private readonly FacebookClient _fb = new FacebookClient();


        //
        private void webBrowser1_Loaded(object sender, RoutedEventArgs e)
        {
            var loginUrl = GetFacebookLoginUrl(AppId, ExtendedPermissions);
            webBrowser1.Navigate(loginUrl);
        }

        //
        private Uri GetFacebookLoginUrl(string appId, string extendedPermissions)
        {
            var parameters = new Dictionary<string, object>();
            parameters["client_id"] = appId;
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

        //
        private void webBrowser1_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            FacebookOAuthResult oauthResult;
            if (!_fb.TryParseOAuthCallbackUrl(e.Uri, out oauthResult))
            {
                isLoggedIn = false;
                return;
            }

            if (oauthResult.IsSuccess)
            {
                isLoggedIn = true;
                var accessToken = oauthResult.AccessToken;
                //GetAlbums(accessToken);
            }
            else
            {
                isLoggedIn = false;
                // user cancelled
                MessageBox.Show(oauthResult.ErrorDescription);
            }
        }

        

        private void GetAlbumImages() { }

        protected new Dispatcher Dispatcher()
        {
            
            return  Deployment.Current.Dispatcher; 
        }
       
    }
}
