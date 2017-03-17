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
    /// Interaction logic for CameraForm.xaml
    /// </summary>
    public partial class CameraForm: Window {
        public bool accepted = false;
        public Camera currCamera = new Camera ();
        Cloner cloner = new Cloner ();
        public CameraForm () {
            InitializeComponent ();
            this.DataContext = currCamera;
        }
        public CameraForm (Camera cameraToEdit) {
            InitializeComponent ();
            currCamera = cloner.CloneCamera (cameraToEdit);
            foreach (ComboBoxItem item in warrantyComboBox.Items) {
                if (item.Content.ToString ().Contains (currCamera.WarrantyLengt.ToString ())) {
                    item.IsSelected = true;
                }
                else
                    item.IsSelected = false;
            }
            this.DataContext = currCamera;
            this.datePicker.SelectedDate = currCamera.PurchaseDate;
        }

        private void acceptedButton_Click (object sender, RoutedEventArgs e) {
            if (cameraNameTextBox.Text.Length < 3) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Duljina naziva kamere mora biti barem 3 znaka");
                return;
            }           
            if (datePicker.SelectedDate == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran datum kupnje");
                return;
            }
            currCamera.PurchaseDate = (DateTime) datePicker.SelectedDate;
            foreach (ComboBoxItem item in warrantyComboBox.Items) {
                if (item.IsSelected) {
                    string warranty = "";
                    foreach (char c in item.Content.ToString ()) {
                        if (char.IsNumber (c))
                            warranty += c.ToString ();
                    }
                    currCamera.WarrantyLengt = int.Parse (warranty);
                    break;
                }
            }
            accepted = true;
            this.Close ();

        }
    }
}
