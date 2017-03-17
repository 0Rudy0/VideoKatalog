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
using System.Web;
using System.Net;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows.Threading;
using System.ComponentModel;
using System.Resources;
using System.Reflection;
using FolderPickerLib;
using System.Data.SqlServerCe;

namespace Video_katalog {    
    public partial class SerieForm: Window {

        #region GLOBAL DATA
        
        SqlCeConnection connection = new SqlCeConnection (ComposeConnectionString.ComposeCEString ());        
        ResourceManager resources = new ResourceManager ("items", Assembly.GetExecutingAssembly ());
        Cloner cloner = new Cloner ();
        Settings settings = new Settings ();
        ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();
        
        SerieCategory serieShowCat;
        SerieCategory passCategory;
        AddOrEdit addOrEdit;
        WishOrRegular wishOrRegular;
        
        Serie downloadedSerie = new Serie ();
        List<Person> allSerieCast = new List<Person> ();
        string notFetched = "";
        bool refreshing = false;
        Serie passedSerie = new Serie ();
        public bool serieExists = false;
        Thread threadDownloadData;
        DispatcherTimer timerCheckDownloadingThread = new DispatcherTimer ();
        ICollectionView _seasonView;
        ICollectionView _episodeView;
        int maxGenreStringLength = 35;
        public bool changedVideo = false;

        public List<HDD> insertedHDDs = new List<HDD> ();
        public bool accepted = false;
        public Serie currSerie = new Serie ();

        #endregion

        #region search strings
                
        string youtubeTrailerSearchPrefix = "http://www.youtube.com/results?search_query=";
        string youtubeTrailerSearchSuffix = "+trailer&aq=f";
        string googleTrailerSearchPrefix = "http://www.google.com/webhp?hl=hr#sclient=psy&hl=hr&site=webhp&source=hp&q=";
        string googleTrailerSearchSuffix = "+trailer&aq=f&aqi=&aql=&oq=&pbx=1&fp=ebd6b5bbc36e5c9c";

        string impAwardsPosterSearchPrefix = "http://www.impawards.com/googlesearch.html?cx=partner-pub-6811780361519631%3A48v46vdqqnk&cof=FORID%3A9&ie=ISO-8859-1&q=";
        string impAwardsPosterSearchSuffix = "&sa=Search&siteurl=www.impawards.com%252F#860";
        string googlePosterSearchPrefix = "http://www.google.hr/search?q=";
        string googlePosterSearchSuffix = "+poster&hl=hr&client=firefox-a&hs=fVv&rls=org.mozilla:en-US:official&prmd=ivns&tbm=isch&tbo=u&source=univ&sa=X&ei=HqiyTfHHCMiZOrORwYAJ&ved=0CBkQsAQ&biw=1440&bih=925";
        string posterDBSearchPrefix = "http://www.movieposterdb.com/movie/";

        string posterSourceBeginsWith1 = "<td class=\"poster\" id=\"";
        string posterSourceEndsWith1 = "<td class=\"poster\" id=\"";
        string posterSourceBeginsWith2 = "<img src=\"";
        string posterSourceEndsWith2 = "\"";

        #endregion

        #region ON LOAD
        
        public SerieForm (Serie serieToEdit, SerieCategory passCategory, AddOrEdit addOrEdit) {
            this.passCategory = passCategory;
            this.addOrEdit = addOrEdit;
            this.currSerie = serieToEdit;
            this.passedSerie = cloner.CloneSerieWithSeasons (serieToEdit);
            this.wishOrRegular = WishOrRegular.regular;

            InitializeComponent ();            
            
            FillGenresCheckBoxes (currSerie.Genres);
            foreach (Person actor in currSerie.Actors)
                allSerieCast.Add (actor);
            _seasonView = CollectionViewSource.GetDefaultView (currSerie.Seasons);
            //_seasonView.SortDescriptions.Add (new SortDescription (Name, ListSortDirection.Ascending));
            this.seasonListBox.DataContext = _seasonView;

            //this.releaseDateGrid.Visibility = System.Windows.Visibility.Hidden;
        }
        public SerieForm (WishSerie wishSerieToEdit, SerieCategory passCategory, AddOrEdit addOrEdit) {
            this.passCategory = passCategory;
            this.addOrEdit = addOrEdit;
            this.currSerie = SerieFromWishSerie (wishSerieToEdit);
            this.passedSerie = cloner.CloneSerieWithSeasons (this.currSerie);
            this.wishOrRegular = WishOrRegular.wish;

            InitializeComponent ();

            FillGenresCheckBoxes (currSerie.Genres);
            foreach (Person actor in currSerie.Actors)
                allSerieCast.Add (actor);
            _seasonView = CollectionViewSource.GetDefaultView (currSerie.Seasons);
            //_seasonView.SortDescriptions.Add (new SortDescription (Name, ListSortDirection.Ascending));
            this.seasonListBox.DataContext = _seasonView;

            serieTechInfoGrid.IsEnabled = false;
            serieTechInfoGrid.Visibility = System.Windows.Visibility.Hidden;
            this.mainWindow.Width = 974;
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            timerCheckDownloadingThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            busyIndicator.BusyContent = "Skidanje potrebnih web stranica...\r\n";
            castLimit.Value = 20;
            hddList = DatabaseManager.FetchHDDList ();
            settings = DatabaseManager.FetchSettings ();
            LoadSettings ();
            foreach (HDD tempHDD in hddList)
                hddComboBox.Items.Add (tempHDD.Name);
            this.DataContext = currSerie;
            setToMainInfos_Click (null, null);

            //ako dodajemo novu seriju
            if (this.addOrEdit == AddOrEdit.add && passCategory == SerieCategory.serie) {
                StartDownloadSetBusyState (new EventHandler (SetAllInfoAfterDownload), new ThreadStart (DownloadInfoAndPoster));
            }
            //ako dodajemo novu sezonu/epizodu
            else if (this.addOrEdit == AddOrEdit.add) {
                StartDownloadSetBusyState (new EventHandler (SetEpisodesAfterDownload), new ThreadStart (DownloadEpisodes));
                _seasonView.MoveCurrentToPosition (0);
                this.seasonListBox.Focus ();
            }
            //ako dodajemo novu epizodu
            if (this.addOrEdit == AddOrEdit.add && this.passCategory == SerieCategory.episode) {
                this.addSeason.IsEnabled = false;
                this.removeSeason.IsEnabled = false;
                _seasonView.MoveCurrentToPosition (0);
                _episodeView.MoveCurrentToPosition (0);
                this.episodeListBox.Focus ();
            }
            //ako radimo izmjenu serije
            else if (this.addOrEdit == AddOrEdit.edit && this.passCategory == SerieCategory.serie) {
                //_seasonView.MoveCurrentToPosition (0);
            }
            //ako radimo izmjenu sezone
            else if (this.addOrEdit == AddOrEdit.edit && this.passCategory == SerieCategory.season) {
                this.addSeason.IsEnabled = false;
                this.removeSeason.IsEnabled = false;
                _seasonView.MoveCurrentToPosition (0);
                _episodeView.MoveCurrentToPosition (-1);
                this.seasonListBox.Focus ();
            }
            //ako radimo izmjenu epizode
            else if (this.addOrEdit == AddOrEdit.edit && this.passCategory == SerieCategory.episode) {
                this.addSeason.IsEnabled = false;
                this.removeSeason.IsEnabled = false;
                this.addEpisode.IsEnabled = false;
                this.removeEpisode.IsEnabled = false;
                this.seasonListBox.IsEnabled = false;
                _seasonView.MoveCurrentToPosition (0);
                _episodeView.MoveCurrentToPosition (0);
                this.episodeListBox.Focus ();
                this.episodeListBox.SelectedIndex = 0;
            }
            if (this.passCategory == SerieCategory.episode || this.passCategory == SerieCategory.season) {
                this.customNameTextBox.IsEnabled = false;
                this.originalNameTextBox.IsEnabled = false;
                this.directorListBox.IsEnabled = false;
                this.castListBox.IsEnabled = false;
                this.addDirector.IsEnabled = false;
                this.removeDirector.IsEnabled = false;
                this.addActor.IsEnabled = false;
                this.removeActor.IsEnabled = false;
                this.genresComboBox.IsEnabled = false;
                this.ratingTextBox.IsEnabled = false;
                this.searchPosterBTN.IsEnabled = false;
                this.pasteImageButton.IsEnabled = false;
                this.copyAndOpenEditorBTN.IsEnabled = false;
                this.imageQualityLevelSlider.IsEnabled = false;
                this.setInfoToMain.IsEnabled = false;
            }
            if (this.addOrEdit == AddOrEdit.edit || 
                (this.addOrEdit == AddOrEdit.add && this.serieShowCat == SerieCategory.season) || 
                (this.addOrEdit == AddOrEdit.add && this.serieShowCat == SerieCategory.episode))

                try {
                    this.posterImage.Source = Clipboard.GetImage ();
                }
                catch {
                }

        }
        void LoadSettings () {
            if (settings.UseOnlyOriginalName) {
                useOriginalNameCB.IsChecked = true;
            }
            if (settings.PosterProvider == "imp") {
                posterImpAwardsRadio.IsChecked = true;
            }
            else if (settings.PosterProvider == "posterDB") {
                posterMoviePosterDBRadio.IsChecked = true;
            }
            else if (settings.PosterProvider == "google") {
                posterGoogleRadio.IsChecked = true;
            }
            if (settings.TrailerProvider == "youtube") {
                trailerYoutubeRadio.IsChecked = true;
            }
            else if (settings.TrailerProvider == "google") {
                trailerGoogleRadio.IsChecked = true;
            }
            imageQualityLevelSlider.Value = this.settings.ImageQualityLevel;
        }

        #endregion

        #region IMDB INFO

        private void useOriginalNameCB_Checked (object sender, RoutedEventArgs e) {
            this.settings.UseOnlyOriginalName = true;
            this.currSerie.Name = this.currSerie.OrigName.Replace (":", "- ");
            DatabaseManager.UpdateSettings (settings);
        }
        private void useOriginalNameCB_Unchecked (object sender, RoutedEventArgs e) {
            this.settings.UseOnlyOriginalName = false;
            DatabaseManager.UpdateSettings (settings);
        }

        //******   CAST AND CREW    ******        
        private void addActor_Click (object sender, RoutedEventArgs e) {
            InputDialog inputActor = new InputDialog ("Novi glumac");
            inputActor.Owner = this;
            inputActor.ShowDialog ();
            if (String.IsNullOrWhiteSpace (inputActor.inputString)) {
                return;
            }
            else {
                Person newActor = new Person (new PersonType (2));
                newActor.Name = inputActor.inputString;
                if (currSerie.Actors.Contains (newActor, new PersonEqualityComparerByName ())) {
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Osoba vec postoji");
                    return;
                }
                int maxActors = (int) castLimit.Value;
                if (allSerieCast.Count < maxActors) {
                    allSerieCast.Add (newActor);
                    return;
                }
                else if (allSerieCast.Count == maxActors) {
                    allSerieCast.Add (newActor);
                }
                else {
                    allSerieCast.Insert (maxActors, newActor);
                }
                castLimit.Value++;
                castListBox.Focus ();
            }
            RefreshCastList ();
        }
        private void removeActor_Click (object sender, RoutedEventArgs e) {
            int index = castListBox.SelectedIndex;
            try {
                if (currSerie.Actors.Count == 1)
                    //return;
                currSerie.Actors.RemoveAt (castListBox.SelectedIndex);
                if (index > currSerie.Actors.Count - 1)
                    index--;
                castListBox.SelectedIndex = index;
                castListBox.Focus ();
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi glumca");
            }
        }
        private void addDirector_Click (object sender, RoutedEventArgs e) {
            InputDialog inputDirector = new InputDialog ("Novi redatelj");
            inputDirector.Owner = this;
            inputDirector.ShowDialog ();
            if (String.IsNullOrWhiteSpace (inputDirector.inputString)) {
                return;
            }
            else {
                Person newDir = new Person (new PersonType (2));
                newDir.Name = inputDirector.inputString;
                currSerie.Directors.Add (newDir);
                directorListBox.Focus ();
            }
        }
        private void removeDirector_Click (object sender, RoutedEventArgs e) {
            try {
                if (currSerie.Directors.Count == 1)
                    //return;
                currSerie.Directors.RemoveAt (directorListBox.SelectedIndex);
                directorListBox.Focus ();
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi redatelja");
            }
        }
        private void RefreshCastList () {
            currSerie.Actors.Clear ();
            int maxActors = (int) castLimit.Value;
            for (int i = 0 ; i < maxActors && i < allSerieCast.Count ; i++) {
                currSerie.Actors.Add (allSerieCast[i]);
            }
        }
        private void castLimit_ValueChanged (object sender, RoutedPropertyChangedEventArgs<object> e) {
            if (castLimit.Value < 1)
                castLimit.Value = 1;
            RefreshCastList ();
        }

        //******  TRAILER AND INTERNET  ******//
        private void openInternetLink_Click (object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start (this.internetLinkTextBox.Text);
            }
            catch {
            }
        }
        private void pasteInternetLink_Click_1 (object sender, RoutedEventArgs e) {
            if (serieShowCat == SerieCategory.episode) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
                selectedEpisode.InternetLink = Clipboard.GetText ();
            }
            else if (serieShowCat == SerieCategory.season) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                selectedSeason.InternetLink = Clipboard.GetText ();
            }
            else if (serieShowCat == SerieCategory.serie) {
                this.currSerie.InternetLink = Clipboard.GetText ();
            }            
        }
        private void openTrailerLink_Click (object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start (this.trailerLinkTextBox.Text);
            }
            catch {
            }
        }
        private void pasteTrailerLink_Click_1 (object sender, RoutedEventArgs e) {
            if (serieShowCat == SerieCategory.episode) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
                selectedEpisode.TrailerLink = Clipboard.GetText ();
            }
            else if (serieShowCat == SerieCategory.season) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                selectedSeason.TrailerLink = Clipboard.GetText ();
            }
            else if (serieShowCat == SerieCategory.serie) {
                this.currSerie.TrailerLink = Clipboard.GetText ();
            }  
        }
        private void searchTrailer_Click (object sender, RoutedEventArgs e) {
            if (serieShowCat == SerieCategory.episode) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
                if (trailerYoutubeRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.youtubeTrailerSearchPrefix + 
                        currSerie.OrigName + "+" + selectedSeason.Name + "+" + selectedEpisode.OrigName + this.youtubeTrailerSearchSuffix);
                    this.settings.TrailerProvider = "youtube";
                }
                else if (trailerGoogleRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.googleTrailerSearchPrefix +
                        currSerie.OrigName + "+" + selectedSeason.Name + "+" + selectedEpisode.OrigName + this.googleTrailerSearchSuffix);
                    this.settings.TrailerProvider = "google";
                }
            }
            else if (serieShowCat == SerieCategory.season) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);                
                if (trailerYoutubeRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.youtubeTrailerSearchPrefix + 
                        currSerie.OrigName + "+" + selectedSeason.Name + this.youtubeTrailerSearchSuffix);
                    this.settings.TrailerProvider = "youtube";
                }
                else if (trailerGoogleRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.googleTrailerSearchPrefix + 
                        currSerie.OrigName + "+" + selectedSeason.Name + this.googleTrailerSearchSuffix);
                    this.settings.TrailerProvider = "google";
                }
            }
            else if (serieShowCat == SerieCategory.serie) {
                if (trailerYoutubeRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.youtubeTrailerSearchPrefix + currSerie.OrigName + this.youtubeTrailerSearchSuffix);
                    this.settings.TrailerProvider = "youtube";
                }
                else if (trailerGoogleRadio.IsChecked == true) {
                    System.Diagnostics.Process.Start (this.googleTrailerSearchPrefix + currSerie.OrigName + this.googleTrailerSearchSuffix);
                    this.settings.TrailerProvider = "google";
                }
            }
            DatabaseManager.UpdateSettings (settings);
        }

        //******   POSTER MANAGEMENT   ******//
        private void searchPoster_Click (object sender, RoutedEventArgs e) {
            if (posterImpAwardsRadio.IsChecked == true) {
                System.Diagnostics.Process.Start (this.impAwardsPosterSearchPrefix + currSerie.OrigName + this.impAwardsPosterSearchSuffix);
                this.settings.PosterProvider = "imp";
            }
            else if (posterGoogleRadio.IsChecked == true) {
                System.Diagnostics.Process.Start (this.googlePosterSearchPrefix + currSerie.OrigName + this.googlePosterSearchSuffix);
                this.settings.PosterProvider = "google";
            }
            else if (posterMoviePosterDBRadio.IsChecked == true) {
                string ImdbID = null;
                this.settings.PosterProvider = "posterDB";
                foreach (char c in currSerie.InternetLink) {
                    if (Char.IsDigit (c)) {
                        ImdbID += c;
                    }
                }
                System.Diagnostics.Process.Start (this.posterDBSearchPrefix + ImdbID);
            }
            DatabaseManager.UpdateSettings (settings);
        }
        private void pasteImageButton_Click (object sender, RoutedEventArgs e) {
            posterImage.Source = Clipboard.GetImage ();
        }
        private void copyAndOpenEditorBTN_Click (object sender, RoutedEventArgs e) {
            if (this.settings.ImageEditorPath == "x") {
                Microsoft.Win32.OpenFileDialog browse = new Microsoft.Win32.OpenFileDialog ();
                browse.DefaultExt = ".exe";
                browse.Filter = "Executable files (.exe)|*.exe";
                Nullable<bool> result = browse.ShowDialog ();
                if (result == true) {
                    this.settings.ImageEditorPath = browse.FileName;
                    DatabaseManager.UpdateSettings (settings);
                }
            }
            try {
                Clipboard.SetImage ((BitmapSource) posterImage.Source);
            }
            catch {
            }
            System.Diagnostics.Process.Start (this.settings.ImageEditorPath);
        }
        private void imageQualityLevelSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            this.settings.ImageQualityLevel = (int) imageQualityLevelSlider.Value;
        }

        //*******    GENRE MANAGER    ******//
        private void FillGenresCheckBoxes (ObservableCollection<Genre> genres) {
            if (genres.Contains (new Genre ("Akcija"), new GenreEqualityComparer ()))
                actionCheckBox.IsChecked = true;
            else
                actionCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Avantura"), new GenreEqualityComparer ()))
                adventureCheckBox.IsChecked = true;
            else
                adventureCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Animacija"), new GenreEqualityComparer ()))
                animationCheckBox.IsChecked = true;
            else
                animationCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Biografija"), new GenreEqualityComparer ()))
                biographyCheckBox.IsChecked = true;
            else
                biographyCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Komedija"), new GenreEqualityComparer ()))
                comedyCheckBox.IsChecked = true;
            else
                comedyCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Krimi"), new GenreEqualityComparer ()))
                crimeCheckBox.IsChecked = true;
            else
                crimeCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Dokumentarni"), new GenreEqualityComparer ()))
                documentaryCheckBox.IsChecked = true;
            else
                documentaryCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Drama"), new GenreEqualityComparer ()))
                dramaCheckBox.IsChecked = true;
            else
                dramaCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Obiteljski"), new GenreEqualityComparer ()))
                familyCheckBox.IsChecked = true;
            else
                familyCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Fantastika"), new GenreEqualityComparer ()))
                fantasyCheckBox.IsChecked = true;
            else
                fantasyCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Crno-bijeli"), new GenreEqualityComparer ()))
                filmNoirCheckBox.IsChecked = true;
            else
                filmNoirCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Povijesni"), new GenreEqualityComparer ()))
                historyCheckBox.IsChecked = true;
            else
                historyCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Horor"), new GenreEqualityComparer ()))
                horrorCheckBox.IsChecked = true;
            else
                horrorCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Muzički"), new GenreEqualityComparer ()))
                musicCheckBox.IsChecked = true;
            else
                musicCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Pjevan"), new GenreEqualityComparer ()))
                musicalCheckBox.IsChecked = true;
            else
                musicalCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Misterija"), new GenreEqualityComparer ()))
                misteryCheckBox.IsChecked = true;
            else
                misteryCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Romantika"), new GenreEqualityComparer ()))
                romanceCheckBox.IsChecked = true;
            else
                romanceCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Znanstvena fantastika"), new GenreEqualityComparer ()))
                sciFiCheckBox.IsChecked = true;
            else
                sciFiCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Sportski"), new GenreEqualityComparer ()))
                sportCheckBox.IsChecked = true;
            else
                sportCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Triler"), new GenreEqualityComparer ()))
                thrillerCheckBox.IsChecked = true;
            else
                thrillerCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Ratni"), new GenreEqualityComparer ()))
                warCheckBox.IsChecked = true;
            else
                warCheckBox.IsChecked = false;

            if (genres.Contains (new Genre ("Vestern"), new GenreEqualityComparer ()))
                westernCheckBox.IsChecked = true;
            else
                westernCheckBox.IsChecked = false;
        }
        private void RefreshGenres (object sender, RoutedEventArgs e) {

            CheckBox senderCheckBox = (CheckBox) sender;
            string genreName = senderCheckBox.Content.ToString ();
            Genre tempGenre = new Genre (genreName);
            if (senderCheckBox.IsChecked == true && currSerie.Genres.Contains (tempGenre, new GenreEqualityComparer ()) == false) {
                currSerie.Genres.Add (tempGenre);
            }
            else if (senderCheckBox.IsChecked == false && currSerie.Genres.Contains (tempGenre, new GenreEqualityComparer ()) == true) {
                for (int i = 0 ; i < currSerie.Genres.Count ; i++) {
                    if (tempGenre.Name == currSerie.Genres[i].Name) {
                        currSerie.Genres.RemoveAt (i);
                    }
                }
            }
            if (senderCheckBox.IsChecked == true) {
                senderCheckBox.FontWeight = FontWeights.Bold;
                senderCheckBox.Foreground = new SolidColorBrush (Colors.Black);
                senderCheckBox.FontSize = 13.667;
            }
            else if (senderCheckBox.IsChecked == false) {
                senderCheckBox.FontWeight = FontWeights.Normal;
                senderCheckBox.Foreground = new SolidColorBrush (Colors.DimGray);
                senderCheckBox.FontSize = 13.333;
            }
            Converters.GenresToStringConverter converter = new Converters.GenresToStringConverter ();
            string genresInString = converter.Convert (currSerie.Genres, typeof (string), maxGenreStringLength, null).ToString ();
            if (genresInString.Contains ("...")) {
                genresComboBox.Text = currSerie.Genres.Count + "/22 odabrano";
            }
            else {
                genresComboBox.Text = genresInString;
            }
        }
        private void genresComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            genresComboBox.SelectedIndex = -1;
        }

        //******   ADD / REMOVE - SEASON / EPISODE   ****//
        private void addSeason_Click (object sender, RoutedEventArgs e) {
            try {
                InputDialog newSeasonDialog = new InputDialog ("Ime nove sezone", string.Format ("Season {0}", (currSerie.Seasons.Count + 1).ToString ("00.")));
                newSeasonDialog.Owner = this;
                newSeasonDialog.ShowDialog ();
                if (newSeasonDialog.accepted) {
                    SerieSeason newSeason = new SerieSeason ();
                    newSeason.Name = newSeasonDialog.inputString;
                    newSeason.ParentSerie = this.currSerie;
                    currSerie.Seasons.Add (newSeason);
                    sortSeasons ();
                }
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Neuspješno dodavanje sezone", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void editSeason_Click (object sender, RoutedEventArgs e) {
            try {
                int seasonIndex = seasonListBox.SelectedIndex;
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonIndex);
                InputDialog seasonNameDialog = new InputDialog ("Naziv sezone", selectedSeason.Name);
                seasonNameDialog.Owner = this;
                seasonNameDialog.ShowDialog ();
                if (seasonNameDialog.accepted) {
                    selectedSeason.Name = seasonNameDialog.inputString;
                    _seasonView.Refresh ();
                    sortSeasons ();
                }
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi sezonu", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void removeSeason_Click (object sender, RoutedEventArgs e) {            
            int seasonIndex = seasonListBox.SelectedIndex;
            try {
                currSerie.Seasons.RemoveAt (seasonIndex);
                if (currSerie.Seasons.Count == 0) { //ako su maknute sve sezone, dodaj novu, praznu
                    SerieSeason newseason = new SerieSeason ();
                    newseason.Name = "Season 1";
                    currSerie.Seasons.Add (newseason);
                    seasonListBox.SelectedIndex = 0;
                    return;
                }
                if (seasonIndex == currSerie.Seasons.Count) //ako je bila oznacena zadnja sezona na listi
                    seasonListBox.SelectedIndex = seasonIndex - 1;
                else
                    seasonListBox.SelectedIndex = seasonIndex;
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odabrati sezonu", "Nije odabrana ni jedna sezona", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void addEpisode_Click (object sender, RoutedEventArgs e) {
            try {
                InputDialog newEpisodeDialog = new InputDialog ("Nova epizoda", string.Format ("Ep {0} - ", (currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.Count + 1).ToString ("00.")));
                newEpisodeDialog.Owner = this;
                newEpisodeDialog.ShowDialog ();
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                if (newEpisodeDialog.accepted) {
                    SerieEpisode newEpisode = new SerieEpisode ();
                    newEpisode.ParentSeason = selectedSeason;
                    newEpisode.AirDate = new DateTime (1800, 1, 1);                    
                    newEpisode.Name = newEpisodeDialog.inputString;
                    if (newEpisode.Name.IndexOf ("-") > 0)
                        newEpisode.OrigName = newEpisode.Name.Substring (newEpisode.Name.IndexOf ("-") + 1).Trim ();
                    else
                        newEpisode.OrigName = newEpisode.Name;
                    selectedSeason.Episodes.Add (newEpisode);
                    episodeListBox.SelectedItem = newEpisode;
                    episodeListBox.Focus ();
                    sortEpisodes (selectedSeason);
                }
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Neuspješno dodavanje epizode", "", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void editEpisode_Click (object sender, RoutedEventArgs e) {
            try {
                SerieEpisode selectedEpisode = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.ElementAt (episodeListBox.SelectedIndex);
                InputDialog episodeNameDialog = new InputDialog ("Unesi naziv epizode", selectedEpisode.Name);
                episodeNameDialog.Owner = this;
                episodeNameDialog.ShowDialog ();
                if (episodeNameDialog.accepted) {
                    selectedEpisode.Name = episodeNameDialog.inputString;
                    _episodeView.Refresh ();
                    sortEpisodes (selectedEpisode.ParentSeason);
                }
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi epizodu", "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void removeEpisode_Click (object sender, RoutedEventArgs e) {
            try {
                int episodeIndex = episodeListBox.SelectedIndex;
                currSerie.Seasons.ElementAt(seasonListBox.SelectedIndex).Episodes.RemoveAt (episodeIndex);
                episodeListBox.SelectedIndex = episodeIndex;

            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odabrati epizodu");
            }
            
        }        
       
        #endregion

        #region DOWNLOAD/REFRESH INFO

        //REFRESH CLICKS
        private void StartDownloadSetBusyState (EventHandler timerTickEvent, ThreadStart threadStartFunction) {
            refreshSplitButton.IsOpen = false;
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = "Skidanje potrebnih web stranica...\r\n";
            threadDownloadData = new Thread (threadStartFunction);
            threadDownloadData.Start ();
            timerCheckDownloadingThread = new DispatcherTimer ();
            timerCheckDownloadingThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            timerCheckDownloadingThread.Tick += timerTickEvent;
            timerCheckDownloadingThread.Start ();
            notFetched = "";
        }
        private void refreshButton_Click (object sender, RoutedEventArgs e) {
            refreshing = true;
            StartDownloadSetBusyState (new EventHandler (SetAllInfoAfterDownload), new ThreadStart (DownloadInfoAndPoster));            
        }
        private void refreshAllButPoster_Click (object sender, RoutedEventArgs e) {
            refreshing = true;
            StartDownloadSetBusyState (new EventHandler (SetAllInfoAfterDownload), new ThreadStart (DownloadAllInfo));
        }
        private void refreshOnlyPoster_Click (object sender, RoutedEventArgs e) {
            refreshing = true;
            StartDownloadSetBusyState (new EventHandler (SetBusyIndicatorAfterDownload), new ThreadStart (DownloadAndSetPoster));            
            busyIndicator.BusyContent = "Skidanje postera...";            
        }
        private void refreshCastAndCrew_Click (object sender, RoutedEventArgs e) {
            refreshing = true;       
            StartDownloadSetBusyState (new EventHandler (SetCastAndCrewAfterDownload), new ThreadStart (DownloadCastAndCrew));
        }
        private void refreshRating_Click (object sender, RoutedEventArgs e) {
            refreshing = true;        
            StartDownloadSetBusyState (new EventHandler (SetRatingAfterDownload), new ThreadStart (DownloadRating));
        }
        private void refreshGenres_Click (object sender, RoutedEventArgs e) {
            refreshing = true;         
            StartDownloadSetBusyState (new EventHandler (SetGenresAfterDownload), new ThreadStart (DownloadGenres));
        }
        private void refreshSummary_Click (object sender, RoutedEventArgs e) {
            refreshing = true;         
            StartDownloadSetBusyState (new EventHandler (SetSummaryAfterDownload), new ThreadStart (DownloadSummary));
        }
        private void refreshEpisodes_Click (object sender, RoutedEventArgs e) {
            refreshing = true;          
            StartDownloadSetBusyState (new EventHandler (SetEpisodesAfterDownload), new ThreadStart (DownloadEpisodes));
        }
        private void DownloadInfoAndPoster () {           
            DownloadAllInfo ();
            DownloadAndSetPoster ();
        }

        //DOWNLOADES     
        bool downloadingAll = false;    
        private void DownloadAllInfo () {
            IMDb webPage = new IMDb ();
            downloadedSerie = webPage.GetSerieInfo (this.currSerie.InternetLink);
            this.notFetched = webPage.notFetched;
            downloadingAll = true;
        }        
        private void DownloadCastAndCrew () {
            string mainSource = ReturnWebPageSource (currSerie.InternetLink);
            string castAndCrewSource = ReturnWebPageSource (currSerie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixFullCredits);
            IMDb webPage = new IMDb ();
            downloadedSerie.Directors = webPage.GetDirectors (castAndCrewSource);
            downloadedSerie.Actors = webPage.GetCast (mainSource, castAndCrewSource);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadRating () {
            string source = ReturnWebPageSource (currSerie.InternetLink);
            IMDb webPage = new IMDb ();
            downloadedSerie.Rating = webPage.GetRating (source);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadGenres () {
            string source = ReturnWebPageSource (currSerie.InternetLink);
            IMDb webPage = new IMDb ();
            downloadedSerie.Genres = webPage.GetGenres (source);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadSummary () {
            string mainSource = ReturnWebPageSource (currSerie.InternetLink);
            string summarySource = ReturnWebPageSource (currSerie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixSummary);
            IMDb webPage = new IMDb ();
            downloadedSerie.Summary = webPage.GetSummary (mainSource, summarySource);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadAndSetPoster () {            
            try {                
                posterImage.Dispatcher.Invoke (DispatcherPriority.Normal, new Action (
                    delegate () {
                        posterImage.Source = new BitmapImage (new Uri (HelpFunctions.GetPosterImageUrl (currSerie.InternetLink)));
                    }
                ));
            }
            catch {
                this.notFetched += "\r\nPoster";
            }            
        }
        private void DownloadEpisodes () {
            //string source = ReturnWebPageSource (currSerie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixEpisodes);
            IMDb webPage = new IMDb ();
            downloadedSerie.Seasons = webPage.GetSerieSeasons (currSerie.InternetLink, this.currSerie);
            //foreach (SerieSeason tempSeason in downloadedSerie.Seasons)
            //    tempSeason.InternetLink = this.currSerie.InternetLink + tempSeason.InternetLink;
            
            this.notFetched += webPage.notFetched;
        }
        
        //SETTING MOVIE INFO
        private void SetAllInfo () {
            if (this.notFetched.Length != 0) {
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            SetCastAndCrew ();
            SetGenres ();            
            SetRating ();
            SetSummary ();            
            currSerie.Name = downloadedSerie.Name.Replace (":", " -");
            currSerie.OrigName = downloadedSerie.OrigName.Replace (":", " -");           
            if (this.settings.UseOnlyOriginalName) {
                currSerie.Name = currSerie.OrigName;
            }
            currSerie.TrailerLink = downloadedSerie.TrailerLink;
            SetEpisodes ();
            downloadingAll = false;
        }
        private void SetCastAndCrew () {
            if (downloadedSerie.Actors.Count == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currSerie.Actors.Clear ();
                foreach (Person actor in downloadedSerie.Actors) {
                    currSerie.Actors.Add (actor);
                }
                allSerieCast.Clear ();
                foreach (Person tempActor in currSerie.Actors) {
                    allSerieCast.Add (tempActor);
                }
                RefreshCastList ();
            }
            if (downloadedSerie.Directors.Count == 0) {
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Redatelji nisu dohvaćeni");
            }
            else {
                currSerie.Directors.Clear ();
                foreach (Person director in downloadedSerie.Directors) {
                    currSerie.Directors.Add (director);
                }
            }            
        }
        private void SetRating () {
            if (downloadedSerie.Rating == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currSerie.Rating = downloadedSerie.Rating;
            }
        }
        private void SetGenres () {
            if (downloadedSerie.Genres.Count == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currSerie.Genres.Clear ();
                foreach (Genre genre in downloadedSerie.Genres) {
                    currSerie.Genres.Add (genre);
                }
                FillGenresCheckBoxes (currSerie.Genres);
            }
        }
        private void SetSummary () {
            if (string.IsNullOrWhiteSpace (downloadedSerie.Summary) && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currSerie.Summary = downloadedSerie.Summary;
            }
        }
        private void SetEpisodes () {
            if (downloadedSerie.Seasons.Count == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
                return;
            }
            currSerie.Seasons.Clear ();

            List<SerieEpisode> episodesToRemove = new List<SerieEpisode> ();
            foreach (SerieSeason tempSeason in downloadedSerie.Seasons) {
                tempSeason.Name = AddLeadingZeroToSeasonNumber (tempSeason.Name);
                //makni sve dohvacene epizode koje jos nisu prikazane
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    if ((tempEpisode.AirDate > DateTime.Now) && this.wishOrRegular == WishOrRegular.regular)
                        episodesToRemove.Add (tempEpisode);
                }

                bool shoudAdd = true;
                bool foundSeason = false;

                //provjeri da li se ta sezona nalazi u passedSerie (passedSerie je prazna ako se dodaje nova serija)
                foreach (SerieSeason passedSeason in passedSerie.Seasons) {
                    if (passedSeason.Name == tempSeason.Name) {
                        foundSeason = true;
                        //tempSeason = passedSeason;
                        tempSeason.ID = passedSeason.ID;
                        tempSeason.ParentSerie = passedSeason.ParentSerie;
                        //ako se nalazi, provjeri da li dodajemo novu sezonu
                        if (passCategory == SerieCategory.season && this.addOrEdit == AddOrEdit.add) {
                            //Ako dodajemo, onda izbaci sve postojeće sezone iz skinutog, prema tome nemoj dodat ovu sezonu i idi na slijedecu
                            shoudAdd = false;
                            break;
                        }
                        else if (passCategory == SerieCategory.episode) {
                            //ako je epizoda u pitanju provjeri da li se nalazi u passedSerie i ovisno o tome da li dodajemo ili mijenjamo epizodu napravi akciju
                            foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                                bool foundEpisode = false;
                                foreach (SerieEpisode passedEpisode in passedSeason.Episodes) {
                                    if (tempEpisode.Name == passedEpisode.Name) {
                                        foundEpisode = true;
                                        tempEpisode.ID = passedEpisode.ID;
                                        break;
                                    }
                                }
                                if (this.addOrEdit == AddOrEdit.add && foundEpisode == true) {
                                    //ako dodajemo novu epizodu, onda nas ne zanimaju one koje se nalaze u passedSerie, pa njih nemoj dodavati
                                    episodesToRemove.Add (tempEpisode);
                                }
                                else if (this.addOrEdit == AddOrEdit.edit && foundEpisode == false) {
                                    //ako editiramo epizodu, onda nas zanima samo jedna epizoda, pa ako nije nadjena epizoda u passedSerie nemoj ju dodati
                                    episodesToRemove.Add (tempEpisode);
                                }
                            }
                        }

                        else if (passCategory == SerieCategory.serie) {
                            //po potrebi implementirati
                        }
                    }
                    else {                        
                    }
                }
                //ako radimo izmjenu sezone, nemoj dodati skinutu sezonu ako ne postoji u passedSerie, odnosno dodaj samo onu sezonu koja je u passedSerie
                if (foundSeason == false && (passCategory == SerieCategory.season || passCategory == SerieCategory.episode) && this.addOrEdit == AddOrEdit.edit)
                    shoudAdd = false;

                foreach (SerieEpisode ep in episodesToRemove)
                    tempSeason.Episodes.Remove (ep);
                //ako nakon micanja epizoda koje nisu prikazane ili ih passedSerie sadrzi, sezona ne sadrzi vise ni jendu epizodu, idi na sljedecu sezonu
                if (tempSeason.Episodes.Count < 1)
                    continue;

                if (shoudAdd)
                    currSerie.Seasons.Add (tempSeason);
                    
            }
            //ako se dodaje/mijenja epizoda, makni sve sezone osim one gdje se epizoda mijenja/dodaje
            if (passCategory == SerieCategory.episode) {
                List<SerieSeason> seasonsToRemove = new List<SerieSeason> ();
                foreach (SerieSeason tempSeason in currSerie.Seasons) {
                    if (tempSeason.Name != passedSerie.Seasons.ElementAt (0).Name)
                        seasonsToRemove.Add (tempSeason);
                }
                foreach (SerieSeason tempSeason in seasonsToRemove)
                    currSerie.Seasons.Remove (tempSeason);
                if (currSerie.Seasons.Count == 0)
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Sve prikazane epizode sezone su u katalogu", "Nema novih epizoda", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            _seasonView = CollectionViewSource.GetDefaultView (currSerie.Seasons);
            //_seasonView.SortDescriptions.Add (new SortDescription (Name, ListSortDirection.Ascending));
            this.seasonListBox.DataContext = _seasonView;

            setToMainInfos_Click (null, null);
            SetTrailers ();
            if (passCategory == SerieCategory.season) {
                this.seasonListBox.SelectedIndex = 0;
                this.seasonListBox.Focus ();
            }
            else if (passCategory == SerieCategory.episode) {
                this.seasonListBox.SelectedIndex = 0;
                this.episodeListBox.SelectedIndex = 0;
                this.seasonListBox.Focus ();
            }
        }
        private void SetTrailers () {
            currSerie.TrailerLink = "http://www.youtube.com/results?search_query=" + currSerie.OrigName.Replace(' ', '+') + "+trailer";
            foreach (SerieSeason tempSeason in currSerie.Seasons) {
                tempSeason.TrailerLink = "http://www.youtube.com/results?search_query=" + currSerie.OrigName.Replace(' ', '+') + "+" + tempSeason.Name + "+trailer";
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    tempEpisode.TrailerLink = this.youtubeTrailerSearchPrefix +
                        currSerie.OrigName + "+" + tempSeason.Name + "+" + tempEpisode.OrigName + this.youtubeTrailerSearchSuffix;
                }
            }
        }

        //TIMERS TICK
        private bool IsThreadFinishedSetBusyState () {
            if (threadDownloadData.IsAlive == false) {
                timerCheckDownloadingThread.Stop ();
                busyIndicator.IsBusy = false;
                return true;
            }
            else {
                return false;
            }
        }
        private void SetAllInfoAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetAllInfo ();
            }
        }
        private void SetCastAndCrewAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetCastAndCrew ();
            }
        }
        private void SetRatingAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetRating ();
            }
        }
        private void SetGenresAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetGenres ();
            }
        }
        private void SetSummaryAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetSummary ();
            }
        }
        private void SetEpisodesAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetEpisodes ();
            }
        }
        private void SetBusyIndicatorAfterDownload (object sender, EventArgs e) {
            IsThreadFinishedSetBusyState ();
        }

        #endregion

        #region CHANGING SEASON / EPISODE SELECTION

        private void seasonListBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            try {
                _episodeView = CollectionViewSource.GetDefaultView (((SerieSeason) _seasonView.CurrentItem).Episodes);
                //_episodeView.SortDescriptions.Add (new SortDescription (Name, ListSortDirection.Ascending));
            }
            catch {
                _episodeView = CollectionViewSource.GetDefaultView (new ObservableCollection<SerieEpisode> ());
                //_episodeView.SortDescriptions.Add (new SortDescription (Name, ListSortDirection.Ascending));
            }
            finally {
                this.episodeListBox.DataContext = _episodeView;
            }
            try {
                _episodeView.MoveCurrentToPosition (-1);
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                internetLinkTextBox.DataContext = selectedSeason;
                trailerLinkTextBox.DataContext = selectedSeason;
                summaryTextBox.DataContext = currSerie;
                string seasonNumber = "";
                foreach (char c in selectedSeason.Name) {
                    if (char.IsDigit (c))
                        seasonNumber += c.ToString ();
                }
                int number;
                try {
                    number = int.Parse (seasonNumber); 
                    trailerLinkLabel.Text = string.Format ("Trailer link sezone (S-{0})", number.ToString ("00."));
                    internetLinkLabel.Text = string.Format ("Internet link sezone (S-{0})", number.ToString ("00."));
                    loadLabel.Text = string.Format ("Učitaj folder: {0}", selectedSeason.Name);
                }
                catch {
                    trailerLinkLabel.Text = string.Format ("Trailer link sezone ({0})", selectedSeason.Name);
                    internetLinkLabel.Text = string.Format ("Internet link sezone ({0})", selectedSeason.Name);
                    loadLabel.Text = string.Format ("Učitaj folder: {0}", selectedSeason.Name);
                }
                
                summaryLabel.Text = string.Format ("Radnja serije");
                List<HDD> seasonHDDList = new List<HDD> ();
                foreach (SerieEpisode tempEpisode in selectedSeason.Episodes) {
                    if (seasonHDDList.Contains (tempEpisode.Hdd, new HDD ()))
                        continue;
                    seasonHDDList.Add (tempEpisode.Hdd);
                }
                serieShowCat = SerieCategory.none;
                if (seasonHDDList.Count == 1 && seasonHDDList.Contains (null) == false)
                    hddComboBox.SelectedItem = seasonHDDList[0].Name;
                else
                    hddComboBox.SelectedValue = null;
                serieShowCat = SerieCategory.season;

                //prodji kroz sve datume epizode, ako su isti, postavi datum izlaska na taj datum, ako nisu, datum izlaska je null
                List<DateTime> releaseDates = new List<DateTime> ();
                foreach (SerieEpisode tempEpisode in selectedSeason.Episodes)
                    if (releaseDates.Contains (tempEpisode.AirDate))
                        continue;
                    else
                        releaseDates.Add (tempEpisode.AirDate);
                if (releaseDates.Count == 1)
                    releaseDatePicker.SelectedDate = releaseDates.ElementAt (0);
                else
                    releaseDatePicker.SelectedDate = null;
            }
            catch {
            }
        }
        private void seasonListBox_GotFocus (object sender, RoutedEventArgs e) {
            if (serieShowCat != SerieCategory.season)
                seasonListBox_SelectionChanged (null, null);
            serieShowCat = SerieCategory.season;
            serieTechInfoGrid.DataContext = null;
        }

        private void episodeListBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            try {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
                internetLinkTextBox.DataContext = selectedEpisode;
                trailerLinkTextBox.DataContext = selectedEpisode;
                summaryTextBox.DataContext = selectedEpisode;
                if (selectedSeason.Name.Contains ("Season")) {
                    string seasonNumber = "";
                    foreach (char c in selectedSeason.Name) {
                        if (char.IsDigit (c))
                            seasonNumber += c.ToString ();
                    }
                    string episodeNumber = "";
                    foreach (char c in selectedEpisode.Name.Substring (0, selectedEpisode.Name.IndexOf ("-"))) {
                        if (char.IsDigit (c))
                            episodeNumber += c.ToString ();
                    }
                    int epNumber = int.Parse (episodeNumber);
                    trailerLinkLabel.Text = string.Format ("Trailer link epizode ({0}x{1})", seasonNumber, (epNumber).ToString ("00."));
                    internetLinkLabel.Text = string.Format ("Internet link epizode ({0}x{1})", seasonNumber, (epNumber).ToString ("00."));
                    summaryLabel.Text = string.Format ("Radnja epizode ({0}x{1})", seasonNumber, (epNumber).ToString ("00."));
                    loadLabel.Text = string.Format ("Učitaj epizodu: {0}x{1}", seasonNumber, (epNumber).ToString ("00."));
                }
                else {
                    trailerLinkLabel.Text = string.Format ("Trailer link epizode ({0}-{1})", selectedSeason.Name, selectedEpisode.Name);
                    internetLinkLabel.Text = string.Format ("Internet link epizode ({0}-{1})", selectedSeason.Name, selectedEpisode.Name);
                    summaryLabel.Text = string.Format ("Radnja epizode ({0}-{1})", selectedSeason.Name, selectedEpisode.Name);
                    loadLabel.Text = string.Format ("Učitaj epizodu: {0}-{1}", selectedSeason.Name, selectedEpisode.Name);
                }                             

                serieTechInfoGrid.DataContext = selectedEpisode;

                serieShowCat = SerieCategory.none;
                if (selectedEpisode.Hdd == null)
                    hddComboBox.SelectedValue = null;
                else
                    hddComboBox.SelectedItem = selectedEpisode.Hdd.Name;
                serieShowCat = SerieCategory.episode;
                serieTechInfoGrid.DataContext = selectedEpisode;

                releaseDatePicker.SelectedDate = selectedEpisode.AirDate;
            }
            catch {
            }
        }
        private void episodeListBox_GotFocus (object sender, RoutedEventArgs e) {
            if (serieShowCat != SerieCategory.episode)
                episodeListBox_SelectionChanged (null, null);
            serieShowCat = SerieCategory.episode;            
        }

        private void setToMainInfos_Click (object sender, RoutedEventArgs e) {
            serieShowCat = SerieCategory.serie;
            trailerLinkLabel.Text = "Trailer link serije";
            summaryLabel.Text = "Radnja serije";
            internetLinkLabel.Text = "Internet link serije";
            loadLabel.Text = "Učitaj folder serije";
            internetLinkTextBox.DataContext = currSerie;
            summaryTextBox.DataContext = currSerie;
            trailerLinkTextBox.DataContext = currSerie;

            List<HDD> serieHDDList = new List<HDD> ();
            foreach (SerieSeason tempSeason in currSerie.Seasons)
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    if (serieHDDList.Contains (tempEpisode.Hdd, new HDD ()) || tempEpisode.Hdd == null)
                        continue;
                    serieHDDList.Add (tempEpisode.Hdd);
                }
            serieShowCat = SerieCategory.none;
            if (serieHDDList.Count == 1 && serieHDDList.Contains (null) == false)
                hddComboBox.SelectedItem = serieHDDList[0].Name;
            else
                hddComboBox.SelectedValue = null;
            serieShowCat = SerieCategory.serie;
            serieTechInfoGrid.DataContext = null;

            //prodji kroz sve datume epizode, ako su isti, postavi datum izlaska na taj datum, ako nisu, datum izlaska je null
            List<DateTime> releaseDates = new List<DateTime> ();
            foreach (SerieSeason tempSeason in currSerie.Seasons)
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                    if (releaseDates.Contains (tempEpisode.AirDate))
                        continue;
                    else
                        releaseDates.Add (tempEpisode.AirDate);
            if (releaseDates.Count == 1)
                releaseDatePicker.SelectedDate = releaseDates.ElementAt (0);
            else
                releaseDatePicker.SelectedDate = null;
        }

        #endregion

        #region TECH INFO (GRID)

        private void addNewHdd_Click (object sender, RoutedEventArgs e) {
            HDDForm newHddForm = new HDDForm ();
            newHddForm.Owner = this;
            newHddForm.ShowDialog ();
            if (newHddForm.accepted) {
                DatabaseManager.InsertHDD (newHddForm.currHDD);
                DatabaseManagerMySql.InsertHDD (newHddForm.currHDD);
                hddList.Add (newHddForm.currHDD);
                hddComboBox.Items.Add (hddList);
                insertedHDDs.Add (newHddForm.currHDD);
            }
        }

        private void ScanFile (SerieEpisode currEpisode, string videoPath) {
            BusyTrue (currEpisode.ParentSeason.Name + "\r\n" + currEpisode.Name);
            SerieEpisode tempEpisode = cloner.CloneEpisode (currEpisode);
            tempEpisode.GetTechInfo (videoPath);
            currEpisode.AudioList = cloner.CloneAudioList (tempEpisode.AudioList);
            currEpisode.SubtitleLanguageList = cloner.CloneLangList (tempEpisode.SubtitleLanguageList);
            this.Dispatcher.Invoke (new Action (() => {
                currEpisode.Size = tempEpisode.Size;
                currEpisode.Runtime = tempEpisode.Runtime;
                currEpisode.Height = tempEpisode.Height;
                currEpisode.Width = tempEpisode.Width;
                currEpisode.Bitrate = tempEpisode.Bitrate;
                currEpisode.AspectRatio = tempEpisode.AspectRatio;
                currEpisode.AddTime = tempEpisode.AddTime;
            }));
            if (serieShowCat == SerieCategory.episode) {
                BusyFalse ();
            }
        }
        private void loadFileButton_Click (object sender, RoutedEventArgs e) {
            changedVideo = true;
            if (serieShowCat == SerieCategory.serie) {
                GetSerieTechInfo ();
            }
            else if (serieShowCat == SerieCategory.season) {
                GetSeasonTechInfo ();
            }
            else if (serieShowCat == SerieCategory.episode) {
                GetEpisodeTechInfo (); 
            }
        }
        private void GetSerieTechInfo () {            
            FolderPickerDialog selectSerieFolderDialog = new FolderPickerDialog ();
            selectSerieFolderDialog.Title = "Odaberi cijeli folder serije";
            //Xceed.Wpf.Toolkit.MessageBox.Show ("Svaka sezona mora biti u svom folderu unutar glavnog foldera serije, te sezone moraju biti posložene abecednim redom.\r\n" +
                //"Unutar svakog foldera sezone moraju biti sve epizode sezone posložene abecednim redom.",
                //"Obavijest", MessageBoxButton.OK, MessageBoxImage.Information);
            if (selectSerieFolderDialog.ShowDialog () == true) {
                ThreadStart starter = delegate { ThreadGetSerieTechInfo (selectSerieFolderDialog); };
                new Thread (starter).Start ();
            }
        }
        private void ThreadGetSerieTechInfo (FolderPickerDialog selectSerieFolderDialog) {
            BusyTrue ();

            //dohvati sve foldere unutar odabranog foldera
            //ako broj foldera nije jednak broju sezona, izbaci error i prekini unos
            DirectoryInfo dirInfo = new DirectoryInfo (selectSerieFolderDialog.SelectedPath);
            DirectoryInfo[] dirs = dirInfo.GetDirectories ();
            if (dirs.Count () != currSerie.Seasons.Count) {
                MessageBox.Show (string.Format ("Svaka sezona mora biti u svom folderu.\r\nBroj unesenih sezona: {0}\r\nBroj foldera: {1}", currSerie.Seasons.Count, dirs.Count ()),
                    "Nepoklapanje", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                BusyFalse ();
                return;
            }
            int countSeasons = 0;
            foreach (DirectoryInfo dir in dirs) {
                //za svaki folder dohvati sve video datoteke
                //ako se broj dohvacenih video datoteka ne poklapa sa broje epizoda sezone, prijavi gresku i odustani od unosa trenutne sezone
                FileInfo[] files = dir.GetFiles ();
                try {                   
                    GetFilesTechInfo (files, currSerie.Seasons.ElementAt (countSeasons).Episodes);
                }
                catch {
                }
                countSeasons++;
            }
            BusyFalse ();
        }
        private void GetSeasonTechInfo () {
            SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
            FolderPickerDialog selectSeasonFolderDialog = new FolderPickerDialog ();
            selectSeasonFolderDialog.Title = "Odaberi cijeli folder: " +  currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Name;
            if (selectSeasonFolderDialog.ShowDialog () == true) {
                DirectoryInfo dirInfo = new DirectoryInfo (selectSeasonFolderDialog.SelectedPath);
                try {
                    ThreadStart starter = delegate { 
                        GetFilesTechInfo (dirInfo.GetFiles (), selectedSeason.Episodes);
                        BusyFalse ();
                    };
                    new Thread (starter).Start ();
                    //GetFilesTechInfo (dirInfo.GetFiles (), selectedSeason.Episodes);
                }
                catch {
                    Xceed.Wpf.Toolkit.MessageBox.Show (string.Format ("Broj video datoteka unutar foldera sezone se ne poklapa sa brojem sezona"),
                        "Nepoklapanje", MessageBoxButton.OK, MessageBoxImage.Error);
                    busyIndicator.IsBusy = false;
                }
            }
        }
        private void GetEpisodeTechInfo () {
            SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
            SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
            Microsoft.Win32.OpenFileDialog browseFile = new Microsoft.Win32.OpenFileDialog ();
            browseFile.Filter = "Podržani video formati |*.avi;*.mkv;*.vob;*.vob;*.mpg;*.mpeg;*.mp4;*.mov;*.3gp;*.wmv;*.mpgv;*.mpv;*.m1v;*.m2v;*.asf;*.rmvb|Svi formati |*.*";
            Nullable<bool> result = browseFile.ShowDialog ();
            if (result == true) {
                string videoPath = browseFile.FileName;
                if (IsVideoFile (new FileInfo (videoPath))) {
                    ThreadStart starter = delegate { ScanFile (selectedEpisode, videoPath); };
                    new Thread (starter).Start ();
                    selectedEpisode.GetTechInfo (videoPath);
                }
                else
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Nije poznati video format.", "Nepoznati format", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
        private void GetFilesTechInfo (FileInfo[] files, ObservableCollection<SerieEpisode> episodes) {
            if (serieShowCat == SerieCategory.season) {
                BusyTrue (episodes.ElementAt (0).ParentSeason.Name);               
            }
            List<FileInfo> videoFiles = new List<FileInfo> ();
            foreach (FileInfo file in files) {
                if (IsVideoFile (file)) {
                    videoFiles.Add (file);
                }
            }
            if (videoFiles.Count != episodes.Count) {
                MessageBox.Show ("Nepoklapanje broja epizoda sezone: " + episodes.ElementAt (0).ParentSeason.Name, "Greska", MessageBoxButton.OK, MessageBoxImage.Error);
                if (serieShowCat == SerieCategory.season) {
                    BusyFalse ();
                }
                return;
            }
            int countEpisodes = 0;
            foreach (FileInfo file in videoFiles) {                        
                ScanFile (episodes.ElementAt (countEpisodes), file.FullName);
                countEpisodes++;
            }
            if (serieShowCat == SerieCategory.season) {
                BusyFalse ();
            }
        }

        private void editAudioLng_Click (object sender, RoutedEventArgs e) {
            try {
                Audio selectedAudio = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.ElementAt (episodeListBox.SelectedIndex).AudioList.ElementAt (audioLanguagesListBox.SelectedIndex);
                InputDialog inputAudio = new InputDialog ("Izmijeni audio jezik", selectedAudio.Language.Name);
                inputAudio.Owner = this;
                inputAudio.ShowDialog ();
                if (inputAudio.accepted) {
                    selectedAudio.Language.Name = inputAudio.inputString;
                    audioLanguagesListBox.Focus ();
                }
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi audio");
            }
            finally {
                audioLanguagesListBox.DataContext = null;
                audioLanguagesListBox.DataContext = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.ElementAt (episodeListBox.SelectedIndex);
            }
        }        
        private void addSubtitleLng_Click (object sender, RoutedEventArgs e) {
            InputDialog inputSub = new InputDialog ("Novi prijevod");
            inputSub.Owner = this;
            inputSub.ShowDialog ();
            if (String.IsNullOrWhiteSpace (inputSub.inputString)) {
                return;
            }
            else {
                Language newSub = new Video_katalog.Language ();
                newSub.Name = inputSub.inputString;
                currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.ElementAt (episodeListBox.SelectedIndex).SubtitleLanguageList.Add (newSub);
                subtitleLangsListBox.Focus ();
            }
        }
        private void removeSubtitleLng_Click (object sender, RoutedEventArgs e){
            try {
                currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex).Episodes.ElementAt (episodeListBox.SelectedIndex).SubtitleLanguageList.RemoveAt (subtitleLangsListBox.SelectedIndex);
                subtitleLangsListBox.Focus ();
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi prijevod.");
            }
        } 

        private void hddComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            if (serieShowCat == SerieCategory.episode) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                SerieEpisode selectedEpisode = selectedSeason.Episodes.ElementAt (episodeListBox.SelectedIndex);
                if (hddComboBox.SelectedValue != null)
                    selectedEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddComboBox.SelectedValue.ToString ());
            }
            else if (serieShowCat == SerieCategory.season) {
                SerieSeason selectedSeason = currSerie.Seasons.ElementAt (seasonListBox.SelectedIndex);
                foreach (SerieEpisode tempEpisode in selectedSeason.Episodes)
                    if (hddComboBox.SelectedValue != null)
                        tempEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddComboBox.SelectedValue.ToString ());
            }
            else if (serieShowCat == SerieCategory.serie) {
                foreach (SerieSeason tempSeason in currSerie.Seasons)
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                        if (hddComboBox.SelectedValue != null)
                            tempEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, hddComboBox.SelectedValue.ToString ());
            }
        }

        #endregion

        private void acceptNewSerieButton_Click (object sender, RoutedEventArgs e) {
            if (RequiredFieldsFilled ()) {
                DatabaseManager.UpdateSettings (settings);
                this.accepted = true;
                SavePoster ();
                foreach (SerieSeason tempSeason in currSerie.Seasons) {
                    tempSeason.ParentSerie = currSerie;
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                        tempEpisode.ParentSeason = tempSeason;
                    }
                }
                this.Close ();
            }            
        }
        private void cancelNewSerieButton_Click (object sender, RoutedEventArgs e) {
            this.accepted = false;
            this.Close ();
        }

        private void releaseDatePicker_SelectedDateChanged (object sender, SelectionChangedEventArgs e) {
            if (releaseDatePicker.SelectedDate == null)
                return;
            if (serieShowCat == SerieCategory.serie) {
                foreach (SerieSeason tempSeason in currSerie.Seasons)
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                        tempEpisode.AirDate = (DateTime) releaseDatePicker.SelectedDate;
            }
            else if (serieShowCat == SerieCategory.season) {
                foreach (SerieEpisode tempEpisode in ((SerieSeason) seasonListBox.SelectedItem).Episodes)
                    tempEpisode.AirDate = (DateTime) releaseDatePicker.SelectedDate;
            }
            else if (serieShowCat == SerieCategory.episode) {
                ((SerieEpisode) episodeListBox.SelectedItem).AirDate = (DateTime) releaseDatePicker.SelectedDate;
                //((SerieEpisode) _episodeView.CurrentItem).AirDate = (DateTime) releaseDatePicker.SelectedDate;
            }
        }

        #region HELP FUNCTIONS

        string ReturnWebPageSource (string link) {
            WebClient client = new WebClient ();
            client.DownloadFile (link, "resultWebPage.txt");
            StreamReader webPageFile = new StreamReader ("resultWebPage.txt");
            string pageSource = webPageFile.ReadToEnd ();
            webPageFile.Close ();
            return pageSource;
        }
        string GetStringBetweenStrins (string sourceStr, string startStr, string endStr) {
            int startIndex = sourceStr.IndexOf (startStr) + startStr.Length;
            if (startIndex == (startStr.Length - 1)) {
                throw new Exception ("Ne postoji trazeni string u source-u");
            }
            string returnString = sourceStr.Substring (startIndex);
            int endIndex = returnString.IndexOf (endStr);
            returnString = returnString.Substring (0, endIndex);
            return returnString;
        }
        bool RequiredFieldsFilled () {
            string notFilled = "";
            if (string.IsNullOrWhiteSpace (currSerie.Name) || string.IsNullOrWhiteSpace (currSerie.OrigName))
                notFetched += "\r\nNaziv serije";
            if (currSerie.Seasons.Count == 0)
                notFilled += "\r\nSezone";
            foreach (SerieSeason tempSeason in currSerie.Seasons) {
                if (tempSeason.Episodes.Count == 0)
                    notFilled += "\r\nEpizode za: " + tempSeason.Name;
                foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                    if (tempEpisode.Size == 0 && this.wishOrRegular == WishOrRegular.regular) {
                        notFilled += "\r\nNije učitana video datoteka za: " + tempSeason.Name + " - " + tempEpisode.Name;
                    }
                    if (tempEpisode.Hdd == null && this.wishOrRegular == WishOrRegular.regular)
                        notFilled += "\r\nNije odabran HDD datoteka za: " + tempSeason.Name + " - " + tempEpisode.Name;
                }
            }
            if (notFilled.Length > 0) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Slijedece stvari moraju biti ispunjene:" + notFilled, "Nije sve ispunjeno", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return false;
            }
            else
                return true;
        }
        bool IsVideoFile (FileInfo file) {
            foreach (VideoFileExtensions ext in Enum.GetValues (typeof(VideoFileExtensions))) {
                //makni tocku sa pocetka extenzije video filea, te prva 3 znaka svake extenzije  ("EXT")
                if (file.Extension.Remove (0,1).ToLower () == ext.ToString ().Remove (0,3))
                    return true;
            }
            return false;
        }
        void SavePoster () {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
            encoder.Frames.Add (BitmapFrame.Create ((BitmapSource) posterImage.Source));
            encoder.QualityLevel = (int) imageQualityLevelSlider.Value;
            try {
                File.Delete (HardcodedStrings.seriePosterFolder + this.currSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg");
            }
            catch {
            }
            try {
                try {
                    Directory.CreateDirectory (HardcodedStrings.seriePosterFolder);
                }
                catch {
                }
                FileStream newStream = new FileStream (HardcodedStrings.seriePosterFolder +
                    this.currSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg", FileMode.Create);
                encoder.Save (newStream);
                newStream.Close ();
            }
            catch {
            }
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
        string AddLeadingZeroToSeasonNumber (string name) {
            //format je ili "Season 1" ili "Season 12"
            string numbers = "";
            string chars = "";
            foreach (char c in name)
                if (char.IsDigit (c))
                    numbers += c;
                else
                    chars += c;
            if (numbers.Length == 1)
                return string.Format ("{0}0{1}", chars, numbers);
            else
                return chars + numbers;
        }
        void sortEpisodes (SerieSeason season) {
            for (int i = 0 ; i < season.Episodes.Count ; i++) {
                for (int j = i; j < season.Episodes.Count; j++) {
                    if (string.CompareOrdinal (season.Episodes.ElementAt (i).Name, season.Episodes.ElementAt (j).Name) > 0) {
                        SerieEpisode temp = season.Episodes.ElementAt (i);
                        season.Episodes[i] = season.Episodes[j];
                        season.Episodes[j] = temp;
                    }
                }
            }
        }
        void sortSeasons () {
            for (int i = 0 ; i < this.currSerie.Seasons.Count ; i++) {
                for (int j = i ; j < this.currSerie.Seasons.Count ; j++) {
                    if (string.CompareOrdinal (this.currSerie.Seasons.ElementAt (i).Name, this.currSerie.Seasons.ElementAt (j).Name) > 0) {
                        SerieSeason temp = this.currSerie.Seasons.ElementAt (i);
                        this.currSerie.Seasons[i] = this.currSerie.Seasons[j];
                        this.currSerie.Seasons[j] = temp;
                    }
                }
            }
        }

        #region Setting busyIndicator

        private void BusyFalse () {
            this.Dispatcher.Invoke (new Action (() => {
                busyIndicator.IsBusy = false;
            }));
        }
        private void BusyTrue () {
            this.Dispatcher.Invoke (new Action (() => {
                busyIndicator.IsBusy = true;
            }));
        }
        private void BusyTrue (string message) {
            this.Dispatcher.Invoke (new Action (() => {
                busyIndicator.IsBusy = false;
                busyIndicator.BusyContent = message;
            }));
        }
        private void BusyMessage (string message) {
            this.Dispatcher.Invoke (new Action (() => {
                busyIndicator.IsBusy = true;
                busyIndicator.BusyContent = message;
            }));
        }

        #endregion

        #endregion
    }
}