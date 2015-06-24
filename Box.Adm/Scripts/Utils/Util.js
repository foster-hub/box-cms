var Box = Box || {};

Box.Util = function () {
};

Box.Util.GetHashValue = function (key) {
    if (!window.location.hash)
        return;

    var hash = window.location.hash.substring(1);

    var pairs = hash.split('&');
    if (pairs.length == 0)
        return null;

    for (var i = 0; i < pairs.length; i++) {
        var kv = pairs[i].split('=');
        var _key = pairs[i];
        var _value = null;
        if (kv.length == 2) {
            _key = kv[0];
            _value = kv[1];
        }
        if (_key.toLowerCase() == key.toLowerCase())
            return _value;
    }

    return null;

}

Box.Util.clone = function(obj, copy) {
    if (null == obj || "object" != typeof obj) return obj;
    if (copy == null)
        copy = obj.constructor();
    for (var attr in obj) {
        if (obj.hasOwnProperty(attr)) copy[attr] = obj[attr];
    }
    return copy;
}
