function GroupVM() {
    CrudVM.call(this, 'people', 'group', '');
}

GroupVM.prototype = new CrudVM();

var pageVM = new GroupVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);
    pageVM.init()
});