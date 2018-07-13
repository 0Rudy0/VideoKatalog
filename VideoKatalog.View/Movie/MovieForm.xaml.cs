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
using System.Data.SqlServerCe;
using MediaInfoLib;

namespace Video_katalog {
    
    public partial class MovieForm: Window {

        #region GLOBAL DATA
        
        SqlCeConnection connection = new SqlCeConnection (ComposeConnectionString.ComposeCEString ());
        System.Resources.ResourceManager resources = new System.Resources.ResourceManager ("items", System.Reflection.Assembly.GetExecutingAssembly ());
        ////FinderInCollection finder = new FinderInCollection ();
        Cloner cloner = new Cloner ();
        public List<HDD> insertedHDDs = new List<HDD> ();
        WishOrRegular movieType;
        public DateTime releaseDateForWishMovie;

        public Movie currMovie = new Movie ();
        Movie downloadedMovie = new Movie ();
        public bool accepted = false;
        List<Person> allMovieCast = new List<Person> ();
        ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();        
        bool loading;
        bool isNewMovie;
        public bool movieExists = false;
        Settings settings = new Settings ();       
        bool videoFileLoaded = false;
        Thread threadDownloadData;
        DispatcherTimer timerCheckDownloadingThread = new DispatcherTimer ();     
        int maxGenreStringLength = 33;
        public string notFetched = "";
        private bool posterEdited = false;
        string refreshParameter = null;

        Thread threadScanVideo;
        DispatcherTimer timerCheckIfScanFinished;

        DispatcherTimer acceptTimer;
        System.Diagnostics.Stopwatch stopwatch;
        bool automatic = false;
        string customName;
        DispatcherTimer refreshTimer = new DispatcherTimer ();

        #endregion

        #region search strings

        string movieLink;
        string youtubeTrailerSearchPrefix = "http://www.youtube.com/results?search_query=";
        string youtubeTrailerSearchSuffix = "+trailer&aq=f";
        string googleTrailerSearchPrefix = "http://www.google.com/webhp?hl=hr#sclient=psy&hl=hr&site=webhp&source=hp&q=";
        string googleTrailerSearchSuffix = "+trailer&aq=f&aqi=&aql=&oq=&pbx=1&fp=ebd6b5bbc36e5c9c";

        string impAwardsPosterSearchPrefix = "http://www.impawards.com/search.php?movie=";
        string impAwardsPosterSearchSuffix = "&sa=Search&siteurl=www.impawards.com%252F#860";
        string googlePosterSearchPrefix = "http://www.google.hr/search?q=";
        string googlePosterSearchSuffix = "+poster&hl=hr&client=firefox-a&hs=fVv&rls=org.mozilla:en-US:official&prmd=ivns&tbm=isch&tbo=u&source=univ&sa=X&ei=HqiyTfHHCMiZOrORwYAJ&ved=0CBkQsAQ&biw=1440&bih=925";
        string moviePosterDBSearchPrefix = "http://www.movieposterdb.com/movie/";

        string moviePosterSourceBeginsWith1 = "<td class=\"poster\" id=\"";
        string moviePosterSourceEndsWith1 = "<td class=\"poster\" id=\"";
        string moviePosterSourceBeginsWith2 = "<img src=\"";
        string moviePosterSourceEndsWith2 = "\"";

        #endregion

        #region ON LOAD

        public MovieForm (string link, WishOrRegular type) {
            InitializeComponent ();
            loading = false;
            currMovie.InternetLink = link;
            isNewMovie = true;
            currMovie.AddTime = DateTime.Now;
            this.movieType = type;
            if (movieType == WishOrRegular.wish) {
                mainWindow.Width = 904;
                viewedCheckBox.Visibility = System.Windows.Visibility.Hidden;
                movieTechInfoGrid.IsEnabled = false;
            }
            else {
                releaseDateGrid.Visibility = System.Windows.Visibility.Hidden;
            }
        }
        public MovieForm (string link, WishOrRegular type, string videoPath, HDD selectedHDD, string customName) {
            InitializeComponent ();
            loading = false;
            currMovie.InternetLink = link;
            isNewMovie = true;
            currMovie.AddTime = DateTime.Now;
            currMovie.Hdd = selectedHDD;
            this.mainWindow.Title = customName;
            this.customNameTextBox.Text = customName;
            this.movieType = type;
            if (movieType == WishOrRegular.wish) {
                mainWindow.Width = 904;
                viewedCheckBox.Visibility = System.Windows.Visibility.Hidden;
                movieTechInfoGrid.IsEnabled = false;
            }
            else {
                releaseDateGrid.Visibility = System.Windows.Visibility.Hidden;
            }
            automatic = true;
            this.pauseTimer.Visibility = System.Windows.Visibility.Visible;
            refreshTimer.Interval = new TimeSpan (0, 0, 0, 0, 100);
            refreshTimer.Tick += (refreshTimerTB);
            stopwatch = new System.Diagnostics.Stopwatch ();
            this.customName = customName;
            this.timerValueTB.DataContext = stopwatch.Elapsed.Seconds.ToString ();

            stopwatch = new System.Diagnostics.Stopwatch ();
            acceptTimer = new DispatcherTimer ();
            acceptTimer.Interval = new TimeSpan (0, 0, 0, 1);
            acceptTimer.Tick += (PressAcceptButton);
            ThreadStart starter = delegate { ScanVideo (videoPath); };
            new Thread (starter).Start ();
        }
        public MovieForm (Movie movieToEdit) {
            InitializeComponent ();  
            isNewMovie = false;                      
            this.currMovie = cloner.CloneMovie (movieToEdit);             
            loading = true;                        
            loading = false;
            videoFileLoaded = true;
            this.movieType = WishOrRegular.regular;
            FillGenresCheckBoxes (movieToEdit.Genres);
            foreach (Person actor in movieToEdit.Actors)
                allMovieCast.Add (actor);
            releaseDateGrid.Visibility = System.Windows.Visibility.Hidden;            
        }
        public MovieForm (Movie movieToEdit, string refreshParameter) {
            InitializeComponent ();
            automatic = true;
            loading = false;
            currMovie = cloner.CloneMovie (movieToEdit);
            videoFileLoaded = true;
            this.movieType = WishOrRegular.regular; 
            foreach (Person actor in movieToEdit.Actors)
                allMovieCast.Add (actor);

            stopwatch = new System.Diagnostics.Stopwatch ();
            acceptTimer = new DispatcherTimer ();
            acceptTimer.Interval = new TimeSpan (0, 0, 0, 0, 10);
            acceptTimer.Tick += (PressAcceptButton);
            this.refreshParameter = refreshParameter;
            //if (refreshParameter == "cast")
            //    refreshCastAndCrew_Click (null, null);            
        }
        public MovieForm (WishMovie movieToEdit) {
            InitializeComponent ();
            isNewMovie = false;
            this.movieType = WishOrRegular.wish;
            this.currMovie = MovieFromWishMovie (movieToEdit);
            loading = false;
            FillGenresCheckBoxes (currMovie.Genres);
            mainWindow.Width = 904;
            foreach (Person actor in currMovie.Actors)
                allMovieCast.Add (actor);
            movieTechInfoGrid.Visibility = System.Windows.Visibility.Hidden;
            movieTechInfoGrid.IsEnabled = false;
            releaseDatePicker.SelectedDate = movieToEdit.ReleaseDate;
            //imdbInfoGrid.DataContext = this.currMovie;
        }

        private void Window_Loaded (object sender, RoutedEventArgs e) {
            timerCheckDownloadingThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            busyIndicator.BusyContent = "Skidam potrebne web stranice...\r\n";
            castLimit.Value = 10;
            settings = DatabaseManager.FetchSettings ();
            LoadSettings ();
            hddList = DatabaseManager.FetchHDDList ();

            this.hddComboBox.ItemsSource = hddList;            

            if (isNewMovie == false) {
                try {
                    this.posterImage.Source = Clipboard.GetImage ();
                }
                catch {
                }
                if (currMovie.Hdd != null)
                    currMovie.Hdd = FinderInCollection.FindInHDDCollection (hddList, currMovie.Hdd.ID);
                if (refreshParameter == "cast")
                    refreshCastAndCrew_Click (null, null);
                if (refreshParameter == "justUpdate" && this.automatic)
                    acceptTimer.Start ();
            }
            else {
                threadDownloadData = new Thread (new ThreadStart (DownloadInfoAndPoster));
                timerCheckDownloadingThread.Tick += new EventHandler (SetAllInfoAfterDownload);
                busyIndicator.IsBusy = true;
                timerCheckDownloadingThread.Start ();
                threadDownloadData.Start ();
            }
            if (movieExists) {
                if (Xceed.Wpf.Toolkit.MessageBox.Show ("Film već postoji u listi. Nastaviti dodavanje?", "Postoji film!", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                    this.Close ();
                }
            }
            //this.imdbInfoGrid.DataContext = currMovie;
            //this.movieTechInfoGrid.DataContext = currMovie;
            this.DataContext = currMovie;
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
            if (isNewMovie) {
                this.currMovie.Name = this.currMovie.OrigName;
            }
            this.settings.UseOnlyOriginalName = true;
            DatabaseManager.UpdateSettings (settings);
        }
        private void useOriginalNameCB_Unchecked (object sender, RoutedEventArgs e) {
            this.settings.UseOnlyOriginalName = false;
            DatabaseManager.UpdateSettings (settings);
        }
        
        //******   CAST AND CREW    ******//
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
                currMovie.Directors.Add (newDir);
                directorListBox.Focus ();
            }
        }
        private void removeDirector_Click (object sender, RoutedEventArgs e) {
            try {
                if (currMovie.Directors.Count == 1)
                    ;//return;
                currMovie.Directors.RemoveAt (directorListBox.SelectedIndex);
                directorListBox.Focus ();
            }
            catch {
            }
        }
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
                if (currMovie.Actors.Contains (newActor, new PersonEqualityComparerByName ())) {
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Osoba vec postoji");
                    return;
                }
                int maxActors = (int)castLimit.Value;
                if (allMovieCast.Count < maxActors) {
                    allMovieCast.Add (newActor);
                    return;
                }
                else if (allMovieCast.Count == maxActors) {
                    allMovieCast.Add (newActor);
                }
                else {
                    allMovieCast.Insert (maxActors, newActor);
                }
                castLimit.Value++;
                castListBox.Focus ();          
            }
            RefreshCastList ();
        }
        private void removeActor_Click (object sender, RoutedEventArgs e) {
            int index = castListBox.SelectedIndex;
            try {
                if (currMovie.Actors.Count == 1)
                    ;// return;
                currMovie.Actors.RemoveAt (castListBox.SelectedIndex);
                if (index > currMovie.Actors.Count - 1)
                    index--;
                castListBox.SelectedIndex = index;
                castListBox.Focus ();
            }
            catch {
            }
        }
        private void castLimit_ValueChanged (object sender, RoutedPropertyChangedEventArgs<object> e) {
            if (castLimit.Value < 1)
                castLimit.Value = 1;
            RefreshCastList ();

        }
        private void RefreshCastList () {            
            currMovie.Actors.Clear ();
            int maxActors = (int) castLimit.Value;
            for (int i = 0 ; i < maxActors && i < allMovieCast.Count ; i++) {
                currMovie.Actors.Add (allMovieCast[i]);
            }
            //this.DataContext = currMovie;
            //this.castListBox.DataContext = currMovie.ActorsA;
        }
        
        //**** TRAILER LINK ****//
        private void searchTrailer_Click (object sender, RoutedEventArgs e) {
            if (trailerYoutubeRadio.IsChecked == true) {
                System.Diagnostics.Process.Start("http://www.youtube.com/results?search_query=" + currMovie.OrigName.Replace(' ', '+') + "+trailer");
                this.settings.TrailerProvider = "youtube";
            }
            else if (trailerGoogleRadio.IsChecked == true) {
                System.Diagnostics.Process.Start (this.googleTrailerSearchPrefix + currMovie.OrigName + this.googleTrailerSearchSuffix);
                this.settings.TrailerProvider = "google";
            }
            DatabaseManager.UpdateSettings (settings);
        }
        private void pasteTrailerLink_Click_1 (object sender, RoutedEventArgs e) {
            currMovie.TrailerLink = Clipboard.GetText ();            
        }
        private void openTrailerLink_Click (object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start (trailerLinkTextBox.Text);
            }
            catch {
            }
        }
        
        //**** INTERNET LINK ****//
        private void pasteInternetLink_Click_1 (object sender, RoutedEventArgs e) {
            currMovie.InternetLink = Clipboard.GetText ();
        }
        private void openInternetLink_Click (object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start (internetLinkTextBox.Text);
            }
            catch {
            }
        }
        
        //**** POSTER MANAGEMENT ****//        
        private void searchPoster_Click (object sender, RoutedEventArgs e) {
            string ImdbID = "";
            foreach (char c in downloadedMovie.InternetLink)
            {
                if (Char.IsDigit(c))
                {
                    ImdbID += c.ToString();
                }
            }
            ImdbID = ImdbID.Replace("tt", "");
            ImdbID = "i" + ImdbID;
            bool replaced = true;
            string movieName = downloadedMovie.OrigName;
            while (replaced)
            {
                replaced = false;
                if (movieName.Contains(' '))
                {
                    replaced = true;
                    movieName = movieName.Replace(' ', '-');
                }
            }

            if (posterImpAwardsRadio.IsChecked == true) {
                System.Diagnostics.Process.Start (this.impAwardsPosterSearchPrefix + currMovie.OrigName);
                this.settings.PosterProvider = "imp";
            }
            else if (posterGoogleRadio.IsChecked == true) {
                System.Diagnostics.Process.Start (this.googlePosterSearchPrefix + currMovie.OrigName + this.googlePosterSearchSuffix);
                this.settings.PosterProvider = "google";
            }
            else if (posterMoviePosterDBRadio.IsChecked == true) {
                this.settings.PosterProvider = "posterDB";
                //System.Diagnostics.Process.Start ((new System.Diagnostics.ProcessStartInfo("explorer.exe", 
                  //  this.moviePosterDBSearchPrefix + ImdbID)));
            }
            Clipboard.SetText("https://www.cinematerial.com/movies/" + movieName + "-" + ImdbID);
            DatabaseManager.UpdateSettings (settings);
        }
        private void pasteImageButton_Click (object sender, RoutedEventArgs e) {
            posterImage.Source = Clipboard.GetImage ();
            posterEdited = true;
        }
        private void imageQualityLevelSlider_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            this.settings.ImageQualityLevel = (int) imageQualityLevelSlider.Value;
        }
        private void copyAndOpenEditorBTN_Click (object sender, RoutedEventArgs e) {
            if (this.settings.ImageEditorPath == "x" || this.settings.ImageEditorPath == null) {
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
            if (senderCheckBox.IsChecked == true && currMovie.Genres.Contains (tempGenre, new GenreEqualityComparer ()) == false) {
                currMovie.Genres.Add (tempGenre);
            }
            else if (senderCheckBox.IsChecked == false && currMovie.Genres.Contains (tempGenre, new GenreEqualityComparer ()) == true) {
                for (int i = 0 ; i < currMovie.Genres.Count ; i++) {
                    if (tempGenre.Name == currMovie.Genres[i].Name) {
                        currMovie.Genres.RemoveAt (i);
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
            string genresInString = converter.Convert (currMovie.Genres, typeof (string), maxGenreStringLength, null).ToString ();
            if (genresInString.Contains ("...")) {
                genresComboBox.Text = currMovie.Genres.Count + "/22 odabrano";                
            }
            else {
                genresComboBox.Text = genresInString;                
            }
        }        
        private void genresComboBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            genresComboBox.SelectedIndex = -1;
            int maxStringLength = 33;
            Converters.GenresToStringConverter converter = new Converters.GenresToStringConverter ();
            string genresInString = converter.Convert (currMovie.Genres, typeof (string), maxStringLength, null).ToString ();
            if (genresInString.Contains ("...")) {
                genresComboBox.Text = currMovie.Genres.Count + "/22 odabrano";
            }
            else {
                genresComboBox.Text = genresInString;
            }
        }

        #endregion

        #region TECH INFO

        public void ScanVideo (string videoPath) {
            Movie tempMovie = cloner.CloneMovie (currMovie);
            try {
                tempMovie.GetTechInfo(videoPath);
                currMovie.AudioList = cloner.CloneAudioList(tempMovie.AudioList);
                currMovie.SubtitleLanguageList = cloner.CloneLangList(tempMovie.SubtitleLanguageList);
                this.Dispatcher.Invoke(new Action(() => {
                    currMovie.Size = tempMovie.Size;
                    currMovie.Runtime = tempMovie.Runtime;
                    currMovie.Height = tempMovie.Height;
                    currMovie.Width = tempMovie.Width;
                    currMovie.Bitrate = tempMovie.Bitrate;
                    currMovie.AspectRatio = tempMovie.AspectRatio;
                    currMovie.AddTime = tempMovie.AddTime;
                    busyIndicatorScanningVideo.IsBusy = false;
                    movieTechInfoGrid.DataContext = null;
                    movieTechInfoGrid.DataContext = currMovie;
                }));
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    MessageBox.Show(ex.Message);
                }));
            }
             
        }
        private void loadFileButton_Click (object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog browseFile = new Microsoft.Win32.OpenFileDialog ();
            browseFile.Filter = "Podržani video formati |*.avi;*.mkv;*.vob;*.vob;*.mpg;*.mpeg;*.mp4;*.mov;*.3gp;*.wmv;*.mpgv;*.mpv;*.m1v;*.m2v;*.asf;*.rmvb|Svi formati |*.*";
            Nullable<bool> result = browseFile.ShowDialog ();
            if (result == true) {
                if (IsVideoFile (new FileInfo (browseFile.FileName)) == false) {
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Nepodržani format video datoteke.", "Nepoznat format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                currMovie.AudioList.Clear ();
                currMovie.SubtitleLanguageList.Clear ();
                videoFileLoaded = true;
                string videoPath = browseFile.FileName;
                busyIndicatorScanningVideo.IsBusy = true;
                busyIndicatorScanningVideo.BusyContent = "Očitavam";
                ThreadStart starter = delegate { ScanVideo(videoPath); };
                new Thread (starter).Start ();
            }
        }
        private void addNewHdd_Click (object sender, RoutedEventArgs e) {
            HDDForm newHDDForm = new HDDForm ();
            newHDDForm.Owner = this;
            newHDDForm.ShowDialog ();
            if (newHDDForm.accepted) {
                try {
                    DatabaseManager.InsertHDD (newHDDForm.currHDD);
                    DatabaseManagerMySql.InsertHDD (newHDDForm.currHDD);
                }
                catch {
                    MessageBox.Show ("Insert new hdd failed");
                }
                hddList.Add (newHDDForm.currHDD);
                insertedHDDs.Add (newHDDForm.currHDD);
            }
        }

        //****  AUDIO  ****
        private void editAudioLng_Click (object sender, RoutedEventArgs e) {
            try {
                Audio selectedAudio = currMovie.AudioList.ElementAt (audioLanguagesListBox.SelectedIndex);
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
                audioLanguagesListBox.DataContext = currMovie;
            }
        }       

        //****  SUBTITLE  ****
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
                currMovie.SubtitleLanguageList.Add (newSub);
                subtitleLangsListBox.Focus ();
            }
        }
        private void removeSubtitleLng_Click (object sender, RoutedEventArgs e) {
            try {
                currMovie.SubtitleLanguageList.RemoveAt (subtitleLangsListBox.SelectedIndex);
                subtitleLangsListBox.Focus ();
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi prijevod.");
            }
        }

        #endregion

        #region REFRESH INFO

        //REFRESH CLICKS
        private void StartDownloadSetBusyState (EventHandler timerTickEvent, ThreadStart threadStartFunction) {
            refreshSplitButton.IsOpen = false;
            busyIndicator.IsBusy = true;
            busyIndicator.BusyContent = "Skidanje potrebnih web stranica...\r\n";
            threadDownloadData = new Thread (threadStartFunction);
            threadDownloadData.IsBackground = true;
            threadDownloadData.Start ();
            timerCheckDownloadingThread = new DispatcherTimer ();
            timerCheckDownloadingThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            timerCheckDownloadingThread.Tick += timerTickEvent;
            timerCheckDownloadingThread.Start ();
            notFetched = "";
        }
        private void refreshButton_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetAllInfoAfterDownload), new ThreadStart (DownloadInfoAndPoster));            
        }
        private void refreshAllButPoster_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetAllInfoAfterDownload), new ThreadStart (DownloadAllInfo));  
        }        
        private void refreshOnlyPoster_Click (object sender, RoutedEventArgs e) {
            StartDownloadSetBusyState (new EventHandler (SetBusyIndicatorAfterDownload), new ThreadStart (DownloadAndSetPoster));           
            busyIndicator.BusyContent = "Skidam poster...";           
        }
        private void refreshCastAndCrew_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetCastAndCrewAfterDownload), new ThreadStart (DownloadCastAndCrew));  
        }
        private void refreshRating_Click (object sender, RoutedEventArgs e) {           
            StartDownloadSetBusyState (new EventHandler (SetRatingAfterDownload), new ThreadStart (DownloadRating));  
        }
        private void refreshGenres_Click (object sender, RoutedEventArgs e) {           
            StartDownloadSetBusyState (new EventHandler (SetGenresAfterDownload), new ThreadStart (DownloadGenres));  
        }
        private void refreshBudget_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetBudgetInfoAfterDownload), new ThreadStart (DownloadBudgetInfo));  
        }
        private void refreshEarnings_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetEarningsInfoAfterDownload), new ThreadStart (DownloadEarningsInfo));  
        }
        private void refreshSummary_Click (object sender, RoutedEventArgs e) {            
            StartDownloadSetBusyState (new EventHandler (SetSummaryAfterDownload), new ThreadStart (DownloadSummary));
        }
        public void DownloadInfoAndPoster () {
            DownloadAllInfo ();
        }

        //DOWNLOADES     
        bool downloadingAll = false;   
        private void DownloadAllInfo () {
            IMDb moviePage = new IMDb ();
            downloadedMovie = moviePage.GetMovieInfo (this.currMovie.InternetLink);
            if (downloadedMovie == null) {
                return;
            }
            DownloadAndSetPoster();
            notFetched = moviePage.notFetched;
            downloadingAll = true;
        }        
        private void DownloadBudgetInfo () {
            string budgetSource = ReturnWebPageSource (currMovie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixBudget);
            IMDb webPage = new IMDb ();
            downloadedMovie.Budget = webPage.GetBudget (budgetSource);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadEarningsInfo () {
            string movieName = currMovie.OrigName;
            movieName = movieName.Replace (":", "%3A");
            movieName = movieName.Replace ("&", "%26");
            
            IMDb webPage = new IMDb (this.currMovie.InternetLink);
            webPage.SetMainSource ();
            webPage.SetEarningsSource (this.currMovie);

            downloadedMovie.Earnings = webPage.GetEarnings ();
            this.notFetched += webPage.notFetched;
        }
        private void DownloadCastAndCrew () {
            string mainSource = ReturnWebPageSource (currMovie.InternetLink);
            string castAndCrewSource = ReturnWebPageSource (currMovie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixFullCredits);
            IMDb webPage = new IMDb ();            
            downloadedMovie.Directors = webPage.GetDirectors (castAndCrewSource);
            downloadedMovie.Actors = webPage.GetCast (mainSource, castAndCrewSource);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadRating () {
            string source = ReturnWebPageSource (currMovie.InternetLink);
            IMDb webPage = new IMDb ();
            downloadedMovie.Rating = webPage.GetRating (source);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadGenres () {
            string source = ReturnWebPageSource (currMovie.InternetLink);
            IMDb webPage = new IMDb ();
            downloadedMovie.Genres = webPage.GetGenres (source);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadSummary () {
            string mainSource = ReturnWebPageSource (currMovie.InternetLink);
            string summarySource = ReturnWebPageSource (currMovie.InternetLink + VideoKatalog.View.Properties.Resources.imdbPostfixSummary);
            IMDb webPage = new IMDb ();
            downloadedMovie.Summary = webPage.GetSummary (mainSource, summarySource);
            this.notFetched += webPage.notFetched;
        }
        private void DownloadAndSetPoster () {            
            try {                
                posterImage.Dispatcher.Invoke (DispatcherPriority.Normal, new Action (
                    delegate () {
                        posterImage.Source = new BitmapImage (new Uri (HelpFunctions.GetPosterImageUrl(currMovie.InternetLink, downloadedMovie.OrigName, "movies")));
                        posterEdited = true;
                    }
                ));
            }
            catch {
                this.notFetched += "\r\nPoster";
            }
        }        

        //SETTING MOVIE INFO
        private void SetAllInfo () {
            if (this.notFetched.Length != 0 && automatic == false) {
                MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
                //Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            if (downloadedMovie == null) {
                currMovie = null;
                return;
            }
            SetCastAndCrew ();
            SetGenres ();
            SetBudget ();
            SetEarnings ();
            SetRating ();
            SetSummary ();
            if (automatic) {
                currMovie.Name = this.customName;
            }
            else
                currMovie.Name = downloadedMovie.Name.Replace (":", " -");
            currMovie.OrigName = downloadedMovie.OrigName;
            currMovie.Year = downloadedMovie.Year;
            if (this.settings.UseOnlyOriginalName && automatic == false) {
                currMovie.Name = currMovie.OrigName.Replace (":", " -");
            }
            currMovie.TrailerLink = downloadedMovie.TrailerLink;
            downloadingAll = false;
            try {
                if (automatic) {
                    acceptTimer.Start ();
                    stopwatch.Start ();
                    refreshTimer.Start ();
                }
            }
            catch {
            }
        }
        private void SetCastAndCrew () {
            if (downloadedMovie.Actors.Count == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currMovie.Actors.Clear ();
                foreach (Person actor in downloadedMovie.Actors) {
                    currMovie.Actors.Add (actor);
                }
                allMovieCast.Clear ();
                foreach (Person tempActor in currMovie.Actors) {
                    allMovieCast.Add (tempActor);
                }
                RefreshCastList ();
            }

            if (downloadedMovie.Directors.Count == 0) {
            }
            else {
                currMovie.Directors.Clear ();

                foreach (Person director in downloadedMovie.Directors) {
                    currMovie.Directors.Add (director);
                }
            }
            try {
                if (automatic && refreshParameter == "cast") {
                    acceptTimer.Start ();
                    stopwatch.Start ();
                    refreshTimer.Start ();
                }
            }
            catch {
            }
        }
        private void SetBudget () {
            if (downloadedMovie.Budget == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currMovie.Budget = downloadedMovie.Budget;
            }
        }
        private void SetEarnings () {
            if (downloadedMovie.Earnings == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno:" + this.notFetched);
            }
            else {
                currMovie.Earnings = downloadedMovie.Earnings;
            }
        }
        private void SetRating () {
            if (downloadedMovie.Rating == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno" + this.notFetched);
            }
            else {
                currMovie.Rating = downloadedMovie.Rating;
            }
        }
        private void SetGenres () {
            if (downloadedMovie.Genres.Count == 0 && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno" + this.notFetched);
            }
            else {
                currMovie.Genres.Clear ();
                foreach (Genre genre in downloadedMovie.Genres) {
                    currMovie.Genres.Add (genre);
                }
                FillGenresCheckBoxes (currMovie.Genres);
            }
        }
        private void SetSummary () {
            if (string.IsNullOrWhiteSpace (downloadedMovie.Summary) && downloadingAll == false) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije dohvaćeno" + this.notFetched);
            }
            else {
                currMovie.Summary = downloadedMovie.Summary;
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
        private void SetBudgetInfoAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetBudget ();
            }
        }
        private void SetEarningsInfoAfterDownload (object sender, EventArgs e) {
            if (IsThreadFinishedSetBusyState ()) {
                SetEarnings ();
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
        private void SetBusyIndicatorAfterDownload (object sender, EventArgs e) {
            IsThreadFinishedSetBusyState ();
        }

        #endregion

        private void PressAcceptButton (object sender, EventArgs e) {
            acceptNewMovieButton_Click (null, null);
            acceptTimer.Stop ();
        }
        private void acceptNewMovieButton_Click (object sender, RoutedEventArgs e) {
            if (RequiredFieldsFilled ()) {
                DatabaseManager.UpdateSettings (settings);
                accepted = true;
                if (posterEdited)
                    SavePoster ();
                if (this.movieType == WishOrRegular.wish)
                    releaseDateForWishMovie = (DateTime) releaseDatePicker.SelectedDate;
                this.currMovie.Name = HelpFunctions.ReplaceIllegalChars (currMovie.Name);
                this.Close ();
            }
        }
        private void cancelNewMovieButton_Click (object sender, RoutedEventArgs e) {
            accepted = false;
            this.Close ();
        }

        #region HELP FUNCTIONS

        private bool RequiredFieldsFilled () {
            string errorInitialMessage = "Sljedeća polja moraju biti ispunjena:";
            string errorString = errorInitialMessage;
            if (String.IsNullOrWhiteSpace (customNameTextBox.Text))
                errorString += "\r\nNaziv";
            //if (String.IsNullOrWhiteSpace (yearTextBox.Text))
            //    errorString += "\r\nGodina";
            if (String.IsNullOrWhiteSpace (originalNameTextBox.Text))
                errorString += "\r\nOrginalni naziv";
            //if (this.currMovie.Genres.Count < 1)
            //    errorString += "\r\nŽanrovi";
            //if (String.IsNullOrWhiteSpace (ratingTextBox.Text))
            //    errorString += "\r\nOcjena";
            //if (this.currMovie.Actors.Count < 0)
            //    errorString += "\r\nGlumci";
            //if (this.currMovie.Directors.Count < 0)
            //    errorString += "\r\nRedatelj(i)";
            //if (String.IsNullOrWhiteSpace (summaryTextBox.Text))
            //    errorString += "\r\nRadnja";
            //if (String.IsNullOrWhiteSpace (internetLinkTextBox.Text))
            //    errorString += "\r\nLink na internet stranicu";
            //if (String.IsNullOrWhiteSpace (trailerLinkTextBox.Text))
            //    errorString += "\r\nLink na stranicu trailera";           
            //if (String.IsNullOrWhiteSpace (budgetTextBox.Text))
            //    errorString += "\r\nBudget";
            //if (String.IsNullOrWhiteSpace (earningsTextBox.Text))
            //    errorString += "\r\nZarada";
            //if (posterImage.Source == null)
            //    errorString += "\r\nNema slike";     
            if (releaseDatePicker.SelectedDate == null && this.movieType == WishOrRegular.wish)
                errorString += "\r\nNije odabran datum izlaska";
            if (currMovie.Size == 0 && this.movieType == WishOrRegular.regular)
                errorString += "\r\n\r\nNIJE UČITANA VIDEO DATOTEKA";
            if (this.currMovie.Hdd == null && this.movieType == WishOrRegular.regular)
                errorString += "\r\n#HDD";
            if (errorString.Length == errorInitialMessage.Length)                 
                return true;
            Xceed.Wpf.Toolkit.MessageBox.Show (errorString);
            return false;
        }
        string ReturnWebPageSource (string link) {
            WebClient client = new WebClient ();
            try {
                client.DownloadFile (link, "resultWebPage.txt");
                StreamReader webPageFile = new StreamReader ("resultWebPage.txt");
                string pageSource = webPageFile.ReadToEnd ();
                webPageFile.Close ();
                return pageSource;
            }
            catch {
                return "";
            }
        }
        
        public void SavePoster () {
            JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
            try {                
                encoder.Frames.Add (BitmapFrame.Create ((BitmapSource) posterImage.Source));
                encoder.QualityLevel = (int) imageQualityLevelSlider.Value;
            }
            catch {
                this.notFetched += "\r\nPoster";
                return;
            }

            try {
                File.Delete (HardcodedStrings.moviePosterFolder + 
                    this.currMovie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg");
            }
            catch {
            }
            try {
                try {
                    Directory.CreateDirectory (HardcodedStrings.moviePosterFolder);
                }
                catch {
                }
                FileStream newStream = new FileStream (HardcodedStrings.moviePosterFolder + 
                    this.currMovie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg", FileMode.Create);
                encoder.Save (newStream);
                newStream.Close ();
            }
            catch {
                this.notFetched += "\r\nPoster";
            }
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
        bool IsVideoFile (FileInfo file) {
            foreach (VideoFileExtensions ext in Enum.GetValues (typeof (VideoFileExtensions))) {
                //makni tocku sa pocetka extenzije video filea, te prva 3 znaka svake extenzije  ("EXT")
                if (file.Extension.Remove (0, 1).ToLower () == ext.ToString ().Remove (0, 3))
                    return true;
            }
            return false;
        }
        
        #endregion

        private void pauseTimer_Click (object sender, RoutedEventArgs e) {
            if (acceptTimer.IsEnabled) {
                acceptTimer.Stop ();
                stopwatch.Reset ();
                pauseTimer.Content = "Start";
            }
            else {
                acceptTimer.Start ();
                stopwatch.Start ();
                pauseTimer.Content = "Stop";
            }
        }
        private void refreshTimerTB (object sender, EventArgs e) {
            this.timerValueTB.Text = stopwatch.Elapsed.Seconds.ToString() + "." + stopwatch.Elapsed.Milliseconds.ToString().Substring (0,1) + " / 1.0";
        }
    }    
}
