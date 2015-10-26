
var nicEditorMediaBox = {};
nicEditorMediaBox.folders = [    
    { icon: 'audio.png', folder: 'audios', name: 'Audios', type: 'audio' },
    { icon: 'video.png', folder: 'videos', name: 'Videos', type: 'video' },
    { icon: 'doc.png', folder: 'documents', name: 'Documents', type: 'doc' }
]

var nicEditorMediaBoxButton = nicEditorSelect.extend({

    init: function () {

        var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif', '') + 'media.png';
        this.setDisplay('<img src="' + icon + '" width="18" height="18"/>');

        nicEditorMediaBox._FOLDERS = new Object();

        for (var f in nicEditorMediaBox.folders) {
            var folder = nicEditorMediaBox.folders[f];
            var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif', '') + folder.icon;
            this.add(folder.folder, '<img src="' + icon + '" width="18" height="18"/>&nbsp;&nbsp;' + folder.name);
            nicEditorMediaBox._FOLDERS[folder.folder] = folder;
        }

    }
});


var nicBoxOptions = {
    buttons: {
        'boxMedia': {
            name: __('Add media'), type: 'nicEditorMediaBoxButton',
            externalCommand: function (ne, folder) {
                var nic = ne.selectedInstance;

                var options = nicEditorMediaBox._FOLDERS[folder];

                var host = 'http://' + _siteHost;
                if (_siteHost == location.host)
                    host = '';

                filesVM.saveSelection(nic.frameDoc);

                showFileDatabase(function (file) {

                    var html = '<a href="' + host + '/files/' + file.Folder + '/' + file.FileUId + '">' + file.FileName + '</a>';
                    
                    if (options.type == 'video')
                        html = '<video controls><source src="' + host + '/files/' + file.Folder + '/' + file.FileUId + '"></video>';

                    if (options.type == 'audio')
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