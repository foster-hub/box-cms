function FormsVM() {
    CrudVM.call(this, 'contactforms', 'forms', 'ContactFormId');
    var me = this;

    this.loadMessage = function (after) {

        if (me.editingItem() == null)
            return;

        me.editingItem().Data = new Object();

        $.ajax({
            url: _webAppUrl + 'api/contactforms_forms/' + me.editingItem().ContactFormUId,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                if (data == null)
                    return;
                me.editingItem().Data = data.Data;
                me.editingItem.valueHasMutated();
                if (after != null)
                    after();
            }
        });
    }

}


FormsVM.prototype = new CrudVM();

var pageVM = new FormsVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);
    pageVM.init()
});
