var nicEditorImageBoxButton = nicEditorSelect.extend({
    sel: { 1: 'Pequena', 2: 'Média', 3: 'Grande', 4: 'Original' },
    init: function () {
        var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif','') + 'image.gif';
        this.setDisplay('<img src="' + icon + '" width="16" height="16"/>');
        this.add(1, '<img src="' + icon + '" width="16" height="16"/>');
        this.add(2, '<img src="' + icon + '" width="24" height="24"/>');
        this.add(3, '<img src="' + icon + '" width="32" height="32"/>');
        this.add(4, 'Original');
        
    }
});


var nicBoxOptions = {
    buttons: {
        'boxImage': { name: __('Add image'), type: 'nicEditorImageBoxButton',
            externalCommand: function (ne, size) {
                var nic = ne.selectedInstance;
                var width = '';
                if (size == 1)
                    width = 100;
                if (size == 2)
                    width = 350;
                if (size == 3)
                    width = 600;

                var widthAttr = '/?width=' + width;
                if (size == 4)
                    widthAttr = '';

                var host = 'http://' + _siteHost;
                if (_siteHost == location.host)
                    host = '';
                showFileDatabase(function (file) {
                    var img = '<img src="' + host + '/files/' + file.Folder + '/' + file.FileUId + widthAttr + '" />';
                    nic.nicCommand('insertHTML', img);
                },
            'IMG_ROOT');
            }
        }
    }
};

nicEditors.registerPlugin(nicPlugin, nicBoxOptions);