using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    public class HDD : IEqualityComparer<HDD>, INotifyPropertyChanged {
        int id;
        string name;
        DateTime purchaseDate;
        int warrantyMonths;
        long capacity = 0;
        long freeSpace = 0;

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
        public int WarrantyLength {
            get {
                return this.warrantyMonths;
            }
            set {
                this.warrantyMonths = value;
                NotifyPropertyChanged ("WarrantyLength");
            }
        }
        public long Capacity {
            get {
                return this.capacity;
            }
            set {
                this.capacity = value;
                NotifyPropertyChanged ("Capacity");
            }
        }
        public long FreeSpace {
            get {
                return this.freeSpace;
            }
            set {
                this.freeSpace = value;
                NotifyPropertyChanged ("FreeSpace");
            }
        }

        public HDD () {
        }
        public HDD (string name, DateTime purchaseDate, int warrantyMonths, long capacity) {
            this.name = name;
            this.purchaseDate = purchaseDate;
            this.WarrantyLength = warrantyMonths;
            this.capacity = capacity;
            this.freeSpace = capacity;
        }
        public void FreeSpaceIncrease (long size) {
            this.freeSpace += size;
        }
        public void FreeSpaceDecrease (long size) {
            this.freeSpace -= size;
        }
        public void ChangeCapacity (long size) {
            long sizeDifference = Math.Abs (size - this.capacity);
            long selectedSize = this.capacity - this.freeSpace;
            if (size < selectedSize)
                throw new Exception ();
            else if (size > this.capacity)
                FreeSpaceIncrease (sizeDifference);
            else if (size < this.capacity)
                FreeSpaceDecrease (sizeDifference);
            this.capacity = size;
            this.freeSpace = size;
        }

        public override string ToString () {
            return this.name;
        }       

        #region IEqualityComparer<HDD> Members

        public bool Equals (HDD x, HDD y) {
            if (x == null || y == null)
                return false;
            if (x.name == y.name && x.purchaseDate == y.purchaseDate)
                return true;
            return false;
        }

        public int GetHashCode (HDD obj) {
            throw new NotImplementedException ();
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged (String info) {
            if (PropertyChanged != null) {
                PropertyChanged (this, new PropertyChangedEventArgs (info));
            }
        }

        #endregion
    }

    public class HDDComparerByName : IEqualityComparer<HDD> {
        public bool Equals (HDD x, HDD y) {
            if (x == null || y == null)
                return false;
            if (x.Name == y.Name && x.PurchaseDate == y.PurchaseDate)
                return true;
            return false;
        }
        public int GetHashCode (HDD obj) {
            return obj.Name.Length;
        }
    }
    public class HDDComparerByID : IEqualityComparer<HDD> {
        public bool Equals (HDD x, HDD y) {
            if (x == null || y == null)
                return false;
            if (x.ID == y.ID && x.Name == y.Name)
                return true;
            return false;
        }
        public int GetHashCode (HDD obj) {
            return obj.Name.Length;
        }
    }
}
