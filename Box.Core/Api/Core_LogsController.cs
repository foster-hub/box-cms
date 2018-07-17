using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Web.Http;
using Box.Core.Services;

namespace Box.Core.Api {
    
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public partial class Core_LogsController : ApiController {

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "LOG_VIEWER")]
        public Log Get(string id) {
            return log.GetLog(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "LOG_VIEWER")]
        [PaginationFilter]
        public IEnumerable<Log> Get(string filter = null, int skip = 0, int top = 0, DateTime? dataDe = null, DateTime? dataAte = null)
        {
            int totalRecords = 0;
            IEnumerable<Log> return_ = log.GetLogs(ref totalRecords, filter, skip, top, dataDe, dataAte);
            Request.Properties["count"] = totalRecords.ToString();
            return return_;
        }
    }
}
