var Box = Box || {};

Box.CommentsVM = function (id, mode) {

    this.contentId = id;
    this.comments = new ko.observableArray();
    this.newComment = new ko.observable();
    this.isLoading = new ko.observable(false);

    this.top = 10;
    this.order = 'DESC';
    this.postFillUp = true;
    this.getFillUp = false;
    this.autoUpdate = new ko.observable(true);

    var me = this;

    if (mode == 'FORUM') {
        me.top = 0;
        me.order = 'NORMAL';
        me.postFillUp = false;
        me.getFillUp = false;
        me.autoUpdate(false);
    }
    else {
        setInterval(function () {
            if (me.autoUpdate()) me.getNewest(true, true);
        },
        2000);
    }

 
    this.getMore = function () {
        if (mode == 'FORUM')
            me.getNewest();
        else
            me.getOldest();
    }

    this.getNewest = function (isNew, quiateLoad) {
        me.get(me.getHighPosition(), -1, me.postFillUp, isNew, quiateLoad);
    }

    this.getOldest = function () {
        me.get(-1, me.getLowPosition(), me.getFillUp);
    }

    this.get = function (after, before, fillUp, isNew, quiateLoad) {
        if (!quiateLoad)
            me.isLoading(true);

        $.ajax({
            url: '/api/cms_Comments/' + me.contentId + '/?afterPosition=' + after + '&beforePosition=' + before + '&top=' + me.top + '&order=' + me.order,
            type: 'GET',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            success: function (data) {
                me.isLoading(false);
                fillComments(data, me.comments, fillUp, isNew);
                setTimeout(function () { $('.newComment').css('background-color', 'transparent'); }, 500);
            }
        });
    }

    this.post = function () {

        var comment = new Object();
        comment.ContentUId = me.contentId;
        comment.Author = 'marcos';
        comment.Comment = me.newComment();

        // Sent to server        
        $.ajax({
            url: '/api/cms_Comments/?returnNewPosts=true&lastCommentPosition=' + me.getHighPosition() + '&order=' + me.order,
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            data: JSON.stringify(comment),
            success: function (data) {
                fillComments(data, me.comments, me.postFillUp, true);
                me.newComment('');
                setTimeout(function () { $('.newComment').css('background-color', 'transparent'); }, 500);
            },
            error: function (request) {

            }
        });

    }

    this.getHighPosition = function () {
        var comments = me.comments();
        if (comments.length == 0)
            return -1;
        var pos = comments[0].Position;
        for (var i = 0; i < comments.length; i++) {
            if (comments[i].Position > pos)
                pos = comments[i].Position;
        }
        return pos;
    }

    this.getLowPosition = function () {
        var comments = me.comments();
        if (comments.length == 0)
            return -1;
        var pos = comments[0].Position;
        for (var i = 0; i < comments.length; i++) {
            if (comments[i].Position < pos)
                pos = comments[i].Position;
        }
        return pos;
    }


    fillComments = function (data, comments, fillUp, isNew) {
        if (data == null)
            return;
        for (var i = 0; i < data.length; i++) {
            if (fillUp)
                comments.splice(i, 0, formatComment(data[i], isNew));
            else
                comments.push(formatComment(data[i], isNew));
        }
    }

    formatComment = function (comment, isNew) {

        var lines = comment.Comment.split('\n');
        comment.lines = lines.length;
        if (lines.length == 0)
            return;

        lines[0] = '<b>' + lines[0] + '</b>';
        for (var i = 0; i < lines.length; i++)
            lines[i] = lines[i] + '</br>';

        comment.Comment = lines.join('');

        comment.isNewComment = isNew;

        return comment;
    }


    this.showFullComment = function (el) {
        var commentDiv = el.children[0];
        if (commentDiv.style['max-height'] != 'none')
            commentDiv.style['max-height'] = 'none';
        else
            commentDiv.style['max-height'] = '70px';
    }

};


