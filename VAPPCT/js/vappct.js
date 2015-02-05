/*
    toggles a radio button inside a gridview
*/
function ToggleGVRadio(rdoID, gvID) {
    var rdo = document.getElementById(rdoID);
    if (rdo != null) {
        var all = document.getElementsByTagName("input");
        for (i = 0; i < all.length; i++) {
            if (all[i].type == "radio" && all[i].id != rdo.id) {
                //make sure radio button is in our gridview
                if (all[i].id.indexOf(gvID) != -1) {
                    all[i].checked = false;
                }
            }
        }

        rdo.checked = true;
    }
}

/*
    flag variable used to determine if the please wait dialog should be shown
*/
var showPleaseWait = true;

/*
    prevents the please wait dialog from showing when a user clicks
    a page link in a grid view
*/
function initPagers() {
    var element;
    var allElements = document.getElementsByTagName("*");
    var className = "gv_pleasewait";
    var hasClassName = new RegExp("(?:^|\\s)" + className + "(?:$|\\s)");
    
    for (var i = 0; (element = allElements[i]) != null; i++) {
        var elementClass = element.className;
        if (elementClass && elementClass.indexOf(className) != -1 && hasClassName.test(elementClass)) {
            element.setAttribute('onclick', 'pagerClick()');
        }
    }
}

/*
    sets the flag variable to false, this is used to prevent the please wait
    dialog from showing
*/
function pagerClick() {
    showPleaseWait = false;
}

/*
    displays the please wait dialog
*/
function PleaseWait() {
    if (document.getElementById('divPleaseWait') != null && showPleaseWait) {
        document.getElementById('divPleaseWait').style.display = 'block';
        ShowCenter(150, 50, 'divPleaseWait');
    }
    showPleaseWait = true
}

/*
    shows the specified div in the center of the screen
*/
function ShowCenter(Xwidth, Yheight, divid) {
    //determine how much the visitor has scrolled
    var scrolledX, scrolledY;
    if (self.pageYOffset) {
        scrolledX = self.pageXOffset;
        scrolledY = self.pageYOffset;
    } else if (document.documentElement && document.documentElement.scrollTop) {
        scrolledX = document.documentElement.scrollLeft;
        scrolledY = document.documentElement.scrollTop;
    } else if (document.body) {
        scrolledX = document.body.scrollLeft;
        scrolledY = document.body.scrollTop;
    }

    //get the coordinates of the center of browser's window
    var centerX, centerY;
    if (self.innerHeight) {
        centerX = self.innerWidth;
        centerY = self.innerHeight;
    } else if (document.documentElement && document.documentElement.clientHeight) {
        centerX = document.documentElement.clientWidth;
        centerY = document.documentElement.clientHeight;
    } else if (document.body) {
        centerX = document.body.clientWidth;
        centerY = document.body.clientHeight;
    }
    var leftOffset = scrolledX + (centerX - Xwidth) / 2;
    var topOffset = scrolledY + (centerY - Yheight) / 2;
    var o = document.getElementById(divid);
    var r = o.style;
    r.position = 'absolute';
    r.top = topOffset + 'px';
    r.left = leftOffset + 'px';
    r.display = "block";
}