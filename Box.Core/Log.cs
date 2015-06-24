using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Box.Core {
   
    public enum LogTypes {
        ACTION,
        ERROR
    }

    public class Log {

        [Key, Column(TypeName = "char"), MaxLength(36)]
        public string LogUId { get; set; }

        public DateTime When { get; set; }

        [MaxLength(255)]
        public string SignedUser { get; set; }

        public string ActionDescription { get; set; }

        public string Url { get; set; }

        [MaxLength(20)]
        public string UserIp { get; set; }

        public string ErrorDescription { get; set; }

        public string Parameters { get; set; }

        public short LogType { get; set; }
    }
}
