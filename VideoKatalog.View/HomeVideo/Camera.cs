using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace Video_katalog {
    public class Camera : INotifyPropertyChanged {
        int id;
        string name;
        DateTime purchaseDate;
        int warrantyLength;

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
        public DateTime PurchaseDate {
            get {
                return this.purchaseDate;
            }
            set {
                this.purchaseDate = value;
                NotifyPropertyChanged ("PurchaseDate");
            }
        }
        public int WarrantyLengt {
            get {
                return this.warrantyLength;
            }
            set {
                this.warrantyLength = value;
                NotifyPropertyChanged ("WarrantyLengt");
            }
        }

        public Camera () {
            this.purchaseDate = new DateTime (2000, 1, 1);
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
