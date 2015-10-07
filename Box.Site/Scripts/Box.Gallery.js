var Box = Box || {};

Box.Gallery = function (galleryId, nThumbs, height, images, appUrl) {

    var me = this;
    me.images = images;
    me.galleryId = '__boxImgGallery' + galleryId;
    me.height = height;        
    me.nThumbs = nThumbs;
    Box.Gallery._images = images;

    if ($('#' + me.galleryId).length == 0)
        return;
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
            
            var allThumbsWidth = me.images.length * thumbWidth;
            htmlGallery = htmlGallery + '<div class="item ' + (images[image] == images[0] ? 'active' : '') + '" onclick="Box.Gallery.showFullImage(\'' + images[image].Folder + '\',\'' + images[image].FileUId + '\',\'' + image + '\')\">';
            htmlGallery = htmlGallery + '<img src="' + Box.Gallery.siteUrl + 'files/' + images[image].Folder + '/' + images[image].FileUId + '/?height=0&maxHeight=' + me.height + '&asThumb=false&width=' + me.maxwidth + '&maxWidth=0" alt="' + images[image].FileName + '" title="' + images[image].FileName + '" class="featurette-image">';
            htmlGallery = htmlGallery + '</div>';
        }
        htmlGallery = htmlGallery + '</div>';

        htmlGallery = htmlGallery + '<a class="left carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="prev" onclick="javascript:setTimeout(function () { gallery.carouselMoving(this); }, 1000);">';
        htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>';
        htmlGallery = htmlGallery + '<span class="sr-only">Previous</span>';
        htmlGallery = htmlGallery + '</a>';
        htmlGallery = htmlGallery + '<a class="right carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="next" onclick="javascript:setTimeout(function () { gallery.carouselMoving(this); }, 1000);">';
        htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>';
        htmlGallery = htmlGallery + '<span class="sr-only">Next</span>';
        htmlGallery = htmlGallery + '</a>';
        
        htmlGallery = htmlGallery + '</div>';


        htmlGallery = htmlGallery + '<div style="overflow:hidden;">';
        htmlGallery = htmlGallery + '<ol id="imageGalleryIndicators_' + galleryId + '" class="carousel-indicators">';
        for (var image in images) {         
            htmlGallery = htmlGallery + '<li data-target="#imageGallerySlide_' + galleryId + '" data-slide-to="' + image + '" class="item ' + (images[image] == images[0] ? 'active' : '') + '" onclick="javascript:setTimeout(function () { gallery.carouselMoving(this); }, 1000);">';
            htmlGallery = htmlGallery + '<img src="' + Box.Gallery.siteUrl + 'files/' + images[image].Folder + '/' + images[image].FileUId + '/?maxHeight=100&height=0&asThumb=false&width=' + thumbWidth + '&maxWidth=0" alt="' + images[image].FileName + '" title="' + images[image].FileName + '">';
            htmlGallery = htmlGallery + "</li>";
        }                        
        htmlGallery = htmlGallery + '</ol>';
        htmlGallery = htmlGallery + '</div>';

        htmlGallery = htmlGallery + '</div>';

        $('#' + me.galleryId).replaceWith(htmlGallery);

        // thumb jump size
        var allThumbsWidth = me.images.length * thumbWidth;
        me.nextSizeThumb = Math.round((allThumbsWidth - me.maxwidth) / me.images.length);
    }

    this.carouselMoving = function (element) {
        var indexImg = $('#imageGallerySlide_' + galleryId + ' .carousel-inner div.active').index();
        var jump = me.nextSizeThumb * (indexImg + 1);
        if (indexImg == 0) jump = 0;

        $('#imageGalleryIndicators_' + galleryId + '.carousel-indicators').css('margin-left', jump * -1 + 'px');
    };
    me.create();    
}

$(".rightButton").click(function () {    
    Box.Gallery.lightBoxNavigation(true);
});

$(".leftButton").click(function () {
    Box.Gallery.lightBoxNavigation(false);
});

Box.Gallery.showFullImage = function (imgFolder, imgId, navigation) {
    var lightBox = $('#__boxLightBox');
    if (!lightBox)
        return;
    $('#__boxLightBox img').attr('src', Box.Gallery.siteUrl + 'files/' + imgFolder + '/' + imgId + '?asThumb=false');
    lightBox.show();
};

Box.Gallery.lightBoxNavigation = function (next) {
    var navi = $('#__boxLightBox img').attr("navigation");
    if (next)
        navi++;
    else
        navi--;

    if (navi < 0) navi = 0;
    if (navi == Box.Gallery._images.length) navi--;

    $('#__boxLightBox img').attr('src', Box.Gallery.siteUrl + 'files/' + Box.Gallery._images[navi].Folder + '/' + Box.Gallery._images[navi].FileUId + '?asThumb=false');
    $('#__boxLightBox img').attr('navigation', navi);
};

