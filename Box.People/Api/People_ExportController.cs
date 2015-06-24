using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Box.Composition;
using System.ComponentModel.Composition;
using System.IO;
using System.IO.Compression;
using Box.Core.Services;

namespace Box.People.Api {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class People_ExportController : ApiController {

        private const string TASK_TYPE = "EXPORT_PEOPLE";
        private const string CONTENT_TYPE = ".csv.gz";
        private const string MIME_TYPE = "application/zip";
        
        [Import]
        private Services.PeopleService peopleService { get; set; }

        [Import]
        private LogService log { get; set; }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE")]
        public string Post(string filter, string description, string optin = "All", string group = "All") {

            Core.Services.AsyncTaskService taskService = new Core.Services.AsyncTaskService();

            string taskUId = taskService.StartTask(TASK_TYPE, "ADM_PEOPLE", "Initializing...", description, "People", CONTENT_TYPE, MIME_TYPE, id => {
                peopleService.Export(taskService, id, filter, optin, group);
            });

            return taskUId;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE")]
        public string Delete(string taskUId) {

            Core.Services.AsyncTaskService taskService = new Core.Services.AsyncTaskService();
            taskService.RemoveTask(taskUId);
            
            return taskUId;
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE"), HttpPost]
        /* Some enviroments does not supports HTTP VERB DELETE 
         * Use this workaround */
        public void REMOVE(string id) {
            Delete(id);
        }

        [Box.Core.Web.WebApiAntiForgery]
        [Authorize(Roles = "ADM_PEOPLE")]
        public IEnumerable<Box.Core.AsyncTask> Get() {
            Core.Services.AsyncTaskService taskService = new Core.Services.AsyncTaskService();
            return taskService.GetTasksWithoutResult(TASK_TYPE);
        }
    }
}
