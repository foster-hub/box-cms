var Box = Box || {};

Box.ContentAsyncVM = function (id, skip, top, kind, location, order, createdFrom, createdTo, area, tags, includeData) {
    var me = new Box.ContentAsyncVMConstructor(id, skip, top, kind, location, order, createdFrom, createdTo, area, tags, includeData);

    var list = document.getElementById(id);
    if (list)
        ko.applyBindings(me, list);

    return me;
}

Box.ContentAsyncVMConstructor = function (id, skip, top, kind, location, order, createdFrom, createdTo, area, tags, includeData) {

    var me = this;
    me.contents = ko.observableArray();

    this._skip = skip;
    this._top = top;
    this._kind = kind;
    this._location = location;
    this._order = order;
    this._createdFrom = createdFrom;
    this._createdTo = createdTo;
    this._area = area;
    this._tags = tags;
    this._includeData = includeData;
    this.filter = '';

    this.nextContentButtonVisible = ko.observable(true);

    this._getTagsArrayStr = function () {

        if (me._tags == null)
            return '';

        var tagss = me._tags.split(',');
        var parametersGet = '';
        for (tag in tagss) {
            if(tagss[tag] !== '') parametersGet = parametersGet + '&tags=' + encodeURIComponent(tagss[tag]);
        };
        if (parametersGet === '')
            parametersGet = '&tags=';
        return parametersGet;
    }

    this._getKindsArrayStr = function () {

        if (me._kind === null)
            return '';

        var kinds = me._kind.split(',');
        if (kinds.length === 0)
            return '&kinds=' + kinds;
        var kindStr = '';
        for (var i = 0; i < kinds.length; i++) {
            kindStr = kindStr + '&kinds=' + encodeURIComponent(kinds[i]);
        }
        return kindStr;
    }

    this._getData = function () {

        if (!Box.antiForgeryToken)
            return;

        if (!me._includeData)
            me._includeData = 'false';

        $.ajax({
            url: Box.webAppUrl + 'api/cms_publishedContents/?filter=' + me.filter + '&skip=' + me._skip + '&top=' + (me._top + 1) + me._getKindsArrayStr() + '&location=' + me._location + '&order=' + me._order + '&createdFrom=' + me._createdFrom + "&createdTo=" + me._createdTo + "&area=" + me._area + me._getTagsArrayStr() + '&includeData=' + me._includeData,
            type: 'GET',
            headers: { 'RequestVerificationToken': Box.antiForgeryToken },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    if (i < top)
                        me.contents.push(data[i]);
                }
                me._skip = me._skip + top;
                if (data.length <= me._top)
                    me.nextContentButtonVisible(false);
                else
                    me.nextContentButtonVisible(true);

                if (me.afterGet)
                    me.afterGet(data);
            }
        });
    }

    this.addTag = function (tag) {
        me._tags = me._tags + ',' + tag;
    }

    this.removeTag = function (tag) {
        me._tags = me._tags.replace(tag, '');
        me._tags = me._tags.replace(',,',',');
    }

    this.newSearch = function () {
        me._skip = 0;
        me.contents.removeAll();
        me._getData();
    }

}
