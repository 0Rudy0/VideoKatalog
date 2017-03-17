using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public partial class HomeVideo : Video  {       

        string location;       
        Camera camera;        
        DateTime filmDate;
        Category videoCategory;

        public string Location {
            get {
                return this.location;
            }
            set {
                this.location = value;
                NotifyPropertyChanged ("Location");
            }
        }
        public Camera FilmingCamera {
            get {
                return this.camera;
            }
            set {
                this.camera = value;
                NotifyPropertyChanged ("FilmingCamera");
            }
        }        
        public Category VideoCategory {
            get {
                return this.videoCategory;
            }
            set {
                this.videoCategory = value;
                NotifyPropertyChanged ("VideoCategory");
            }
        }
        public DateTime FilmDate {
            get {
                return this.filmDate;
            }
            set {
                this.filmDate = value;
                NotifyPropertyChanged ("FilmDate");
            }
        }
        public ObservableCollection<Person> Camermans {
            get;
            set;
        }
        public ObservableCollection<Person> PersonsInVideo {
            get;
            set;
        }

        public HomeVideo () {
            Camermans = new ObservableCollection<Person> ();
            PersonsInVideo = new ObservableCollection<Person> ();
            AudioList = new ObservableCollection<Audio> ();
            SubtitleLanguageList = new ObservableCollection<Language> ();
        }
        public override string ToString () {
            return this.OrigName;
        }
        
    }
}
