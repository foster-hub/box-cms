function CrossLinksVM(area) {

    
    CrudVM.call(this, 'cms', 'contents', 'ContentUId');
    var me = this;

    me.area = area;

    this.getCustomFilter = function () {
        return '&area=' + me.area;
    }

}

CrossLinksVM.prototype = new CrudVM();
