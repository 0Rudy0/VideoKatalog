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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Video_katalog {
    /// <summary>
    /// Interaction logic for HDDForm.xaml
    /// </summary>
    public partial class HDDsManagerForm: Window {

        ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();
        //DatabaseManager DatabaseManager = new DatabaseManager (); 
        CollectionView _hddView;
        public List<HDD> removedHDDs = new List<HDD> ();
        bool loading = true;

        public HDDsManagerForm () {
            InitializeComponent ();
            hddList = DatabaseManager.FetchHDDList ();
            _hddView = (CollectionView) CollectionViewSource.GetDefaultView (hddList);
            _hddView.MoveCurrentToPosition (0);  
            loading = false;
            this.DataContext = _hddView;            
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            SortDescription sortByName = new SortDescription ("Name", ListSortDirection.Ascending);
            _hddView.SortDescriptions.Add (sortByName);
            _hddView.MoveCurrentToPosition (0);
        }

        private void hddComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {            
            if (loading == false) {
                HDD selectedHDD = (HDD) _hddView.CurrentItem;
                if (selectedHDD == null)
                    return;
                int fullPercentage = (int) ((((float)selectedHDD.Capacity - selectedHDD.FreeSpace) / selectedHDD.Capacity) * 100);
                this.freeSpacePercentage.Text = fullPercentage.ToString () + " %";
                this.filledHddProgressBar.Value = fullPercentage;

                if (selectedHDD.PurchaseDate.AddMonths (selectedHDD.WarrantyLength) > DateTime.Now) {
                    warrantyValidTB.Text = "JOŠ VRIJEDI";
                    warrantyValidTB.Foreground = new SolidColorBrush (Color.FromRgb (29, 141, 13));
                }
                else {
                    warrantyValidTB.Text = "ISTEKLA";
                    warrantyValidTB.Foreground = new SolidColorBrush (Color.FromRgb (226, 7, 7));
                }

                long sizeBytes = (long) selectedHDD.Capacity;
                double size = sizeBytes / 1000;
                string sizeString = size.ToString ();
                if (sizeString.Length < 4) {
                    this.capacityTB.Text = size.ToString ("0.0") + " KB";
                    return;
                }
                size = size / 1000;
                sizeString = size.ToString (".0");
                if (sizeString.Length < 4) {
                    this.capacityTB.Text = size.ToString ("0.0") + " MB";
                    return;
                }
                size = size / 1000;
                sizeString = size.ToString ();
                if (sizeString.Length < 4) {
                    this.capacityTB.Text = size.ToString ("0.0") + " GB";
                    return;
                }
                size = size / 1000;
                sizeString = size.ToString ();
                this.capacityTB.Text = size.ToString ("0.0") + " TB";
                return;               
            }
        }
                
        private void addNewHddBTN_Click (object sender, RoutedEventArgs e) {
            HDDForm newHDD = new HDDForm ();
            newHDD.Owner = this;
            newHDD.ShowDialog ();
            if (newHDD.accepted) {
                DatabaseManager.InsertHDD (newHDD.currHDD);
                //DatabaseManagerMySql.InsertHDD (newHDD.currHDD);
                hddList.Add (newHDD.currHDD);
            }
        }
        private void editHddBTN_Click (object sender, RoutedEventArgs e) {
            HDDForm editHDD = new HDDForm ((HDD) _hddView.CurrentItem);
            editHDD.Owner = this;
            editHDD.ShowDialog ();
            if (editHDD.accepted) {
                DatabaseManager.UpdateHDD (editHDD.currHDD);
                //DatabaseManagerMySql.UpdateHDD (editHDD.currHDD);
                int listIndex = hddList.IndexOf ((HDD) _hddView.CurrentItem);
                hddList[listIndex] = editHDD.currHDD;
            }
        }
        private void removeHddBtn_Click (object sender, RoutedEventArgs e) {
            HDD selectedHDD = (HDD) _hddView.CurrentItem;
            if (Xceed.Wpf.Toolkit.MessageBox.Show ( 
                    string.Format ("Brisanje HDDa: {0}\r\nJeste li sigurni? Pobrisati ce se i sve iz kataloga što se nalazi na HDDu {1}.", selectedHDD.Name, selectedHDD.Name), 
                    "Potvrda brisanja", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                DatabaseManager.DeleteHDD (selectedHDD);
                //DatabaseManagerMySql.DeleteHDD (selectedHDD);
                hddList.Remove (selectedHDD);
                _hddView.MoveCurrentToPrevious ();
                _hddView.MoveCurrentToNext ();
                removedHDDs.Add (selectedHDD);
            }

        }
    }
}
