using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.CMS {

    public class File {

        [Key, Column(TypeName = "char"), MaxLength(36)]   // this is action the FOLDER + GUID
        public string FileUId { get; set; }

        [MaxLength(255)]
        public string FileName { get; set; }

        public int Size { get; set; }

        [MaxLength(100)]
        public string Type { get; set; }

        [MaxLength(50)]
        public string Folder { get; set; }

        public FileData Data { get; set; }

    }

    public class FileData {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string FileUId { get; set; }

        public byte[] StoredData { get; set; }

        public byte[] StoredThumbData { get; set; }

    }
}
