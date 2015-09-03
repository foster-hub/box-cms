using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using Box.CMS.Services;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net;
using System.Threading.Tasks;
using Box.Core.Services;

namespace Box.CMS.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CMS_ImageTransformController : ApiController {

        [Import]
        private CMSService cms { get; set; }

        [Import]
        private LogService log { get; set; }


        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public string Post([FromUri] string id, [FromBody] dynamic data, [FromUri] bool createCopy) {

            cms.VerifyAuthorizationToEditFiles();

            File file = cms.GetFile(id);
            if (file == null)
                return null;

            int x = data.x;
            int y = data.y;
            int width = data.w;
            int height = data.h;
            double scale = data.scale;

            byte[] bytes = cms.GetScaledImageFile(file.Data.StoredData, scale, x, y, width, height);
            
            if (createCopy) {
                file.FileUId = System.Guid.NewGuid().ToString();
                file.FileName = "x" + file.FileName;
            }
            
            file.Size = bytes.Length;
            file.Data = new FileData() { FileUId = file.FileUId, StoredData = bytes };

            

            cms.SetFileThumb(file);

            cms.SaveFile(file, FileStorages.Database);

            log.Log(String.Format(SharedStringsLog.IMAGE_0_CROP, file.FileUId));

            return file.FileUId;
        }

    }

}
