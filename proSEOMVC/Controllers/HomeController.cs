using System;
using System.Web.Mvc;
using Google.Apis.Analytics.v3;
using Google.Apis.Authentication;
using Google.Apis.Oauth2.v2.Data;
using proSEOMVC.Models;

namespace proSEOMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                Session["authenticator"] = Utils.GetCredentials();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }

            IAuthenticator auth = Session["authenticator"] as IAuthenticator;

            Userinfo userInfo = Utils.GetUserInfo(auth);
            var user = new UserProfile();
            user.id = userInfo.Id;
            user.email = userInfo.Email;
            user.name = userInfo.Name;
            //ViewBag.Email = userInfo.Email;
           
            IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
            AnalyticsService service = new AnalyticsService(authenticator);
            
            var profiles = service.Management.Profiles.List("~all", "~all").Fetch();
            user.profiles = profiles;

            return View(user);
        }

    }
}