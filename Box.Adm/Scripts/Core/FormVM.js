function FormVM() {

    var me = this;

    me.data = new ko.observable(new Object());
    me.onSuccess = null;
    me.url = '';
    me.failedMessage = 'request error';

    me.dataField = null;

    me.post = function (query) {
        _ajax(query, 'POST');
    }

    me.put = function (query) {
        _ajax(query, 'PUT');
    }

    _ajax = function (query, method) {
    
        var postUrl = me.url + (query==null?'':query);
        var data = me.data();
        if (me.dataField != null)
            data = data[me.dataField];

        $.ajax({
            url: postUrl,
            type: method,
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            data: JSON.stringify(data),
            success: function (result) {
                if (me.onSuccess != null)
                    me.onSuccess(result);
            },
            error: function (request) {
                if (request.status == 409) {
                    dialogHelper.setOperationMessage(me.failedMessage);
                    return;
                }
            }
        });
    }

}

var pageVM = new FormVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);    
});
