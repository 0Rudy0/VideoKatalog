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
    /// Interaction logic for SelectSerieForm.xaml
    /// </summary>
    public partial class SelectSerieForm: Window {
        public object SelectedSerie;
        public object SelectedSeason;
        public bool accepted = false;
        ObservableCollection<Serie> serieList;
        ObservableCollection<WishSerie> wishSerieList;
        SerieCategory serieCat;

        public SelectSerieForm (ObservableCollection<Serie> serieList, ObservableCollection<WishSerie> wishSerieList, SerieCategory seasonOrEpisode) {
            InitializeComponent ();
            this.serieList = serieList;
            this.wishSerieList = wishSerieList;
            this.serieCat = seasonOrEpisode;
            //dodaj sve WishSerie u listu
            foreach (WishSerie tempWishSerie in this.wishSerieList)
                allSerieListBox.Items.Add (tempWishSerie);

            //dodaj sve serije u listu koje nisu u wishserie listi
            foreach (Serie tempSerie in this.serieList) {
                bool found = false;
                foreach (WishSerie tempWishSerie in this.wishSerieList)
                    if (tempWishSerie.Name == tempSerie.Name)
                        found = true;
                if (found == false)
                    allSerieListBox.Items.Add (tempSerie);
            }

            if (seasonOrEpisode == SerieCategory.season) {
                selectSeasonLabel.Visibility = System.Windows.Visibility.Hidden;
                seasonListBox.Visibility = System.Windows.Visibility.Hidden;
                acceptButton.Margin = new Thickness (0, 0, 0, 15);
            }
            else if (seasonOrEpisode == SerieCategory.episode) {
            }
            
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            if (this.allSerieListBox.SelectedIndex < 0) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi seriju");
                return;
            }
            if (this.seasonListBox.SelectedIndex < 0 && this.serieCat == SerieCategory.episode) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi sezonu");
                return;
            }
            this.accepted = true;
            this.SelectedSerie = this.allSerieListBox.SelectedItem;
            if (serieCat == SerieCategory.episode)
                this.SelectedSeason = this.seasonListBox.SelectedItem;
            this.Close ();
        }

        private void allSerieListBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            try {
                WishSerie wish = (WishSerie) allSerieListBox.SelectedItem;
                seasonListBox.ItemsSource = wish.WishSeasons;
            }
            catch {
                Serie regular = (Serie) allSerieListBox.SelectedItem;
                seasonListBox.ItemsSource = regular.Seasons;
            }
        }
    }
}
