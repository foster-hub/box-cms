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
    public class CMS_FilesController : ApiController {

        [Import]
        private CMSService cms { get; set; }

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public IEnumerable<File> Get(string folder, string filter = null, int skip = 0, int top = 0, bool unUsed = false) {
            cms.VerifyAuthorizationToEditFiles();
            return cms.GetFiles(filter, skip, top, folder, unUsed);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public void Delete(string folder, string id, bool unUsed = false) {
            cms.VerifyAuthorizationToEditFiles();
            File file;
            var nameLog = "";
            if (unUsed) {
                cms.RemoveUnusedFiles();
                nameLog = "Limpeza de imagens não usadas executada com sucesso.";
            } else {
                file = cms.GetFile(id);
                if (folder.ToLower() != file.Folder.ToLower())
                    throw new System.Security.SecurityException("Could not delete file - wrong folder");
                nameLog = file.FileName;
                cms.RemoveFile(id);
            }

            log.Log(String.Format(SharedStringsLog.FILE_REMOVE_0_1, nameLog, folder));
        }

        [Box.Core.Web.WebApiAntiForgery]
        [HttpPost]
        [Authorize]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void Remove(string folder, string id, bool unUsed = false) {
            Delete(folder, id, unUsed);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize]
        public Task<File[]> Post(string folder, [FromUri] int storage = 0) {

            cms.VerifyAuthorizationToEditFiles();

            return SaveMimeMultipartContent(Request, folder, storage);

        }

        public Task<File[]> SaveMimeMultipartContent(HttpRequestMessage request, string folder, int storage) {

            log.Log(string.Format(SharedStringsLog.FILE_UPLOAD, folder), saveParameters: false);

            if (!request.Content.IsMimeMultipartContent())
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotAcceptable, "This request is not properly formatted"));

            var streamProvider = new MultipartMemoryStreamProvider();

            var task = request.Content.ReadAsMultipartAsync(streamProvider).ContinueWith(t => {

                if (t.IsFaulted || t.IsCanceled)
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);

                List<File> files = new List<File>();
                foreach (HttpContent content in streamProvider.Contents) {

                    byte[] bytes = content.ReadAsByteArrayAsync().Result;

                    File file = new File();
                    file.FileUId = Guid.NewGuid().ToString();
                    file.FileName = cms.CleanFileName(content.Headers.ContentDisposition.FileName);
                    file.Type = content.Headers.ContentType.MediaType;
                    file.Folder = (folder == null ? "Images" : folder);
                    file.Size = bytes.Length;
                    file.Data = new FileData() { FileUId = file.FileUId, StoredData = bytes };

                    cms.SetFileThumb(file);

                    cms.SaveFile(file, (FileStorages)storage);

                    file.Data = null;

                    files.Add(file);

                }

                return files.ToArray();
            });

            return task;
        }




    }
}
