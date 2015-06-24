function SigninVM() {

    var me = this;

    
    me.signedUser = new ko.observable({ Password: new Object() });
    
    me.loginResult = new ko.observable('');

    setLoginResult = function (msg) {
        me.loginResult(msg);        
    }

    me.signin = function () {

        setLoginResult('');
        me.signedUser().Password.Email = me.signedUser().Email;
        $.ajax({
            url: _webAppUrl + 'api/core_signin',
            type: 'POST',
            headers: { 'RequestVerificationToken': _antiForgeryToken },
            data: JSON.stringify(me.signedUser()),
            success: function (data) {
                if (data == 0) {
                    setLoginResult('INVALID');
                    return;
                }
                if (data == -1) {
                    setLoginResult('BLOCKED');
                    return;
                }
                redirect();
            }
        });
    }

    redirect = function () {
        var url = getQueryParameterByName('ReturnUrl');
        if (url == '' || url == null)
            url = '.';
        location.href = url;

    }

    getQueryParameterByName = function(name) {
        var match = RegExp('[?&]' + name + '=([^&]*)')
                    .exec(window.location.search);
        return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
    }

    return me;

}

var pageVM = new SigninVM();
$(document).ready(function () {
    ko.applyBindings(pageVM); 
});



