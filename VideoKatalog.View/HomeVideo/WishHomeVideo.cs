using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class WishHomeVideo : INotifyPropertyChanged {
        private int id;
        private string name;
        private string location;
        private DateTime filmDate;
        private string comment;
        private Camera camera;
        private Category category;

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
                NotifyPropertyChanged ("Name");
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
        public string Location {
            get {
                return this.location;
            }
            set {
                this.location = value;
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
        public string Comment {
            get {
                return this.comment;
            }
            set {
                this.comment = value;
                NotifyPropertyChanged ("Comment");
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
                return this.category;
            }
            set {
                this.category = value;
                NotifyPropertyChanged ("VideoCategory");
            }
        }
        public ObservableCollection<Person> PersonsInVideo {
            get;
            set;
        }
        public ObservableCollection<Person> Camermans {
            get;
            set;
        }

        public WishHomeVideo () {
            Camermans = new ObservableCollection<Person> ();
            PersonsInVideo = new ObservableCollection<Person> ();
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
