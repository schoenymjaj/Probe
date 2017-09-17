
/*
    CALLING FUNCTIONS
*/


function SetLeftHeaderLogoSize() {

    width = $(window).width();
    height = $(window).height();

    if (width > 975) { //975 - was used by testing in google chrome browser. Not sure why this number isn't at the md breakpoint
        if (detectIE()) {
            $('#leftHeaderLogo img').attr('width', '225');
            $('#leftHeaderLogo img').attr('height', '89');
            $('#RightHeaderLogo img').attr('width', '225');
            $('#RightHeaderLogo img').attr('height', '89');
            $('#Password').width('150px');
        }
        $('#leftHeaderLogo img').attr('src', PrepareURL(root + '/Content/images/InCommon-JustLogo-Transparent-279x110.png'))
        $('#RightHeaderLogo img').attr('src', PrepareURL(root + '/Content/images/InCommon-JustLogo-Transparent-279x110.png'))
    } else {
        $('#leftHeaderLogo img').attr('src', PrepareURL(root + 'Content/images/InCommon-Header-Transparent-240x40.png'))
    }
};//function SetLeftHeaderLogoSize

function CredentialsSubmitValidation(e) {

    //if nothing has been entered for credentials then we just go to the login page. We don't try to authenticate
    if (e.Email.value == "" & e.Password.value == "") {
        self.location = 'Account/Login';
        return false;
    } else {
        return true;
    }


};//function CredentialsSubmitValidation()

function detectIE() {
    var ua = window.navigator.userAgent;

    var msie = ua.indexOf('MSIE ');
    if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
    }

    var trident = ua.indexOf('Trident/');
    if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
    }

    var edge = ua.indexOf('Edge/');
    if (edge > 0) {
        // IE 12 => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
    }

    // other browser
    return false;
}//function detectIE() {

/*
    END OF CALLING FUNCTIONS
*/

$(document).on("ready", function (event) {

    $('.loginLink').click(function (e) {
        if (self.location.hostname != 'localhost') {
            self.location = 'Account/Login';
        } else {
            self.location = 'Account/Login';
        }
    });

    $('.registerLink').click(function (e) {
        if (self.location.hostname != 'localhost') {
            self.location = 'Account/Register';
        } else {
            self.location = 'Account/Register';
        }
    });

    $(window).resize(function () {
        console.log('window resize');
        SetLeftHeaderLogoSize();
    });//$(window).resize(function () {


    //add link (interactivity) to tags that have the following classes
    if (self.location.hostname != 'localhost') {
        $('.aboutLink').attr('href', 'Home/About');
        $('.registerLink').attr('href', 'Account/Register');
        $('.loginLink').attr('href', 'Account/Login');
        $('.blogLink').attr('href', 'http://productivityedge.com/?page_id=28');
        $('.homeLink').attr('href', 'Home/Index');
    } else {
        $('.aboutLink').attr('href', 'Home/About');
        $('.registerLink').attr('href', 'Account/Register');
        $('.loginLink').attr('href', 'Account/Login');
        $('.blogLink').attr('href', 'http://productivityedge.com/?page_id=28');
        $('.homeLink').attr('href', 'Home/Index');
    }

    SetLeftHeaderLogoSize(); //set the initial size of the left header logo

});//$(document).on("ready", function (event) {

