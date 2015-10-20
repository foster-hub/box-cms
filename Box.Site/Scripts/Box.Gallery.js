var Box = Box || {};

Box.Gallery = function (galleryId, images, appUrl) {

    Box.Gallery.siteUrl = appUrl;

    var me = this;
    me.images = images;
    me.galleryId = '__boxImgGallery' + galleryId;

    var elm = $('[id="' + me.galleryId + '"]');
    if (elm.length == 0)
        return;

    elm.each(function () {
        this.__boxGallery = me;
    });

    me.showFullImg = function () {

        me.zoomImgidx = parseInt($(this).attr('imgidx'));

        html = '<img src="' + me.getImageUrl(me.zoomImgidx) + '" class="img-responsive zoomedImg"/>';
        $('.image-zoom-modal .modal-content').empty();
        $('.image-zoom-modal .modal-content').append(html);

        html = '<a href="#"><span class="glyphicon glyphicon-chevron-right nextImageButton"></span></a><a href="#"><span class="glyphicon glyphicon-chevron-left previousImageButton"></span></a>';
        $('.image-zoom-modal .modal-round').empty();
        $('.image-zoom-modal .modal-round').append(html);

        $('.image-zoom-modal .modal-round .nextImageButton').click(me.nextImage);
        $('.image-zoom-modal .modal-round .previousImageButton').click(me.previousImage);

        $('.image-zoom-modal').modal({ show: true });

        return false;
    }

    me.nextImage = function () {
        me.zoomImgidx = (me.zoomImgidx + 1) % me.images.length;
        $('.image-zoom-modal .modal-content .zoomedImg').attr('src', me.getImageUrl(me.zoomImgidx));
        return false;
    }

    me.previousImage = function () {
        me.zoomImgidx = me.zoomImgidx - 1;
        if (me.zoomImgidx < 0)
            me.zoomImgidx = me.images.length - 1;
        $('.image-zoom-modal .modal-content .zoomedImg').attr('src', me.getImageUrl(me.zoomImgidx));
        return false;
    }

    me.getImageUrl = function (idx) {
        return Box.Gallery.siteUrl + 'files/' + me.images[idx].Folder + '/' + me.images[idx].FileUId
    }
  
    me.createAsCarousel = function (height, nThumbs) {

        if (!nThumbs || nThumbs==0) {
            nThumbs = me.images.length;
        }

        if (nThumbs > images.length)
            nThumbs = images.length;

        var parent = $('#' + me.galleryId).parent();
        var anchor = $('#' + me.galleryId);
        maxwidth = parent.width() - anchor.css('padding-left').replace('px', '') - anchor.css('padding-right').replace('px', '');

        var thumbWidth = Math.round(maxwidth / nThumbs);
        var htmlGallery = "";

        htmlGallery = '<div id="' + me.galleryId + '" class="__boxImgGallery">'
        htmlGallery = htmlGallery + '<div id="imageGallerySlide_' + galleryId + '" class="carousel slide" data-ride="carousel" data-wrap="false">';
        htmlGallery = htmlGallery + '<div class="carousel-inner" role="listbox">';
        for (var idx in me.images) {
            htmlGallery = htmlGallery + '<div class="item ' + (idx == 0 ? 'active' : '') + '">';
            htmlGallery = htmlGallery + '<img src="' + me.getImageUrl(idx) + '/?height=0&maxHeight=' + height + '&asThumb=false&width=' + maxwidth + '&maxWidth=0" alt="' + me.images[idx].FileName + '" title="' + me.images[idx].Caption + '" class="featurette-image">';
            htmlGallery = htmlGallery + '</div>';
        }
        htmlGallery = htmlGallery + '</div>';

        if (me.images.length > 1) {
            htmlGallery = htmlGallery + '<a class="left carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="prev">';
            htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-left" aria-hidden="true"></span>';
            htmlGallery = htmlGallery + '<span class="sr-only">Previous</span>';
            htmlGallery = htmlGallery + '</a>';
            htmlGallery = htmlGallery + '<a class="right carousel-control" href="#imageGallerySlide_' + galleryId + '" role="button" data-slide="next">';
            htmlGallery = htmlGallery + '<span class="glyphicon glyphicon-chevron-right" aria-hidden="true"></span>';
            htmlGallery = htmlGallery + '<span class="sr-only">Next</span>';
            htmlGallery = htmlGallery + '</a>';
        }

        htmlGallery = htmlGallery + '</div>';

        if (me.images.length > 1) {
            htmlGallery = htmlGallery + '<div style="overflow:hidden;">';
            htmlGallery = htmlGallery + '<ol id="imageGalleryIndicators_' + galleryId + '" class="carousel-indicators">';
            for (var idx in me.images) {
                htmlGallery = htmlGallery + '<li data-target="#imageGallerySlide_' + galleryId + '" data-slide-to="' + idx + '" class="item ' + (idx == 0 ? 'active' : '') + '">';
                htmlGallery = htmlGallery + '<img src="' + me.getImageUrl(idx) + '/?maxHeight=100&height=0&asThumb=false&width=' + thumbWidth + '&maxWidth=0" alt="' + me.images[idx].FileName + '" title="' + me.images[idx].Caption + '">';
                htmlGallery = htmlGallery + "</li>";
            }
            htmlGallery = htmlGallery + '</ol>';
            htmlGallery = htmlGallery + '</div>';
        }

        htmlGallery = htmlGallery + '</div>';

        if (elm == null)
            elm = $('#' + me.galleryId);

        $(elm).replaceWith(htmlGallery);

        // thumb jump size
        var allThumbsWidth = me.images.length * thumbWidth;
        if (me.images.length > 1)
            me.thumbJump = Math.round((allThumbsWidth - maxwidth) / (me.images.length - 1));
        else
            me.thumbJump = 0;

        $('#' + me.galleryId).on('slide.bs.carousel', function (e) {
            if (me.thumbJump <5)
                return;
            var indexImg = $(e.relatedTarget).index();
            var jump = me.thumbJump * indexImg;
            $('#imageGalleryIndicators_' + galleryId + '.carousel-indicators').css('margin-left', jump * -1 + 'px');
        });



    }

    me.createAsThumb = function (thumbWidth, thumbHeight, elm) {

        var htmlGallery = "";

        htmlGallery = '<div id="' + me.galleryId + '" class="__boxImgGallery row">';
        for (var idx in me.images) {
            htmlGallery = htmlGallery + '<div class="col-lg-3 col-sm-6 col-xs-12 thumb" imgidx="' + idx + '">';
            htmlGallery = htmlGallery + '<a href="#" tilte="' + me.images[idx].Caption + '"><img src="' + me.getImageUrl(idx) + '/?height=0&maxHeight=' + thumbHeight + '&width=' + thumbWidth + '&maxWidth=0" alt="' + me.images[idx].Caption + '" title="' + me.images[idx].Caption + '" class="img-responsive"></a>';
            htmlGallery = htmlGallery + '</div>';
        }
        htmlGallery = htmlGallery + '</div>';

        if (elm == null)
            elm = $('#' + me.galleryId);

        $(elm).replaceWith(htmlGallery);

        $('#' + me.galleryId + ' .thumb').click(me.showFullImg);

    }

}

Box.Gallery.createAll = function (thumbWidth, tumbHeight, height, nThumbs) {    
    if (!height) height = 350;
    if (!thumbWidth) thumbWidth = 350;
    if (!tumbHeight) tumbHeight = 200;

    $('.__boxImgGallery.asThumb').each(function () {
        if (this.__boxGallery)
            this.__boxGallery.createAsThumb(thumbWidth, tumbHeight, this);
    });
    $('.__boxImgGallery.asCarousel').each(function () {
        if (this.__boxGallery)
            this.__boxGallery.createAsCarousel(height, nThumbs, this);
    });
    
}
