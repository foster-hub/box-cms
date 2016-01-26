function FileDatabaseVM() {
    CrudVM.call(this, 'cms', 'files', 'FileUId');

    this.onSelect = null;
    this.folder = new ko.observable('ROOT');
    this.unUsed = new ko.observable(false);
    this.removingUnUsedItem = new ko.observable(false);
    this.notRemovingUnUsedItem = new ko.observable(false);
    this.selection = null;

    var me = this;

    this._getData = function (skip) {

        $.ajax({
            url: _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + me.folder() + '/?filter=' + encodeURIComponent(me.searchFilter()) + '&skip=' + skip + '&top=' + me.paging.itemsPerPage + '&unUsed=' + me.unUsed(),
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {

                me[me._resourceName](data);

                // set paging
                me.paging.applySkip();
                me.hasPreviousPage(skip > 0);
                me.hasNextPage(!(data.length < me.paging.itemsPerPage));

            }
        });
    }

    this._deleteData = function (id, unUsed) {
        if (!unUsed)
            unUsed = false;

        var verb = 'DELETE';
        var url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + me.folder() + '/' + id;

        if (!CrudOptions.AllowHttpDELETE) {
            verb = 'POST';
            url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/REMOVE/' + me.folder() + '/' + id;
        }

        $.ajax({
            url: url + '?unused=' + unUsed,
            type: verb,
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function () {

                if (unUsed)
                    filesVM._getData(0);
                else
                    me[me._resourceName].remove(me.removingItem());

                me.removingItem(null);
            }
        });
    }


    this.setItemsPerPage = function (width, height) {

        var itemsPerLine = Math.floor(width / 130);
        var linesPerPage = Math.floor(height / 150);
        var itensPerPage = itemsPerLine * linesPerPage;

        me.paging.itemsPerPage = itensPerPage;
        if (me.paging.itemsPerPage < 0)
            me.paging.itemsPerPage = 8;
    }

    this.uploadFiles = function (files) {

        if (files == null || files.length <= 0)
            return;

        if (window.FormData == null)
            return;

        var data = new FormData();
        var waitFiles = new Array();

        var len = files.length;

        for (i = 0; i < len; i++) {
            data.append("file" + i, files[i]);
        }

        $.ajax({
            type: 'POST',
            url: _webAppUrl + 'api/cms_files/' + me.folder() + '?' + '&storage=0',
            contentType: false,
            processData: false,
            data: data,
            success: function (resFiles) {

                for (var r = 0; r < resFiles.length; r++) {
                    var res = resFiles[r];
                    var newFile = { FileUId: res.FileUId, FileName: res.FileName, Size: res.Size, Folder: me.folder(), Type: res.Type };
                    me.files.unshift(newFile);
                    if (me.files().length > me.paging.itemsPerPage)
                        me.files.pop();
                }

                if (me.afterUpload)
                    me.afterUpload();
            }
        });
    }

    this.saveSelection = function (w) {
        if (w == null)
            return;
        me.selection = null;

        if (w.getSelection) {
            sel = w.getSelection();
            if (sel.getRangeAt && sel.rangeCount) {
                me.selection = sel.getRangeAt(0);
            }
        } else if (document.selection && document.selection.createRange) {
            me.selection = document.selection.createRange();
        }

    }

    this.restoreSelection = function (w) {
        if (!me.selection || w == null)
            return;

        if (w.getSelection) {
            sel = w.getSelection();
            sel.removeAllRanges();
            sel.addRange(me.selection);
        } else if (document.selection && me.selection.select) {
            me.selection.select();
        }
    }

}

FileDatabaseVM.prototype = new CrudVM();

var filesVM = new FileDatabaseVM();

