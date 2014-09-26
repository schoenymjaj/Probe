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
        alert('VERSION CONTROL: Client Test Version 1.25');
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

                //determine if game play code has been feed from a query string parm
                codeFromURL = undefined;
                if (QryStr('code') != undefined) {
                    if (QryStr('code') != "") {
                        codeFromURL = QryStr('code')
                    }
                }

                if (codeFromURL == undefined) {
                        app.SetHomePageInitialDisplay();
                }


                //bind "new game", and "go home events" - misc
                app.BindPageStaticEvents('misc');

                //bind all question page events - question paging buttons, and question answering button
                app.BindPageStaticEvents('#question');

                app.BindPageStaticEvents('#summary');

                //We need to check if a code was passed through the query string parms
                if (codeFromURL != undefined) {
                    app.SetHomePageStyle(false);
                    app.GetGamePlayServer(codeFromURL);
                }


            }); //$(document).on

            //We needed to do this because mysteriously the page padding was dynamically changing to a value of 2.xxx
            //Don't know why.
            $(document).on("pagechange", function (event) {
                console.log('event pagechange');

                switch ($.mobile.pageContainer.pagecontainer("getActivePage").attr('id'))
                {
                    case "home":
                        $('#home').css("padding-top", "42px");
                        break;
                    case "question":
                        $('#question').css("padding-top", "42px");
                        break;
                    case "summary":
                        $('#summary').css("padding-top", "42px");
                        break;
                }
            });

            /*
            pageinit on all pages - bind on pagebeforeshow event (all pages/w data-role page) - Set the Nav and Toolbars based on the page showing
            */
            $(document).on('pageinit', function () {
                console.log('event doc pageinit');

                $(document).on("pagebeforeshow", "[data-role='page']", function () {
                    //app.setNavAndToolBars($(this)); //MNS
                });

            });

        }; //app.bindings = function () {

        /*
        Set Homepage Initial Display - Listview of active game and previous games played
        */
        app.SetHomePageInitialDisplay = function () {
            console.log('func app.SetHomePageInitialDisplay');
            gamePlayListQueue = app.GetGamePlayListQueueLocalStorage();

            listViewHtml = '';

            if (app.IsGameInProgress() || gamePlayListQueue.length > 0) {
                listViewHtml += '<ul id="gameList" data-role="listview" data-inset="true">';

                if (app.IsGameInProgress()) {
                    $("[data-icon='plus']").addClass('ui-disabled');
                    $("[data-icon='minus']").removeClass('ui-disabled');
                    app.SetHomePageStyle(false);

                    gamePlayData = app.GetGamePlayLocalStorage();
                    result = app.GetResultLocalStorage();

                    listViewHtml += '<li data-role="list-divider">Active Game</li>' +
                                    '<li data-icon="star" data-gameplay="active"><a href="#">' +
                                     gamePlayData.Name +
                                     '</a></li>';
                } else {
                    listViewHtml += '<li data-role="list-divider">No Active Game</li>'
                }//if (app.IsGameInProgress()) {


                if (gamePlayListQueue.length > 0) {
                    listViewHtml += '<li data-role="list-divider">Submitted Games</li>';
                }
                gamePlayListQueue.forEach(function (value, index, ar) {
                    listViewHtml += '<li data-icon="star" data-gameplay="submitted"' +
                                    ' data-index="' + index + '"' +
                                    ' data-gameplayid="' + value.GamePlayId + '"' +
                                    '><a href="#">' +
                                     value.Name +
                                     '</a></li>';
                });


                listViewHtml += '</ul>';

            }// if (app.IsGameInProgress() || gamePlayListQueue > 0) {

            $('#homePageContent').html(listViewHtml);
            $('#gameList').listview().listview("refresh").trigger("create");
            $('#home').trigger('create');

            app.BindPageStaticEvents("#home");

        };//app.SetHomePageInitialDisplay

        /*
        Update the home page with a text box to enter game play code
        */
        app.SetGamePlayCodePrompt = function () {
            console.log('func app.SetGamePlayCodePrompt');

            //Will fill in code if app started with code query string parm
            promptforCodeHtml =
                '<div style="margin-top: 10px"><label for="code">Game Code</label>' +
                '<input name="code" id="gameCode" type="text" ' +
                'value="' +
                ((codeFromURL != undefined) ? codeFromURL : "") +
                '" ' +
                'data-clear-btn="true">' +
                '<table><tr>' +
                '<td><button id="callGetPlays" class="ui-btn" data-icon="action">Find Game</button></td>' +
                '<td><button id="cancelGamePlay" class="ui-btn" data-icon="action">Cancel</button></td>' +
                '</tr></table></div>';


            $('#homePageContent').html(promptforCodeHtml);
            $('#homePageContent').trigger("create");

            $('#callGetPlays').click(function (event) {
                gameCode = $('#gameCode').val();
                if (gameCode.length > 0) { //check to see that a game code was entered
                    app.GetGamePlayServer($('#gameCode').val());
                } else {
                    popupArgs = new PopupArgs();
                    popupArgs.header = 'Error';
                    popupArgs.msg1 = 'The game code cannot be blank.';
                    popupArgs.msg2 = 'Please enter a game code.';
                    app.popUp(popupArgs);
                }
            });

            $('#cancelGamePlay').click(function (event) {
                app.CancelGame();
            });

        } //app.SetGamePlayCodePrompt

        /*
        Get GamePlay from Probe Server
        FYI. The GetJSON call to server is asynchronous. We wait for a good response, then
        call the next display (prompt for player info)
        */
        app.GetGamePlayServer = function (gameCode) {
            console.log('func app.GetGamePlayServer MNS');

            url = ProbeAPIurl + 'GamePlays/GetGamePlay/' + gameCode;
            $.mobile.loading('show'); //to show the spinner

            console.log('func app.GetGamePlayServer AJAX url:' + url);
            $.getJSON(url)
                .done(function (gamePlayData) {
                    console.log('return GetGamePlay success');

                    // On success, 'data' contains a GamePlay JSON object
                    if (gamePlayData.errorid == undefined) {
                        //SUCCESS
                        //We've got the game play data; we also need the game configuration
                        url = ProbeAPIurl + 'GameConfigurations/GetGameConfigurationByGame/' + gamePlayData.GameId;
                        console.log('func app.GetGamePlayServer AJAX url:' + url);
                        $.getJSON(url)
                            .done(function (gameConfig) {
                                console.log('return GetGameConfiguration success');

                                $.mobile.loading('hide'); //to hide the spinner

                                app.PutGameConfigLocalStorage(gameConfig);
                                if (gameConfig = {}) {

                                    if (!app.IsGameSubmitted(gamePlayData.Id) || !$.parseJSON(app.GetConfigValue('DeviceCanPlayGameOnlyOnce'))) {
                                        app.InitalizeGamePlay(gamePlayData);
                                        app.SetGamePlayPlayerPrompt(); //SUCCESS - NEXT STEP IS FOR PLAYER TO ENTER PLAYER INFO
                                    } else {
                                        popupArgs = new PopupArgs();
                                        popupArgs.header = 'Error';
                                        popupArgs.msg1 = 'The Game \'' + gamePlayData.Name + '\' has already been submitted.'
                                        popupArgs.msg2 = 'A device cannot submit the same game twice for this game type.';
                                        app.popUp(popupArgs);
                                    }//if (!app.IsGameSubmitted(gamePlayData.Id))

                                } else {
                                    popupArgs = new PopupArgs();
                                    popupArgs.header = "Error";
                                    popupArgs.msg1 = 'Configuration could not be found for Game \'' + gamePlayData.Name + '\'';
                                    app.popUp(popupArgs);

                                }//if (gameConfig = {})

                            })
                            .fail(function (jqxhr, textStatus, error) {
                                console.log('return GetGameConfiguration fail');
                                $.mobile.loading('hide'); //to hide the spinner

                                probeError = error;
                                if (probeError == "") {
                                    probeError = "The Probe web server could not be found. There may be connectivity issues."
                                }
                                var err = textStatus + ", " + probeError;
                                popupArgs = new PopupArgs();
                                popupArgs.header = "Error";
                                popupArgs.msg1 = 'Request Failed:' + err;
                                app.popUp(popupArgs);

                            });
                        
                    } else {
                        //THERE WAS A PROBE BUSINESS ERROR
                        $.mobile.loading('hide'); //to hide the spinner
                        errorMessage = gamePlayData.errormessage;
                        switch (gamePlayData.errorid) {
                            case 1:
                                errorMessage = 'There is no game found for the code entered.';
                                break;
                            case 2:
                                errorMessage = 'The game found for the entered code is no longer active.';
                                break;
                            default:
                                errorMessage = gamePlayData.errormessage;
                                break;
                        }
                        popupArgs = new PopupArgs();
                        popupArgs.header = 'Error';
                        popupArgs.msg1 = errorMessage;
                        popupArgs.msg2 = 'Please enter the correct code.';
                        app.popUp(popupArgs);
                    }

                }) //done
              .fail(function (jqxhr, textStatus, error) {
                  console.log('return GetGamePlay fail');
                  $.mobile.loading('hide'); //to hide the spinner

                  probeError = error;
                  if (probeError == "") {
                      probeError = "The Probe web server could not be found. There may be connectivity issues."
                  }
                  var err = textStatus + ", " + probeError;
                  popupArgs = new PopupArgs();
                  popupArgs.header = "Error";
                  popupArgs.msg1 = 'Request Failed:' + err;
                  app.popUp(popupArgs);

              }); //fail

        };//app.GetGamePlayServer

        /*
        Get GamePlayStatus from Probe Server
        Will record the ClientReportAccess boolean in result["ClientReportAccess"]
        FYI. The GetJSON call to server is asynchronous. We wait for a good response, then
        call the next display (read-only player info)

        */
        app.GetGamePlayStatusServer = function (gamePlayId) {
            console.log('func app.GetGamePlayStatusServer');

            url = ProbeAPIurl + 'GamePlays/GetGamePlayById/' + gamePlayId;

            $.mobile.loading('show'); //to show the spinner

            console.log('func app.GetGamePlayStatusServer AJAX url:' + url);
            $.getJSON(url)
              .done(function (gamePlayStatusData) {
                  console.log('return GetGamePlayStatus success');

                  $.mobile.loading('hide'); //to hide the spinner
                  // On success, 'data' contains a GamePlay(only one level) JSON object
                  if (gamePlayStatusData.errorid == undefined) {
                      //SUCCESS
                      result = app.GetResultLocalStorage();
                      result["ClientReportAccess"] = gamePlayStatusData.ClientReportAccess;
                      app.PutResultLocalStorage(result);

                      //set the home page for read-only view
                      gameState = GameState.ReadOnly;
                      app.SetHomePageStyle(false); //set backgrounded faded
                      app.ResumeGame(GameState.ReadOnly); //resume game read-only

                  } else {
                      //THERE WAS A PROBE BUSINESS ERROR
                      $.mobile.loading('hide'); //to hide the spinner
                      errorMessage = gamePlayStatusData.errormessage;
                      switch (gamePlayStatusData.errorid) {
                          case 1:
                              errorMessage = 'There is no game found for the id entered.';
                              break;
                          default:
                              errorMessage = gamePlayStatusData.errormessage;
                              break;
                      }
                      popupArgs = new PopupArgs();
                      popupArgs.header = 'Error';
                      popupArgs.msg1 = errorMessage;
                      app.popUp(popupArgs);
                  }

              }) //done
              .fail(function (jqxhr, textStatus, error) {
                  console.log('return GetGamePlayStatus fail');
                  $.mobile.loading('hide'); //to hide the spinner

                  probeError = error;
                  if (probeError == "") {
                      probeError = "The Probe web server could not be found. There may be connectivity issues."
                  }
                  var err = textStatus + ", " + probeError;
                  popupArgs = new PopupArgs();
                  popupArgs.header = "Error";
                  popupArgs.msg1 = 'Request Failed:' + err;
                  app.popUp(popupArgs);

              }); //fail
        };//app.GetGamePlayStatusServer

        /*
        Submit Player and GamePlay Answers for Player
        */
        app.PostGamePlayAnswersServer = function () {
            console.log('app.PostGamePlayAnswersServer');

            returnErrMsg = null;

            $.mobile.loading('show'); //to show the spinner

            //create player object for POST
            player = {};
            player["GamePlayId"] = result["GamePlayId"];
            player["FirstName"] = result["FirstName"];
            player["NickName"] = result["NickName"];
            (result["LastName"] != {}) ? player["LastName"] : result["LastName"]; //curently last name will always be empty 8/1/14
            player["Sex"] = result["Sex"];

            //mns debug
            console.log('player["GamePlayId"]=' + player["GamePlayId"]);
            console.log('result["GamePlayId"]=' + result["GamePlayId"]);

            url = ProbeAPIurl + 'Players/PostPlayer';
            console.log('func app.PostGamePlayAnswersServer AJAX url:' + url);
            app.ajaxHelper(url, 'POST', player)
                .done(function (playerDTO) {
                    console.log('return POSTPlayer success');
                    // On success, 'playerDTO' contains a Player object
                    if (playerDTO.errorid == undefined) {
                        //SUCCESS
                        gamePlayData = app.GetGamePlayLocalStorage();
                        result = app.GetResultLocalStorage();
                        result["PlayerId"] = playerDTO.Id; //set player id from probe DB (just added)
                        app.PutResultLocalStorage(result);

                        //create gamePlayAnswers array object for POST
                        var gamePlayAnswers = new Array();
                        for (i = 0; i < gamePlayData.GameQuestions.length; i++) {
                            gamePlayAnswers[i] = {};
                            gamePlayAnswers[i]["PlayerId"] = playerDTO.Id;
                            gamePlayAnswers[i]["ChoiceId"] = result.GameQuestions[i]["SelChoiceId"];
                        }

                        url = ProbeAPIurl + 'GamePlayAnswers/PostGamePlayAnswers';
                    } else
                    {
                        //THERE WAS A PROBE BUSINESS ERROR
                        $.mobile.loading('hide'); //to hide the spinner

                        switch (playerDTO.errorid) {
                            case 3:
                                errorMessage = 'Your player name (first name + nickname) is already in use. ' + 
                                    'Please enter another nickname to be unique for the game. Use Menu => Home option (upper right icon). Select Active Game and modify your nickname';
                                break;
                            default:
                                errorMessage = playerDTO.errormessage;
                                break;
                        }
                        return returnErrMsg = errorMessage;
                    }//if (playerDTO.errorid == undefined) {

                    console.log('func app.PostGamePlayAnswersServer AJAX success url:' + url);
                    app.ajaxHelper(url, 'POST', gamePlayAnswers)
                        .done(function (item) {
                            console.log('return POSTGamePlayAnswers success');

                            // On success, 'gamePlayAnswers' contains a GamePlayAnswers object
                            if (gamePlayAnswers.errorid == undefined) {
                                //SUCCESS

                                returnErrMsg = null; //successful return
                                $.mobile.loading('hide'); //to hide the spinner
                            } else {
                                //THERE WAS A PROBE BUSINESS ERROR
                                $.mobile.loading('hide'); //to hide the spinner
                                return returnErrMsg = gamePlayAnswers.errormessage + '(Error #: ' + gamePlayAnswers.errorid + ')';

                            }//if (gamePlayAnswers.errorid == undefined) {

                        })
                        .fail(function (jqxhr, textStatus, error) {
                            console.log('return POSTGamePlayAnswers fail');

                            $.mobile.loading('hide'); //to hide the spinner
                            probeError = error;
                            if (probeError == "") {
                                probeError = "The Probe web server could not be found. There may be connectivity issues."
                            }
                            return returnErrMsg = textStatus + ", " + probeError;
                        });//fail for POST GamePlayAnswers
                })//done for POST Player
                .fail(function (jqxhr, textStatus, error) {
                    console.log('return POSTPlayer fail');

                    $.mobile.loading('hide'); //to hide the spinner
                    probeError = error;
                    if (probeError == "") {
                        probeError = "The Probe web server could not be found. There may be connectivity issues."
                    }
                    return returnErrMsg = textStatus + ", " + probeError;
                }); //fail for POST Player

            return returnErrMsg;
        };//app.PostGamePlayAnswersServer
        
        /*
        Update the home page with the game play information and a prompt for first name and nick name 
        before starting the game
        */
        app.SetGamePlayPlayerPrompt = function () {
            console.log('func app.SetGamePlayPlayerPrompt');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            promptforPlayerHtml =
                '<div style="margin-top: 10px; font-weight:bold">' +
                '(' + gamePlayData.GameType + ') Game Type<br/><br/>' +
                '<label for="gpName">Game Name</label>' +
                '<textarea name="gpName" id="gpName" class="ui-disabled">' + gamePlayData.Name + '</textarea>' +
                '<label for="gpDesc">Game Description</label>' +
                '<textarea name="gpDesc" id="gpDesc" class="ui-disabled">' + gamePlayData.Description + '</textarea>' +
                '<label for="C">First Name</label>' +
                '<input name="firstName" id="firstName" type="text" value="" data-clear-btn="true">' +
                '<label for="nickName">Nick Name</label>' +
                '<input name="nickName" id="nickName" type="text" value="" data-clear-btn="true">' +
                '<fieldset data-role="controlgroup" data-type="horizontal">' +
                '<legend>Sex</legend>' +
                '<input name="sex" id="sex-male" type="radio" checked="checked" value="on">' +
                '<label for="sex-male">Male</label>' +
                '<input name="sex" id="sex-female" type="radio">' +
                '<label for="sex-female">Female</label>' +
                '</fieldset>' +
                '<table><tr>' +
                '<td><button id="startGamePlay" class="ui-btn" data-icon="action">Start Game</button></td>' +
                '<td><button id="cancelGamePlay" class="ui-btn" data-icon="action">Cancel</button></td>' +
                '<td><button id="reportGamePlay" class="ui-btn" data-icon="action">Reports</button></td>' +
                '</tr></table></div>';

            $('#homePageContent').html(promptforPlayerHtml);

            /*
            Dynamically update the Player Prompt based on the GameState
            */
            $('#firstName').removeClass('ui-disabled');
            $('#nickName').removeClass('ui-disabled');
            $('#reportGamePlay').hide();
            if (gameState != GameState.Idle) {
                $("input[name='firstName']").attr('value', result.FirstName);
                $("input[name='nickName']").attr('value', result.NickName);
                if (result.Sex == SexType.Male) {
                    $('#sex-male').attr("checked", true);
                    $('#sex-female').attr("checked", false);
                } else {
                    $('#sex-male').attr("checked", false);
                    $('#sex-female').attr("checked", true);
                }

                if (gameState == GameState.ReadOnly) {
                    $('#firstName').addClass('ui-disabled')
                    $('#nickName').addClass('ui-disabled')

                    /*
                    Display the Report Button - it will only be enabled if the GamePlay.ClientReportAccess 
                    field is TRUE.
                    */
                    if (result.ClientReportAccess) {
                        $('#reportGamePlay').removeClass('ui-disabled')
                    } else {
                        $('#reportGamePlay').addClass('ui-disabled')

                    }
                    $('#reportGamePlay').show();

                    //$('[data-role="controlgroup"]').addClass('ui-disabled') //works but the legend is also greyed out
                    $('#startGamePlay').text('View Game');
                } else {
                    $('#startGamePlay').text('Resume Game');
                }

                //$('#cancelGamePlay').hide(); //if game is not idle; we don't want to give the user the cancel ability here (too easy)
            }//if (gameState != GameState.Idle)

            $('#homePageContent').trigger("create");
            $('#firstName').focus(); //put the focus on the firstname text input

            //toggle the sex radio boxes
            $('input[name="sex"]').on('change', function () {
                if ($(this).attr("id") == "sex-male") {
                    $('#sex-male').attr("checked", true);
                    $('#sex-female').attr("checked", false);
                } else {
                    $('#sex-male').attr("checked", false);
                    $('#sex-female').attr("checked", true);
                }
            });

            //bind event handlers to the start and cancel buttons
            $('#startGamePlay').click(function (event) {

                //error handling 
                if ($('#firstName').val().length < 3 ||
                    $('#firstName').val().length > 10 ||
                    $('#firstName').val().indexOf(" ") != -1)
                    {
                    popupArgs = new PopupArgs();
                    popupArgs.header = 'Error';
                    popupArgs.msg1 = 'The first name must be between 3 and 10 characters and contain no spaces.';
                    popupArgs.msg2 = 'Please enter a first name again.';
                    app.popUp(popupArgs);
                    return;
                }

                if ($('#nickName').val().length < 3 ||
                    $('#nickName').val().length > 10 ||
                    $('#nickName').val().indexOf(" ") != -1)
                    {
                    popupArgs = new PopupArgs();
                    popupArgs.header = 'Error';
                    popupArgs.msg1 = 'The nick name must be between 3 and 10 characters and contain no spaces.';
                    popupArgs.msg2 = 'Please enter a nick name again.';
                    app.popUp(popupArgs);
                    return;
                }

                result = app.GetResultLocalStorage();
                result["FirstName"] = $('#firstName').val()
                result["NickName"] = $('#nickName').val()
                if ($('#sex-male').attr("checked") == "checked") {
                    result["Sex"] = SexType.Male;
                } else {
                    result["Sex"] = SexType.Female;
                }

                app.PutResultLocalStorage(result);

                app.StartGame(0);

            });

            $('#cancelGamePlay').click(function (event) {
                app.CancelGame();
            });

            $('#reportGamePlay').click(function (event) {
                app.DisplayReportPage();
            });

        };

        /*
        Initialize the GamePlay data structure, add answer property to question, and put 
        in local storage
        */
        app.InitalizeGamePlay = function (JSONdata) {
            console.log('func app.InitalizeGamePlay');

            result["GamePlayId"] = JSONdata.Id;
            result["FirstName"] = {};
            result["LastName"] = {};
            result["NickName"] = {};
            result["Sex"] = SexType.Male;
            result["GameQuestions"] = new Array();
            result["DirtyFlag"] = true;

            JSONdata.GameQuestions.forEach(function (value, index, ar) {
                //value.Question["Answer"] = NO_ANSWER;  //NOT NEEDED FOR 
                result["GameQuestions"][index] = {};
                result["GameQuestions"][index]["QuestionId"] = value.Question.$id;
                result["GameQuestions"][index]["SelChoiceId"] = NO_ANSWER;
            });

            app.PutGamePlayLocalStorage(JSONdata);
            app.PutResultLocalStorage(result); //hold all the results

        };//app.InitalizeGamePlay 

        /*
        Create a test page of the api/GetPlays/{code} dump - accepts the JSON
        data from an ajax call to the Probe API and parse it, and create a test page
        */
        app.CreateGamePlayTestPage = function (JSONdata) {
            console.log('func app.CreateGamePlayTestPage');

            testHtml = 'GamePlayName:' + JSONdata.Name + '<br/>' +
           'GameName:' + JSONdata.GameName + '<br/>' +
           'Code:' + JSONdata.Code + '<br/>';

            JSONdata.GameQuestions.forEach(function (value, index, ar) {
                testHtml += 'Question ' + index + 1 + ':' + value.Question.Text + '<br/>';

                value.Question.Choices.forEach(function (value, index, ar) {
                    testHtml += 'Choice ' + index + 1 + ':' + value.Text + '<br/>';
                });
            });

            page = '<section id="testpage" data-role="page" data-title="Test Page" data-theme="a">' +
                   '<div data-role="header">' +
                    '<h1>Test Page</h1>' +
                    '</div>' +
                    '<article data-role="content">' +
                    testHtml +
                    '</article></section>';

            var newPage = $(page);

            newPage.appendTo($.mobile.pageContainer);
            $.mobile.changePage(newPage);

        }; //app.CreateGetPlayTestPage

        /*
        Render the question page for the GameQuestion[questionNbr] in the GamePlay dataset
        */
        app.SetQuestionPage = function (questionNbr) {
            console.log('func app.SetQuestionPage');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            question = gamePlayData.GameQuestions[questionNbr].Question;
            questionText = question.Text;

            fieldset = '<fieldset data-role="controlgroup">';
            question.Choices.forEach(function (value, index, ar) {
                choiceText = value.Text;
                choiceName = value.Name;
                selectChoiceId = value.Id;

                checkedStr = "";
                if (result["GameQuestions"][questionNbr]["SelChoiceId"] == selectChoiceId) checkedStr = ' checked';

                fieldset +=
                '<input name="choice" id="choice-' + selectChoiceId + '" type="radio" data-theme="a"' + checkedStr + '>'

                fieldset +=
                '<label for="choice-' + selectChoiceId + '" data-theme="a">' + choiceText + '</label>';
            });
            fieldset += '</fieldset>'

            $('#questionText h2').html(questionText + '?');
            $('#choiceListLegend').html('Question #' + (questionNbr + 1) + ' of ' + app.NbrQuestions());
            $('#choiceList').html(fieldset);

            if (gameState != GameState.ReadOnly)
            {
                $("input[name ='choice']").checkboxradio().checkboxradio('enable').trigger("create");
                $('.submitButton').removeClass('ui-disabled');

            } else {
                $("input[name ='choice']").checkboxradio().checkboxradio('disable').trigger("create");
                $('.submitButton').addClass('ui-disabled');
            }

            $('#question').trigger('create');

            $("input[name ='choice']").on('change', function () {

                radioButtonSelectedID = $('input[name="choice"]:checked').attr('id'); //id of the radio box

                gamePlayData = app.GetGamePlayLocalStorage();
                //set choice number of answer
                result["GameQuestions"][currentQuestionNbr]["SelChoiceId"] = radioButtonSelectedID.substr(7, radioButtonSelectedID.length - 6);
                app.PutResultLocalStorage(result);

            }); //$("input[name ='choice']").on('change', function () {

            $.mobile.changePage('#question');

        }; //app.SetQuestionPage

        /*
        Render the summary page
        */
        app.SetSummaryPage = function () {
            console.log('func app.SetSummaryPage');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            summaryText = 'Questions - ' + app.NbrQuestionsAnswered() + ' out of ' + app.NbrQuestions() + ' answered'


            listViewHtml = '<ul data-role="listview" data-inset="true">';

            gamePlayData.GameQuestions.forEach(function (value, index, ar) {

                listViewHtml += '<li' +
                ((result.GameQuestions[index]["SelChoiceId"] == NO_ANSWER) ? ' data-icon="alert" ' : ' data-icon="check" ') +
                ' data-qnum=' + index + '>' +
                '<a href="#">' +
                (index + 1) + '. ' +
                ((value.Question.Text.length <= 30) ? value.Question.Text : value.Question.Text.substr(0, 30))
                + '...</a></li>';

            });
            listViewHtml += '</ul>';

            $('#summaryText h2').html(summaryText);
            $('#questionList').html(listViewHtml);

            $('#summary').trigger('create');

            //don't need the refresh. In fact is creates a mysterious scroll bar
            //$('#questionList').listview().listview("refresh").trigger("create"); 

            /*
            user can't submit a game play unless they have completed all the questions
            */
            if (app.IsAllQuestionsAnswered() && gameState != GameState.ReadOnly) {
                if ($('.submitButton').hasClass('ui-disabled')) {
                    $('.submitButton').removeClass('ui-disabled');
                }
            } else {
                if (!$('.submitButton').hasClass('ui-disabled')) {
                    $('.submitButton').addClass('ui-disabled');
                }
            }



            //setup event handler for summary page listview to return to a specific question
            $('[data-qnum]').click(function (event) {
                app.SetNavBars(true, false);
                currentQuestionNbr = parseInt(this.attributes["data-qnum"].value);
                app.SetQuestionPage(currentQuestionNbr);
            });

            $.mobile.changePage('#summary');

        };//app.SetSummaryPage

        /*
        Bind Page Events
        */
        app.BindPageStaticEvents = function (pageSelector) {
            console.log('func app.BindPageStaticEvents');

            switch (pageSelector) {
                case "#home":
                    $('[data-gameplay]').click(function (event) {

                        if(this.attributes["data-gameplay"].value == 'active') { //is it the active game selected
                            app.ResumeGame(GameState.Active);
                        } else if(this.attributes["data-gameplay"].value == 'submitted') {
                            
                            //copy gameplay out of queue into current gameplay (even though it's read-only)
                            index = this.attributes["data-index"].value;
                            gamePlayQueue = app.GetGamePlayQueueLocalStorage();
                            gamePlayData = gamePlayQueue[index].GamePlay;
                            result = gamePlayQueue[index].Result;
                            app.PutGamePlayLocalStorage(gamePlayData);
                            app.PutResultLocalStorage(result);

                            //will get GamePlay status info from server and then resume game (read-only)
                            app.GetGamePlayStatusServer(result.GamePlayId);

                        }
                    });

                    break;
                case "#question":

                    //FYI. jquery would not work with #question as a pre-cursor to #backButton
                    //$('#qfooter #backButton').click(function (event) { MNS DEBUG
                    $('#backButton').click(function (event) {
                            (currentQuestionNbr == 0) ? currentQuestionNbr = result.GameQuestions.length - 1 : currentQuestionNbr--;
                        app.SetQuestionPage(currentQuestionNbr);
                    });

                    $('.summaryButton').click(function (event) {
                        app.SetNavBars(false, true);
                        app.SetSummaryPage();
                    });

                    //$('#qfooter #nextButton').click(function (event) { //MNS DEBUG
                    $('#nextButton').click(function (event) {
                            (currentQuestionNbr == result.GameQuestions.length - 1) ? currentQuestionNbr = 0 : currentQuestionNbr++;
                        app.SetQuestionPage(currentQuestionNbr);
                    });

                    break;
                case "#summary":

                    $('.submitButton').click(function (event) {

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

                        //MNS - ALL CODE BELOW IS DEBUG TO REMOVE POPUPS FROM THE EQUATION FOR IPHONE DEBUG TEST
                        result = app.GetResultLocalStorage();
                        console.log('func submitButton.click - GamePlayId:' + result["GamePlayId"]);
                        returnErrMsg = app.PostGamePlayAnswersServer();
                        console.log('completed app.PostGamePlayAnswersServer' );
                        if (returnErrMsg == null) {
                            app.SubmitSuccess();
                            console.log('success - all done');
                        }
                        //MNS - ALL CODE TO HERE



                    });

                    break;

                case "misc":

                    //bind all GO HOME events
                    $('[data-icon="home"]').click(function (event) {
                        $('#menu').panel("close"); //if menu open

                        app.SetNavBars(false, false);
                        app.SetHomePageStyle(false);
                        app.SetHomePageInitialDisplay();
                        $.mobile.changePage('#home');
                    });

                    //bind all "Add Game" (plus) icons events
                    $("[data-icon='plus'],#newGame").click(function (event) {
                        $('#menu').panel("close"); //if menu open

                        if (app.IsGameInProgress()) {
                            gamePlayData = app.GetGamePlayLocalStorage();
                            popupArgs = new PopupArgs();
                            popupArgs.header = 'Error';
                            popupArgs.msg1 = 'There is a Game \'' + gamePlayData.Name + '\' that is in progress';
                            popupArgs.msg2 = 'You must cancel this game first to start a new game.';
                            app.popUp(popupArgs);
                            return;
                        }

                        $("[data-icon='plus']").addClass('ui-disabled');
                        $("[data-icon='minus']").removeClass('ui-disabled');
                        app.SetNavBars(false, false);
                        app.SetHomePageStyle(false);
                        app.SetGamePlayCodePrompt();
                        gameState = GameState.Idle; //just added MNS 7/27
                        $.mobile.changePage('#home'); //just added MNS 7/27
                    });

                    //bind all "Cancel Game" (plus) icons events
                    $("[data-icon='minus']").click(function (event) {

                        if (!app.IsGameInProgress())
                        {
                            popupArgs = new PopupArgs();
                            popupArgs.header = 'Error';
                            popupArgs.msg1 = 'There is no Game in progress';
                            popupArgs.msg2 = undefined;
                            app.popUp(popupArgs);
                            return
                        } else {
                            gamePlayData = app.GetGamePlayLocalStorage();
                            popupArgs = new PopupArgs();
                            popupArgs.header = 'Confirmation';
                            popupArgs.msg1 = 'Are you sure you want to cancel the Game \'' + gamePlayData.Name + '\' that is in progress?';
                            popupArgs.btnYesHandler = 'funcCancelGamePlay';
                            popupArgs.btnNoHandler = 'back';
                            popupArgs.btnYesLabel = 'Yes';
                            popupArgs.btnNoLabel = 'No';
                            app.popUp(popupArgs);
                            //return
                        }

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
        app.SetHomePageStyle = function (initialState) {
            console.log('func app.SetHomePageStyle');

            if (initialState) {
                $('#home').css('background-image', 'url(./images/bckground/ProbeBackground.jpg)');

            } else {
                $('#home').css('background-image', 'url(./images/bckground/ProbeBackground-Opacity3.jpg)');
            }
        };

        /*
        Cancel game in progress
        */
        app.CancelGame = function () {
            console.log('func app.CancelGame');

            $('#menu').panel("close"); //if calling from a panel
            localStorage.removeItem("GamePlay");
            localStorage.removeItem("Result");
            app.SetHomePageStyle(true);
            $('#homePageContent').html('');
            $('#homePageContent').trigger("create");
            $("[data-icon='plus']").removeClass('ui-disabled');
            $("[data-icon='minus']").addClass('ui-disabled');
            gameState = GameState.Idle;
            app.SetHomePageInitialDisplay();
            $.mobile.changePage('#home');
        };

        /*
        Start game
        */
        app.StartGame = function (questionNbr) {
            console.log('func app.StartGame');

            app.SetNavBars(true, false);

            /*
            we are starting up the game. If the game state is original idle; then we
            know its a new game and we are going active, otherwise we are going to stary in read only state
            */
            if (gameState == GameState.Idle) {
                gameState = GameState.Active;
            } else if (gameState == GameState.ReadOnly) {
                gameState = GameState.ReadOnly;
            }
            currentQuestionNbr = questionNbr;
            app.SetQuestionPage(currentQuestionNbr)
        };

        /*
        Resume game
        arguments:
        gameStateArg
        */
        app.ResumeGame = function (gameStateArg) {
            console.log('func app.ResumeGame');

            gameState = gameStateArg;
            app.SetGamePlayPlayerPrompt();
        };

        app.DisplayReportPage = function () {
            console.log('func app.DisplayReportPage');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            if (gamePlayData.GameType == "Match") {
                url = ProbeMatchReporturl +
                    +result.GamePlayId
                    + '/' + result.PlayerId + '/1'; //with mobile indicator attached
            } else {
                url = ProbeTestReporturl +
                    +result.GamePlayId
                    + '/' + result.PlayerId + '/1'; //with mobile indicator attached
            }

            window.location = url;
        };

        /*
        returns true if game is in progress
        */
        app.IsGameInProgress = function () {
            console.log('func app.IsGameInProgress');

            gameInProgress = false;
            result = app.GetResultLocalStorage();
            if (result != {}) {
                if (result.DirtyFlag) {
                    gameInProgress = true;
                }
            }
            return gameInProgress;
        };

        /*
        returns true if all questions are answered, otherwise it returns false
        */
        app.IsAllQuestionsAnswered = function () {
            console.log('func app.IsAllQuestionsAnswered');

            allAnswered = true;

            for (i = 0; i < gamePlayData.GameQuestions.length; i++) {
                if (result.GameQuestions[i]["SelChoiceId"] == NO_ANSWER) allAnswered = false;
                
                if (!allAnswered) break;
            }

            return allAnswered;
        };

        /*
        returns number of questions answered
        */
        app.NbrQuestionsAnswered = function () {
            console.log('func app.NbrQuestionsAnswered');

            questionsAnswered = 0;

            for (i = 0; i < gamePlayData.GameQuestions.length; i++) {
                if (result.GameQuestions[i]["SelChoiceId"] != NO_ANSWER) questionsAnswered++;
            }

            return questionsAnswered;
        };

        /*
        returns the number of questions
        */
        app.NbrQuestions = function () {
            console.log('func app.NbrQuestions');

            return result.GameQuestions.length;
        };

        /*
        Queue Games Submitted - Keep the last 5
        */
        app.QueueGamePlays = function () {
            console.log('func app.QueueGamePlays');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            gamePlayQueue = app.GetGamePlayQueueLocalStorage();
            gamePlayListQueue = app.GetGamePlayListQueueLocalStorage();

            queueNbrStart = Math.min(gamePlayQueue.length - 1, gamePlayQueueMax - 2); //we are only going to save 5 submitted games

            for (var i = queueNbrStart; i >= 0; i--) {
                gamePlayListQueue[i + 1] = {};
                gamePlayQueue[i + 1] = {};
                gamePlayListQueue[i + 1] = gamePlayListQueue[i];
                gamePlayQueue[i + 1] = gamePlayQueue[i];
            }

            gamePlayListQueue[0] = {};
            gamePlayQueue[0] = {};
            gamePlayListQueue[0]["GamePlayId"] = result["GamePlayId"];
            gamePlayListQueue[0]["Name"] = gamePlayData.Name;
            gamePlayQueue[0].GamePlay = gamePlayData;
            gamePlayQueue[0].Result = result;
            app.PutGamePlayListQueueLocalStorage(gamePlayListQueue);
            app.PutGamePlayQueueLocalStorage(gamePlayQueue);

        };//app.QueueGamePlays

        /*
        return if the game has been submitted already (looking at the queue)
        arguments
        gamePlayId
        */
        app.IsGameSubmitted = function (gamePlayId) {
            console.log('func app.IsGameSubmitted');

            isSubmitted = false;

            gamePlayListQueue = app.GetGamePlayListQueueLocalStorage();

            for (i = 0; i < gamePlayListQueue.length; i++) {
                if (gamePlayListQueue[i]["GamePlayId"] == gamePlayId) {
                    isSubmitted = true;
                }

                if (isSubmitted) break;
            }

            return isSubmitted;
        };//app.IsGameSubmitted 

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
                    $('#popupMsgYesBtn').click(function (event) {
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
                    $('#popupMsgNoBtn').click(function (event) {
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
        GetConfigurationValue
        */
        app.GetConfigValue = function (configName) {
            console.log('func app.GetConfigValue');

            gameConfig = app.GetGameConfigLocalStorage();
            parmValue = {};

            for (i = 0; i < gameConfig.length; i++) {

                if (gameConfig[i].Name == configName) {
                    parmValue = gameConfig[i].Value;
                }

                if(parmValue != {}) break;
            }
            return parmValue;
            
        };//app.GetConfigValue

        /*
        Set Nav Bars
        qNavInd = true or false (true - show question nav bar)
        sNavIdn = true or false (true - show summary nav bar)
        */
        app.SetNavBars = function (qNavInd, sNavInd) {
            console.log('func app.SetNavBars qNavInd:' + qNavInd + ' sNavInd:' + sNavInd);

            /* MNS DEBUG
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
            */

        };//app.SetNavBars 

        /*
        submit success
        */
        app.SubmitSuccess = function () {
            console.log('func app.SubmitSuccess');

            //Set result data dirty flag and game state to Read Only
            result = app.GetResultLocalStorage();
            result.DirtyFlag = false; //we just submitted successfully, so the dirty flag must be reset.
            app.PutResultLocalStorage(result);

            gameState = GameState.ReadOnly;

            //set summary page for read-only game state
            $('.submitButton').addClass('ui-disabled');

            //set the newgame and cancel game buttons (enable new game, disable cancel game)
            $("[data-icon='plus']").removeClass('ui-disabled');
            $("[data-icon='minus']").addClass('ui-disabled');

            app.QueueGamePlays();

            //MNS COMMENT OUT ALL THE POPUP FOR IPHONE DEBUG TEST
            //update the current pop-up to a successful submission (a little bit of a hack, but couldn't put
            //another popup display. This doesn't work
            //$('#popMsgHeader').html('Informational'); //set header
            //$('#popupMsgContent').html('The submission of the Game \'' + gamePlayData.Name + '\' was successful.');
            //$('#popupMsgNoBtn').hide();
            //$('#popupMsgYesBtn').remove();

            //$mybutton = $('<a id="popupMsgYesBtn" href="#" data-role="button" data-inline="true" style="display:none" class="ui-link ui-btn ui-btn-inline ui-shadow ui-corner-all" role="button">OK</a>');
            //$mybutton.prependTo('#popupMsg div .buttonRight');
            //$('#popupMsgYesBtn').attr('data-rel', 'back');
            //$('#popupMsgYesBtn').show();

        };//app.SubmitSuccess

        /*
        create Javascript Objects JSON for HTTP request
        */
        app.CreatePlayer = function () {
            console.log('func app.CreatePlayer');

            gamePlayData = app.GetGamePlayLocalStorage();
            result = app.GetResultLocalStorage();

            result["GamePlayId"] = JSONdata.Id;
            result["FirstName"] = "NO-NO-NO";
            result["LastName"] = "NO-NO-NO";
            result["NickName"] = "NO-NO-NO";
            result["Sex"] = SexType.Female;

            player = {};
            player["FirstName"] = result["FirstName"];
            player["LastName"] = result["LastName"];
            player["NickName"] = result["NickName"];
            player["Sex"] = result["Sex"];

            return player;
        };

    /* ***************************************************
    Helper routines for local storage of GamePlay, Results,
    GamePlayListQueue, and GamePlayListQueue data
    */
    app.PutGamePlayLocalStorage = function (gamePlay) {
        localStorage["GamePlay"] = JSON.stringify(gamePlay);
    };

    app.GetGamePlayLocalStorage = function () {
        localResult = undefined;
        if (localStorage["GamePlay"] != undefined) {
            localResult = JSON.parse(localStorage["GamePlay"]);
        }

        return localResult;
    };

    app.PutResultLocalStorage = function (result) {
        localStorage["Result"] = JSON.stringify(result);
    };

    app.GetResultLocalStorage = function () {
        localResult = {};
        if (localStorage["Result"] != undefined) {
            localResult = JSON.parse(localStorage["Result"]);
        }
        return localResult;
    };

    app.PutGamePlayQueueLocalStorage = function (gamePlayQueue) {
        localStorage["GamePlayQueue"] = JSON.stringify(gamePlayQueue);
    };

    app.GetGamePlayQueueLocalStorage = function () {
        gamePlayQueue = new Array();
        if (localStorage["GamePlayQueue"] != undefined) {
            gamePlayQueue = JSON.parse(localStorage["GamePlayQueue"]);
        }
        return gamePlayQueue;
    };

    app.PutGamePlayListQueueLocalStorage = function (gamePlayListQueue) {
        localStorage["GamePlayListQueue"] = JSON.stringify(gamePlayListQueue);
    };

    app.GetGamePlayListQueueLocalStorage = function () {
        gamePlayListQueue = new Array();
        if (localStorage["GamePlayListQueue"] != undefined) {
            gamePlayListQueue = JSON.parse(localStorage["GamePlayListQueue"]);
        }
        return gamePlayListQueue;
    };

    app.PutGameConfigLocalStorage = function (gameConfig) {
        localStorage["GameConfig"] = JSON.stringify(gameConfig);
    };

    app.GetGameConfigLocalStorage = function () {
        gameConfig = {};
        if (localStorage["GameConfig"] != undefined) {
            gameConfig = JSON.parse(localStorage["GameConfig"]);
        }
        return gameConfig;
    };

    app.ajaxHelper = function (uri, method, data) {
        console.log('func app.ajaxHelper uri:' + uri);

        return $.ajax({
            type: method,
            async: false,
            url: uri,
            dataType: 'json',
            contentType: 'application/json',
            data: data ? JSON.stringify(data) : null
        })
    }//function ajaxHelper

    /* ***************************************************** */

    //initialize the Prob app
    app.init();


})(probeApp); //app

funcCancelGamePlay = function (button, theApp) {
    theApp.CancelGame();
    $('#popupMsg').enhanceWithin().popup().popup("close", { transition: "slide" });
};

funcSubmitGamePlay = function (button, theApp) {
    result = theApp.GetResultLocalStorage();
    console.log('func funcSubmitGamePlay - GamePlayId:' + result["GamePlayId"]);

    returnErrMsg = theApp.PostGamePlayAnswersServer();

    if (returnErrMsg == null) {
        theApp.SubmitSuccess();
    } else {
        $('#popMsgHeader').html('Error'); //set header
        $('#popupMsgContent').html('The submission of the Game \'' + gamePlayData.Name + '\' was NOT successful.<br/>' + returnErrMsg);
        $('#popupMsgNoBtn').hide();
        $('#popupMsgYesBtn').remove();

        $mybutton = $('<a id="popupMsgYesBtn" href="#" data-role="button" data-inline="true" style="display:none" class="ui-link ui-btn ui-btn-inline ui-shadow ui-corner-all" role="button">OK</a>');
        $mybutton.prependTo('#popupMsg div .buttonRight');
        $('#popupMsgYesBtn').attr('data-rel', 'back');
        $('#popupMsgYesBtn').show();

    }//if (isSubmitASuccess) {


};//funcSubmitGamePlay

});



