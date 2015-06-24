var Box = Box || {};

Box.Quiz = function (contentUId) {

    this.contentUId = contentUId;
    this.result = new ko.observable();
    this.quizContentUid = "QUIZ_" + contentUId;

    var me = this;

    this.vote = function () {

        var quiz = $('input[name=' + me.contentUId + '_QUIZ]:checked').val();

        if (quiz == null) {
            alert("Choose an option")
            return;
        }

        $.ajax({
            url: '/api/cms_Quiz/' + me.contentUId,
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            data: JSON.stringify(quiz),
            success: function (data) {
                me.result(JSON.parse(data.Data.JSON));

                //Apply Bindings here.
                ko.applyBindings(me, document.getElementById(me.quizContentUid));

                $("#divQuestion").css("display", "none");
                $("#divResult").css("display", "block");
                
                //Creates Cookie(name, value, days)
                setCookie('QUIZ_' + me.contentUId, me.contentUId, 365);
            },
            error: function () {
                alert("Something bad happened");
            }
        });
    }
}

function setCookie(c_name, value, exdays) {
    var exdate = new Date();
    exdate.setDate(exdate.getDate() + exdays);
    var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());
    document.cookie = c_name + "=" + c_value;
}