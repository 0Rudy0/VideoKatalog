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
    public partial class PersonForm: Window {

        public bool accepted = false;
        public Person currPerson = new Person (new PersonType (1));
        Cloner cloner = new Cloner ();

        public PersonForm () {
            InitializeComponent ();
            currPerson.Name = "Ime Prezime";
            this.birthDate.SelectedDate = currPerson.DateOfBirth;
        }
        public PersonForm (Person personToEdit) {
            InitializeComponent ();
            currPerson = cloner.ClonePerson (personToEdit);
            this.birthDate.SelectedDate = currPerson.DateOfBirth;
            this.personNameTextBox.Text = currPerson.Name;
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            this.DataContext = currPerson;
            this.birthDate.SelectedDate = currPerson.DateOfBirth;
        }

        private void acceptButton_Click (object sender, RoutedEventArgs e) {            
            if (this.birthDate.SelectedDate != null) {
                this.accepted = true;
                currPerson.DateOfBirth = (DateTime) this.birthDate.SelectedDate;
                this.Close ();
            }
            else {
                this.accepted = false;
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran datum rođenja.", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }
    }
}
