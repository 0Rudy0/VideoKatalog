using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class SerieSeason: INotifyPropertyChanged {
        private int id;
        private string name = "";        
        private string trailerLink = "";
        private string internetLink = "";
        private Serie parentSerie;
        private bool isViewed = false;

        public string InternetLink {
            get {
                return this.internetLink;
            }
            set {
                this.internetLink = value;
                NotifyPropertyChanged ("InternetLink");
            }
        }
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
        public string TrailerLink {
            get {
                return this.trailerLink;
            }
            set {
                this.trailerLink = value;
                NotifyPropertyChanged ("TrailerLink");
            }
        }
        public Serie ParentSerie {
            get {
                return this.parentSerie;
            }
            set {
                this.parentSerie = value;
                NotifyPropertyChanged ("ParentSerie");
            }
        }
        public ObservableCollection<SerieEpisode> Episodes;
        public ObservableCollection<WishSerieEpisode> WishEpisodes;
        public bool IsViewed {
            get {
                return this.isViewed;
            }
            set {
                this.isViewed = value;
                NotifyPropertyChanged ("IsViewed");
            }
        }

        public SerieSeason () {
            ID = new int ();
            this.Episodes = new ObservableCollection<SerieEpisode> ();
            this.WishEpisodes = new ObservableCollection<WishSerieEpisode> ();
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

    public class SeasonComparerByID : IEqualityComparer<SerieSeason> {
        public bool Equals (SerieSeason x, SerieSeason y) {
            if (x.ID == y.ID && x.Name == y.Name && x.ParentSerie.ID == y.ParentSerie.ID)
                return true;
            else
                return false;
        }

        public int GetHashCode (SerieSeason obj) {
            if (obj.ParentSerie == null) {
                string num = "";
                foreach (char c in obj.Name)
                    if (char.IsDigit (c))
                        num += c.ToString();
                if (num.Length > 0)
                    return 3 * int.Parse (num);
                else
                    return 0;
            }
            return obj.ParentSerie.ID;
        }
    }
}
