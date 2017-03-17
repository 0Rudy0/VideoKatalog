using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class WishSerieEpisode : INotifyPropertyChanged {
        private int id;
        private string name = "";
        private string origName = "";
        private string summary = "";
        private string internetLink = "";
        private string trailerLink = "";
        private DateTime airDate;        
        private WishSerieSeason parentWishSeason;

        public int ID {
            get {
                return this.id;
            }
            set {
                if (value != this.id) {
                    this.id = value;
                    NotifyPropertyChanged ("ID");
                }
            }
        }
        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
                NotifyPropertyChanged ("Name");
            }
        }
        public string OrigName {
            get {
                return this.origName;
            }
            set {
                this.origName = value;
                NotifyPropertyChanged ("OrigName");
            }
        }
        public string Summary {
            get {
                return this.summary;
            }
            set {
                this.summary = value;
                NotifyPropertyChanged ("Summary");
            }
        }
        public string InternetLink {
            get {
                return this.internetLink;
            }
            set {
                this.internetLink = value;
                NotifyPropertyChanged ("InternetLink");
            }
        }
        public string TrailerLink {
            get {
                return this.trailerLink;
            }
            set {
                this.trailerLink = value;
                NotifyPropertyChanged ("TrailerLink");
            }
        }
        public DateTime AirDate {
            get {
                return this.airDate;
            }
            set {
                this.airDate = value;
                NotifyPropertyChanged ("AirDate");
            }
        }        
        public WishSerieSeason ParentWishSeason {
            get {
                return this.parentWishSeason;
            }
            set {
                this.parentWishSeason = value;
                NotifyPropertyChanged ("ParentWishSeason");
            }
        }

        public WishSerieEpisode () {
        }
        public override string ToString () {
            return this.name;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged (String info) {
            if (PropertyChanged != null) {
                PropertyChanged (this, new PropertyChangedEventArgs (info));
            }
        }

        #endregion
    }
}
