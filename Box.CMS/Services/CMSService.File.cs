using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System.Configuration;

namespace Box.CMS.Services {

    public partial class CMSService {


        public void VerifyAuthorizationToEditFiles() {

            // if CMS ADM
            if (System.Threading.Thread.CurrentPrincipal.IsInRole(ADM_CMS_GROUP.UserGroupUId))
                return;

            // if CMS FILE ADM
            if (System.Threading.Thread.CurrentPrincipal.IsInRole(ADM_CMSFILE_GROUP.UserGroupUId))
                return;

            throw new System.Security.SecurityException("Not autorized to edit content");
        }

        public IEnumerable<File> GetFiles(string filter, int skip, int top, string folder) {
            using (var context = new Data.CMSContext()) {
                IQueryable<File> files = context.Files;

                if (!String.IsNullOrEmpty(filter)) {
                    filter = filter.ToLower();
                    files = files.Where(f => f.FileName.ToLower().Contains(filter));
                }

                if (folder != null)
                    files = files.Where(f => f.Folder == folder);

                files = files.OrderBy(f => f.FileName);

                if (skip != 0)
                    files = files.Skip(skip);

                if (top != 0)
                    files = files.Take(top);

                return files.ToArray();

            }
        }

        public File GetFile(string fileUId, bool includeData = true) {
            using (var context = new Data.CMSContext()) {
                IQueryable<File> file = context.Files;
                
                if(includeData)
                    file = context.Files.Include("Data");
                
                return file.SingleOrDefault(f => f.FileUId == fileUId);
            }
        }

        public void SaveFile(File file, FileStorages storage) {
            using (var context = new Data.CMSContext()) {
                var oldfile = context.Files.SingleOrDefault(f => f.FileUId == file.FileUId);
                if (oldfile == null) {
                    context.Files.Add(file);
                } else {
                    context.Files.Remove(oldfile);
                    context.Files.Add(file);
                }
                context.SaveChanges();
            }
        }

        public byte[] GetScaledImageFile(byte[] bytes, double scale = 1, int xdes = 0, int ydes = 0, int finalW = 0, int finalH = 0) {

            if (scale == 1 && xdes==0 && ydes==0)
                return bytes;

            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);

            int height = (int)(image.Height * scale);
            int width = (int)(image.Width * scale);

            if (finalW == 0)
                finalW = width;

            if (finalH == 0)
                finalH = height;
        
            System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(finalW, finalH);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImg);
            g.DrawImage(image, new System.Drawing.Rectangle(-xdes, -ydes, width, height));

            g.Dispose();

            System.Drawing.ImageConverter conv = new System.Drawing.ImageConverter();
            return conv.ConvertTo(newImg, typeof(byte[])) as byte[];
        }

        public byte[] GetImageFileThumb(byte[] bytes, int width, int height, int maxWidth, int maxHeight, string vAlign = "center", string hAlign = "center") {
            System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
            System.Drawing.Image image = System.Drawing.Image.FromStream(stream);
            if (height == 0) {
                float rt = width / (float)image.Width;
                height = (int)(image.Height * rt);
            }
            if (width == 0) {
                float rt = height / (float)image.Height;
                width = (int)(image.Width * rt);
            }

            if (maxWidth == 0)
                maxWidth = width;

            if (maxHeight == 0)
                maxHeight = height;

            if (height < maxHeight)
                maxHeight = height;

            if (width < maxWidth)
                maxWidth = width;

            int top = 0;
            if (vAlign == "bottom")
                top = (maxHeight - height);
            if (vAlign == "center")
                top = (maxHeight - height) / 2;

            int left = 0;
            if (hAlign == "right")
                left = (maxWidth - width);
            if (hAlign == "center")
                left = (maxWidth - width) / 2;

            System.Drawing.Bitmap newImg = new System.Drawing.Bitmap(maxWidth, maxHeight);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(newImg);
            g.DrawImage(image, new System.Drawing.Rectangle(left, top, width, height));



            g.Dispose();

            System.Drawing.ImageConverter conv = new System.Drawing.ImageConverter();
            return conv.ConvertTo(newImg, typeof(byte[])) as byte[];
        }

        public void RemoveFile(string fileUId) {
            using (var context = new Data.CMSContext()) {
                File file = context.Files.SingleOrDefault(f => f.FileUId == fileUId);
                if (file == null)
                    return;
                context.Files.Remove(file);
                context.SaveChanges();
            }
        }

    

        public void SetFileThumb(File file) {
            if (file.Type.StartsWith("image"))
                file.Data.StoredThumbData = GetImageFileThumb(file.Data.StoredData, CMSThumbWidth, CMSThumbHeight, 0, 0);
            else {
                string path = System.Web.Hosting.HostingEnvironment.MapPath("~");
                file.Data.StoredThumbData = GetDocumentThumb(path, file.FileName);
            }
        }



        public byte[] GetDocumentThumb(string path, string fileName) {
            string iconFile = "document";

            int dotIndex = fileName.LastIndexOf(".");
            if (dotIndex >= 0) {
                string ext = fileName.Substring(dotIndex);
                switch (ext) {
                    case ".xls":
                    case ".xlsx":
                    case ".csv":
                        iconFile = "xls";
                    break;
                    case ".doc": case ".docx":
                        iconFile = "doc";
                    break;
                    case ".ppt": case ".pptx":
                        iconFile = "doc";
                    break;
                    case ".mp3":
                        iconFile = "mp3";
                    break;
                    case ".pdf":
                        iconFile = "pdf";
                    break;
                }
            }

            return System.IO.File.ReadAllBytes(path + "\\Content\\CMS_Images\\FileIcons\\" + iconFile + ".png");
        }


        public string CleanFileName(string name) {
            string cleanName = name.Replace("\"", string.Empty);
            int idxSlash = cleanName.LastIndexOf("\\");
            if (idxSlash > 0)
                cleanName = cleanName.Substring(idxSlash + 1);
            return cleanName;
        }

        private int CMSThumbWidth {
            get {
                object i = ConfigurationManager.AppSettings["CMS_THUMB_WIDTH"];
                if (i == null)
                    return 150;

                int value = 150;
                Int32.TryParse(i.ToString(), out value);

                return value;
            }
        }

        private int CMSThumbHeight {
            get {
                object i = ConfigurationManager.AppSettings["CMS_THUMB_HEIGHT"];
                if (i == null)
                    return 0;

                int value = 0;
                Int32.TryParse(i.ToString(), out value);

                return value;
            }
        }

    }

}
