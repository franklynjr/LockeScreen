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
    public class FacebookLoginHelper
    {


        const string AppId = "139219706255772";
        const string extendedPermissions = "user_about_me,read_stream,publish_stream,user_photos";
        FacebookClient _fb = new FacebookClient();
        string AccessToken = "";

        protected Dispatcher Dispatcher
        {
            get { return Deployment.Current.Dispatcher; }
        }

        public delegate void AccessTokenReceivedHandler(string result);
        public delegate void AccessTokenRequestFailedHandler( string result);

        public event AccessTokenReceivedHandler AccessTokenReceived;
        public event AccessTokenRequestFailedHandler AccessTokenRequestFailed;

        WebBrowser web_browser1;

        public void GetAccessToken()
        {
            

                 web_browser1 = new WebBrowser();
                web_browser1.IsScriptEnabled = true;

                IsLoggedin = false;
           
                //Load Operations
                var loginUrl = GetFacebookLoginUrl(AppId, extendedPermissions);


                web_browser1.Navigated += (nav_o, nav_e) =>
                {

                    FacebookOAuthResult oauthResult;
                    if (!_fb.TryParseOAuthCallbackUrl(nav_e.Uri, out oauthResult))
                    {

                        AccessTokenRequestFailed("Login Failed");
                        return;
                    }

                    if (oauthResult.IsSuccess)
                    {

                        AccessToken = oauthResult.AccessToken;
                        IsLoggedin = true;
                        AccessTokenReceived(AccessToken);
                        //GetAlbums(accessToken);
                    }
                    else
                    {
                        // user cancelled
                        AccessTokenRequestFailed(oauthResult.ErrorDescription);
                    }
                };

            web_browser1.Navigate(loginUrl);

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

        public bool IsLoggedin
        {
            get;
            private set;

        }

    }
}
