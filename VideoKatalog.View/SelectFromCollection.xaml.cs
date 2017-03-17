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

namespace Video_katalog {
    /// <summary>
    /// Interaction logic for SelectFromCollection.xaml
    /// </summary>
    public partial class SelectFromCollection : Window {
        public object selectedObject = null;
        public SelectFromCollection (ObservableCollection<HDD> hddList, string windowTitle) {
            InitializeComponent ();
            this.Title = windowTitle;
            foreach (object obj in hddList)
                objectComboBox.Items.Add (obj);
        }
        public SelectFromCollection (ObservableCollection<Selection> selectionList, string windowTitle) {
            InitializeComponent ();
            this.Title = windowTitle;
            foreach (object obj in selectionList)
                objectComboBox.Items.Add (obj);
        }

        private void ok_Click (object sender, RoutedEventArgs e) {
            selectedObject = objectComboBox.SelectedItem;
            this.Close ();

        }

        private void cancel_Click (object sender, RoutedEventArgs e) {
            selectedObject = null;
            this.Close ();

        }
    }
}
