var Box = Box || {};

Box.ContentAsyncVM = function (id, skip, top, kind, location, order, createdFrom, createdTo, area) {
    
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
        
    this.nextContentButtonVisible = ko.observable(true);

    this._getData = function () {

        if (!Box.antiForgeryToken)
            return;
                
        
        $.ajax({
            url: Box.webAppUrl + 'api/cms_publishedContents/?filter=' + '&skip=' + me._skip + '&top=' + me._top + '&kind=' + me._kind + '&location=' + me._location + '&order=' + me._order + '&createdFrom=' + me._createdFrom + "&createdTo=" + me._createdTo + "&area=" + me._area,
            type: 'GET',
            headers: { 'RequestVerificationToken': Box.antiForgeryToken },
            success: function (data) {
                for (var i = 0; i < data.length; i++) {
                    me.contents.push(data[i]);
                }                
                me._skip = me._skip + top;
                if (data.length < me._top)
                    me.nextContentButtonVisible(false);
            }
        });
    }

    var list = document.getElementById(id);    
    if(list)
        ko.applyBindings(this, list);

}






