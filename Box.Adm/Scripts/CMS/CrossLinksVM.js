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

}

CrossLinksVM.prototype = new CrudVM();
