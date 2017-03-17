using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class SerieEpisode:Video {        
        private DateTime airDate;
        private SerieSeason parentSeason;  
       
        public DateTime AirDate {
            get {
                return this.airDate;
            }
            set {
                this.airDate = value;
                NotifyPropertyChanged ("AirDate");
            }
        }
        public SerieSeason ParentSeason {
            get {
                return this.parentSeason;
            }
            set {
                this.parentSeason = value;
                NotifyPropertyChanged ("ParentSeason");
            }
        }

        public SerieEpisode () {
            AirDate = new DateTime (1800, 1, 1);
            this.InternetLink = "";
            this.Summary = "";
            this.TrailerLink = "";            
            SubtitleLanguageList = new ObservableCollection<Language> ();
            AudioList = new ObservableCollection<Audio> ();
        }

        public override string ToString () {
            return this.Name;
        }
    }
    public class EpisodeComparerByName : IEqualityComparer<SerieEpisode> {
        public bool Equals (SerieEpisode x, SerieEpisode y) {
            if (x.Name == y.Name)
                return true;
            else
                return false;
        }
        public int GetHashCode (SerieEpisode obj) {
            if (obj.ParentSeason == null)
                return obj.AirDate.Year + obj.AirDate.Month;
            if (obj.ParentSeason.ParentSerie == null)
                return obj.AirDate.Year + obj.AirDate.Month;
            return obj.ParentSeason.ParentSerie.ID + obj.ParentSeason.ID;
        }
    }
    public class EpisodeComparerByID : IEqualityComparer<SerieEpisode> {
        public bool Equals (SerieEpisode x, SerieEpisode y) {
            if (x.ID == y.ID && x.Name == y.Name)
                return true;
            else
                return false;
        }
        public int GetHashCode (SerieEpisode obj) {
            if (obj.ParentSeason == null)
                return obj.AirDate.Year + obj.AirDate.Month;
            if (obj.ParentSeason.ParentSerie == null)
                return obj.AirDate.Year + obj.AirDate.Month;
            return obj.ParentSeason.ParentSerie.ID + obj.ParentSeason.ID;
        }
    }
}
