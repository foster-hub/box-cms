function ContentBrowseVM() {
    CrudVM.call(this, 'cms', 'contents', 'ContentUId');
    var me = this;

    this.kind = new ko.observable();
    this.oldKind;
 
    this.previewUrl = new ko.observable();

    me.kind.subscribe(function (newValue) {
        if (newValue != me.oldKind && me.oldKind != null) me.loadData();
        me.oldKind = newValue;
    });
    
    this.isContentExpired = function (item) {
        return item.PublishUntil != null && item.PublishUntil < new Date();
    }

    this.isContentScheduled = function (item) {
        return !me.isContentExpired(item) && item.PublishAfter > new Date();
    }

    this.isContentNotPublished = function (item) {
        return !me.isContentExpired(item) && item.PublishAfter == null;
    }

    this.afterGet = function (data) {
        if (!(data instanceof Array)) {
            me.formatContent(data);
            return;
        }        
        for (var c in data)
            me.formatContent(data[c]);
    }

    this.formatContent = function (data) {
        data.contentUrl = me.getContentUrl(data.Location, data.CanonicalName);
        me.addTagsCss(data);
    }

    this.addTagsCss = function (data) {
        if (data == null)
            return;
        for (var i = 0; i < data.Tags.length; i++) {
            data.Tags[i].tagCss = '';
            if (data.Tags[i].Tag != '') {
                data.Tags[i].tagCss = 'tagStyle_' + _adjustTagName(data.Tags[i].Tag);
            }
        }
    }

    _adjustTagName = function (tag) {
        var str = tag;
        str = str.replace(/[áàâãª]/g, 'a');
        str = str.replace(/[ÁÀÂÃ]/g, 'A');
        str = str.replace(/[éèêë]/g, 'e');
        str = str.replace(/[ÉÈÊË]/g, 'E');
        str = str.replace(/[íìî]/g, 'i');
        str = str.replace(/[ÍÌÎ]/g, 'I');
        str = str.replace(/[óòôõº]/g, 'o');
        str = str.replace(/[ÓÒÔÕ]/g, 'O');
        str = str.replace(/[úùû]/g, 'u');
        str = str.replace(/[ÚÙÛÜ]/g, 'U');
        str = str.replace(/[ç]/g, 'c');
        str = str.replace(/[Ç]/g, 'C');
        str = str.replace(/[@#&*\s\.]/g, '_');
        return str.toLowerCase();
    }

    this.getContentUrl = function (location, name) {
        return 'http://' + _siteHost + location + name;
    }

    this.getCustomFilter = function () {
        return '&kind=' + me.kind();
    }

    getTagsString = function (tags) {
        var str = '';
        for (var t in tags) {
            str = str + ', ' + tags[t].Tag;
        }
        if (str == ', ')
            return '';
        return str.substring(2);
    }

    this.resetPublish = function () {
        me.editingItem().PublishAfter = null;
        me.editingItem().PublishUntil = null;
        me.editingItem.valueHasMutated();
    }

    this.browseContent = function (content) {

        if (content.ExternalLinkUrl != null) {
            var w = window.open(content.ExternalLinkUrl, content.ContentUId);
            return;
        }

        var w = window.open(_webAppUrl + 'cms_contents/Preview/' + content.ContentUId, content.ContentUId);
    }

    this.setItemsPerPage = function (width, height) {

        var itemsPerLine = Math.floor(width / 260);
        var linesPerPage = Math.floor(height / 130);
        var itensPerPage = itemsPerLine * linesPerPage;

        me.paging.itemsPerPage = itensPerPage;
        if (me.paging.itemsPerPage < 0)
            me.paging.itemsPerPage = 8;
    }

    this.selectContent = function (content) {
        if (me.onSelect == null)
            return;
        me.onSelect(content);
    }

}
ContentBrowseVM.prototype = new CrudVM();
var contentBrowseVM = new ContentBrowseVM();
