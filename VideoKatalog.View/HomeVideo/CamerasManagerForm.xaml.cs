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
    
    public partial class CamerasManagerForm: Window {

        ObservableCollection<Camera> cameraList;
        ICollectionView _cameraView;

        public CamerasManagerForm (ObservableCollection<Camera> cameraList) {
            InitializeComponent ();
            this.cameraList = cameraList;
            _cameraView = CollectionViewSource.GetDefaultView (this.cameraList);
            SortDescription byName = new SortDescription ("Name", ListSortDirection.Ascending);
            _cameraView.SortDescriptions.Add (byName);
            _cameraView.MoveCurrentToPosition (0);
            this.cameraComboBox.ItemsSource =  this.cameraList;
            this.DataContext = _cameraView;
        }

        private void addNewCameraBTN_Click (object sender, RoutedEventArgs e) {
            CameraForm newCameraForm = new CameraForm ();
            newCameraForm.Owner = this;
            newCameraForm.ShowDialog ();
            if (newCameraForm.accepted) {
                DatabaseManager.InsertCamera (newCameraForm.currCamera);
                cameraList.Add (newCameraForm.currCamera);
            }
        }
        private void removeCameraBtn_Click (object sender, RoutedEventArgs e) {
            Camera selected = cameraList.ElementAt (cameraComboBox.SelectedIndex);
            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje kamere: " + selected.Name + "\r\nJeste li sigurni?", 
                "Potvrda brisanja", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                DatabaseManager.DeleteCamera (selected);
                cameraList.Remove (selected);
            }
        }
        private void editCameraBTN_Click (object sender, RoutedEventArgs e) {
            Camera selected = (Camera) _cameraView.CurrentItem;
            if (selected == null) {
                MessageBox.Show ("Nije odabrana ni jedna kamera");
                return;
            }
            CameraForm editCameraForm = new CameraForm (selected);
            editCameraForm.Owner = this;
            editCameraForm.ShowDialog ();
            if (editCameraForm.accepted) {
                DatabaseManager.UpdateCamera (editCameraForm.currCamera);
                cameraList.Remove (selected);
                cameraList.Add (editCameraForm.currCamera);
                cameraComboBox.SelectedItem = editCameraForm.currCamera;
            }
        }
        private void cameraComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            Camera selected = (Camera) _cameraView.CurrentItem;
            if (selected == null)
                return;
            if (selected.PurchaseDate.AddMonths (selected.WarrantyLengt) > DateTime.Now) {
                this.warrantyValidTB.Text = "JOŠ VRIJEDI";
                warrantyValidTB.Foreground = new SolidColorBrush (Color.FromRgb (29, 141, 13));
            }
            else {
                this.warrantyValidTB.Text = "ISTEKLA";
                warrantyValidTB.Foreground = new SolidColorBrush (Color.FromRgb (226, 7, 7));
            }
        }
    }
}
