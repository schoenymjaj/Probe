

/*
SUPPORT COMMAND EVENT HANDLERS FOR GRID
*/
function OnCloneToUserButtonClick(e) {
    e.preventDefault();

    userDropDownList = $("#ProbeUserDropdown").data('kendoDropDownList');
    gameDropDownList = $("#ProbeGameDropdown").data('kendoDropDownList');

    if (userDropDownList.value() != "" && gameDropDownList.value() != "") {

        //prepare and open confirmation dialog
        message1 = 'You are about to clone the game <span style="font-style: italic;font-weight:bold">' + gameDropDownList.text() +
            '</span> TO user <span style="font-style: italic;font-weight:bold">' + userDropDownList.text() + '</span>.';
        message2 = '<span style="font-weight:bold">Are you sure?</span>';
        ShowGeneralDialog(wndGen, 'Clone Game To User', message1, message2, true, 'OK', true, 'Cancel');
        $("#yesGen").click(function () {
            console.log('ShowGeneralDialog click YES');
            ActivityIndicatorOn('#spinner');

            gameId = gameDropDownList.value();
            userId = userDropDownList.value();

            url = PrepareURL('CloneToUser/' + gameId + '/' + userId);
            $.getJSON(url, {},
            function (data) {

                $("#yesGen").unbind("click");
                $("#noGen").unbind("click");
                ActivitySelectorOff('#spinner');

                //prepare and open informational dialog for clone action
                ShowGeneralDialog(wndGen, 'Clone Game to User', data.Message, '', false, '', true, 'Close');
                $("#noGen").click(function () {
                    console.log('ShowGeneralDialog2 click CLOSE');

                    wndGen.close();
                });

            });//post

            wndGen.close();
        });

        $("#noGen").click(function () {
            wndGen.close();
        });

    }//if (userDropDownList.value() != "" && userDropDownList.value() != "") {
}//function openDeleteConfirm(e)


/*
 FUNCTIONS THAT WILL SUPPORT THE EVENT HANDLERS
*/
function ShowGeneralDialog(aWnd, aTitle, message1, message2, okInd, okText, closeInd, closeText) {
    console.log('func ShowGeneralDialog start');

    aWnd.title(aTitle);

    $('#dialog-generalMessage').html(message1);
    $('#dialog-generalMessage2').html(message2);

    aWnd.center().open();

    if (okInd) {
        $("#yesGen").show();
        $("#yesGen").html(okText);
    } else {
        $("#yesGen").hide();
    }

    if (closeInd) {
        $("#noGen").show();
        $("#noGen").html(closeText);
    } else {
        $("#noGen").hide();
    }

    console.log('func ShowGeneralDialog end');
}//function ShowGeneralDialog

/*
MNS DEBUG
*/

var wndGen;
var wndGen2;

$(document).ready(function () {

    userdropdown = $("ProbeUserDropdown").data('kendoDropDownList');
    gamedropdown = $("ProbeGameDropdown").data('kendoDropDownList');

    /*
    Supporting General Messaging
    */
    wndGen = $("#dialog-general").kendoWindow({
        title: "A title",
        modal: true,
        visible: false,
        resizable: false,
        width: 300
    }).data("kendoWindow");

    /*
    Supporting General Messaging
    */
    wndGen2 = $("#dialog2-general").kendoWindow({
        title: "A title",
        modal: true,
        visible: false,
        resizable: false,
        width: 300
    }).data("kendoWindow");


    wndProgress = $("#dialog-progress").kendoWindow({
        title: "In Progress. Please Wait ...",
        modal: true,
        visible: false,
        resizable: false,
        width: 250
    }).data("kendoWindow");

    $("#progressbar").kendoProgressBar({
        type: "chunk",
        max: 100,
        chunkCount: 10,
        value: 50
    });

    /*
    Supporting the iFrame Dialog
    */
    wndIframe = $("#dialog-iframe").kendoWindow({
        title: "IFrame Preview",
        modal: true,
        visible: false,
        resizable: true,
        draggable: true
    }).data("kendoWindow");

    /*
    Supporting the Details Dialog
    */
    wndDetails = $("#dialog-details").kendoWindow({
        title: "Details",
        modal: true,
        visible: false,
        resizable: true,
        draggable: true,
        minWidth: 160,
        maxWidth: 500
    }).data("kendoWindow");

}); //$(document).ready(function () {

