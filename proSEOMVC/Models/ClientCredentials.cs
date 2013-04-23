using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace proSEOMVC.Models
{
    public class ClientCredentials
    {
        /// <summary>
        /// The OAuth2.0 Client ID of your project.
        /// </summary>

      
        public static readonly string CLIENT_ID = "CLIENTID.apps.googleusercontent.com";
        
        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
        public static readonly string CLIENT_SECRET = "CLIENT_SECRET";
        public static readonly string ServiceAccountId = "CLIENTID.apps.googleusercontent.com";
        public static readonly string ServiceAccountUser = "CLIENTID@developer.gserviceaccount.com";

        /// <summary>
        /// The OAuth2.0 scopes required by your project.
        /// </summary>
        public static readonly string SCOPES =
            "https://www.googleapis.com/auth/userinfo.email" + " " +
            "https://www.googleapis.com/auth/userinfo.profile" + " " +
    "https://www.googleapis.com/auth/analytics.readonly";
      

        /// <summary>
        /// The Redirect URI of your project.
        /// </summary>
        public static readonly string REDIRECT_URI = "http://localhost:60748/";
    }
}