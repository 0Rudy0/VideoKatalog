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
    public class Movie : Video {        
        private int year = 0;
        private decimal rating = 0;
        private float budget = 0;
        private float earnings = 0;
        
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

        public Movie () {
            ID = new int ();            
            Genres = new ObservableCollection<Genre> (); 
            Directors = new ObservableCollection<Person> ();
            Actors = new ObservableCollection<Person> ();
            AudioList = new ObservableCollection<Audio> ();
            SubtitleLanguageList = new ObservableCollection<Language> ();
           
        }
        public void AddToDatabase () {
            DatabaseManagerMySql.InsertNewMovie (this);
        }
        public override string ToString () {
            return this.Name;
        }
    }

    #region IEqualityComparer<Movie> Members

    public class MovieComparerByNameYear: IEqualityComparer<Movie> {
        public bool Equals (Movie x, Movie y) {
            if (x.OrigName == y.OrigName && x.Year == y.Year)
                return true;
            else
                return false;
        }
        public int GetHashCode (Movie obj) {
            return obj.Year + obj.Name.Length;
        }
    }
    public class MovieComparerByID : IEqualityComparer<Movie> {
        public bool Equals (Movie x, Movie y) {
            if (x.ID == y.ID && x.Name == y.Name)
                return true;
            else
                return false;
        }
        public int GetHashCode (Movie obj) {
            return obj.Year + obj.Name.Length;
        }
    }

    #endregion

}
