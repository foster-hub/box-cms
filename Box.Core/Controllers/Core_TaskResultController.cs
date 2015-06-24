using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.Composition;
using Box.Composition;
using Box.Composition.Web;
using System.Text;

namespace Box.Core.Controllers {

    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class Core_TaskResultController : Controller {

        public FileResult Index(string id) {

            Services.AsyncTaskService taskService = new Services.AsyncTaskService();
            AsyncTask task = taskService.GetTask(id);
            if (task != null && User.IsInRole(task.RequiredRole)) {
                string filename = task.FileName + task.ResultContentType;
                return File(task.Result, task.ResultMimeType, filename);
            }
            throw new System.Security.SecurityException();
        }
    }
}
