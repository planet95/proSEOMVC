using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Hosting;
using System.Web.Routing;
using Google.Apis.Authentication;
using Google;
using System.Net;
using System.IO;
using Google.Apis.Authentication.OAuth2;
using Google.Apis.Authentication.OAuth2.DotNetOpenAuth;
using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using System.Text;
using Google.Apis.Oauth2.v2;
using Google.Apis.Oauth2.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util;
using Codaxy.WkHtmlToPdf;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using log4net;
using PdfDocument = Codaxy.WkHtmlToPdf.PdfDocument;

namespace proSEOMVC.Models
{
    public class Utils
    {

        private static readonly ILog log = LogManager.GetLogger(typeof (Utils));

    
    public static IAuthenticator getCredentials(string scope)
    {
        const string ServiceAccountId = "SERVICEACCOUNTID.apps.googleusercontent.com";
        const string ServiceAccountUser = "ServiceAccountUser@developer.gserviceaccount.com";
      try
      {
          AssertionFlowClient client = new AssertionFlowClient(
              GoogleAuthenticationServer.Description,
              new X509Certificate2(
                  HttpContext.Current.Server.MapPath(
                      "~/Content/key/0eee0d919aaef70bbb5da23b192aede576577058-privatekey.p12"), "notasecret",
                  X509KeyStorageFlags.Exportable))
              {

                  Scope = scope,
                  //  AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue(),
                  ServiceAccountId = ServiceAccountUser
                  //Bug, why does ServiceAccountUser have to be assigned to ServiceAccountId
                  //,ServiceAccountUser = ServiceAccountUser
              };

          OAuth2Authenticator<AssertionFlowClient> authenticator = new OAuth2Authenticator<AssertionFlowClient>(client,
                                                                                                                AssertionFlowClient
                                                                                                                    .GetState);

          return authenticator;
      }
      catch (Exception ex)
      {
          log.Error("NOOOOOOO: " + ex.Message);
          throw new Exception("AHHHHHHHHHHHHH");
          
          
      }

       
     //   AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });
        //string profileId = "ga:64968920";
        //string startDate = "2010-10-01";
        //string endDate = "2010-10-31";
        //string metrics = "ga:visits";
        //DataResource.GaResource.GetRequest request = service.Data.Ga.Get(profileId, startDate, endDate, metrics);
        //request.Dimensions = "ga:date";
        //GaData data = request.Fetch();  


        return null;
    }
  
    public static string JWT(String client_id, String key)
            {
                String lauth_token = "";
                String SCOPE = ClientCredentials.SCOPES;
                long now = unix_timestamp();
                long exp = now + 3600;
                String jwt_header = "{\"alg\":\"RS256\",\"typ\":\"JWT\"}";
                String claim = "{\"iss\":\"" + client_id + "\",\"scope\":\"" + SCOPE +
                    "\",\"aud\":\"https://accounts.google.com/o/oauth2/token\",\"exp\":" +
                    exp + ",\"iat\":" + now + "}";
                System.Text.ASCIIEncoding e = new System.Text.ASCIIEncoding();
                String clearjwt = Base64UrlEncode(e.GetBytes(jwt_header)) + "." +
                    Base64UrlEncode(e.GetBytes(claim));
                byte[] buffer = Encoding.Default.GetBytes(clearjwt);
                X509Certificate2 cert = new X509Certificate2(key, "notasecret");
                CspParameters cp = new CspParameters(24, "Microsoft Enhanced RSA and AES Cryptographic Provider",
                    ((RSACryptoServiceProvider)cert.PrivateKey).CspKeyContainerInfo.KeyContainerName);
                RSACryptoServiceProvider provider = new RSACryptoServiceProvider(cp);
                byte[] signature;
                signature = provider.SignData(buffer, "SHA256");
                string assertion = clearjwt + "." + Base64UrlEncode(signature);
                WebClient client = new WebClient();
                NameValueCollection formData = new NameValueCollection();
                formData["grant_type"] = "assertion";
                formData["assertion_type"] = "http://oauth.net/grant_type/jwt/1.0/bearer";
                formData["assertion"] = assertion;
                client.Headers["Content-type"] = "application/x-www-form-urlencoded";
                try
                {
                    byte[] responseBytes = client.UploadValues("https://accounts.google.com/o/oauth2/token",
                        "POST", formData);
                    string Result = Encoding.UTF8.GetString(responseBytes);
                    string[] tokens = Result.Split(':');
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        if (tokens[i].Contains("access_token"))
                            lauth_token = (tokens[i + 1].Split(',')[0].Replace("\"", ""));
                    }
                    return lauth_token;
                }
                catch (WebException ex)
                {
                    Stream receiveStream = ex.Response.GetResponseStream();
                    Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
                    StreamReader readStream = new StreamReader(receiveStream, encode);
                    string pageContent = readStream.ReadToEnd();
                    return ex.Message + pageContent;
                }
            }
    
    private static long unix_timestamp()
        {
            TimeSpan unix_time = (System.DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)unix_time.TotalSeconds;
        }
    
    private static string Base64UrlEncode(byte[] input)
    {
        var output = Convert.ToBase64String(input);
        output = output.Split('=')[0]; // Remove any trailing '='s
        output = output.Replace('+', '-'); // 62nd char of encoding
        output = output.Replace('/', '_'); // 63rd char of encoding
        return output;
    }

    public static Userinfo GetUserInfo(IAuthenticator credentials)
    {

             Oauth2Service userInfoService = new Oauth2Service(new BaseClientService.Initializer() { Authenticator = credentials });
        Userinfo userInfo = null;
        string err = "";
        try
        {
            userInfo = userInfoService.Userinfo.Get().Fetch();

        }
        catch (GoogleApiRequestException e)
        {
            err = "An error occurred: " + e.Message;
            Console.WriteLine(err);
        }

        if (userInfo != null && !String.IsNullOrEmpty(userInfo.Id))
        {
            return userInfo;
        }
        else
        {
            throw new Exception(err);
        }
    }

    public static AnalyticsReport GetProfileReport(string id, IAuthenticator authenticator)
    {
        AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });

        var data =
                   service.Data.Ga.Get("ga:" + id, "2012-10-01", "2012-11-01", "ga:visits,ga:bounces,ga:pageviews");
        data.Dimensions = "ga:source,ga:keyword,ga:pagePath,ga:city,ga:date,ga:landingPagePath,ga:visitorType";
       // data.MaxResults = 500;

        GaData garesults = data.Fetch();
        AnalyticsReport report = new AnalyticsReport();
        report.id = id;
        report.JSON = garesults.Rows;

        var test = garesults.TotalsForAllResults;
        report.TotalVisits = Convert.ToInt32(test["ga:visits"]);
        report.TotalPageViews = Convert.ToInt32(test["ga:pageviews"]);
        var bounces = Convert.ToInt32(test["ga:bounces"]);
        var visits = Convert.ToInt32(test["ga:visits"]);
        report.BounceRate = (bounces / visits);

        
        //Referral List
        List<string> reflinks = new List<string>();
        List<string> keyWordList = new List<string>();
        List<string> landingPages = new List<string>();
        List<string> pagePathList = new List<string>();
        List<string> cityList = new List<string>();

        List<string> visitorType = new List<string>();
        List<int> dayList = new List<int>();
        List<int> newVisitors = new List<int>();
        List<int> returningVisitors = new List<int>();
        int maxReturningVisitors = 0;
        int maxNewVisitors = 0;
        foreach (var a in garesults.Rows)
        {
            string visType = a[6];
            var day = Convert.ToInt32(a[4]);
            if (dayList.Count() == 0)
            {
                dayList.Add(day);
            }
            else
            {
                var lastday = dayList.Last();
                if (day != lastday)
                {
                    dayList.Add(day);
                    filltoSameSize(newVisitors, returningVisitors); 
                }
            }
            int numVisits = Convert.ToInt32(a[7]);
            if(visType == "New Visitor"){
                newVisitors.Add(numVisits);
                report.TotalNewVisitors = (report.TotalNewVisitors + numVisits);
                maxNewVisitors = Math.Max(maxNewVisitors, numVisits);
            }
            else
            {
                returningVisitors.Add(numVisits);
                report.TotalReturningVisitors = (report.TotalReturningVisitors + numVisits);
                maxReturningVisitors = Math.Max(maxReturningVisitors, numVisits);
            }

            reflinks.Add(a[0]);
            keyWordList.Add(a[1]);
            pagePathList.Add(a[2]);
            cityList.Add(a[3]);
            dayList.Add(day);
            landingPages.Add(a[5]);
            visitorType.Add(a[6]);
        }
        filltoSameSize(newVisitors, returningVisitors);
        report.maxNewVisitors = maxNewVisitors;
        report.maxReturningVisitors = maxReturningVisitors;
        report.newVisitors = newVisitors;
        report.retVisitors = returningVisitors;
        report.keylist = keyWordList;
        report.reflist = reflinks;
        report.citylist = cityList;
        report.landingpagelist = pagePathList;
        report.daylist = dayList;

        //KeyWord Entrances
       //  report.newVisitors = (from pages in arrPage group pages by pages into p where p.Key != null && !p.Key.Contains("Error") orderby p.Count() descending select new { Key = p.Key, Count = p.Count() }).Take(7);
               
     
        return report;
    }

    public static AnalyticsReport GetReport(AnalyticsProfile profile, IAuthenticator authenticator)
    {
        AnalyticsService service = new AnalyticsService(new BaseClientService.Initializer() { Authenticator = authenticator });


        var data =
                   service.Data.Ga.Get("ga:" + profile.id, profile.startDate.ToString("yyyy-MM-dd"), profile.endDate.ToString("yyyy-MM-dd"), "ga:visits,ga:bounces,ga:pageviews");
        data.Dimensions = "ga:source,ga:keyword,ga:pagePath,ga:city,ga:date,ga:landingPagePath,ga:visitorType";
        // data.MaxResults = 500;
        if (data.Fetch() == null)
        {
            try
            {
                //reauth if auth fail
                IAuthenticator auth = Utils.getCredsnew(AnalyticsService.Scopes.AnalyticsReadonly.GetStringValue());
                 data =service.Data.Ga.Get("ga:" + profile.id, profile.startDate.ToString("yyyy-MM-dd"), profile.endDate.ToString("yyyy-MM-dd"), "ga:visits,ga:bounces,ga:pageviews");
                data.Dimensions = "ga:source,ga:keyword,ga:pagePath,ga:city,ga:date,ga:landingPagePath,ga:visitorType";
            }
            catch (Exception ex)
            {
            }

        }

        GaData garesults = data.Fetch();
        AnalyticsReport report = new AnalyticsReport();
        report.id = profile.id;
        report.JSON = garesults.Rows;

        var test = garesults.TotalsForAllResults;
        report.TotalVisits = Convert.ToInt32(test["ga:visits"]);
        report.TotalPageViews = Convert.ToInt32(test["ga:pageviews"]);
        var bounces = Convert.ToInt32(test["ga:bounces"]);
        var visits = Convert.ToInt32(test["ga:visits"]);
        report.BounceRate = ( visits / 2);

        //Referral List
        List<string> reflinks = new List<string>();
        List<string> keyWordList = new List<string>();
        List<string> landingPages = new List<string>();
        List<string> cityList = new List<string>();
        List<string> pagePathList = new List<string>();
        List<string> visitorType = new List<string>();
        List<int> dayList = new List<int>();
        List<int> totalVisitors = new List<int>();
        List<int> newVisitors = new List<int>();
        List<int> returningVisitors = new List<int>();
        int maxReturningVisitors = 0;
        int maxNewVisitors = 0;
        if (garesults.Rows != null){
        foreach (var a in garesults.Rows)
        {
            string visType = a[6];
            var day = Convert.ToInt32(a[4]);
            if (dayList.Count() == 0)
            {
                dayList.Add(day);
            }
            else
            {
                var lastday = dayList.Last();
                if (day != lastday)
                {
                    dayList.Add(day);
                    filltoSameSize(newVisitors, returningVisitors);
                }
            }
            int numVisits = Convert.ToInt32(a[7]);
            if (visType == "New Visitor")
            {
                newVisitors.Add(numVisits);
                report.TotalNewVisitors = (report.TotalNewVisitors + numVisits);
                maxNewVisitors = Math.Max(maxNewVisitors, numVisits);
                totalVisitors.Add(numVisits);
            }

            else
            {
                totalVisitors.Add(numVisits);
                returningVisitors.Add(numVisits);
                report.TotalReturningVisitors = (report.TotalReturningVisitors + numVisits);
                maxReturningVisitors = Math.Max(maxReturningVisitors, numVisits);
            }
            reflinks.Add(a[0]);
            keyWordList.Add(a[1]);
            pagePathList.Add(a[2]);
            cityList.Add(a[3]);
            dayList.Add(day);
            landingPages.Add(a[5]);
            visitorType.Add(a[6]);
        }
        filltoSameSize(newVisitors, returningVisitors);
        report.totalVisitors = totalVisitors;
        report.maxNewVisitors = maxNewVisitors;
        report.maxReturningVisitors = maxReturningVisitors;
        report.newVisitors = newVisitors;
        report.retVisitors = returningVisitors;
        report.landingpagelist = landingPages;
        report.keylist = keyWordList;
        report.reflist = reflinks;
        report.keylist = keyWordList;
        report.citylist = cityList;
        report.pagelist = pagePathList;
        report.daylist = dayList;

        //KeyWord Entrances
        //  report.newVisitors = (from pages in arrPage group pages by pages into p where p.Key != null && !p.Key.Contains("Error") orderby p.Count() descending select new { Key = p.Key, Count = p.Count() }).Take(7);


        return report;
            }
    else
        {
            return null;
        }
    }


    public static void filltoSameSize(List<int> firstArray, List<int> secondArray)
    {
     if (firstArray.Count() < secondArray.Count())
     {
         firstArray.Add(0);
     }
     else if (secondArray.Count() < firstArray.Count())
     {
         secondArray.Add(0);
     }
    }

    public void getVisitChart(AnalyticsReport report)
    {
        var entries = report.JSON;
        string[] returningVisitors;
        string[] newVisitors;
        string[] days;
        int maxReturn = 0;
        int maxNewVisitor = 0;

        foreach (var a in entries)
        {



        }
        //var retVisitors;


    }

    public static string DownloadFile(IAuthenticator auth, String downloadUrl)
    {
        string result = "";
        try
        {
            HttpWebRequest request = (HttpWebRequest) WebRequest.Create(new Uri(downloadUrl));
            auth.ApplyAuthenticationToRequest(request);

            HttpWebResponse response = (HttpWebResponse) request.GetResponse();
            System.IO.Stream stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
        catch (Exception e)
        {
            Console.WriteLine("An error occurred: " + e.Message);
        }
        return result;
    }}

    public class CustomAuthenticator : IAuthenticator
    {
        public String AccessToken { get; set; }

        public void ApplyAuthenticationToRequest(HttpWebRequest request)
        {
            request.Headers[HttpRequestHeader.Authorization] = "Bearer" + AccessToken;
        }

    }

    public static class Printer
    {
        public const string HtmlToPdfExePath = @"C:\Program Files (x86)\wkhtmltopdf\wkhtmltopdf.exe";
      
        public static String GeneratePDF(string html, string name)
        {
            MemoryStream mem = new MemoryStream();

            PdfConvert.ConvertHtmlToPdf(new PdfDocument { Url = "-",Html = html}, new PdfOutput
            {
               // OutputStream = mem
                OutputFilePath = HostingEnvironment.MapPath(@"\Reports\" +DateTime.Now.Month.ToString("mm") + name + ".pdf" )
            });
            String reportloc = @"\Reports\" + DateTime.Now.Month.ToString("mm") + name + ".pdf";

            return reportloc;

            //  ShowPdf("C:\\temp\\seoreportoutput.pdf");
        }

        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[32768];
            int read;
            while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, read);
            }
        }


        
    }

}