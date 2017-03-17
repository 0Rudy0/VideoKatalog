$(document).ready(function () {
    initAll();
    $('div[style*="z-index: 2147483647"]').remove()
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
    this.hasCroSub = hasCroSub;
    this.hasEngSub = hasEngSub;
    this.sortProperty = sortProperty;
}

var movieList = new Array();
var movieFilter = new MovieFilter();
var selectedMovies = new Array();
var page = 1;
var moviesPerPage = 20;

function initAll() {
    //localStorage.clear();
    //sessionStorage.clear();
    window.onresize = debounce(function () {
        rearrangeLayout();
    }, 10);
    movieFilter = jQuery.parseJSON(sessionStorage.getItem('movieFilter'));
    if (movieFilter == null) {
        movieFilter = new MovieFilter();
        if (localStorage.getItem("movieList") == null) {
            movieList = new Array();
            getSetMoviesFromJsonResult("allMovies");
        }
        else {
            var retreived = localStorage.getItem("movieList");
            jsonAsTextToMovieList(retreived, false);
        }
    }
    else {
        if (movieFilter.sortProperty == '') {
            if (localStorage.getItem("movieList") == null) {
                movieList = new Array();
                getSetMoviesFromJsonResult("allMovies");
            }
            else {
                var retreived = localStorage.getItem("movieList");
                jsonAsTextToMovieList(retreived, false);
            }
        }
        else {
            movieList = new Array();
            getSetMoviesFromJsonResult("allMoviesOrdered?sortProperty=" + movieFilter.sortProperty);
        }
    }

    selectedMovies = JSON.parse(sessionStorage.getItem('selectedMovies'));
    movieFilter.sortProperty = ''
    sessionStorage.setItem('movieFilter', JSON.stringify(movieFilter));
    if (selectedMovies == null) {
        selectedMovies = new Array();
    }

    sessionStorage.setItem('moviePage', 1);
    fillList();
}

function fillList() {
    page = sessionStorage.getItem('moviePage');
    var listString = '';
    var countMovs = 0;
    for (var i = moviesPerPage * (page - 1) ;
        i < movieList.length && countMovs < moviesPerPage ;
        i++) {
        if (doesComplyFilters(movieList[i]) == false) {
            continue;
        }
        countMovs++;
        var style = "";
        if (isSelected(movieList[i].id)) {
            style = 'style="opacity: 0.3; left: -10px"';
        }
        var listItem;
        if (i == moviesPerPage * (page - 1)) {
            listItem = '<li class="ui-li-has-alt ui-first-child" id="' + movieList[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="#" onclick="goToMovieDetails(' + i + ')">' + movieList[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-plus ui-btn-b" href="#" onclick="selectMovie(' + movieList[i].id + ')" title=""></a>' +
                '</li>';
        }
        else if (i == movieList.length - 1 || i == (moviesPerPage * page - 1)) {
            listItem = '<li class="ui-li-has-alt ui-last-child" id="' + movieList[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="#" onclick="goToMovieDetails(' + i + ')">' + movieList[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-plus ui-btn-b" href="#" onclick="selectMovie(' + movieList[i].id + ')" title=""></a>' +
                '</li>';
        }
        else {
            listItem = '<li class="ui-li-has-alt" id="' + movieList[i].id + '"' + style + '>' +
                '<a class="ui-btn" href="#" onclick="goToMovieDetails(' + i + ')">' + movieList[i].name + '</a>' +
                '<a class="ui-btn ui-btn-icon-notext ui-icon-plus ui-btn-b" href="#" onclick="selectMovie(' + movieList[i].id + ')" title=""></a>' +
                '</li>';
        }
        listString += listItem;
    }
    $('#movieList').html(listString);
    $('html,body').animate({ scrollTop: 0 }, 200);

    $("#movieList").animate({
        opacity: 1
    }, 300, function () {
    });
    updateSelectedSize();
}

function doesComplyFilters(movieObj) {
    if (movieFilter.name != null &&
        movieFilter.name.length > 0 &&
        movieObj.name.toLowerCase().indexOf(movieFilter.name.trim().toLowerCase()) < 0) {
        return false;
    }
    if (movieFilter.cast != null &&
        movieFilter.cast.length > 0 &&
        movieObj.cast.toLowerCase().indexOf(movieFilter.cast.trim().toLowerCase()) < 0) {
        return false;
    }
    if (movieFilter.director != null &&
        movieFilter.director.length > 0 &&
        movieObj.director.toLowerCase().indexOf(movieFilter.director.trim().toLowerCase()) < 0) {
        return false;
    }

    var foundGenre = false;
    for (var i = 0; i < movieFilter.genre.length; i++) {
        if (movieObj.genre.indexOf(movieFilter.genre[i]) >= 0) {
            foundGenre = true;
            break;
        }
    }
    if (!foundGenre && movieFilter.genre.length > 0) {
        return false;
    }

    if ((movieObj.size / (1024.0 * 1024 * 1024)) < movieFilter.minSize ||
        (movieObj.size / (1024.0 * 1024 * 1024)) > movieFilter.maxSize)
        return false;

    if (movieObj.year < movieFilter.minYear ||
        movieObj.year > movieFilter.maxYear)
        return false;

    if ((movieObj.runtime / 60000) < movieFilter.minRuntime ||
        (movieObj.runtime / 60000) > movieFilter.maxRuntime)
        return false;

    if (movieObj.rating < movieFilter.minRating ||
        movieObj.rating > movieFilter.maxRating)
        return false;

    return true;
}

function getSetMoviesFromJsonResult(requestUrl) {
    showLoading();
    $.ajax({
        url: requestUrl,
        type: "GET",
        dataType: "text",
        async: true,
        success: function (msg) {
            if (msg.indexOf('<!--SCRIPT GENERATED BY SERVER! PLEASE REMOVE-->') > -1) {
                msg = msg.substring(0, msg.indexOf("<!--SCRIPT GENERATED BY SERVER! PLEASE REMOVE-->"));
            }
            var result = jQuery.parseJSON(msg);
            jsonAsTextToMovieList(JSON.stringify(result.movies), true);

            fillList();
            $.mobile.loading("hide");
        },
        error: function (e) {
            $.mobile.loading("hide");
        }
    });
}

function jsonAsTextToMovieList(text, isFullJson) {
    var result = jQuery.parseJSON(text);

    var newMovies = new Array();
    for (var i = 0; i < result.length; i++) {
        newMovies[i] = new Movie();
        newMovies[i].id = result[i].id;
        newMovies[i].name = result[i].name;
        newMovies[i].origName = result[i].origName;
        newMovies[i].size = result[i].size;
        newMovies[i].rating = result[i].rating;
        newMovies[i].plot = result[i].plot;
        newMovies[i].year = result[i].year;
        newMovies[i].runtime = result[i].runtime;
        newMovies[i].videoRes = result[i].videoRes;
        newMovies[i].dateAdded = result[i].dateAdded;
        newMovies[i].imdbLink = result[i].imdbLink;
        newMovies[i].trailerLink = result[i].trailerLink;
        if (isFullJson) {
            var castString = "";
            for (var j = 0; j < result[i].cast.length; j++) {
                castString += result[i].cast[j].name + ", ";
            }
            castString = castString.substring(0, castString.length - 2);
            newMovies[i].cast = castString;

            var dirString = "";
            for (var j = 0; j < result[i].directors.length; j++) {
                dirString += result[i].directors[j].name + ", ";
            }
            dirString = dirString.substring(0, dirString.length - 2);
            newMovies[i].director = dirString;

            var genresString = "";
            for (var j = 0; j < result[i].genres.length; j++) {
                genresString += result[i].genres[j].name + ", ";
            }
            genresString = genresString.substring(0, genresString.length - 2);
            newMovies[i].genre = genresString;

            var langsArray = "";
            newMovies[i].hasCroSub = 0;
            newMovies[i].hasEngSub = 0;
            for (var j = 0; j < result[i].languages.length; j++) {
                if (result[i].languages[j].name.trim().toLowerCase() == "croatian") {
                    newMovies[i].hasCroSub = 1;
                }
                if (result[i].languages[j].name.trim().toLowerCase() == "english") {
                    newMovies[i].hasEngSub = 1;
                }
                langsArray += result[i].languages[j].name + ", ";
            }
            langsArray = langsArray.substring(0, langsArray.length - 2);
            newMovies[i].langs = langsArray;
        }
        else {
            newMovies[i].cast = result[i].cast;
            newMovies[i].director = result[i].director;
            newMovies[i].genre = result[i].genre;
            newMovies[i].langs = result[i].langs;
            newMovies[i].hasCroSub = result[i].langs.trim().toLowerCase().indexOf("croatian") > -1;
            newMovies[i].hasEngSub = result[i].langs.trim().toLowerCase().indexOf("english") > -1;
        }
    }
    /*for (var i = 0; i < newMovies.length; i++) {
        movies[movies.length] = newMovies[i];
    }*/
    for (var i = 0; i < movieList.length; i++) {
        newMovies[newMovies.length] = movieList[i];
    }
    movieList = newMovies;
    localStorage.setItem("movieList", JSON.stringify(movieList));
}

function showLoading() {
    var $this = $(this),
        theme = $this.jqmData("theme") || $.mobile.loader.prototype.options.theme,
        msgText = $this.jqmData("msgtext") || $.mobile.loader.prototype.options.text,
        textVisible = $this.jqmData("textvisible") || $.mobile.loader.prototype.options.textVisible,
        textonly = !!$this.jqmData("textonly");
    html = $this.jqmData("html") || "";
    $.mobile.loading("show", {
        text: msgText,
        textVisible: textVisible,
        theme: theme,
        textonly: textonly,
        html: html
    });
}

function reloadData() {
    movieList = new Array();
    getSetMoviesFromJsonResult("allMovies");
}

function selectMovie(id) {
    var selected = false;
    for (var i = 0; i < selectedMovies.length; i++) {
        if (selectedMovies[i].id == id) {
            selectedMovies.splice(i);
            selected = true;
            showListElement(id);
            break;
        }
    }
    if (!selected) {
        for (var i = 0; i < movieList.length; i++) {
            if (movieList[i].id == id) {
                selectedMovies.push(movieList[i]);
                break;
            }
        }
        removeListElement(id);
    }
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
        opacity: 0.25,
        left: '-10px'
    }, 100);
}
function showListElement(id) {
    $("#" + id).animate({
        opacity: 1,
        left: 0
    }, 100);
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

function previousPage() {
    if (page > 1) {
        page--;
        sessionStorage.setItem('moviePage', page);
        $("#movieList").animate({
            opacity: 0
        }, 300, function () {
            fillList();
        });
    }
}

function nextPage() {
    if (movieList.length > moviesPerPage * (page - 1)) {
        page++;
        sessionStorage.setItem('moviePage', page);
        $("#movieList").animate({
            opacity: 0
        }, 300, function () {
            fillList();
        });
    }
}

function goToMovieDetails(movieIndex) {
    sessionStorage.setItem('currMovie', JSON.stringify(movieList[movieIndex]));
    window.location = "Details/" + movieList[movieIndex].id;
}

function updateSelectedSize() {
    var selectedSize = 0.0;
    for (var i = 0; i < selectedMovies.length; i++) {
        selectedSize += selectedMovies[i].size;
    }
    $('#selectedSize').html("Odabrano: " + getSize(selectedSize).toFixed(2) + " GB");
}

function rearrangeLayout() {
    if (window.innerWidth <= 360) {
        //$("#navText").addClass("ui-btn-icon-notext");
        $("#navText").html('.');
    }
    else {
        //$("#navText").removeClass("ui-btn-icon-notext");
        $("#navText").html('Navigacija');
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
