/*
Var to determine mobile device
*/
var isMobile = {
    Android: function () {
        return navigator.userAgent.match(/Android/i);
    },
    BlackBerry: function () {
        return navigator.userAgent.match(/BlackBerry/i);
    },
    iOS: function () {
        return navigator.userAgent.match(/iPhone|iPad|iPod/i);
    },
    Opera: function () {
        return navigator.userAgent.match(/Opera Mini/i);
    },
    Windows: function () {
        return navigator.userAgent.match(/IEMobile/i);
    },
    any: function () {
        return (isMobile.Android() || isMobile.BlackBerry() || isMobile.iOS() || isMobile.Opera() || isMobile.Windows());
    }
};

DeviceIsMobile = function () {
    if (isMobile.any() == null) {
        return false;
    } else {
        return true;
    }
}//DeviceIsMobile = function () {

//We're going use just 5 gridlines (vertical). The first will be 0, the last will be the max
//
GetChartAxisTickets = function (minValue, maxValue, nbrOfTicks) {

    //if there is no difference between max and min value
    //we will return an array of one.
    if (minValue == maxValue) {
        tickArray = new Array(1);
        tickArray[0] = maxValue;
        return tickArray;
    }

    tickCount = 0;
    tickArray = new Array();
    tickArray[tickCount++] = 0;

    tickIncrement = maxValue / nbrOfTicks;

    tickValue = 0;
    while (tickValue < maxValue) {
        tickValue = tickValue + tickIncrement;
        if (Math.round(tickValue) > tickArray[tickCount - 1]) {
            tickArray[tickCount++] = Math.round(tickValue);
        }
    }

    return tickArray;
}//GetChartAxisTickets


