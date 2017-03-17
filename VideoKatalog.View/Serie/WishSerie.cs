using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class WishSerie: INotifyPropertyChanged {
        private int id;
        private int regularSerieID;
        private string name = "";
        private string origName = "";
        private decimal rating = 0;
        private string internetLink = "";
        private string trailerLink = "";
        private string summary = "";

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
        public int RegularSerieID {
            get {
                return this.regularSerieID;
            }
            set {
                this.regularSerieID = value;
                NotifyPropertyChanged ("RegularSerieID");
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
        public ObservableCollection<WishSerieSeason> WishSeasons {
            get;
            set;
        }

        public WishSerie () {
            ID = new int ();
            Genres = new ObservableCollection<Genre> ();
            Directors = new ObservableCollection<Person> ();
            Actors = new ObservableCollection<Person> ();
            WishSeasons = new ObservableCollection<WishSerieSeason> ();
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
}
