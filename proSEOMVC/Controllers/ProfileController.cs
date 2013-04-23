using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Authentication;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using proSEOMVC.Models;

namespace proSEOMVC.Controllers
{
    public class ProfileController : Controller
    {
        public JsonResult Index()
        {
            
            IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
            AnalyticsService service = new AnalyticsService(authenticator);
            Profiles profiles = new Profiles();
            // profiles = 
            profiles = service.Management.Profiles.List("~all", "~all").Fetch();
            JsonResult results = Json(profiles);
            results.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return results;

        }

        public JsonResult Detail(string id)
        {
            IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
            AnalyticsService service = new AnalyticsService(authenticator);
            if (id == null) 
            {
                var data = service.Management.Profiles.List("~all", "~all").Fetch();
                JsonResult datas = Json(data);
                datas.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                
                return datas;
            }
            else
            {

                var garesults = Utils.GetProfileReport(id, authenticator);
                JsonResult results = Json(garesults);
              //  JsonResult rows = Json(garesults.JSON);
                results.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
              //  rows.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                return results;
            }

        }

        [HttpPost]
        public ActionResult Report(AnalyticsProfile profile)
        {
           IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;

           var garesults = Utils.GetReport(profile, authenticator);
            garesults.profile = profile;
           
            return View(garesults);
        }

   

    }
}
