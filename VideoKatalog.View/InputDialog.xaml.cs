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
    /// Interaction logic for InputDialog.xaml
    /// </summary>
    public partial class InputDialog: Window {
        public string inputString = null;
        public bool accepted = false;
        public InputDialog (string title) {
            InitializeComponent ();
            this.Title = title;
            inputTextBox.Focus ();
        }
        public InputDialog (string title, string defaultInputText) {
            InitializeComponent ();
            this.Title = title;
            this.inputTextBox.Text = defaultInputText;
            inputTextBox.Focus ();
        }

        private void okButton_Click (object sender, RoutedEventArgs e) {
            inputString = inputTextBox.Text;
            if (inputString.Length < 3) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Duljina mora biti barem 3 znaka", "Neispravan unos", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            accepted = true;
            this.Close ();
        }

        private void cancelButton_Click (object sender, RoutedEventArgs e) {
            accepted = false;
            this.Close ();
        }
    }
}
