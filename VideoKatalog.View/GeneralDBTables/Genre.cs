using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {    
    public class Genre {
        int id;
        string name;

        public int ID {
            get {
                return this.id;
            }
            set {
                this.id = value;
            }
        }
        public string Name {
            get {
                return this.name;
            }
            set {
                this.name = value;
            }
        }

        public List<string> genresEng = new List<string> ();
        public List<string> genresCro = new List<string> ();

        public Genre () {
            FillCroatianGenres ();
            FillEnglishGenres ();
        }

        public Genre (string genreName) {
            this.name = genreName;
            FillEnglishGenres ();
            FillCroatianGenres ();
        }
        void FillCroatianGenres () {
            genresCro.Add ("Akcija");
            genresCro.Add ("Avantura");
            genresCro.Add ("Animacija");
            genresCro.Add ("Biografija");
            genresCro.Add ("Komedija");
            genresCro.Add ("Krimi");
            genresCro.Add ("Dokumentarni");
            genresCro.Add ("Drama");
            genresCro.Add ("Obiteljski");
            genresCro.Add ("Fantastika");
            genresCro.Add ("Crno-bijeli");
            genresCro.Add ("Povijesni");
            genresCro.Add ("Horor");
            genresCro.Add ("Muzički");
            genresCro.Add ("Pjevan");
            genresCro.Add ("Misterija");
            genresCro.Add ("Romantika");
            genresCro.Add ("Znanstvena fantastika");
            genresCro.Add ("Sportski");
            genresCro.Add ("Triler");
            genresCro.Add ("Ratni");
            genresCro.Add ("Vestern");
        }

        void FillEnglishGenres () {
            genresEng.Add ("Action");
            genresEng.Add ("Adventure");
            genresEng.Add ("Animation");
            genresEng.Add ("Biography");
            genresEng.Add ("Comedy");
            genresEng.Add ("Crime");
            genresEng.Add ("Documentary");
            genresEng.Add ("Drama");
            genresEng.Add ("Family");
            genresEng.Add ("Fantasy");
            genresEng.Add ("Film-Noir");
            genresEng.Add ("History");
            genresEng.Add ("Horror");
            genresEng.Add ("Music");
            genresEng.Add ("Musical");
            genresEng.Add ("Mystery");
            genresEng.Add ("Romance");
            genresEng.Add ("Sci-Fi");
            genresEng.Add ("Sport");
            genresEng.Add ("Thriller");
            genresEng.Add ("War");
            genresEng.Add ("Western");
        }

        public override string ToString () {
            return this.name;
        }
    }

    class GenreEqualityComparer : IEqualityComparer <Genre> {

        #region IEqualityComparer<Genre> Members

        public bool Equals (Genre x, Genre y) {
            if (x.Name == y.Name) {
                return true;
            }
            return false;
        }

        public int GetHashCode (Genre obj) {
            throw new NotImplementedException ();
        }

        #endregion
    }
}
