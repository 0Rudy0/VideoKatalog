var loadData = true;
var set = false;
var selectedGenres = "empty";
var changedSorting = false;

var movies = new Array();
var selectedMovies = new Array();
var filter = new MovieFilter();
var selectedFocus = 0;
var lastDate;
var imdbLink;

var posterRootUrl = 'http://videokatalog.codius.hr/VideoKatalog/posters/';
var posterMovieUrl = posterRootUrl + 'movie/';
var posterSerieUrl = posterRootUrl + 'serie/'

function Movie (id, name, origName, size, rating, cast, director, year, plot, runtime, hasCroSub, hasEngSub, videoRes, dateAdded, imdbLink, trailerLink, genre, langs) {
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


//#region ON START

$(document).ready(function () {
    initAll();
});

function initAll() {
    initSliders();
    initRestComponents ();
    preloadIcons();
    addEventListeners();
    //loadInitData(); - ovo se odvija nakon animacije koja je zadnja
    resetSearchForm();
    if (loadData) {
        $("#reloadDiv").css("display", "block");
        $("#reloadIndicator").css("display", "block");
    }
    setTimeout(function () {
        showCurrentSession();
    }, 1000);
}
function initSliders() {
    $("#sliderImdb").noUiSlider({
        range: [1, 10],
        start: [1, 10],
        connect: true,
        margin: 1,
        step: 0.1,
        behaviour: "tap-drag",
        slide: function () {
            $("#imdbMin").html($("#sliderImdb").val()[0].substring(0, $("#sliderImdb").val()[0].length - 1));
            $("#imdbMax").html($("#sliderImdb").val()[1].substring(0, $("#sliderImdb").val()[1].length - 1));
        }
    });
    $("#sliderSize").noUiSlider({
        range: [0, 50],
        start: [0, 50],
        connect: true,
        margin: 3,
        step: 0.1,
        behaviour: "tap-drag",
        slide: function () {
            $("#sizeMin").html($("#sliderSize").val()[0].substring(0, $("#sliderSize").val()[0].length - 1));
            $("#sizeMax").html($("#sliderSize").val()[1].substring(0, $("#sliderSize").val()[1].length - 1));
        }
    });
    $("#sliderRuntime").noUiSlider({
        range: [1, 300],
        start: [1, 300],
        connect: true,
        margin: 25,
        step: 1,
        behaviour: "tap-drag",
        slide: function () {
            $("#runtimeMin").html($("#sliderRuntime").val()[0].substring(0, $("#sliderRuntime").val()[0].length - 3));
            $("#runtimeMax").html($("#sliderRuntime").val()[1].substring(0, $("#sliderRuntime").val()[1].length - 3));
        }
    });
    $("#sliderYear").noUiSlider({
        range: [1937, 2014],
        start: [1937, 2014],
        connect: true,
        margin: 1,
        step: 1,
        behaviour: "tap-drag",
        slide: function () {
            $("#yearMin").html($("#sliderYear").val()[0].substring(0, $("#sliderYear").val()[0].length - 3));
            $("#yearMax").html($("#sliderYear").val()[1].substring(0, $("#sliderYear").val()[1].length - 3));
        }
    });
    
    $("#croatianSubSlider").addClass('toggle').noUiSlider({
        range: [0, 1]
	    , start: 0
	    , handles: 1
	    , step: 1
	    , orientation: "vertical"
	    , serialization: {
	        resolution: 1
		    , to: function (value) {
		        $(this).toggleClass('off', !parseInt(value));
		    }
	    }
    });
    $("#englishSubSlider").addClass('toggle').noUiSlider({
        range: [0, 1]
	    , start: 0
	    , handles: 1
	    , step: 1
	    , orientation: "vertical"
	    , serialization: {
	        resolution: 1
		    , to: function (value) {
		        $(this).toggleClass('off', !parseInt(value));
		    }
	    }
    });
}
function addEventListeners() {
    $(document).keyup(function (e) {
        if (e.keyCode == 27) {
            slideUpSearchAdvanced();
        }
    });
    $("#movieList").keyup(function (e) {
        if (e.keyCode == 13) {
            selectMovie();
        }
    });
    $("#sessionSelected").keyup(function (e) {
        if (e.keyCode == 13) {
            unselectMovie();
        }
    });
    $("#searchMainInput").keyup(function (e) {
        if (e.keyCode == 13) {
            fillListBox();
        }
    });
    $("#searchActor").keyup(function (e) {
        if (e.keyCode == 13) {
            fillListBox();
        }
    });
    $("#searchDirector").keyup(function (e) {
        if (e.keyCode == 13) {
            fillListBox();
        }
    });
    $("#imdbIcon").click(function (e) {
        window.open(imdbLink, '_blank');
    });
    $('#trailerIcon').click(function () {
        $('#trailerWindowContent').jqxWindow('open');
    });
}
function initRestComponents () {
    $(document).tooltip({
        track: true
    });

    $("#selectGenre").multiselect({
        buttonWidth: '400px',
        buttonText: function (options, select) {
            if (options.length == 0) {
                return "Odaberi zanrove" + ' <b class="caret"></b>';
            }
            else {
                if (options.length > 3) {
                    return options.length + " odabrano" + ' <b class="caret"></b>';
                }
                else {
                    var selected = '';
                    options.each(function () {
                        var label = ($(this).attr('label') !== undefined) ? $(this).attr('label') : $(this).html();

                        selected += label + ', ';
                    });
                    return selected.substr(0, selected.length - 2) + ' <b class="caret"></b>';
                }
            }
        }
    });

    $('#sortSelectProperty').fancySelect();
}

function preloadIcons() {
    preload(['/images/addIconGl_hover.png',
                '/images/addIconGl_click.png',
                '/images/removeIconSp_hover.png',
                '/images/removeIconSp_click.png',
                '/images/saveIcon_hover.png',
                '/images/saveIcon_click.png',
                '/images/imdbIcon_hover.png',
                '/images/imdbIcon_click.png',
                '/images/trailerIcon_hover.png',
                '/images/trailerIcon_click.png',
                '/images/reload.png',
                '/images/reload_click.png',
                '/images/reload_hover.png',
    ]);
}
function preload(arrayOfImages) {
    $(arrayOfImages).each(function () {
        $('<img/>')[0].src = this;
        // Alternatively you could use:
        // (new Image()).src = this;
    });
}

function loadInitData() {
    //ako ne postoji nista u local storage-u, dohvati sve filmove koje nakon toga spremi u local storage
    if (localStorage.getItem("movieList") == null) {
        movies = new Array();
        getSetMoviesFromJsonResult("allMovies");
    }
    //inace, prikazi one koji postoje, te u medjuvremenu provjeri da li ima nesto novo od datuma spremljenog u local storage
    else {
        var retreived = localStorage.getItem("movieList");
        jsonAsTextToMovieList(retreived, false);

        lastDate = new Date(localStorage.getItem("lastDateYear"),
        localStorage.getItem("lastDateMonth") - 1,
        localStorage.getItem("lastDateDay"),
        localStorage.getItem("lastDateHours"),
        localStorage.getItem("lastDateMinutes"),
        localStorage.getItem("lastDateSecondss"),
        0);

        getSetMoviesFromJsonResult("moviesAfterDate?year=" + lastDate.getFullYear() +
        "&month=" + (lastDate.getMonth() + 1) +
        "&day=" + lastDate.getDate() +
        "&hours=" + lastDate.getHours() +
        "&minutes=" + lastDate.getMinutes() +
        "&seconds=" + lastDate.getSeconds());
    }  

    //spremi trenutni datum u local storage
    lastDate = new Date();
    localStorage.setItem("lastDateDay", lastDate.getDate());
    localStorage.setItem("lastDateMonth", lastDate.getMonth() + 1);
    localStorage.setItem("lastDateYear", lastDate.getFullYear());
    localStorage.setItem("lastDateHours", lastDate.getHours());
    localStorage.setItem("lastDateMinutes", lastDate.getMinutes());
    localStorage.setItem("lastDateSecondss", lastDate.getSeconds());
}

//#endregion

//#region SELECTING

function movieListChangedIndex(movieID) {
    selectedFocus = 0;
    showMovieDetails(movieID);
}
function sessionSelectedChangedIndex(movieID) {
    selectedFocus = 1;
    movieListChangedIndex(movieID);
}

function showMovieDetails(movieID) {
    var movie = movies[returnArrayIndex(movies, movieID)]
    if (selectedFocus) {
        movie = selectedMovies[returnArrayIndex(selectedMovies, movieID)];
    }

    if (movie.hasCroSub) {
        document.getElementById("subtitleCroIcon").src = "../images/subtitleCroIcon.png";
    }
    else {
        document.getElementById("subtitleCroIcon").src = "../images/1pixel.png";
    }
    if (movie.hasEngSub) {
        document.getElementById("subtitleEngIcon").src = "../images/subtitleEngIcon.png";
    }
    else {
        document.getElementById("subtitleEngIcon").src = "../images/1pixel.png";
    }
    $("#rating").html(movie.rating.toFixed(1));
    $("#plot").html(movie.plot);
    $("#dateAdded").html(movie.dateAdded);
    $("#size").html(getSize(movie.size).toFixed(2) + " GB");
    $("#runtime").html(getRuntimeInText(movie.runtime));
    $("#resolution").html(getResolution(movie.videoRes));
    $("#cast").html(movie.cast);
    $("#director").html(movie.director);
    $("#name").html(movie.origName + " (" + movie.year + ")");
    $("#genres").html(movie.genre);

    var posterLink = posterMovieUrl + movie.name.replace("&#39;", "'").replace("&amp;", "&") + ".jpg";
    var imageObject = document.getElementById("moviePoster");
    imageObject.src = posterLink;

    var youtubeID = movie.trailerLink.substring(movie.trailerLink.indexOf("?v=") + 3);
    imdbLink = movie.imdbLink;

    document.getElementById("movieTrailerFrame").src = "http://www.youtube.com/embed/" + youtubeID;
}

function selectMovie() {
    //reloadData();

    var listBox = document.getElementById("movieList");
    var selectedListBox = document.getElementById("sessionSelected");

    //add to array and sort
    //selectedMovieIDs[selectedMovieIDs.length] = listBox.value;
    selectedMovies[selectedMovies.length] = movies[returnArrayIndex(movies, listBox.value)];
    sortSelectedMovies();

    updateTotalSizeLabel();

    //remove from movie listbox 
    var selectedIndex = listBox.selectedIndex;
    listBox.remove(listBox.selectedIndex);
    //sortMovies(document.getElementById("sortElementSelect").value);
    listBox.selectedIndex = selectedIndex;
    showMovieDetails(listBox[selectedIndex].value);

    //fill selected array
    fillSelectedListbox();
}
function unselectMovie() {
    //remove from array
    var selectedListBox = document.getElementById("sessionSelected");
    var selectedMovieID = selectedListBox.value;
    selectedMovies.splice(selectedMovies.indexOf(selectedMovieID));

    updateTotalSizeLabel();

    //remove from selected listbox
    var selectedIndex = selectedListBox.selectedIndex;
    selectedListBox.remove(selectedListBox.selectedIndex);
    if (selectedIndex > 0 && selectedIndex >= selectedListBox.options.length) {
        selectedIndex = selectedListBox.options.length - 1;
    }
    selectedListBox.selectedIndex = selectedIndex;

    //add to movie listbox
    var listBox = document.getElementById("movieList");
    var selectedIndex = listBox.selectedIndex;
    sortMovies(document.getElementById("sortElementSelect").value);
    listBox.selectedIndex = selectedIndex;
    showMovieDetails(selectedListBox[selectedListBox.selectedIndex].value);
}
function updateTotalSizeLabel() {
    var totalSize = 0;
    for (var i = 0; i < selectedMovies.length; i++) {
        totalSize += selectedMovies[i].size;
    }
    $("#totalSelectedSizeLabel").html(getSize(totalSize).toFixed(2) + " GB");

}
function isSelected(movieID) {
    for (var i = 0; i < selectedMovies.length; i++) {
        if (selectedMovies[i].id == movieID) {
            return true;
        }
    }
    return false;
}

function saveSelected() {
    $.getScript("../Scripts/FileSaver.min.js", function () {
        var selectedListBox = document.getElementById("sessionSelected");
        var selectedString = "";

        for (var i = 0; i < selectedMovies.length; i++) {
            selectedString += selectedMovies[i].name + "\n";
        }
        var blob = new Blob([selectedString], { type: "text/plain;charset=utf-8" });
        saveAs(blob, "Odabrani filmovi.txt");
    });
}

//#endregion

//#region FILL LISTBOXES

function fillListBox() {
    if (changedSorting) {
        changedSorting = false;
        reloadData();
        return;
    }
    $("#reloadDiv").css("display", "block");
    document.getElementById("movieList").options.length = 0;
    var counter = 0;
    setTimeout(function () {
        for (var i = 0; i < movies.length; i++) {
            if (isSelected(movies[i].id) == false && doesComplyFilters(movies[i].id)) {
                document.getElementById("movieList").options[counter] = new Option(movies[i].name.replace("&#39;", "'").replace("&amp;", "&"));
                document.getElementById("movieList").options[counter].value = movies[i].id;
                counter++;
            }
        }
        $("#labelCount").html(document.getElementById("movieList").options.length);
        $("#reloadDiv").css("display", "none");
    }, 200);
    
}
function fillSelectedListbox() {
    var selectedListBox = document.getElementById("sessionSelected");
    selectedListBox.options.length = 0;

    for (var i = 0; i < selectedMovies.length; i++) {
        selectedListBox.options[i] = new Option(selectedMovies[i].name.replace("&#39;", "'"));
        selectedListBox.options[i].value = selectedMovies[i].id;
    }
}

function doesComplyFilters(movieID) {
    var index = returnArrayIndex(movies, movieID);
    if (movies[index].name.trim().toLowerCase().indexOf($("#searchMainInput").val()) < 0 || 
        movies[index].origName.trim().toLowerCase().indexOf($("#searchMainInput").val()) < 0) {
        return false;
    }
    if (movies[index].cast.trim().toLowerCase().indexOf($("#searchActor").val()) < 0) {
        return false;
    }
    if (movies[index].director.trim().toLowerCase().indexOf($("#searchDirector").val()) < 0) {
        return false;
    }
    if ($("#selectGenre").val() != null) {
        var genresSplitArray = $("#selectGenre").val().toString().split(',');
        for (var i = 0; i < genresSplitArray.length; i++) {
            if (genresSplitArray[i].trim().length < 1) {
                continue;
            }
            if (movies[index].genre.trim().toLowerCase().indexOf(genresSplitArray[i].trim().toLowerCase()) < 0) {
                return false;
            }
        }
    }
    /*if (movies[index].genre.trim().toLowerCase().indexOf($("#selectGenre").val()) < 0 ) {
        return false;
    }*/
    if (getSize(movies[index].size) < $("#sliderSize").val()[0] ||
        getSize(movies[index].size) > $("#sliderSize").val()[1]) {
        return false;
    }
    if (getRuntimeInMinutes(movies[index].runtime) < $("#sliderRuntime").val()[0] ||
        getRuntimeInMinutes(movies[index].runtime) > $("#sliderRuntime").val()[1]) {
        return false;
    }
    if (movies[index].rating != 0 && (movies[index].rating < $("#sliderImdb").val()[0] ||
        movies[index].rating > $("#sliderImdb").val()[1])) {
        return false;
    }
    if (movies[index].year < $("#sliderYear").val()[0] ||
        movies[index].year > $("#sliderYear").val()[1]) {
        return false;
    }
    if (movies[index].hasCroSub == false && $("#croatianSubSlider").val()[0] == 1) {
        return false;
    }
    if (movies[index].hasEngSub == false && $("#englishSubSlider").val()[0] == 1) {
        return false;
    }
    return true;
}

//#endregion

//#region SORTING

function changedSortAttribute() {
    changedSorting = true;
}
function sortMovies(byProperty) {
    var listBox = document.getElementById("movieList");
    listBox.options.length = 0;

    var inputText = document.getElementById("searchMain").value;

    if (inputText.length > 0) {
        var countMovies = 0;
        for (var i = 0; i < nameArray.length; i++) {
            isSelected = false;
            if (nameArray[i].toLowerCase().indexOf(inputText.toLowerCase()) > -1 && isSelected(idArray[i]) == false) {
                listBox.options[countMovies] = new Option(nameArray[i].replace("&#39;", "'").replace("&amp;", "&"));
                listBox.options[countMovies].value = idArray[i];
                countMovies++;
            }
        }
    }
    else {
        fillListBox();
    }

    if (byProperty == "name") {
        //nista ne radi, vec je sortirano po imenu
    }
    else if (byProperty == "date") {
        var swapped = true;
        while (swapped) {
            swapped = false;
            for (var i = 1; i < listBox.length; i++) {
                var index1 = returnArrayIndex(movies, listBox[i - 1].value);
                var index2 = returnArrayIndex(movies, listBox[i].value);
                if (dateAddedArray[index1] < dateAddedArray[index2]) {
                    listBox.options[i - 1] = new Option(nameArray[index2]);
                    listBox.options[i - 1].value = idArray[index2];
                    listBox.options[i] = new Option(nameArray[index1]);
                    listBox.options[i].value = idArray[index1];
                    swapped = true;
                }

            }
        }
    }
    else if (byProperty == "rating") {
        var swapped = true;
        while (swapped) {
            swapped = false;
            for (var i = 1; i < listBox.length; i++) {
                var index1 = returnArrayIndex(movies, listBox[i - 1].value);
                var index2 = returnArrayIndex(movies, listBox[i].value);
                if (ratingArray[index1] < ratingArray[index2]) {
                    listBox.options[i - 1] = new Option(nameArray[index2]);
                    listBox.options[i - 1].value = idArray[index2];
                    listBox.options[i] = new Option(nameArray[index1]);
                    listBox.options[i].value = idArray[index1];
                    swapped = true;
                }

            }
        }
    }
    else if (byProperty == "year") {
        var swapped = true;
        while (swapped) {
            swapped = false;
            for (var i = 1; i < listBox.length; i++) {
                var index1 = returnArrayIndex(movies, listBox[i - 1].value);
                var index2 = returnArrayIndex(movies, listBox[i].value);
                if (yearArray[index1] < yearArray[index2]) {
                    listBox.options[i - 1] = new Option(nameArray[index2]);
                    listBox.options[i - 1].value = idArray[index2];
                    listBox.options[i] = new Option(nameArray[index1]);
                    listBox.options[i].value = idArray[index1];
                    swapped = true;
                }

            }
        }
    }
}
function sortSelectedMovies() {
    var swapped = true;
    while (swapped) {
        swapped = false;
        for (var i = 1 ; i < selectedMovies.length ; i++) {
            if (selectedMovies[i].name.localeCompare(selectedMovies[i - 1].name) < 0) {
                var temp = selectedMovies[i];
                selectedMovies[i] = selectedMovies[i - 1];
                selectedMovies[i - 1] = temp;
                swapped = true;
            }
        }
    }
}

//#endregion

//#region SLIDING SEARCH PANEL  

function slideDownSearch() {
    $("#hideSearch").css("display", "block");
    $("#showSearch").css("display", "none");
    $("#hideSearchAdvanced").css("display", "none");
    $("#showSearchAdvanced").css("display", "block");

    $("#searchDiv").animate(
		{ top: '-225px' },
		800,
		function () {
		    $("#searchDiv").animate(
				{ top: '-230px' },
				80
			)
		}
	);
}
function slideUpSearch() {
    $("#hideSearch").css("display", "none");
    $("#showSearch").css("display", "block");
    $("#hideSearchAdvanced").css("display", "none");
    $("#showSearchAdvanced").css("display", "block");

    changeAdvancedElementsDisplay("none");

    $("#searchDiv").animate(
		{ top: '-305px' },
		800,
		function () {
		    $("#searchDiv").animate(
				{ top: '-300px' },
				80,
                function () {
                    $("#navigation-block").css("display", "block");
                }
			)
		}
	);
}
function slideDownSearchAdvanced() {
    $("#hideSearch").css("display", "block");
    $("#showSearch").css("display", "none");
    $("#hideSearchAdvanced").css("display", "block");
    $("#showSearchAdvanced").css("display", "none");
    $("#navigation-block").css("display", "none");


    $("#searchDiv").animate(
		{ top: '-5px' },
		800,
		function () {
		    $("#searchDiv").animate(
				{ top: '-10px' },
				80,
				function () {
				    changeAdvancedElementsDisplay("block");
				}
			)
		}
	);

}
function slideUpSearchAdvanced() {
    $("#hideSearch").css("display", "block");
    $("#showSearch").css("display", "none");
    $("#hideSearchAdvanced").css("display", "none");
    $("#showSearchAdvanced").css("display", "block");

    changeAdvancedElementsDisplay("none");

    $("#searchDiv").animate(
		{ top: '-235px' },
		800,
		function () {
		    $("#searchDiv").animate(
				{ top: '-230' },
				80,
                function () {
                    $("#navigation-block").css("display", "block");
                }
			)
		}
	);
}

function changeAdvancedElementsDisplay(status) {
    $("#searchImdbRating").css("display", status);
    $("#searchSize").css("display", status);
    $("#searchRuntime").css("display", status);
    $("#searchYear").css("display", status);
    $("#searchGenre").css("display", status);
    $("#searchActorDiv").css("display", status);
    $("#searchDirectorDiv").css("display", status);
    $("#croatianSubSearchDiv").css("display", status);
    $("#englishSubSearchDiv").css("display", status);
}

function showCurrentSession () {
    $("#currentSession").animate (
        {right: '-40%'},
        400,
        function () {
            $("#currentSession").animate (
                {right: '-39.5%'},
                80
            );
        }
    );

    $("#currentSessionInfo").animate(
        { right: '-40%' },
        400,
        function () {
            $("#currentSessionInfo").animate(
                { right: '-39.5%' },
                80,
                function () {
                    $("#currentSessionInfo").animate(
                        { top: '430px' },
                        200,
                        function () {
                            $("#currentSessionInfo").animate(
                                { top: '425px' },
                                80,
                                function () {
                                    if (loadData)
                                        loadInitData();
                                }
                            );
                        }
                    );
                }
            );
        }
    );
}

//#endregion


//#region UTIL FUNCTIONS

function getRuntimeInText(valueInMiliseconds) {
    var valueInMinutes = valueInMiliseconds / 60000;
    var valueInHours = valueInMinutes / 60;
    valueInMinutes = valueInMinutes % 60;
    if (valueInHours > 0) {
        return Math.floor(valueInHours) + "h " + Math.round(valueInMinutes) + "m";
    }
    else {
        return Math.round(valueInMinutes) + "m";;
    }
}
function getRuntimeInMinutes(valueInMiliseconds) {
    var valueInMinutes = valueInMiliseconds / 60000;
    return valueInMinutes;
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
function getResolution(heightValue) {
    if (heightValue > 720) {
        return "1080p";
    }
    else {
        return "720p";
    }
}
function returnArrayIndex(movieList, movieID) {
    var index;
    for (var i = 0; i < movieList.length; i++) {
        if (movieList[i].id == movieID) {
            index = i;
            break;
        }
    }
    return index;
}

//#endregion

function reloadData() {
    movies = new Array();
    var requestUrl = "allMoviesOrdered?sortProperty=" + $("#sortSelectProperty").val();
    getSetMoviesFromJsonResult(requestUrl);
    //slideUpSearchAdvanced();
}

function getSetMoviesFromJsonResult(requestUrl) {
    //alert(requestUrl);
    var self = this;
    if (movies.length == 0) {
        $("#reloadIndicator").css("display", "block");
        $("#reloadDiv").css("display", "block");
    }
    else {
        $('#miniReloadDiv').css('display', 'block');
    }
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
            
            $("#reloadIndicator").css("display", "none");
            $("#reloadDiv").css("display", "none");
            $("#miniReloadDiv").css("display", "none");
            $("#labelCount").html(movies.length);
        },
        error: function (e) {
            $("#reloadIndicator").css("display", "none");
            $("#miniReloadDiv").css("display", "none");
            $("#reloadDiv").css("display", "none");
            $("#labelCount").html(movies.length);
        }
    });
}

function jsonAsTextToMovieList (text, isFullJson) {
    var result = jQuery.parseJSON(text);  
    if (result.length == 0) {
        return;
    }
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
    
    for (var i = 0; i < movies.length; i++) {
        newMovies[newMovies.length] = movies[i];
    }
    movies = newMovies;
    localStorage.setItem("movieList", JSON.stringify(movies));
    fillListBox();
}
function resetSearchForm() {
    $("#sliderImdb").val([1, 10]);
    $("#sliderSize").val([0, 50]);
    $("#sliderYear").val([1937, 2014]);
    $("#sliderRuntime").val([1, 300]);
    $("#searchMainInput").val("")
    $("#searchActor").val("")
    $("#searchDirector").val("")
    $('#croatianSubSlider').val(0);
    $('#englishSubSlider').val(0);
    //$("#selectGenre").multiselect('destroy');

    for (var i = 0; i < selectGenre.options.length; i++) {
        $("#selectGenre").multiselect('deselect', selectGenre.options[i].value);
    }

    $("#selectGenre").multiselect({
        buttonWidth: '350px'
    });
}



