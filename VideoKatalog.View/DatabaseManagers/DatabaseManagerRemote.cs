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
using Video_katalog;

namespace VideoKatalog.View {
    class DatabaseManagerRemote {
        static SqlConnection connection = new SqlConnection(ComposeConnectionString.ComposeSqlRemote());
        static SqlCommand command;
        static SqlDataReader reader;
        static string stringCommand;

        #region retreive

        #region MOVIE

        public static int FetchMovieCountList() {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();
            int count = 0;

            stringCommand = "SELECT COUNT(*) as count FROM [Movie]";
            command = new SqlCommand(stringCommand, connection);
            reader = command.ExecuteReader();
            while (reader.Read()) {
                count += (int)reader["count"];
            }

            if (wasClosed)
                connection.Close();
            reader.Close();

            return count;
        }
        public static ObservableCollection<Movie> FetchMovieList(ObservableCollection<Genre> genreList,
            ObservableCollection<Person> personList, ObservableCollection<Language> languageList, ObservableCollection<HDD> hddList, ref int counter) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();

            ObservableCollection<Movie> listOfMovies = new ObservableCollection<Movie>();
            string SqlCommandGetMovies = "SELECT * FROM [Movie] ORDER BY customName";
            //string SqlCommandGetCast = "SELECT * FROM [Movie-Cast] ORDER BY actorNum";
            //string SqlCommandGetDirectors = "SELECT * FROM [Movie-Director] ORDER BY directorNum";
            //string SqlCommandGetSubtitleLanguages = "SELECT * FROM [Movie-SubtitleLanguage]";
            //string SqlCommandGetGenres = "SELECT * FROM [Movie-Genre] ORDER BY genreNum";
            //SqlCommand command;
            //SqlCeDataReader reader;

            //**** MOVIES ****
            command = new SqlCommand(SqlCommandGetMovies, connection);
            reader = command.ExecuteReader();
            while (reader.Read()) {
                Movie tempMovie = new Movie();
                tempMovie.ID = (int)reader["movieID"];
                tempMovie.Name = (string)reader["customName"];
                tempMovie.Version = (int)reader["version"];
                listOfMovies.Add(tempMovie);
                counter++;
            }
            reader.Close();
            if (listOfMovies.Count == 0)
                return listOfMovies;

            //**** GENRES ****
            //command = new SqlCommand(SqlCommandGetGenres, connection);
            //reader = command.ExecuteReader();
            //while (reader.Read()) {
            //    int movieID = (int)reader["movieID"];
            //    int genreID = (int)reader["genreID"];
            //    FinderInCollection.FindInMovieCollection(listOfMovies, movieID).Genres.Add(FinderInCollection.FindInGenreCollection(genreList, genreID));
            //    counter++;
            //}
            //reader.Close();

            //**** DIRECTORS ****
            //command = new SqlCommand(SqlCommandGetDirectors, connection);
            //reader = command.ExecuteReader();
            //while (reader.Read()) {
            //    int movieID = (int)reader["movieID"];
            //    int directorID = (int)reader["directorID"];
            //    FinderInCollection.FindInMovieCollection(listOfMovies, movieID).Directors.Add(FinderInCollection.FindInPersonCollection(personList, directorID));
            //    counter++;
            //}
            //reader.Close();

            //**** CAST ****
            //command = new SqlCommand(SqlCommandGetCast, connection);
            //reader = command.ExecuteReader();
            //while (reader.Read()) {
            //    int movieID = (int)reader["movieID"];
            //    int actorID = (int)reader["personID"];
            //    FinderInCollection.FindInMovieCollection(listOfMovies, movieID).Actors.Add(FinderInCollection.FindInPersonCollection(personList, actorID));
            //    counter++;
            //}
            //reader.Close();

            //**** SUBTITLES ****
            //command = new SqlCommand(SqlCommandGetSubtitleLanguages, connection);
            //reader = command.ExecuteReader();
            //while (reader.Read()) {
            //    int movieID = (int)reader["movieID"];
            //    int languageID = (int)reader["subtitleLanguageID"];
            //    FinderInCollection.FindInMovieCollection(listOfMovies, movieID).SubtitleLanguageList.Add(FinderInCollection.FindInLanguageCollection(languageList, languageID));
            //    counter++;
            //}
            //reader.Close();
            
            if (wasClosed)
                connection.Close();

            return listOfMovies;
        }

        #endregion

        #region Serie

         public static int FetchSerieCount () {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open ();
            int count = 0;

            stringCommand = "SELECT COUNT(*) as count FROM Serie";
            command = new SqlCommand (stringCommand, connection);
            reader = command.ExecuteReader ();            
            while (reader.Read ()) {
                count += (int) reader["count"];
            }
            if (wasClosed)
                connection.Close();
            reader.Close();

            return count;
        }
         public static int FetchSeasonCount() {
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             int count = 0;


             stringCommand = "SELECT COUNT(*) as count FROM SerieSeason";
             command = new SqlCommand(stringCommand, connection);
             reader = command.ExecuteReader();
             while (reader.Read()) {
                 count = (int)reader["count"];
             }

             if (wasClosed)
                 connection.Close();
             reader.Close();

             return count;
         }
         public static int FetchEpisodeCount() {
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             int count = 0;

             stringCommand = "SELECT COUNT(*) as count FROM SerieEpisode";
             command = new SqlCommand(stringCommand, connection);
             reader = command.ExecuteReader();
             while (reader.Read()) {
                 count += (int)reader["count"];
             }
             reader.Close();


             if (wasClosed)
                 connection.Close();
             reader.Close();

             return count;
         }

         public static ObservableCollection<Serie> FetchSerieList(ObservableCollection<Genre> genreList, ObservableCollection<Person> personList, ref int counter) {
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             ObservableCollection<Serie> listOfSeries = new ObservableCollection<Serie>();
             listOfSeries = new ObservableCollection<Serie>();

             string getSeriesCommand = "SELECT * FROM Serie ORDER BY name";
             //string getSerieGenresCommand = "SELECT * FROM [Serie-Genre] ORDER BY genreNum";
             //string getSerieCastCommand = "SELECT * FROM [Serie-Cast] ORDER BY actorNum";
             //string getSerieDirectorsCommand = "SELECT * FROM [Serie-Director] ORDER BY directorNum";
             SqlCommand sqlCMNDGetSeries;

             #region Main

             sqlCMNDGetSeries = new SqlCommand(getSeriesCommand, connection);
             reader = sqlCMNDGetSeries.ExecuteReader();

             while (reader.Read()) {
                 Serie newSerie = new Serie();
                 newSerie.ID = (int)reader["serieID"];
                 newSerie.Name = (string)reader["name"];
                 //newSerie.Rating = (decimal)reader["rating"];
                 //newSerie.OrigName = (string)reader["origName"];
                 //newSerie.InternetLink = (string)reader["internetLink"];
                 //newSerie.TrailerLink = (string)reader["trailerLink"];
                 //newSerie.Summary = (string)reader["summary"];
                 listOfSeries.Add(newSerie);
                 counter++;
             }
             reader.Close();
             if (listOfSeries.Count == 0)
                 return listOfSeries;

             #endregion

             #region Genres

             //sqlCMNDGetSeries = new SqlCommand(getSerieGenresCommand, connection);
             //reader = sqlCMNDGetSeries.ExecuteReader();
             //while (reader.Read()) {
             //    int serieID = (int)reader["serieID"];
             //    int genreID = (int)reader["genreID"];
             //    FinderInCollection.FindInSerieCollection(listOfSeries, serieID).Genres.Add(FinderInCollection.FindInGenreCollection(genreList, genreID));
             //    counter++;
             //}
             //reader.Close();

             #endregion

             #region Cast

             //sqlCMNDGetSeries = new SqlCommand(getSerieCastCommand, connection);
             //reader = sqlCMNDGetSeries.ExecuteReader();
             //while (reader.Read()) {
             //    int serieID = (int)reader["serieID"];
             //    int actorID = (int)reader["actorID"];
             //    FinderInCollection.FindInSerieCollection(listOfSeries, serieID).Actors.Add(FinderInCollection.FindInPersonCollection(personList, actorID));
             //    counter++;
             //}
             //reader.Close();

             #endregion

             #region Directors

             //sqlCMNDGetSeries = new SqlCommand(getSerieDirectorsCommand, connection);
             //reader = sqlCMNDGetSeries.ExecuteReader();
             //while (reader.Read()) {
             //    int serieID = (int)reader["serieID"];
             //    int directorID = (int)reader["directorID"];
             //    FinderInCollection.FindInSerieCollection(listOfSeries, serieID).Directors.Add(FinderInCollection.FindInPersonCollection(personList, directorID));
             //    counter++;
             //}

             #endregion

             if (wasClosed)
                 connection.Close();
             reader.Close();

             return listOfSeries;
         }
         public static ObservableCollection<SerieSeason> FetchSerieSeasonList(ObservableCollection<Serie> serieList, ref int counter) {
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             ObservableCollection<SerieSeason> listOfSerSeasons = new ObservableCollection<SerieSeason>();
             string getSeasonsCommand = "SELECT * FROM SerieSeason ORDER BY name";
             SqlCommand slqCMNDGetSeasons = new SqlCommand(getSeasonsCommand, connection);
             reader = slqCMNDGetSeasons.ExecuteReader();
             while (reader.Read()) {
                 SerieSeason newSeason = new SerieSeason();
                 newSeason.ID = (int)reader["seasonID"];
                 newSeason.Name = (string)reader["name"];
                 newSeason.InternetLink = (string)reader["internetLink"];
                 int parentSerieID = (int)reader["serieID"];
                 newSeason.ParentSerie = FinderInCollection.FindInSerieCollection(serieList, parentSerieID);
                 FinderInCollection.FindInSerieCollection(serieList, parentSerieID).Seasons.Add(newSeason);
                 listOfSerSeasons.Add(newSeason);
                 counter++;
             }
             if (wasClosed)
                 connection.Close();
             reader.Close();

             return listOfSerSeasons;
         }
         public static ObservableCollection<SerieEpisode> FetchSerieEpisodeList(ObservableCollection<SerieSeason> seasonList, ObservableCollection<Language> languageList,
            ObservableCollection<HDD> hddList, ref int counter) {
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode>();
             string getEpisodesCommand = "SELECT * FROM SerieEpisode ORDER BY name";
             //string getEpisodeSubtitle = "SELECT * FROM [SerieEpisode-SubtitleLanguage]";
             SqlCommand slqCMNDGetEpisodes;

             #region Main

             slqCMNDGetEpisodes = new SqlCommand(getEpisodesCommand, connection);
             reader = slqCMNDGetEpisodes.ExecuteReader();
             while (reader.Read()) {
                 SerieEpisode newEpisode = new SerieEpisode();
                 newEpisode.ID = (int)reader["episodeID"];
                 newEpisode.Name = (string)reader["name"];
                 //newEpisode.OrigName = (string)reader["origName"];
                 //newEpisode.AddTime = (DateTime)reader["addDate"];
                 //newEpisode.AirDate = (DateTime)reader["airDate"];
                 //newEpisode.Size = (long)reader["size"];
                 //newEpisode.Runtime = (int)reader["runtime"];
                 //newEpisode.Width = (int)reader["width"];
                 //newEpisode.Height = (int)reader["height"];
                 //int hddID = (int)reader["hddID"];
                 //newEpisode.Hdd = FinderInCollection.FindInHDDCollection(hddList, hddID);
                 //newEpisode.IsViewed = (bool)reader["viewed"];
                 //int seasonID = (int)reader["seasonID"];
                 //newEpisode.InternetLink = (string)reader["internetLink"];
                 //newEpisode.ParentSeason = FinderInCollection.FindInSerieSeasonCollection(seasonList, seasonID);
                 //FinderInCollection.FindInSerieSeasonCollection(seasonList, seasonID).Episodes.Add(newEpisode);
                 listOfSerEpisodes.Add(newEpisode);
                 counter++;
             }
             reader.Close();
             if (listOfSerEpisodes.Count == 0)
                 return listOfSerEpisodes;

             #endregion

             
             #region Subtitles

             //slqCMNDGetEpisodes = new SqlCommand(getEpisodeSubtitle, connection);
             //reader = slqCMNDGetEpisodes.ExecuteReader();
             //while (reader.Read()) {
             //    int episodeID = (int)reader["episodeID"];
             //    int subtitleID = (int)reader["subtitleID"];
             //    FinderInCollection.FindInSerieEpisodeCollection(listOfSerEpisodes, episodeID).SubtitleLanguageList.Add(FinderInCollection.FindInLanguageCollection(languageList, subtitleID));
             //    counter++;
             //}
             //reader.Close();

             #endregion

             if (wasClosed)
                 connection.Close();

             return listOfSerEpisodes;
         }

        #endregion

        #region MISC

         public static int CountPersons() {
             stringCommand = "SELECT COUNT(*) as count FROM person";
             command = new SqlCommand(stringCommand, connection);
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             reader = command.ExecuteReader();
             reader.Read();
             int toReturn = int.Parse(reader["count"].ToString());
             if (wasClosed)
                 connection.Close();
             return toReturn;
         }
         public static int CountHDDs() {
             stringCommand = "SELECT COUNT(*) as count FROM hdd";
             command = new SqlCommand(stringCommand, connection);
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             reader = command.ExecuteReader();
             reader.Read();
             int toReturn = int.Parse(reader["count"].ToString());
             if (wasClosed)
                 connection.Close();
             return toReturn;
         }
         public static int CountLanguages() {
             stringCommand = "SELECT COUNT(*) as count FROM language";
             command = new SqlCommand(stringCommand, connection);
             bool wasClosed = true;
             if (connection.State == ConnectionState.Open)
                 wasClosed = false;
             else
                 connection.Open();
             reader = command.ExecuteReader();
             reader.Read();
             int toReturn = int.Parse(reader["count"].ToString());
             if (wasClosed)
                 connection.Close();
             return toReturn;
         }

        public static ObservableCollection<Person> FetchPersonList() {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();

            ObservableCollection<Person> personList = new ObservableCollection<Person>();
            stringCommand = "SELECT * FROM Person";// WHERE typeID = 2"; 
            command = new SqlCommand(stringCommand, connection);
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read()) {
                int id = (int)reader["personID"];
                string name = (string)reader["personName"];
                Person tempPerson = new Person(new Video_katalog.PersonType(2));
                tempPerson.Name = name;
                tempPerson.ID = id;
                personList.Add(tempPerson);
            }
            reader.Close();
            if (wasClosed)
                connection.Close();
            return personList;
        }
        public static ObservableCollection<HDD> FetchHDDList() {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();
            ObservableCollection<HDD> hddList = new ObservableCollection<HDD>();
            string SqlCommandGetHdds = "SELECT * FROM HDD";
            SqlCommand command = new SqlCommand(SqlCommandGetHdds, connection);
            reader = command.ExecuteReader();

            while (reader.Read()) {
                HDD tempHdd = new HDD();
                tempHdd.ID = (int)reader["hddID"];
                tempHdd.Name = (string)reader["name"];
                tempHdd.PurchaseDate = (DateTime)reader["purchaseDate"];
                tempHdd.WarrantyLength = (int)reader["warrantyLength"];
                tempHdd.Capacity = (long)reader["capacity"];
                tempHdd.FreeSpace = (long)reader["freeSpace"];
                hddList.Add(tempHdd);
            }
            reader.Close();
            if (wasClosed)
                connection.Close();
            return hddList;
        }
        public static ObservableCollection<Genre> FetchGenreList() {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();
            ObservableCollection<Genre> genreList = new ObservableCollection<Genre>();
            string SqlCommandGetGenres2 = "SELECT * FROM [Genre]";
            SqlCommand command = new SqlCommand(SqlCommandGetGenres2, connection);
            reader = command.ExecuteReader();

            while (reader.Read()) {
                Genre tempGenre = new Genre();
                tempGenre.ID = (int)reader["genreID"];
                tempGenre.Name = (string)reader["genreName"];
                genreList.Add(tempGenre);
            }
            if (wasClosed)
                connection.Close();
            reader.Close();
            return genreList;
        }
        public static ObservableCollection<Language> FetchLanguageList() {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();
            ObservableCollection<Language> languageList = new ObservableCollection<Language>();
            string SqlCommandGetLanguages = "SELECT * FROM Language";
            SqlCommand command = new SqlCommand(SqlCommandGetLanguages, connection);
            reader = command.ExecuteReader();

            while (reader.Read()) {
                Language tempLanguage = new Language();
                tempLanguage.ID = (int)reader["languageID"];
                tempLanguage.Name = (string)reader["name"];
                languageList.Add(tempLanguage);
            }
            reader.Close();
            if (wasClosed)
                connection.Close();
            return languageList;
        }

        #endregion

        #endregion

        #region INSERT / UPDATE / DELETE

        static void ExecuteCommand(SqlCommand SqlCommand) {
            bool wasClosed = true;
            if (connection.State == ConnectionState.Open)
                wasClosed = false;
            else
                connection.Open();
            SqlCommand.ExecuteNonQuery();
            SqlCommand.Parameters.Clear();
            if (wasClosed)
                connection.Close();
        }

        #region Movie

        public static void InsertNewMovie(Movie movie) {
            stringCommand = "INSERT INTO [movie] (movieID, customName, originalName, addDate, year, rating, internetLink, " +
                                        "trailerLink, plot, size, runtime, " +
                                        "hddNum, version, hasCroSub, hasEngSub, cast, " +
                                        "director, genres) VALUES " +
                                        "(@movieID, @customName, @originalName, @addDate, @year, @rating, " +
                                        "@internetLink, @trailerLink, @plot, @size, " +
                                        "@runtime, @hddNum, @version, @hasCroSub, " +
                                        "@hasEngSub, @cast, @director, @genres)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            command.Parameters.AddWithValue("@customName", movie.Name);
            command.Parameters.AddWithValue("@originalName", movie.OrigName);
            command.Parameters.AddWithValue("@addDate", movie.AddTime);
            command.Parameters.AddWithValue("@year", movie.Year);
            command.Parameters.AddWithValue("@rating", movie.Rating);
            command.Parameters.AddWithValue("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue("@plot", movie.Summary);
            command.Parameters.AddWithValue("@size", movie.Size);
            command.Parameters.AddWithValue("@runtime", movie.Runtime);
            command.Parameters.AddWithValue("@hddNum", movie.Hdd.Name);
            command.Parameters.AddWithValue("@version", movie.Version);
            bool hasCroSub = false;
            bool hasEngSub = false;
            foreach (Language movieLang in movie.SubtitleLanguageList) {
                if (movieLang.Name.Trim().ToLower() == "croatian") {
                    hasCroSub = true;
                }
                else if (movieLang.Name.Trim().ToLower() == "english") {
                    hasEngSub = true;
                }
            }
            string cast = "";
            int countCast = 0;
            foreach (Person actor in movie.Actors) {
                cast += actor.Name + ", ";
                countCast++;
                if (countCast >= 3) {
                    break;
                }
            }
            cast = cast.Substring(0, Math.Max(0, cast.Length - 2));
            string director = "";
            foreach (Person dir in movie.Directors) {
                director += dir.Name + ", ";
            }
            director = director.Substring(0, Math.Max(0, director.Length - 2));
            string genres = "";
            foreach (Genre tempGen in movie.Genres) {
                genres += tempGen.Name + ", ";
            }
            genres = genres.Substring(0, Math.Max(0, genres.Length - 2));
            command.Parameters.AddWithValue("@hasCroSub", hasCroSub);
            command.Parameters.AddWithValue("@hasEngSub", hasEngSub);
            command.Parameters.AddWithValue("@cast", cast);
            command.Parameters.AddWithValue("@director", director);
            command.Parameters.AddWithValue("@genres", genres);

            ExecuteCommand(command);
           
            //InsertMovieSubtitles(movie);
            //InsertMovieCast(movie);
            //InsertMovieDirectors(movie);
            //InsertMovieGenres(movie);
        }
        public static void UpdateMovie(Movie movie) {
            stringCommand = "UPDATE [movie] SET customName = @customName ,originalName = @originalName ,addDate = @addDate ," +
                                        "year = @year ,rating = @rating ,internetLink = @internetLink ,trailerLink = @trailerLink ," +
                                        "plot = @plot ,size = @size ,runtime = @runtime ," +                                       
                                        "hddNum = @hddNum, version = @version, " +
                                        "hasCroSub = @hasCroSub, hasEngSub = @hasEngSub, cast = @cast, " +
                                        "director = @director, genres = @genres WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@customName", movie.Name);
            command.Parameters.AddWithValue("@originalName", movie.OrigName);
            command.Parameters.AddWithValue("@addDate", movie.AddTime);
            command.Parameters.AddWithValue("@year", movie.Year);
            command.Parameters.AddWithValue("@rating", movie.Rating);
            command.Parameters.AddWithValue("@internetLink", movie.InternetLink);
            command.Parameters.AddWithValue("@trailerLink", movie.TrailerLink);
            command.Parameters.AddWithValue("@plot", movie.Summary);
            command.Parameters.AddWithValue("@size", movie.Size);
            command.Parameters.AddWithValue("@runtime", movie.Runtime);
            command.Parameters.AddWithValue("@hddNum", movie.Hdd.Name);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            command.Parameters.AddWithValue("@version", movie.Hdd.ID);

            bool hasCroSub = false;
            bool hasEngSub = false;
            foreach (Language movieLang in movie.SubtitleLanguageList) {
                if (movieLang.Name.Trim().ToLower() == "croatian") {
                    hasCroSub = true;
                }
                else if (movieLang.Name.Trim().ToLower() == "english") {
                    hasEngSub = true;
                }
            }
            string cast = "";
            int countCast = 0;
            foreach (Person actor in movie.Actors) {
                cast += actor.Name + ", ";
                countCast++;
                if (countCast >= 3) {
                    break;
                }
            }
            cast = cast.Substring(0, Math.Max(0, cast.Length - 2));
            string director = "";
            foreach (Person dir in movie.Directors) {
                director += dir.Name + ", ";
            }
            director = director.Substring(0, Math.Max(director.Length - 2, 0));
            string genres = "";
            foreach (Genre tempGen in movie.Genres) {
                genres += tempGen.Name + ", ";
            }
            genres = genres.Substring(0, Math.Max(0, genres.Length - 2));
            command.Parameters.AddWithValue("@hasCroSub", hasCroSub);
            command.Parameters.AddWithValue("@hasEngSub", hasEngSub);
            command.Parameters.AddWithValue("@cast", cast);
            command.Parameters.AddWithValue("@director", director);
            command.Parameters.AddWithValue("@genres", genres);

            ExecuteCommand(command);

            //DeleteMovieSubtitles(movie);
            //InsertMovieSubtitles(movie);
            //DeleteMovieCast(movie);
            //InsertMovieCast(movie);
            //DeleteMovieDirectors(movie);
            //InsertMovieDirectors(movie);
            //DeleteMovieGenres(movie);
            //InsertMovieGenres(movie);
        }
        public static void DeleteMovie(Movie movie) {
            stringCommand = "DELETE FROM [movie] WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            ExecuteCommand(command);
            //DeleteMovieCast(movie);
            //DeleteMovieDirectors(movie);
            //DeleteMovieGenres(movie);
            //DeleteMovieSubtitles(movie);
        }

        #region NO NEED

        static void InsertMovieSubtitles(Movie movie) {
            stringCommand = "INSERT INTO [movie-subtitlelanguage] (movieID ,subtitleLanguageID) VALUES (@movieID, @subtitleLanguageID)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Language tempLang in movie.SubtitleLanguageList) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@movieID", movie.ID);
                command.Parameters.AddWithValue("@subtitleLanguageID", tempLang.ID);
                ExecuteCommand(command);
            }
        }
        static void DeleteMovieSubtitles(Movie movie) {
            stringCommand = "DELETE FROM [movie-subtitlelanguage] WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            ExecuteCommand(command);
        }
        static void InsertMovieGenres(Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [movie-genre] (movieID ,genreID, genreNum) VALUES (@movieID, @genreID, @genreNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Genre tempGenre in movie.Genres) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@movieID", movie.ID);
                command.Parameters.AddWithValue("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue("@genreNum", count++);
                ExecuteCommand(command);
            }
        }
        static void DeleteMovieGenres(Movie movie) {
            stringCommand = "DELETE FROM [movie-genre] WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            ExecuteCommand(command);
        }
        static void InsertMovieCast(Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [movie-cast] (personID ,movieID, actorNum) VALUES (@personID, @movieID, @actorNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Person actor in movie.Actors) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@movieID", movie.ID);
                command.Parameters.AddWithValue("@personID", actor.ID);
                command.Parameters.AddWithValue("@actorNum", count++);
                ExecuteCommand(command);
            }
        }
        static void DeleteMovieCast(Movie movie) {
            stringCommand = "DELETE FROM [movie-cast] WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            ExecuteCommand(command);
        }
        static void InsertMovieDirectors(Movie movie) {
            byte count = 0;
            stringCommand = "INSERT INTO [movie-director] (movieID ,directorID, directorNum) VALUES (@movieID, @directorID, @directorNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Person director in movie.Directors) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@movieID", movie.ID);
                command.Parameters.AddWithValue("@directorID", director.ID);
                command.Parameters.AddWithValue("@directorNum", count++);
                ExecuteCommand(command);
            }
        }
        static void DeleteMovieDirectors(Movie movie) {
            stringCommand = "DELETE FROM [movie-director] WHERE movieID = @movieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@movieID", movie.ID);
            ExecuteCommand(command);
        }

        #endregion
        

        #endregion

        #region Serie

        public static void InsertSerie(Serie serie) {
            stringCommand = "INSERT INTO [serie] (serieID, name, origName, rating, internetLink, " +
                            "summary, trailerLink, cast, director, genres) " + 
                            "VALUES (@serieID, @name, @origName, @rating, " +
                            "@internetLink, @summary, @trailerLink, @cast, @director, @genres)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            command.Parameters.AddWithValue("@name", serie.Name);
            command.Parameters.AddWithValue("@origName", serie.OrigName);
            command.Parameters.AddWithValue("@rating", serie.Rating);
            command.Parameters.AddWithValue("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue("@summary", serie.Summary);
            command.Parameters.AddWithValue("@trailerLink", serie.TrailerLink);

            string cast = "";
            int countCast = 0;
            foreach (Person actor in serie.Actors) {
                cast += actor.Name + ", ";
                countCast++;
                if (countCast >= 8) {
                    break;
                }
            }
            cast = cast.Substring(0, Math.Max(0, cast.Length - 2));
            string director = "";
            int countDirs = 0;
            foreach (Person dir in serie.Directors) {
                director += dir.Name + ", ";
                countDirs++;
                if (countDirs >= 8) {
                    break;
                }
            }
            director = director.Substring(0, Math.Max(0, director.Length - 2));
            string genres = "";
            foreach (Genre tempGen in serie.Genres) {
                genres += tempGen.Name + ", ";
            }
            genres = genres.Substring(0, Math.Max(0, genres.Length - 2));
            command.Parameters.AddWithValue("@cast", cast);
            command.Parameters.AddWithValue("@director", director);
            command.Parameters.AddWithValue("@genres", genres);

            ExecuteCommand(command);

            //InsertSerieCast(serie);
            //InsertSerieDirectors(serie);
            //InsertSerieGenres(serie);
        }
        public static void UpdateOnlySerie(Serie serie) {
            stringCommand = "UPDATE [serie] SET name = @name, origName = @origName, rating = @rating, " + 
                            "internetLink = @internetLink, summary = @summary, trailerLink = @trailerLink, " +
                            ", cast = @cast, director = @director, " +
                            "genres = @genres WHERE serieID = @serieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@name", serie.Name);
            command.Parameters.AddWithValue("@origName", serie.OrigName);
            command.Parameters.AddWithValue("@rating", serie.Rating);
            command.Parameters.AddWithValue("@internetLink", serie.InternetLink);
            command.Parameters.AddWithValue("@summary", serie.Summary);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            command.Parameters.AddWithValue("@trailerLink", serie.TrailerLink);

            string cast = "";
            int countCast = 0;
            foreach (Person actor in serie.Actors) {
                cast += actor.Name + ", ";
                countCast++;
                if (countCast >= 8) {
                    break;
                }
            }
            cast = cast.Substring(0, Math.Max(0, cast.Length - 2));
            string director = "";
            int countDirs = 0;
            foreach (Person dir in serie.Directors) {
                director += dir.Name + ", ";
                countDirs++;
                if (countDirs >= 8) {
                    break;
                }
            }
            director = director.Substring(0, Math.Max(0, director.Length - 2));
            string genres = "";
            foreach (Genre tempGen in serie.Genres) {
                genres += tempGen.Name + ", ";
            }
            genres = genres.Substring(0, Math.Max(0, genres.Length - 2));
            command.Parameters.AddWithValue("@cast", cast);
            command.Parameters.AddWithValue("@director", director);
            command.Parameters.AddWithValue("@genres", genres);

            ExecuteCommand(command);

            //DeleteSerieCast(serie);
            //InsertSerieCast(serie);
            //DeleteSerieDirectors(serie);
            //InsertSerieDirectors(serie);
            //DeleteSerieGenres(serie);
            //InsertSerieGenres(serie);           

        }
        public static void UpdateSerie(Serie serie) {
            try {
                DeleteSerie(serie);
            }
            catch {
            }
            InsertSerie(serie);
            foreach (SerieSeason tempSeason in serie.Seasons) {
                InsertSerieSeason(tempSeason);
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    InsertSerieEpisode(tempEpisode);
                }
            }
        }

        public static void DeleteSerie(Serie serie) {
            stringCommand = "DELETE FROM [serie] WHERE serieID = @serieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            ExecuteCommand(command);
            foreach (SerieSeason tempSeason in serie.Seasons)
                DeleteSerieSeason(tempSeason);
            //DeleteSerieCast(serie);
            //DeleteSerieDirectors(serie);
            //DeleteSerieGenres(serie);
        }

        public static void InsertSerieSeason(SerieSeason season) {
            stringCommand = "INSERT INTO [serieseason] (seasonID, serieID ,name ,internetLink)" +
                "VALUES (@seasonID, @serieID , @name , @internetLink)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@seasonID", season.ID);
            command.Parameters.AddWithValue("@serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue("@name", season.Name);
            command.Parameters.AddWithValue("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue("@trailerLink", season.TrailerLink);
            
            ExecuteCommand(command);
        }
        public static void UpdateSerieSeason(SerieSeason season) {
            stringCommand = "UPDATE [serieseason] SET serieID = @serieID ,name = @name ,internetLink = @internetLink ,trailerLink = @trailerLink WHERE seasonID = @seasonID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue("@name", season.Name);
            command.Parameters.AddWithValue("@internetLink", season.InternetLink);
            command.Parameters.AddWithValue("@trailerLink", season.TrailerLink);
            command.Parameters.AddWithValue("@seasonID", season.ID);
            ExecuteCommand(command);


            //osvjezi i listu epizoda

            List<int> episodesIDList = new List<int>();
            foreach (SerieEpisode tempEpisode in season.Episodes) {
                if (tempEpisode.ID == 0)  //ako je ID null znaci da epizoda nije dodana u bazu
                    InsertSerieEpisode(tempEpisode);
                else
                    UpdateSerieEpisode(tempEpisode);
                episodesIDList.Add(tempEpisode.ID);
            }

            //dohvati sve epizode koje pripadaju seriji
            bool wasClosed = true;
            stringCommand = "SELECT episodeID FROM serieepisode WHERE seasonID = @seasonID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@seasonID", season.ID);
            try {
                connection.Open();
            }
            catch {
                wasClosed = false;
            }
            List<int> fetchedEpisodeIDList = new List<int>(); //lista svih ID-jeva sezona koje pripadaju seriji u bazi podataka
            reader = command.ExecuteReader();
            while (reader.Read()) {
                fetchedEpisodeIDList.Add((int)reader["episodeID"]);
            }
            reader.Close();

            //prodji kroz sve dohvacene epizode, ako dohvacena epizoda nije u listi epizoda sezone, obriši ju iz baze
            foreach (int episodeID in fetchedEpisodeIDList) {
                if (episodesIDList.Contains(episodeID) == false) {
                    stringCommand = "DELETE FROM serieepisode WHERE episodeID = @episodeID";
                    command = new SqlCommand(stringCommand, connection);
                    command.Parameters.AddWithValue("@episodeID", episodeID);
                    command.ExecuteNonQuery();
                }
            }

            if (wasClosed)
                connection.Close();
        }
        public static void DeleteSerieSeason(SerieSeason season) {
            stringCommand = "DELETE FROM [serieseason] WHERE seasonID = @seasonID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@seasonID", season.ID);
            ExecuteCommand(command);
            foreach (SerieEpisode tempepisode in season.Episodes)
                DeleteSerieEpisode(tempepisode);
        }

        public static void InsertSerieEpisode(SerieEpisode episode) {
            stringCommand = "INSERT INTO [serieepisode] (episodeID, name ,origName ,seasonID ,airDate ," +
                "size ,runtime ,hddID ,internetLink, " +
                "version, hasCroSub, hasEngSub) VALUES " +
                "(@episodeID, @name, @origName, @seasonID, @airDate, @size, @runtime," +
                "@hddID, @internetLink, @version" +
                ", @hasCroSub, @hasEngSub)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@episodeID", episode.ID);
            command.Parameters.AddWithValue("@name", episode.Name);
            command.Parameters.AddWithValue("@origName", episode.OrigName);
            command.Parameters.AddWithValue("@seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue("@airDate", episode.AirDate);
            command.Parameters.AddWithValue("@size", episode.Size);
            command.Parameters.AddWithValue("@runtime", episode.Runtime);
            command.Parameters.AddWithValue("@hddID", episode.Hdd.Name);
            command.Parameters.AddWithValue("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue("@version", episode.Version);

            bool hasCroSub = false;
            bool hasEngSub = false;
            foreach (Language serieLang in episode.SubtitleLanguageList) {
                if (serieLang.Name.Trim().ToLower() == "croatian") {
                    hasCroSub = true;
                }
                else if (serieLang.Name.Trim().ToLower() == "english") {
                    hasEngSub = true;
                }
            }
            command.Parameters.AddWithValue("@hasCroSub", hasCroSub);
            command.Parameters.AddWithValue("@hasEngSub", hasEngSub);

            ExecuteCommand(command);
            
            //InsertSerieEpisodeSubtitles(episode);
        }
        public static void UpdateSerieEpisode(SerieEpisode episode) {
            stringCommand = "UPDATE [serieepisode] SET name = @name, origName = @origName, seasonID = @seasonID, " + 
                "airDate = @airDate, size = @size, " +
                "runtime = @runtime, " +
                "hddID = @hddID," +
                "internetLink = @internetLink, version = @version, " +
                "hasCroSub = @hasCroSub, hasEngSub = @hasEngSub WHERE episodeID = @episodeID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@episodeID", episode.ID);
            command.Parameters.AddWithValue("@name", episode.Name);
            command.Parameters.AddWithValue("@origName", episode.OrigName);
            command.Parameters.AddWithValue("@seasonID", episode.ParentSeason.ID);
            command.Parameters.AddWithValue("@airDate", episode.AirDate);
            command.Parameters.AddWithValue("@size", episode.Size);
            command.Parameters.AddWithValue("@runtime", episode.Runtime);
            command.Parameters.AddWithValue("@hddID", episode.Hdd.Name);
            command.Parameters.AddWithValue("@internetLink", episode.InternetLink);
            command.Parameters.AddWithValue("@version", episode.Version);

            bool hasCroSub = false;
            bool hasEngSub = false;
            foreach (Language movieLang in episode.SubtitleLanguageList) {
                if (movieLang.Name.Trim().ToLower() == "croatian") {
                    hasCroSub = true;
                }
                else if (movieLang.Name.Trim().ToLower() == "english") {
                    hasEngSub = true;
                }
            }
            command.Parameters.AddWithValue("@hasCroSub", hasCroSub);
            command.Parameters.AddWithValue("@hasEngSub", hasEngSub);

            ExecuteCommand(command);
            //DeleteSerieEpisodeSubtitles(episode);
            //InsertSerieEpisodeSubtitles(episode);
        }
        public static void DeleteSerieEpisode(SerieEpisode episode) {
            stringCommand = "DELETE FROM [serieepisode] WHERE episodeID = @episodeID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@episodeID", episode.ID);
            ExecuteCommand(command);
            //DeleteSerieEpisodeSubtitles(episode);
        }
        static void InsertSerieEpisodeSubtitles(SerieEpisode episode) {
            foreach (Language tempLang in episode.SubtitleLanguageList) {
                stringCommand = "INSERT INTO [serieepisode-subtitlelanguage] (episodeID, subtitleID) VALUES (@episodeID, @subtitleID)";
                command = new SqlCommand(stringCommand, connection);
                command.Parameters.AddWithValue("@episodeID", episode.ID);
                command.Parameters.AddWithValue("@subtitleID", tempLang.ID);
                ExecuteCommand(command);
            }
        }
        static void DeleteSerieEpisodeSubtitles(SerieEpisode episode) {
            stringCommand = "DELETE FROM [serieepisode-subtitlelanguage] WHERE episodeID = @episodeID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@episodeID", episode.ID);

            ExecuteCommand(command);
        }

        static void InsertSerieCast(Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [serie-cast] (actorID ,serieID, actorNum) VALUES (@actorID, @serieID, @actorNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Person actor in serie.Actors) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@actorID", actor.ID);
                command.Parameters.AddWithValue("@serieID", serie.ID);
                command.Parameters.AddWithValue("@actorNum", count++);
                ExecuteCommand(command);
            }
        }
        static void DeleteSerieCast(Serie serie) {
            stringCommand = "DELETE FROM [serie-cast] WHERE serieID = @serieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            ExecuteCommand(command);
        }
        static void InsertSerieDirectors(Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [serie-director] (serieID ,directorID, directorNum) VALUES (@serieID, @directorID, @directorNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Person director in serie.Directors) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@serieID", serie.ID);
                command.Parameters.AddWithValue("@directorID", director.ID);
                command.Parameters.AddWithValue("@directorNum", count++);
                ExecuteCommand(command);
            }
        }
        static void DeleteSerieDirectors(Serie serie) {
            stringCommand = "DELETE FROM [serie-director] WHERE serieID = @serieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            ExecuteCommand(command);
        }
        static void InsertSerieGenres(Serie serie) {
            byte count = 0;
            stringCommand = "INSERT INTO [serie-genre] (serieID ,genreID, genreNum) VALUES (@serieID, @genreID, @genreNum)";
            command = new SqlCommand(stringCommand, connection);
            foreach (Genre tempGenre in serie.Genres) {
                command.Parameters.Clear();
                command.Parameters.AddWithValue("@serieID", serie.ID);
                command.Parameters.AddWithValue("@genreID", tempGenre.ID);
                command.Parameters.AddWithValue("@genreNum", count++);
                ExecuteCommand(command);
            }

        }
        static void DeleteSerieGenres(Serie serie) {
            stringCommand = "DELETE FROM [serie-genre] WHERE serieID = @serieID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@serieID", serie.ID);
            ExecuteCommand(command);
        }

        #endregion

        #region Misc

        #region Person

        public static void InsertPerson(Person person) {
            stringCommand = "INSERT INTO [person] (personID, personName) VALUES (@personID, @name)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@personID", person.ID);
            command.Parameters.AddWithValue("@name", person.Name);

            ExecuteCommand(command);
        }
        public static void DeletePerson(Person person) {
            stringCommand = "DELETE FROM [person] WHERE personID = @personID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@personID", person.ID);
            ExecuteCommand(command);
        }
        public static void UpdatePerson(Person person) {
            stringCommand = "UPDATE `person` SET personName = @personName WHERE personID = @personID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@name", person.Name);
            command.Parameters.AddWithValue("@personID", person.ID);
        }

        #endregion

        #region Language

        public static void InsertLanguage(Language language) {
            stringCommand = "INSERT INTO language (languageID, name) VALUES (@languageID, @name)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@languageID", language.ID);
            command.Parameters.AddWithValue("@name", language.Name);

            ExecuteCommand(command);
        }
        public static void DeleteLanguage(Language language) {
            stringCommand = "DELETE FROM language WHERE languageID = @languageID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@languageID", language.ID);
            ExecuteCommand(command);
        }
        public static void UpdateLanguage(Language language) {
            throw new NotImplementedException();
        }

        #endregion

        #region Hdd

        public static void InsertHDD(HDD hdd) {
            stringCommand = "INSERT INTO [hdd] (hddID, name ,purchaseDate ,warrantyLength ,capacity ,freeSpace) VALUES (@hddID, @name, @purchaseDate, @warrantyLength, @capacity, @freeSpace)";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@hddID", hdd.ID);
            command.Parameters.AddWithValue("@name", hdd.Name);
            command.Parameters.AddWithValue("@purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue("@warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue("@capacity", hdd.Capacity);
            command.Parameters.AddWithValue("@freeSpace", hdd.FreeSpace);

            ExecuteCommand(command);
        }
        public static void DeleteHDD(HDD hdd) {
            stringCommand = "DELETE FROM hdd WHERE hddID = @hddID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@hddID", hdd.ID);
            ExecuteCommand(command);
        }
        public static void UpdateHDD(HDD hdd) {
            stringCommand = "UPDATE hdd SET name = @name ,purchaseDate = @purchaseDate ,warrantyLength = @warrantyLength ,capacity = @capacity ,freeSpace = @freeSpace WHERE hddID = @hddID";
            command = new SqlCommand(stringCommand, connection);
            command.Parameters.AddWithValue("@name", hdd.Name);
            command.Parameters.AddWithValue("@purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue("@warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue("@capacity", hdd.Capacity);
            command.Parameters.AddWithValue("@freeSpace", hdd.FreeSpace);
            command.Parameters.AddWithValue("@hddID", hdd.ID);
            ExecuteCommand(command);
        }

        #endregion

        #endregion

        #endregion

    }
}
