using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;


namespace Video_katalog {
    static class DatabaseManagerMySql {

        public static MySqlConnection connection = new MySqlConnection (ComposeConnectionString.ComposeMySqlRemoteCS ());
        public static MySqlCommand command;
        public static MySqlDataReader reader;
        public static string stringCommand;

        #region FETCH

        #region Movie

        public static int CountMovies () {
            stringCommand = "SELECT COUNT(*) as count FROM movie";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static ObservableCollection<Movie> FetchMovieList (ObservableCollection<Genre> genreList, ObservableCollection<Audio> audioList,
            ObservableCollection<Person> personList, ObservableCollection<Language> languageList, ObservableCollection<HDD> hddList) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            ObservableCollection<Movie> listOfMovies = new ObservableCollection<Movie> ();
            string SqlCeCommandGetMovies = "SELECT * FROM `movie` ORDER BY customName";
            string SqlCeCommandGetCast = "SELECT * FROM `movie-cast` ORDER BY actorNum";
            string SqlCeCommandGetDirectors = "SELECT * FROM `movie-director` ORDER BY directorNum";
            string SqlCeCommandGetAudio = "SELECT * FROM `movie-audio`";
            string SqlCeCommandGetSubtitleLanguages = "SELECT * FROM `movie-subtitlelanguage`";
            string SqlCeCommandGetGenres = "SELECT * FROM `movie-genre` ORDER BY genreNum";

            //**** MOVIES ****
            command = new MySqlCommand (SqlCeCommandGetMovies, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                Movie tempMovie = new Movie ();
                tempMovie.ID = (int) reader["movieID"];
                tempMovie.Name = (string) reader["customName"];
                tempMovie.OrigName = (string) reader["originalName"];
                //tempMovie.Poster = (Image) reader["image"];
                tempMovie.AddTime = (DateTime) reader["addDate"];
                tempMovie.Year = (int) reader["year"];
                tempMovie.Rating = (decimal) reader["rating"];
                tempMovie.InternetLink = (string) reader["internetLink"];
                tempMovie.TrailerLink = (string) reader["trailerLink"];
                tempMovie.Summary = (string) reader["plot"];
                tempMovie.Size = (long) reader["size"];
                tempMovie.Width = (int) reader["width"];
                tempMovie.Height = (int) reader["height"];
                tempMovie.Runtime = (int) reader["runtime"];
                tempMovie.Bitrate = (int) reader["bitrate"];
                tempMovie.AspectRatio = (string) reader["aspectRatio"];
                tempMovie.Budget = float.Parse (reader["budget"].ToString ());
                tempMovie.Earnings = float.Parse (reader["earnings"].ToString ());
                tempMovie.IsViewed = Convert.ToBoolean((byte) reader["isViewed"]);
                int hddID = (int) reader["hddnum"];
                tempMovie.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddID);
                tempMovie.Version = (byte) reader["version"];
                listOfMovies.Add (tempMovie);
            }
            reader.Close ();

            //**** GENRES ****
            command = new MySqlCommand (SqlCeCommandGetGenres, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
            }
            reader.Close ();

            //**** DIRECTORS ****
            command = new MySqlCommand (SqlCeCommandGetDirectors, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int directorID = (int) reader["directorID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
            }
            reader.Close ();

            //**** CAST ****
            command = new MySqlCommand (SqlCeCommandGetCast, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int actorID = (int) reader["personID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
            }
            reader.Close ();

            //**** SUBTITLES ****
            command = new MySqlCommand (SqlCeCommandGetSubtitleLanguages, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int languageID = (int) reader["subtitleLanguageID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).SubtitleLanguageList.Add (FinderInCollection.FindInLanguageCollection (languageList, languageID));
            }
            reader.Close ();

            //**** AUDIO ****
            command = new MySqlCommand (SqlCeCommandGetAudio, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int audioID = (int) reader["audioID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).AudioList.Add (FinderInCollection.FindInAudioCollection (audioList, audioID));
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();

            return listOfMovies;
        }

        #endregion

        #region Serie

        public static int CountSeries () {
            stringCommand = "SELECT COUNT(*) as count FROM serie";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static int CountSeasons () {
            stringCommand = "SELECT COUNT(*) as count FROM serieseason";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static int CountEpisodes () {
            stringCommand = "SELECT COUNT(*) as count FROM serieepisode";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static ObservableCollection<Serie> FetchSerieList (ObservableCollection<Genre> genreList, ObservableCollection<Person> personList) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Serie> listOfSeries = new ObservableCollection<Serie> ();
            listOfSeries = new ObservableCollection<Serie> ();

            string getSeriesCommand = "SELECT * FROM serie ORDER BY origName";
            string getSerieGenresCommand = "SELECT * FROM `serie-genre` ORDER BY genreNum";
            string getSerieCastCommand = "SELECT * FROM `serie-cast` ORDER BY actorNum";
            string getSerieDirectorsCommand = "SELECT * FROM `serie-director` ORDER BY directorNum";

            #region Main

            command = new MySqlCommand (getSeriesCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                Serie newSerie = new Serie ();
                newSerie.ID = (int) reader["serieID"];
                newSerie.Name = (string) reader["name"];
                newSerie.Rating = (decimal) reader["rating"];
                newSerie.OrigName = (string) reader["origName"];
                newSerie.InternetLink = (string) reader["internetLink"];
                newSerie.TrailerLink = (string) reader["trailerLink"];
                newSerie.Summary = (string) reader["summary"];
                listOfSeries.Add (newSerie);
            }
            reader.Close ();

            #endregion

            #region Genres

            command = new MySqlCommand (getSerieGenresCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
            }
            reader.Close ();

            #endregion

            #region Cast

            command = new MySqlCommand (getSerieCastCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int actorID = (int) reader["actorID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
            }
            reader.Close ();

            #endregion

            #region Directors

            command = new MySqlCommand (getSerieDirectorsCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int directorID = (int) reader["directorID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
            }

            #endregion

            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return listOfSeries;
        }
        public static ObservableCollection<SerieSeason> FetchSerieSeasonList (ObservableCollection<Serie> serieList) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<SerieSeason> listOfSerSeasons = new ObservableCollection<SerieSeason> ();
            string getSeasonsCommand = "SELECT * FROM serieseason ORDER BY name";
            command = new MySqlCommand (getSeasonsCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                SerieSeason newSeason = new SerieSeason ();
                newSeason.ID = (int) reader["seasonID"];
                newSeason.Name = (string) reader["name"];
                newSeason.InternetLink = (string) reader["internetLink"];
                newSeason.TrailerLink = (string) reader["trailerLink"];
                int parentSerieID = (int) reader["serieID"];
                try {
                    newSeason.ParentSerie = FinderInCollection.FindInSerieCollection (serieList, parentSerieID);
                    FinderInCollection.FindInSerieCollection (serieList, parentSerieID).Seasons.Add (newSeason);
                    listOfSerSeasons.Add (newSeason);
                }
                catch {
                    newSeason.ParentSerie = new Serie ();
                    listOfSerSeasons.Add (newSeason);
                }
            }
            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return listOfSerSeasons;
        }
        public static ObservableCollection<SerieEpisode> FetchSerieEpisodeList (ObservableCollection<SerieSeason> seasonList, ObservableCollection<Language> languageList,
            ObservableCollection<Audio> audioList, ObservableCollection<HDD> hddList, int countOnlineEpisodes) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode> ();
            string getEpisodesCommand = "SELECT * FROM `serieepisode` ORDER BY name LIMIT ?downLimit, ?step";
            string getEpisodeAudio = "SELECT * FROM `serieepisode-audio`";
            string getEpisodeSubtitle = "SELECT * FROM `serieepisode-subtitlelanguage`";

            #region Main

            int step = 1000;
            int downLimit = 0;
            int upLimit = step;
            int count = 0;
            while (downLimit < countOnlineEpisodes) {
                command = new MySqlCommand (getEpisodesCommand, connection);
                command.Parameters.AddWithValue ("?downLimit", downLimit);
                command.Parameters.AddWithValue ("?step", step);
                reader = command.ExecuteReader ();
                //reader.Read ();
                while (reader.Read ()) {
                    SerieEpisode newEpisode = new SerieEpisode ();
                    newEpisode.ID = (int) reader["episodeID"];
                    newEpisode.Name = (string) reader["name"];
                    newEpisode.OrigName = (string) reader["origName"];
                    newEpisode.AddTime = (DateTime) reader["addDate"];
                    newEpisode.AirDate = (DateTime) reader["airDate"];
                    newEpisode.Size = (long) reader["size"];
                    newEpisode.Runtime = (int) reader["runtime"];
                    newEpisode.Bitrate = (int) reader["bitrate"];
                    newEpisode.Width = (int) reader["width"];
                    newEpisode.Height = (int) reader["height"];
                    newEpisode.AspectRatio = (string) reader["aspectRatio"];
                    int hddID = (int) reader["hddID"];
                    newEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddID);
                    newEpisode.Summary = (string) reader["summary"];
                    newEpisode.IsViewed = Convert.ToBoolean ((byte) reader["viewed"]);
                    int seasonID = (int) reader["seasonID"];
                    newEpisode.TrailerLink = (string) reader["trailerLink"];
                    newEpisode.InternetLink = (string) reader["internetLink"];
                    newEpisode.Version = (byte) reader["version"];
                    try {
                        newEpisode.ParentSeason = FinderInCollection.FindInSerieSeasonCollection (seasonList, seasonID);
                        FinderInCollection.FindInSerieSeasonCollection (seasonList, seasonID).Episodes.Add (newEpisode);
                        listOfSerEpisodes.Add (newEpisode);
                    }
                    catch {
                        newEpisode.ParentSeason = new SerieSeason ();
                        listOfSerEpisodes.Add (newEpisode);
                    }

                    try {
                        //reader.Read ();
                    }
                    catch {                       
                        break;
                    }
                    if (count % 999 == 0) ;
                        //MessageBox.Show ("Hello");
                    if (count == countOnlineEpisodes - 1) {
                        int i;
                    }
                    count++;
                }
                reader.Close ();
                downLimit += step;
            }

            
            if (count < countOnlineEpisodes) {
                try {
                    //connection = new MySqlConnection (ComposeConnectionString.ComposeMySqlRemoteCS ());
                }
                catch {
                }
                throw new Exception ("Dohvat neuspješan. Probati ponovno.");
            }
            reader.Close ();
            

            //MessageBox.Show (count.ToString ());

            #endregion

            #region Audios

            command = new MySqlCommand (getEpisodeAudio, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int episodeID = (int) reader["episodeID"];
                int audioID = (int) reader["audioID"];
                try {
                    FinderInCollection.FindInSerieEpisodeCollection (listOfSerEpisodes, episodeID).AudioList.Add (FinderInCollection.FindInAudioCollection (audioList, audioID));
                }
                catch {
                }
            }
            reader.Close ();

            #endregion

            #region Subtitles

            command = new MySqlCommand (getEpisodeSubtitle, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int episodeID = (int) reader["episodeID"];
                int subtitleID = (int) reader["subtitleID"];
                try {
                    FinderInCollection.FindInSerieEpisodeCollection (listOfSerEpisodes, episodeID).SubtitleLanguageList.Add (FinderInCollection.FindInLanguageCollection (languageList, subtitleID));
                }
                catch {
                }
            }
            reader.Close ();

            #endregion

            if (wasClosed)
                connection.Close ();

            return listOfSerEpisodes;
        }

        #endregion

        #region Misc

        public static int CountPersons () {
            stringCommand = "SELECT COUNT(*) as count FROM person";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static int CountHDDs () {
            stringCommand = "SELECT COUNT(*) as count FROM hdd";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static int CountLanguages () {
            stringCommand = "SELECT COUNT(*) as count FROM language";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static int CountAudios () {
            stringCommand = "SELECT COUNT(*) as count FROM audio";
            command = new MySqlCommand (stringCommand, connection);
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            reader = command.ExecuteReader ();
            reader.Read ();
            int toReturn = int.Parse (reader["count"].ToString ());
            if (wasClosed)
                connection.Close ();
            return toReturn;
        }
        public static ObservableCollection<Person> FetchPersonList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            ObservableCollection<Person> personList = new ObservableCollection<Person> ();
            stringCommand = "SELECT * FROM person";// WHERE typeID = 2"; 
            command = new MySqlCommand (stringCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int id = (int) reader["personID"];
                string name = (string) reader["personName"];
                int typeID = (int) reader["typeID"];
                Person tempPerson = new Person (new PersonType (typeID));
                tempPerson.Name = name;
                tempPerson.ID = id;
                //tempPerson.DateOfBirth = (DateTime) reader["dateOfBirth"];
                personList.Add (tempPerson);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return personList;
        }
        public static ObservableCollection<HDD> FetchHDDList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();
            string SqlCeCommandGetHdds = "SELECT * FROM hdd";
            command = new MySqlCommand (SqlCeCommandGetHdds, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                HDD tempHdd = new HDD ();
                tempHdd.ID = (int) reader["hddID"];
                tempHdd.Name = (string) reader["name"];
                //tempHdd.PurchaseDate = (DateTime) reader["purchaseDate"];
                tempHdd.WarrantyLength = (int) reader["warrantyLength"];
                tempHdd.Capacity = (long) reader["capacity"];
                tempHdd.FreeSpace = (long) reader["freeSpace"];
                hddList.Add (tempHdd);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return hddList;
        }
        public static ObservableCollection<Language> FetchLanguageList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Language> languageList = new ObservableCollection<Language> ();
            string SqlCeCommandGetLanguages = "SELECT * FROM language";
            command = new MySqlCommand (SqlCeCommandGetLanguages, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                Language tempLanguage = new Language ();
                tempLanguage.ID = (int) reader["languageID"];
                tempLanguage.Name = (string) reader["name"];
                languageList.Add (tempLanguage);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return languageList;
        }
        public static ObservableCollection<Audio> FetchAudioList (ObservableCollection<Language> languageList) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Audio> audioList = new ObservableCollection<Audio> ();
            string SqlCeCommandAudio = "SELECT * FROM audio";
            command = new MySqlCommand (SqlCeCommandAudio, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                Audio newAudio = new Audio ();
                newAudio.ID = (int) reader["ID"];
                newAudio.Channels = (decimal) reader["channels"];
                newAudio.Format = (string) reader["format"];
                int langID = (int) reader["languageID"];
                newAudio.Language = FinderInCollection.FindInLanguageCollection (languageList, langID);
                audioList.Add (newAudio);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return audioList;
        }
        public static decimal NewestVersionNumber () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            stringCommand = "SELECT * FROM `version`";
            command = new MySqlCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            reader.Read ();
            decimal version = (decimal) reader["versionNumber"];
            reader.Close ();

            if (wasClosed)
                connection.Close ();
            return version;
        }
        public static string NewestVersionChanges () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            stringCommand = "SELECT * FROM `version`";
            command = new MySqlCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            reader.Read ();
            string changes = (string) reader["changes"];
            reader.Close ();

            if (wasClosed)
                connection.Close ();
            return changes;
        }

        #endregion

        #endregion

        #region INSERT / UPDATE / DELETE

        static void ExecuteCommand () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            command.ExecuteNonQuery ();
            command.Parameters.Clear ();
            if (wasClosed)
                connection.Close ();
        }        

        #region Movie

        public static void InsertNewMovie (Movie movie) {
            stringCommand = "INSERT INTO `movie` (movieID, customName, originalName, addDate, year, rating, internetLink, " +
                                        "trailerLink, plot, size, height, width, runtime, bitrate, aspectRatio, budget, " +
                                        "earnings, isViewed, hddNum, version) VALUES " +
                                        "(?movieID, ?customName, ?originalName, ?addDate, ?year, ?rating, " +
                                        "?internetLink, ?trailerLink, ?plot, ?size, ?height, ?width, " +
                                        "?runtime, ?bitrate, ?aspectRatio, ?budget, ?earnings, ?isViewed, ?hddNum, ?version)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            command.Parameters.AddWithValue ("?customName", movie.Name);
            command.Parameters.AddWithValue ("?originalName", movie.OrigName);
            command.Parameters.AddWithValue ("?addDate", movie.AddTime);
            command.Parameters.AddWithValue ("?year", movie.Year);
            command.Parameters.AddWithValue ("?rating", movie.Rating);
            command.Parameters.AddWithValue ("?internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("?plot", movie.Summary);
            command.Parameters.AddWithValue ("?size", movie.Size);
            command.Parameters.AddWithValue ("?height", movie.Height);
            command.Parameters.AddWithValue ("?width", movie.Width);
            command.Parameters.AddWithValue ("?runtime", movie.Runtime);
            command.Parameters.AddWithValue ("?bitrate", movie.Bitrate);
            command.Parameters.AddWithValue ("?aspectRatio", movie.AspectRatio);
            command.Parameters.AddWithValue ("?budget", movie.Budget);
            command.Parameters.AddWithValue ("?earnings", movie.Earnings);
            command.Parameters.AddWithValue ("?isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("?hddNum", movie.Hdd.ID);
            command.Parameters.AddWithValue ("?version", movie.Version);

            ExecuteCommand ();

            InsertMovieAudios (movie);
            InsertMovieSubtitles (movie);
            InsertMovieCast (movie);
            InsertMovieDirectors (movie);
            InsertMovieGenres (movie);
        }
        public static void UpdateMovie (Movie movie) {
            stringCommand = "UPDATE `movie` SET customName = ?customName ,originalName = ?originalName ,addDate = ?addDate ," +
                                        "year = ?year ,rating = ?rating ,internetLink = ?internetLink ,trailerLink = ?trailerLink ," +
                                        "plot = ?plot ,size = ?size ,height = ?height ,width = ?width ,runtime = ?runtime ," +
                                        "bitrate = ?bitrate ,aspectRatio = ?aspectRatio ,budget = ?budget ,earnings = ?earnings ," +
                                        "isViewed = ?isViewed ,hddNum = ?hddNum, version = ?version WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?customName", movie.Name);
            command.Parameters.AddWithValue ("?originalName", movie.OrigName);
            command.Parameters.AddWithValue ("?addDate", movie.AddTime);
            command.Parameters.AddWithValue ("?year", movie.Year);
            command.Parameters.AddWithValue ("?rating", movie.Rating);
            command.Parameters.AddWithValue ("?internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("?plot", movie.Summary);
            command.Parameters.AddWithValue ("?size", movie.Size);
            command.Parameters.AddWithValue ("?height", movie.Height);
            command.Parameters.AddWithValue ("?width", movie.Width);
            command.Parameters.AddWithValue ("?runtime", movie.Runtime);
            command.Parameters.AddWithValue ("?bitrate", movie.Bitrate);
            command.Parameters.AddWithValue ("?aspectRatio", movie.AspectRatio);
            command.Parameters.AddWithValue ("?budget", movie.Budget);
            command.Parameters.AddWithValue ("?earnings", movie.Earnings);
            command.Parameters.AddWithValue ("?isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("?hddNum", movie.Hdd.ID);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            command.Parameters.AddWithValue ("?version", movie.Version);

            ExecuteCommand ();

            DeleteMovieAudios (movie);
            InsertMovieAudios (movie);
            DeleteMovieSubtitles (movie);
            InsertMovieSubtitles (movie);
            DeleteMovieCast (movie);
            InsertMovieCast (movie);
            DeleteMovieDirectors (movie);
            InsertMovieDirectors (movie);
            DeleteMovieGenres (movie);
            InsertMovieGenres (movie);

        }
        public static void DeleteMovie (Movie movie) {
            stringCommand = "DELETE FROM `movie` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            ExecuteCommand ();
            DeleteMovieAudios (movie);
            DeleteMovieCast (movie);
            DeleteMovieDirectors (movie);
            DeleteMovieGenres (movie);
            DeleteMovieSubtitles (movie);
        }

        static void InsertMovieAudios (Movie movie) {
            foreach (Audio tempAudio in movie.AudioList) {
                stringCommand = "INSERT INTO `movie-audio` (movieID, audioID) VALUES (?movieID, ?audioID)";
                command = new MySqlCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?audioID", tempAudio.ID);
                ExecuteCommand ();
            }
        }
        static void DeleteMovieAudios (Movie movie) {
            stringCommand = "DELETE FROM `movie-audio` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);

            ExecuteCommand ();
        }
        static void InsertMovieSubtitles (Movie movie) {
            stringCommand = "INSERT INTO `movie-subtitlelanguage` (movieID ,subtitleLanguageID) VALUES (?movieID, ?subtitleLanguageID)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Language tempLang in movie.SubtitleLanguageList) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?subtitleLanguageID", tempLang.ID);
                ExecuteCommand ();
            }
        }
        static void DeleteMovieSubtitles (Movie movie) {
            stringCommand = "DELETE FROM `movie-subtitlelanguage` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            ExecuteCommand ();
        }
        static void InsertMovieGenres (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO `movie-genre` (movieID ,genreID, genreNum) VALUES (?movieID, ?genreID, ?genreNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Genre tempGenre in movie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("?genreNum", count++);
                ExecuteCommand ();
            }
        }
        static void DeleteMovieGenres (Movie movie) {
            stringCommand = "DELETE FROM `movie-genre` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            ExecuteCommand ();
        }
        static void InsertMovieCast (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO `movie-cast` (personID ,movieID, actorNum) VALUES (?personID, ?movieID, ?actorNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Person actor in movie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?personID", actor.ID);
                command.Parameters.AddWithValue ("?actorNum", count++);
                ExecuteCommand ();
            }
        }
        static void DeleteMovieCast (Movie movie) {
            stringCommand = "DELETE FROM `movie-cast` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            ExecuteCommand ();
        }
        static void InsertMovieDirectors (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO `movie-director` (movieID ,directorID, directorNum) VALUES (?movieID, ?directorID, ?directorNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Person director in movie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?directorID", director.ID);
                command.Parameters.AddWithValue ("?directorNum", count++);
                ExecuteCommand ();
            }
        }
        static void DeleteMovieDirectors (Movie movie) {
            stringCommand = "DELETE FROM `movie-director` WHERE movieID = ?movieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?movieID", movie.ID);
            ExecuteCommand ();
        }

        #endregion

        #region Serie

        public static void InsertSerie (Serie serie) {
            stringCommand = "INSERT INTO `serie` (serieID, name, origName, rating, internetLink, summary, trailerLink) VALUES (?serieID, ?name, ?origName, ?rating, ?internetLink, ?summary, ?trailerLink)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            command.Parameters.AddWithValue ("?name", serie.Name);
            command.Parameters.AddWithValue ("?origName", serie.OrigName);
            command.Parameters.AddWithValue ("?rating", serie.Rating);
            command.Parameters.AddWithValue ("?internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("?summary", serie.Summary);
            command.Parameters.AddWithValue ("?trailerLink", serie.TrailerLink);

            ExecuteCommand ();

            InsertSerieCast (serie);
            InsertSerieDirectors (serie);
            InsertSerieGenres (serie);
        }
        public static void UpdateOnlySerie (Serie serie) {
            stringCommand = "UPDATE `serie` SET name = ?name, origName = ?origName, rating = ?rating, internetLink = ?internetLink, summary = ?summary, trailerLink = ?trailerLink WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?name", serie.Name);
            command.Parameters.AddWithValue ("?origName", serie.OrigName);
            command.Parameters.AddWithValue ("?rating", serie.Rating);
            command.Parameters.AddWithValue ("?internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("?summary", serie.Summary);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            command.Parameters.AddWithValue ("?trailerLink", serie.TrailerLink);

            ExecuteCommand ();

            DeleteSerieCast (serie);
            InsertSerieCast (serie);
            DeleteSerieDirectors (serie);
            InsertSerieDirectors (serie);
            DeleteSerieGenres (serie);
            InsertSerieGenres (serie);

            //osvjezi i listu sezona

            //List<int> seasonsIdList = new List<int> (); //lista svih ID-jeva sezona koje postoje u seriji
            /*foreach (SerieSeason tempSeason in serie.Seasons) {
                if (tempSeason.ID == null) //ako je ID null znaci da je sezona nova i da ju treba unijet
                    InsertSerieSeason (tempSeason);
                else
                    UpdateSerieSeason (tempSeason);
                seasonsIdList.Add (tempSeason.ID);
            }

            //dohvati sve sezone koje pripadaju seriji
            bool wasClosed = true;
            stringCommand = "SELECT seasonID FROM SerieSeason WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedSeasonIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji po bazi podataka
            MySqlDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedSeasonIDList.Add ((int) reader["seasonID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene sezone iz baze, ako dohvacena serija nije u listi sezona obriši ju iz baze
            foreach (int seasonID in fetchedSeasonIDList) {
                if (seasonsIdList.Contains (seasonID) == false) {
                    stringCommand = "DELETE FROM SerieSeason WHERE seasonID = ?seasonID";
                    command = new MySqlCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("?seasonID", seasonID);
                    command.ExecuteNonQuery ();
                }
            }*/

            /*if (wasClosed)
                connection.Close ();*/

        }
        public static void UpdateSerie (Serie serie) {
            try {
                DeleteSerie (serie);
            }
            catch {
            }
            InsertSerie (serie);
            foreach (SerieSeason tempSeason in serie.Seasons) {
                InsertSerieSeason (tempSeason);
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    InsertSerieEpisode (tempEpisode);
                }
            }
        }
        public static void DeleteSerie (Serie serie) {
            stringCommand = "DELETE FROM `serie` WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            ExecuteCommand ();
            foreach (SerieSeason tempSeason in serie.Seasons)
                DeleteSerieSeason (tempSeason);
            DeleteSerieCast (serie);
            DeleteSerieDirectors (serie);
            DeleteSerieGenres (serie);
        }

        public static void InsertSerieSeason (SerieSeason season) {
            stringCommand = "INSERT INTO `serieseason` (seasonID, serieID ,name ,internetLink ,trailerLink) VALUES (?seasonID, ?serieID ,?name ,?internetLink ,?trailerLink)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?seasonID", season.ID);
            command.Parameters.AddWithValue ("?serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("?name", season.Name);
            command.Parameters.AddWithValue ("?internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", season.TrailerLink);

            ExecuteCommand ();
        }
        public static void UpdateSerieSeason (SerieSeason season) {
            stringCommand = "UPDATE `serieseason` SET serieID = ?serieID ,name = ?name ,internetLink = ?internetLink ,trailerLink = ?trailerLink WHERE seasonID = ?seasonID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("?name", season.Name);
            command.Parameters.AddWithValue ("?internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", season.TrailerLink);
            command.Parameters.AddWithValue ("?seasonID", season.ID);
            ExecuteCommand ();


            //osvjezi i listu epizoda

            List<int> episodesIDList = new List<int> ();
            foreach (SerieEpisode tempEpisode in season.Episodes) {
                if (tempEpisode.ID == 0)  //ako je ID null znaci da epizoda nije dodana u bazu
                    InsertSerieEpisode (tempEpisode);
                else
                    UpdateSerieEpisode (tempEpisode);
                episodesIDList.Add (tempEpisode.ID);
            }

            //dohvati sve epizode koje pripadaju seriji
            bool wasClosed = true;
            stringCommand = "SELECT episodeID FROM serieepisode WHERE seasonID = ?seasonID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?seasonID", season.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedEpisodeIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji u bazi podataka
            MySqlDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedEpisodeIDList.Add ((int) reader["episodeID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene epizode, ako dohvacena epizoda nije u listi epizoda sezone, obriši ju iz baze
            foreach (int episodeID in fetchedEpisodeIDList) {
                if (episodesIDList.Contains (episodeID) == false) {
                    stringCommand = "DELETE FROM serieepisode WHERE episodeID = ?episodeID";
                    command = new MySqlCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("?episodeID", episodeID);
                    command.ExecuteNonQuery ();
                }
            }

            if (wasClosed)
                connection.Close ();
        }
        public static void DeleteSerieSeason (SerieSeason season) {
            stringCommand = "DELETE FROM `serieseason` WHERE seasonID = ?seasonID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?seasonID", season.ID);
            ExecuteCommand ();
            foreach (SerieEpisode tempepisode in season.Episodes)
                DeleteSerieEpisode (tempepisode);
        }

        public static void InsertSerieEpisode (SerieEpisode episode) {
            stringCommand = "INSERT INTO `serieepisode` (episodeID, name ,origName ,seasonID ,addDate ,airDate ," +
                "size ,runtime ,bitrate ,width ,height ,aspectRatio ,hddID ,summary ,viewed, internetLink, trailerLink, version) VALUES " +
                "(?episodeID, ?name, ?origName, ?seasonID, ?addDate, ?airDate, ?size, ?runtime, ?bitrate, " +
                "?width, ?height, ?aspectRatio, ?hddID, ?summary, ?viewed, ?internetLink, ?trailerLink, ?version)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?episodeID", episode.ID);
            command.Parameters.AddWithValue ("?name", episode.Name);
            command.Parameters.AddWithValue ("?origName", episode.OrigName);
            command.Parameters.AddWithValue ("?seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue ("?addDate", episode.AddTime);
            command.Parameters.AddWithValue ("?airDate", episode.AirDate);
            command.Parameters.AddWithValue ("?size", episode.Size);
            command.Parameters.AddWithValue ("?runtime", episode.Runtime);
            command.Parameters.AddWithValue ("?bitrate", episode.Bitrate);
            command.Parameters.AddWithValue ("?width", episode.Width);
            command.Parameters.AddWithValue ("?height", episode.Height);
            command.Parameters.AddWithValue ("?aspectRatio", episode.AspectRatio);
            command.Parameters.AddWithValue ("?hddID", episode.Hdd.ID);
            command.Parameters.AddWithValue ("?summary", episode.Summary);
            command.Parameters.AddWithValue ("?viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("?internetLink", episode.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", episode.TrailerLink);
            command.Parameters.AddWithValue ("?version", episode.Version);

            ExecuteCommand ();

            InsertSerieEpisodeAudios (episode);
            InsertSerieEpisodeSubtitles (episode);
        }
        public static void UpdateSerieEpisode (SerieEpisode episode) {
            stringCommand = "UPDATE `serieepisode` SET name = ?name, origName = ?origName, seasonID = ?seasonID, addDate = ?addDate, airDate = ?airDate, size = ?size, " +
                "runtime = ?runtime, bitrate = ?bitrate, width = ?width, height = ?height, aspectRatio = ?aspectRatio, " +
                "hddID = ?hddID, summary = ?summary, viewed = ?viewed, version = ?version WHERE episodeID = ?episodeID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?name", episode.Name);
            command.Parameters.AddWithValue ("?origName", episode.OrigName);
            command.Parameters.AddWithValue ("?seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue ("?addDate", episode.AddTime);
            command.Parameters.AddWithValue ("?airDate", episode.AirDate);
            command.Parameters.AddWithValue ("?size", episode.Size);
            command.Parameters.AddWithValue ("?runtime", episode.Runtime);
            command.Parameters.AddWithValue ("?bitrate", episode.Bitrate);
            command.Parameters.AddWithValue ("?width", episode.Width);
            command.Parameters.AddWithValue ("?height", episode.Height);
            command.Parameters.AddWithValue ("?aspectRatio", episode.AspectRatio);
            command.Parameters.AddWithValue ("?hddID", episode.Hdd.ID);
            command.Parameters.AddWithValue ("?summary", episode.Summary);
            command.Parameters.AddWithValue ("?viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("?episodeID", episode.ID);
            command.Parameters.AddWithValue ("?version", episode.Version);

            ExecuteCommand ();
            DeleteSerieEpisodeAudios (episode);
            DeleteSerieEpisodeSubtitles (episode);
            InsertSerieEpisodeAudios (episode);
            InsertSerieEpisodeSubtitles (episode);
        }
        public static void DeleteSerieEpisode (SerieEpisode episode) {
            stringCommand = "DELETE FROM `serieepisode` WHERE episodeID = ?episodeID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?episodeID", episode.ID);
            ExecuteCommand ();
            DeleteSerieEpisodeAudios (episode);
            DeleteSerieEpisodeSubtitles (episode);
        }

        static void InsertSerieEpisodeAudios (SerieEpisode episode) {
            foreach (Audio tempAudio in episode.AudioList) {
                stringCommand = "INSERT INTO `serieepisode-audio` (episodeID, audioID) VALUES (?episodeID, ?audioID)";
                command = new MySqlCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("?episodeID", episode.ID);
                command.Parameters.AddWithValue ("?audioID", tempAudio.ID);
                ExecuteCommand ();
            }
        }
        static void DeleteSerieEpisodeAudios (SerieEpisode episode) {
            stringCommand = "DELETE FROM `serieepisode-audio` WHERE episodeID = ?episodeID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?episodeID", episode.ID);

            ExecuteCommand ();
        }
        static void InsertSerieEpisodeSubtitles (SerieEpisode episode) {
            foreach (Language tempLang in episode.SubtitleLanguageList) {
                stringCommand = "INSERT INTO `serieepisode-subtitlelanguage` (episodeID, subtitleID) VALUES (?episodeID, ?subtitleID)";
                command = new MySqlCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("?episodeID", episode.ID);
                command.Parameters.AddWithValue ("?subtitleID", tempLang.ID);
                ExecuteCommand ();
            }
        }
        static void DeleteSerieEpisodeSubtitles (SerieEpisode episode) {
            stringCommand = "DELETE FROM `serieepisode-subtitlelanguage` WHERE episodeID = ?episodeID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?episodeID", episode.ID);

            ExecuteCommand ();
        }

        static void InsertSerieCast (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO `serie-cast` (actorID ,serieID, actorNum) VALUES (?actorID, ?serieID, ?actorNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Person actor in serie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?actorID", actor.ID);
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?actorNum", count++);
                ExecuteCommand ();
            }
        }
        static void DeleteSerieCast (Serie serie) {
            stringCommand = "DELETE FROM `serie-cast` WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            ExecuteCommand ();
        }
        static void InsertSerieDirectors (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO `serie-director` (serieID ,directorID, directorNum) VALUES (?serieID, ?directorID, ?directorNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Person director in serie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?directorID", director.ID);
                command.Parameters.AddWithValue ("?directorNum", count++);
                ExecuteCommand ();
            }
        }
        static void DeleteSerieDirectors (Serie serie) {
            stringCommand = "DELETE FROM `serie-director` WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            ExecuteCommand ();
        }
        static void InsertSerieGenres (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO `serie-genre` (serieID ,genreID, genreNum) VALUES (?serieID, ?genreID, ?genreNum)";
            command = new MySqlCommand (stringCommand, connection);
            foreach (Genre tempGenre in serie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("?genreNum", count++);
                ExecuteCommand ();
            }

        }
        static void DeleteSerieGenres (Serie serie) {
            stringCommand = "DELETE FROM `serie-genre` WHERE serieID = ?serieID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            ExecuteCommand ();
        }

        #endregion

        #region Misc

        #region Person

        public static void InsertPerson (Person person) {
            stringCommand = "INSERT INTO `person` (personID, typeID, personName, dateOfBirth) VALUES (?personID, ?typeID, ?name, ?dateOfBirth)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?personID", person.ID);
            command.Parameters.AddWithValue ("?name", person.Name);
            command.Parameters.AddWithValue ("?typeID", person.Type.ID);
            command.Parameters.AddWithValue ("?dateOfBirth", person.DateOfBirth);

            ExecuteCommand ();
        }
        public static void DeletePerson (Person person) {
            stringCommand = "DELETE FROM `person` WHERE personID = ?personID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?personID", person.ID);
            ExecuteCommand ();
        }
        public static void UpdatePerson (Person person) {
            stringCommand = "UPDATE `person` SET typeID = ?typeID ,personName = ?personName ,dateOfBirth = ?dateOfBirth WHERE personID = ?personID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?name", person.Name);
            command.Parameters.AddWithValue ("?typeID", person.Type.ID);
            command.Parameters.AddWithValue ("?dateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue ("?personID", person.ID);
        }

        #endregion

        #region Language

        public static void InsertLanguage (Language language) {
            stringCommand = "INSERT INTO language (languageID, name) VALUES (?languageID, ?name)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?languageID", language.ID);
            command.Parameters.AddWithValue ("?name", language.Name);

            ExecuteCommand ();
        }
        public static void DeleteLanguage (Language language) {
            stringCommand = "DELETE FROM language WHERE languageID = ?languageID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?languageID", language.ID);
            ExecuteCommand ();
        }
        public static void UpdateLanguage (Language language) {
            throw new NotImplementedException ();
        }

        #endregion

        #region Hdd

        public static void InsertHDD (HDD hdd) {
            stringCommand = "INSERT INTO `hdd` (hddID, name ,purchaseDate ,warrantyLength ,capacity ,freeSpace) VALUES (?hddID, ?name, ?purchaseDate, ?warrantyLength, ?capacity, ?freeSpace)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?hddID", hdd.ID);
            command.Parameters.AddWithValue ("?name", hdd.Name);
            command.Parameters.AddWithValue ("?purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue ("?warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("?capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("?freeSpace", hdd.FreeSpace);

            ExecuteCommand ();
        }
        public static void DeleteHDD (HDD hdd) {
            stringCommand = "DELETE FROM hdd WHERE hddID = ?hddID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?hddID", hdd.ID);
            ExecuteCommand ();
        }
        public static void UpdateHDD (HDD hdd) {
            stringCommand = "UPDATE hdd SET name = ?name ,purchaseDate = ?purchaseDate ,warrantyLength = ?warrantyLength ,capacity = ?capacity ,freeSpace = ?freeSpace WHERE hddID = ?hddID";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?name", hdd.Name);
            command.Parameters.AddWithValue ("?purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue ("?warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("?capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("?freeSpace", hdd.FreeSpace);
            command.Parameters.AddWithValue ("?hddID", hdd.ID);
            ExecuteCommand ();
        }

        #endregion

        #region Audio

        public static void InsertAudio (Audio audio) {
            stringCommand = "INSERT INTO audio (ID, channels, languageID, format) VALUES (?ID, ?channels, ?languageID, ?format)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?ID", audio.ID);
            command.Parameters.AddWithValue ("?channels", audio.Channels);
            command.Parameters.AddWithValue ("?languageID", audio.Language.ID);
            command.Parameters.AddWithValue ("?format", audio.Format);

            ExecuteCommand ();
        }
        public static void DeleteAudio (Audio audio) {
            throw new NotImplementedException ();
        }
        public static void UpdateAudio (Audio audio) {
            throw new NotImplementedException ();
        }

        #endregion

        #region Version

        public static void InsertnewVersion (decimal version, string changes) {
            DeleteAllVersions ();
            stringCommand = "INSERT INTO `version` (versionNumber, changes) VALUES (?version, ?changes)";
            command = new MySqlCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("?version", version);
            command.Parameters.AddWithValue ("?changes", changes);
            ExecuteCommand ();
        }
        public static void DeleteAllVersions () {
            stringCommand = "DELETE FROM `version`";
            command = new MySqlCommand (stringCommand, connection);
            ExecuteCommand ();
        }

        #endregion

        #endregion

        #endregion
    }
}
