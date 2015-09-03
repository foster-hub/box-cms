var Box = Box || {};
Box.Dialog = Box.Dialog || {};

Box.Dialog.Helper = function () {

    this.operationMessageText = '';
    this.operationMessage = null;
    this.loadingData = null;

    var me = this;

    this.endark = function () {
        var darkScreen = document.getElementById('darkScreen');
        darkScreen.style.display = 'block';
    }

    this.undark = function () {
        var darkScreen = document.getElementById('darkScreen');
        darkScreen.style.display = 'none';
    }

    this.showDialog = function (dialogId) {
        me.endark();
        var dialog = document.getElementById(dialogId);
        dialog.style.display = 'block';
    }

    this.closeDialog = function (dialogId, keepDark) {

        if (keepDark != true)
            me.undark();
        var dialog = document.getElementById(dialogId);
        dialog.style.display = 'none';
    }

    this.setOperationMessage = function (message) {        
        if (message != null && message != '') {
            me.operationMessageText.innerHTML = message;
            me.operationMessage.style.display = 'block';
        }
        else {
            me.operationMessageText.innerHTML = '';
            me.operationMessage.style.display = 'none';
        }
    }

    this.setLoadingData = function (value) {
        if(value)
            me.loadingData.style.display = 'block';
        else
            me.loadingData.style.display = 'none';
    }

    this.ifEnter = function (f) {
        if (event.keyCode != 13)
            return;
        if (f != null)
            f();
    }

    this.blurOnEnter = function (e) {
        if (event.keyCode != 13 && event.keyCode != 7)
            return;
        if (e != null)
            e.blur();
    }

}

var dialogHelper = new Box.Dialog.Helper();

function showContextMenu(item) {
    if (item._contextMenuDisabled) 
        return;
    $('.contextMenu', item).css('display','block');    
}

function hideContextMenu(item) {
    $('.contextMenu', item).css('display', 'none');
}

$(document).ready(function () {    
    dialogHelper.operationMessage = document.getElementById('operationMessage');
    dialogHelper.operationMessageText = document.getElementById('operationMessageText');
    dialogHelper.loadingData = document.getElementById('loadingData');
});
