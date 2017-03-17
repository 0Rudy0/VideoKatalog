using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public static class FinderInCollection {
        //public staFinderInCollection () {
        //}

        public static Audio FindInAudioCollection (ObservableCollection<Audio> audioList, int audioID) {
            foreach (Audio tempAudio in audioList)
                if (tempAudio.ID == audioID)
                    return tempAudio;
            return null;
        }
        public static HDD FindInHDDCollection (ObservableCollection<HDD> hddList, int hddID) {
            foreach (HDD tempHdd in hddList) {
                if (hddID == tempHdd.ID) {
                    return tempHdd;
                }
            }
            return null;
        }
        public static HDD FindInHDDCollection (ObservableCollection<HDD> hddList, string hddName) {
            foreach (HDD tempHDD in hddList) {
                if (tempHDD.Name == hddName) {
                    return tempHDD;
                }
            }
            return null;
        }
        public static Person FindInPersonCollection (ObservableCollection<Person> personList, int personID) {
            foreach (Person tempPerson in personList) {
                if (personID == tempPerson.ID)
                    return tempPerson;
            }
            return null;
        }
        public static Language FindInLanguageCollection (ObservableCollection<Language> languageList, int langID) {
            foreach (Language tempLang in languageList) {
                if (langID == tempLang.ID)
                    return tempLang;
            }
            return null;
        }
        public static Genre FindInGenreCollection (ObservableCollection<Genre> genreList, int genreID) {
            foreach (Genre tempGenre in genreList) {
                if (tempGenre.ID == genreID) {
                    return tempGenre;
                }
            }
            return null;
        }
        public static Movie FindInMovieCollection (ObservableCollection<Movie> movieList, int movieID) {
            foreach (Movie tempMovie in movieList) {
                if (movieID == tempMovie.ID)
                    return tempMovie;
            }
            return null;
        }
        public static Serie FindInSerieCollection (ObservableCollection<Serie> serieList, int serieID) {
            foreach (Serie tempSerie in serieList) {
                if (tempSerie.ID == serieID)
                    return tempSerie;
            }
            return null;
        }
        public static SerieSeason FindInSerieSeasonCollection (ObservableCollection<SerieSeason> seasonList, int seasonID) {
            foreach (SerieSeason tempSeason in seasonList) {
                if (tempSeason.ID == seasonID)
                    return tempSeason;
            }
            return null;
        }
        public static SerieEpisode FindInSerieEpisodeCollection (ObservableCollection<SerieEpisode> episodeList, int episodeID) {
            foreach (SerieEpisode tempEpisode in episodeList) {
                if (tempEpisode.ID == episodeID)
                    return tempEpisode;
            }
            return null;
        }
        public static Category FindInCategoryCollection (ObservableCollection<Category> categoryList, int categoryID) {
            foreach (Category tempCat in categoryList)
                if (tempCat.ID == categoryID)
                    return tempCat;
            return null;
        }
        public static HomeVideo FindInHomeVideoCollection (ObservableCollection<HomeVideo> homeVideoList, int homeVideoID) {
            foreach (HomeVideo tempHV in homeVideoList)
                if (tempHV.ID == homeVideoID)
                    return tempHV;
            return null;
        }
        public static Camera FindInCameraCollection (ObservableCollection<Camera> cameraList, int cameraID) {
            foreach (Camera tempCamera in cameraList)
                if (tempCamera.ID == cameraID)
                    return tempCamera;
            return null;
        }
        public static WishMovie FindInWishMovieCollection (ObservableCollection<WishMovie> movieList, int movieID) {
            foreach (WishMovie tempMovie in movieList)
                if (tempMovie.ID == movieID)
                    return tempMovie;
            return null;
        }
        public static WishHomeVideo FindInWishHomeVideoCollection (ObservableCollection<WishHomeVideo> wishHVList, int wishID) {
            foreach (WishHomeVideo tempHV in wishHVList)
                if (tempHV.ID == wishID)
                    return tempHV;
            return null;
        }
        public static WishSerie FindInWishSerieCollection (ObservableCollection<WishSerie> wishList, int wishID) {
            foreach (WishSerie tempSerie in wishList)
                if (tempSerie.ID == wishID)
                    return tempSerie;
            return null;
        }
        public static WishSerieSeason FindInWishSerieSeasonCollection (ObservableCollection<WishSerieSeason> wishList, int wishID) {
            foreach (WishSerieSeason tempSeason in wishList)
                if (tempSeason.ID == wishID)
                    return tempSeason;
            return null;
        }
        public static WishSerieEpisode FindInWishSerieEpisodeCollection (ObservableCollection<WishSerieEpisode> wishList, int wishID) {
            foreach (WishSerieEpisode tempEpisode in wishList)
                if (tempEpisode.ID == wishID)
                    return tempEpisode;
            return null;
        }
        public static Selection FindInSelectionCollection (ObservableCollection<Selection> selectionList, int selectionID) {
            foreach (Selection tempSel in selectionList)
                if (tempSel.ID == selectionID)
                    return tempSel;
            return null;
        }
    }    
}
