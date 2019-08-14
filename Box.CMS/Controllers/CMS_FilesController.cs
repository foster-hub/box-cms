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
    public class CMS_FilesController : CMS_FilesReadOnlyController {

    
        [Import]
        private IPageModel PageModel { get; set; }

    
        public ActionResult Upload() {
            cms.VerifyAuthorizationToEditFiles();
            return View(PageModel);
        }
        
        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file) {

            cms.VerifyAuthorizationToEditFiles();

            string folder = Request.QueryString["upFolder"];
            int storage = 0;

            if (file != null && file.ContentLength > 0) {
                File uploaded = SaveFile(file, folder, storage);
                ViewData["uploadedFile"] = uploaded.FileName;
            }

       
            return View(PageModel);
        }

        private File SaveFile(HttpPostedFileBase fileRequest, string folder, int storage) {
            byte[] bytes = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream()) {
                fileRequest.InputStream.CopyTo(ms);
                bytes = ms.GetBuffer();
            }

            File file = new File();
            file.FileUId = Guid.NewGuid().ToString();
            file.FileName = cms.CleanFileName(fileRequest.FileName);
            file.Type = fileRequest.ContentType;
            file.Folder = (folder == null ? "Images" : folder);
            file.Size = fileRequest.ContentLength;
            file.Data = new FileData() { FileUId = file.FileUId, StoredData = bytes };

            cms.SetFileThumb(file);

            cms.SaveFile(file);

            return file;

        }

    
    }
}
