function CrossLinksVM(area) {

    
    CrudVM.call(this, 'cms', 'contents', 'ContentUId');
    var me = this;

    me.area = area;

    this.getCustomFilter = function () {
        return '&area=' + me.area;
    }

    this.editContent = function (content) {        
        var w = window.open(_webAppUrl + 'cms_contents/?Kind=' + content.Kind + '#ContentUId=' + content.ContentUId);
    }

    this.browseContent = function (content) {

        if (content.ExternalLinkUrl != null) {
            var w = window.open(content.ExternalLinkUrl, content.ContentUId);
            return;
        }

        var w = window.open(_webAppUrl + 'cms_contents/Preview/' + content.ContentUId, content.ContentUId);
    }

    this.unLink = function (link) {
        me.removingItem(link);
        me.removeItem();
    }

    this._deleteData = function (id) {
        var verb = 'DELETE';
        var url = _webAppUrl + 'api/cms_crosslinks/' + id + '?area=' + me.area;

        if (!CrudOptions.AllowHttpDELETE) {
            verb = 'POST';
            url = _webAppUrl + 'api/cms_crosslinks/REMOVE/' + id + '?area=' + me.area;
        }

        $.ajax({
            url: url,
            type: verb,
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function () {
                me[me._resourceName].remove(me.removingItem());
                me.removingItem(null);
            }
        });

    }

    this.addContent = function (content) {
        me._putData(content.ContentUId,
            function () { me['contents'].splice(0, 0, content); });
    }

    this._putData = function (id, onSuccess) {

        var verb = 'PUT';
        var url = _webAppUrl + 'api/cms_crosslinks/' + id;

        if (!CrudOptions.AllowHttpPUT) {
            verb = 'POST';
            url = _webAppUrl + 'api/cms_crosslinks/UPDATE/' + id;
        }

        $.ajax({
            url: url,
            type: verb,
            data: JSON.stringify(me.area),
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function(data) {
                if (data == true) { onSuccess(); }
            },
            error: function (request) {
                if (request.status == 409) {
                    dialogHelper.setOperationMessage(me.errorMsgItemAlreadyExists);
                    return;
                }
                dialogHelper.setOperationMessage('Unknow error');
            }
        });
    }

}

CrossLinksVM.prototype = new CrudVM();
