var Box = Box || {};



Box.CarouselGallery = function (galleryId, nThumbs, height, images, appUrl) {

    Box.CarouselGallery.siteUrl = appUrl;

    var me = this;
    me.images = images;
    me.galleryId = '__boxImgGallery' + galleryId;
    
    if ($('#' + me.galleryId).length == 0)
        return;

    this.create = function () {

        var parent = $('#' + me.galleryId).parent();
        var anchor = $('#' + me.galleryId);
        maxwidth = parent.width() - anchor.css('padding-left').replace('px', '') - anchor.css('padding-right').replace('px', '');

        var thumbWidth = Math.round(maxwidth / nThumbs);
        var htmlGallery = "";

        htmlGallery = '<div id="' + me.galleryId + '" class="__boxImgGallery">'
        htmlGallery = htmlGallery + '<div id="imageGallerySlide_' + galleryId + '" class="carousel slide" data-ride="carousel" data-wrap="false">';
        htmlGallery = htmlGallery + '<div class="carousel-inner" role="listbox">';
        for (var idx in me.images) {                        
            htmlGallery = htmlGallery + '<div class="item ' + (idx==0? 'active' : '') + '">';
            htmlGallery = htmlGallery + '<img src="' + me.getImageUrl(idx) + '/?height=0&maxHeight=' + height + '&asThumb=false&width=' + maxwidth + '&maxWidth=0" alt="' + me.images[idx].FileName + '" title="' + me.images[idx].Caption + '" class="featurette-image">';
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


        htmlGallery = htmlGallery + '<div style="overflow:hidden;">';
        htmlGallery = htmlGallery + '<ol id="imageGalleryIndicators_' + galleryId + '" class="carousel-indicators">';
        for (var idx in me.images) {         
            htmlGallery = htmlGallery + '<li data-target="#imageGallerySlide_' + galleryId + '" data-slide-to="' + idx + '" class="item ' + (idx==0 ? 'active' : '') + '">';
            htmlGallery = htmlGallery + '<img src="' + me.getImageUrl(idx) + '/?maxHeight=100&height=0&asThumb=false&width=' + thumbWidth + '&maxWidth=0" alt="' + me.images[idx].FileName + '" title="' + me.images[idx].Caption + '">';
            htmlGallery = htmlGallery + "</li>";
        }                        
        htmlGallery = htmlGallery + '</ol>';
        htmlGallery = htmlGallery + '</div>';

        htmlGallery = htmlGallery + '</div>';

        $('#' + me.galleryId).replaceWith(htmlGallery);

        // thumb jump size
        var allThumbsWidth = me.images.length * thumbWidth;
        if (me.images.length > 1)
            me.thumbJump = Math.round((allThumbsWidth - maxwidth) / (me.images.length - 1));
        else
            me.thumbJump = 0;
        
        $('#' + me.galleryId).on('slide.bs.carousel', function (e) {
            if (me.thumbJump == 0)
                return;
            var indexImg = $(e.relatedTarget).index();            
            var jump = me.thumbJump * indexImg;            
            $('#imageGalleryIndicators_' + galleryId + '.carousel-indicators').css('margin-left', jump * -1 + 'px');
        });



    }

    me.getImageUrl = function (idx) {
        return Box.CarouselGallery.siteUrl + 'files/' + me.images[idx].Folder + '/' + me.images[idx].FileUId
    }

    me.create();


}

Box.ThumbsGallery = function (galleryId, images, thumbWidth, thumbHeight, appUrl) {

    Box.ThumbsGallery.siteUrl = appUrl;

    var me = this;
    me.images = images;
    me.galleryId = '__boxImgGallery' + galleryId;
    
    if ($('#' + me.galleryId).length == 0)
        return;

    me.create = function () {

        var htmlGallery = "";

        htmlGallery = '<div id="' + me.galleryId + '" class="__boxImgGallery row">';   
        for (var idx in me.images) {
            htmlGallery = htmlGallery + '<div class="col-lg-3 col-sm-6 col-xs-12 thumb" imgidx="' + idx + '">';
            htmlGallery = htmlGallery + '<a href="#" tilte="' + me.images[idx].Caption + '"><img src="' + me.getImageUrl(idx) + '/?height=0&maxHeight=' + thumbHeight + '&width=' + thumbWidth + '&maxWidth=0" alt="' + me.images[idx].Caption + '" title="' + me.images[idx].Caption + '" class="img-responsive"></a>';
            htmlGallery = htmlGallery + '</div>';
        }        
        htmlGallery = htmlGallery + '</div>';

        $('#' + me.galleryId).replaceWith(htmlGallery);

    }

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
        return Box.ThumbsGallery.siteUrl + 'files/' + me.images[idx].Folder + '/' + me.images[idx].FileUId
    }

    me.create();

    $('#' + me.galleryId + ' .thumb').click(me.showFullImg);    

}