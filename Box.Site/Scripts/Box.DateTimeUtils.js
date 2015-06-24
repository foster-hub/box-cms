var Box = Box || {};

Box.DateTimeUtils = {}

Box.DateTimeUtils.secondsStr = ' second(s) ago';
Box.DateTimeUtils.minutesStr = ' minutes(s) ago';
Box.DateTimeUtils.hoursStr = ' hour(s) ago';
Box.DateTimeUtils.atStr = 'at ';
Box.DateTimeUtils.dateFormat = 'mdy';

Box.DateTimeUtils.localize = function (seconds, minutes, hours, at, format) {
    Box.DateTimeUtils.secondsStr = seconds;
    Box.DateTimeUtils.minutesStr = minutes;
    Box.DateTimeUtils.hoursStr = hours;
    Box.DateTimeUtils.atStr = at;
    Box.DateTimeUtils.dateFormat = format;
}

Box.DateTimeUtils.timeFromNow = function (date) {

    var now = new Date();

    var dif = now - date;

    var seconds = Math.round(dif / 1000);
    var minutes = Math.round(dif / 1000 / 60);
    var hours = Math.round(dif / 1000 / 60 / 60);

    if (seconds < 60)
        return seconds + ' ' + Box.DateTimeUtils.secondsStr;

    if (minutes < 60)
        return minutes + ' ' + Box.DateTimeUtils.minutesStr;

    if (hours < 24)
        return hours + ' ' + Box.DateTimeUtils.hoursStr;

    if (Box.DateTimeUtils.dateFormat == 'mdy')
        return Box.DateTimeUtils.atStr + ' ' + (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();

    return Box.DateTimeUtils.atStr + ' ' + date.getDate() + '/' + (date.getMonth() + 1) + '/' + date.getFullYear();
}