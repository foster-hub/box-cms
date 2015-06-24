
$(function () {
    $.ajaxSetup({
        contentType: 'application/json; charset=utf-8',
        error: function (jqXHR, exception) {
            if (jqXHR.status === 0) {
                dialogHelper.setOperationMessage('Not connect or POST too large. Verify Network.');
            } else if (jqXHR.status == 404) {
                dialogHelper.setOperationMessage('Requested page not found. [404]');
            } else if (jqXHR.status == 500) {
                dialogHelper.setOperationMessage('Internal Server Error [500].');
            } else if (exception === 'parsererror') {
                dialogHelper.setOperationMessage('Requested JSON parse failed.');
            } else if (exception === 'timeout') {
                dialogHelper.setOperationMessage('Time out error.');
            } else if (exception === 'abort') {
                dialogHelper.setOperationMessage('Ajax request aborted.');
            } else {
                dialogHelper.setOperationMessage('Uncaught Error.\n' + jqXHR.responseText);
            }
        },
        beforeSend: function (jqXHR) {
            dialogHelper.setOperationMessage('');
            dialogHelper.setLoadingData(true);
            $('input[type=button]').attr('disabled', true);
        },
        complete: function (jqXHR, exception) {
            dialogHelper.setLoadingData(false);
            $('input[type=button]').attr('disabled', false);
        }
    });
});

window.Object.defineProperty(Element.prototype, 'documentOffsetTop', {
    get: function () {
        return this.offsetTop + (this.offsetParent ? this.offsetParent.documentOffsetTop : 0);
    }
});

window.Object.defineProperty(Element.prototype, 'documentOffsetLeft', {
    get: function () {
        return this.offsetLeft + (this.offsetParent ? this.offsetParent.documentOffsetLeft : 0);
    }
});