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
    public partial class PersonsManagerForm: Window {

        ObservableCollection<Person> personsList = new ObservableCollection<Person> ();
        public ICollectionView _personsView;
        public List<Person> insertedPersons = new List<Person> ();
        public bool accepted = false;

        public PersonsManagerForm (ObservableCollection<Person> passedPersonsList) {
            InitializeComponent ();
            this.personsList = passedPersonsList;
            _personsView = CollectionViewSource.GetDefaultView (personsList);
            _personsView.Filter = new Predicate<object> (FilterTypes);
            this.personsListBox.DataContext = _personsView;
            _personsView.MoveCurrentToPosition (0);
        }
        bool FilterTypes (object ob) {
            Person tempPerson = ob as Person;
            return (tempPerson.Type.ID == 1);
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            _personsView.MoveCurrentToPosition (-1);
        }

        private void addNewPersonBTN_Click (object sender, RoutedEventArgs e) {
            PersonForm newPersonForm = new PersonForm ();
            newPersonForm.Owner = this;
            newPersonForm.ShowDialog ();
            if (newPersonForm.accepted) {
                DatabaseManager.InsertPerson (newPersonForm.currPerson);
                personsList.Add (newPersonForm.currPerson);
                insertedPersons.Add (newPersonForm.currPerson);
            }
        }
        private void removePersonBtn_Click (object sender, RoutedEventArgs e) {
            Person selectedPerson = (Person) _personsView.CurrentItem;
            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje osobe: " + 
                selectedPerson.Name + "\r\nJeste li sigurni?", "Potvrda brisanja", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                    DatabaseManager.DeletePerson (selectedPerson);
                    personsList.Remove (selectedPerson);
            }
        }
        private void editPersonBTN_Click (object sender, RoutedEventArgs e) {
            Person selectedPerson = (Person) _personsView.CurrentItem;
            PersonForm editPersonForm = new PersonForm (selectedPerson);
            editPersonForm.Owner = this;
            editPersonForm.ShowDialog ();
            if (editPersonForm.accepted) {
                DatabaseManager.UpdatePerson (editPersonForm.currPerson);
                personsList.Add (editPersonForm.currPerson);
                personsList.Remove (selectedPerson);
            }
        }

        private void acceptedButton_Click (object sender, RoutedEventArgs e) {
            this.accepted = true;
            this.Close ();
        }

    }
}
