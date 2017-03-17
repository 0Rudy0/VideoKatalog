using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace Video_katalog {
    public class Cloner {

        public Movie CloneMovie (Movie origMovie) {
            Movie copyMovie = new Movie ();
            copyMovie.ID = origMovie.ID;
            copyMovie.Name = origMovie.Name;
            copyMovie.OrigName = origMovie.OrigName;
            copyMovie.Height = origMovie.Height;
            copyMovie.Width = origMovie.Width;
            copyMovie.Runtime = origMovie.Runtime;
            copyMovie.Size = origMovie.Size;
            copyMovie.Hdd = CloneHDD (origMovie.Hdd);
            copyMovie.Bitrate = origMovie.Bitrate;
            copyMovie.AspectRatio = origMovie.AspectRatio;
            copyMovie.AddTime = origMovie.AddTime;
            copyMovie.Summary = origMovie.Summary;
            copyMovie.InternetLink = origMovie.InternetLink;
            copyMovie.TrailerLink = origMovie.TrailerLink;
            copyMovie.IsViewed = origMovie.IsViewed;
            copyMovie.Year = origMovie.Year;
            copyMovie.Rating = origMovie.Rating;
            copyMovie.Budget = origMovie.Budget;
            copyMovie.Earnings = origMovie.Earnings;
            copyMovie.AudioList = CloneAudioList (origMovie.AudioList);
            copyMovie.SubtitleLanguageList = CloneLangList (origMovie.SubtitleLanguageList);
            copyMovie.Actors = ClonePersonList (origMovie.Actors);
            copyMovie.Directors = ClonePersonList (origMovie.Directors);
            copyMovie.Genres = origMovie.Genres;
            copyMovie.AudioList = CloneAudioList (origMovie.AudioList);
            copyMovie.Version = origMovie.Version;
            return copyMovie;
        }

        public Serie CloneSerieWithSeasons (Serie orginal) {
            Serie copySerie = new Serie ();
            copySerie.Actors = ClonePersonList (orginal.Actors);
            copySerie.Directors = ClonePersonList (orginal.Directors);
            copySerie.Genres = CloneGenreList (orginal.Genres);
            copySerie.ID = orginal.ID;
            copySerie.InternetLink = orginal.InternetLink;
            copySerie.IsViewed = orginal.IsViewed;
            copySerie.Name = orginal.Name;
            copySerie.OrigName = orginal.OrigName;
            copySerie.Rating = orginal.Rating;
            copySerie.Summary = orginal.Summary;
            copySerie.TrailerLink = orginal.TrailerLink;
            foreach (SerieSeason origSeason in orginal.Seasons) {
                SerieSeason copySeason = CloneSeasonWithEpisodes (origSeason);
                copySeason.ParentSerie = copySerie;
                copySerie.Seasons.Add (copySeason);
            }
            return copySerie;
        }
        public Serie CloneSerieOnlyInfo (Serie orginal) {
            Serie copySerie = new Serie ();
            copySerie.Actors = ClonePersonList (orginal.Actors);
            copySerie.Directors = ClonePersonList (orginal.Directors);
            copySerie.Genres = CloneGenreList (orginal.Genres);
            copySerie.ID = orginal.ID;
            copySerie.InternetLink = orginal.InternetLink;
            copySerie.IsViewed = orginal.IsViewed;
            copySerie.Name = orginal.Name;
            copySerie.OrigName = orginal.OrigName;
            copySerie.Rating = orginal.Rating;
            copySerie.Summary = orginal.Summary;
            copySerie.TrailerLink = orginal.TrailerLink;
            return copySerie;
        }
        public SerieSeason CloneSeasonWithEpisodes (SerieSeason orginal) {
            SerieSeason copySeason = new SerieSeason ();
            copySeason.Name = orginal.Name;
            copySeason.ID = orginal.ID;
            copySeason.IsViewed = orginal.IsViewed;
            copySeason.TrailerLink = orginal.TrailerLink;
            copySeason.InternetLink = orginal.InternetLink;
            foreach (SerieEpisode origEpisode in orginal.Episodes) {
                SerieEpisode cloneEp = CloneEpisode (origEpisode);
                cloneEp.ParentSeason = copySeason;
                origEpisode.ParentSeason = copySeason;
                copySeason.Episodes.Add (cloneEp);
            }
            return copySeason;
        }
        public SerieSeason CloneSeasonOnlyInfo (SerieSeason orginal) {
            SerieSeason copySeason = new SerieSeason ();
            copySeason.Name = orginal.Name;
            copySeason.ID = orginal.ID;
            copySeason.IsViewed = orginal.IsViewed;
            copySeason.TrailerLink = orginal.TrailerLink;
            copySeason.InternetLink = orginal.InternetLink;
            return copySeason;
        }
        public SerieEpisode CloneEpisode (SerieEpisode orginal) {
            SerieEpisode copyEpisode = new SerieEpisode ();
            copyEpisode.Name = orginal.Name;
            copyEpisode.OrigName = orginal.OrigName;
            copyEpisode.AddTime = orginal.AddTime;
            copyEpisode.AirDate = orginal.AirDate;
            copyEpisode.AspectRatio = orginal.AspectRatio;
            copyEpisode.AudioList = CloneAudioList (orginal.AudioList);
            copyEpisode.Bitrate = orginal.Bitrate;            
            copyEpisode.Hdd = CloneHDD (orginal.Hdd);
            copyEpisode.Height = orginal.Height;
            copyEpisode.ID = orginal.ID;
            copyEpisode.InternetLink = orginal.InternetLink;
            copyEpisode.IsViewed = orginal.IsViewed;            
            copyEpisode.Runtime = orginal.Runtime;
            copyEpisode.Size = orginal.Size;
            copyEpisode.SubtitleLanguageList = CloneLangList (orginal.SubtitleLanguageList);
            copyEpisode.Summary = orginal.Summary;
            copyEpisode.TrailerLink = orginal.TrailerLink;
            copyEpisode.Width = orginal.Width;
            copyEpisode.Version = orginal.Version;
            return copyEpisode;
        }

        public WishSerie CloneWishSerieWithSeason (WishSerie orginal) {
            WishSerie clone = new WishSerie ();
            clone.ID = orginal.ID;
            clone.Name = orginal.Name;
            clone.OrigName = orginal.OrigName;
            clone.TrailerLink = orginal.TrailerLink;
            clone.InternetLink = orginal.InternetLink;
            clone.Summary = orginal.Summary;
            clone.Rating = orginal.Rating;
            clone.Actors = ClonePersonList (orginal.Actors);
            clone.Directors = ClonePersonList (orginal.Directors);
            clone.Genres = CloneGenreList (orginal.Genres);
            foreach (WishSerieSeason tempSeason in orginal.WishSeasons) {
                clone.WishSeasons.Add (CloneWishSeasonWithEpisodes (tempSeason));
                tempSeason.ParentWishSerie = clone;
            }
            return clone;
        }
        public WishSerie CloneWishSerieonlyinfo (WishSerie orginal) {
            WishSerie clone = new WishSerie ();
            clone.ID = orginal.ID;
            clone.Name = orginal.Name;
            clone.OrigName = orginal.OrigName;
            clone.TrailerLink = orginal.TrailerLink;
            clone.InternetLink = orginal.InternetLink;
            clone.Summary = orginal.Summary;
            clone.Rating = orginal.Rating;
            clone.Actors = ClonePersonList (orginal.Actors);
            clone.Directors = ClonePersonList (orginal.Directors);
            clone.Genres = CloneGenreList (orginal.Genres);
            return clone;
        }
        public WishSerieSeason CloneWishSeasonWithEpisodes (WishSerieSeason orginal) {
            WishSerieSeason clone = new WishSerieSeason ();
            clone.ID = orginal.ID;
            clone.Name = orginal.Name;
            clone.InternetLink = orginal.InternetLink;
            clone.TrailerLink = orginal.TrailerLink;
            foreach (WishSerieEpisode tempEpisode in orginal.WishEpisodes) {
                clone.WishEpisodes.Add (CloneWishEpisode (tempEpisode));
                tempEpisode.ParentWishSeason = clone;
            }
            return clone;
        }
        public WishSerieSeason CloneWishSeasonOnlyInfo (WishSerieSeason orginal) {
            WishSerieSeason clone = new WishSerieSeason ();
            clone.ID = orginal.ID;
            clone.Name = orginal.Name;
            clone.InternetLink = orginal.InternetLink;
            clone.TrailerLink = orginal.TrailerLink;
            return clone;
        }
        public WishSerieEpisode CloneWishEpisode (WishSerieEpisode orginal) {
            WishSerieEpisode clone = new WishSerieEpisode ();
            clone.ID = orginal.ID;
            clone.Name = orginal.Name;
            clone.OrigName = orginal.OrigName;
            clone.InternetLink = orginal.InternetLink;
            clone.TrailerLink = orginal.TrailerLink;
            clone.Summary = orginal.Summary;
            clone.AirDate = orginal.AirDate;
            return clone;
        }

        public ObservableCollection<Language> CloneLangList (ObservableCollection<Language> origList) {
            ObservableCollection<Language> copyList = new ObservableCollection<Language> ();
            if (origList == null)
                return copyList;
            foreach (Language tempLang in origList) {
                Language newLang = new Language ();
                newLang.ID = tempLang.ID;
                newLang.Name = tempLang.Name;
                copyList.Add (newLang);
            }
            return copyList;
        }
        public ObservableCollection<Person> ClonePersonList (ObservableCollection<Person> origList) {            
            ObservableCollection<Person> copyList = new ObservableCollection<Person> ();
            if (origList == null)
                return copyList;
            foreach (Person tempPerson in origList) {
                Person clone = ClonePerson (tempPerson);
                copyList.Add (clone);
            }
            return copyList;
        }
        public ObservableCollection<Genre> CloneGenreList (ObservableCollection<Genre> origList) {
            ObservableCollection<Genre> copyList = new ObservableCollection<Genre> ();
            if (origList == null)
                return copyList;
            foreach (Genre tempGenre in origList) {
                Genre newGenre = new Genre ();
                newGenre.Name = tempGenre.Name;
                newGenre.ID = tempGenre.ID;
                copyList.Add (newGenre);
            }
            return copyList;
        }
        public ObservableCollection<Audio> CloneAudioList (ObservableCollection<Audio> orgList) {
            ObservableCollection<Audio> copyList = new ObservableCollection<Audio> ();
            if (orgList == null)
                return copyList;
            foreach (Audio tempAudio in orgList) {
                Audio newAudio = new Audio ();
                newAudio.ID = tempAudio.ID;
                newAudio.Format = tempAudio.Format;
                Language newLang = new Language ();
                newLang.Name = tempAudio.Language.Name;
                newAudio.Language = newLang;
                newAudio.Channels = tempAudio.Channels;                
                copyList.Add (newAudio);
            }
            return copyList;
        }
        public HDD CloneHDD (HDD orgHDD) {
            HDD clone = new HDD ();
            if (orgHDD == null)
                return null;
            clone.Name = orgHDD.Name;
            clone.Capacity = orgHDD.Capacity;
            clone.FreeSpace = orgHDD.FreeSpace;
            clone.ID = orgHDD.ID;
            clone.PurchaseDate = orgHDD.PurchaseDate;
            clone.WarrantyLength = orgHDD.WarrantyLength;
            return clone;
        }
        public Person ClonePerson (Person orgPerson) {
            Person clonePerson = new Person (orgPerson.Type);
            clonePerson.Name = orgPerson.Name;
            clonePerson.ID = orgPerson.ID;
            clonePerson.DateOfBirth = orgPerson.DateOfBirth;
            return clonePerson;
        }
        public Camera CloneCamera (Camera orgCamera) {
            Camera clone = new Camera ();
            if (orgCamera == null)
                return clone;
            clone.ID = orgCamera.ID;
            clone.Name = orgCamera.Name;
            clone.PurchaseDate = orgCamera.PurchaseDate;
            clone.WarrantyLengt = orgCamera.WarrantyLengt;
            return clone;
        }
        public HomeVideo CloneHomeVideo (HomeVideo homeVideo) {
            HomeVideo clone = new HomeVideo ();
            if (homeVideo == null)
                return clone;
            clone.ID = homeVideo.ID;
            clone.AddTime = homeVideo.AddTime;
            clone.AspectRatio = homeVideo.AspectRatio;
            clone.AudioList = CloneAudioList (homeVideo.AudioList);
            clone.Bitrate = homeVideo.Bitrate;
            clone.Camermans = ClonePersonList (homeVideo.Camermans);
            clone.VideoCategory = homeVideo.VideoCategory;
            clone.FilmDate = homeVideo.FilmDate;
            clone.FilmingCamera = homeVideo.FilmingCamera;
            clone.Hdd = CloneHDD (homeVideo.Hdd);
            clone.Height = homeVideo.Height;
            clone.IsViewed = homeVideo.IsViewed;
            clone.Location = homeVideo.Location;
            clone.Name = homeVideo.Name;
            clone.OrigName = homeVideo.OrigName;
            clone.PersonsInVideo = ClonePersonList (homeVideo.PersonsInVideo);
            clone.Runtime = homeVideo.Runtime;
            clone.Size = homeVideo.Size;
            clone.SubtitleLanguageList = CloneLangList (homeVideo.SubtitleLanguageList);
            clone.Summary = homeVideo.Summary;
            clone.Width = homeVideo.Width;
            return clone;
        }
    }
}
