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
       
      // public static readonly string CLIENT_ID = "769700535267-h0a3vgv6q3soj7i0kv2d19h01mpk61mk.apps.googleusercontent.com";
       public static readonly string CLIENT_ID = "769700535267.apps.googleusercontent.com";
        
        /// <summary>
        /// The OAuth2.0 Client secret of your project.
        /// </summary>
        public static readonly string CLIENT_SECRET = "Jq1diirfS_7ktlBE5OzfHi8v";
        public static readonly string ServiceAccountId = "769700535267.apps.googleusercontent.com";
        public static readonly string ServiceAccountUser = "769700535267@developer.gserviceaccount.com";

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