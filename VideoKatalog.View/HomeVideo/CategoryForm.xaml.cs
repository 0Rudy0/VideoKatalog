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
    /// Interaction logic for CategoryForm.xaml
    /// </summary>
    public partial class CategoryForm: Window {
        ObservableCollection<Category> categories;
        public bool accepted = false;
        public CategoryForm (ObservableCollection<Category> categoryList) {
            InitializeComponent ();
            categories = categoryList;
            this.categoriesListBox.ItemsSource = categories;
        }
               
        private void addNewCategoryBTN_Click (object sender, RoutedEventArgs e) {
            InputDialog newCategoryDialog = new InputDialog ("Nova kategorija");
            newCategoryDialog.Owner = this;
            newCategoryDialog.ShowDialog ();
            if (newCategoryDialog.accepted) {
                Category newCat = new Category ();
                newCat.Name = newCategoryDialog.inputString;
                DatabaseManager.InsertCategory (newCat);
                categories.Add (newCat);
            }
        }

        private void removeCategoryBTN_Click (object sender, RoutedEventArgs e) {
            Category selectedCategory = categories.ElementAt (categoriesListBox.SelectedIndex);
            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje kategorije: " + selectedCategory.Name + "\r\nJeste li sigurni?", 
                "Potvrda brisanja", MessageBoxButton.OK, MessageBoxImage.Warning) == MessageBoxResult.OK) {
                    DatabaseManager.DeleteCategory (selectedCategory);
                    categories.Remove (selectedCategory);
            }
        }

        private void editCategoryBTN_Click (object sender, RoutedEventArgs e) {
            Category selectedCategory = categories.ElementAt (categoriesListBox.SelectedIndex);
            InputDialog editCategoryDialog = new InputDialog ("Izmijeni kategoriju", selectedCategory.Name);
            editCategoryDialog.Owner = this;
            editCategoryDialog.ShowDialog ();
            if (editCategoryDialog.accepted) {
                selectedCategory.Name = editCategoryDialog.inputString;
                DatabaseManager.UpdateCategory (selectedCategory);
                categories.Remove (selectedCategory);
                categories.Add (selectedCategory);                
            }
        }
        private void acceptedButton_Click (object sender, RoutedEventArgs e) {
            this.accepted = true;
            this.Close ();
        }
    }
}
