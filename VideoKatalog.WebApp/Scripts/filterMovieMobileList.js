function MovieFilter(name, cast, director, genre, minSize, maxSize, minYear, maxYear, minRuntime, maxRuntime, minRating, maxRating, hasCroSub, hasEngSub, sortProperty) {
    this.name = name;
    this.cast = cast;
    this.director = director;
    this.genre = genre;
    this.minSize = minSize;
    this.maxSize = maxSize;
    this.minYear = minYear;
    this.maxYear = maxYear;
    this.minRuntime = minRuntime;
    this.maxRuntime = maxRuntime;
    this.minRating = minRating;
    this.maxRating = maxRating;
    this.hasCroSub = false;
    this.hasEngSub = false;
    this.sortProperty = sortProperty;
}
var movieFilter = new MovieFilter();


$(document).ready(function () {
    initAll();
});

function initAll() {
    movieFilter = jQuery.parseJSON(sessionStorage.getItem('movieFilter'));
    if (movieFilter == null) {
        movieFilter = new MovieFilter();
    }
    else {
        $('#movieName').text(movieFilter.name);
    }
}

function saveFilters() {
    movieFilter.name = $('#movieName').val();
    movieFilter.cast = $('#cast').val();
    movieFilter.director = $('#dir').val();
    movieFilter.minSize = $('#sizeMin').val();
    movieFilter.maxSize = $('#sizeMax').val();
    movieFilter.minYear = $('#yearMin').val();
    movieFilter.maxYear = $('#yearMax').val();
    movieFilter.minRuntime = $('#runtimeMin').val();
    movieFilter.maxRuntime = $('#runtimeMax').val();
    movieFilter.minRating = $('#ratingMin').val();
    movieFilter.maxRating = $('#ratingMax').val();
    movieFilter.sortProperty = $('#selectSortAttr').val();
    if (movieFilter.sortProperty.indexOf(' ') > -1) {
        movieFilter.sortProperty = '';
    }
    movieFilter.genre = new Array();

    if ($("#Akcija").is(':checked')) {
        movieFilter.genre.push("Akcija");
    }
    if ($("#Animacija").is(':checked')) {
        movieFilter.genre.push("Animacija");
    }
    if ($("#Avantura").is(':checked')) {
        movieFilter.genre.push("Avantura");
    }
    if ($("#Crtani").is(':checked')) {
        movieFilter.genre.push("Crtani");
    }
    if ($("#Drama").is(':checked')) {
        movieFilter.genre.push("Drama");
    }
    if ($("#Dokumentarni").is(':checked')) {
        movieFilter.genre.push("Dokumentarni");
    }
    if ($("#Fantastika").is(':checked')) {
        movieFilter.genre.push("Fantastika");
    }
    if ($("#Horor").is(':checked')) {
        movieFilter.genre.push("Horor");
    }
    if ($("#Komedija").is(':checked')) {
        movieFilter.genre.push("Komedija");
    }
    if ($("#Krimi").is(':checked')) {
        movieFilter.genre.push("Krimi");
    }
    if ($("#Misterija").is(':checked')) {
        movieFilter.genre.push("Misterija");
    }
    if ($("#Muzicki").is(':checked')) {
        movieFilter.genre.push("Muzički");
    }
    if ($("#Obiteljski").is(':checked')) {
        movieFilter.genre.push("Obiteljski");
    }
    if ($("#Povijesni").is(':checked')) {
        movieFilter.genre.push("Povijesni");
    }
    if ($("#Ratni").is(':checked')) {
        movieFilter.genre.push("Ratni");
    }
    if ($("#Romantika").is(':checked')) {
        movieFilter.genre.push("Romantika");
    }
    if ($("#Sportski").is(':checked')) {
        movieFilter.genre.push("Sportski");
    }
    if ($("#Vestern").is(':checked')) {
        movieFilter.genre.push("Vestern");
    }
    if ($("#Triler").is(':checked')) {
        movieFilter.genre.push("Triler");
    }
    if ($("#ZnanstvenaFantastika").is(':checked')) {
        movieFilter.genre.push("Znanstvena fantastika");
    }


    sessionStorage.setItem('movieFilter', JSON.stringify(movieFilter));
    window.location = "List";
}
   

   