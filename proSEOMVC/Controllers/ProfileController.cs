using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Authentication;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using proSEOMVC.Models;

namespace proSEOMVC.Controllers
{
    public class ProfileController : Controller
    {
        public JsonResult Index()
        {
            
            IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
            AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });
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
            AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });
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

        public JsonResult CheckAuth()
        {
            const string ServiceAccountId = "ServiceAccountId.apps.googleusercontent.com";
            const string ServiceAccountUser = "ServiceAccountUser@developer.gserviceaccount.com";
            X509Certificate2 cert = new X509Certificate2();
            try
            {
                 cert =
                    new X509Certificate2(
                        System.Web.HttpContext.Current.Server.MapPath(
                            "~/Content/key/0eee0d919aaef70bbb5da23b192aede576577058-privatekey.p12"), "notasecret",
                        X509KeyStorageFlags.Exportable);

            }
            catch (Exception ex)
            {
            }

            AssertionFlowClient client = new AssertionFlowClient(
            GoogleAuthenticationServer.Description, cert)
            {

                Scope = Oauth2Service.Scopes.UserinfoProfile.GetStringValue(),
                //  AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue(),
                ServiceAccountId = ServiceAccountUser //Bug, why does ServiceAccountUser have to be assigned to ServiceAccountId
                //,ServiceAccountUser = ServiceAccountUser
            };
            OAuth2Authenticator<AssertionFlowClient> authenticator = new OAuth2Authenticator<AssertionFlowClient>(client, AssertionFlowClient.GetState);

          
        
            JsonResult results = Json(authenticator);
              results.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return results;

        }

        [HttpPost]
        public ActionResult Report(AnalyticsProfile profile)
        {
           IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
           var garesults = Utils.GetReport(profile, authenticator);
            if(garesults != null){
            garesults.profile = profile;
            var html = RenderViewToString("Report", garesults, this.ControllerContext);
            Printer.GeneratePDF(html, profile.id);
            return View(garesults);
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        public JsonResult ViewPDF(AnalyticsProfile profile)
        {
            IAuthenticator authenticator = Session["authenticator"] as IAuthenticator;
            var garesults = Utils.GetReport(profile, authenticator);

                garesults.profile = profile;
                var html = RenderViewToString("Report", garesults, this.ControllerContext);
               var output = Printer.GeneratePDF(html, profile.id);
            //    Response.BinaryWrite(output.GetBuffer());
            //    Response.Buffer = true;
            //    //Response.Clear();

            //    Response.ContentType = "application/pdf";
            //    Response.AddHeader("content-length", output.Length.ToString());
            //    Response.AddHeader("content-disposition", "attachment; filename=Test.pdf");
               
            ////Response.Flush();
             //  return File(result.Attachment.Data, MimeExtensionHelper.GetMimeType(result.Attachment.FileName), result.Attachment.FileName);
            //FileContentResult result = new FileContentResult(output.GetBuffer(), "application/octet-stream")
            //    {
            //        FileDownloadName = "Test.pdf"
            //    };

            return Json(output);
            // return FileContentResult(output, "application/pdf", Server.UrlEncode("Test.pdf"));


        }

        public static string RenderViewToString(string viewName, object model, ControllerContext context)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }

        //public void ShowPdf(string strFileName)
        //{
            
        //    Response.AppendHeader("Content-Disposition", "inline; filename=test.pdf");
        //    var output = Printer.GeneratePDF(strFileName);
        //  //  return File(output, "application/pdf");
        //    Response.ClearContent();
        //    Response.ClearHeaders();
        //    Response.AddHeader("Content-Disposition", "inline;filename=" + strFileName);
        //    Response.ContentType = "application/pdf";
        //    Response.WriteFile(strFileName);
        //    Response.Flush();
        //    Response.Clear();
        //}
    }
}
