function ContentCaptureVM() {
    CrudVM.call(this, 'cms', 'contents', 'ContentUId');
    var me = this;

    this.createdFromFilter = new ko.observable();
    this.oldCreatedFromFilter = null;
    this.contentLocation = new ko.observable();
    this.kind = null;
 
    this.previewUrl = new ko.observable();
    this.customDataBeforePost = null;

    this.filterOnlyPublished = new ko.observable(false);

    this.publishAlert = 'Publish?';

    this.isContentExpired = function (item) {
        return item.PublishUntil != null && item.PublishUntil < new Date();
    }

    this.isContentScheduled = function (item) {
        return !me.isContentExpired(item) && item.PublishAfter > new Date();
    }

    this.isContentNotPublished = function (item) {
        return !me.isContentExpired(item) && item.PublishAfter == null;
    }


    this.applyContentChanges = function (publish) {
        me.customPostParameters = '';
        if (publish) {
            if (!window.confirm(me.publishAlert))
                return;
            me.editingItem().PublishAfter = new Date();
            me.editingItem().PublishAfter.setMinutes(0, 0, 0);
            me.customPostParameters = '?publishNow=1';
        }
        me.applyItemChanges();
    }

    me.createdFromFilter.subscribe(function (newValue) {
        if (newValue != me.oldCreatedFromFilter && me.oldCreatedFromFilter!=null) me.loadData();
        me.oldCreatedFromFilter = newValue;
    });

    me.filterOnlyPublished.subscribe(function (newValue) {
        me.loadData();        
    });

    this.afterGet = function (data) {
        if (!(data instanceof Array)) {
            me.formatContent(data);
            return;
        }        
        for (var c in data)
            me.formatContent(data[c]);
    }

    this.afterPost = function (data) {
        data.contentUrl = me.getContentUrl(data.Location, data.CanonicalName);
        
        if (me.editingItem != null) {
            me.editingItem.contentUrl = data.contentUrl;
            me.editingItem.PublishAfter = data.PublishAfter;
        }

        me[me._resourceName].remove(
            function (item) {
                return item.Location.toLowerCase() != me.contentLocation().toLowerCase();
        })  

    }

    this.formatContent = function (data) {
        data.contentUrl = me.getContentUrl(data.Location, data.CanonicalName);        
    }

    this.getContentUrl = function (location, name) {
        return 'http://' + _siteHost + location + name;
    }

    this.beforePost = function (contentHead) {

        if(contentHead.Location==null || contentHead.Location=='')
            contentHead.Location = me.contentLocation();
        contentHead.Kind = me.kind;

        if (me.customDataBeforePost != null)
            me.customDataBeforePost(contentHead);

        contentHead.Data = new Object();
        contentHead.Data.JSON = JSON.stringify(contentHead.CONTENT);

        return contentHead;
    }

    this.getCustomFilter = function () {
        return '&location=' + me.contentLocation() + '&kind=' + me.kind + '&createdFrom=' + me.createdFromFilter() + '&onlyPublished=' + me.filterOnlyPublished();
    }

    this.isCrossLinkAt = function (area) {
        if (me.editingItem() == null || me.editingItem().CrossLinks == null)
            return -1;
        for (var li = me.editingItem().CrossLinks.length - 1, i=li; i >= 0; i--) {
            if (me.editingItem().CrossLinks[i].PageArea == area)
                return i;
        }
        return -1;
    }

    this.addCrossLink = function (area) {
        if (me.editingItem == null)
            return;
        if (me.editingItem().CrossLinks == null)
            me.editingItem().CrossLinks = new Array();

        var idx = me.isCrossLinkAt(area);
        if (idx < 0)
            me.editingItem().CrossLinks.push({ PageArea: area });
        else
            me.editingItem().CrossLinks.splice(idx, 1);

        me.editingItem.valueHasMutated();
    }

    this.loadContent = function (after) {

        if (me.editingItem() == null)
            return;

        me.editingItem().CONTENT = new Object();
        
        $.ajax({
            url: _webAppUrl + 'api/cms_contents/WithData/' + me.editingItem().ContentUId,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                me.editingItem().CONTENT = JSON.parse(data.Data.JSON);
                me.editingItem().TagsString = getTagsString(data.Tags);
                me.editingItem.valueHasMutated();
                if (after != null)
                    after();

            }
        });
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

    this.setContentLocation = function (loc, refresh) {
        me.contentLocation(loc);        
        if (refresh)
            me.loadData();
    }

    this.stopPreview = function () {
        if (me.previewUrl() == null)
            return;
        me.previewUrl(null);        
    }
    this.setPreviewUrl = function (content) {
        me.previewUrl("Preview/" + content.ContentUId);
    }

    this.resetPublish = function () {
        me.editingItem().PublishAfter = null;
        me.editingItem().PublishUntil = null;
        me.editingItem.valueHasMutated();
    }

    this.browseContent = function (content) {
        var w = window.open('Preview/' + content.ContentUId, content.ContentUId);
    }

}
ContentCaptureVM.prototype = new CrudVM();
var pageVM = new ContentCaptureVM();

FileUrlConverter = function () {

    filePathToModel = function (url, thumb) {
        url = url.replace(_webAppUrl + 'files/', '');
        if (thumb)
            url = url.replace('/?asThumb=true', '');
        return url;
    }

    filePathToView = function (filePath, thumb) {
        if (filePath == null)
            return '';
        filePath = _webAppUrl + 'files/' + filePath
        if (thumb)
            filePath = filePath + '/?asThumb=true';
        return filePath;
    }

    this.toModel = function (url, thumb) {
        return filePathToModel(url, thumb);
    }

    this.toView = function (fileOrPath, thumb) {
        if (fileOrPath == null)
            return '';

        if (typeof fileOrPath == 'string')
            return filePathToView(fileOrPath, thumb);

        return filePathToView(fileOrPath.Folder + '/' + fileOrPath.FileUId, thumb);
    }
}

var FileUrl = new FileUrlConverter();
