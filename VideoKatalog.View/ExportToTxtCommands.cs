using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.IO;

namespace Video_katalog {
    public static class ExportToTxtCommands {
        public static MySqlCommand command;
        public static string stringCommand;

        static void ReplaceParameters (MySqlCommand comm, ref string stringComm) {
            foreach (MySqlParameter tempPar in comm.Parameters) {
                if (tempPar.Value.GetType () == typeof (string)) {
                    stringComm = stringComm.Replace (tempPar.ParameterName, "\"" + tempPar.Value.ToString ().Replace ("\"", "'") + "\"");
                    //stringComm = "\"" + stringComm + "\"";
                } 
                else if (tempPar.Value.GetType () == typeof(DateTime)) {
                    DateTime date = (DateTime) tempPar.Value;
                    string dateStringToInsert = string.Format ("'{0}-{1}-{2} {3}'", date.Year, date.Month, date.Day, date.ToLongTimeString());
                    stringComm = stringComm.Replace (tempPar.ParameterName, dateStringToInsert);
                }
                else
                    stringComm = stringComm.Replace (tempPar.ParameterName, tempPar.Value.ToString ().Replace(",", "."));
               
            }
            stringComm = stringComm + ";";
        }      


        #region Misc

        public static void InsertPersonType (PersonType type, StreamWriter personOutput) {
            stringCommand = "INSERT INTO `persontype` (typeID, typeName) VALUES (?typeID, ?typeName)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?typeID", type.ID);
            command.Parameters.AddWithValue ("?typeName", type.Name);

            ReplaceParameters (command, ref stringCommand);
            personOutput.WriteLine (stringCommand);
        }
        public static void InsertPerson (Person person, StreamWriter personOutput) {
            stringCommand = "INSERT INTO `person` (personID, typeID, personName, dateOfBirth) VALUES (?personID, ?typeID, ?name, ?dateOfBirth)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?personID", person.ID);
            command.Parameters.AddWithValue ("?name", person.Name);
            command.Parameters.AddWithValue ("?typeID", person.Type.ID);
            command.Parameters.AddWithValue ("?dateOfBirth", person.DateOfBirth);

            ReplaceParameters (command, ref stringCommand);
            personOutput.WriteLine (stringCommand);
        }
        public static void InsertLanguage (Language language, StreamWriter languageOutput) {
            stringCommand = "INSERT INTO language (languageID, name) VALUES (?languageID, ?name)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?languageID", language.ID);
            command.Parameters.AddWithValue ("?name", language.Name);

            ReplaceParameters (command, ref stringCommand);
            languageOutput.WriteLine (stringCommand);

        }
        public static void InsertHDD (HDD hdd, StreamWriter hddOutput) {
            stringCommand = "INSERT INTO `hdd` (hddID, name ,purchaseDate ,warrantyLength ,capacity ,freeSpace) VALUES (?hddID, ?name, ?purchaseDate, ?warrantyLength, ?capacity, ?freeSpace)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?hddID", hdd.ID);
            command.Parameters.AddWithValue ("?name", hdd.Name);
            command.Parameters.AddWithValue ("?purchaseDate", hdd.PurchaseDate);
            command.Parameters.AddWithValue ("?warrantyLength", hdd.WarrantyLength);
            command.Parameters.AddWithValue ("?capacity", hdd.Capacity);
            command.Parameters.AddWithValue ("?freeSpace", hdd.FreeSpace);

            ReplaceParameters (command, ref stringCommand);
            hddOutput.WriteLine (stringCommand);

        }
        public static void InsertAudio (Audio audio, StreamWriter audioOutput) {
            stringCommand = "INSERT INTO audio (ID, channels, languageID, format) VALUES (?ID, ?channels, ?languageID, ?format)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?ID", audio.ID);
            command.Parameters.AddWithValue ("?channels", audio.Channels);
            command.Parameters.AddWithValue ("?languageID", audio.Language.ID);
            command.Parameters.AddWithValue ("?format", audio.Format);

            ReplaceParameters (command, ref stringCommand);
            audioOutput.WriteLine (stringCommand);
        }
        public static void InsertGenres (Genre gen, StreamWriter genreOutput) {
            stringCommand = "INSERT INTO genre (genreID, genreName) VALUES (?genreID, ?genreName)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?genreID", gen.ID);
            command.Parameters.AddWithValue ("?genreName", gen.Name);

            ReplaceParameters (command, ref stringCommand);
            genreOutput.WriteLine (stringCommand);
        }

        #endregion

        #region Movie

        public static void InsertNewMovie (Movie movie, StreamWriter movieOutput) {
            stringCommand = "INSERT INTO `movie` (movieID, customName, originalName, addDate, year, rating, internetLink, " +
                                        "trailerLink, plot, size, height, width, runtime, bitrate, aspectRatio, budget, " +
                                        "earnings, isViewed, hddNum, version) VALUES " +
                                        "(?movieID, ?customName, ?originalName, ?addDate, ?year, ?rating, " +
                                        "?internetLink, ?trailerLink, ?plot, ?size, ?height, ?width, " +
                                        "?runtime, ?bitrate, ?aspectRatio, ?budget, ?earnings, ?isViewed, ?hddNum, ?version)";
            command = new MySqlCommand (stringCommand);
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

            ReplaceParameters (command, ref stringCommand);
            movieOutput.WriteLine (stringCommand);

            InsertMovieAudios (movie, movieOutput);
            InsertMovieSubtitles (movie, movieOutput);
            InsertMovieCast (movie, movieOutput);
            InsertMovieDirectors (movie, movieOutput);
            InsertMovieGenres (movie, movieOutput);
        }
        static void InsertMovieAudios (Movie movie, StreamWriter movieOutput) {
            foreach (Audio tempAudio in movie.AudioList) {
                stringCommand = "INSERT INTO `movie-audio` (movieID, audioID) VALUES (?movieID, ?audioID)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?audioID", tempAudio.ID);

                ReplaceParameters (command, ref stringCommand);
                movieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertMovieSubtitles (Movie movie, StreamWriter movieOutput) {            
            foreach (Language tempLang in movie.SubtitleLanguageList) {
                stringCommand = "INSERT INTO `movie-subtitlelanguage` (movieID ,subtitleLanguageID) VALUES (?movieID, ?subtitleLanguageID)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?subtitleLanguageID", tempLang.ID);

                ReplaceParameters (command, ref stringCommand);
                movieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertMovieGenres (Movie movie, StreamWriter movieOutput) {
            byte count = 0;            
            foreach (Genre tempGenre in movie.Genres) {
                stringCommand = "INSERT INTO `movie-genre` (movieID ,genreID, genreNum) VALUES (?movieID, ?genreID, ?genreNum)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("?genreNum", count++);

                ReplaceParameters (command, ref stringCommand);
                movieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertMovieCast (Movie movie, StreamWriter movieOutput) {
            byte count = 0;           
            foreach (Person actor in movie.Actors) {
                stringCommand = "INSERT INTO `movie-cast` (personID ,movieID, actorNum) VALUES (?personID, ?movieID, ?actorNum)"; 
                command = new MySqlCommand (stringCommand);
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?personID", actor.ID);
                command.Parameters.AddWithValue ("?actorNum", count++);

                ReplaceParameters (command, ref stringCommand);
                movieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertMovieDirectors (Movie movie, StreamWriter movieOutput) {
            byte count = 0;            
            foreach (Person director in movie.Directors) {
                stringCommand = "INSERT INTO `movie-director` (movieID ,directorID, directorNum) VALUES (?movieID, ?directorID, ?directorNum)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?movieID", movie.ID);
                command.Parameters.AddWithValue ("?directorID", director.ID);
                command.Parameters.AddWithValue ("?directorNum", count++);

                ReplaceParameters (command, ref stringCommand);
                movieOutput.WriteLine (stringCommand);
            }
        }

        #endregion

        #region Serie

        public static void InsertSerie (Serie serie, StreamWriter serieOutput) {
            stringCommand = "INSERT INTO `serie` (serieID, name, origName, rating, internetLink, summary, trailerLink) VALUES (?serieID, ?name, ?origName, ?rating, ?internetLink, ?summary, ?trailerLink)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?serieID", serie.ID);
            command.Parameters.AddWithValue ("?name", serie.Name);
            command.Parameters.AddWithValue ("?origName", serie.OrigName);
            command.Parameters.AddWithValue ("?rating", serie.Rating);
            command.Parameters.AddWithValue ("?internetLink", serie.InternetLink);
            command.Parameters.AddWithValue ("?summary", serie.Summary);
            command.Parameters.AddWithValue ("?trailerLink", serie.TrailerLink);

            ReplaceParameters (command, ref stringCommand);
            serieOutput.WriteLine (stringCommand);
            
            InsertSerieCast (serie, serieOutput);
            InsertSerieDirectors (serie, serieOutput);
            InsertSerieGenres (serie, serieOutput);
        }
        static void InsertSerieCast (Serie serie, StreamWriter serieOutput) {
            byte count = 0;            
            foreach (Person actor in serie.Actors) {
                stringCommand = "INSERT INTO `serie-cast` (actorID ,serieID, actorNum) VALUES (?actorID, ?serieID, ?actorNum)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?actorID", actor.ID);
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?actorNum", count++);

                ReplaceParameters (command, ref stringCommand);
                serieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertSerieDirectors (Serie serie, StreamWriter serieOutput) {
            byte count = 0;
            foreach (Person director in serie.Directors) {
                stringCommand = "INSERT INTO `serie-director` (serieID ,directorID, directorNum) VALUES (?serieID, ?directorID, ?directorNum)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?directorID", director.ID);
                command.Parameters.AddWithValue ("?directorNum", count++);

                ReplaceParameters (command, ref stringCommand);
                serieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertSerieGenres (Serie serie, StreamWriter serieOutput) {
            byte count = 0;
            foreach (Genre tempGenre in serie.Genres) {
                stringCommand = "INSERT INTO `serie-genre` (serieID ,genreID, genreNum) VALUES (?serieID, ?genreID, ?genreNum)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.Clear ();
                command.Parameters.AddWithValue ("?serieID", serie.ID);
                command.Parameters.AddWithValue ("?genreID", tempGenre.ID);
                command.Parameters.AddWithValue ("?genreNum", count++);

                ReplaceParameters (command, ref stringCommand);
                serieOutput.WriteLine (stringCommand);
            }

        }

        public static void InsertSerieSeason (SerieSeason season, StreamWriter serieOutput) {
            stringCommand = "INSERT INTO `serieseason` (seasonID, serieID ,name ,internetLink ,trailerLink) VALUES (?seasonID, ?serieID ,?name ,?internetLink ,?trailerLink)";
            command = new MySqlCommand (stringCommand);
            command.Parameters.AddWithValue ("?seasonID", season.ID);
            command.Parameters.AddWithValue ("?serieID", season.ParentSerie.ID);
            command.Parameters.AddWithValue ("?name", season.Name);
            command.Parameters.AddWithValue ("?internetLink", season.InternetLink);
            command.Parameters.AddWithValue ("?trailerLink", season.TrailerLink);

            ReplaceParameters (command, ref stringCommand);
            serieOutput.WriteLine (stringCommand);
        }

        public static void InsertSerieEpisode (SerieEpisode episode, StreamWriter serieOutput) {
            stringCommand = "INSERT INTO `serieepisode` (episodeID, name ,origName ,seasonID ,addDate ,airDate ," +
                "size ,runtime ,bitrate ,width ,height ,aspectRatio ,hddID ,summary ,viewed, internetLink, trailerLink, version) VALUES " +
                "(?episodeID, ?name, ?origName, ?seasonID, ?addDate, ?airDate, ?size, ?runtime, ?bitrate, " +
                "?width, ?height, ?aspectRatio, ?hddID, ?summary, ?viewed, ?internetLink, ?trailerLink, ?version)";
            command = new MySqlCommand (stringCommand);
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

            ReplaceParameters (command, ref stringCommand);
            serieOutput.WriteLine (stringCommand);

            InsertSerieEpisodeAudios (episode, serieOutput);
            InsertSerieEpisodeSubtitles (episode, serieOutput);
        }
        static void InsertSerieEpisodeAudios (SerieEpisode episode, StreamWriter serieOutput) {
            foreach (Audio tempAudio in episode.AudioList) {
                stringCommand = "INSERT INTO `serieepisode-audio` (episodeID, audioID) VALUES (?episodeID, ?audioID)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.AddWithValue ("?episodeID", episode.ID);
                command.Parameters.AddWithValue ("?audioID", tempAudio.ID);

                ReplaceParameters (command, ref stringCommand);
                serieOutput.WriteLine (stringCommand);
            }
        }
        static void InsertSerieEpisodeSubtitles (SerieEpisode episode, StreamWriter serieOutput) {
            foreach (Language tempLang in episode.SubtitleLanguageList) {
                stringCommand = "INSERT INTO `serieepisode-subtitlelanguage` (episodeID, subtitleID) VALUES (?episodeID, ?subtitleID)";
                command = new MySqlCommand (stringCommand);
                command.Parameters.AddWithValue ("?episodeID", episode.ID);
                command.Parameters.AddWithValue ("?subtitleID", tempLang.ID);

                ReplaceParameters (command, ref stringCommand);
                serieOutput.WriteLine (stringCommand);
            }
        }

        #endregion
    }
}
