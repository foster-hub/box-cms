var nicEditorGalleryBoxButton = nicEditorButton.extend({    
    init: function () {    
    }
});

var nicEditorGalleryBox = {};
nicEditorGalleryBox.addButton = function (htmlEditorId, galleryId) {

        var options = {
            buttons: {
                'boxGallery': {
                    name: __('Add gallery'), type: 'nicEditorGalleryBoxButton',
                    externalCommand: function (ne) {
                        var nic = ne.selectedInstance;
                        var html = '<img id="__boxImgGallery' + galleryId + '" class="__boxImgGallery"  src="' + _webAppUrl + 'Scripts/nicedit/galleryAnchor.png' +'"></img>';
                        nic.nicCommand('insertHTML', html);
                    }
                }
            },
            iconFiles: { 'boxGallery': _webAppUrl + 'Scripts/nicedit/gallery.png' }
        };

        var nic = htmlEditor.instanceById(htmlEditorId);

        var b = nic.ne.nicPanel.addButtonExt('boxGallery', options);
}
