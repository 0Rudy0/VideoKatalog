$(document).ready(function () {    
    rearrangeLayoutMDetails();
})

function initAllMDetails() {
    window.onresize = debounce(function () {
        rearrangeLayoutMDetails();
    }, 10);
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

function selectSeason(seasonID) {
    $('#footer').css('display', 'block');
    var serieSeasons = sessionStorage.getItem('serieSeasons');
    if (serieSeasons == null) {
        serieSeasons = new Array();
    }
    serieSeasons.push(seasonID);
    sessionStorage.setItem('serieSeasons', serieSeasons);
}

function selectEpisode(episodeID) {
    $('#footer').css('display', 'block');
    var serieEpisodes = sessionStorage.getItem('serieEpisodes');
    if (serieEpisodes == null) {
        serieEpisodes = new Array();
    }
    serieEpisodes.push(episodeID);
    sessionStorage.setItem('serieEpisodes', serieEpisodes);
}

