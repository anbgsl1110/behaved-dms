using System.Web;
using System.Web.Optimization;

namespace OMTB.Dms.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/app/lib/jquery-{version}.js",
                        "~/Scripts/app/lib/jquery.dataTables.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/app/lib/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/app/lib/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/app/lib/bootstrap.js",
                      "~/Scripts/app/lib/respond.js"));
                  
            bundles.Add(new ScriptBundle("~/bundles/common").Include(
                      "~/Scripts/app/commonFilter.js",
                      "~/Scripts/app/errorMessage.js"));
                    
            bundles.Add(new ScriptBundle("~/bundles/login").Include(
                      "~/Scripts/app/modules/login/login.js"));
                    
            bundles.Add(new ScriptBundle("~/bundles/sqlOperation").Include(
                      "~/Scripts/app/modules/sqlOperation/index.js"));    

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/css/bootstrap.css",
                      "~/Content/css/site.css",
                      "~/Content/css/jquery.dataTables.css"));
        }
    }
}
