$(document).ready(function () {
    initAllLayout();
    rearrangeLayout();
    FastClick.attach(document.body);
});

function initAllLayout() {
    window.onresize = debounce(function () {
        rearrangeLayout();
    }, 10);
}

function rearrangeLayout() {
    if (window.innerWidth <= 360) {
        //$("#navText").addClass("ui-btn-icon-notext");
    }
    else {
        //$("#navText").removeClass("ui-btn-icon-notext");
    }
}

function debounce(func, wait, immediate) {
    var timeout;
    return function () {
        var context = this, args = arguments;
        var later = function () {
            timeout = null;
            if (!immediate) func.apply(context, args);
        };
        if (immediate && !timeout) func.apply(context, args);
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

