function require(script) {
    $.ajax({
        url: script,
        dataType: "script",
        async: false,           // <-- This is the key
        success: function () {
            // all good...
        },
        error: function () {
            throw new Error("Could not load script " + script);
        }
    });
}

/*
jSelector = jquery selector for origin of spinner
*/
ActivityIndicatorOn = function (jSelector) {

    $(jSelector).css({
        position: 'fixed',
        top: '50%',
        left: '50%'
    });

    //require("../../Scripts/spin.min.js");

    var spinnerOpts = {
        lines: 11, // The number of lines to draw
        length: 15, // The length of each line
        width: 10, // The line thickness
        radius: 30, // The radius of the inner circle
        corners: 1, // Corner roundness (0..1)
        rotate: 0, // The rotation offset
        direction: 1, // 1: clockwise, -1: counterclockwise
        color: '#000', // #rgb or #rrggbb
        speed: 0.6, // Rounds per second
        trail: 60, // Afterglow percentage
        shadow: false, // Whether to render a shadow
        hwaccel: false, // Whether to use hardware acceleration
        className: 'spinner', // The CSS class to assign to the spinner
        zIndex: 2e9, // The z-index (defaults to 2000000000)
        top: 'auto', // Top position relative to parent in px
        left: 'auto' // Left position relative to parent in px
    };

    var spinner_div = $(jSelector).get(0);
    spinner = new Spinner(spinnerOpts).spin(spinner_div);

}

ActivitySelectorOff = function (jSelector) {
    spinner.stop($(jSelector));
}