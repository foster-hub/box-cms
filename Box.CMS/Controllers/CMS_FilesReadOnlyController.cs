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

            File file = cms.GetFile(id);
            if (file == null)
                return null;

            if (file.Folder.ToLower() != folder.ToLower())
                throw new System.Security.SecurityException("File folder does not match");

            if (asThumb.HasValue && asThumb.Value)
                return new FileContentResult(file.Data.StoredThumbData, file.Type);
                        
            if (width == 0 && height == 0)
                return new FileContentResult(cms.GetScaledImageFile(file.Data.StoredData, scale, mimeType: file.Type), file.Type);

            return new FileContentResult(cms.GetImageFileThumb(file.Data.StoredData, width, height, maxWidth, maxHeight, vAlign, hAlign, file.Type, mode), file.Type);
        }


        protected FileResult GetLoadingImage() {            
            string path = Server.MapPath(Url.Content("~/Content/CMS_Images/FileIcons/loading.gif"));
            return new FilePathResult(path, "image/gif");
        }
     

    }
}
