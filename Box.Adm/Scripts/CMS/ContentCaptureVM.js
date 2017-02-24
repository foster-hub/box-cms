﻿function ContentCaptureVM() {
    CrudVM.call(this, 'cms', 'contents', 'ContentUId');
    var me = this;

    this.createdFromFilter = new ko.observable();
    this.oldCreatedFromFilter = null;
    this.contentLocation = new ko.observable();
    this.kind = null;
    this.SuggestedTags = new ko.observableArray([]);

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

    this.getTags = function () {        
        $.ajax({
            url: _webAppUrl + 'api/cms_contents/SuggestedTags/' + me.editingItem().Kind,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                me.SuggestedTags(data);
            }
        });
    }


    this.applyContentChanges = function (publish, dontClose, afterSave) {
        me.customPostParameters = '';
        if (publish) {
            if (!window.confirm(me.publishAlert))
                return;
            me.editingItem().PublishAfter = new Date();
            me.editingItem().PublishAfter.setMinutes(0, 0, 0);
            me.customPostParameters = '?publishNow=1';
        }
        me.applyItemChanges(dontClose, afterSave);
    }

    me.createdFromFilter.subscribe(function (newValue) {
        if (newValue != me.oldCreatedFromFilter && me.oldCreatedFromFilter != null) me.loadData();
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

        me.formatContent(data);

        if (me.editingItem != null) {
            me.editingItem.contentUrl = data.contentUrl;
            if (me.editingItem() != null) {
                me.editingItem().PublishAfter = data.PublishAfter;
                me.editingItem().Tags = data.Tags;
            }
        }

        me[me._resourceName].remove(
            function (item) {
                return item.Location.toLowerCase() != me.contentLocation().toLowerCase();
            })

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

    this.beforePost = function (contentHead) {

        if (contentHead.Location == null || contentHead.Location == '')
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
        for (var li = me.editingItem().CrossLinks.length - 1, i = li; i >= 0; i--) {
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

        //me.getTags();

        me.editingItem().CONTENT = new Object();

        $.ajax({
            url: _webAppUrl + 'api/cms_contents/WithData/' + me.editingItem().ContentUId,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                me.editingItem().CONTENT = JSON.parse(data.Data.JSON);
                me.editingItem().TagsString = getTagsString(data.Tags);

                me.contentCopyJson = JSON.stringify(me.editingItem().CONTENT);

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

        if (content.ExternalLinkUrl != null) {
            var w = window.open(content.ExternalLinkUrl, content.ContentUId);
            return;
        }

        var w = window.open('Preview/' + content.ContentUId, content.ContentUId);
    }

    this.browseEditingContent = function () {

        var item = me.editingItem();
        if (item == null)
            return;

        if (!me.isSaved())
            alert(me.contetNotSavedAlert);

        me.browseContent(item);
    }



}
ContentCaptureVM.prototype = new CrudVM();
var pageVM = new ContentCaptureVM();

FileUrlConverter = function () {

    var me = this;
    filePathToModel = function (url, thumb) {
        url = url.replace(_webAppUrl + 'files/', '');
        if (thumb)
            url = url.replace('/?asThumb=true', '');
        return url;
    }

    filePathToView = function (filePath, thumb, scale) {
        if (filePath == null)
            return '';
        filePath = _webAppUrl + 'files/' + filePath
        if (thumb)
            filePath = filePath + '/?asThumb=true';

        if (scale)
            filePath = filePath + '/?scale=' + scale + '&r=' + Math.floor(Math.random() * 1000);

        return filePath;
    }

    this.toModel = function (url, thumb) {
        return filePathToModel(url, thumb);
    }

    this.isImage = function (type) {
        if (type == null)
            return true;

        if (me.isAudio(type) || me.isVideo(type))
            return false;
        return true;
    }

    this.isAudio = function (type) {
        if (type == null)
            return false;

        if (type.indexOf("audio") < 0)
            return false;
        return true;
    }

    this.isVideo = function (type) {
        if (type == null)
            return false;

        if (type.indexOf("video") < 0)
            return false;
        return true;
    }

    this.getFileName = function (data) {
        if (data == null)
            return "";
        if (!data.FileName)
            return "";
        return data.FileName;
    }

    this.toView = function (fileOrPath, thumb, scale) {
        if (fileOrPath == null)
            return '';

        if (typeof fileOrPath == 'string')
            return filePathToView(fileOrPath, thumb, scale);

        if (fileOrPath.isLoading)
            return '../Content/CMS_Images/FileIcons/loading.gif';

        return filePathToView(fileOrPath.Folder + '/' + fileOrPath.FileUId, thumb, scale);
    }
}

var FileUrl = new FileUrlConverter();
