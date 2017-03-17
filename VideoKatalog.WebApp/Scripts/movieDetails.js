$(document).ready(function () {    
    rearrangeLayoutMDetails();
    initAll();
})

function initAll() {
    var currMovie = jQuery.parseJSON(sessionStorage.getItem('currMovie'));

}

function Movie(id, name, origName, size, rating, cast, director, year, plot, runtime, hasCroSub, hasEngSub, videoRes, dateAdded, imdbLink, trailerLink, genre, langs) {
    this.id = id;
    this.name = name;
    this.origName = origName;
    this.size = size;
    this.rating = rating;
    this.cast = cast;
    this.director = director;
    this.year = year;
    this.plot = plot;
    this.runtime = runtime;
    this.hasCroSub = hasCroSub;
    this.hasEngSub = hasEngSub;
    this.videoRes = videoRes;
    this.dateAdded = dateAdded;
    this.imdbLink = imdbLink;
    this.trailerLink = trailerLink;
    this.genre = genre;
    this.langs = langs;
}

function initAllMDetails() {
    window.onresize = debounce(function () {
        rearrangeLayoutMDetails();
    }, 10);
    $(window).trigger('orientationchange');
}

function rearrangeLayoutMDetails() {
    /*if (window.innerWidth <= 360) {
        //alert("");
        var left = (window.innerWidth - 300) / 2;
        $("#posterDiv").css("left", left + "px");
    }
    else {
        $("#posterDiv").css("left", '10px');
    }*/
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
