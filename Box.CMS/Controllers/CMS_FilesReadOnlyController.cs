using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;


namespace Box.CMS.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_FilesReadOnlyController : Controller {

        [Import]
        protected CMS.Services.CMSService cms { get; set; }

        [OutputCache(VaryByParam = "*", Duration = 2678400, Location=System.Web.UI.OutputCacheLocation.ServerAndClient)]       
        public FileResult Index(string folder, string id, bool? asThumb, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, double scale = 1.0, string vAlign = "center", string hAlign = "center", string mode = null) {

            if (folder=="box" && id == "loading")
                return GetLoadingImage();

            File file = null;

            if (asThumb == true)
            {
                file = cms.GetFileThumb(id);
                if (file.Data == null || file.Data.StoredThumbData == null)
                {
                    file = cms.GetFile(id);
                    asThumb = false;
                }
            }
            else
            {
                file = cms.GetFile(id);
            }

            if (file == null)
                return null;

            if (file.Folder.ToLower() != folder.ToLower())
                throw new System.Security.SecurityException("File folder does not match");

            if (asThumb.HasValue && asThumb.Value)
                return new FileContentResult(file.Data.StoredThumbData, file.Type) { FileDownloadName = file.FileName };

            byte[] returnImg = null;

            if (width == 0 && height == 0)
            {
                returnImg = cms.GetScaledImageFile(file.Data.StoredData, scale, mimeType: file.Type);                
            }
            else
            {
                returnImg = cms.GetImageFileThumb(file.Data.StoredData, width, height, maxWidth, maxHeight, vAlign, hAlign, file.Type, mode);
            }

            if (returnImg == null)
            {
                return null;
            }
            return new FileContentResult(returnImg, file.Type) { FileDownloadName = file.FileName };
        }

        
        /// <summary>
        /// Returns the file with some especial cache headers that allow this one to be cached by service workers at PWA apps.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="id"></param>
        /// <param name="asThumb"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="maxWidth"></param>
        /// <param name="maxHeight"></param>
        /// <param name="scale"></param>
        /// <param name="vAlign"></param>
        /// <param name="hAlign"></param>
        /// <param name="mode"></param>
        /// <param name="donwload"></param>
        /// <returns></returns>
        public FileResult Cache(string folder, string id, bool? asThumb, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, double scale = 1.0, string vAlign = "center", string hAlign = "center", string mode = null, bool donwload = false)
        {

            ControllerContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.Public);
            ControllerContext.HttpContext.Response.Cache.SetExpires(DateTime.Today.AddDays(120));
            ControllerContext.HttpContext.Response.Cache.SetLastModified(DateTime.Today);
            ControllerContext.HttpContext.Response.Cache.SetETag(folder + id);            
            ControllerContext.HttpContext.Response.AddHeader("Accept-Ranges", "bytes");            

            if (folder == "box" && id == "loading")
                return GetLoadingImage();

            File file = null;

            if (asThumb == true)
                file = cms.GetFileThumb(id);
            else
                file = cms.GetFile(id);

            if (file == null)
                return null;

            if (file.Folder.ToLower() != folder.ToLower())
                throw new System.Security.SecurityException("File folder does not match");

            if (asThumb.HasValue && asThumb.Value)
            {
                return new FileContentResult(file.Data.StoredThumbData, file.Type);                
            }

            if (width == 0 && height == 0)
                return new FileContentResult(cms.GetScaledImageFile(file.Data.StoredData, scale, mimeType: file.Type), file.Type);


            var result = new FileContentResult(cms.GetImageFileThumb(file.Data.StoredData, width, height, maxWidth, maxHeight, vAlign, hAlign, file.Type, mode), file.Type);
            if(donwload)
                result.FileDownloadName = file.FileName;
            return result;
        }


        protected FileResult GetLoadingImage() {            
            string path = Server.MapPath(Url.Content("~/Content/CMS_Images/FileIcons/loading.gif"));
            return new FilePathResult(path, "image/gif");
        }
     

    }
}
