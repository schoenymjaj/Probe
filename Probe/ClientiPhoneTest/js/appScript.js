/*
This function executes after the following events
pagebeforechange, pagebeforecreate, pagecreate, 
pageinit, pagebeforeshow, pageshow, pagechange
note: document ready occurs after all these.
*/
$(function () {

    // define the application
    var probeApp = {};

    // start the external panel
    $("[data-role=panel]").panel();
    $("[data-role=panel]").listview();
    $("[data-role=panel]").trigger("create");

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
        console.log('VERSION CONTROL: removed popup for submit');
        root = GetRootUrl();

        var ProbeAPIurl = root + "api/";
        var ProbeMatchReporturl = root + "Reports/PlayerMatchSummary/";
        var ProbeTestReporturl = root + "Reports/PlayerTestDetail/";

        var currentQuestionNbr = 0;
        var NO_ANSWER = -1;
        var result = {};
        var GameState = { "Idle": 0, "Active": 1, "Submitted": 2, "ReadOnly": 3 };
        var SexType = { 'Unknow': 0, 'Male': 1, 'Female': 2 };
        var gameState = GameState.Idle;
        var gamePlayQueueMax = 10;
        var codeFromURL = undefined;

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

                //bind "new game", and "go home events" - misc
                app.BindPageStaticEvents('#home');
                app.BindPageStaticEvents('misc');

                //bind all question page events - question paging buttons, and question answering button
                app.BindPageStaticEvents('#question');

                app.BindPageStaticEvents('#summary');

            }); //$(document).on

            //We needed to do this because mysteriously the page padding was dynamically changing to a value of 2.xxx
            //Don't know why.
            //$(document).on("pagechange", function (event) {
            //    console.log('event pagechange');

            //    switch ($.mobile.pageContainer.pagecontainer("getActivePage").attr('id'))
            //    {
            //        case "home":
            //            $('#home').css("padding-top", "42px");
            //            break;
            //        case "question":
            //            $('#question').css("padding-top", "42px");
            //            break;
            //        case "summary":
            //            $('#summary').css("padding-top", "42px");
            //            break;
            //    }
            //});

        }; //app.bindings = function () {

        /*
        Bind Page Events
        */
        app.BindPageStaticEvents = function (pageSelector) {
            console.log('func app.BindPageStaticEvents');

            switch (pageSelector) {
                case "#home":
                    $('[data-gameplay]').tap(function () {
                        //event.preventDefault();

                        alert('event - nothing is done');
                    });
                    
                    $('#linkToPopup').tap(function () {
                        popupArgs = new PopupArgs();
                        popupArgs.header = 'Info';
                        popupArgs.msg1 = 'Clicked Linked to Popup';
                        popupArgs.msg2 = 'Not to Exciting';
                        app.popUp(popupArgs);
                    });

                    break;
                case "#question":

                    //FYI. jquery would not work with #question as a pre-cursor to #backButton
                    $('#qfooter #backButton').tap(function () {
                        alert('back');
                        $.mobile.changePage('#question');
                    });

                    $('#qfooter #summaryButton').tap(function () {
                        //event.preventDefault();
                        app.SetNavBars(false, true);
                        $.mobile.changePage('#summary');
                    });

                    $('#qfooter #nextButton').tap(function () {
                        //event.preventDefault();
                        alert('next');
                        $.mobile.changePage('#question');
                    });

                    //MNS DEBUG
                    $('#linkToSummary').tap(function () {
                        //event.preventDefault();
                        app.SetNavBars(false, true);
                        $.mobile.changePage('#summary');
                    });
                    $('#linkToQuestion').tap(function () {
                        //event.preventDefault();
                        app.SetNavBars(true, false);
                        $.mobile.changePage('#question');
                    });

                    break;
                case "#summary":

                    $('#submitButton').tap(function () {
                        //event.preventDefault();


                        //MNS - REMOVE POPUP FROM SUBMISSION EQUATION
                        //popupArgs = new PopupArgs();
                        //popupArgs.header = 'Confirmation';
                        //popupArgs.msg1 = 'Are you sure you want to submit the Game \'' + gamePlayData.Name + '\'.';
                        //popupArgs.msg2 = 'You will not be able to edit your answers once you submit.';
                        //popupArgs.btnYesHandler = 'funcSubmitGamePlay';
                        //popupArgs.btnNoHandler = 'back';
                        //popupArgs.btnYesLabel = 'Yes';
                        //popupArgs.btnNoLabel = 'No';
                        //app.popUp(popupArgs);

                        popupArgs = new PopupArgs();
                        popupArgs.header = 'Info';
                        popupArgs.msg1 = 'Submit Button Clicked';
                        popupArgs.msg2 = 'MNS';
                        app.popUp(popupArgs);

                    });

                    break;

                case "misc":

                    //bind all GO HOME events
                    $('[data-icon="home"]').tap(function () {
                        $('#menu').panel("close"); //if menu open

                        app.SetNavBars(false, false);
                        $.mobile.changePage('#home');
                    });

                    //bind all "Add Game" (plus) icons events
                    $("[data-icon='plus'],#newGame").tap(function () {
                        //event.preventDefault();
                        $('#menu').panel("close"); //if menu open


                        //popupArgs = new PopupArgs();
                        //popupArgs.header = 'Info';
                        //popupArgs.msg1 = 'Clicked Plus';
                        //app.popUp(popupArgs);

                        alert('move-question page');
                        app.SetNavBars(true, false);
                        $.mobile.changePage('#question'); //just added MNS 7/27
                    });

                    //bind all "Cancel Game" (plus) icons events
                    $("[data-icon='minus']").tap(function () {
                        //event.preventDefault();

                        popupArgs = new PopupArgs();
                        popupArgs.header = 'Info';
                        popupArgs.msg1 = 'Clicked Minus';
                        popupArgs.msg2 = 'Page will not change';
                        app.popUp(popupArgs);


                    });

                    break;
            }
        };//app.BindPageStaticEvents 

        /*
        Setup home page
        arguments
        initialState = true   //setup for original
                     = false //setup for prompts
        */
        //app.SetHomePageStyle = function (initialState) {
        //    console.log('func app.SetHomePageStyle');

        //    if (initialState) {
        //        $('#home').css('background-image', 'url(./images/bckground/ProbeBackground.jpg)');

        //    } else {
        //        $('#home').css('background-image', 'url(./images/bckground/ProbeBackground-Opacity3.jpg)');
        //    }
        //};

 
        /*
        Setup and display popup
        */
        app.popUp = function (popupArgs) { //(header, msg1, msg2, btnYesHandler, btnNoHandler) {
            console.log('func app.popUp');

            /*
            header = header of popup
            msg1 = first cell of a one column table
            msg2 = second cell of a one column table (if null, it won't display)
            btnYesHandler = the function that is called when yes button is clicked //if back then will use data-rel="back"
            btnNoHandler = the function that is called when no button is clicked //if back then will use data-rel="back"
            btnYesLabel = label of yes button //default is OK
            btnNoLabel = label of no button //default is No

            Note: if btnYesHandler and buttonNoHandler are both null, then the yes button will be displayed with data-rel="back"

            Note: the function handlers should have a convention fnc<name of function) to be written outside of the app, and theire
            arguments are button, theApp

            Note: within handler, the popup can be closed by:
            $('#popupMsg').enhanceWithin().popup().popup("close", { transition: "slide" });

            */


            /*
            Set visibility of buttons and event handlers based on the eventhandler property arg
            */
            $('#popupMsgYesBtn').hide();
            $('#popupMsgYesBtn').attr('data-rel', '');
            $('#popupMsgNoBtn').hide();
            $('#popupMsgNoBtn').attr('data-rel', '');
            if (popupArgs.btnYesHandler != null) {
                if (popupArgs.btnYesHandler == 'back') {
                    $('#popupMsgYesBtn').attr('data-rel', 'back');
                } else {
                    //BIG BUG: event handler - gets called for each time the app.popup is called in a browser session. Can't seem to 
                    //find a fix for this.
                    $('#popupMsgYesBtn').tap(function () {
                        window[popupArgs.btnYesHandler](this, app);
                    });
                }
                $('#popupMsgYesBtn').show();
            }//if (popupArgs.btnYesHandler != null) {

            if (popupArgs.btnNoHandler != null) {
                $('#popupMsgNoBtn').show();
                if (popupArgs.btnNoHandler == 'back') {
                    $('#popupMsgNoBtn').attr('data-rel', 'back');
                } else {
                    //event handler
                    $('#popupMsgNoBtn').tap(function () {
                        window[popupArgs.btnNoHandler](this, app);
                    });
                }
            }// if (popupArgs.btnNoHandler != null) {

            if (popupArgs.btnYesHandler == null && popupArgs.btnNoHandler == null) {
                $('#popupMsgYesBtn').attr('data-rel', 'back')
                $('#popupMsgYesBtn').show();
            }

            /*
            Set text of buttons
            */
            if (popupArgs.btnYesLabel != null) {
                $('#popupMsgYesBtn').text(popupArgs.btnYesLabel);
            }
            if (popupArgs.btnNoLabel != null) {
                $('#popupMsgNoBtn').text(popupArgs.btnNoLabel);
            }


            /*
            Set the the content of the popup
            */
            contentHtml = '<table>' +
            '<tr><td>' + popupArgs.msg1 + '</td></tr>' +
            '<tr><td>' + ((popupArgs.msg2 != null) ? popupArgs.msg2 : "") + '</tr></td>' +
            '</table>';     
            $('#popMsgHeader').html(popupArgs.header); //set header
            $('#popupMsgContent').html(contentHtml); //set content

            //display popup
            $('#popupMsg').enhanceWithin().popup().popup("open", { transition: "fade" }); 
            $('#popupMsg').enhanceWithin().popup().popup("open", { transition: "fade" });
        };

        /*
        Set Nav Bars
        qNavInd = true or false (true - show question nav bar)
        sNavIdn = true or false (true - show summary nav bar)
        */
        app.SetNavBars = function (qNavInd, sNavInd) {
            console.log('func app.SetNavBars qNavInd:' + qNavInd + ' sNavInd:' + sNavInd);

            if (qNavInd) {
                $('#qfooter nav').navbar().removeClass('ui-fixed-hidden'); //shouldn't have to do this, but remove this to make sure
                $('#qfooter nav').navbar().show()
            } else {
                $('#qfooter nav').navbar().hide()
            }

            if (sNavInd) {
                $('#sfooter nav').navbar().removeClass('ui-fixed-hidden'); //shouldn't have to do this, but remove this to make sure
                $('#sfooter nav').navbar().show()
            } else {
                $('#sfooter nav').navbar().hide()
            }

        };//app.SetNavBars 

    //initialize the Prob app
    app.init();


})(probeApp); //app

});



