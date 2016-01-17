var PLAYER_NOT_NAMED = "NO-NAME";

/*
FUNCTIONS FOR FORMATTING GRID COLUMNS
*/

function ActiveText(ActiveInd) {
    html = 'No';
    if (ActiveInd) {
        html = 'Yes';
    }

    return html;
}

/*
function template for Grid - Active
*/
function DisplayActive(player) {
    return ActiveText(player.Active);
}//function DisplayActive(player) {

/*
Get sex type text from the sex
*/
function SexTypeText(sexTypeId) {

    sexTypeText = "";
    switch (sexTypeId) {
        case 0:
            sexTypeText = 'UNKNOWN';
            break;
        case 1:
            sexTypeText = 'MALE';
            break;
        case 2:
            sexTypeText = 'FEMALE';
            break;
    }

    return sexTypeText;
}

/*
END FUNCTIONS FOR FORMATTING GRID COLUMNS
*/


/*
OnGridDataBound event for MyGameSchedules Grid
1. Setup event handler when the player count is clicked in the grid player count column
2. Show or Hide commands based on conditionality
3. Save Grid options/configuration/command buttons-event handler support
*/
function OnGridDataBound(e) {

    grid = $("#MyGameSchedulesGrid").data("kendoGrid");
    gridViewLevel = grid.dataSource.view();

}//function OnGridDataBound() {

function SyncServerData() {
    console.log('SyncServerData');
    gridDataSource = $("#MyGameSchedulesGrid").data("kendoGrid").dataSource;
    gridDataSource.read();

}//SyncServerData()

/*
MNS DEBUG
*/
function Oncancel() {
    console.log('Oncancel event');
}
function Onchange() {
    console.log('Onchange event');
}
function OncolumnHide() {
    console.log('OncolumnHide event');
}
function OncolumnMenuInit() {
    console.log('OncolumnMenuInit event');
}
function OncolumnReorder() {
    console.log('OncolumnReorder event');
}
function OncolumnResize() {
    console.log('OncolumnResize event');
}
function OncolumnShow() {
    console.log('OncolumnShow event');
}
function OndataBinding(e) {
    console.log('OndataBinding event');
}
function OndetailExpand() {
    console.log('OndetailExpand event');
}
function OndetailCollapse() {
    console.log('OndetailCollapse event');
}
function OndetailInit() {
    console.log('OndetailInit event');
}
function OnfilterMenuInit() {
    console.log('OnfilterMenuInit event');
}
function Onremove() {
    console.log('Onremove event');
}
function Onsave() {
    console.log('Onsave event');
}
function Onsavechanges() {
    console.log('Onsavechanges event');
}

var wndGen;

$(document).ready(function () {

    var grid = $("#MyGameSchedulesGrid").data("kendoGrid"); //NEED THIS GLOBAL GRID VAR

    grid.bind("cancel", Oncancel);
    grid.bind("change", Onchange);
    grid.bind("columnHide", OncolumnHide);
    grid.bind("columnMenuInit", OncolumnMenuInit);
    grid.bind("columnReorder", OncolumnReorder);
    grid.bind("columnResize", OncolumnResize);
    grid.bind("columnShow", OncolumnShow);
    grid.bind("dataBinding", OndataBinding);
    grid.bind("detailExpand", OndetailExpand);
    grid.bind("detailCollapse", OndetailCollapse);
    grid.bind("detailInit", OndetailInit);
    grid.bind("filterMenuInit", OnfilterMenuInit);
    grid.bind("remove", Onremove);
    grid.bind("save", Onsave);
    grid.bind("savechanges", Onsavechanges);

}); //$(document).ready(function () {

