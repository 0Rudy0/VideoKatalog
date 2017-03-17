$(document).ready(function () {
    initAll();
});

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

var selectedMovies = new Array();

function initAll() {    
    selectedMovies = JSON.parse(sessionStorage.getItem('selectedMovies'));
    if (selectedMovies == null) {
        selectedMovies = new Array();
    }
    fillList();
}

function fillList() {
    var listString = '';
    for (var i = 0 ; i < selectedMovies.length ; i++) {

        var style = "";
        
        var listItem;
        if (i == 0) {
            listItem = '<li class="ui-li-has-alt ui-first-child" id="' + selectedMovies[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="Details/' + selectedMovies[i].id + '">' + selectedMovies[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-minus ui-btn-b" href="#" onclick="unselectMovie(' + selectedMovies[i].id + ')" title=""></a>' +
                '</li>';
        }
        else if (i == selectedMovies.length - 1) {
            listItem = '<li class="ui-li-has-alt ui-last-child" id="' + selectedMovies[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="Details/' + selectedMovies[i].id + '">' + selectedMovies[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-minus ui-btn-b" href="#" onclick="unselectMovie(' + selectedMovies[i].id + ')" title=""></a>' +
                '</li>';
        }
        else {
            listItem = '<li class="ui-li-has-alt" id="' + selectedMovies[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="Details/' + selectedMovies[i].id + '">' + selectedMovies[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-minus ui-btn-b" href="#" onclick="unselectMovie(' + selectedMovies[i].id + ')" title=""></a>' +
                '</li>';
        }
        listString += listItem;
    }
    $('#movieList').html(listString);
    updateSelectedSize();
}

function unselectMovie(id) {
    var selected = false;
    for (var i = 0; i < selectedMovies.length; i++) {
        if (selectedMovies[i].id == id) {
            selectedMovies.splice(i, 1);
            selected = true;
            break;
        }
    }
    removeListElement(id);
    sessionStorage.setItem('selectedMovies', JSON.stringify(selectedMovies));
    updateSelectedSize();
}

function isSelected(id) {
    for (var i = 0; i < selectedMovies.length; i++) {
        if (selectedMovies[i].id == id) {
            return true;
        }
    }
    return false;
}

function removeListElement(id) {
    $("#" + id).animate({
        opacity: 0.25
    }, 100, function () {
        $("#" + id).css('display', 'none');
    });
}
function showListElement(id) {
    $("#" + id).animate({
        opacity: 1
    }, 300);
}

function getSize(valueInBytes) {
    var size = valueInBytes;
    var count = 0;
    while (count < 3) {
        size /= 1024.0;
        count++;
    }
    return size;
}


function updateSelectedSize() {
    var selectedSize = 0.0;
    for (var i = 0; i < selectedMovies.length; i++) {
        selectedSize += selectedMovies[i].size;
    }
    $('#selectedSize').html("Odabrano: " + getSize(selectedSize).toFixed(2) + " GB");
}

