using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

/*public enum PersonType {
    zaKucniVideo,
    zaOstalo
}*/

namespace Video_katalog {
    public class Person {
        public int ID {
            get;
            set;
        }       
        public PersonType Type {
            get;
            set;
        }
        public string Name {
            get;
            set;
        }
        public DateTime DateOfBirth {
            get;
            set;
        }
        
        public Person (PersonType type) {
            if (type.ID == 2) {
                this.Type = type;
                this.DateOfBirth = new DateTime (1950, 1, 1);
            }
            else if (type.ID == 1) {
                this.Type = type;
                this.DateOfBirth = new DateTime (1950, 1, 1);
            }
        }
        

        public override string ToString () {
            return HttpUtility.HtmlDecode (this.Name);
        }
    }

    public class PersonEqualityComparerByName: IEqualityComparer<Person> {
        public bool Equals (Person x, Person y) {
            if (x.Name == y.Name) {
                return true;
            }
            else {
                return false;
            }
        }
        public int GetHashCode (Person obj) {
            return obj.Name.Length;
        }
    }
    public class PersonEqualityComparerByID : IEqualityComparer<Person> {
        public bool Equals (Person x, Person y) {
            if (x.ID == y.ID && x.Name == y.Name) {
                return true;
            }
            else {
                return false;
            }
        }
        public int GetHashCode (Person obj) {
            return obj.Name.Length;
        }
    }

    public class PersonType {
        int id;
        string name;
        
        public PersonType (int id) {
            if (id == 1) {
                this.id = 1;
                this.name = "za kućni video";
            }
            else if (id == 2) {
                this.id = 2;
                this.name = "za filmove/serije";
            }
        }

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
    }
}
