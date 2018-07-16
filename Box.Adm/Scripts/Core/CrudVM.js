
function PagingHelper() {

    this.itemsPerPage = 20;
    var skip = 0;

    var me = this;

    var newSkip = skip;

    this.resetSkip = function () {
        return newSkip = 0;
    }

    this.getNextSkip = function () {
        return newSkip = skip + me.itemsPerPage;
    }

    this.getPreviousSkip = function () {
        return newSkip = skip - me.itemsPerPage;
    }

    this.applySkip = function () {
        skip = newSkip;
    }

    //paginação
    this.currentPage = new ko.observable(1);
    this.totalPages = new ko.observable(0);
    this.totalRecords = 0;

    this.getLastSkip = function () {
        var isInt = me.totalRecords / me.itemsPerPage;

        if (isInt === parseInt(isInt))
            return newSkip = me.totalRecords - me.itemsPerPage;
        else
            return newSkip = (Math.floor(me.totalRecords / me.itemsPerPage) * me.itemsPerPage);
    }
    //paginação
}

var CrudOptions = {};
CrudOptions.AllowHttpDELETE = false;
CrudOptions.AllowHttpPUT = false;

function CrudVM(moduleName, name, uIdField) {
    

    this._module = moduleName;
    this._resourceName = name;
    this._resourceUIdField = uIdField == null ? 'id' : uIdField;

    this[this._resourceName] = new ko.observableArray();

    this.afterGet = null;
    this.afterPost = null;

    this.removingItem = new ko.observable();
    this.editingItem = new ko.observable();
    this.newItem = new ko.observable();            
    this.lastEditingItem = null;

    this.searchFilter = new ko.observable('');
    this.order = new ko.observable();
    this.oldOrder = null;
    
    this.errorMsgItemAlreadyExists = 'Error inserting item.<br/>Another item already exists.';

    this.paging = new PagingHelper();
    this.hasPreviousPage = new ko.observable(false);
    this.hasNextPage = new ko.observable(false);

    this.customPostParameters = '';

    this.editingItemCopy = null;    

    this.firstLoaded = new ko.observable(false);

    this.isSaved = new ko.observable(true);


    var me = this;

    me._actionData = '';


    me.searchFilter.subscribe(function (newValue) {
        if (newValue != me._lastFilter) me.filter();        
    });

    me.order.subscribe(function (newValue) {        
        if (newValue != me.oldOrder && me.oldOrder != null) me.loadData();
        me.oldOrder = newValue;
    });

    this.init = function () {
        me.setEditingItem();
        me.loadData();        
    }

    this.setAddingItem = function (item) {
        me.newItem(item);
        me.setEditingItem(item);        
    }

    this.setEditingItem = function (item) {
        me.lastEditingItem = me.editingItem();
        me.editingItemCopy = Box.Util.clone(item);
        me.editingItem(item);

        if (me.editingItem() != null) {
            //Ao pressionar o botão voltar do navegador, define o EditingItem = null para exibir a listagem novamente
            $(window).on('popstate', function () {
                pageVM.setEditingItem(null);
            });

            //Ao entrar no detalhe de um item, guarda a adicionar a pagina no histórico de navegação
            if (window.history.pushState)
                window.history.pushState(document.URL, "", document.URL);
        }
    }

    this.setRemovingItem = function (item) {
        me.removingItem(item);        
    }

    this.removeItem = function () {
        me._deleteData(me.removingItem()[me._resourceUIdField]);                
    }

    this.applyItemChanges = function (dontClose, afterSave) {

        if (me.newItem() == me.editingItem()) {
            me._postData(me.newItem(), afterSave);
            if (!dontClose)
                me.setAddingItem(null); // goes back to list            
            return;
        }

        me._putData(me.editingItem()[me._resourceUIdField], me.editingItem(), dontClose, afterSave);

        //me.setEditingItem(null);
    }


    this.cancelItemChanges = function () {
        dialogHelper.setOperationMessage('');
        Box.Util.clone(me.editingItemCopy, me.editingItem());
        me.setAddingItem(null);
        //Como no setEditingItem andamos um pagina p/ frente, no cancelItemChanges o navegador deve voltar uma pagína.
        if (window.history.pushState)
            window.history.back();
    }

    this.loadData = function () {        
        me._getData(me.paging.resetSkip());
    }

    this.loadNextData = function () {
        me._getData(me.paging.getNextSkip());
    }

    this.loadPreviousData = function () {
        me._getData(me.paging.getPreviousSkip());
    }

    //paginação
    this.loadLastData = function () {
        me._getData(me.paging.getLastSkip());
    }

    this.loadFirstData = function () {
        me._getData(me.paging.resetSkip());
    }
    //paginação

    this.beforePost = function (data) {
        return data;
    }

    this._lastKeyTime = null;
    this._lastFilter;

    this.filter = function () {

        if (me._lastKeyTime == null) {
            me._lastKeyTime = new Date();
            setTimeout(function () { me._checkKeyPress() }, 500);            
        }
        else {            
            me._lastKeyTime = new Date();            
        }
    }

    this._checkKeyPress = function () {
        var now = new Date();
        var dif = now - me._lastKeyTime;
        if (dif > 500) {
            if (me._lastFilter != me.searchFilter())
                me._getData(0);
            me._lastFilter = me.searchFilter();
            me._lastKeyTime = null;
            
        }
        else {
            setTimeout(function () { me._checkKeyPress() }, 500 - dif);            
        }
    }

    this.getCustomFilter = function() {
        return '';
    }

    this._getData = function (skip) {

        $.ajax({
            url: _webAppUrl + 'api/' + me._module + '_' + me._resourceName + me._actionData + '/?filter=' + encodeURIComponent(me.searchFilter()) + '&skip=' + skip + '&top=' + me.paging.itemsPerPage + '&order=' + me.order() + me.getCustomFilter(),
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data, textStatus, request) {

                if (me.afterGet != null)
                    me.afterGet(data);

                me[me._resourceName](data);

                // set paging
                me.paging.applySkip();
                me.hasPreviousPage(skip > 0);
                //me.hasNextPage(!(data.length < me.paging.itemsPerPage));

                //paginação
                if (request.getResponseHeader('TotalRecords') != null) {
                    var totalReg = request.getResponseHeader('TotalRecords');
                    me.paging.currentPage(Math.ceil((skip / me.paging.itemsPerPage) + 1));
                    me.paging.totalRecords = totalReg;
                    me.paging.totalPages(Math.ceil(totalReg / me.paging.itemsPerPage));
                    me.hasNextPage(me.paging.currentPage() < me.paging.totalPages());
                } else {
                    me.hasNextPage(!(data.length < me.paging.itemsPerPage));
                }

                me.firstLoaded(true);


            }
        });
    }


    this._postData = function (data, afterSave) {

        // Sent to server        
        $.ajax({
            url: _webAppUrl + 'api/' + me._module + '_' + me._resourceName + me.customPostParameters,
            type: 'POST',
            data: JSON.stringify(me.beforePost(data)),
            headers: { 'RequestVerificationToken': window._antiForgeryToken },
            success: function (data) {

                me.isSaved(true);

                if (me.afterPost)
                    me.afterPost(data);

                if (afterSave)
                    afterSave(data);

                me[me._resourceName].splice(0, 0, data);
                
                if (me.editingItem() != null) {
                    data.CONTENT = me.editingItem().CONTENT;
                    me.setEditingItem(data);
                    me.newItem(null);
                }

                
            },
            error: function (request) {
                if (request.status == 409) {
                    dialogHelper.setOperationMessage(me.errorMsgItemAlreadyExists);
                    return;
                }
                dialogHelper.setOperationMessage('Unknow error');
            }
        });
    }

    this._putData = function (id, data, dontClose, afterSave) {

        var verb = 'PUT';
        var url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + id + me.customPostParameters;

        if (!CrudOptions.AllowHttpPUT) {
            verb = 'POST';
            url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/UPDATE/' + id + me.customPostParameters;
        }

        $.ajax({
            url: url,
            type: verb,
            data: JSON.stringify(me.beforePost(data)),
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {

                me.isSaved(true);

                if (me.afterPost)
                    me.afterPost(data);

                if (afterSave)
                    afterSave(data);


                if(!dontClose)
                    me.setEditingItem(null);
            },
            error: function (request) {
                if (request.status == 409) {
                    dialogHelper.setOperationMessage(me.errorMsgItemAlreadyExists);
                    return;
                }
                dialogHelper.setOperationMessage('Unknow error');
            }
        });
    }

    this._deleteData = function (id) {
        var verb = 'DELETE';
        var url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + id;

        if (!CrudOptions.AllowHttpDELETE) {
            verb = 'POST';
            url = _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/REMOVE/' + id;
        }

        $.ajax({
            url: url,
            type: verb,
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function () {
                me[me._resourceName].remove(me.removingItem());
                me.removingItem(null);
            }
        });

    }

    this._getItemData = function (id, afterGet) {
        
        $.ajax({
            url: _webAppUrl + 'api/' + me._module + '_' + me._resourceName + '/' + id,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function(data) { afterGet(data); }            
        });
    }

    this.showDetailByUrl = function () {
        var id = Box.Util.GetHashValue(me._resourceUIdField);
        if (!id)
            return;
        me._getItemData(id, function (data) {
            if (!data)
                return;
            me.setEditingItem(data);
            me.onShowDetail();
        });

    }

    return me;

  

}


