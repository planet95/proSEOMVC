using System.Web;
using System.Web.Optimization;

namespace proSEOMVC
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.flot.js",
                 "~/Scripts/jquery.flot*",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/ajaxlogin").Include(
                "~/Scripts/app/ajaxlogin.js"));
        
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/bootstrap*",
                "~/Scripts/modernizr-*",
                "~/Scripts/app/bindings.js",
                "~/Scripts/app/viewmodel.js",
                "~/Scripts/app/datacontext.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
               "~/Content/css/bootstrap.css", "~/Content/css/bootstrap-responsive.css",   "~/Content/css/Site.css"));
        }
    }
}