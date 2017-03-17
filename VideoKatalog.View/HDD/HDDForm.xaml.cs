using System;
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
using System.Windows.Shapes;

namespace Video_katalog {
    /// <summary>
    /// Interaction logic for HDDForm.xaml
    /// </summary>
    public partial class HDDForm: Window {
        public HDD currHDD = new HDD ();
        Cloner cloner = new Cloner ();
        public bool accepted = false;
        public bool isNew;

        public HDDForm () {
            InitializeComponent ();
            isNew = true;
        }
        public HDDForm (HDD passedHDD) {
            InitializeComponent ();
            currHDD = cloner.CloneHDD (passedHDD);
            this.hddName.Text = currHDD.Name;
            this.datePicker.SelectedDate = currHDD.PurchaseDate;
            foreach (ComboBoxItem item in warrantyComboBox.Items) {
                if (item.Content.ToString ().Contains (currHDD.WarrantyLength.ToString ())) {
                    item.IsSelected = true;
                }
            }
            float capacity = (float) (currHDD.Capacity / 1000000000000.0); //dijelimo kao da je veličina u TB-ima
            if (capacity < 1) { //znaci da je veličina u GB
                capacity = (float) (currHDD.Capacity / 1000000000);
                this.sizeTextBox.Text = capacity.ToString ("0.");
                foreach (ComboBoxItem item in sizeTypeComboBox.Items) {
                    if (item.Content.ToString ().Contains ("GB")) {
                        item.IsSelected = true;
                    }
                    else
                        item.IsSelected = false;
                }
            }
            else {
                this.sizeTextBox.Text = capacity.ToString (".0");
                foreach (ComboBoxItem item in sizeTypeComboBox.Items) {
                    if (item.Content.ToString ().Contains ("TB")) {
                        item.IsSelected = true;
                    }
                    else
                        item.IsSelected = false;
                }
            }
            isNew = false;
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            datePicker.DisplayDateEnd = DateTime.Now;
            if (isNew)
                datePicker.SelectedDate = DateTime.Now;
        }

        private void acceptButton_Click (object sender, RoutedEventArgs e) {
            if (ValidateInsertedData ()) {
                accepted = true;
                this.Close ();
            }
        }

        bool ValidateInsertedData () {
            bool allOK = true;
            if (hddName.Text.Length > 8) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Duljina HDDa prevelika. Maksimalno 8 znakova", "Duljina naziva prevelika", MessageBoxButton.OK, MessageBoxImage.Warning);
                allOK = false;
            }
            else {
                currHDD.Name = hddName.Text;
            }
            try {
                double size = double.Parse (sizeTextBox.Text.Replace (".", ",")); //ako ne uspije parsirati baca exception
                long oldCapacity = 0;
                if (isNew == false) {
                    oldCapacity = currHDD.Capacity;
                    if (size < currHDD.Capacity - currHDD.FreeSpace) {
                        Xceed.Wpf.Toolkit.MessageBox.Show ("HDD sadrži više nego uneseni kapacitet.", "Unesen premali kapacitet", MessageBoxButton.OK, MessageBoxImage.Warning);
                        allOK = false;
                    }
                }

                foreach (ComboBoxItem item in sizeTypeComboBox.Items) {
                    if (item.IsSelected) {
                        if (item.Content.ToString () == "GB")
                            currHDD.Capacity = (long) (size * 1000000000);
                        else if (item.Content.ToString () == "TB")
                            currHDD.Capacity = (long) (size * 1000000000000);
                        else {
                            Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran tip kapaciteta (GB/TB)", "Nepravilan unos kapaciteta", MessageBoxButton.OK, MessageBoxImage.Warning);
                            allOK = false;
                        }
                    }
                }
                //postavi slobodan prostor. Ako je hard postojeci, osvježi kapacitet. Freespace i oldcapacity su 0 u slučaju novog harda
                currHDD.FreeSpace = currHDD.FreeSpace - oldCapacity + currHDD.Capacity;
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Kapacitet HDDa nije pravilan unos", "Nepravilan unos kapaciteta", MessageBoxButton.OK, MessageBoxImage.Warning);
                allOK = false;
            }
            if (datePicker.SelectedDate == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran datum kupnje", "Datum kupnje ne postoji", MessageBoxButton.OK, MessageBoxImage.Warning);
                allOK = false;
            }
            else {
                currHDD.PurchaseDate = (DateTime)datePicker.SelectedDate;
            }
            foreach (ComboBoxItem item in warrantyComboBox.Items) {
                if (item.IsSelected) {
                    string warranty = "";
                    foreach (char c in item.Content.ToString ())
                        if (char.IsNumber (c))
                            warranty += c.ToString ();
                    currHDD.WarrantyLength = int.Parse (warranty);
                    break;
                }
            }

            return allOK;
        }
    }
}
