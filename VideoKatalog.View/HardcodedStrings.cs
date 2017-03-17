using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {
    public static class HardcodedStrings {

        public static string imdbMainString = "http://www.imdb.com";
        public static string appDataFolder = Environment.GetFolderPath (Environment.SpecialFolder.Personal) + @"/Video Katalog/";
        public static string appExecutablePath = System.Reflection.Assembly.GetExecutingAssembly ().Location;
        public static string posterFolder = HardcodedStrings.appDataFolder + @"/Images/";
        //public static string posterFolder2 = Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData) +  @"/Images/";
        public static string moviePosterFolder = HardcodedStrings.posterFolder + @"movie/";
        public static string seriePosterFolder = HardcodedStrings.posterFolder + @"serie/";
        public static string homeVideoPosterFolder = HardcodedStrings.posterFolder + @"Home Video/";

        //FREE
        public static string ftpAdress = "ftp://hddpunjenje.herobo.com/public_html/posters/";
        public static string ftpAppUpdatesPath = "ftp://ftp.hdfilmovi.org/public_html/updates/Video Katalog.exe"; 
        public static string ftpUsername = "a7194937";
        public static string ftpPassword = "rudXYZ1%";

        //HOSTING24
        /*public static string ftpAdress = "ftp://ftp.hdfilmovi.org/public_html/posters/";
        public static string ftpAppUpdatesPath = "ftp://ftp.hdfilmovi.org/public_html/updates/Video Katalog.exe"; 
        public static string ftpUsername = "hdfilmov";
        public static string ftpPassword = "rudXYZ1%";*/

       

        #region Postfixes

        public static string postfixFullCredits = "fullcredits";
        public static string postfixSummary = "plotsummary";
        public static string postfixBusiness = "business";
        public static string postfixEpisodes = "episodes";

        #endregion

        #region Search results

        public static string onlySearchPageContainsString = "<title>IMDb Title Search</title>";

        public static string resultPopularTitlesBeginsWith = "Popular Titles";
        public static string resultExachMatchesBeginsWith = "Titles (Exact Matches)";
        public static string resultPartialMatchesBeginsWith = "Titles (Partial Matches)";
        public static string resultApproxMatchesBeginsWith = "Titles (Approx Matches)";

        public static string resultMovieNameStartsWith1 = "onclick";
        public static string resultTitleLinkStartsWith1 = "\"";

        public static string resultYearStartsWith1 = "</a>";
        public static string resultYearStartsWith2 = "(";
        public static string resultYearEndsWith = ")";

        public static string resultOriginalNameStartsWith1 = "<p class=\"find-aka\">";
        public static string resultOriginalNameStartsWith2 = "<em>(original title)</em>";

        #endregion

        #region Trailer and poster

        public static string youtubeTrailerSearchPrefix = "http://www.youtube.com/results?search_query=";
        public static string youtubeTrailerSearchSuffix = "+trailer&aq=f";
        public static string googleTrailerSearchPrefix = "http://www.google.com/webhp?hl=hr#sclient=psy&hl=hr&site=webhp&source=hp&q=";
        public static string googleTrailerSearchSuffix = "+trailer&aq=f&aqi=&aql=&oq=&pbx=1&fp=ebd6b5bbc36e5c9c";

        public static string impAwardsPosterSearchPrefix = "http://www.impawards.com/googlesearch.html?cx=partner-pub-6811780361519631%3A48v46vdqqnk&cof=FORID%3A9&ie=ISO-8859-1&q=";
        public static string impAwardsPosterSearchSuffix = "&sa=Search&siteurl=www.impawards.com%252F#860";
        public static string googlePosterSearchPrefix = "http://www.google.hr/search?q=";
        public static string googlePosterSearchSuffix = "+poster&hl=hr&client=firefox-a&hs=fVv&rls=org.mozilla:en-US:official&prmd=ivns&tbm=isch&tbo=u&source=univ&sa=X&ei=HqiyTfHHCMiZOrORwYAJ&ved=0CBkQsAQ&biw=1440&bih=925";
        public static string posterDBSearchPrefix = "http://www.movieposterdb.com/movie/";

        public static string posterSourceBeginsWith1 = "<td class=\"poster\" id=\"";
        public static string posterSourceEndsWith1 = "<td class=\"poster\" id=\"";
        public static string posterSourceBeginsWith2 = "<img src=\"";
        public static string posterSourceEndsWith2 = "\"";
       
        public static string firstTrailerBeginsWith1 = "<div id=\"search-results\">";
        public static string firstTrailerBeginsWith2 = "<a href=\"";
        public static string firstTrailerEndsWith1 = "</div>";
        public static string firstTrailerEndsWith2 = "\" class=\"";

        //World wide box office adress bar search string
        public static string WWOBOfficePreSearchString = "http://www.worldwideboxoffice.com/movie.cgi?title=";
        public static string WWOBOfficePostSearchString = "&year=";

        public static string postersPagePreString = "http://www.movieposterdb.com/movie/";

        #endregion

        #region Title, year, rating

        public static string titleBeginsWith = "<title>";
        public static string titleEndsWith1 = "</title>";
        public static string titleEndsWith2 = "(";

        public static string origTitleBeginsWith = "<span class=\"title-extra\">";
        public static string origTitleEndsWith = "<i>(original title)";

        public static string yearBeginsWith = "(";
        public static string yearEndsWith = ")";

        public static string ratingBeginsWith = "class=\"rating-rating\">";
        public static string ratingEndsWith = "<span>/10</span>";

        #endregion

        #region Cast and crew

        public static string directorsBeginsWith1 = "name=\"directors\"";
        public static string directorsBeginsWith2 = "<a href=";
        public static string directorsSplitString = "<a href=\"";
        public static string directorsEndsWith = "name=\"writers\"";
        public static string directorsEndsWithAlt = "name=\"cast\"";

        public static string castBeginsWith1 = "class=\"cast\"";
        public static string castBeginsWith2 = "<a href=\"";
        public static string castSplitString = "<a href=\"";
        public static string castEndsWith1 = ">Create a character page for:&#32;<select name=\"name\">";
        public static string uniqueActorString = "<td class=\"ddd\">";
        public static string actorBeginsWith = "\">";
        public static string actorEndsWith = "</a>";

        public static string starCastBeginsWith1 = "name=\"description\" content=\"";
        public static string starCastEndsWith = ".\" />";
        public static string starCastBeginsWith2 = ". With";
        public static char starCastSplitString = ',';

        #endregion

        #region Summary

        public static string fullSummaryBeginsWith1 = "<p class=\"plotpar\">\n";
        public static string fullSummaryEndsWith = "\n";
        public static string shortSummaryBeginsWith1 = "href=\"http://www.metacritic.com\"";
        public static string shortSummaryEndsWith = "<div class=\"txt-block\">";
        public static string shortSummaryBeginsWith2 = "<p>\n";
        public static string shortSummaryEndsWith2 = "\n</p>";

        #endregion

        #region Budget, earnings

        public static string budgetBeginsWith = "<h5>Budget</h5>\n";
        public static string budgetBeginsWith2 = "$";
        public static string budgetEndsWith = "<h5>";
        public static string budgetEndsWith2 = "(";
        //string budgetEndsWith = "<br/>";
        public static string earningsBeginsWith = "Worldwide Gross:</TD><TD>";
        public static string earningsEndsWith = "</TD>";

        #endregion

        #region Genres

        public static string genresBeginsWith = "<div class=\"infobar\">\n";
        public static string genresSplitString = "href=\"/genre/";
        public static string eachGenreBeginsWith = ">";
        public static string eachGenreEndsWith = "</a>";
        public static string genresEndsWith = "</div>";

        #endregion

        #region Serie

        //****  SERIE  **** 
        public static string serieSeasonSplitString = "class=\"season-header\"";
        public static string serieSeasonsEndsWith = "<h3>Related Links</h3>";

        //****  SEASON  ****
        public static string serieSeasonNameStartsWith = ">\n";
        public static string serieSeasonNameEndsWith = "\n";
        public static string serieSeasonLinkStartsWith = "href=\"";
        public static string serieSeasonLinkEndsWith = "\">";

        //****  EPISODE****
        public static string serieEpisodeSplitString = "<div class=\"filter-all";
        public static string serieEpisodeNameStartsWith = "\">";
        public static string serieEpisodeNameEndsWIth = "</a>";
        public static string serieEpisodeLinkStartsWith = "<a href=\"";
        public static string serieEpisodeLinkEndsWith = "\">";
        public static string serieEpisodeAirDateStartsWith = "Original Air Date&mdash;<strong>";
        public static string serieEpisodeAirDateEndsWith = "</strong>";
        public static string serieEpisodeSummaryStartsWith = "</span><br>";
        public static string serieEpisodeSummaryEndsWith = "</td></tr></table></div>";
        public static string serieEpisodeSummaryEndsWithAlt = "<br/><h5>";

        #endregion        
    }
}
