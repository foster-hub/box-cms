function LogsVM() {
    var me = new CrudVM('core', 'logs', 'LogUId');

    me.dataDe = new ko.observable(null);
    me.dataAte = new ko.observable(null);

    me.dataDe.subscribe(function (data) {
        me.loadData();
    });

    me.dataAte.subscribe(function (data) {
        me.loadData();
    });



    me.getCustomFilter = function () {
        var filtro = '';

        var dataDe = me.dataDe();
        if (dataDe) {
            filtro = filtro + '&dataDe=' + dataDe.getFullYear() + '-' + (dataDe.getMonth() + 1) + '-' + dataDe.getDate();
        }

        var dataAte = me.dataAte();
        if (dataAte) {
            filtro = filtro + '&dataAte=' + dataAte.getFullYear() + '-' + (dataAte.getMonth() + 1) + '-' + dataAte.getDate();
        }

        return filtro;
    }    

    //Quando clica em um dos logs na listagem ele irá puxar todos os dados do log para exibir na tela.
    me.showDetailById = function (id) {
        if (!id)
            return;
        me._getItemData(id, function (data) {
            if (!data)
                return;
            me.setEditingItem(data);            
        });
    }    
    return me;
}

var pageVM = new LogsVM();
$(document).ready(function () {
    ko.applyBindings(pageVM);
    pageVM.init();
    
});