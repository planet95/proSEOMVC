using System;
using System.Web.Mvc;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using proSEOMVC.Models;

namespace proSEOMVC.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            var user = new UserProfile();
            try
            {
                Session["authenticator"] = Utils.getCredentials(Oauth2Service.Scopes.UserinfoProfile.GetStringValue());
          //  AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue(),
     
            //ViewBag.Email = userInfo.Email;

            //try
            //{
            //    Session["authenticator"] = Utils.getCredsnew(AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue());
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.Error = ex.Message;
            //    return View();
            //}

            IAuthenticator auth = Session["authenticator"] as IAuthenticator;

            Userinfo userInfo = Utils.GetUserInfo(auth);
            user = new UserProfile();
            user.id = userInfo.Id;
            user.email = userInfo.Email;
            user.name = userInfo.Name;
            }

              catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }

           
            try
            {
                Session["authenticator"] = Utils.getCredentials(AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue());
                IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
                AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });
                var profiles = service.Management.Profiles.List("~all", "~all").Fetch();
                user.profiles = profiles;
            
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View("Error");
            }
         

            return View(user);
        }

    }
}