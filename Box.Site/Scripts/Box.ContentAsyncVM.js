var Box = Box || {};

Box.ContentAsyncVM = function (id, skip, top, kind, location, order, createdFrom, createdTo, area, tags) {
    
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
        
    this.nextContentButtonVisible = ko.observable(true);

    this._tagsGetFormat = function () {

        var tagss = me._tags.split(",");
        var parametersGet = "";
        for (tag in tagss) {
            parametersGet = parametersGet + "&tags=" + encodeURIComponent(tagss[tag]);
        };
        return parametersGet;
    }

    this.getKindsArray = function () {
        var kinds = me._kind.split(',');
        if (kinds.length === 0)
            return '&kinds=' + kinds;
        var kindStr = '';
        for (var i = 0; i < kinds.length; i++)
            kindStr = kindStr + '&kinds=' + kinds[i];
        return kindStr;
    }

    this._getData = function () {

        if (!Box.antiForgeryToken)
            return;
        
        $.ajax({
            url: Box.webAppUrl + 'api/cms_publishedContents/?filter=' + '&skip=' + me._skip + '&top=' + (me._top + 1) + me.getKindsArray() + '&location=' + me._location + '&order=' + me._order + '&createdFrom=' + me._createdFrom + "&createdTo=" + me._createdTo + "&area=" + me._area + me._tagsGetFormat(),
            type: 'GET',
            headers: { 'RequestVerificationToken': Box.antiForgeryToken },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    if(i < top)
                        me.contents.push(data[i]);
                }                
                me._skip = me._skip + top;
                if (data.length <= me._top)
                    me.nextContentButtonVisible(false);
            }
        });
    }

    var list = document.getElementById(id);    
    if(list)
        ko.applyBindings(this, list);

}






