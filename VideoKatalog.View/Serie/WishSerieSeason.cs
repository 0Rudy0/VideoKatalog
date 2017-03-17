using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class WishSerieSeason :INotifyPropertyChanged {
        int id;
        private int regularSeasonID;        
        WishSerie parentWishSerie;
        string name;
        string trailerLink;
        string internetLink;

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
                NotifyPropertyChanged ("ID");
            }
        }
        public int RegularSeasonID {
            get {
                return this.regularSeasonID;
            }
            set {
                this.regularSeasonID = value;
                NotifyPropertyChanged ("RegularSeasonID");
            }
        }        
        public WishSerie ParentWishSerie {
            get {
                return this.parentWishSerie;
            }
            set {
                this.parentWishSerie = value;
                NotifyPropertyChanged ("ParentWishSerie");
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
        public string TrailerLink {
            get {
                return this.trailerLink;
            }
            set {
                this.trailerLink = value;
                NotifyPropertyChanged ("TrailerLink");
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
        public ObservableCollection<WishSerieEpisode> WishEpisodes {
            get;
            set;
        }

        public WishSerieSeason () {
            this.WishEpisodes = new ObservableCollection<WishSerieEpisode> ();
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
