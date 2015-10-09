
var nicEditorMediaBox = {};
nicEditorMediaBox.folders = [
    { icon: 'image.gif', folder: 'images', name: 'Images' },
    { icon: 'audio.png', folder: 'audios', name: 'Audios' },
    { icon: 'video.png', folder: 'videos', name: 'Videos' }
]

var nicEditorMediaBoxButton = nicEditorSelect.extend({    
    init: function () {

        var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif', '') + 'media.png';
        this.setDisplay('<img src="' + icon + '" width="18" height="18"/>');

        this.sel = new Object();

        for (var f in nicEditorMediaBox.folders) {
            var folder = nicEditorMediaBox.folders[f];
            var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif', '') + folder.icon;
            this.add(folder.folder, '<img src="' + icon + '" width="18" height="18"/>&nbsp;&nbsp;' + folder.name);
            this.sel[folder.folder] = folder;
        }

    }
});


var nicBoxOptions = {
    buttons: {
        'boxMedia': {
            name: __('Add media'), type: 'nicEditorMediaBoxButton',
            externalCommand: function (ne, folder) {
                var nic = ne.selectedInstance;
                
                var widthAttr = '/?width=200';

                var host = 'http://' + _siteHost;
                if (_siteHost == location.host)
                    host = '';

                filesVM.saveSelection(nic.frameDoc);

                showFileDatabase(function (file) {

                    var html = '<img src="' + host + '/files/' + file.Folder + '/' + file.FileUId + widthAttr + '" />';

                    if (FileUrl.isVideo(file.Type))
                        html = '<video controls><source src="' + host + '/files/' + file.Folder + '/' + file.FileUId + '"></video>';

                    if (FileUrl.isAudio(file.Type))
                        html = '<audio controls><source src="' + host + '/files/' + file.Folder + '/' + file.FileUId + '"></audio>';
                    
                    filesVM.restoreSelection(nic.frameDoc);

                    nic.nicCommand('insertHTML', html);
                },
            folder);
            }
        }
    }
};

nicEditors.registerPlugin(nicPlugin, nicBoxOptions);