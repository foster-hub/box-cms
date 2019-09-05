using System.Web;
using System.Web.Optimization;

namespace Box.Adm {
    public class BundleConfig {

        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles) {

            
            bundles.Add(new ScriptBundle("~/bundles/box")
                .Include("~/Scripts/knockout-3.4.2.js")
                .Include("~/Scripts/knockout.Extensions.js")                
                .Include("~/Scripts/Core/CrudVM.js")                
                .Include("~/Scripts/Utils/AjaxSetup.js")
                .Include("~/Scripts/Utils/Util.js")
                .Include("~/Scripts/Utils/DialogHelper.js"));

            bundles.Add(new ScriptBundle("~/bundles/boxCMS")                                
                .Include("~/Scripts/CMS/ContentCaptureVM.js")
                .Include("~/Scripts/CMS/FileDatabaseVM.js")
                .Include("~/Scripts/CMS/UploadArea.js")
                .Include("~/Scripts/guillotine/jquery.guillotine.js")
                .Include("~/Scripts/nicEdit/nicEdit.js")
                .Include("~/Scripts/nicEdit/nicEdit.ImageBoxExtension.js")
                .Include("~/Scripts/nicEdit/nicEdit.MediaBoxExtension.js")
                .Include("~/Scripts/nicEdit/nicEdit.CleanFormatExtension.js")
                .Include("~/Scripts/nicEdit/nicEdit.GalleryBoxExtension.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery")                
                .Include("~/Scripts/jquery-{version}.js")
                .Include("~/Scripts/jquery.validate.js")                
                .Include("~/Scripts/jquery-ui.js")                    
                .Include("~/Scripts/jquery.parsejson.extension.js"));

            bundles.Add(new ScriptBundle("~/bundles/metro")
                .Include("~/Scripts/metro/metro-global.js")
                .Include("~/Scripts/metro/*.js"));


            bundles.Add(new StyleBundle("~/Content/css")
                .Include("~/content/metro-bootstrap.css")
                .Include("~/content/metro-bootstrap-responsive.css")
                .Include("~/content/iconFont.css")
                .Include("~/content/font-awesome.css")
                .Include("~/Content/adm.css")
                .Include("~/Content/jquery-ui.min.css")
                .Include("~/Content/jquery.guillotine.css")
                .Include("~/Content/custom.css"));

            
            
        }
    }
}