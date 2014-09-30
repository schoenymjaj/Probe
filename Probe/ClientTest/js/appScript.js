/*
This function executes after the following events
pagebeforechange, pagebeforecreate, pagecreate, 
pageinit, pagebeforeshow, pageshow, pagechange
note: document ready occurs after all these.
*/
$(function () {

    // define the application
    var probeApp = {};

    (function (app) {
        console.log('func app');
        /* Localstorage
        localStorage["GamePlay"]
        localStorage["Result"]
        localStorage["GameConfig"]
        localStorage["GamePlayQueueList"]
        localStorage["GamePlayQueue""]
        */

        /*
        Globals
        */
        alert('VERSION CONTROL: Client Test Version 1.01');


        app.init = function () {
            //this occurs after document is ready (runs once)
            console.log('func app.init');
            app.bindings();
            //app.checkForStorage();
        };

        app.bindings = function () {

            /*
            document ready event
            Start the XMLHttpRequest (download xml and jason files) 
            when document ready event is triggered. Store documents in localStorage
            */
            $(document).on("ready", function (event) {  //jquery document ready event gets you jquery mobile styles, and data rendered
                console.log('event doc ready');

                //override the console.log if production (disable console)
                $(function () {
                    if ($('body').data('env') == 'production') {
                        console.log = function () { };
                    }
                });

            }); //$(document).on


            //We needed to do this because mysteriously the page padding was dynamically changing to a value of 2.xxx
            //Don't know why.
            $(document).on("pagechange pagebeforechange popupafteropen popupafterclose resize", function (event) {
                console.log('event pagechange-pagebeforechange popupafteropen popupafterclose resize');
            });

            $(document).on("pagecontainerchange", function (event) {
                console.log('pagecontainerchange');
            });

            $(document).on("touchend", function (event) {
                console.log('touchend no default,propagation');
                event.preventDefault();
            //    event.stopPropagation();
            //    //alert('touchend');
            });

            $('#buttonOnHome').click(function (event) {
                $(":mobile-pagecontainer").pagecontainer('change', '#page2');
            });

            $('#buttonOnPage2').click(function (event) {
                $(":mobile-pagecontainer").pagecontainer('change', '#home');
            });

            $('.summaryButton').click(function (event) {
                $(":mobile-pagecontainer").pagecontainer('change', '#home');
            });

            $('.submitButton').click(function (event) {
                $(":mobile-pagecontainer").pagecontainer('change', '#page2');
            });


        }; //app.bindings = function () {


    //initialize the Prob app
    app.init();


})(probeApp); //app

});



