function FileDatabaseVM() {
    CrudVM.call(this, 'cms', 'files', 'FileUId');

    this.onSelect = null;
    this.folder = new ko.observable('ROOT');

    var me = this;

    this._getData = function (skip) {

        $.ajax({
            url: _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + me.folder() + '/?filter=' + encodeURIComponent(me.searchFilter()) + '&skip=' + skip + '&top=' + me.paging.itemsPerPage,
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

    this._deleteData = function (id) {

        var verb = 'DELETE';
        var url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + me.folder() + '/' + id;

        if (!CrudOptions.AllowHttpDELETE) {
            verb = 'POST';
            url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/REMOVE/' + me.folder() + '/' + id;
        }

        $.ajax({
            url: url,
            type: verb,
            success: function () {
                me[me._resourceName].remove(me.removingItem());
                me.removingItem(null);
            }
        });
    }


    this.setItemsPerPage = function (width, height) {

        var itemsPerLine = Math.floor(width / 130);
        var linesPerPage = Math.floor(height / 130);
        var itensPerPage = itemsPerLine * linesPerPage;

        me.paging.itemsPerPage = itensPerPage;
        if (me.paging.itemsPerPage < 0)
            me.paging.itemsPerPage = 8;
    }

    
}

FileDatabaseVM.prototype = new CrudVM();

var filesVM = new FileDatabaseVM();

