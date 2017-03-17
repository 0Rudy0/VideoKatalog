using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Windows.Controls;

namespace Video_katalog {
    public class DatabaseManagerSdf {
        static SqlCeConnection connection = new SqlCeConnection(ComposeConnectionString.ComposeCEString());
        static SqlCeCommand command;
        static SqlCeDataReader reader;
        static string stringCommand;
        
        #region RETREIVE

        #region Movie

        //REGULAR
        public static int FetchMovieCountList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            int count = 0;

            stringCommand = "SELECT COUNT(*) as count FROM [Movie]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Movie-Cast]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();           
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Movie-Director]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Movie-Audio]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Movie-SubtitleLanguage]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Movie-Genre]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return count;
        }
        public static ObservableCollection<Movie> FetchMovieList (ObservableCollection<Genre> genreList, ObservableCollection<Audio> audioList,
            ObservableCollection<Person> personList, ObservableCollection<Language> languageList, ObservableCollection<HDD> hddList, ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            ObservableCollection<Movie> listOfMovies = new ObservableCollection<Movie> ();
            string SqlCeCommandGetMovies = "SELECT * FROM [Movie] ORDER BY customName";
            string SqlCeCommandGetCast = "SELECT * FROM [Movie-Cast] ORDER BY actorNum";
            string SqlCeCommandGetDirectors = "SELECT * FROM [Movie-Director] ORDER BY directorNum";
            string SqlCeCommandGetAudio = "SELECT * FROM [Movie-Audio]";
            string SqlCeCommandGetSubtitleLanguages = "SELECT * FROM [Movie-SubtitleLanguage]";
            string SqlCeCommandGetGenres = "SELECT * FROM [Movie-Genre] ORDER BY genreNum";
            //SqlCeCommand command;
            //SqlCeDataReader reader;

            //**** MOVIES ****
            command = new SqlCeCommand (SqlCeCommandGetMovies, connection);
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
                tempMovie.IsViewed = (bool) reader["isViewed"];
                int hddID = (int) reader["hddnum"];
                tempMovie.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddID);
                tempMovie.Version = (byte) reader["version"];
                listOfMovies.Add (tempMovie);
                counter++;
            }
            reader.Close ();
            if (listOfMovies.Count == 0)
                return listOfMovies;

            //**** GENRES ****
            command = new SqlCeCommand (SqlCeCommandGetGenres, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
                counter++;
            }
            reader.Close ();

            //**** DIRECTORS ****
            command = new SqlCeCommand (SqlCeCommandGetDirectors, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int directorID = (int) reader["directorID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
                counter++;
            }
            reader.Close ();

            //**** CAST ****
            command = new SqlCeCommand (SqlCeCommandGetCast, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int actorID = (int) reader["personID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
                counter++;
            }
            reader.Close ();

            //**** SUBTITLES ****
            command = new SqlCeCommand (SqlCeCommandGetSubtitleLanguages, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int languageID = (int) reader["subtitleLanguageID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).SubtitleLanguageList.Add (FinderInCollection.FindInLanguageCollection (languageList, languageID));
                counter++;
            }
            reader.Close ();

            //**** AUDIO ****
            command = new SqlCeCommand (SqlCeCommandGetAudio, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["movieID"];
                int audioID = (int) reader["audioID"];
                FinderInCollection.FindInMovieCollection (listOfMovies, movieID).AudioList.Add (FinderInCollection.FindInAudioCollection (audioList, audioID));
                counter++;
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();

            return listOfMovies;
        }

        //WISH
        public static ObservableCollection<WishMovie> FetchWishMovieList (ObservableCollection<Person> personList, ObservableCollection<Genre> genreList) {
            ObservableCollection<WishMovie> movieList = new ObservableCollection<WishMovie> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            string SqlCeCommandGetMovies = "SELECT * FROM [WishMovie] ORDER BY name";
            string SqlCeCommandGetCast = "SELECT * FROM [WishMovie-Cast] ORDER BY actorNum";
            string SqlCeCommandGetDirectors = "SELECT * FROM [WishMovie-Director] ORDER BY directorNum";
            string SqlCeCommandGetGenres = "SELECT * FROM [WishMovie-Genre] ORDER BY genreNum";
            SqlCeCommand command;
            SqlCeDataReader reader;

            #region Main

            command = new SqlCeCommand (SqlCeCommandGetMovies, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                WishMovie tempMovie = new WishMovie ();
                tempMovie.ID = (int) reader["wishMovieID"];
                tempMovie.Name = (string) reader["name"];
                tempMovie.OrigName = (string) reader["origName"];
                tempMovie.Year = (int) reader["year"];
                tempMovie.Rating = (decimal) reader["rating"];
                tempMovie.InternetLink = (string) reader["internetLink"];
                tempMovie.TrailerLink = (string) reader["trailerLink"];
                tempMovie.Summary = (string) reader["plot"];
                tempMovie.Budget = float.Parse (reader["budget"].ToString ());
                tempMovie.Earnings = float.Parse (reader["earnings"].ToString ());
                tempMovie.ReleaseDate = (DateTime) reader["releaseDate"];
                movieList.Add (tempMovie);
            }
            reader.Close ();

            #endregion

            #region Cast

            command = new SqlCeCommand (SqlCeCommandGetCast, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["wishMovieId"];
                int actorID = (int) reader["actorID"];
                FinderInCollection.FindInWishMovieCollection (movieList, movieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
            }
            reader.Close ();

            #endregion

            #region Directors

            command = new SqlCeCommand (SqlCeCommandGetDirectors, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["wishMovieID"];
                int directorID = (int) reader["directorID"];
                FinderInCollection.FindInWishMovieCollection (movieList, movieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
            }
            reader.Close ();

            #endregion

            #region Genres

            command = new SqlCeCommand (SqlCeCommandGetGenres, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int movieID = (int) reader["wishMovieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInWishMovieCollection (movieList, movieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
            }
            reader.Close ();

            #endregion


            if (wasClosed)
                connection.Close ();

            return movieList;
        }

        #endregion

        #region Serie

        //REGULAR
        public static int FetchSerieCount () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            int count = 0;

            stringCommand = "SELECT COUNT(*) as count FROM Serie";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Serie-Genre]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Serie-Cast]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [Serie-Director]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                count += (int) reader["count"];
            }


            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return count;
        }
        public static int FetchSeasonCount () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            int count = 0;


            stringCommand = "SELECT COUNT(*) as count FROM SerieSeason";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count = (int) reader["count"];
            }

            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return count;
        }
        public static int FetchEpisodeCount () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            int count = 0;

            stringCommand = "SELECT COUNT(*) as count FROM SerieEpisode";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [SerieEpisode-Audio]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                count += (int) reader["count"];
            }

            stringCommand = "SELECT COUNT(*) as count FROM [SerieEpisode-SubtitleLanguage]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                count += (int) reader["count"];
            }


            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return count;
        }
        public static ObservableCollection<Serie> FetchSerieList (ObservableCollection<Genre> genreList, ObservableCollection<Person> personList, ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Serie> listOfSeries = new ObservableCollection<Serie> ();
            listOfSeries = new ObservableCollection<Serie> ();

            string getSeriesCommand = "SELECT * FROM Serie ORDER BY name";
            string getSerieGenresCommand = "SELECT * FROM [Serie-Genre] ORDER BY genreNum";
            string getSerieCastCommand = "SELECT * FROM [Serie-Cast] ORDER BY actorNum";
            string getSerieDirectorsCommand = "SELECT * FROM [Serie-Director] ORDER BY directorNum";
            SqlCeCommand sqlCMNDGetSeries;
            SqlCeDataReader reader;

            #region Main

            sqlCMNDGetSeries = new SqlCeCommand (getSeriesCommand, connection);
            reader = sqlCMNDGetSeries.ExecuteReader ();

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
                counter++;
            }
            reader.Close ();
            if (listOfSeries.Count == 0)
                return listOfSeries;

            #endregion

            #region Genres

            sqlCMNDGetSeries = new SqlCeCommand (getSerieGenresCommand, connection);
            reader = sqlCMNDGetSeries.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
                counter++;
            }
            reader.Close ();

            #endregion

            #region Cast

            sqlCMNDGetSeries = new SqlCeCommand (getSerieCastCommand, connection);
            reader = sqlCMNDGetSeries.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int actorID = (int) reader["actorID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
                counter++;
            }
            reader.Close ();

            #endregion

            #region Directors

            sqlCMNDGetSeries = new SqlCeCommand (getSerieDirectorsCommand, connection);
            reader = sqlCMNDGetSeries.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["serieID"];
                int directorID = (int) reader["directorID"];
                FinderInCollection.FindInSerieCollection (listOfSeries, serieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
                counter++;
            }            

            #endregion

            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return listOfSeries;
        }
        public static ObservableCollection<SerieSeason> FetchSerieSeasonList (ObservableCollection<Serie> serieList, ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<SerieSeason> listOfSerSeasons = new ObservableCollection<SerieSeason> ();
            string getSeasonsCommand = "SELECT * FROM SerieSeason ORDER BY name";
            SqlCeCommand slqCMNDGetSeasons = new SqlCeCommand (getSeasonsCommand, connection);
            SqlCeDataReader reader = slqCMNDGetSeasons.ExecuteReader ();
            while (reader.Read ()) {
                SerieSeason newSeason = new SerieSeason ();
                newSeason.ID = (int) reader["seasonID"];
                newSeason.Name = (string) reader["name"];
                newSeason.InternetLink = (string) reader["internetLink"];
                newSeason.TrailerLink = (string) reader["trailerLink"];
                int parentSerieID = (int) reader["serieID"];
                newSeason.ParentSerie = FinderInCollection.FindInSerieCollection (serieList, parentSerieID);
                FinderInCollection.FindInSerieCollection (serieList, parentSerieID).Seasons.Add (newSeason);
                listOfSerSeasons.Add (newSeason);
                counter++;
            }
            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return listOfSerSeasons;
        }
        public static ObservableCollection<SerieEpisode> FetchSerieEpisodeList (ObservableCollection<SerieSeason> seasonList, ObservableCollection<Language> languageList,
            ObservableCollection<Audio> audioList, ObservableCollection<HDD> hddList, ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode> ();
            string getEpisodesCommand = "SELECT * FROM SerieEpisode ORDER BY name";
            string getEpisodeAudio = "SELECT * FROM [SerieEpisode-Audio]";
            string getEpisodeSubtitle = "SELECT * FROM [SerieEpisode-SubtitleLanguage]";
            SqlCeCommand slqCMNDGetEpisodes;
            SqlCeDataReader reader;

            #region Main

            slqCMNDGetEpisodes = new SqlCeCommand (getEpisodesCommand, connection);
            reader = slqCMNDGetEpisodes.ExecuteReader ();
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
                newEpisode.IsViewed = (bool) reader["viewed"];
                int seasonID = (int) reader["seasonID"];
                newEpisode.TrailerLink = (string) reader["trailerLink"];
                newEpisode.InternetLink = (string) reader["internetLink"];
                newEpisode.ParentSeason = FinderInCollection.FindInSerieSeasonCollection (seasonList, seasonID);
                FinderInCollection.FindInSerieSeasonCollection (seasonList, seasonID).Episodes.Add (newEpisode);
                newEpisode.Version = (byte) reader["version"];
                listOfSerEpisodes.Add (newEpisode);
                counter++;
            }
            reader.Close ();
            if (listOfSerEpisodes.Count == 0)
                return listOfSerEpisodes;

            #endregion

            #region Audios

            slqCMNDGetEpisodes = new SqlCeCommand (getEpisodeAudio, connection);
            reader = slqCMNDGetEpisodes.ExecuteReader ();
            while (reader.Read ()) {
                int episodeID = (int) reader["episodeID"];
                int audioID = (int) reader["audioID"];
                FinderInCollection.FindInSerieEpisodeCollection (listOfSerEpisodes, episodeID).AudioList.Add (FinderInCollection.FindInAudioCollection (audioList, audioID));
                counter++;
            }
            reader.Close ();

            #endregion

            #region Subtitles

            slqCMNDGetEpisodes = new SqlCeCommand (getEpisodeSubtitle, connection);
            reader = slqCMNDGetEpisodes.ExecuteReader ();
            while (reader.Read ()) {
                int episodeID = (int) reader["episodeID"];
                int subtitleID = (int) reader["subtitleID"];
                FinderInCollection.FindInSerieEpisodeCollection (listOfSerEpisodes, episodeID).SubtitleLanguageList.Add (FinderInCollection.FindInLanguageCollection (languageList, subtitleID));
                counter++;
            }
            reader.Close ();

            #endregion

            if (wasClosed)
                connection.Close ();

            return listOfSerEpisodes;
        }

        //WISH
        public static ObservableCollection<WishSerie> FetchWishSerieList (ObservableCollection<Genre> genreList, ObservableCollection<Person> personList) {
            ObservableCollection<WishSerie> wishList = new ObservableCollection<WishSerie> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            #region Main

            stringCommand = "SELECT * FROM WishSerie ORDER BY name";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                WishSerie newSerie = new WishSerie ();
                newSerie.ID = (int) reader["wishSerieID"];
                try {
                    newSerie.RegularSerieID = (int) reader["regularSerieID"];
                }
                catch {
                    newSerie.RegularSerieID = 0;
                }
                newSerie.Name = (string) reader["name"];
                newSerie.OrigName = (string) reader["orgName"];
                newSerie.Rating = (decimal) reader["imdbRating"];
                newSerie.TrailerLink = (string) reader["trailerLink"];
                newSerie.InternetLink = (string) reader["internetLink"];
                newSerie.Summary = (string) reader["summary"];
                wishList.Add (newSerie);
            }
            reader.Close ();

            #endregion

            #region Genres

            stringCommand = "SELECT * FROM [WishSerie-Genre] ORDER BY genreNum";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["wishSerieID"];
                int genreID = (int) reader["genreID"];
                FinderInCollection.FindInWishSerieCollection (wishList, serieID).Genres.Add (FinderInCollection.FindInGenreCollection (genreList, genreID));
            }
            reader.Close ();

            #endregion

            #region Cast

            stringCommand = "SELECT * FROM [WishSerie-Cast] ORDER BY actorNum";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int serieID = (int) reader["wishSerieID"];
                int actorID = (int) reader["actorID"];
                FinderInCollection.FindInWishSerieCollection (wishList, serieID).Actors.Add (FinderInCollection.FindInPersonCollection (personList, actorID));
            }
            reader.Close ();

            #endregion

            #region Directors

            stringCommand = "SELECT * FROM [WishSerie-Director] ORDER BY directorNum";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int directorID = (int) reader["directorID"];
                int serieID = (int) reader["wishSerieID"];
                FinderInCollection.FindInWishSerieCollection (wishList, serieID).Directors.Add (FinderInCollection.FindInPersonCollection (personList, directorID));
            }
            reader.Close ();

            #endregion

            if (wasClosed)
                connection.Close ();

            return wishList;
        }
        public static ObservableCollection<WishSerieSeason> FetchWishSerieSeasonList (ObservableCollection<WishSerie> wishSerieList) {
            ObservableCollection<WishSerieSeason> wishList = new ObservableCollection<WishSerieSeason> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            stringCommand = "SELECT * FROM WishSerieSeason ORDER BY name";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                WishSerieSeason newSeason = new WishSerieSeason ();
                newSeason.ID = (int) reader["wishSerieSeasonID"];
                try {
                    newSeason.RegularSeasonID = (int) reader["regularSeasonID"];
                }
                catch {
                    newSeason.RegularSeasonID = 0;
                }
                int parentWishSerieID = (int) reader["wishSerieID"];
                newSeason.ParentWishSerie = FinderInCollection.FindInWishSerieCollection (wishSerieList, parentWishSerieID);
                FinderInCollection.FindInWishSerieCollection (wishSerieList, parentWishSerieID).WishSeasons.Add (newSeason);

                newSeason.Name = (string) reader["name"];
                newSeason.InternetLink = (string) reader["internetLink"];
                newSeason.TrailerLink = (string) reader["trailerLink"];
                wishList.Add (newSeason);
            }
            reader.Close ();


            if (wasClosed)
                connection.Close ();
            return wishList;
        }
        public static ObservableCollection<WishSerieEpisode> FetchWishSerieEpisodeList (ObservableCollection<WishSerieSeason> wishSeasonList) {
            ObservableCollection<WishSerieEpisode> wishList = new ObservableCollection<WishSerieEpisode> ();            
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            stringCommand = "SELECT * FROM WishSerieEpisode ORDER BY name";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                WishSerieEpisode newEpisode = new WishSerieEpisode ();
                newEpisode.ID = (int) reader["wishSerieEpisodeID"];
                newEpisode.Name = (string) reader["name"];
                newEpisode.OrigName = (string) reader["orgName"];
                int parentWishSeasonID = (int) reader["wishSerieSeasonID"];
                newEpisode.ParentWishSeason = FinderInCollection.FindInWishSerieSeasonCollection (wishSeasonList, parentWishSeasonID);
                FinderInCollection.FindInWishSerieSeasonCollection (wishSeasonList, parentWishSeasonID).WishEpisodes.Add (newEpisode);
                newEpisode.AirDate = (DateTime) reader["airDate"];
                newEpisode.Summary = (string) reader["summary"];
                newEpisode.TrailerLink = (string) reader["trailerLink"];
                newEpisode.InternetLink = (string) reader["internetLink"];
                wishList.Add (newEpisode);
            }
            reader.Close ();

            if (wasClosed)
                connection.Close ();
            return wishList;
        }

        #endregion

        #region Home Video

        //REGULAR
        public static ObservableCollection<HomeVideo> FetchHomeVideosList (ObservableCollection<Category> categoryList, ObservableCollection<Camera> cameraList,
            ObservableCollection<Language> languageList, ObservableCollection<Audio> audioList, ObservableCollection<HDD> hddList,
            ObservableCollection<Person> homeVideoPersonsList) {
            ObservableCollection<HomeVideo> homeVideoList = new ObservableCollection<HomeVideo> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode> ();
            string getHomeVideosCommand = "SELECT * FROM HomeVideo";
            string getHomeVideosAudioCommand = "SELECT * FROM [HomeVideo-Audio]";
            string getHomeVideoCamermansCommand = "SELECT * FROM [HomeVideo-Camerman]";
            string getHomeVideoPersonsCommand = "SELECT * FROM [HomeVideo-Person]";
            SqlCeDataReader reader;

            #region Main

            command = new SqlCeCommand (getHomeVideosCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                HomeVideo newHV = new HomeVideo ();
                newHV.ID = (int) reader["homeVideoID"];
                newHV.Name = (string) reader["name"];
                int catID = (int) reader["categoryID"];
                newHV.VideoCategory = FinderInCollection.FindInCategoryCollection (categoryList, catID);
                newHV.FilmDate = (DateTime) reader["date"];
                newHV.Location = (string) reader["Location"];
                int cameraID = (int) reader["cameraID"];
                newHV.FilmingCamera = FinderInCollection.FindInCameraCollection (cameraList, cameraID);
                newHV.Size = (long) reader["size"];
                newHV.Runtime = (int) reader["runtime"];
                newHV.Height = (int) reader["height"];
                newHV.Width = (int) reader["width"];
                newHV.Bitrate = (int) reader["bitrate"];
                newHV.AspectRatio = (string) reader["aspectRatio"];
                int hddID = (int) reader["hddID"];
                newHV.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddID);
                newHV.Summary = (string) reader["comment"];
                homeVideoList.Add (newHV);
            }
            reader.Close ();

            #endregion

            #region Audio

            command = new SqlCeCommand (getHomeVideosAudioCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int hvID = (int) reader["homeVideoID"];
                int audioID = (int) reader["audioID"];
                FinderInCollection.FindInHomeVideoCollection (homeVideoList, hvID).AudioList.Add (FinderInCollection.FindInAudioCollection (audioList, audioID));
            }
            reader.Close ();

            #endregion

            #region Camermans

            command = new SqlCeCommand (getHomeVideoCamermansCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int camermanID = (int) reader["camermanID"];
                int hvID = (int) reader["homeVideoID"];
                FinderInCollection.FindInHomeVideoCollection (homeVideoList, hvID).Camermans.Add (FinderInCollection.FindInPersonCollection (homeVideoPersonsList, camermanID));
            }
            reader.Close ();

            #endregion

            #region Persons

            command = new SqlCeCommand (getHomeVideoPersonsCommand, connection);
            reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int personID = (int) reader["personID"];
                int hvID = (int) reader["homeVideoID"];
                FinderInCollection.FindInHomeVideoCollection (homeVideoList, hvID).PersonsInVideo.Add (FinderInCollection.FindInPersonCollection (homeVideoPersonsList, personID));
            }
            reader.Close ();

            #endregion

            if (wasClosed)
                connection.Close ();
            return homeVideoList;
        }

        //WISH
        public static ObservableCollection<WishHomeVideo> FetchWishHomeVideoList (ObservableCollection<Person> personList, ObservableCollection<Category> categoryList,
            ObservableCollection<Camera> cameraList) {
            ObservableCollection<WishHomeVideo> wishList = new ObservableCollection<WishHomeVideo> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            string SqlCeCommandGetHVs = "SELECT * FROM [WishHomeVideo]";
            string SqlCeCommandGetPersons = "SELECT * FROM [WishHV-Person] ";
            string SqlCeCommandGetCamermans = "SELECT * FROM [WishHV-Camerman]";
            SqlCeCommand command;
            SqlCeDataReader reader;

            #region Main

            command = new SqlCeCommand (SqlCeCommandGetHVs, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                WishHomeVideo wish = new WishHomeVideo ();
                wish.ID = (int) reader["wishHomeVideoID"];
                wish.Name = (string) reader["name"];
                int catID = (int) reader["categoryID"];
                wish.VideoCategory = FinderInCollection.FindInCategoryCollection (categoryList, catID);
                wish.Location = (string) reader["location"];
                int cameraID = (int) reader["cameraID"];
                wish.FilmingCamera = FinderInCollection.FindInCameraCollection (cameraList, cameraID);
                wish.FilmDate = (DateTime) reader["expectedDate"];
                wish.Comment = (string) reader["comment"];
                wishList.Add (wish);
            }
            reader.Close ();

            #endregion

            #region Persons

            command = new SqlCeCommand (SqlCeCommandGetPersons, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int personID = (int) reader["personID"];
                int wishID = (int) reader["wishHVID"];
                FinderInCollection.FindInWishHomeVideoCollection (wishList, wishID).PersonsInVideo.Add (FinderInCollection.FindInPersonCollection (personList, personID));
            }
            reader.Close ();

            #endregion

            #region Camerman

            command = new SqlCeCommand (SqlCeCommandGetCamermans, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int camermanID = (int) reader["camermanID"];
                int wishID = (int) reader["wishHVID"];
                FinderInCollection.FindInWishHomeVideoCollection (wishList, wishID).Camermans.Add (FinderInCollection.FindInPersonCollection (personList, camermanID));
            }
            reader.Close ();

            #endregion

            if (wasClosed)
                connection.Close ();
            return wishList;
        }

        #endregion

        #region Misc

        public static int FetchPersonCount () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            stringCommand = "SELECT COUNT(*) as count FROM Person";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();

            int count = 0;
            while (reader.Read ()) {
                count = (int) reader["count"];
            } 
            if (wasClosed)
                connection.Close ();
            reader.Close ();
            return count;
        }
        public static ObservableCollection<Person> FetchPersonList (ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            ObservableCollection<Person> personList = new ObservableCollection<Person> ();
            stringCommand = "SELECT * FROM Person";// WHERE typeID = 2"; 
            command = new SqlCeCommand (stringCommand, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int id = (int) reader["personID"];
                string name = (string) reader["personName"];
                int typeID = (int) reader["typeID"];
                Person tempPerson = new Person (new PersonType (typeID));
                tempPerson.Name = name;
                tempPerson.ID = id;
                tempPerson.DateOfBirth = (DateTime) reader["dateOfBirth"];
                personList.Add (tempPerson);
                counter++;
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return personList;
        }
        public static ObservableCollection<Person> FetchHomeVideoPersonList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            ObservableCollection<Person> personList = new ObservableCollection<Person> ();
            stringCommand = "SELECT * FROM Person WHERE typeID = 1";
            command = new SqlCeCommand (stringCommand, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

            while (reader.Read ()) {
                int id = (int) reader["personID"];
                string name = (string) reader["personName"];
                int typeID = (int) reader["typeID"];
                Person tempPerson = new Person (new PersonType (typeID));
                tempPerson.Name = name;
                tempPerson.ID = id;
                tempPerson.DateOfBirth = (DateTime) reader["dateOfBirth"];
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
            string SqlCeCommandGetHdds = "SELECT * FROM HDD";
            SqlCeCommand command = new SqlCeCommand (SqlCeCommandGetHdds, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

            while (reader.Read ()) {
                HDD tempHdd = new HDD ();
                tempHdd.ID = (int) reader["hddID"];
                tempHdd.Name = (string) reader["name"];
                tempHdd.PurchaseDate = (DateTime) reader["purchaseDate"];
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
        public static ObservableCollection<Genre> FetchGenreList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Genre> genreList = new ObservableCollection<Genre> ();
            string SqlCeCommandGetGenres2 = "SELECT * FROM [Genre]";
            SqlCeCommand command = new SqlCeCommand (SqlCeCommandGetGenres2, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

            while (reader.Read ()) {
                Genre tempGenre = new Genre ();
                tempGenre.ID = (int) reader["genreID"];
                tempGenre.Name = (string) reader["genreName"];
                genreList.Add (tempGenre);
            }
            if (wasClosed)
                connection.Close ();
            reader.Close ();
            return genreList;
        }
        public static ObservableCollection<Language> FetchLanguageList () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            ObservableCollection<Language> languageList = new ObservableCollection<Language> ();
            string SqlCeCommandGetLanguages = "SELECT * FROM Language";
            SqlCeCommand command = new SqlCeCommand (SqlCeCommandGetLanguages, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

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
            string SqlCeCommandAudio = "SELECT * FROM Audio";
            SqlCeCommand command = new SqlCeCommand (SqlCeCommandAudio, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

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
        public static ObservableCollection<Camera> FetchCameraList () {
            ObservableCollection<Camera> cameraList = new ObservableCollection<Camera> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            stringCommand = "SELECT * FROM Camera";
            command = new SqlCeCommand (stringCommand, connection);
            SqlCeDataReader reader = command.ExecuteReader ();

            while (reader.Read ()) {
                Camera newCamera = new Camera ();
                newCamera.ID = (int) reader["cameraID"];
                newCamera.Name = (string) reader["cameraName"];
                newCamera.PurchaseDate = (DateTime) reader["purchaseDate"];
                newCamera.WarrantyLengt = (int) reader["warrantyLength"];
                cameraList.Add (newCamera);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();

            return cameraList;
        }
        public static Settings FetchSettings () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            Settings settings = new Settings ();
            string getSettingsCommand = "SELECT * FROM Settings";
            SqlCeCommand SqlCeCommandGetSettings = new SqlCeCommand (getSettingsCommand, connection);
            SqlCeDataReader reader = SqlCeCommandGetSettings.ExecuteReader ();
            reader.Read ();
            try {
                settings.ID = (int) reader["settingsID"];
                settings.TrailerProvider = (string) reader["trailerSearchProvider"];
                settings.PosterProvider = (string) reader["posterSearchProvider"];
                settings.UseOnlyOriginalName = (bool) reader["useOnlyOriginalName"];
                settings.ImageQualityLevel = (int) reader["imageQualityLevel"];
                settings.ImageEditorPath = (string) reader["imageEditorPath"];
            }
            catch {
            }

            if (wasClosed)
                connection.Close ();
            reader.Close ();

            return settings;
        }
        public static ObservableCollection<Category> FetchCategories () {
            ObservableCollection<Category> categories = new ObservableCollection<Category> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            string getCategoriesCommand = "SELECT * FROM HVCategory";
            SqlCeCommand command = new SqlCeCommand (getCategoriesCommand, connection);
            SqlCeDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                Category newCat = new Category ();
                newCat.Name = (string) reader["name"];
                newCat.ID = (int) reader["homeVideoCatID"];
                categories.Add (newCat);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return categories;
        }
        public static List<PersonType> FetchPersonTypes () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            List<PersonType> list = new List<PersonType> ();
            command = new SqlCeCommand ("SELECT * FROM PersonType", connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                //PersonType newType = new PersonType ();
                //newType.ID = (int) reader["typeID"];
                //newType.Name = (string) reader["typeName"];
                //list.Add (newType);
            }
            reader.Close ();
            if (wasClosed)
                connection.Close ();
            return list;
        }
        public static ObservableCollection<Selection> FetchSelectionList (ObservableCollection<Movie> movieList, ObservableCollection<SerieEpisode> episodeList) {
            ObservableCollection<Selection> selectionList = new ObservableCollection<Selection> ();
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();

            stringCommand = "SELECT * FROM Selection";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                Selection newSel = new Selection ();
                newSel.ID = (int) reader["selectionID"];
                newSel.Name = (string) reader["name"];
                newSel.Size = (long) reader["hddSize"];
                newSel.DateOfLastChange = (DateTime) reader["dateLastChange"];
                selectionList.Add (newSel);
            }
            reader.Close ();

            stringCommand = "SELECT * FROM [Selection-Movie]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int selectionID = (int) reader["selectionID"];
                int movieID = (int) reader["movieID"];
                FinderInCollection.FindInSelectionCollection (selectionList, selectionID).SelectedMovies.Add (FinderInCollection.FindInMovieCollection (movieList, movieID));
            }
            reader.Close ();

            stringCommand = "SELECT * FROM [Selection-SerieEpisode]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            while (reader.Read ()) {
                int selectionID = (int) reader["selectionID"];
                int episodeID = (int) reader["serieEpisodeID"];
                FinderInCollection.FindInSelectionCollection (selectionList, selectionID).selectedEpisodes.Add (FinderInCollection.FindInSerieEpisodeCollection (episodeList, episodeID));
            }
            reader.Close ();

            if (wasClosed)
                connection.Close ();
            return selectionList;
        }
        public static decimal ThisVersionNumber () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            stringCommand = "SELECT versionNumber FROM [version]";
            command = new SqlCeCommand (stringCommand, connection);
            reader = command.ExecuteReader ();
            reader.Read ();
            decimal version;
            try {
                version = (decimal) reader["versionNumber"];
            }
            catch {
                version = 0;
            }
            reader.Close ();

            if (wasClosed)
                connection.Close ();
            return version;
        }

        #endregion
        
        #endregion

        #region INSERT / UPDATE / DELETE

        static int GetIDOfLastEntry (string tableIDName, string tableName) {
            string retreiveString = string.Format ("SELECT {0} FROM {1} ORDER BY {2} DESC", tableIDName, tableName, tableIDName);
            SqlCeCommand SqlCeCommandGetEntry = new SqlCeCommand (retreiveString, connection);
            SqlCeDataReader reader = SqlCeCommandGetEntry.ExecuteReader ();
            reader.Read ();
            int id = (int) reader[tableIDName];
            reader.Close ();
            return id;
        }
        static void ExecuteCommand (SqlCeCommand SqlCeCommand) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            SqlCeCommand.ExecuteNonQuery ();
            SqlCeCommand.Parameters.Clear ();
            if (wasClosed)
                connection.Close ();
        }
        static int ExecuteCommandAndGetID (SqlCeCommand SqlCeCommand, string tableIDName, string tableName) {
            int id;
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            command.ExecuteNonQuery ();
            command.Parameters.Clear ();
            id = GetIDOfLastEntry (tableIDName, tableName);
            if (wasClosed)
                connection.Close ();
            return id;
        }

        #region MOVIE

        //Regular
        public static void InsertNewMovieForSelection (Movie movie) {
            stringCommand = "INSERT INTO Movie (movieID, customName, originalName, addDate, year, rating, internetLink, " +
                                        "trailerLink, plot, size, height, width, runtime, bitrate, aspectRatio, budget, " +
                                        "earnings, isViewed, hddNum, version) VALUES " +
                                        "(@movieID, @customName, @originalName, @addDate, @year, @rating, " +
                                        "@internetLink, @trailerLink, @plot, @size, @height, @width, " +
                                        "@runtime, @bitrate, @aspectRatio, @budget, @earnings, @isViewed, @hddNum, @version)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            command.Parameters.AddWithValue ("@customName", movie.Name);
            command.Parameters.AddWithValue ("@originalName", movie.OrigName);
            command.Parameters.AddWithValue ("@addDate", movie.AddTime);
            command.Parameters.AddWithValue ("@year", movie.Year);
            command.Parameters.AddWithValue ("@rating", movie.Rating);
            command.Parameters.AddWithValue ("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("@plot", movie.Summary);
            command.Parameters.AddWithValue ("@size", movie.Size);
            command.Parameters.AddWithValue ("@height", movie.Height);
            command.Parameters.AddWithValue ("@width", movie.Width);
            command.Parameters.AddWithValue ("@runtime", movie.Runtime);
            command.Parameters.AddWithValue ("@bitrate", movie.Bitrate);
            command.Parameters.AddWithValue ("@aspectRatio", movie.AspectRatio);
            command.Parameters.AddWithValue ("@budget", movie.Budget);
            command.Parameters.AddWithValue ("@earnings", movie.Earnings);
            command.Parameters.AddWithValue ("@isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("@hddNum", movie.Hdd.ID);
            command.Parameters.AddWithValue ("@version", movie.Version);

            ExecuteCommand (command);

            InsertMovieAudios (movie);
            InsertMovieSubtitles (movie);
            InsertMovieCast (movie);
            InsertMovieDirectors (movie);
            InsertMovieGenres (movie);
        }
        public static void InsertNewMovie (Movie movie) {
            movie.Version = 1;
            movie.AddTime = DateTime.Now;
            stringCommand = "INSERT INTO Movie (customName, originalName, addDate, year, rating, internetLink, " +
                                        "trailerLink, plot, size, height, width, runtime, bitrate, aspectRatio, budget, " +
                                        "earnings, isViewed, hddNum, version) VALUES " +
                                        "(@customName, @originalName, @addDate, @year, @rating, " +
                                        "@internetLink, @trailerLink, @plot, @size, @height, @width, " +
                                        "@runtime, @bitrate, @aspectRatio, @budget, @earnings, @isViewed, @hddNum, @version)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@customName", movie.Name);
            command.Parameters.AddWithValue ("@originalName", movie.OrigName);
            command.Parameters.AddWithValue ("@addDate", movie.AddTime);
            command.Parameters.AddWithValue ("@year", movie.Year);
            command.Parameters.AddWithValue ("@rating", movie.Rating);
            command.Parameters.AddWithValue ("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("@plot", movie.Summary);
            command.Parameters.AddWithValue ("@size", movie.Size);
            command.Parameters.AddWithValue ("@height", movie.Height);
            command.Parameters.AddWithValue ("@width", movie.Width);
            command.Parameters.AddWithValue ("@runtime", movie.Runtime);
            command.Parameters.AddWithValue ("@bitrate", movie.Bitrate);
            command.Parameters.AddWithValue ("@aspectRatio", movie.AspectRatio);
            command.Parameters.AddWithValue ("@budget", movie.Budget);
            command.Parameters.AddWithValue ("@earnings", movie.Earnings);
            command.Parameters.AddWithValue ("@isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("@hddNum", movie.Hdd.ID);
            command.Parameters.AddWithValue ("@version", movie.Version);

            movie.ID = ExecuteCommandAndGetID (command, "movieID", "Movie");

            InsertMovieAudios (movie);
            InsertMovieSubtitles (movie);
            InsertMovieCast (movie);
            InsertMovieDirectors (movie);
            InsertMovieGenres (movie);
        }
        public static void UpdateMovie (Movie movie) {
            movie.Version++;
            stringCommand = "UPDATE Movie SET customName = @customName ,originalName = @originalName ,addDate = @addDate ," +
                                        "year = @year ,rating = @rating ,internetLink = @internetLink ,trailerLink = @trailerLink ," +
                                        "plot = @plot ,size = @size ,height = @height ,width = @width ,runtime = @runtime ," +
                                        "bitrate = @bitrate ,aspectRatio = @aspectRatio ,budget = @budget ,earnings = @earnings ," +
                                        "isViewed = @isViewed ,hddNum = @hddNum, version = @version WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@customName", movie.Name);
            command.Parameters.AddWithValue ("@originalName", movie.OrigName);
            command.Parameters.AddWithValue ("@addDate", movie.AddTime);
            command.Parameters.AddWithValue ("@year", movie.Year);
            command.Parameters.AddWithValue ("@rating", movie.Rating);
            command.Parameters.AddWithValue ("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("@plot", movie.Summary);
            command.Parameters.AddWithValue ("@size", movie.Size);
            command.Parameters.AddWithValue ("@height", movie.Height);
            command.Parameters.AddWithValue ("@width", movie.Width);
            command.Parameters.AddWithValue ("@runtime", movie.Runtime);
            command.Parameters.AddWithValue ("@bitrate", movie.Bitrate);
            command.Parameters.AddWithValue ("@aspectRatio", movie.AspectRatio);
            command.Parameters.AddWithValue ("@budget", movie.Budget);
            command.Parameters.AddWithValue ("@earnings", movie.Earnings);
            command.Parameters.AddWithValue ("@isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("@hddNum", movie.Hdd.ID);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            command.Parameters.AddWithValue ("@version", movie.Version);

            ExecuteCommand (command);

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
        public static void UpdateMovieChangeOnlyIsViewed (Movie movie) {
            stringCommand = "UPDATE Movie SET isViewed = @isViewed WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@isViewed", movie.IsViewed);
            command.Parameters.AddWithValue ("@movieID", movie.ID);

            ExecuteCommand (command);
        }
        public static void DeleteMovie (Movie movie) {
            stringCommand = "DELETE FROM Movie WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }

        static void InsertMovieAudios (Movie movie) {
            foreach (Audio tempAudio in movie.AudioList) {
                stringCommand = "INSERT INTO [Movie-Audio] (movieID, audioID) VALUES (@movieID, @audioID)";
                command = new SqlCeCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@audioID", tempAudio.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteMovieAudios (Movie movie) {
            stringCommand = "DELETE FROM [Movie-Audio] WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);

            ExecuteCommand (command);
        }
        static void InsertMovieSubtitles (Movie movie) {
            stringCommand = "INSERT INTO [Movie-SubtitleLanguage] (movieID ,subtitleLanguageID) VALUES (@movieID, @subtitleLanguageID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Language tempLang in movie.SubtitleLanguageList) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@subtitleLanguageID", tempLang.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteMovieSubtitles (Movie movie) {
            stringCommand = "DELETE FROM [Movie-SubtitleLanguage] WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }
        static void InsertMovieGenres (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Movie-Genre] (movieID ,genreID, genreNum) VALUES (@movieID, @genreID, @genreNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Genre tempGenre in movie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("@genreNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteMovieGenres (Movie movie) {
            stringCommand = "DELETE FROM [Movie-Genre] WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }
        static void InsertMovieCast (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Movie-Cast] (personID ,movieID, actorNum) VALUES (@personID, @movieID, @actorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person actor in movie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@personID", actor.ID);
                command.Parameters.AddWithValue ("@actorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteMovieCast (Movie movie) {
            stringCommand = "DELETE FROM [Movie-Cast] WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }
        static void InsertMovieDirectors (Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Movie-Director] (movieID ,directorID, directorNum) VALUES (@movieID, @directorID, @directorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person director in movie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@directorID", director.ID);
                command.Parameters.AddWithValue ("@directorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteMovieDirectors (Movie movie) {
            stringCommand = "DELETE FROM [Movie-Director] WHERE movieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }

        //Wish
        public static void InsertNewWishMovie (WishMovie movie) {
            stringCommand = "INSERT INTO WishMovie (name, origName, rating, year, internetLink, " +
                                        "trailerLink, plot, budget, earnings, releaseDate) VALUES " +
                                        "(@name, @origName, @rating, @year, " +
                                        "@internetLink, @trailerLink, @plot, " +
                                        "@budget, @earnings, @releaseDate)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", movie.Name);
            command.Parameters.AddWithValue ("@origName", movie.OrigName);
            command.Parameters.AddWithValue ("@rating", movie.Rating);
            command.Parameters.AddWithValue ("@year", movie.Year);
            command.Parameters.AddWithValue ("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("@plot", movie.Summary);
            command.Parameters.AddWithValue ("@budget", movie.Budget);
            command.Parameters.AddWithValue ("@earnings", movie.Earnings);
            command.Parameters.AddWithValue ("@releaseDate", movie.ReleaseDate);

            movie.ID = ExecuteCommandAndGetID (command, "wishMovieID", "WishMovie");

            InsertWishMovieCast (movie);
            InsertWishMovieDirectors (movie);
            InsertWishMovieGenres (movie);
        }
        public static void UpdateWishMovie (WishMovie movie) {
            stringCommand = "UPDATE WishMovie SET name = @name ,origName = @origName ,rating = @rating ,year = @year ," +
                "internetLink = @internetLink ,trailerLink = @trailerLink ,plot = @plot ," +
                "budget = @budget ,earnings = @earnings ,releaseDate = @releaseDate WHERE wishMovieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", movie.Name);
            command.Parameters.AddWithValue ("@origName", movie.OrigName);
            command.Parameters.AddWithValue ("@rating", movie.Rating);
            command.Parameters.AddWithValue ("@year", movie.Year);
            command.Parameters.AddWithValue ("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue ("@plot", movie.Summary);
            command.Parameters.AddWithValue ("@budget", movie.Budget);
            command.Parameters.AddWithValue ("@earnings", movie.Earnings);
            command.Parameters.AddWithValue ("@releaseDate", movie.ReleaseDate);
            command.Parameters.AddWithValue ("@movieID", movie.ID);

            DeleteWishMovieCast (movie);
            DeleteWishmovieDirectors (movie);
            DeleteWishMovieGenres (movie);
            InsertWishMovieCast (movie);
            InsertWishMovieDirectors (movie);
            InsertWishMovieGenres (movie);
        }
        public static void DeleteWishMovie (WishMovie movie) {
            stringCommand = "DELETE FROM WishMovie WHERE wishMovieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }

        static void InsertWishMovieGenres (WishMovie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishMovie-Genre] (wishMovieID ,genreID, genreNum) VALUES (@movieID, @genreID, @genreNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Genre tempGenre in movie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("@genreNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteWishMovieGenres (WishMovie movie) {
            stringCommand = "DELETE FROM [WishMovie-Genre] where wishMovieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }
        static void InsertWishMovieCast (WishMovie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishMovie-Cast] (actorID ,wishMovieID, actorNum) VALUES (@personID, @movieID, @actorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person actor in movie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@personID", actor.ID);
                command.Parameters.AddWithValue ("@actorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteWishMovieCast (WishMovie movie) {
            stringCommand = "DELETE FROM [WishMovie-Cast] WHERE wishMovieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }
        static void InsertWishMovieDirectors (WishMovie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishMovie-Director] (wishMovieID ,directorID, directorNum) VALUES (@movieID, @directorID, @directorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person director in movie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@movieID", movie.ID);
                command.Parameters.AddWithValue ("@directorID", director.ID);
                command.Parameters.AddWithValue ("@directorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteWishmovieDirectors (WishMovie movie) {
            stringCommand = "DELETE FROM [WishMovie-Director] WHERE wishMovieID = @movieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@movieID", movie.ID);
            ExecuteCommand (command);
        }

        #endregion

        #region SERIE

        //Regular
        public static void InsertSerieForSelection (Serie serie) {
            stringCommand = "INSERT INTO Serie (serieID, name, origName, rating, internetLink, summary, trailerLink) VALUES (@serieID, @name, @origName, @rating, @internetLink, @summary, @trailerLink)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            command.Parameters.AddWithValue ("@name", serie.Name);
            command.Parameters.AddWithValue ("@origName", serie.OrigName);
            command.Parameters.AddWithValue ("@rating", serie.Rating);
            command.Parameters.AddWithValue ("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("@summary", serie.Summary);
            command.Parameters.AddWithValue ("@trailerLink", serie.TrailerLink);

            ExecuteCommand (command);

            InsertSerieCast (serie);
            InsertSerieDirectors (serie);
            InsertSerieGenres (serie);
        }
        public static void InsertSerie (Serie serie) {
            stringCommand = "INSERT INTO Serie (name, origName, rating, internetLink, summary, trailerLink) VALUES (@name, @origName, @rating, @internetLink, @summary, @trailerLink)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", serie.Name);
            command.Parameters.AddWithValue ("@origName", serie.OrigName);
            command.Parameters.AddWithValue ("@rating", serie.Rating);
            command.Parameters.AddWithValue ("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("@summary", serie.Summary);
            command.Parameters.AddWithValue ("@trailerLink", serie.TrailerLink);

            serie.ID = ExecuteCommandAndGetID (command, "serieID", "Serie");

            InsertSerieCast (serie);
            InsertSerieDirectors (serie);
            InsertSerieGenres (serie);
        }
        public static void UpdateSerie (Serie serie) {
            stringCommand = "UPDATE Serie SET name = @name, origName = @origName, rating = @rating, internetLink = @internetLink, summary = @summary, trailerLink = @trailerLink WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", serie.Name);
            command.Parameters.AddWithValue ("@origName", serie.OrigName);
            command.Parameters.AddWithValue ("@rating", serie.Rating);
            command.Parameters.AddWithValue ("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("@summary", serie.Summary);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            command.Parameters.AddWithValue ("@trailerLink", serie.TrailerLink);

            ExecuteCommand (command);

            DeleteSerieCast (serie);
            InsertSerieCast (serie);

            DeleteSerieDirectors (serie);
            InsertSerieDirectors (serie);

            DeleteSerieGenres (serie);
            InsertSerieGenres (serie);

            //osvjezi i listu sezona
            
            List<int> seasonsIdList = new List<int> (); //lista svih ID-jeva sezona koje postoje u seriji
            foreach (SerieSeason tempSeason in serie.Seasons) {
                if (tempSeason.ID == 0) //ako je ID 0 znaci da je sezona nova i da ju treba unijet
                    InsertSerieSeason (tempSeason);
                else
                    UpdateSerieSeason (tempSeason);
                seasonsIdList.Add (tempSeason.ID);
            }

            //dohvati sve sezone koje pripadaju seriji
            bool wasClosed = true;            
            stringCommand = "SELECT seasonID FROM SerieSeason WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedSeasonIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji po bazi podataka
            SqlCeDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedSeasonIDList.Add ((int) reader["seasonID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene sezone iz baze, ako dohvacena serija nije u listi sezona obriši ju iz baze
            foreach (int seasonID in fetchedSeasonIDList) {
                if (seasonsIdList.Contains (seasonID) == false) {
                    stringCommand = "DELETE FROM SerieSeason WHERE seasonID = @seasonID";
                    command = new SqlCeCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("@seasonID", seasonID);
                    command.ExecuteNonQuery ();
                }
            }

            if (wasClosed)
                connection.Close ();

        }
        public static void DeleteSerie (Serie serie) {
            stringCommand = "DELETE FROM Serie WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            ExecuteCommand (command);
        }

        public static void InsertSerieSeasonSelection (SerieSeason season) {
            stringCommand = "INSERT INTO SerieSeason (seasonID, serieID ,name ,internetLink ,trailerLink) VALUES (@seasonID, @serieID ,@name ,@internetLink ,@trailerLink)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@seasonID", season.ID);
            command.Parameters.AddWithValue ("@serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("@name", season.Name);
            command.Parameters.AddWithValue ("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", season.TrailerLink);

            ExecuteCommand (command);
        }
        public static void InsertSerieSeason (SerieSeason season) {
            stringCommand = "INSERT INTO SerieSeason (serieID ,name ,internetLink ,trailerLink) VALUES (@serieID ,@name ,@internetLink ,@trailerLink)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("@name", season.Name);
            command.Parameters.AddWithValue ("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", season.TrailerLink);

            season.ID = ExecuteCommandAndGetID (command, "seasonID", "SerieSeason");
        }
        public static void UpdateSerieSeason (SerieSeason season) {
            stringCommand = "UPDATE SerieSeason SET serieID = @serieID ,name = @name ,internetLink = @internetLink ,trailerLink = @trailerLink WHERE seasonID = @seasonID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("@name", season.Name);
            command.Parameters.AddWithValue ("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", season.TrailerLink);
            command.Parameters.AddWithValue ("@seasonID", season.ID);
            ExecuteCommand (command);
                        

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
            stringCommand = "SELECT episodeID FROM SerieEpisode WHERE seasonID = @seasonID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@seasonID", season.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedEpisodeIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji u bazi podataka
            SqlCeDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedEpisodeIDList.Add ((int) reader["episodeID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene epizode, ako dohvacena epizoda nije u listi epizoda sezone, obriši ju iz baze
            foreach (int episodeID in fetchedEpisodeIDList) {
                if (episodesIDList.Contains (episodeID) == false) {
                    stringCommand = "DELETE FROM SerieEpisode WHERE episodeID = @episodeID";
                    command = new SqlCeCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("@episodeID", episodeID);
                    command.ExecuteNonQuery ();
                }
            }

            if (wasClosed)
                connection.Close ();
        }
        public static void DeleteSerieSeason (SerieSeason season) {
            stringCommand = "DELETE FROM SerieSeason WHERE seasonID = @seasonID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@seasonID", season.ID);
            ExecuteCommand (command);
        }

        public static void InsertSerieEpisodeForSelection (SerieEpisode episode) {
            stringCommand = "INSERT INTO SerieEpisode (episodeID, name ,origName ,seasonID ,addDate ,airDate ," +
                "size ,runtime ,bitrate ,width ,height ,aspectRatio ,hddID ,summary ,viewed, internetLink, trailerLink, version) VALUES " +
                "(@episodeID, @name, @origName, @seasonID, @addDate, @airDate, @size, @runtime, @bitrate, " +
                "@width, @height, @aspectRatio, @hddID, @summary, @viewed, @internetLink, @trailerLink, @version)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);
            command.Parameters.AddWithValue ("@name", episode.Name);
            command.Parameters.AddWithValue ("@origName", episode.OrigName);
            command.Parameters.AddWithValue ("@seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue ("@addDate", episode.AddTime);
            command.Parameters.AddWithValue ("@airDate", episode.AirDate);
            command.Parameters.AddWithValue ("@size", episode.Size);
            command.Parameters.AddWithValue ("@runtime", episode.Runtime);
            command.Parameters.AddWithValue ("@bitrate", episode.Bitrate);
            command.Parameters.AddWithValue ("@width", episode.Width);
            command.Parameters.AddWithValue ("@height", episode.Height);
            command.Parameters.AddWithValue ("@aspectRatio", episode.AspectRatio);
            command.Parameters.AddWithValue ("@hddID", episode.Hdd.ID);
            command.Parameters.AddWithValue ("@summary", episode.Summary);
            command.Parameters.AddWithValue ("@viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", episode.TrailerLink);
            command.Parameters.AddWithValue ("@version", episode.Version);

            ExecuteCommand (command);

            InsertSerieEpisodeAudios (episode);
            InsertSerieEpisodeSubtitles (episode);
        }
        public static void InsertSerieEpisode (SerieEpisode episode) {
            episode.Version = 1;
            stringCommand = "INSERT INTO SerieEpisode (name ,origName ,seasonID ,addDate ,airDate ," +
                "size ,runtime ,bitrate ,width ,height ,aspectRatio ,hddID ,summary ,viewed, internetLink, trailerLink, version) VALUES " +
                "(@name, @origName, @seasonID, @addDate, @airDate, @size, @runtime, @bitrate, " +
                "@width, @height, @aspectRatio, @hddID, @summary, @viewed, @internetLink, @trailerLink, @version)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", episode.Name);
            command.Parameters.AddWithValue ("@origName", episode.OrigName);
            command.Parameters.AddWithValue ("@seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue ("@addDate", episode.AddTime);
            command.Parameters.AddWithValue ("@airDate", episode.AirDate);
            command.Parameters.AddWithValue ("@size", episode.Size);
            command.Parameters.AddWithValue ("@runtime", episode.Runtime);
            command.Parameters.AddWithValue ("@bitrate", episode.Bitrate);
            command.Parameters.AddWithValue ("@width", episode.Width);
            command.Parameters.AddWithValue ("@height", episode.Height);
            command.Parameters.AddWithValue ("@aspectRatio", episode.AspectRatio);
            command.Parameters.AddWithValue ("@hddID", episode.Hdd.ID);
            command.Parameters.AddWithValue ("@summary", episode.Summary);
            command.Parameters.AddWithValue ("@viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", episode.TrailerLink);
            command.Parameters.AddWithValue ("@version", episode.Version);

            episode.ID = ExecuteCommandAndGetID (command, "episodeID", "SerieEpisode");

            InsertSerieEpisodeAudios (episode);
            InsertSerieEpisodeSubtitles (episode);
        }
        public static void UpdateSerieEpisode (SerieEpisode episode) {
            episode.Version++;
            stringCommand = "UPDATE SerieEpisode SET name = @name, origName = @origName, seasonID = @seasonID, addDate = @addDate, airDate = @airDate, size = @size, " +
                "runtime = @runtime, bitrate = @bitrate, width = @width, height = @height, aspectRatio = @aspectRatio, " +
                "hddID = @hddID, summary = @summary, viewed = @viewed, version = @version WHERE episodeID = @episodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", episode.Name);
            command.Parameters.AddWithValue ("@origName", episode.OrigName);
            command.Parameters.AddWithValue ("@seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue ("@addDate", episode.AddTime);
            command.Parameters.AddWithValue ("@airDate", episode.AirDate);
            command.Parameters.AddWithValue ("@size", episode.Size);
            command.Parameters.AddWithValue ("@runtime", episode.Runtime);
            command.Parameters.AddWithValue ("@bitrate", episode.Bitrate);
            command.Parameters.AddWithValue ("@width", episode.Width);
            command.Parameters.AddWithValue ("@height", episode.Height);
            command.Parameters.AddWithValue ("@aspectRatio", episode.AspectRatio);
            command.Parameters.AddWithValue ("@hddID", episode.Hdd.ID);
            command.Parameters.AddWithValue ("@summary", episode.Summary);
            command.Parameters.AddWithValue ("@viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);
            command.Parameters.AddWithValue ("@version", episode.Version);

            ExecuteCommand (command);
            DeleteSerieEpisodeAudios (episode);
            DeleteSerieEpisodeSubtitles (episode);
            InsertSerieEpisodeAudios (episode);
            InsertSerieEpisodeSubtitles (episode);
        }
        public static void UpdateSerieEpisodeChangeOnlyIsViewed (SerieEpisode episode) {
            stringCommand = "UPDATE SerieEpisode SET viewed = @viewed WHERE episodeID = @episodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.Clear ();
            command.Parameters.AddWithValue ("@viewed", episode.IsViewed);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);

            ExecuteCommand (command);
        }
        public static void DeleteSerieEpisode (SerieEpisode episode) {
            stringCommand = "DELETE FROM SerieEpisode WHERE episodeID = @episodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);
            ExecuteCommand (command);
        }

        static void InsertSerieEpisodeAudios (SerieEpisode episode) {
            foreach (Audio tempAudio in episode.AudioList) {
                stringCommand = "INSERT INTO [SerieEpisode-Audio] (episodeID, audioID) VALUES (@episodeID, @audioID)";
                command = new SqlCeCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("@episodeID", episode.ID);
                command.Parameters.AddWithValue ("@audioID", tempAudio.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteSerieEpisodeAudios (SerieEpisode episode) {
            stringCommand = "DELETE FROM [SerieEpisode-Audio] WHERE episodeID = @episodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);

            ExecuteCommand (command);
        }
        static void InsertSerieEpisodeSubtitles (SerieEpisode episode) {
            foreach (Language tempLang in episode.SubtitleLanguageList) {
                stringCommand = "INSERT INTO [SerieEpisode-SubtitleLanguage] (episodeID, subtitleID) VALUES (@episodeID, @subtitleID)";
                command = new SqlCeCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("@episodeID", episode.ID);
                command.Parameters.AddWithValue ("@subtitleID", tempLang.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteSerieEpisodeSubtitles (SerieEpisode episode) {
            stringCommand = "DELETE FROM [SerieEpisode-SubtitleLanguage] WHERE episodeID = @episodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@episodeID", episode.ID);

            ExecuteCommand (command);
        }

        static void InsertSerieCast (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Serie-Cast] (actorID ,serieID, actorNum) VALUES (@actorID, @serieID, @actorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person actor in serie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@actorID", actor.ID);
                command.Parameters.AddWithValue ("@serieID", serie.ID);
                command.Parameters.AddWithValue ("@actorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteSerieCast (Serie serie) {
            stringCommand = "DELETE FROM [Serie-Cast] WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            ExecuteCommand (command);
        }
        static void InsertSerieDirectors (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Serie-Director] (serieID ,directorID, directorNum) VALUES (@serieID, @directorID, @directorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person director in serie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@serieID", serie.ID);
                command.Parameters.AddWithValue ("@directorID", director.ID);
                command.Parameters.AddWithValue ("@directorNum", count++);
                ExecuteCommand (command);
            }
        }
        static void DeleteSerieDirectors (Serie serie) {
            stringCommand = "DELETE FROM [Serie-Director] WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            ExecuteCommand (command);
        }
        static void InsertSerieGenres (Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [Serie-Genre] (serieID ,genreID, genreNum) VALUES (@serieID, @genreID, @genreNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Genre tempGenre in serie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@serieID", serie.ID);
                command.Parameters.AddWithValue ("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("@genreNum", count++);
                ExecuteCommand (command);
            }

        }
        static void DeleteSerieGenres (Serie serie) {
            stringCommand = "DELETE FROM [Serie-Genre] WHERE serieID = @serieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@serieID", serie.ID);
            ExecuteCommand (command);
        }


        //Wish
        public static void InsertWishSerie (WishSerie serie) {
            stringCommand = "INSERT INTO WishSerie (regularSerieID, name, orgName, imdbRating, trailerLink, internetLink, summary ) VALUES (@regularSerieID, @name , @orgName, @imdbRating ,@trailerLink ,@internetLink ,@summary)";
            command = new SqlCeCommand (stringCommand, connection);
            if (serie.RegularSerieID == 0)
                command.Parameters.AddWithValue ("@regularSerieID", DBNull.Value);
            else
                command.Parameters.AddWithValue ("@regularSerieID", serie.RegularSerieID);            
            command.Parameters.AddWithValue ("@name", serie.Name);
            command.Parameters.AddWithValue ("@imdbRating", serie.Rating);
            command.Parameters.AddWithValue ("@orgName", serie.OrigName);
            command.Parameters.AddWithValue ("@trailerLink", serie.TrailerLink);
            command.Parameters.AddWithValue ("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("@summary", serie.Summary);

            serie.ID = ExecuteCommandAndGetID (command, "wishSerieID", "WishSerie");

            InsertWishSerieCast (serie);
            InsertWishSerieDirectors (serie);
            InsertWishSerieGenres (serie);

            foreach (WishSerieSeason tempSeason in serie.WishSeasons) {
                InsertWishSerieSeason (tempSeason);
            }            
        }
        public static void UpdateWishSerie (WishSerie serie) {
            stringCommand = "UPDATE WishSerie SET regularSerieID = @regularSerieID, name = @name ,orgName = @orgName, imdbRating = @imdbRating ,trailerLink = @trailerLink ,internetLink = @internetLink ,summary = @summary WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            if (serie.RegularSerieID == 0)
                command.Parameters.AddWithValue ("@regularSerieID", DBNull.Value);
            else
                command.Parameters.AddWithValue ("@regularSerieID", serie.RegularSerieID); 
            command.Parameters.AddWithValue ("@name", serie.Name);
            command.Parameters.AddWithValue ("@imdbRating", serie.Rating);
            command.Parameters.AddWithValue ("@orgName", serie.OrigName);
            command.Parameters.AddWithValue ("@trailerLink", serie.TrailerLink);
            command.Parameters.AddWithValue ("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("@summary", serie.Summary);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            ExecuteCommand (command);

            DeleteWishSerieCast (serie);
            InsertWishSerieCast (serie);

            DeleteWishSerieDirectors (serie);
            InsertWishSerieDirectors (serie);

            DeleteWishSerieGenres (serie);
            InsertWishSerieGenres (serie);


            //osvjezi i listu sezona

            List<int> seasonsIdList = new List<int> (); //lista svih ID-jeva sezona koje postoje u seriji
            foreach (WishSerieSeason tempSeason in serie.WishSeasons) {
                if (tempSeason.ID == null) //ako je ID null znaci da je sezona nova i da ju treba unijet
                    InsertWishSerieSeason (tempSeason);
                else
                    UpdateWishSerieSeason (tempSeason);
                seasonsIdList.Add (tempSeason.ID);
            }

            //dohvati sve sezone koje pripadaju seriji
            bool wasClosed = true;
            stringCommand = "SELECT wishSerieSeasonID FROM WishSerieSeason WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedSeasonIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji po bazi podataka
            SqlCeDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedSeasonIDList.Add ((int) reader["wishSerieSeasonID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene sezone iz baze, ako dohvacena serija nije u listi sezona obriši ju iz baze
            foreach (int seasonID in fetchedSeasonIDList) {
                if (seasonsIdList.Contains (seasonID) == false) {
                    stringCommand = "DELETE FROM WishSerieSeason WHERE wishSerieSeasonID = @wishSerieSeasonID";
                    command = new SqlCeCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("@wishSerieSeasonID", seasonID);
                    command.ExecuteNonQuery ();
                }
            }

            if (wasClosed)
                connection.Close ();
        }
        public static void DeleteWishSerie (WishSerie serie) {
            stringCommand = "DELETE FROM WishSerie WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            ExecuteCommand (command);
        }

        public static void InsertWishSerieSeason (WishSerieSeason season) {
            stringCommand = "INSERT INTO WishSerieSeason (regularSeasonID, wishSerieID ,name ,trailerLink ,internetLink) VALUES " +
                "(@regularSeasonID, @wishSerieID ,@name ,@trailerLink, @internetLink) ";
            command = new SqlCeCommand (stringCommand, connection);
            if (season.RegularSeasonID == 0)
                command.Parameters.AddWithValue ("@regularSeasonID", DBNull.Value);
            else
                command.Parameters.AddWithValue ("@regularSeasonID", season.RegularSeasonID);             
            command.Parameters.AddWithValue ("@wishSerieID", season.ParentWishSerie.ID);
            command.Parameters.AddWithValue ("@name", season.Name);
            command.Parameters.AddWithValue ("@trailerLink", season.TrailerLink);
            command.Parameters.AddWithValue ("@internetLink", season.InternetLink);
            season.ID = ExecuteCommandAndGetID (command, "wishSerieSeasonID", "WishSerieSeason");

            foreach (WishSerieEpisode tempEpisode in season.WishEpisodes)
                InsertWishSerieEpisode (tempEpisode);
        }
        public static void UpdateWishSerieSeason (WishSerieSeason season) {
            stringCommand = "UPDATE WishSerieSeason SET regularSeasonID = @regularSeasonID, wishSerieID = @wishSerieID ," +
                "name = @name ,trailerLink = @trailerLink ,internetLink = @internetLink WHERE wishSerieSeasonID = @wishSerieSeasonID";
            command = new SqlCeCommand (stringCommand, connection);
            if (season.RegularSeasonID == 0)
                command.Parameters.AddWithValue ("@regularSeasonID", DBNull.Value);
            else
                command.Parameters.AddWithValue ("@regularSeasonID", season.RegularSeasonID);
            command.Parameters.AddWithValue ("@wishSerieID", season.ParentWishSerie.ID);
            command.Parameters.AddWithValue ("@name", season.Name);
            command.Parameters.AddWithValue ("@trailerLink", season.TrailerLink);
            command.Parameters.AddWithValue ("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("@wishSerieSeasonID", season.ID);
            ExecuteCommand (command);



            //osvjezi i listu epizoda

            List<int> episodesIDList = new List<int> ();
            foreach (WishSerieEpisode tempEpisode in season.WishEpisodes) {
                if (tempEpisode.ID == 0)  //ako je ID null znaci da epizoda nije dodana u bazu
                    InsertWishSerieEpisode (tempEpisode);
                else
                    UpdateWishSerieEpisode (tempEpisode);
                episodesIDList.Add (tempEpisode.ID);
            }

            //dohvati sve epizode koje pripadaju seriji
            bool wasClosed = true;
            stringCommand = "SELECT wishSerieEpisodeID FROM WishSerieEpisode WHERE wishSerieSeasonID = @wishSerieSeasonID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieSeasonID", season.ID);
            try {
                connection.Open ();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedEpisodeIDList = new List<int> (); //lista svih ID-jeva sezona koje pripadaju seriji u bazi podataka
            SqlCeDataReader reader = command.ExecuteReader ();
            while (reader.Read ()) {
                fetchedEpisodeIDList.Add ((int) reader["wishSerieEpisodeID"]);
            }
            reader.Close ();

            //prodji kroz sve dohvacene epizode, ako dohvacena epizoda nije u listi epizoda sezone, obriši ju iz baze
            foreach (int episodeID in fetchedEpisodeIDList) {
                if (episodesIDList.Contains (episodeID) == false) {
                    stringCommand = "DELETE FROM WishSerieEpisode WHERE wishSerieSeasonID = @wishSerieSeasonID";
                    command = new SqlCeCommand (stringCommand, connection);
                    command.Parameters.AddWithValue ("@wishSerieSeasonID", episodeID);
                    command.ExecuteNonQuery ();
                }
            }

            if (wasClosed)
                connection.Close ();
        }
        public static void DeleteWishSerieSeason (WishSerieSeason season) {
            stringCommand = "DELETE FROM WishSerieSeason WHERE wishSerieSeasonID = @wishSerieSeasonID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieSeasonID", season.ID);
            ExecuteCommand (command);
        }

        public static void InsertWishSerieEpisode (WishSerieEpisode episode) {
            stringCommand = "INSERT INTO WishSerieEpisode ( name , orgName, wishSerieSeasonID , airDate , summary, internetLink, trailerLink )" +
                "VALUES (@name ,@orgName, @wishSerieSeasonID ,@airDate ,@summary, @internetLink, @trailerLink)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieSeasonID", episode.ParentWishSeason.ID);
            command.Parameters.AddWithValue ("@name", episode.Name);
            command.Parameters.AddWithValue ("@orgName", episode.OrigName);
            command.Parameters.AddWithValue ("@airDate", episode.AirDate);
            command.Parameters.AddWithValue ("@summary", episode.Summary);
            command.Parameters.AddWithValue ("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", episode.TrailerLink);
            episode.ID = ExecuteCommandAndGetID (command, "wishSerieEpisodeID", "WishSerieEpisode");
        }
        public static void UpdateWishSerieEpisode (WishSerieEpisode episode) {
            stringCommand = "UPDATE WishSerieEpisode SET name = @name , orgName = @orgName, wishSerieSeasonID = @wishSerieSeasonID , " +
                "airDate = @airDate , summary = @summary, internetLink = @internetLink, trailerLink = @trailerLink WHERE wishSerieEpisodeID = @wishSerieEpisodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", episode.Name);
            command.Parameters.AddWithValue ("@orgName", episode.OrigName);            
            command.Parameters.AddWithValue ("@wishSerieSeasonID", episode.ParentWishSeason.ID);            
            command.Parameters.AddWithValue ("@airDate", episode.AirDate);
            command.Parameters.AddWithValue ("@summary", episode.Summary);
            command.Parameters.AddWithValue ("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue ("@trailerLink", episode.TrailerLink);
            command.Parameters.AddWithValue ("@wishSerieEpisodeID", episode.ID);
            ExecuteCommand (command);
        }
        public static void DeleteWishSerieEpisode (WishSerieEpisode episode) {
            stringCommand = "DELETE FROM WishSerieEpisode WHERE wishSerieEpisodeID = @wishSerieEpisodeID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieEpisodeID", episode.ID);
            ExecuteCommand (command);
        }

        static void InsertWishSerieCast (WishSerie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishSerie-Cast] (actorID, wishSerieID, actorNum) VALUES (@actorID, @wishSerieID, @actorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person actor in serie.Actors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@actorID", actor.ID);
                command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
                command.Parameters.AddWithValue ("@actorNum", count++);
                ExecuteCommand (command);
            }            
        }
        static void DeleteWishSerieCast (WishSerie serie) {
            stringCommand = "DELETE FROM [WishSerie-Cast] WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            ExecuteCommand (command);
        }
        static void InsertWishSerieDirectors (WishSerie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishSerie-Director] (directorID, wishSerieID, directorNum) VALUES (@directorID, @wishSerieID, @directorNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person director in serie.Directors) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@directorID", director.ID);
                command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
                command.Parameters.AddWithValue ("@directorNum", count++);
                ExecuteCommand (command);
            }        
        }
        static void DeleteWishSerieDirectors (WishSerie serie) {
            stringCommand = "DELETE FROM [WishSerie-Director] WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            ExecuteCommand (command);
        }
        static void InsertWishSerieGenres (WishSerie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [WishSerie-Genre] (genreID, wishSerieID, genreNum) VALUES (@genreID, @wishSerieID, @genreNum)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Genre  tempGenre in serie.Genres) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
                command.Parameters.AddWithValue ("@genreNum", count++);
                ExecuteCommand (command);
            }   
        }
        static void DeleteWishSerieGenres (WishSerie serie) {
            stringCommand = "DELETE FROM [WishSerie-Genre] WHERE wishSerieID = @wishSerieID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishSerieID", serie.ID);
            ExecuteCommand (command);
        }


        #endregion

        #region HOME VIDEO

        //Regular
        public static void InsertHomeVideo (HomeVideo homevideo) {
            stringCommand = "INSERT INTO HomeVideo (name ,categoryID ,date ,location ,cameraID ,size ,runtime ,height ,width ,bitrate ,aspectRatio ,hddID ,comment) " +
            "VALUES (@name ,@categoryID ,@date ,@location ,@cameraID ,@size ,@runtime ,@height ,@width ,@bitrate ,@aspectRatio ,@hddID ,@comment)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", homevideo.Name);
            command.Parameters.AddWithValue ("@categoryID", homevideo.VideoCategory.ID);
            command.Parameters.AddWithValue ("@date", homevideo.FilmDate);
            command.Parameters.AddWithValue ("@location", homevideo.Location);
            command.Parameters.AddWithValue ("@cameraID", homevideo.FilmingCamera.ID);
            command.Parameters.AddWithValue ("@size", homevideo.Size);
            command.Parameters.AddWithValue ("@runtime", homevideo.Runtime);
            command.Parameters.AddWithValue ("@height", homevideo.Height);
            command.Parameters.AddWithValue ("@width", homevideo.Width);
            command.Parameters.AddWithValue ("@bitrate", homevideo.Bitrate);
            command.Parameters.AddWithValue ("@aspectRatio", homevideo.AspectRatio);
            command.Parameters.AddWithValue ("@hddID", homevideo.Hdd.ID);
            command.Parameters.AddWithValue ("@comment", homevideo.Summary);

            homevideo.ID = ExecuteCommandAndGetID (command, "homeVideoID", "HomeVideo");

            InsertHomeVideoCamermans (homevideo);
            InsertHomeVideoPersons (homevideo);
            InsertHomeVideoAudios (homevideo);
        }
        public static void UpdateHomeVideo (HomeVideo homevideo) {
            stringCommand = "UPDATE HomeVideo SET name = @name ,categoryID = @categoryID ,date = @date ,location = @location ," +
                "cameraID = @cameraID ,size = @size ,runtime = @runtime ,height = @height ,width = @width ,bitrate = @bitrate ," +
                "aspectRatio = @aspectRatio ,hddID = @hddID ,comment = @comment WHERE homeVideoID = @homeVideoID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", homevideo.Name);
            command.Parameters.AddWithValue ("@categoryID", homevideo.VideoCategory.ID);
            command.Parameters.AddWithValue ("@date", homevideo.FilmDate);
            command.Parameters.AddWithValue ("@location", homevideo.Location);
            command.Parameters.AddWithValue ("@cameraID", homevideo.FilmingCamera.ID);
            command.Parameters.AddWithValue ("@size", homevideo.Size);
            command.Parameters.AddWithValue ("@runtime", homevideo.Runtime);
            command.Parameters.AddWithValue ("@height", homevideo.Height);
            command.Parameters.AddWithValue ("@width", homevideo.Width);
            command.Parameters.AddWithValue ("@bitrate", homevideo.Bitrate);
            command.Parameters.AddWithValue ("@aspectRatio", homevideo.AspectRatio);
            command.Parameters.AddWithValue ("@hddID", homevideo.Hdd.ID);
            command.Parameters.AddWithValue ("@comment", homevideo.Summary);
            command.Parameters.AddWithValue ("@homeVideoID", homevideo.ID);
            ExecuteCommand (command);

            DeleteHomeVideoCamermans (homevideo);
            DeleteHomeVideoPersons (homevideo);
            DeleteHomeVideoAudios (homevideo);

            InsertHomeVideoCamermans (homevideo);
            InsertHomeVideoPersons (homevideo);
            InsertHomeVideoAudios (homevideo);
        }
        public static void DeleteHomeVideo (HomeVideo homevideo) {
            stringCommand = "DELETE FROM HomeVideo WHERE homeVideoID = @homeVideoID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@homeVideoID", homevideo.ID);
            ExecuteCommand (command);
        }

        static void InsertHomeVideoCamermans (HomeVideo homevideo) {
            stringCommand = "INSERT INTO [HomeVideo-Camerman] (homeVideoID, camermanID) VALUES (@homeVideoID, @camermanID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person camerman in homevideo.Camermans) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@homeVideoID", homevideo.ID);
                command.Parameters.AddWithValue ("@camermanID", camerman.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteHomeVideoCamermans (HomeVideo homevideo) {
            stringCommand = "DELETE FROM [HomeVideo-Camerman] WHERE homeVideoID = @ID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", homevideo.ID);
            ExecuteCommand (command);
        }
        static void InsertHomeVideoPersons (HomeVideo homevideo) {
            stringCommand = "INSERT INTO [HomeVideo-Person] (personID, homeVideoID) VALUES (@personID, @homeVideoID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person person in homevideo.PersonsInVideo) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@homeVideoID", homevideo.ID);
                command.Parameters.AddWithValue ("@personID", person.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteHomeVideoPersons (HomeVideo homeVideo) {
            stringCommand = "DELETE FROM [HomeVideo-Person] WHERE homeVideoID = @ID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", homeVideo.ID);
            ExecuteCommand (command);
        }
        static void DeleteHomeVideoAudios (HomeVideo homevideo) {
            stringCommand = "DELETE FROM [HomeVideo-Audio] WHERE homeVideoID = @ID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", homevideo.ID);
            ExecuteCommand (command);
        }
        static void InsertHomeVideoAudios (HomeVideo homevideo) {
            stringCommand = "INSERT INTO [HomeVideo-Audio] (audioID, homeVideoID) VALUES (@audioID, @homeVideoID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Audio tempAudio in homevideo.AudioList) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@audioID", tempAudio.ID);
                command.Parameters.AddWithValue ("@homeVideoID", homevideo.ID);
                ExecuteCommand (command);
            }
        }

        //Wish
        public static void InsertWishHomeVideo (WishHomeVideo wish) {
            stringCommand = "INSERT INTO WishHomeVideo (name ,categoryID ,location ,cameraID ,expectedDate, comment) VALUES (@name ,@categoryID ,@location ,@cameraID ,@expectedDate, @comment)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", wish.Name);
            command.Parameters.AddWithValue ("@categoryID", wish.VideoCategory.ID);
            command.Parameters.AddWithValue ("@location", wish.Location);
            command.Parameters.AddWithValue ("@cameraID", wish.FilmingCamera.ID);
            command.Parameters.AddWithValue ("@expectedDate", wish.FilmDate);
            command.Parameters.AddWithValue ("@comment", wish.Comment);

            wish.ID = ExecuteCommandAndGetID (command, "wishHomeVideoID", "WishHomeVideo");

            InsertWishHVPersons (wish);
            InsertWishHVCamermans (wish);
        }
        public static void UpdateWishHomeVideo (WishHomeVideo wish) {
            stringCommand = "UPDATE WishHomeVideo SET name = @name ,categoryID = @categoryID ,location = @location ,cameraID = @cameraID ,expectedDate = @expectedDate, comment = @comment WHERE wishHomeVideoID = @wishID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", wish.Name);
            command.Parameters.AddWithValue ("@categoryID", wish.VideoCategory.ID);
            command.Parameters.AddWithValue ("@location", wish.Location);
            command.Parameters.AddWithValue ("@cameraID", wish.FilmingCamera.ID);
            command.Parameters.AddWithValue ("@expectedDate", wish.FilmDate);
            command.Parameters.AddWithValue ("@comment", wish.Comment);
            command.Parameters.AddWithValue ("@wishID", wish.ID);
            ExecuteCommand (command);

            DeleteWishHVCamermans (wish);
            DeleteWishHVPersons (wish);

            InsertWishHVCamermans (wish);
            InsertWishHVPersons (wish);
        }
        public static void DeleteWishHomeVideo (WishHomeVideo wish) {
            stringCommand = "DELETE FROM WishHomeVideo WHERE wishHomeVideoID = @wishID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishID", wish.ID);
            ExecuteCommand (command);
        }

        static void InsertWishHVPersons (WishHomeVideo wish) {
            stringCommand = "INSERT INTO [WishHV-Person] (personID, wishHVID) VALUES (@personID, @wishID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person tempPerson in wish.PersonsInVideo) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@personID", tempPerson.ID);
                command.Parameters.AddWithValue ("@wishID", wish.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteWishHVPersons (WishHomeVideo wish) {
            stringCommand = "DELETE FROM [WishHV-Person] WHERE wishHVID = @wishID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishID", wish.ID);
            ExecuteCommand (command);
        }
        static void InsertWishHVCamermans (WishHomeVideo wish) {
            stringCommand = "INSERT INTO [WishHV-Camerman] (camermanID, wishHVID) VALUES (@camermanID, @wishID)";
            command = new SqlCeCommand (stringCommand, connection);
            foreach (Person tempPerson in wish.Camermans) {
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("@camermanID", tempPerson.ID);
                command.Parameters.AddWithValue ("@wishID", wish.ID);
                ExecuteCommand (command);
            }
        }
        static void DeleteWishHVCamermans (WishHomeVideo wish) {
            stringCommand = "DELETE FROM [WishHV-Camerman] WHERE wishHVID = @wishID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@wishID", wish.ID);
            ExecuteCommand (command);
        }
        
        #endregion

        #region MISC

        #region PERSON

        public static void InsertPersonForSelection (Person person) {
            stringCommand = "INSERT INTO Person (personID, typeID, personName, dateOfBirth) VALUES (@personID, @typeID, @name, @dateOfBirth)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@personID", person.ID);
            command.Parameters.AddWithValue ("@name", person.Name);
            command.Parameters.AddWithValue ("@typeID", person.Type.ID);
            command.Parameters.AddWithValue ("@dateOfBirth", DateTime.Now);

            ExecuteCommand(command);
        }
        public static void InsertPerson (Person person) {
            stringCommand = "INSERT INTO Person (typeID, personName, dateOfBirth) VALUES (@typeID, @name, @dateOfBirth)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", person.Name);
            command.Parameters.AddWithValue ("@typeID", person.Type.ID);
            command.Parameters.AddWithValue ("@dateOfBirth", person.DateOfBirth);

            person.ID = ExecuteCommandAndGetID (command, "personID", "Person");
        }
        public static void DeletePerson (Person person) {
            stringCommand = "DELETE FROM Person WHERE personID = @personID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@personID", person.ID);
            ExecuteCommand (command);
        }
        public static void UpdatePerson (Person person) {
            stringCommand = "UPDATE Person SET typeID = @typeID ,personName = @personName ,dateOfBirth = @dateOfBirth WHERE personID = @personID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", person.Name);
            command.Parameters.AddWithValue ("@typeID", person.Type.ID);
            command.Parameters.AddWithValue ("@dateOfBirth", person.DateOfBirth);
            command.Parameters.AddWithValue ("@personID", person.ID);
        }

        #endregion

        #region LANGUAGE

        public static void InsertLanguageForSelection (Language language) {
            stringCommand = "INSERT INTO Language (languageID, name) VALUES (@languageID, @name)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@languageID", language.ID);
            command.Parameters.AddWithValue ("@name", language.Name);

            ExecuteCommand (command);
        }
        public static void InsertLanguage (Language language) {
            stringCommand = "INSERT INTO Language (name) VALUES (@name)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", language.Name);

            language.ID = ExecuteCommandAndGetID (command, "languageID", "Language");
        }
        public static void DeleteLanguage (Language language) {
            stringCommand = "DELETE FROM Language WHERE languageID = @languageID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@languageID", language.ID);
            ExecuteCommand (command);
        }
        public static void UpdateLanguage (Language language) {
            throw new NotImplementedException ();
        }

        #endregion

        #region HDD

        public static void InsertHDDForSelection (HDD hdd) {
            stringCommand = "INSERT INTO HDD (hddID, name ,purchaseDate ,warrantyLength ,capacity ,freeSpace) VALUES (@hddID, @name, @purchaseDate, @warrantyLength, @capacity, @freeSpace)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@hddID", hdd.ID);
            command.Parameters.AddWithValue ("@name", hdd.Name);
            command.Parameters.AddWithValue ("@purchaseDate", DateTime.Now);
            command.Parameters.AddWithValue ("@warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("@capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("@freeSpace", hdd.FreeSpace);

            hdd.ID = ExecuteCommandAndGetID (command, "hddID", "HDD");
        }
        public static void InsertHDD (HDD hdd) {
            stringCommand = "INSERT INTO HDD (name ,purchaseDate ,warrantyLength ,capacity ,freeSpace) VALUES (@name, @purchaseDate, @warrantyLength, @capacity, @freeSpace)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", hdd.Name);
            command.Parameters.AddWithValue ("@purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue ("@warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("@capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("@freeSpace", hdd.FreeSpace);

            hdd.ID = ExecuteCommandAndGetID (command, "hddID", "HDD");
        }
        public static void DeleteHDD (HDD hdd) {
            stringCommand = "DELETE FROM HDD WHERE hddID = @hddID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@hddID", hdd.ID);
            ExecuteCommand (command);
        }
        public static void UpdateHDD (HDD hdd) {
            stringCommand = "UPDATE HDD SET name = @name ,purchaseDate = @purchaseDate ,warrantyLength = @warrantyLength ,capacity = @capacity ,freeSpace = @freeSpace WHERE hddID = @hddID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", hdd.Name);
            command.Parameters.AddWithValue ("@purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue ("@warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("@capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("@freeSpace", hdd.FreeSpace);
            command.Parameters.AddWithValue ("@hddID", hdd.ID);
            ExecuteCommand (command);
        }

        #endregion

        #region AUDIO

        public static void InsertAudioForSelection (Audio audio) {
            stringCommand = "INSERT INTO Audio (ID, channels, languageID, format) VALUES (@ID, @channels, @languageID, @format)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", audio.ID);
            command.Parameters.AddWithValue ("@channels", audio.Channels);
            command.Parameters.AddWithValue ("@languageID", audio.Language.ID);
            command.Parameters.AddWithValue ("@format", audio.Format);

            ExecuteCommand (command);
        }
        public static void InsertAudio (Audio audio) {
            stringCommand = "INSERT INTO Audio (channels, languageID, format) VALUES (@channels, @languageID, @format)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@channels", audio.Channels);
            command.Parameters.AddWithValue ("@languageID", audio.Language.ID);
            command.Parameters.AddWithValue ("@format", audio.Format);

            audio.ID = ExecuteCommandAndGetID (command, "ID", "Audio");
        }
        public static void DeleteAudio (Audio audio) {
            stringCommand = "DELETE FROM Audio WHERE ID = @id";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", audio.ID);

            ExecuteCommand (command);
        }
        public static void UpdateAudio (Audio audio) {
            throw new NotImplementedException ();
        }

        #endregion

        #region CAMERA

        public static void InsertCamera (Camera camera) {
            stringCommand = "INSERT INTO Camera (cameraName, purchaseDate, warrantyLength) VALUES (@cameraName, @purchaseDate, @warrantyLength)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@cameraName", camera.Name);
            command.Parameters.AddWithValue ("@purchaseDate", camera.PurchaseDate);
            command.Parameters.AddWithValue ("@warrantyLength", camera.WarrantyLengt);
            camera.ID = ExecuteCommandAndGetID (command, "cameraID", "Camera");
        }
        public static void UpdateCamera (Camera camera) {
            stringCommand = "UPDATE Camera SET cameraName = @cameraName, purchaseDate = @purchaseDate, warrantyLength = @warrantyLength WHERE cameraID = @cameraID";
            command = new SqlCeCommand (stringCommand, connection); 
            command.Parameters.AddWithValue ("@cameraName", camera.Name);
            command.Parameters.AddWithValue ("@purchaseDate", camera.PurchaseDate);
            command.Parameters.AddWithValue ("@warrantyLength", camera.WarrantyLengt);
            command.Parameters.AddWithValue ("@cameraID", camera.ID);
            ExecuteCommand (command);
        }
        public static void DeleteCamera (Camera camera) {
            stringCommand = "DELETE FROM Camera WHERE cameraID = @cameraID";
            command = new SqlCeCommand (stringCommand, connection); 
            command.Parameters.AddWithValue ("@cameraID", camera.ID);
            ExecuteCommand (command);
        }

        #endregion

        #region SETTINGS

        public static void InsertSettings (Settings settings) {
            throw new NotImplementedException ();
        }
        public static void UpdateSettings (Settings settings) {
            stringCommand = "UPDATE Settings SET trailerSearchProvider = @trailerSearchProvider ,posterSearchProvider = @posterSearchProvider ," +
                "useOnlyOriginalName = @useOnlyOriginalName ,imageEditorPath = @imageEditorPath ,imageQualityLevel = @imageQualityLevel "+
                "WHERE settingsID = @settingsID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@settingsID", settings.ID);
            if (settings.ImageEditorPath == null)
                command.Parameters.AddWithValue ("imageEditorPath", DBNull.Value);
            else
                command.Parameters.AddWithValue ("imageEditorPath", settings.ImageEditorPath);
            command.Parameters.AddWithValue ("@trailerSearchProvider", settings.TrailerProvider);
            command.Parameters.AddWithValue ("@posterSearchProvider", settings.PosterProvider);
            command.Parameters.AddWithValue ("@useOnlyOriginalName", settings.UseOnlyOriginalName);
            command.Parameters.AddWithValue ("@imageQualityLevel", settings.ImageQualityLevel);

            ExecuteCommand (command);
        }
        public static void DeleteSettings (Settings settings) {
            throw new NotImplementedException ();
        }

        #endregion

        #region Category

        public static void InsertCategory (Category category) {
            stringCommand = "INSERT INTO HVCategory (name) VALUES (@name)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", category.Name);
            category.ID = ExecuteCommandAndGetID (command, "homeVideoCatID", "HVCategory");
        }
        public static void UpdateCategory (Category category) {
            stringCommand = "UPDATE HVCategory SET name = @name WHERE homeVideoCatID = @ID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", category.Name);
            command.Parameters.AddWithValue ("@ID", category.ID);
            ExecuteCommand (command);
        }
        public static void DeleteCategory (Category category) {
            stringCommand = " DELETE FROM HVCategory WHERE homeVideoCatID = @ID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@ID", category.ID);
            ExecuteCommand (command);
        }

        #endregion

        #region Selection

        public static void InsertSelection (Selection sel, ref int progress) {
            stringCommand = "INSERT INTO Selection (name, dateLastChange, hddSize) VALUES (@name, @dateLastChange, @hddSize)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", sel.Name);
            command.Parameters.AddWithValue ("@dateLastChange", sel.DateOfLastChange);
            command.Parameters.AddWithValue ("@hddSize", sel.Size);
            sel.ID = ExecuteCommandAndGetID (command, "selectionID", "Selection");

            InsertSelectedMovies (sel, ref progress);
            InsertSelectedEpisodes (sel, ref progress);
        }
        public static void UpdateSelection (Selection sel, ref int progress) {
            stringCommand = "UPDATE Selection SET name = @name, datelastChange = @dateLastChange, hddSize = @hddSize WHERE selectionID = @selectionID)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@name", sel.Name);
            command.Parameters.AddWithValue ("@dateLastChange", sel.DateOfLastChange);
            command.Parameters.AddWithValue ("@selectionID", sel.ID);
            command.Parameters.AddWithValue ("@hddSize", sel.Size);
            ExecuteCommand (command);

            DeleteSelectedEpisodes (sel);
            DeleteSelectedMovies (sel);
            InsertSelectedMovies (sel, ref progress);
            InsertSelectedEpisodes (sel, ref progress);
        }
        public static void DeleteSelection (Selection sel) {
            stringCommand = "DELETE FROM Selection WHERE selectionID = @selectionID)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@selectionID", sel.ID);
            ExecuteCommand (command);

            DeleteSelectedEpisodes (sel);
            DeleteSelectedMovies (sel);
        }

        public static void InsertSelectedMovies (Selection sel, ref int progress) {
            foreach (Movie tempMovie in sel.SelectedMovies) {
                stringCommand = "INSERT INTO [Selection-Movie] (selectionID, movieID) VALUES (@selectionID, @movieID)";
                command = new SqlCeCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("@selectionID", sel.ID);
                command.Parameters.AddWithValue ("@movieID", tempMovie.ID);
                ExecuteCommand (command);
                progress++;
            }            
        }
        public static void InsertSelectedEpisodes (Selection sel, ref int progress) {
            //int counter = 0;
            foreach (SerieEpisode tempEpisde in sel.selectedEpisodes) {
                stringCommand = "INSERT INTO [Selection-SerieEpisode] (selectionID, serieEpisodeID) VALUES (@selectionID, @serieEpisodeID)";
                command = new SqlCeCommand (stringCommand, connection);
                command.Parameters.AddWithValue ("@selectionID", sel.ID);
                command.Parameters.AddWithValue ("@serieEpisodeID", tempEpisde.ID);
                ExecuteCommand (command);
                progress++;
                //if (++counter % 200 == 0) ;
                    //MessageBox.Show (counter.ToString () + "/" + sel.selectedEpisodes.Count);
            }
        }
        public static void DeleteSelectedMovies (Selection sel) {
            stringCommand = "DELETE FROM [Selection-Movie] WHERE selectionID = @selectionID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@selectionID", sel.ID);
            ExecuteCommand (command);
        }
        public static void DeleteSelectedEpisodes (Selection sel) {
            stringCommand = "DELETE FROM [Selection-SerieEpisode] WHERE selectionID = @selectionID";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@selectionID", sel.ID);
            ExecuteCommand (command);
        }

        #endregion

        #region Version

        public static void InsertnewVersion (decimal version, string changes) {
            DeleteAllVersions ();
            stringCommand = "INSERT INTO [version] (versionNumber, changes) VALUES (@version, @changes)";
            command = new SqlCeCommand (stringCommand, connection);
            command.Parameters.AddWithValue ("@version", version);
            command.Parameters.AddWithValue ("@changes", changes);
            ExecuteCommand (command);
        }
        public static void DeleteAllVersions () {
            stringCommand = "DELETE FROM [version]";
            command = new SqlCeCommand (stringCommand, connection);
            ExecuteCommand (command);
        }

        #endregion

        #endregion

        #endregion

    }
}
