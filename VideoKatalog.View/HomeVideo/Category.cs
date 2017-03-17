using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Video_katalog {
    public class Category: INotifyPropertyChanged {
        int id;
        string name;

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

        public Category () {
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
