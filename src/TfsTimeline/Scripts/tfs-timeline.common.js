var tfsTimeline = tfsTimeline || {};

tfsTimeline.parseJsonDate = function (jsonDate) {
    var offset = new Date().getTimezoneOffset() * 60000;
    var parts = /\/Date\((-?\d+)([+-]\d{2})?(\d{2})?.*/.exec(jsonDate);

    if (parts[2] == undefined) parts[2] = 0;
    if (parts[3] == undefined) parts[3] = 0;
    if (parts[2] == 0 && parts[3] == 0) offset = 0;

    return new Date(+parts[1] + offset + parts[2] * 3600000 + parts[3] * 60000);
};

tfsTimeline.formatDuration = function (duration) {
    var hours = Math.floor(duration / 3600);
    var minutes = Math.floor((duration - (hours * 3600)) / 60);
    var seconds = duration - (minutes * 60) - (hours * 3600);
    
    var result = (hours > 0) 
                    ? hours + ":" + tfsTimeline.padLeadingZero(minutes) + ":" + tfsTimeline.padLeadingZero(seconds)
                    : minutes + ":" + tfsTimeline.padLeadingZero(seconds);

    return result;
};

tfsTimeline.formatShortTime = function (time) {
    return time.getHours() + ":" + tfsTimeline.padLeadingZero(time.getMinutes());
};

tfsTimeline.padLeadingZero = function(number) {
    return ("0" + number).slice(-2);
}