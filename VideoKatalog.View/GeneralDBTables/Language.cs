using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Video_katalog {
    public class Language {
        public int ID {
            get;
            set;
        }
        public string Name {
            get;
            set;
        }
        public Language () {
        }

        public override string ToString () {
            return this.Name;
        }
    }

    public class LanguageComparerByName: IEqualityComparer<Language> {
        public bool Equals (Language x, Language y) {
            if (x.Name == y.Name) {
                return true;
            }
            else {
                return false;
            }
        }
        public int GetHashCode (Language obj) {
            return System.Convert.ToInt32 (obj.ToString ().Substring (0,1)) + System.Convert.ToInt32 (obj.ToString().Substring (1,2));
        }
    }
    public class LanguageComparerByID : IEqualityComparer<Language> {
        public bool Equals (Language x, Language y) {
            if (x.ID == y.ID && x.Name == y.Name) {
                return true;
            }
            else {
                return false;
            }
        }
        public int GetHashCode (Language obj) {
            return System.Convert.ToInt32 (obj.ToString ().Substring (0, 1)) + System.Convert.ToInt32 (obj.ToString ().Substring (1, 2));
        }
    }
}
