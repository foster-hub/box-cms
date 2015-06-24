function ExportVM() {
    CrudVM.call(this, 'people', 'export', 'AsyncTaskUId');
    var me = this;
    this.AsyncStatus = new ko.observable();   // 0 = NO TASKS, 1 = TASK RUNNING, 2 = TASK OVER, 3 = OTHER USER
    this.CurrentPercentage = new ko.observable();
    this.CurrentAtivity = new ko.observable();
    this.AsyncTaskUId = new ko.observable();
    this.Description = new ko.observable();
    this.StartDate = new ko.observable();
    this.EndDate = new ko.observable();
    this.FileSize = new ko.observable();
    this.FileName = new ko.observable();
    this.ResultContentType = new ko.observable();
    this.CurrentExportDescription = new ko.observable();

    this.cancelTask = function () {
        var currentTask = me.export()[0];
        pageVM.setRemovingItem(currentTask);
        pageVM.removeItem();
        me.AsyncStatus(0);
    }

    this.startTask = function () {
        this.customPostParameters = '?filter=' + ExportVM.filter + "&description=" + me.CurrentExportDescription() + '&optin=' + ExportVM.optin + '&group=' + ExportVM.group;
        pageVM.applyItemChanges();
    }

    this.afterPost = function (taskUId) {
        me.AsyncTaskUId(taskUId);
        me.loadData();
    }

    this.afterGet = function (data) {

        me.setCurrentExportDescription();

        // ther is not task running or finished
        if (data.length == 0) {
            me.AsyncStatus(0);
            return;
        }

        var task = data[0];

        // the current task was canceled by another user.
        if (me.AsyncTaskUId() != null && me.AsyncTaskUId() != task.AsyncTaskUId) {
            me.AsyncStatus(3);
            return;
        }

        me.CurrentPercentage(task.CurrentPercentage);
        me.CurrentAtivity(task.CurrentAtivity);
        me.Description(task.Description);
        me.StartDate(task.StartDate);
        me.EndDate(task.EndDate);

        // the task is over
        if (task.EndDate != null) {
            me.AsyncStatus(2);
            me.AsyncTaskUId(task.AsyncTaskUId);
            me.FileSize(task.FileSize);
            me.FileName(task.FileName);
            me.ResultContentType(task.ResultContentType);
            return;
        }

        me.AsyncStatus(1);

        setTimeout(function () { me.loadData() }, 1000);
    }


    this.bytesToSize = function (bytes) {
        var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
        if (bytes == 0)
            return 'n/a';

        var i = parseInt(Math.floor(Math.log(bytes) / Math.log(1024)));
        if (i == 0)
            return bytes + ' ' + sizes[i];

        return (bytes / Math.pow(1024, i)).toFixed(1) + ' ' + sizes[i];
    }

    this.setCurrentExportDescription = function () {
        var exportDescription = "All users";

        if (ExportVM.optin != "All")
            exportDescription += " accepting " + ExportVM.optin + " contact";

        if (ExportVM.group != "All")
            exportDescription += " from " + ExportVM.group + " group";

        if (ExportVM.filter != "") {
            if (ExportVM.group != "All" || ExportVM.optin != "All")
                exportDescription += " and";

            exportDescription += " filtered by " + ExportVM.filter;
        }

        me.CurrentExportDescription(exportDescription);
    }
}

ExportVM.prototype = new CrudVM();

var pageVM = new ExportVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);
    pageVM.init();

});