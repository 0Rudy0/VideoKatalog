using System;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MediaInfoLib;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public partial class WishMovie: INotifyPropertyChanged {
        private int id;
        private string name;
        private string origName;
        private int year;
        private decimal rating;        
        private float budget;
        private float earnings;
        private string summary;
        private string internetLink;
        private string trailerLink;
        private DateTime releaseDate;

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
                NotifyPropertyChanged ("ID");
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
        public int Year {
            get {
                return this.year;
            }
            set {
                this.year = value;
                NotifyPropertyChanged ("Year");
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
        public float Budget {
            get{
                return this.budget;
            }
            set {
                this.budget = value;
                NotifyPropertyChanged ("Budget");
            }
        }
        public float Earnings {
            get {
                return this.earnings;
            }
            set {
                this.earnings = value;
                NotifyPropertyChanged ("Earnings");
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
        public DateTime ReleaseDate {
            get {
                return this.releaseDate;
            }
            set {
                this.releaseDate = value;
                NotifyPropertyChanged ("ReleaseDate");
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

        public WishMovie () {                     
            Genres = new ObservableCollection<Genre> (); 
            Directors = new ObservableCollection<Person> ();
            Actors = new ObservableCollection<Person> ();            
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
