using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.Core {

    public class AsyncTask {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string AsyncTaskUId { get; set; }

        [MaxLength(15)]    
        public string Type { get; set; }

        [MaxLength(50)]    
        public string CurrentAtivity { get; set; }

        public short CurrentPercentage { get; set; }

        [MaxLength(20)]
        public string RequiredRole { get; set; }

        [MaxLength(20)]
        public string FileName { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [MaxLength(50)]    
        public string ResultContentType { get; set; }

        [MaxLength(25)]
        public string ResultMimeType { get; set; }

        public byte[] Result { get; set; }

        public long? FileSize { get; set; }
    }
}
