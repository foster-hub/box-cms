var nicEditorGalleryBoxButton = nicEditorSelect.extend({    
    init: function () {
        var icon = this.ne.options.iconsPath.replace('nicEditorIcons.gif', '') + 'gallery.png';
        this.setDisplay('<img src="' + icon + '" width="18" height="18"/>');
        this.add('asThumb', 'As thumbs');
        this.add('asCarousel', 'As a carousel');
    }
});

var nicEditorGalleryBox = {};
nicEditorGalleryBox.addButton = function (htmlEditorId, galleryId) {

        var options = {
            buttons: {
                'boxGallery': {
                    name: __('Add gallery'), type: 'nicEditorGalleryBoxButton',
                    externalCommand: function (ne, cssType) {
                        var nic = ne.selectedInstance;
                        var host = 'http://' + _siteHost;
                        if (_siteHost == location.host)
                            host = '';
                        var html = '<img id="__boxImgGallery' + galleryId + '" class="__boxImgGallery ' + cssType + '"  src="' + host + '/images/galleryAnchor.png"></img>';
                        nic.nicCommand('insertHTML', html);
                    }
                }
            },
            iconFiles: { 'boxGallery': _webAppUrl + 'Scripts/nicedit/gallery.png' }
        };

        var nic = htmlEditor.instanceById(htmlEditorId);

        var b = nic.ne.nicPanel.addButtonExt('boxGallery', options);
}
