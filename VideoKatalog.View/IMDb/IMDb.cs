using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Net;
using System.Data;
using System.Collections.ObjectModel;
using System.Windows;

namespace Video_katalog {
    class IMDb {

        #region GLOBAL DATA

        string mainLink;
        string mainSource;             
        public string notFetched = "";
        string earningsSource;

        #endregion

        #region HARDCODED SEARCH STRINGS

        string imdbMainString = "http://www.imdb.com";

        #region Postfixes

        string postfixFullCredits = "fullcredits";
        string postfixSummary = "plotsummary";
        string postfixBusiness = "business";
        string postfixEpisodes = "episodes";

        #endregion

        #region Search results

        string onlySearchPageContainsString = "<title>IMDb Title Search</title>";

        string resultPopularTitlesBeginsWith = "Popular Titles";
        string resultExachMatchesBeginsWith = "Titles (Exact Matches)";
        string resultPartialMatchesBeginsWith = "Titles (Partial Matches)";
        string resultApproxMatchesBeginsWith = "Titles (Approx Matches)";

        string resultMovieNameStartsWith1 = "onclick";
        string resultTitleLinkStartsWith1 = "\"";

        string resultYearStartsWith1 = "</a>";
        string resultYearStartsWith2 = "(";
        string resultYearEndsWith = ")";

        string resultOriginalNameStartsWith1 = "<p class=\"find-aka\">";
        string resultOriginalNameStartsWith2 = "<em>(original title)</em>"; 

        #endregion

        #region Trailer and poster

        string youtubeTrailerSearchPrefix = "http://www.youtube.com/results?search_query=";
        string youtubeTrailerSearchSuffix = "+trailer\"&aq=f";
        string impAwardsPosterSearchPrefix = "http://www.impawards.com/googlesearch.html?cx=partner-pub-6811780361519631%3A48v46vdqqnk&cof=FORID%3A9&ie=ISO-8859-1&q=";
        string impAwardsPosterSearchSuffix = "&sa=Search&siteurl=www.impawards.com%252F#860";

        string firstTrailerBeginsWith1 = "<div id=\"search-results\">";
        string firstTrailerBeginsWith2 = "<a href=\"";
        string firstTrailerEndsWith1 = "</div>";
        string firstTrailerEndsWith2 = "\" class=\"";

        //World wide box office adress bar search string
        string WWOBOfficePreSearchString = "http://www.worldwideboxoffice.com/movie.cgi?title=";
        string WWOBOfficePostSearchString = "&year=";

        string postersPagePreString = "http://www.movieposterdb.com/movie/";

        #endregion

        #region Title, year, rating

        string titleBeginsWith = "<title>";
        string titleEndsWith1 = "</title>";
        string titleEndsWith2 = "(";

        string origTitleBeginsWith = "<span class=\"title-extra\">";
        string origTitleEndsWith = "<i>(original title)";

        string yearBeginsWith = "(";
        string yearEndsWith = ")";

        string ratingBeginsWith = "itemprop=\"ratingValue\">";
        string ratingEndsWith = "</span>";

        #endregion

        #region Cast and crew

        string directorsBeginsWith1 = "name=\"directors\"";
        string directorsBeginsWith2 = "<a href=";
        string directorsSplitString = "<a href=\"";
        string directorsEndsWith = "name=\"writers\"";
        string directorsEndsWithAlt = "name=\"cast\"";

        string castBeginsWith1 = "class=\"cast\"";
        string castBeginsWith2 = "<a href=\"";
        string castSplitString = "<a href=\"";
        //string castEndsWith1 = "\n";
        string castEndsWith1 = ">Create a character page for:&#32;<select name=\"name\">";
        string uniqueActorString = "<td class=\"ddd\">";
        string actorBeginsWith = "\">";
        string actorEndsWith = "</a>";

        string starCastBeginsWith1 = "name=\"description\" content=\"";
        string starCastEndsWith = ".\" />";
        string starCastEndsWith2 = ". ";
        string starCastBeginsWith2 = ". With";
        char starCastSplitString = ',';

        #endregion

        #region Summary

        string fullSummaryBeginsWith1 = "<p class=\"plotpar\">\n";
        string fullSummaryEndsWith = "\n";
        string shortSummaryBeginsWith1 = "<h2>Storyline</h2>";
        string shortSummaryEndsWith = "\n";
        string shortSummaryBeginsWith2 = "<p>";

        #endregion

        #region Budget, earnings

        string budgetBeginsWith = "<h5>Budget</h5>\n";
        string budgetBeginsWith2 = "$";
        string budgetEndsWith = "<h5>";
        string budgetEndsWith2 = "(";
        //string budgetEndsWith = "<br/>";
        string earningsBeginsWith = "Worldwide Gross:</TD><TD>";
        string earningsEndsWith = "</TD>";

        #endregion

        #region Genres

        string genresBeginsWith = "<div class=\"infobar\">\n";
        string genresSplitString = "href=\"/genre/";
        string eachGenreBeginsWith = ">";
        string eachGenreEndsWith = "</a>";
        string genresEndsWith = "</div>";

        #endregion

        #region Serie

        //****  SERIE  **** 
        string serieSeasonSplitString = "class=\"season-header\"";
        string serieSeasonsEndsWith = "<h3>Related Links</h3>";

        //****  SEASON  ****
        string serieSeasonNameStartsWith = ">\n";
        string serieSeasonNameEndsWith = "\n";
        string serieSeasonLinkStartsWith = "href=\"";
        string serieSeasonLinkEndsWith = "\">";        

        //****  EPISODE****
        string serieEpisodeSplitString = "<div class=\"filter-all";
        string serieEpisodeNameStartsWith = "\">";
        string serieEpisodeNameEndsWIth = "</a>";
        string serieEpisodeLinkStartsWith = "<a href=\"";
        string serieEpisodeLinkEndsWith = "\">";
        string serieEpisodeAirDateStartsWith = "Original Air Date&mdash;<strong>";
        string serieEpisodeAirDateEndsWith = "</strong>";
        string serieEpisodeSummaryStartsWith = "</span><br>";
        string serieEpisodeSummaryEndsWith = "</td></tr></table></div>";
        string serieEpisodeSummaryEndsWithAlt = "<br/><h5>";

        #endregion

        #endregion

        #region CONSTRUCTORS

        public IMDb (string mainLink) {
            this.mainLink = mainLink;            
        }
        public IMDb () {
        }

        #endregion

        #region SEARCH RESULTS

        public ObservableCollection<SearchResult> GetSearchResults (string searchLink, int maxResults) {
            string pageSource;
            try {
                pageSource = ReturnWebPageSource (searchLink);
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Problem sa internet konekcijom", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return new ObservableCollection<SearchResult> ();
            }
            ObservableCollection<SearchResult> result = new ObservableCollection<SearchResult> ();
            if (pageSource.Contains ("<span id=\"findSubHeaderLabel\">Search category: </span>") == false) {
                //ako ne sadrzi navedeni string znaci da je rezultat pretrage samo jedan film i IMDb nas je preusmjerio na stranicu tog filma,
                //a ta stranica ne sadrzava navedeni string
                this.mainSource = pageSource;
                string name = GetOriginalName (pageSource);
                if (name == null) {
                    name = GetName (pageSource);
                }
                SearchResult onlyOne = new SearchResult ();
                onlyOne.Name = name;
                int startSelfLink = pageSource.IndexOf ("link rel=\"canonical\"");
                onlyOne.Link = pageSource.Substring (startSelfLink);
                startSelfLink = onlyOne.Link.IndexOf ("href=\"");
                onlyOne.Link = onlyOne.Link.Substring (startSelfLink + 6); //6 je duljina gornjeg stringa
                int endSelfLink = onlyOne.Link.IndexOf ("\"");
                onlyOne.Link = onlyOne.Link.Substring (0, endSelfLink);
                result.Add (onlyOne);
                return result;
            }          

            string results = TryToFetchResult(pageSource, "<table class=\"findList\">");
            if (results != null) {
                FillListToReturn(ref result, "popular", results, maxResults);
                if (result.Count == maxResults)
                    return result;
            }

            return result;
        }
        void FillListToReturn(ref ObservableCollection<SearchResult> list, string description, string source, int maxResults) {
            string allResultsEndsWith = "</td> </tr></table>";
            int indexOfEnd = source.IndexOf(allResultsEndsWith);
            //ako ne postoji rezultat pretrage, vrati null
            if (source == null || indexOfEnd < 0) {
                return;
            }
            source = source.Substring(0, indexOfEnd);

            foreach (string searchResult in HelpFunctions.SplitByString(source, "<tr class=\"findResult")) {
                SearchResult tempMovie = ExtractTitle(searchResult, description);
                if (tempMovie == null) {
                    continue;
                }
                list.Add(tempMovie);
                if (list.Count == maxResults)
                    return;
            }

        }
        SearchResult ExtractTitle (string searchResult, string description) {
            if (searchResult.Contains("<a href=") == false) {
                return null;
            }
            SearchResult resultToReturn = new SearchResult();
            resultToReturn.Description = description;
            string movieLink = HelpFunctions.GetStringBetweenStrings(searchResult, "<a href=\"", "/?ref_=fn");
            movieLink = movieLink.Replace("\"", "");
            movieLink = movieLink.Trim();
            resultToReturn.Link = "http://www.imdb.com" + movieLink;

            string movieName = HelpFunctions.GetStringFromStringToEnd(searchResult, "<td class=\"result_text\">");
            movieName = HelpFunctions.GetStringFromStringToEnd(movieName, "<a href=\"");
            movieName = HelpFunctions.GetStringFromStringToEnd(movieName, ">");
            string year = HelpFunctions.GetStringFromStringToEnd(movieName, "</a>");
            movieName = HelpFunctions.GetStringBetweenStrings(movieName, "", "</a>");
            year = HelpFunctions.GetStringBetweenStrings(year, "(", ")");
            resultToReturn.Name = movieName + " (" + year + ")";

            return resultToReturn;
        }       
        

        #endregion

        #region GET IMDB INFO

        public void SetMainSource () {
            mainSource = ReturnWebPageSource (mainLink);
        }
        public void SetEarningsSource (Movie newMovie) {
            string url = "http://www.worldwideboxoffice.com/movie.cgi?title=" +
                FormatStringForUrl(HttpUtility.HtmlDecode(newMovie.OrigName)) + "&year=" + newMovie.Year;
            if (GetMovieNumberInYear(mainSource) != "")
            {
                url += "/" + GetMovieNumberInYear(mainSource);
            }

            earningsSource = ReturnWebPageSource(url);
        }

        public Movie GetMovieInfo (string mainLink) {
            this.mainLink = mainLink;
            SetMainSource ();
            string castAndCrewSource = ReturnWebPageSource (mainLink + "/fullcredits");
            string budgetSource = ReturnWebPageSource (mainLink + "/business");
            string summarySource = ReturnWebPageSource (mainLink + "/plotsummary");

            if (string.IsNullOrEmpty (mainSource))
                return null;
            Movie newMovie = new Movie ();
            newMovie.Name = HttpUtility.HtmlDecode (GetName (mainSource));
            //newMovie.OrigName = newMovie.Name;
            newMovie.OrigName = HttpUtility.HtmlDecode(GetOriginalName(mainSource)); 
            newMovie.Year = GetYear (mainSource);                               
            if (String.IsNullOrWhiteSpace (newMovie.OrigName)) {
                newMovie.OrigName = newMovie.Name;
            }
            //string earningsSource;
            SetEarningsSource (newMovie);

            foreach (Person actor in GetCast (mainSource, castAndCrewSource)) {
                Person tempAct = actor;
                tempAct.Name = HttpUtility.HtmlDecode (tempAct.Name);
                newMovie.Actors.Add (tempAct);
            }
            foreach (Person dir in GetDirectors (castAndCrewSource)) {
                Person tempDir = dir;
                tempDir.Name = HttpUtility.HtmlDecode (tempDir.Name);
                newMovie.Directors.Add (tempDir);
            }
            foreach (Genre tempGenre in GetGenres (mainSource)) {
                newMovie.Genres.Add (tempGenre);
            }
            newMovie.Summary = HttpUtility.HtmlDecode (GetSummary (mainSource, summarySource));
            newMovie.Rating = GetRating (mainSource);
            newMovie.Budget = GetBudget (mainSource);
            newMovie.TrailerLink = GetTrailerLink (newMovie.OrigName);
            newMovie.Earnings = GetEarnings ();            
            newMovie.InternetLink = this.mainLink;
            //if (notFetched.Length != 0) {
            //    Xceed.Wpf.Toolkit.MessageBox.Show ("Slijedeće stvari nisu dohvaćene:" + notFetched);
            //}
            return newMovie;
        }
        public Serie GetSerieInfo (string mainLink) {
            this.mainLink = mainLink;
            string mainSource = ReturnWebPageSource (mainLink);
            string castAndCrewSource = ReturnWebPageSource(mainLink + "/fullcredits");
            //string budgetSource = ReturnWebPageSource (mainLink + VideoKatalog.View.Properties.Resources.imdbPostfixBudget);
            //string summarySource = ReturnWebPageSource (mainLink + VideoKatalog.View.Properties.Resources.imdbPostfixSummary);
            
            Serie newSerie = new Serie ();
            newSerie.Name = HttpUtility.HtmlDecode (GetName (mainSource));
            newSerie.OrigName = HttpUtility.HtmlDecode (GetOriginalName (mainSource));
            if (string.IsNullOrWhiteSpace (newSerie.OrigName))
                newSerie.OrigName = newSerie.Name;
            else
                newSerie.OrigName = RemoveQuotes (newSerie.OrigName);

            foreach (Person actor in GetCast (mainSource, castAndCrewSource)) {
                Person tempAct = actor;
                tempAct.Name = HttpUtility.HtmlDecode (tempAct.Name);
                newSerie.Actors.Add (tempAct);
            }
            foreach (Person dir in GetDirectors (castAndCrewSource)) {
                Person tempDir = dir;
                tempDir.Name = HttpUtility.HtmlDecode (tempDir.Name);
                newSerie.Directors.Add (tempDir);
            }
            foreach (Genre tempGenre in GetGenres (mainSource)) {
                newSerie.Genres.Add (tempGenre);
            }
            newSerie.Rating = GetRating (mainSource);
            newSerie.Summary = HttpUtility.HtmlDecode (GetSummary (mainSource, ""));
            newSerie.Seasons = GetSerieSeasons (mainLink, newSerie);

            return newSerie;
        }
       
        public string GetName (string source) {
            try {
                string movieName = GetStringBetweenStrings(source, "<script type=\"application", "</script>");
                movieName = GetStringBetweenStrings(movieName, "\"name\":", "\n");
                movieName = movieName.Trim ();
                movieName = movieName.Substring(1, movieName.Length - 3);
                //movieName = movieName.Replace ("IMDb - ", "");
                return movieName;
            }
            catch {
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju imena");
                return "";
            }
        }        
        public string GetOriginalName (string source) {
            try {                
                string originalName = GetStringBetweenStrings (source, "<div class=\"originalTitle\">", "<span class=\"description\">");
                originalName = originalName.Replace ("\n", "");
                originalName = originalName.Replace ("\r", "");
                originalName = originalName.Trim ();
                originalName = RemoveQuotes(originalName);
                return originalName;
            }
            catch {
                return "";
            }
        }
        public string GetMovieNumberInYear (string source) {
            try {
                string movieNumber = GetStringFromStringToEnd(source, "<h1 class=\"header\"> <span class=\"itemprop\" itemprop=\"name\">");
                movieNumber = GetStringFromStringToEnd(movieNumber, "<span>(");
                movieNumber = GetStringFromStartToString(movieNumber, ")</span>");
                if (movieNumber.Contains ("year"))
                    return "";
                else
                    return movieNumber;
            }
            catch {
                return "";
            }
        }
        public ObservableCollection<Genre> GetGenres (string source) {
            Genre tempGenre = new Genre ();
            var genres = DatabaseManager.FetchGenreList();
            ObservableCollection<Genre> movieGenres = new ObservableCollection<Genre> ();
            try {               
                string genresString = GetStringBetweenStrings (source, "<script type=\"application/", "\"actor\":");
                genresString = GetStringBetweenStrings(genresString, "\"genre\": [", "],");
                //genresString = genresString.Substring (genresString.IndexOf ("<"));
                foreach (string genre in SplitByString (genresString, ",")) {
                    if (genre.Trim() == "")
                        continue;
                    string genreTrim = genre;
                    try {
                        genreTrim = GetStringBetweenStrings (genreTrim, "\"", "\"");
                        //genreTrim = GetStringFromStringToEnd(genreTrim, ">");
                        genreTrim = genreTrim.Trim ();
                        //if (genreTrim == tempGenre.genresEng[i])
                        //{
                        //    Genre newGenre = new Genre();
                        //    newGenre.Name = tempGenre.genresCro[i];
                        //    movieGenres.Add(newGenre);
                        //}
                        for (int i = 0; i < tempGenre.genresEng.Count; i++)
                        {
                            if (genreTrim == tempGenre.genresEng[i])
                            {
                                Genre newGenre = new Genre();
                                newGenre.Name = tempGenre.genresCro[i];
                                movieGenres.Add(genres.Where(g => g.Name == tempGenre.genresCro[i]).FirstOrDefault());
                                break;
                            }
                        }
                    }
                    catch {
                        continue;
                    }
                }
            }
            catch {
                if (movieGenres.Count == 0) {
                    notFetched += "\r\nŽanrovi";
                    //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju žanrova");
                }
            }
            return movieGenres;
        }        
        public ObservableCollection<Person> GetDirectors (string source) {
            ObservableCollection<Person> directorList = new ObservableCollection<Person>();
            try {
                string directorsString = "";
                try {
                    //za filmove
                    directorsString = HelpFunctions.GetStringBetweenStrings(source, "<h4 class=\"dataHeaderWithBorder\">Directed by", "</table>");
                    if (directorsString == "") {
                        throw new Exception();
                    }
                }
                catch {
                    //za serije
                    directorsString = HelpFunctions.GetStringBetweenStrings(source, "<h4 class=\"dataHeaderWithBorder\">Series Directed by", "</table>");
                }
                directorsString = HelpFunctions.GetStringFromStringToEnd(directorsString, "<td class=\"name\">\n");
                foreach (string curDir in HelpFunctions.SplitByString(directorsString, "<a href=")) {
                    if (curDir.Trim() == "") {
                        continue;
                    }
                    string dirTrim = HelpFunctions.GetStringBetweenStrings(curDir, "> ", "\n</a>");
                    Person tempDirector = new Person(new PersonType(2));
                    tempDirector.Name = dirTrim;
                    directorList.Add(tempDirector);
                }
            }
            catch {
                if (directorList.Count == 0) {
                    notFetched += "\r\nRedatelj";
                }
            }
            return directorList;
        }        
        public ObservableCollection<Person> GetCast (string mainSource, string creditsSource) {
            ObservableCollection<Person> castList = new ObservableCollection<Person> ();
            try {
                foreach (Person actor in GetStarCast (mainSource)) {
                    if (string.IsNullOrWhiteSpace (actor.Name))
                        continue;
                    castList.Add (actor);
                }
                string castString;
                try {
                    castString = HelpFunctions.GetStringBetweenStrings(creditsSource, "<table class=\"cast_list\">", "</table>");
                }
                catch {
                    castString = HelpFunctions.GetStringBetweenStrings(creditsSource, "class=\"cast\"", "\n");
                }

                foreach (string actor in SplitByString(castString, "<tr class=\"")) {
                    if (actor.Contains ("<td class=\"primary_photo\">")) {
                        var td = SplitByString(actor, "<td").ElementAt(2);
                        var a = GetStringBetweenStrings(td, "<a href", "</a>");
                        var b = GetStringFromStringToEnd(a, ">").Trim();
                        //foreach (var td in SplitByString(actor, "<td").ElementAt(2))
                        //{

                        //}
                        //int actorStart = actor.IndexOf("\">") + ("\">").Length;
                        //int actorEnd = actor.IndexOf("</a>");
                        //string actorTrim = GetStringBetweenStrings (actor, "<span class=\"itemprop\" itemprop=\"name\">", "</span>");
                        Person tempActor = new Person (new PersonType (2));
                        tempActor.Name = b;
                        tempActor.DateOfBirth = new DateTime (1800, 1, 1);                        
                        if (castList.Contains (tempActor, new PersonEqualityComparerByName ())) {
                            continue;
                        }
                        else {
                            castList.Add (tempActor);
                        }
                    }
                }
            }
            catch {
                if (castList.Count == 0) {
                    notFetched += "\r\nUloge";
                    //Xceed.Wpf.Toolkit.MessageBox.Show ("Greska u dohvaćanju glumačke postave.");
                }
            }
            return castList;
        }
        private ObservableCollection<Person> GetStarCast (string source) {
            ObservableCollection<Person> starCast = new ObservableCollection<Person> ();
            try {
                string cast = GetStringBetweenStrings (source, "<h4 class=\"inline\">Stars:</h4>\n", "<a href=\"fullcredits");
                //cast = GetStringBetweenStrings(cast, this.starCastBeginsWith2, this.starCastEndsWith2);
                //cast = cast.Substring (cast.IndexOf (this.starCastBeginsWith2) + this.starCastBeginsWith2.Length);
                foreach (string actor in SplitByString (cast, "<a href=")) {
                    if (string.IsNullOrEmpty(actor) || !actor.Contains("a href="))
                        continue;
                    string actorCopy = actor;
                    actorCopy = GetStringFromStringToEnd(actorCopy, "<span class=\"itemprop\" itemprop=\"name\">");
                    actorCopy = GetStringFromStartToString(actorCopy, "</span>");
                    Person tempActor = new Person (new PersonType (2));
                    tempActor.DateOfBirth = new DateTime (1800, 1, 1);
                    tempActor.Name = actorCopy.Trim();
                    starCast.Add (tempActor);
                }
            }
            catch {
            }
            return starCast;
        }
        
        public int GetYear (string source) {
            try {
                string year = GetStringBetweenStrings (source, this.titleBeginsWith, this.titleEndsWith1);                
                year = GetStringBetweenStrings (year, this.yearBeginsWith, this.yearEndsWith);
                string yearFinall = null;
                foreach (char c in year) {
                    if (Char.IsDigit (c)) {
                        yearFinall += c.ToString ();
                    }
                }
                return Convert.ToInt32 (yearFinall);
            }
            catch {
                notFetched += "\r\nGodina";
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju godine");
                return 0;
            }
        }
        public decimal GetRating (string source) {
            try {
                string ratingString = GetStringBetweenStrings(source, "\"aggregateRating\"", "}");
                ratingString = GetStringBetweenStrings(source, "\"ratingValue\":", "\n");
                ratingString = ratingString.Replace("\"", "");
                //string ratingString = GetStringBetweenStrings (source, this.ratingBeginsWith, this.ratingEndsWith);
                //MessageBox.Show(ratingString);
                return Decimal.Parse (ratingString.Replace ('.', ','));
            }
            catch {
                notFetched += "\r\nOcjena";
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju ocjene");
                return 0;
            }
        }
        public string GetSummary (string mainSource, string summarySource) {
            try {
                string shortSummary = GetStringBetweenStrings(mainSource, "<div class=\"summary_text\">", "</div>");
                //shortSummary = GetStringBetweenStrings (shortSummary, this.shortSummaryBeginsWith2, this.shortSummaryEndsWith);
                shortSummary = shortSummary.Trim();
                if (string.IsNullOrWhiteSpace(shortSummary))
                    throw new Exception();
                if (shortSummary.Length > 2999)
                    return shortSummary.Substring(0, 2999);
                else
                    return shortSummary;
            }
            catch {
                notFetched += "\r\nRadnja";
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju radnje");
                return "";
            }
        }        
        public float GetBudget (string source) {
            try
            {
                string budgetString = HelpFunctions.GetStringBetweenStrings(source, "id=\"titleDetails\"", "id=\"titleDidYouKnow\"");
                budgetString = HelpFunctions.GetStringBetweenStrings(budgetString, "<h4 class=\"inline\">Budget:</h4>", "\n");
                //budgetString = HelpFunctions.GetStringBetweenStrings(budgetString, "$", "(");
                string budgetClean = null;
                foreach (char c in budgetString) {
                    if (Char.IsDigit(c)) {
                        budgetClean += c.ToString();
                    }
                }
                return float.Parse(budgetClean.ToString());
            }
            catch {
                notFetched += "\r\nBudget";
                return 0;
            }
        }
        public float GetEarnings () {
            try {
                string earningsString = GetStringBetweenStrings (earningsSource.ToLower(), "worldwide gross</td>", "</td>");
                earningsString = GetStringBetweenStrings(earningsString, "<span itemprop=\"value\">", "</span>");
                string earningsClean = null;
                foreach (char c in earningsString) {
                    if (Char.IsDigit (c)) {
                        earningsClean += c.ToString ();
                    }
                }
                if (float.Parse (earningsClean) == 0) {
                    notFetched += "\r\nZarada";
                }
                return float.Parse (earningsClean);
            }
            catch {
                notFetched += "\r\nZarada";
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Greška u dohvaćanju zarade");
                return 0;
            }
        }
        public string GetTrailerLink (string movieName) {
            string trailerLink = "";
            try {
                trailerLink = null;
                string trailerSearchResultSource = HelpFunctions.ReturnWebPageSource("http://www.youtube.com/results?search_query=" + movieName.Replace(' ', '+') + "+trailer");
                trailerLink = HelpFunctions.GetStringFromStringToEnd(trailerSearchResultSource, "<ol id=\"section-list");
                trailerLink = HelpFunctions.GetStringBetweenStrings(trailerLink, "<a aria-hidden=\"true\"", "class=");
                trailerLink = HelpFunctions.GetStringBetweenStrings(trailerLink, "href=\"", "\"");
                trailerLink = "www.youtube.com" + trailerLink;
            }
            catch {
                notFetched += "\r\nTrailer";
            }
            return trailerLink;
        }

        public ObservableCollection<SerieSeason> GetSerieSeasons (string mainLink, Serie parentSerie) {
            try {
                ObservableCollection<SerieSeason> seasons = new ObservableCollection<SerieSeason> ();
                int countSeasons = -1;
                while (true) {
                    string seasonEpisodesLink = mainLink + "/episodes?season=" + countSeasons.ToString ();
                    seasonEpisodesLink = seasonEpisodesLink.Replace("//episodes", "/episodes");
                    string seasonEpisodesSource = ReturnWebPageSource (seasonEpisodesLink);

                    SerieSeason newSeason = new SerieSeason();
                    newSeason.InternetLink = seasonEpisodesLink;
                    newSeason.ParentSerie = parentSerie;
                    if (countSeasons == -1)
                        newSeason.Name = "Unknown";
                    else
                        newSeason.Name = "Season " + countSeasons.ToString ("00");

                    string episodesStartsWith = "div class=\"clear\" itemscope itemtype=\"http://schema.org/TVSeason\"";
                    string allEpisodesString = GetStringFromStringToEnd (seasonEpisodesSource, episodesStartsWith);
                    string episodesSplitString = "div class=\"info\" itemprop=\"episodes\"";
                    allEpisodesString = GetStringFromStringToEnd (allEpisodesString, episodesSplitString);
                    foreach (SerieEpisode newEpisode in GetSerieEpisodes (allEpisodesString, newSeason.Name)) {
                        if (newEpisode == null)
                            continue;
                        newEpisode.ParentSeason = newSeason;
                        newSeason.Episodes.Add (newEpisode);
                        string youtubeSearchLinkEp = this.youtubeTrailerSearchPrefix + "\"" + newSeason.ParentSerie.OrigName.Replace (" ", "+") + " " +
                            newSeason.Name.Replace (" ", "+") + " " + newEpisode.OrigName + " trailer";
                        newEpisode.TrailerLink = youtubeSearchLinkEp;
                    }

                    string youtubeSearchLinkSer = this.youtubeTrailerSearchPrefix + "\"" + parentSerie.OrigName.Replace (" ", "+") + " " +
                            newSeason.Name.Replace (" ", "+") + " trailer";
                    newSeason.TrailerLink = youtubeSearchLinkSer;

                    if (seasons.Count > 0)
                        if (newSeason.Episodes.ElementAt (0).OrigName == seasons.ElementAt (seasons.Count - 1).Episodes.ElementAt (0).OrigName &&
                            newSeason.Episodes.Count == seasons.ElementAt(seasons.Count - 1).Episodes.Count)
                            break;
                    //if (newSeason.Episodes.ElementAt(0).AirDate == new DateTime (1800, 1, 1))
                    //    break;
                    seasons.Add (newSeason);
                    countSeasons++;
                    if (countSeasons == 0)
                        countSeasons++;
                }
                return seasons;
            }
            catch (Exception ex) {
                notFetched += "Sezone i epizode";
                return new ObservableCollection<SerieSeason>();
            }
        }
        ObservableCollection<SerieEpisode> GetSerieEpisodes (string source, string parentSeasonName) {
            int count = 0;
            string episodesSplitString = "div class=\"info\" itemprop=\"episodes\"";

            ObservableCollection<SerieEpisode> episodes = new ObservableCollection<SerieEpisode> ();
            foreach (string episodeString in SplitByString (source, episodesSplitString)) {
                //if (episodeString.Contains (this.serieEpisodeLinkStartsWith) == false)
                //    continue;

                count++;
                SerieEpisode newEpisode = new SerieEpisode ();
                string airDateStartsWith = "<div class=\"airdate\">\n";
                string airDateEndsWith = "\n";
                newEpisode.AirDate = DateFromString (GetStringBetweenStrings (episodeString, airDateStartsWith, airDateEndsWith).Trim());
                string nameStartsWith = "itemprop=\"name\">";
                string nameEndsWith = "</a>";
                newEpisode.OrigName = HttpUtility.HtmlDecode (GetStringBetweenStrings (episodeString, nameStartsWith, nameEndsWith));
                newEpisode.Name = string.Format ("Ep {0} - {1}", count.ToString ("00"), newEpisode.OrigName);
                string summaryStartsWith = "itemprop=\"description\">\n";
                string summaryEndsWith = "</div>";
                newEpisode.Summary = GetStringBetweenStrings (episodeString, summaryStartsWith, summaryEndsWith);
                string internetLinkStartsWith = "href=\"";
                string internetLinkEndsWith = "title=" + "\"" + newEpisode.OrigName + "\"";
                newEpisode.InternetLink = "www.imdb.com" + GetStringBetweenStrings (episodeString, internetLinkStartsWith, internetLinkEndsWith);
                newEpisode.InternetLink = newEpisode.InternetLink.Replace ("\"", "").Replace ("\n", "");
                episodes.Add (newEpisode);
            }
            return episodes;
        }

        #endregion

        #region HELP FUNCTIONS

        string TryToFetchResult (string source, string searchString) {
            try {
                string returnString = source.Substring (source.IndexOf (searchString));
                return returnString.Substring (1);
            }
            catch {
                return null;
            }
        }
        static string ReverseString (string s) {
            char[] arr = s.ToCharArray ();
            Array.Reverse (arr);
            return new string (arr);
        }
        string ReturnWebPageSource (string link) {
            
            //StringBuilder sb = new StringBuilder ();
            //byte[] buf = new byte[8192];
            try {
                byte[] source = (new WebClient()).DownloadData(link);
                string html = Encoding.UTF8.GetString(source);

                return html;

                //byte[] buf = [];
                //var sb = new StringBuilder();
                //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                //HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Stream resStream = response.GetResponseStream();
                //string tempString = null;
                //int count = 0;
                //do
                //{
                //    // fill the buffer with data
                //    count = resStream.Read(buf, 0, buf.Length);

                //    // make sure we read some data
                //    if (count != 0)
                //    {
                //        // translate from bytes to ASCII text
                //        tempString = Encoding.ASCII.GetString(buf, 0, count);

                //        // continue building the string
                //        sb.Append(tempString);
                //    }
                //}
                //while (count > 0); // any more data to read
                //return sb.ToString();
            }
            catch {
                MessageBox.Show ("Greška u internet konekciji");
                return null;
            }            
        }
        string GetStringBetweenStrings (string sourceStr, string startStr, string endStr) {
            try
            {
                int startIndex = sourceStr.IndexOf(startStr) + startStr.Length;
                if (startIndex == (startStr.Length - 1))
                {
                    return "";
                    //throw new Exception ("Ne postoji trazeni string u source-u");
                }
                string returnString = sourceStr.Substring(startIndex);
                int endIndex = returnString.IndexOf(endStr);
                returnString = returnString.Substring(0, endIndex);
                return returnString;
            }
            catch
            {                             
                return "";
            }
        }
        string GetStringFromStringToEnd (string sourceStr, string startStr) {
            int startIndex = sourceStr.IndexOf (startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                return "";
                //throw new Exception ("Ne postoji trazeni string u source-u");
            }
            return sourceStr.Substring (startIndex);
        }
        string GetStringFromStartToString (string sourceStr, string endStr) {
            int endIndex = sourceStr.IndexOf (endStr);
            if (endIndex < 0) {
                return "";
                //throw new Exception ("Trazeni string nije nadjen u source-u.");
            }
            return sourceStr.Substring (0, endIndex);
        }
        string FormatStringForUrl (string source) {
            source = source.Replace (":", "%3A");
            source = source.Replace ("&", "%26");
            return source;
        }
        List<string> SplitByString (string source, string splitString) {
            List<string> listToReturn = new List<string> ();
            int indexOf = source.IndexOf (splitString);
            while (indexOf > -1) {
                string temp = source.Substring (0, indexOf);
                source = source.Substring (indexOf + 1);
                listToReturn.Add (temp);
                indexOf = source.IndexOf (splitString);
            }
            listToReturn.Add (source);
            return listToReturn;
        }
        DateTime DateFromString (string source) {
            DateTime date = new DateTime ();
            //moguca su slijedeca 3 formata datuma
            try {
                source = source.Trim ();
                source = source.Replace (", ", " ").Replace (". ", " ").Replace ("\n", "").Replace("\r", "");
                string[] formatDate = source.Split (' ');
                if (formatDate.Count () == 2)
                    throw new Exception ();
                switch (formatDate[1]) {
                    case "Jan":
                        date = new DateTime (int.Parse (formatDate[2]), 1, int.Parse (formatDate[0]));
                        break;
                    case "Feb":
                        date = new DateTime (int.Parse (formatDate[2]), 2, int.Parse (formatDate[0]));
                        break;
                    case "Mar":
                        date = new DateTime (int.Parse (formatDate[2]), 3, int.Parse (formatDate[0]));
                        break;
                    case "Apr":
                        date = new DateTime (int.Parse (formatDate[2]), 4, int.Parse (formatDate[0]));
                        break;
                    case "May":
                        date = new DateTime (int.Parse (formatDate[2]), 5, int.Parse (formatDate[0]));
                        break;
                    case "Jun":
                        date = new DateTime (int.Parse (formatDate[2]), 6, int.Parse (formatDate[0]));
                        break;
                    case "Jul":
                        date = new DateTime (int.Parse (formatDate[2]), 7, int.Parse (formatDate[0]));
                        break;
                    case "Aug":
                        date = new DateTime (int.Parse (formatDate[2]), 8, int.Parse (formatDate[0]));
                        break;
                    case "Sep":
                        date = new DateTime (int.Parse (formatDate[2]), 9, int.Parse (formatDate[0]));
                        break;
                    case "Oct":
                        date = new DateTime (int.Parse (formatDate[2]), 10, int.Parse (formatDate[0]));
                        break;
                    case "Nov":
                        date = new DateTime (int.Parse (formatDate[2]), 11, int.Parse (formatDate[0]));
                        break;
                    case "Dec":
                        date = new DateTime (int.Parse (formatDate[2]), 12, int.Parse (formatDate[0]));
                        break;
                    default:
                        date = new DateTime (1800, 1, 1);
                        break;
                }
            }
            catch {
                date = new DateTime (1800, 1, 1);
            }         
            return date;
        }
        string CleanStringSummary (string summary) {
            summary = GetStringFromStartToString (summary, this.serieEpisodeSummaryEndsWithAlt);
            return summary.Trim ();            
        }
        string RemoveQuotes (string source) {
            //ako su navodnici na pocetku i/ili na kraju, makni ih
            if (source[0] == '"')
                source = source.Substring (1);
            int length = source.Length;
            if (source[length - 1] == '"')
                source = source.Substring (0, length - 1);
            return source;
        }
        

        #endregion
    }    
}
