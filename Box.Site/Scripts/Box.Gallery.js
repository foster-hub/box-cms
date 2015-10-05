var Box = Box || {};

Box.Gallery = function (galleryId, nThumbs, height, images, appUrl) {

    var me = this;

    me.images = images;
    me.galleryId = '__boxImgGallery' + galleryId;
    me.height = height;        
    me.nThumbs = nThumbs;

    Box.Gallery.siteUrl = appUrl;
    
    var parent = $('#' + me.galleryId).parent();
    var anchor = $('#' + me.galleryId);
    me.maxwidth = parent.width() - anchor.css('padding-left').replace('px', '') - anchor.css('padding-right').replace('px', '');

    me.nextSizeThumb = 0;

    this.create = function () {

        var thumbWidth = Math.round(me.maxwidth / me.nThumbs);
        var htmlGallery = "";

        htmlGallery = '<div class="__boxImgGallery">';
        htmlGallery = htmlGallery + '<div id="imageGallerySlide_' + galleryId + '" class="carousel slide" data-ride="carousel" data-wrap="false">';
        htmlGallery = htmlGallery + '<div class="carousel-inner" role="listbox">';
                for (var image in images) {
                    htmlGallery = htmlGallery + '<div class="item ' + (images[image] == images[0] ? 'active' : '') + '" onclick="Box.Gallery.showFullImage(\'' + images[image].Folder + '\',\'' + images[image].FileUId + '\')\">';
                    htmlGallery = htmlGallery + '<img src="' + Box.Gallery.siteUrl + 'files/' + images[image].Folder + '/' + images[image].FileUId + '/?height=0&maxHeight=' + me.height + '&asThumb=false&width=' + me.maxwidth + '&maxWidth=0" alt="' + images[image].FileName + '" title="' + images[image].FileName + '" class="featurette-image">';
                    htmlGallery = htmlGallery + '</div>';
                }
        htmlGallery = htmlGallery + '</div>';

        htmlGallery = htmlGallery + '<a class="left carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="prev">';
        htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>';
        htmlGallery = htmlGallery + '<span class="sr-only">Previous</span>';
        htmlGallery = htmlGallery + '</a>';
        htmlGallery = htmlGallery + '<a class="right carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="next">';
        htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>';
        htmlGallery = htmlGallery + '<span class="sr-only">Next</span>';
        htmlGallery = htmlGallery + '</a>';
        
        htmlGallery = htmlGallery + '</div>';

        htmlGallery = htmlGallery + '<ol class="carousel-indicators">';
        for (var image in images) {         
            htmlGallery = htmlGallery + '<li data-target="#imageGallerySlide_' + galleryId + '" data-slide-to="' + image + '" class="item ' + (images[image] == images[0] ? 'active' : '') + '">';
            htmlGallery = htmlGallery + '<img src="' + Box.Gallery.siteUrl + 'files/' + images[image].Folder + '/' + images[image].FileUId + '/?maxHeight=100&height=0&asThumb=false&width=' + thumbWidth + '&maxWidth=0" alt="' + images[image].FileName + '" title="' + images[image].FileName + '">';
            htmlGallery = htmlGallery + "</li>";
        }                        
        htmlGallery = htmlGallery + '</ol>';


        htmlGallery = htmlGallery + '</div>';

        $('#' + me.galleryId).replaceWith(htmlGallery);

        $('#' + me.galleryId + ' .carousel-control, #' + galleryId + ' .imageGallery .carousel-indicators').click(function () {
            setInterval(function () { gallery.carouselMoving(this); }, 1000);
        });

        // thumb jump size
        var allThumbsWidth = me.images.length * thumbWidth;
        me.nextSizeThumb = Math.round((allThumbsWidth - me.maxwidth) / me.images.length);
    }

    this.carouselMoving = function (element) {
        var indexImg = $('#' + me.galleryId + ' .carousel-inner div.active').index();
        var jump = me.nextSizeThumb * (indexImg + 1);
        $('#' + me.galleryId + ' .carousel-indicators').css('margin-left', jump * -1 + 'px');
    };

    me.create();
}

Box.Gallery.showFullImage = function (imgFolder, imgId) {
    var lightBox = $('#__boxLightBox');
    if (!lightBox)
        return;   
    $('#__boxLightBox img').attr('src', Box.Gallery.siteUrl + 'files/' + imgFolder + '/' + imgId + '?asThumb=false');
    lightBox.show();
};
