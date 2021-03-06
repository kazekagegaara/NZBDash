﻿using System.Web.Optimization;

namespace NZBDash.UI
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap-notify.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js",
                        "~/Scripts/bootstrap-switch.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/gridster").Include("~/Scripts/jquery.gridster.js"));

            bundles.Add(new StyleBundle("~/Content/gridster").Include("~/Content/jquery.gridster.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/Content/font-awesome.css",
                      "~/Content/bootstrap-switch.css"));

            var ajaxGrid = new ScriptBundle("~/bundles/gridmvc").Include(
                "~/Scripts/URI.js",
                "~/Scripts/gridmvc.js",
                "~/Scripts/gridmvc-ext.js",
                "~/Scripts/ladda-bootstrap/ladda.min.js",
                "~/Scripts/ladda-bootstrap/spin.min.js");
            bundles.Add(ajaxGrid);

        }
    }
}
