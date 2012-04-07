var tfsTimeline = tfsTimeline || {};

tfsTimeline.parseJsonDate = function (jsonDate) {
    var offset = new Date().getTimezoneOffset() * 60000;
    var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);

    if (parts[2] == undefined) parts[2] = 0;

    if (parts[3] == undefined) parts[3] = 0;

    if (parts[2] == 0 && parts[3] == 0) offset = 0;

    return new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
};

tfsTimeline.formatDurationAsMinutes = function (duration) {
    var minutes = Math.floor(duration / 60);
    var seconds = ("0" + (duration - minutes * 60)).slice(-2);
    return minutes + ":" + seconds;
};

tfsTimeline.formatShortTime = function (time) {
    return time.getHours() + ":" + ("0" + time.getMinutes()).slice(-2);
};