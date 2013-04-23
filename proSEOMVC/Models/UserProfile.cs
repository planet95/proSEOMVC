using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google;
using Google.Apis.Authentication;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;

namespace proSEOMVC.Models
{
    public class UserProfile
    {
        public UserProfile()
        {
            this.profiles = profiles;
        }

        public string name { get; set; }
        public string test { get; set; }
        public string id { get; set; }
        public string email { get; set; }
        public Profiles profiles { get; set; }

    }


}