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
using System.IO;

namespace Video_katalog {

    public partial class WishListForm: Window {

        #region GLOBAL DATA

        ObservableCollection<Person> personList;
        ObservableCollection<Genre> genreList;
        ObservableCollection<Camera> cameraList;
        ObservableCollection<Category> HVCategoryList;

        ObservableCollection<WishMovie> wishMovieList = new ObservableCollection<WishMovie> ();
        ObservableCollection<WishHomeVideo> wishHomeVideoList = new ObservableCollection<WishHomeVideo> ();
        ObservableCollection<WishSerie> wishSerieList = new ObservableCollection<WishSerie> ();
        ObservableCollection<WishSerieSeason> wishSeasonList = new ObservableCollection<WishSerieSeason> ();
        ObservableCollection<WishSerieEpisode> wishEpisodeList = new ObservableCollection<WishSerieEpisode> ();

        ObservableCollection<Serie> serieList;
        ObservableCollection<SerieSeason> seasonList;
        
        ICollectionView _wishMovieView;
        ICollectionView _wishHomeVideoView;
        ICollectionView _wishSerieView;
        ICollectionView _wishSeasonView;
        ICollectionView _wishEpisodeView;

        Cloner cloner = new Cloner ();
        public List<Movie> insertedMovies = new List<Movie> ();
        public List<HomeVideo> insertedHomeVideos = new List<HomeVideo> ();
        public List<Serie> insertedSeries = new List<Serie> ();
        public List<HDD> insertedHDDs = new List<HDD> ();
        public List<TextBlock> serieReleaseDateTBs = new List<TextBlock> ();
        public List<ListBox> seasonListBoxes = new List<ListBox> ();

        #endregion

        #region ON LOAD

        public WishListForm (ObservableCollection<Person> personList, ObservableCollection<Genre> genreList, ObservableCollection<Camera> cameraList, 
            ObservableCollection<Category> homeVideoCategoryList, ObservableCollection<Serie> serieList, ObservableCollection<SerieSeason> seasonList) {
            InitializeComponent ();
            this.personList = personList;
            this.genreList = genreList;
            this.cameraList = cameraList;
            this.HVCategoryList = homeVideoCategoryList;
            this.serieList = serieList;
            this.seasonList = seasonList;
            SetViewesAndDataContext ();
        }
        private void wishListWindow_Loaded (object sender, RoutedEventArgs e) {
            foreach (ComboBoxItem item in movieSortCategory.Items)
                if (item.ToString ().Contains ("Po vremenu izlaska"))
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            foreach (ComboBoxItem item in homeVideoSortCategory.Items)
                if (item.ToString ().Contains ("Po zakazanom datumu"))
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            foreach (ComboBoxItem item in serieSortCategory.Items)
                if (item.ToString ().Contains ("Po vremenu izlaska"))
                    item.IsSelected = true;
                else
                    item.IsSelected = false;
            foreach (TextBlock tempTB in FindTreeControls.FindChildren<TextBlock> (this.wishSerieListBox)) {
                if (tempTB.Name == "serieReleaseDateTB") {
                }
            }
        }
        void SetViewesAndDataContext () {
            //Movie
            wishMovieList = DatabaseManager.FetchWishMovieList (personList, genreList);
            _wishMovieView = CollectionViewSource.GetDefaultView (wishMovieList);
            _wishMovieView.MoveCurrentToPosition (-1);
            this.wishMovieListBox.DataContext = _wishMovieView;

            //Home Video
            wishHomeVideoList = DatabaseManager.FetchWishHomeVideoList (personList, HVCategoryList, cameraList);
            _wishHomeVideoView = CollectionViewSource.GetDefaultView (wishHomeVideoList);
            _wishHomeVideoView.MoveCurrentToPosition (-1);
            this.wishHomeVideoListBox.DataContext = _wishHomeVideoView;
            
            //Serie
            wishSerieList = DatabaseManager.FetchWishSerieList (genreList, personList);
            wishSeasonList = DatabaseManager.FetchWishSerieSeasonList (wishSerieList);
            wishEpisodeList = DatabaseManager.FetchWishSerieEpisodeList (wishSeasonList);

            _wishSerieView = CollectionViewSource.GetDefaultView (wishSerieList);
            _wishSerieView.MoveCurrentToPosition (-1);
            this.wishSerieListBox.DataContext = _wishSerieView;
        }
        private void serieReleaseDateTB_Loaded (object sender, RoutedEventArgs e) {
            //TextBlock tempTB = sender as TextBlock;
            ////serieReleaseDateTBs.Add (tempTB);
            //WishSerie wish = tempTB.DataContext as WishSerie;
            //DateTime dateStart = new DateTime (3000, 1, 1);
            //DateTime dateEnd = new DateTime (1000, 1, 1);
            //foreach (WishSerieSeason tempSeason in wish.WishSeasons) {
            //    foreach (WishSerieEpisode tempEpisode in tempSeason.WishEpisodes) {
            //        if (tempEpisode.AirDate < dateStart)
            //            dateStart = tempEpisode.AirDate;
            //        if (tempEpisode.AirDate > dateEnd)
            //            dateEnd = tempEpisode.AirDate;
            //    }
            //}
            //tempTB.Text = dateStart.ToShortDateString () + " - " + dateEnd.ToShortDateString ();
        }
        private void seasonListBox_Loaded (object sender, RoutedEventArgs e) {
            ListBox tempLB = sender as ListBox;
            tempLB.SelectedIndex = -1;
            tempLB.SelectedIndex = 0;
        }

        #endregion

        string SearchTitleReturnLink () {
            SearchForm search = new SearchForm ();
            search.Owner = this;
            search.ShowDialog ();
            return search.selectedLink;
        }

        void SetPersonPointersOrInsert (ObservableCollection<Person> persons) {
            //prodji kroz sve osobe u filmu, te provjeri da li postoje u listi osoba (ako postoje, onda postoje i u bazi)
            for (int i = 0 ; i < persons.Count ; i++) {
                bool found = false;

                //za svaku osobu, ako vec postoji, postavi pokazivac na postojecu osobu (da se ne stvaraju dodatne iste osobe u memoriji)
                foreach (Person tempPerson in personList) {
                    if (tempPerson.Name == persons[i].Name) {
                        found = true;
                        persons[i] = tempPerson;
                    }
                }

                //ako osoba ne postoji, unesi ju u bazu podataka i dohvati ID osobe
                if (found == false) {
                    DatabaseManager.InsertPerson (persons[i]);
                    DatabaseManagerMySql.InsertPerson (persons[i]);
                    personList.Add (persons[i]);
                }
            }
        }
        void SetGenrePointers (ObservableCollection<Genre> genres) {
            //prodji kroz sve zanrove filmova i postavi pokazivac za svaki na vec postojeci zanr u glavnoj listi zanrova
            for (int i = 0 ; i < genres.Count ; i++) {
                foreach (Genre tempGenre in genreList) {
                    if (tempGenre.Name == genres[i].Name) {
                        genres[i] = tempGenre;
                    }
                }
            }
        }
       
        #region WISH MOVIE

        private void newWishMovie_Click (object sender, RoutedEventArgs e) {
            string link = SearchTitleReturnLink ();
            if (link == null)
                return;
            MovieForm newWishMovie = new MovieForm (link, WishOrRegular.wish);
            newWishMovie.Owner = this;
            newWishMovie.ShowDialog ();
            if (newWishMovie.accepted) { 
                WishMovie newWM = new WishMovie ();
                newWM = WishMovieFromMovie (newWishMovie.currMovie);
                newWM.ReleaseDate = newWishMovie.releaseDateForWishMovie;
                SetPersonPointersOrInsert (newWM.Actors);
                SetPersonPointersOrInsert (newWM.Directors);
                SetGenrePointers (newWM.Genres);

                wishMovieList.Add (newWM);
                //wishMovieListBox.Items.Add (newWM);
                DatabaseManager.InsertNewWishMovie (newWM);
            }
        }       
        private void moveRemove_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishMovie clickedMovie = itemButton.DataContext as WishMovie;

            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje filma: " + clickedMovie.OrigName + " iz liste želja.\r\nJeste li sigurni?",
                "Potvrda brisanja", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                DatabaseManager.DeleteWishMovie (clickedMovie);
                wishMovieList.Remove (clickedMovie);
                _wishMovieView.MoveCurrentToPrevious ();
                _wishMovieView.MoveCurrentToNext ();
            }
        }
        private void movieEdit_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishMovie clickedMovie = itemButton.DataContext as WishMovie;

            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.moviePosterFolder + HelpFunctions.ReplaceIllegalChars (clickedMovie.Name) + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }

            MovieForm editWishMovieForm = new MovieForm (clickedMovie);
            editWishMovieForm.Owner = this;
            wishMovieList.Remove (clickedMovie);
            editWishMovieForm.ShowDialog ();
            if (editWishMovieForm.accepted) {
                WishMovie editedMovie = WishMovieFromMovie (editWishMovieForm.currMovie);
                editedMovie.ReleaseDate = (DateTime) editWishMovieForm.releaseDatePicker.SelectedDate;
                SetGenrePointers (editedMovie.Genres);
                SetPersonPointersOrInsert (editedMovie.Actors);
                SetPersonPointersOrInsert (editedMovie.Directors);
                DatabaseManager.UpdateWishMovie (editedMovie);
                //wishMovieList.Remove (selectedMovie);
                wishMovieList.Add (editedMovie);
            }
            else {
                wishMovieList.Add (clickedMovie);
            }
        }
        private void wishMovieToRegular_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishMovie clickedMovie = itemButton.DataContext as WishMovie;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.moviePosterFolder + clickedMovie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            wishMovieList.Remove (clickedMovie);
            MovieForm wishMovieToRegularForm = new MovieForm (MovieFromWishMovie (clickedMovie));
            wishMovieToRegularForm.Owner = this;
            wishMovieToRegularForm.ShowDialog ();
            if (wishMovieToRegularForm.accepted) {
                foreach (HDD tempHDD in wishMovieToRegularForm.insertedHDDs)
                    insertedHDDs.Add (tempHDD);
                Movie newMovie = wishMovieToRegularForm.currMovie;
                insertedMovies.Add (newMovie);
                DatabaseManager.DeleteWishMovie (clickedMovie);

            }
            else {
                wishMovieList.Add (clickedMovie);
            }
        }

        #region Sort / Filter

        private void movieSortCategory_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            SortDescription sortingWishMovie = new SortDescription ();
            _wishMovieView.SortDescriptions.Clear ();

            string sortBy = movieSortCategory.SelectedValue.ToString ();
            if (sortBy.Contains ("Po nazivu")) {
                sortingWishMovie.PropertyName = "OrigName";
            }
            else if (sortBy.Contains ("Po vremenu izlaska")) {
                sortingWishMovie.PropertyName = "ReleaseDate";
            }
            else if (sortBy.Contains ("Po godini")) {
                sortingWishMovie.PropertyName = "Year";
            }
            _wishMovieView.SortDescriptions.Add (sortingWishMovie);


        }
        private void searchWishMovieTB_TextChanged (object sender, TextChangedEventArgs e) {
            _wishMovieView.Filter = new Predicate<object> (FilterNames);
        }
        bool FilterNames (object ob) {
            WishMovie movie = ob as WishMovie;
            return (movie.OrigName.ToLower ().Contains (searchWishMovieTB.Text));
        }
        private void movieClearSearchName_Click (object sender, RoutedEventArgs e) {
            searchWishMovieTB.Text = "";
        }

        #endregion

        WishMovie WishMovieFromMovie (Movie movie) {
            WishMovie wishMovie = new WishMovie ();
            wishMovie.ID = movie.ID;
            wishMovie.Name = movie.Name;
            wishMovie.OrigName = movie.OrigName;
            wishMovie.InternetLink = movie.InternetLink;
            wishMovie.TrailerLink = movie.TrailerLink;
            wishMovie.Actors = movie.Actors;
            wishMovie.Directors = movie.Directors;
            wishMovie.Genres = movie.Genres;
            wishMovie.Budget = movie.Budget;
            wishMovie.Earnings = movie.Earnings;
            wishMovie.Year = movie.Year;
            wishMovie.Rating = movie.Rating;
            wishMovie.Summary = movie.Summary;
            return wishMovie;
        }
        Movie MovieFromWishMovie (WishMovie wishMovie) {
            Movie movie = new Movie ();
            movie.ID = wishMovie.ID;
            movie.Name = wishMovie.Name;
            movie.OrigName = wishMovie.OrigName;
            movie.Rating = wishMovie.Rating;
            movie.Year = wishMovie.Year;
            movie.TrailerLink = wishMovie.TrailerLink;
            movie.InternetLink = wishMovie.InternetLink;
            movie.Summary = wishMovie.Summary;
            movie.Directors = wishMovie.Directors;
            movie.Actors = wishMovie.Actors;
            movie.Budget = wishMovie.Budget;
            movie.Earnings = wishMovie.Earnings;
            movie.Genres = wishMovie.Genres;
            return movie;
        }       

        #endregion        

        #region WISH HOME VIDEO

        private void addWishHomeVideo_Click (object sender, System.Windows.RoutedEventArgs e) {
            HomeVideoForm newWishHomeVideo = new HomeVideoForm (WishOrRegular.wish, personList, cameraList, HVCategoryList);
            newWishHomeVideo.Owner = this;
            newWishHomeVideo.ShowDialog ();
            if (newWishHomeVideo.accepted) {
                WishHomeVideo wishHV = WishHomeVideoFromRegular (newWishHomeVideo.currHomeVideo);
                SetPersonPointersOrInsert (wishHV.PersonsInVideo);
                SetPersonPointersOrInsert (wishHV.Camermans);
                DatabaseManager.InsertWishHomeVideo (wishHV);
                wishHomeVideoList.Add (wishHV);
            }
        }        
        private void homeVideoRemove_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishHomeVideo clickedHV = itemButton.DataContext as WishHomeVideo;
            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje kućnog videa: " + clickedHV.Name + " iz liste želja.\r\nJeste li sigurni?",
               "Potvrda brisanja", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                   DatabaseManager.DeleteWishHomeVideo (clickedHV);
                   wishHomeVideoList.Remove (clickedHV);
                _wishHomeVideoView.MoveCurrentToPrevious ();
                _wishHomeVideoView.MoveCurrentToNext ();
            }
        }
        private void homeVideoEdit_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishHomeVideo clickedHV = itemButton.DataContext as WishHomeVideo;

            HomeVideoForm editWishForm = new HomeVideoForm (clickedHV, personList, cameraList, HVCategoryList);
            editWishForm.Owner = this;
            wishHomeVideoList.Remove (clickedHV);
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.homeVideoPosterFolder + clickedHV.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            editWishForm.ShowDialog ();
            if (editWishForm.accepted) {
                WishHomeVideo editedWishHV = WishHomeVideoFromRegular (editWishForm.currHomeVideo);
                SetPersonPointersOrInsert (editedWishHV.PersonsInVideo);
                SetPersonPointersOrInsert (editedWishHV.Camermans);
                DatabaseManager.UpdateWishHomeVideo (editedWishHV);
                wishHomeVideoList.Add (editedWishHV);
            }
            else {
                wishHomeVideoList.Add (clickedHV);
            }
        }
        private void wishHomeVideoToRegular_Click (object sender, RoutedEventArgs e) {
            Button itemButton = sender as Button;
            WishHomeVideo clickedHV = itemButton.DataContext as WishHomeVideo;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.homeVideoPosterFolder + clickedHV.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            wishHomeVideoList.Remove (clickedHV);
            HomeVideoForm wishHVToRegularForm = new HomeVideoForm (HomeVideoFromWish (clickedHV), personList, cameraList, HVCategoryList);
            wishHVToRegularForm.Owner = this;
            wishHVToRegularForm.ShowDialog ();
            if (wishHVToRegularForm.accepted) {
                foreach (HDD tempHDD in wishHVToRegularForm.insertedHDDs)
                    insertedHDDs.Add (tempHDD);
                HomeVideo newHV = wishHVToRegularForm.currHomeVideo;
                DatabaseManager.DeleteWishHomeVideo (clickedHV);
                //DatabaseManager.InsertHomeVideo (newHV);
                insertedHomeVideos.Add (newHV);
            }
            else {
                wishHomeVideoList.Add (clickedHV);
            }
        }

        #region Sort / Filter

        private void homeVideoSortCategory_SelectionChanged (object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            SortDescription sortingWishHV = new SortDescription ();
            _wishHomeVideoView.SortDescriptions.Clear ();

            //ComboBoxItem selectedItem = homeVideoSortCategory.SelectedItem;
            string sortBy = homeVideoSortCategory.SelectedItem.ToString ();
            if (sortBy.Contains ("Po zakazanom datumu")) {
                sortingWishHV.PropertyName = "FilmDate";
            }
            else if (sortBy.Contains ("Po nazivu")) {
                sortingWishHV.PropertyName = "Name";
            }
            _wishHomeVideoView.SortDescriptions.Add (sortingWishHV);
        }
        private void searchWishHomeVideo_TextChanged (object sender, System.Windows.Controls.TextChangedEventArgs e) {
            _wishHomeVideoView.Filter = new Predicate<object> (FilterHomeVideo);
        }
        bool FilterHomeVideo (object ob) {
            WishHomeVideo homeVideo = ob as WishHomeVideo;
            return (homeVideo.Name.ToLower ().Contains (searchWishHomeVideo.Text));
        }
        private void homeVideoClearSearch_Click (object sender, System.Windows.RoutedEventArgs e) {
            searchWishHomeVideo.Text = "";
        }

        #endregion

        WishHomeVideo WishHomeVideoFromRegular (HomeVideo regular) {
            WishHomeVideo wish = new WishHomeVideo ();
            wish.Name = regular.Name;
            wish.Camermans = regular.Camermans;
            wish.Comment = regular.Summary;
            wish.FilmDate = regular.FilmDate;
            wish.FilmingCamera = regular.FilmingCamera;
            wish.ID = regular.ID;
            wish.Location = regular.Location;
            wish.PersonsInVideo = regular.PersonsInVideo;
            wish.VideoCategory = regular.VideoCategory;
            return wish;
        }
        HomeVideo HomeVideoFromWish (WishHomeVideo wish) {
            HomeVideo regular = new HomeVideo ();
            regular.Name = wish.Name;
            regular.Camermans = wish.Camermans;
            regular.Summary = wish.Comment;
            regular.FilmDate = wish.FilmDate;
            regular.FilmingCamera = wish.FilmingCamera;
            regular.ID = wish.ID;
            regular.Location = wish.Location;
            regular.PersonsInVideo = wish.PersonsInVideo;
            regular.VideoCategory = wish.VideoCategory;
            return regular;
        }
        
        #endregion 

        #region WISH SERIE

        //ADD
        private void addWishSerie_Click (object sender, RoutedEventArgs e) {
            string link = SearchTitleReturnLink ();
            if (link == null)
                return;
            WishSerie newSerie = new WishSerie ();
            newSerie.InternetLink = link;
            SerieForm addNewWish = new SerieForm (newSerie, SerieCategory.serie, AddOrEdit.add);
            addNewWish.Owner = this;
            addNewWish.ShowDialog ();
            if (addNewWish.accepted) {
                newSerie = WishSerieFromRegular (addNewWish.currSerie);
                SetPersonPointersOrInsert (newSerie.Directors);
                SetPersonPointersOrInsert (newSerie.Actors);
                SetGenrePointers (newSerie.Genres);

                DatabaseManager.InsertWishSerie (newSerie);
                wishSerieList.Add (newSerie);
            }
        }
        private void addWishSerieSeason_Click (object sender, RoutedEventArgs e) {
            SelectSerieForm selectSerieForSeasonForm = new SelectSerieForm (serieList, wishSerieList, SerieCategory.season);
            selectSerieForSeasonForm.Owner = this;
            selectSerieForSeasonForm.ShowDialog ();
            if (selectSerieForSeasonForm.accepted) {
                WishSerie selectedWishSerie;
                try {
                    selectedWishSerie = (WishSerie) selectSerieForSeasonForm.SelectedSerie;// as WishSerie;
                }
                catch {
                    Serie selectedSerie = selectSerieForSeasonForm.SelectedSerie as Serie;
                    Serie cloneSerie = cloner.CloneSerieOnlyInfo (selectedSerie);
                    selectedWishSerie = WishSerieFromRegular (cloneSerie);
                    selectedWishSerie.RegularSerieID = cloneSerie.ID;
                    selectedWishSerie.ID = 0;      
                    DatabaseManager.InsertWishSerie (selectedWishSerie);
                    wishSerieList.Add (selectedWishSerie);
                }
                SerieForm newWishSeasonForm = new SerieForm (selectedWishSerie, SerieCategory.season, AddOrEdit.add);
                newWishSeasonForm.Owner = this;
                try {
                    Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                        selectedWishSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                }
                catch {
                    Clipboard.Clear ();
                }
                newWishSeasonForm.ShowDialog ();
                if (newWishSeasonForm.accepted) {
                    foreach (SerieSeason tempSeason in newWishSeasonForm.currSerie.Seasons) {
                        WishSerieSeason newWishSeason = WishSerieSeasonFromRegular (tempSeason);
                        newWishSeason.ParentWishSerie = selectedWishSerie;
                        selectedWishSerie.WishSeasons.Add (newWishSeason);
                        DatabaseManager.InsertWishSerieSeason (newWishSeason);
                    }
                }     
                else {
                    if (selectedWishSerie.WishSeasons.Count == 0) {
                        DatabaseManager.DeleteWishSerie (selectedWishSerie);
                        wishSerieList.Remove (selectedWishSerie);
                    }
                }
            }
        }
        private void addWishSerieEpisode_Click (object sender, RoutedEventArgs e) {
            SelectSerieForm selectSerieSeasonForm = new SelectSerieForm (serieList, wishSerieList, SerieCategory.episode);
            selectSerieSeasonForm.Owner = this;
            selectSerieSeasonForm.ShowDialog ();
            if (selectSerieSeasonForm.accepted) {
                WishSerie selectedWishSerie;
                WishSerieSeason selectedWishSeason;               
                try {
                    selectedWishSerie = (WishSerie) selectSerieSeasonForm.SelectedSerie;
                    selectedWishSerie.WishSeasons.Clear ();
                    selectedWishSeason = (WishSerieSeason) selectSerieSeasonForm.SelectedSeason;
                    selectedWishSerie.WishSeasons.Add (selectedWishSeason);
                    selectedWishSeason.ParentWishSerie = selectedWishSerie;
                }
                catch {
                    Serie selectedSerie = selectSerieSeasonForm.SelectedSerie as Serie;
                    Serie cloneSerie = cloner.CloneSerieOnlyInfo (selectedSerie);
                    SerieSeason selectedSeason = selectSerieSeasonForm.SelectedSeason as SerieSeason;
                    SerieSeason cloneSeason = cloner.CloneSeasonOnlyInfo (selectedSeason);
                    cloneSerie.Seasons.Add (cloneSeason);
                    cloneSeason.ParentSerie = cloneSerie;

                    selectedWishSerie = WishSerieFromRegular (cloneSerie);
                    selectedWishSerie.RegularSerieID = cloneSerie.ID;
                    selectedWishSerie.ID = 0;
                    selectedWishSeason = selectedWishSerie.WishSeasons.ElementAt (0);
                    selectedWishSeason.RegularSeasonID = selectedSeason.ID;
                    selectedWishSeason.ID = 0;
                    DatabaseManager.InsertWishSerie (selectedWishSerie);
                    wishSerieList.Add (selectedWishSerie);
                    wishSeasonList.Add (selectedWishSerie.WishSeasons.ElementAt (0));
                }
                SerieForm addWishEpisodeForm = new SerieForm (selectedWishSerie, SerieCategory.episode, AddOrEdit.add);
                addWishEpisodeForm.Owner = this;
                try {
                    Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                        selectedWishSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                }
                catch {
                    Clipboard.Clear ();
                }
                addWishEpisodeForm.ShowDialog ();
                if (addWishEpisodeForm.accepted) {
                    foreach (SerieEpisode tempEpisode in addWishEpisodeForm.currSerie.Seasons.ElementAt (0).Episodes) {
                        WishSerieEpisode newWishEpisode = WishSerieEpisodeFromRegular (tempEpisode);
                        newWishEpisode.ParentWishSeason = selectedWishSeason;
                        selectedWishSeason.WishEpisodes.Add (newWishEpisode);
                        DatabaseManager.InsertWishSerieEpisode (newWishEpisode);
                    }
                }
                else {
                    int countNotEmpty = 0;
                    foreach (WishSerieSeason tempSeason in selectedWishSerie.WishSeasons)
                        if (tempSeason.WishEpisodes.Count > 0)
                            countNotEmpty++;
                    if (countNotEmpty == 0) {
                        DatabaseManager.DeleteWishSerie (selectedWishSerie);
                        wishSerieList.Remove (selectedWishSerie);
                    }                    
                }
            }
        }

        //EDIT
        private void serieEdit_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie clickedSerie = clickedButton.DataContext as WishSerie;
            if (clickedSerie == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi seriju");
                return;
            }
            //Serie regularFormat = SerieFromWishSerie (clickedSerie);
            SerieForm editWishSerieForm = new SerieForm (clickedSerie, SerieCategory.serie, AddOrEdit.edit);
            editWishSerieForm.Owner = this;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + 
                    clickedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();
            }
            wishSerieList.Remove (clickedSerie);            
            editWishSerieForm.ShowDialog ();
            if (editWishSerieForm.accepted) {
                WishSerie editedSerie = WishSerieFromRegular (editWishSerieForm.currSerie);
                editedSerie.RegularSerieID = clickedSerie.RegularSerieID;
                SetPersonPointersOrInsert (editedSerie.Actors);
                SetPersonPointersOrInsert (editedSerie.Directors);
                SetGenrePointers (editedSerie.Genres);

                DatabaseManager.UpdateWishSerie (editedSerie);
                wishSerieList.Add (editedSerie);
            }
            else {
                wishSerieList.Add (clickedSerie);
            }

        }
        private void seasonEdit_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "seasonListBox") {
                    WishSerieSeason clickedSeason = tempLB.SelectedItem as WishSerieSeason;
                    WishSerieSeason cloneSeason = cloner.CloneWishSeasonWithEpisodes (clickedSeason);
                    WishSerie cloneSerie = cloner.CloneWishSerieonlyinfo (selectedSerie);
                    cloneSerie.WishSeasons.Add (cloneSeason);
                    cloneSeason.ParentWishSerie = cloneSerie;
                    SerieForm editWishSeason = new SerieForm (cloneSerie, SerieCategory.season, AddOrEdit.edit);
                    editWishSeason.Owner = this;
                    wishSerieList.Remove (selectedSerie);
                    try {
                        Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                            selectedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                    }
                    catch {
                        Clipboard.Clear ();
                    }
                    editWishSeason.ShowDialog ();
                    if (editWishSeason.accepted) {
                        selectedSerie.WishSeasons.Remove (clickedSeason);
                        WishSerieSeason editedSeason = WishSerieSeasonFromRegular (editWishSeason.currSerie.Seasons.ElementAt (0)); //ako je bila izmjena sezone, onda currSerie sadrzava samo jednu sezonu, tu koja se mijenjala
                        editedSeason.RegularSeasonID = clickedSeason.RegularSeasonID;
                        editedSeason.ParentWishSerie = selectedSerie;
                        selectedSerie.WishSeasons.Add (editedSeason);
                        DatabaseManager.UpdateWishSerieSeason (editedSeason);
                        wishSerieList.Add (selectedSerie);
                    }
                    else {
                        wishSerieList.Add (selectedSerie);
                    }
                }

            }
        }
        private void episodeEdit_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            //prvo nadji listbox za epizode, te dohvati oznacenu epizodu
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "episodesListBox") {
                    WishSerieEpisode clickedEpisode = tempLB.SelectedItem as WishSerieEpisode;
                    //wishEpisodeList.Remove (clickedEpisode);
                    //DatabaseManager.DeleteWishSerieEpisode (clickedEpisode);

                    //
                    TabItem serieTab = gridContainer.Parent as TabItem;
                    TabControl tabControl = serieTab.Parent as TabControl;

                    foreach (ListBox tempLB2 in FindTreeControls.FindChildren<ListBox> (tabControl)) {
                        if (tempLB2.Name == "seasonListBox") {
                            WishSerieSeason selectedSeason = tempLB2.SelectedItem as WishSerieSeason;
                            //selectedSeason.WishEpisodes.Remove (clickedEpisode);
                            WishSerie cloneSerie = cloner.CloneWishSerieonlyinfo (selectedSerie);
                            WishSerieSeason cloneSeason = cloner.CloneWishSeasonOnlyInfo (selectedSeason);
                            WishSerieEpisode cloneEpisode = cloner.CloneWishEpisode (clickedEpisode);
                            cloneSeason.WishEpisodes.Add (cloneEpisode);
                            cloneEpisode.ParentWishSeason = cloneSeason;
                            cloneSeason.ParentWishSerie = cloneSerie;
                            cloneSerie.WishSeasons.Add (cloneSeason);

                            SerieForm editWishEpForm = new SerieForm (cloneSerie, SerieCategory.episode, AddOrEdit.edit);
                            editWishEpForm.Owner = this;
                            try {
                                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                                    selectedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                            }
                            catch {
                                Clipboard.Clear ();
                            }
                            editWishEpForm.ShowDialog ();
                            if (editWishEpForm.accepted) {
                                WishSerieEpisode editedEpisode = WishSerieEpisodeFromRegular (editWishEpForm.currSerie.Seasons.ElementAt (0).Episodes.ElementAt (0));
                                editedEpisode.ParentWishSeason = selectedSeason;
                                selectedSeason.WishEpisodes.Remove (clickedEpisode);
                                selectedSeason.WishEpisodes.Add (editedEpisode);
                                DatabaseManager.UpdateWishSerieEpisode (editedEpisode);
                                wishEpisodeList.Remove (clickedEpisode);
                                wishEpisodeList.Add (editedEpisode);
                            }
                        }
                    }
                }
            }
        }

        //REMOVE
        private void serieRemove_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie clickedSerie = clickedButton.DataContext as WishSerie;
            if (Xceed.Wpf.Toolkit.MessageBox.Show ("Brisanje filma: " + clickedSerie.OrigName + " iz liste želja.\r\nJeste li sigurni?",
                "Potvrda brisanja", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    wishSerieList.Remove (clickedSerie);
                    DatabaseManager.DeleteWishSerie (clickedSerie);
                    _wishSerieView.MoveCurrentToPrevious ();
                    _wishSerieView.MoveCurrentToNext ();
            }
        }
        private void seasonRemove_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "seasonListBox") {
                    WishSerieSeason clickedSeason = tempLB.SelectedItem as WishSerieSeason;
                    wishSeasonList.Remove (clickedSeason);
                    selectedSerie.WishSeasons.Remove (clickedSeason);
                    DatabaseManager.DeleteWishSerieSeason (clickedSeason);
                }
            }
        }
        private void episodeRemove_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            //prvo nadji listbox za epizode, te dohvati oznacenu epizodu nakon cega makni tu epizodu
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "episodesListBox") {
                    WishSerieEpisode clickedEpisode = tempLB.SelectedItem as WishSerieEpisode;
                    wishEpisodeList.Remove (clickedEpisode);
                    DatabaseManager.DeleteWishSerieEpisode (clickedEpisode);

                    //
                    TabItem serieTab = gridContainer.Parent as TabItem;
                    TabControl tabControl = serieTab.Parent as TabControl;
                    
                    foreach (ListBox tempLB2 in FindTreeControls.FindChildren<ListBox> (tabControl)) {
                        if (tempLB2.Name == "seasonListBox") {
                            WishSerieSeason selectedSeason = tempLB2.SelectedItem as WishSerieSeason;
                            selectedSeason.WishEpisodes.Remove (clickedEpisode);
                        }
                    }
                }
            }
        }

        //WISH TO REGULAR
        private void wishSerieToRegular_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie clickedSerie = clickedButton.DataContext as WishSerie;
            SerieForm wishToRegularForm = new SerieForm (SerieFromWishSerie (clickedSerie), SerieCategory.serie, AddOrEdit.edit);
            wishToRegularForm.Owner = this;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                    clickedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();
            }
            wishToRegularForm.ShowDialog ();
            if (wishToRegularForm.accepted) {
                Serie newSerie = wishToRegularForm.currSerie;
                wishSerieList.Remove (clickedSerie);
                newSerie.ID = clickedSerie.RegularSerieID;
                foreach (SerieSeason tempSeason in newSerie.Seasons) {
                    foreach (WishSerieSeason tempWishSeason in clickedSerie.WishSeasons) {
                        if (tempSeason.Name == tempWishSeason.Name)
                            tempSeason.ID = tempWishSeason.RegularSeasonID;
                    }
                }
                DatabaseManager.DeleteWishSerie (clickedSerie);                
                insertedSeries.Add (newSerie);
            }
            else {
            }
        }
        private void wishEpisodeToRegular_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            //prvo nadji listbox za epizode, te dohvati oznacenu epizodu nakon cega makni tu epizodu
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "episodesListBox") {
                    WishSerieEpisode clickedEpisode = tempLB.SelectedItem as WishSerieEpisode;
                    TabItem serieTab = gridContainer.Parent as TabItem;
                    TabControl tabControl = serieTab.Parent as TabControl;
                    foreach (ListBox tempLB2 in FindTreeControls.FindChildren<ListBox> (tabControl)) {
                        if (tempLB2.Name == "seasonListBox") {
                            WishSerieSeason selectedSeason = tempLB2.SelectedItem as WishSerieSeason;
                            WishSerie cloneSerie = cloner.CloneWishSerieonlyinfo (selectedSerie);
                            WishSerieSeason cloneSeason = cloner.CloneWishSeasonOnlyInfo (selectedSeason);
                            WishSerieEpisode cloneEpisode = cloner.CloneWishEpisode (clickedEpisode);
                            cloneEpisode.ParentWishSeason = cloneSeason;
                            cloneSeason.ParentWishSerie = cloneSerie;
                            cloneSerie.WishSeasons.Add (cloneSeason);
                            cloneSeason.WishEpisodes.Add (cloneEpisode);
                            SerieForm wishToRegularForm = new SerieForm (SerieFromWishSerie (cloneSerie), SerieCategory.episode, AddOrEdit.edit);
                            wishToRegularForm.Owner = this;
                            try {
                                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                                    cloneSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                            }
                            catch {
                                Clipboard.Clear ();
                            }
                            wishToRegularForm.ShowDialog ();
                            if (wishToRegularForm.accepted) {
                                Serie newSerie = wishToRegularForm.currSerie;
                                newSerie.ID = selectedSerie.RegularSerieID;
                                newSerie.Seasons.ElementAt (0).ID = selectedSeason.RegularSeasonID;
                                DatabaseManager.DeleteWishSerieEpisode (clickedEpisode);
                                selectedSeason.WishEpisodes.Remove (clickedEpisode);
                                insertedSeries.Add (newSerie);
                                                               
                                List<WishSerieSeason> seasonsToRemove = new List<WishSerieSeason> ();
                                foreach (WishSerieSeason tempSeason in selectedSerie.WishSeasons) {
                                    if (tempSeason.WishEpisodes.Count == 0)
                                        seasonsToRemove.Add (tempSeason);
                                }
                                foreach (WishSerieSeason tempSeason in seasonsToRemove)
                                    selectedSerie.WishSeasons.Remove (tempSeason);
                                if (selectedSerie.WishSeasons.Count == 0) {
                                    wishSerieList.Remove (selectedSerie);
                                    DatabaseManager.DeleteWishSerie (selectedSerie);
                                }
                            }
                        }
                    }
                }
            }
        }
        private void wishSeasonToRegular_Click (object sender, RoutedEventArgs e) {
            Button clickedButton = sender as Button;
            WishSerie selectedSerie = clickedButton.DataContext as WishSerie;
            Grid gridContainer = clickedButton.Parent as Grid;
            foreach (ListBox tempLB in FindTreeControls.FindChildren<ListBox> (gridContainer)) {
                if (tempLB.Name == "seasonListBox") {
                    WishSerieSeason clickedSeason = tempLB.SelectedItem as WishSerieSeason;
                    WishSerie cloneSerie = cloner.CloneWishSerieonlyinfo (selectedSerie);
                    WishSerieSeason cloneSeason = cloner.CloneWishSeasonWithEpisodes (clickedSeason);
                    cloneSerie.WishSeasons.Add (cloneSeason);
                    cloneSeason.ParentWishSerie = cloneSerie;
                    SerieForm wishToRegularForm = new SerieForm (SerieFromWishSerie (cloneSerie), SerieCategory.season, AddOrEdit.edit);
                    wishToRegularForm.Owner = this;
                    try {
                        Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder +
                            cloneSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                    }
                    catch {
                        Clipboard.Clear ();
                    }
                    wishToRegularForm.ShowDialog ();
                    if (wishToRegularForm.accepted) {
                        Serie newSerie = wishToRegularForm.currSerie;
                        newSerie.ID = selectedSerie.RegularSerieID;
                        newSerie.Seasons.ElementAt (0).ID = clickedSeason.RegularSeasonID;
                        DatabaseManager.DeleteWishSerieSeason (clickedSeason);
                        selectedSerie.WishSeasons.Remove (clickedSeason);
                        insertedSeries.Add (newSerie);

                        List<WishSerieSeason> seasonsToRemove = new List<WishSerieSeason> ();
                        foreach (WishSerieSeason tempSeason in selectedSerie.WishSeasons) {
                            if (tempSeason.WishEpisodes.Count == 0)
                                seasonsToRemove.Add (tempSeason);
                        }
                        foreach (WishSerieSeason tempSeason in seasonsToRemove)
                            selectedSerie.WishSeasons.Remove (tempSeason);
                        if (selectedSerie.WishSeasons.Count == 0) {
                            wishSerieList.Remove (selectedSerie);
                            DatabaseManager.DeleteWishSerie (selectedSerie);
                        }
                    }                    
                }                
            }
        }

        WishSerie WishSerieFromRegular (Serie regular) {
            WishSerie copy = new WishSerie ();
            copy.ID = regular.ID;
            copy.Name = regular.Name;
            copy.OrigName = regular.OrigName;
            copy.InternetLink = regular.InternetLink;
            copy.TrailerLink = regular.TrailerLink;
            copy.Summary = regular.Summary;
            copy.Actors = regular.Actors;
            copy.Directors = regular.Directors;
            copy.Genres = regular.Genres;
            copy.Rating = regular.Rating;
            foreach (SerieSeason tempSeason in regular.Seasons) {
                WishSerieSeason tempWishSeason = WishSerieSeasonFromRegular (tempSeason);
                tempWishSeason.ParentWishSerie = copy;
                copy.WishSeasons.Add (tempWishSeason);
            }
            return copy;
        }
        WishSerieSeason WishSerieSeasonFromRegular (SerieSeason regular) {
            WishSerieSeason copy = new WishSerieSeason ();
            copy.ID = regular.ID;
            copy.Name = regular.Name;
            copy.InternetLink = regular.InternetLink;
            copy.TrailerLink = regular.TrailerLink;
            foreach (SerieEpisode tempEpisode in regular.Episodes) {
                WishSerieEpisode tempWishEpisode = WishSerieEpisodeFromRegular (tempEpisode);
                tempWishEpisode.ParentWishSeason = copy;
                copy.WishEpisodes.Add (tempWishEpisode);
            }
            return copy;
        }
        WishSerieEpisode WishSerieEpisodeFromRegular (SerieEpisode regular) {
            WishSerieEpisode copy = new WishSerieEpisode ();
            copy.ID = regular.ID;
            copy.Name = regular.Name;
            copy.OrigName = regular.OrigName;
            copy.InternetLink = regular.InternetLink;
            copy.TrailerLink = regular.TrailerLink;
            copy.Summary = regular.Summary;
            copy.AirDate = regular.AirDate;
            return copy;
        }
        Serie SerieFromWishSerie (WishSerie wish) {
            Serie copy = new Serie ();
            copy.ID = wish.ID;
            copy.Name = wish.Name;
            copy.OrigName = wish.OrigName;
            copy.Actors = cloner.ClonePersonList (wish.Actors);
            copy.Directors = cloner.ClonePersonList (wish.Directors);
            copy.Genres = cloner.CloneGenreList (wish.Genres);
            copy.InternetLink = wish.InternetLink;
            copy.TrailerLink = wish.TrailerLink;
            copy.Rating = wish.Rating;
            copy.Summary = wish.Summary;
            foreach (WishSerieSeason tempWishSeason in wish.WishSeasons) {
                SerieSeason tempSeason = SerieSeasonFromWishSeason (tempWishSeason);
                tempSeason.ParentSerie = copy;
                copy.Seasons.Add (tempSeason);
            }
            return copy;
        }
        SerieSeason SerieSeasonFromWishSeason (WishSerieSeason wish) {
            SerieSeason copy = new SerieSeason ();
            copy.ID = wish.ID;
            copy.Name = wish.Name;
            copy.InternetLink = wish.InternetLink;
            copy.TrailerLink = wish.TrailerLink;
            foreach (WishSerieEpisode tempWishEpisode in wish.WishEpisodes) {
                SerieEpisode tempEpisode = SerieEpisodeFromWishEpisode (tempWishEpisode);
                tempEpisode.ParentSeason = copy;
                copy.Episodes.Add (tempEpisode);
            }
            return copy;
        }
        SerieEpisode SerieEpisodeFromWishEpisode (WishSerieEpisode wish) {
            SerieEpisode copy = new SerieEpisode ();
            copy.ID = wish.ID;
            copy.Name = wish.Name;
            copy.AirDate = wish.AirDate;
            copy.InternetLink = wish.InternetLink;
            copy.OrigName = wish.OrigName;
            copy.Summary = wish.Summary;
            copy.TrailerLink = wish.TrailerLink;
            return copy;
        }

        #region Sort / Filter

        private void searchWishSerie_TextChanged (object sender, TextChangedEventArgs e) {
            _wishSerieView.Filter = new Predicate<object> (FilterSerieByNames);
        }

        private void serieClearSearch_Click (object sender, RoutedEventArgs e) {
            searchWishSerie.Text = "";
        }

        private void serieSortCategory_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            SortDescription sortingWishSerie = new SortDescription ();
            _wishSerieView.SortDescriptions.Clear ();
            //WishSerie tempserie = new WishSerie ();
            //tempserie.WishSeasons.ElementAt(0).WishEpisodes.ElementAt(0).AirDate

            string sortBy = serieSortCategory.SelectedValue.ToString ();
            if (sortBy.Contains ("Po nazivu")) {
                sortingWishSerie.PropertyName = "OrigName";
            }
            else if (sortBy.Contains ("Po vremenu izlaska")) {
                sortingWishSerie.PropertyName = "WishSeasons.ElementAt(0).WishEpisodes.ElementAt(0).AirDate";
            }
            _wishSerieView.SortDescriptions.Add (sortingWishSerie);
            this.wishSerieListBox.DataContext = null;
            this.wishSerieListBox.DataContext = _wishSerieView;

        }
        bool FilterSerieByNames (object ob) {
            WishSerie serie = ob as WishSerie;
            return (serie.OrigName.ToLower ().Contains (searchWishSerie.Text));
        }

        #endregion

        private void seasonListBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            ListBox seasonListBox = sender as ListBox;
            WishSerie clickedSerie = seasonListBox.DataContext as WishSerie;
            WishSerieSeason clickedSeason = seasonListBox.SelectedItem as WishSerieSeason;
            try {
                _wishEpisodeView = CollectionViewSource.GetDefaultView (clickedSeason.WishEpisodes);
            }
            catch {
                _wishEpisodeView = null;
                return;
            }

            Grid grid = seasonListBox.Parent as Grid;
            TabItem seasonTab = grid.Parent as TabItem;
            TabControl serieEpisodeTabControl = seasonTab.Parent as TabControl;
            Grid mainGrid = serieEpisodeTabControl.Parent as Grid;            
            foreach (ListBox tempListBox in FindTreeControls.FindChildren<ListBox> (serieEpisodeTabControl)) {
                if (tempListBox.Name == "episodesListBox")
                    if (_wishEpisodeView == null) {
                        tempListBox.Items.Clear ();
                        return;
                    }
                    else
                        tempListBox.DataContext = _wishEpisodeView;
            }
            foreach (TextBlock tempTB in FindTreeControls.FindChildren<TextBlock> (serieEpisodeTabControl)) {                
                if (tempTB.Name == "episodeReleaseDate") {
                    tempTB.DataContext = _wishEpisodeView;
                }
            }
            foreach (TabItem tab in FindTreeControls.FindChildren<TabItem> (serieEpisodeTabControl)) {
                if (tab.Name == "episodesTab")
                    tab.Header = string.Format ("{0}: Episode", clickedSeason.Name);
            }
        }

        #endregion

        private childItem FindVisualChild<childItem> (DependencyObject obj)
        where childItem: DependencyObject {

            for (int i = 0 ; i < VisualTreeHelper.GetChildrenCount (obj) ; i++) {

                DependencyObject child = VisualTreeHelper.GetChild (obj, i);

                if (child != null && child is childItem)

                    return (childItem) child;

                else {

                    childItem childOfChild = FindVisualChild<childItem> (child);

                    if (childOfChild != null)

                        return childOfChild;

                }

            }

            return null;

        }
    }
}
