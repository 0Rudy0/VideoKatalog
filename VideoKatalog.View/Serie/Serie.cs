using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class Serie: INotifyPropertyChanged {
        private int id;
        private string name = "";
        private string origName = "";        
        private decimal rating = 0;
        private string internetLink = "";
        private string trailerLink = "";
        private string summary = "";
        private bool isViewed = false;

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
        public decimal Rating {
            get {
                return this.rating;
            }
            set {
                this.rating = value;
                NotifyPropertyChanged ("Rating");
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
        public string Summary {
            get {
                return this.summary;
            }
            set {
                this.summary = value;
                NotifyPropertyChanged ("Summary");
            }
        }
        public bool IsViewed {
            get {
                return this.isViewed;
            }
            set {
                this.isViewed = value;
                NotifyPropertyChanged ("IsViewed");
            }
        }

        
        public ObservableCollection<Genre> Genres {
            get;
            set;
        }
        public ObservableCollection<Person> Directors {
            get;
            set;
        }
        public ObservableCollection<Person> Actors {
            get;
            set;
        }
               
        public ObservableCollection<SerieSeason> Seasons {
            get;
            set;
        }

        public Serie () {
            ID = new int ();
            Genres = new ObservableCollection<Genre> ();
            Directors = new ObservableCollection<Person> ();
            Actors = new ObservableCollection<Person> ();
            Seasons = new ObservableCollection<SerieSeason> ();
        }

        public override string ToString () {
            return this.Name;
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
    public class SerieComparerByName : IEqualityComparer<Serie> {
        public bool Equals (Serie x, Serie y) {
            if (x.Name == y.Name)
                return true;
            else
                return false;
        }

        public int GetHashCode (Serie obj) {
            return obj.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
        }
    }
    public class SerieComparerByID : IEqualityComparer<Serie> {
        public bool Equals (Serie x, Serie y) {
            if (x.ID == y.ID && x.Name == y.Name)
                return true;
            else
                return false;
        }

        public int GetHashCode (Serie obj) {
            return obj.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
        }
    }
}
