using System;
using System.Data.Sql;
using System.Data.SqlClient;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using MediaInfoLib;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Resources;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Drawing;
using System.Data.SqlServerCe;
using VideoKatalog.View;

namespace Video_katalog {
    public partial class CatalogForm : Window {

        Boolean loadData = true;

        #region GLOBAL DATA

        //SqlCeConnection connection = new SqlCeConnection (ComposeConnectionString.ComposeCEString ());
        Cloner cloner = new Cloner ();
        List<Thread> allThreads = new List<Thread> ();

        private ObservableCollection<Person> personList = new ObservableCollection<Person> ();
        private ObservableCollection<Language> languageList = new ObservableCollection<Language> ();
        private ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();
        private ObservableCollection<Genre> genreList = new ObservableCollection<Genre> ();
        private ObservableCollection<Audio> audioList = new ObservableCollection<Audio> ();
        private ObservableCollection<Camera> cameraList = new ObservableCollection<Camera> ();
        private ObservableCollection<Category> homeVideoCatList = new ObservableCollection<Category> ();

        List<string> imagesToDelete = new List<string> ();        

        Thread ThreadLoadData;
        DispatcherTimer TimerCheckLoadingThread = new DispatcherTimer ();
        Thread threadInsertInDatabase;

        int progressBarCounter = 0;
        DispatcherTimer TimerRefreshLoadProgressBar = new DispatcherTimer ();

        #endregion

        #region ON START

        public CatalogForm () {
            InitializeComponent ();
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            busyIncidator.IsBusy = true;
            busyIncidator.BusyContent = "Učitavanje podataka...\r\n";
            allThreads.Add (ThreadLoadData);
            ThreadLoadData = new Thread (new ThreadStart (FillInitialData));
            ThreadLoadData.IsBackground = true;
            TimerCheckLoadingThread.Tick += new EventHandler (CheckIsThreadAlive);
            TimerCheckLoadingThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            ThreadLoadData.Start ();
            TimerCheckLoadingThread.Start ();

            TimerRefreshLoadProgressBar.Interval = new TimeSpan (0, 0, 0, 0, 100);
            TimerRefreshLoadProgressBar.Tick += new EventHandler (RefreshStatusBarProgressBar);
            TimerRefreshLoadProgressBar.Start ();
        }
        private void FillInitialData () {
            //DeployMediaInfoDLL ();
            if (loadData == false)
                return;
            try {
                //connection.Open ();                
                personList = DatabaseManager.FetchPersonList (ref progressBarCounter);
                hddList = DatabaseManager.FetchHDDList ();
                genreList = DatabaseManager.FetchGenreList ();
                languageList = DatabaseManager.FetchLanguageList ();
                audioList = DatabaseManager.FetchAudioList (languageList);
                homeVideoCatList = DatabaseManager.FetchCategories ();
                cameraList = DatabaseManager.FetchCameraList ();

                this.Dispatcher.Invoke (new Action (() => {
                    statusBarTextBlock.Text = "Filmovi";
                    statusBarProgressBar.Maximum = DatabaseManager.FetchMovieCountList ();
                    statusBarProgressBar.Value = 0;
                }));
                progressBarCounter = 0;
                listOfMovies = DatabaseManager.FetchMovieList (genreList, audioList, personList, languageList, hddList, ref progressBarCounter);

                this.Dispatcher.Invoke (new Action (() => {
                    statusBarTextBlock.Text = "Serije";
                    statusBarProgressBar.Maximum = DatabaseManager.FetchSerieCount () + DatabaseManager.FetchSeasonCount () + DatabaseManager.FetchEpisodeCount () + 50;
                    statusBarProgressBar.Value = 0;
                }));
                progressBarCounter = 0;
                listOfSeries = DatabaseManager.FetchSerieList (genreList, personList, ref progressBarCounter);
                listOfSerSeasons = DatabaseManager.FetchSerieSeasonList (listOfSeries, ref progressBarCounter);
                listOfSerEpisodes = DatabaseManager.FetchSerieEpisodeList (listOfSerSeasons, languageList, audioList, hddList, ref progressBarCounter);


                listOfHomeVideos = DatabaseManager.FetchHomeVideosList (homeVideoCatList, cameraList, languageList, audioList, hddList, personList);

                //connection.Close ();
                //CheckDoubles ();

            }
            catch (Exception newExc) {
                try {
                    //connection.Close ();
                }
                catch {
                }
                this.Dispatcher.Invoke (new Action (() => {
                    Xceed.Wpf.Toolkit.MessageBox.Show ("ERROR: " + newExc.Message);
                }));
            }
            TimerRefreshLoadProgressBar.Stop ();
        }

        private void RefreshStatusBarProgressBar (object sender, EventArgs e) {
            statusBarProgressBar.Value = progressBarCounter;
        }
        
        private void CheckDoubles () {
            foreach (Movie tempMovie in listOfMovies) {
                int count = 0;
                foreach (Movie tempMovie2 in listOfMovies) 
                    if (tempMovie.Name == tempMovie2.Name)                        
                        count++;                    
                
                if (count > 1)
                    this.Dispatcher.Invoke (new Action (() => {
                        MessageBox.Show (tempMovie.Name);
                    }));
            }
        }

        private void CheckIsThreadAlive (object sender, EventArgs e)  {
            if (ThreadLoadData.IsAlive == false) {
                TimerCheckLoadingThread.Stop ();
                busyIncidator.IsBusy = false;
                SetViewsAndDataContent ();
                SetFilters ();
                statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
                statusBarTextBlock.Text = "";
            }
        }
        private void SetViewsAndDataContent () {

            #region Movie

            _movieView = (CollectionView)CollectionViewSource.GetDefaultView (listOfMovies);
            SortDescription sortByName = new SortDescription ("Name", ListSortDirection.Ascending);
            _movieView.SortDescriptions.Add (sortByName);
            _movieView.MoveCurrentToPosition (0);
            this.tabMovies.DataContext = _movieView;

            #endregion

            #region Serie

            //SetFilters ();
            _serieView = (CollectionView) CollectionViewSource.GetDefaultView (listOfSeries);
            _serieView.SortDescriptions.Add (sortByName);
            _serieView.MoveCurrentToPosition (0);
            this.tabSeries.DataContext = _serieView;
            SetSerieTabConstantBindings ();
            //FillSerieTreeView ();
            FillSerieTreeViewFirstTime ();

            #endregion

            #region Home Video

            _homeVideoView = (CollectionView) CollectionViewSource.GetDefaultView (listOfHomeVideos);
            _homeVideoView.SortDescriptions.Add (sortByName);
            this.tabHomeVideo.DataContext = _homeVideoView;

            #endregion
      
        }
        private void SetFilters () {

            #region Movie

            #region Inicijalizacija

            int maxMovieYear = 1000;
            int minMovieYear = 3000;
            double maxMovieSize = 0;
            double minMovieSize = 999999999999999999;
            double maxMovieRuntime = 0;
            double minMovieRuntime = 999999999999999999;
            double maxBudget = 0;
            double minBudget = 999999999;
            float maxEarnings = 0;
            float minEarnigns = 999999999;
            double maxBitrate = 0;
            double minBitrate = 999999999999999999;

            #endregion

            #region Prolalazak min / max

            foreach (Movie tempMovie in listOfMovies) {
                if (tempMovie.Year > maxMovieYear)
                    maxMovieYear = tempMovie.Year;
                if (tempMovie.Year < minMovieYear)
                    minMovieYear = tempMovie.Year;
                if (tempMovie.Size > maxMovieSize)
                    maxMovieSize = tempMovie.Size;
                if (tempMovie.Size < minMovieSize)
                    minMovieSize = tempMovie.Size;
                if (tempMovie.Runtime > maxMovieRuntime)
                    maxMovieRuntime = tempMovie.Runtime;
                if (tempMovie.Runtime < minMovieRuntime)
                    minMovieRuntime = tempMovie.Runtime;
                if (tempMovie.Budget > maxBudget)
                    maxBudget = tempMovie.Budget;
                if (tempMovie.Budget < minBudget)
                    minBudget = tempMovie.Budget;
                if (tempMovie.Earnings > maxEarnings)
                    maxEarnings = tempMovie.Earnings;
                if (tempMovie.Earnings < minEarnigns)
                    minEarnigns = tempMovie.Earnings;
                if (tempMovie.Bitrate > maxBitrate)
                    maxBitrate = tempMovie.Bitrate;
                if (tempMovie.Bitrate < minBitrate)
                    minBitrate = tempMovie.Bitrate;
            }

            #endregion

            #region Postavljanje min / max slidera

            yearSliderMovie.Minimum = (double) minMovieYear;
            yearSliderMovie.LowerValue = yearSliderMovie.Minimum;
            yearSliderMovie.Maximum = (double) maxMovieYear;
            yearSliderMovie.UpperValue = yearSliderMovie.Maximum;

            sizeSliderMovie.Minimum = (double) minMovieSize;
            sizeSliderMovie.LowerValue = sizeSliderMovie.Minimum;
            sizeSliderMovie.Maximum = (double) maxMovieSize;
            sizeSliderMovie.UpperValue = sizeSliderMovie.Maximum;

            runtimeSliderMovie.Minimum = (double) minMovieRuntime;
            runtimeSliderMovie.LowerValue = runtimeSliderMovie.Minimum;
            runtimeSliderMovie.Maximum = (double) maxMovieRuntime;
            runtimeSliderMovie.UpperValue = runtimeSliderMovie.Maximum;

            budgetSliderMovie.Minimum = (double) minBudget;
            budgetSliderMovie.LowerValue = budgetSliderMovie.Minimum;
            budgetSliderMovie.Maximum = (double) maxBudget;
            budgetSliderMovie.UpperValue = budgetSliderMovie.Maximum;

            earningsSliderMovie.Minimum = (double) minEarnigns;
            earningsSliderMovie.LowerValue = earningsSliderMovie.Minimum;
            earningsSliderMovie.Maximum = maxEarnings;
            earningsSliderMovie.UpperValue = earningsSliderMovie.Maximum;

            //bitrateSliderMovie.Minimum = (double) minBitrate;
            //bitrateSliderMovie.LowerValue = bitrateSliderMovie.Minimum;
            //bitrateSliderMovie.Maximum = (double) maxBitrate;
            //bitrateSliderMovie.UpperValue = bitrateSliderMovie.Maximum;

            #endregion

            movieSearchHddComboBox.ItemsSource = hddList;
            foreach (HDD tempHDD in hddList) {
                showingMoviesFromHDDs.Add (tempHDD.Name);
            }

            #endregion
            

            #region Serie

            #region Inicijalizacija

            int maxSerieYear = 0;
            int minSerieYear = 9999;
            long maxSerieSize = 0;
            long minSerieSize = 999999999999999999;

            #endregion

            #region Pronalazak min / max

            foreach (Serie tempSerie in listOfSeries) {
                if (tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year > maxSerieYear)
                    maxSerieYear = tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
                if (tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year < minSerieYear)
                    minSerieYear = tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
                long size = FetchSerieShowingSize (tempSerie);
                if (size > maxSerieSize)
                    maxSerieSize = size;
                if (size < minSerieSize)
                    minSerieSize = size;
            }

            #endregion

            #region Postavljanje min / max za slidere

            yearRangeSliderSerie.Minimum = (double) minSerieYear;
            yearRangeSliderSerie.Maximum = (double) maxSerieYear;
            yearRangeSliderSerie.UpperValue = yearRangeSliderSerie.Maximum;
            yearRangeSliderSerie.LowerValue = yearRangeSliderSerie.Minimum;

            sizeRangeSliderSerie.Minimum = (double) minSerieSize;
            sizeRangeSliderSerie.Maximum = (double) maxSerieSize;
            sizeRangeSliderSerie.UpperValue = sizeRangeSliderSerie.Maximum;
            sizeRangeSliderSerie.LowerValue = sizeRangeSliderSerie.Minimum;

            #endregion


            serieSearchHddComboBox.ItemsSource = hddList;
            foreach (HDD tempHDD in hddList) {
                showingSeriesFromHDDs.Add (tempHDD.Name);
            }

            #endregion

            #region HomeVideo

            #endregion

        }
        
        private void DeployMediaInfoDLL () {
            if (File.Exists (Directory.GetCurrentDirectory () + "\\MediaInfo.dll"))
                return;
            //provjeri da li je sustav 64 ili 32 bitan
            if (IntPtr.Size == 8) {
                //sustav je 64 bitan
                //byte[] dll = VideoKatalog.View.Properties.Resources.MediaInfo_x64;
                //FileStream streamDLL = new FileStream (Directory.GetCurrentDirectory () + "\\Mediainfo.dll", FileMode.Create, FileAccess.ReadWrite);//  VideoKatalog.View.Properties.Resources.MediaInfo_x64;
                //BinaryWriter bw = new BinaryWriter (streamDLL);
                //bw.Write (dll);
                //bw.Close ();
                //File.Copy (VideoKatalog.View.Properties.Resources.MediaInfo_x64, Directory.GetCurrentDirectory () + "Mediainfo.dll");
                //File.Move (Directory.GetCurrentDirectory () + "MediaInfo-x64.dll", Directory.GetCurrentDirectory () + "Mediainfo.dll");
                //File.Delete (Directory.GetCurrentDirectory () + "MediaInfo-x64.dll");
                //File.Delete (Directory.GetCurrentDirectory () + "MediaInfo-x32.dll");
            }
            else {
                //sustav je 32 bitan
                //byte[] dll = VideoKatalog.View.Properties.Resources.MediaInfo_x32;
                //FileStream streamDLL = new FileStream (Directory.GetCurrentDirectory () + "\\Mediainfo.dll", FileMode.Create, FileAccess.ReadWrite);//  VideoKatalog.View.Properties.Resources.MediaInfo_x64;
                //BinaryWriter bw = new BinaryWriter (streamDLL);
                //bw.Write (dll);
                //bw.Close ();
                //File.Copy (Directory.GetCurrentDirectory () + "MediaInfo-x32.dll", Directory.GetCurrentDirectory () + "Mediainfo.dll");
                //File.Delete (Directory.GetCurrentDirectory () + "MediaInfo-x64.dll");
                //File.Delete (Directory.GetCurrentDirectory () + "MediaInfo-x32.dll");
            }
        }

        #endregion

        #region Animation & sound
        

        private void playOpenSound (object sender, MouseButtonEventArgs e) {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer (VideoKatalog.View.Properties.Resources.open2);
            player.Play ();
            //ChangeDefaultButton (sender);
        }
        private void playCloseSound (object sender, RoutedEventArgs e) {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer (VideoKatalog.View.Properties.Resources.close5);
            player.Play ();
            //ChangeDefaultButton (sender);
        }

        #endregion

        string SearchTitleReturnLink () {
            SearchForm search = new SearchForm ();
            search.Owner = this;
            search.ShowDialog ();
            return search.selectedLink;
        }
        private void SetRatingCanvas (Canvas canvas, decimal rating) {
            float ratingFloat = float.Parse (rating.ToString ());
            Bitmap ratingImageBitmap = VideoKatalog.View.Properties.Resources.ratingStars;
            MemoryStream ms = new MemoryStream ();
            ratingImageBitmap.Save (ms, System.Drawing.Imaging.ImageFormat.Png);
            BitmapImage ratingStars = new BitmapImage ();
            ratingStars.BeginInit ();
            ratingStars.StreamSource = new MemoryStream (ms.ToArray ());
            ratingStars.EndInit ();
            ImageBrush myBrush = new ImageBrush ();
            myBrush.ImageSource = ratingStars;
            myBrush.TileMode = TileMode.Tile;
            myBrush.Stretch = Stretch.None;
            int fullWidth = (int) this.ratingMask.Width;
            int width = (int) (ratingFloat * fullWidth / 10);
            myBrush.Viewport = new Rect (0, 0, 10 / ratingFloat * 0.202, 1);

            System.Windows.Shapes.Rectangle newRect = new System.Windows.Shapes.Rectangle ();
            newRect.Height = 29;
            newRect.Width = width; //(ratingFloat * this.ratingMask.Width / 10)
            newRect.Stroke = null;
            newRect.Fill = myBrush;

            canvas.Children.Clear ();
            canvas.Children.Add (newRect);
        }

        private void openHDDsManagerMenu_Click (object sender, RoutedEventArgs e) {
            HDDsManagerForm hddManager = new HDDsManagerForm ();
            hddManager.Owner = this;
            hddManager.ShowDialog ();
            foreach (HDD tempHDD in hddManager.removedHDDs) {
                foreach (Movie tempMovie in listOfMovies) {
                    if (tempMovie.Hdd.ID == tempHDD.ID)
                        listOfMovies.Remove (tempMovie);
                }
                foreach (SerieEpisode tempEpisode in listOfSerEpisodes) {
                    if (tempEpisode.Hdd.ID == tempHDD.ID) {
                        listOfSerEpisodes.Remove (tempEpisode);
                        tempEpisode.ParentSeason.Episodes.Remove (tempEpisode);
                        if (tempEpisode.ParentSeason.Episodes.Count == 0) {
                            listOfSerSeasons.Remove (tempEpisode.ParentSeason);
                            tempEpisode.ParentSeason.ParentSerie.Seasons.Remove (tempEpisode.ParentSeason);
                            DatabaseManager.DeleteSerieSeason (tempEpisode.ParentSeason);
                            //DatabaseManagerMySql.DeleteSerieSeason (tempEpisode.ParentSeason);
                            if (tempEpisode.ParentSeason.ParentSerie.Seasons.Count == 0) {
                                listOfSeries.Remove (tempEpisode.ParentSeason.ParentSerie);
                                DatabaseManager.DeleteSerie (tempEpisode.ParentSeason.ParentSerie);
                                //DatabaseManagerMySql.DeleteSerie (tempEpisode.ParentSeason.ParentSerie);
                            }
                        }
                    }
                }
                foreach (HomeVideo tempHomeVideo in listOfHomeVideos) {
                    if (tempHomeVideo.Hdd.ID == tempHDD.ID)
                        listOfHomeVideos.Remove (tempHomeVideo);
                }
            }
        }
        private void startSelection_Click (object sender, RoutedEventArgs e) {
            SelectionWindow selection = new SelectionWindow (listOfMovies, listOfSeries, personList, languageList, genreList, hddList, audioList);
            selection.Show ();
        }
        private void wishListMenu_Click (object sender, RoutedEventArgs e) {
            WishListForm wishList = new WishListForm (personList, genreList, cameraList, homeVideoCatList, listOfSeries, listOfSerSeasons);
            wishList.Owner = this;
            wishList.ShowDialog ();
            foreach (HDD tempHDD in wishList.insertedHDDs)
                hddList.Add (tempHDD);
            foreach (Movie tempMovie in wishList.insertedMovies) {
                string temp = "";
                AddNewMovie (tempMovie, false, ref temp);
            }
            foreach (HomeVideo tempHomeVideo in wishList.insertedHomeVideos) {
                AddHomeVideo (tempHomeVideo);
            }
            foreach (Serie tempSerie in wishList.insertedSeries) {
                bool foundSerie = false;
                bool foundSeason = false;
                foreach (Serie existingSerie in listOfSeries) {
                    if (tempSerie.ID == existingSerie.ID) {
                        foundSerie = true;
                        foreach (SerieSeason tempSeason in tempSerie.Seasons) {
                            foreach (SerieSeason existingSeason in existingSerie.Seasons) {
                                if (tempSeason.ID == existingSeason.ID) {
                                    foundSeason = true;
                                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                                        AddSerieEpisode (tempEpisode);
                                        existingSeason.Episodes.Add (tempEpisode);
                                    }
                                    break;
                                }
                            }
                            if (foundSeason == false) {
                                AddSerieSeason (tempSeason);
                                existingSerie.Seasons.Add (tempSeason);
                            }
                        }
                        break;
                    }
                }
                if (foundSerie == false)
                    AddSerie (tempSerie);
            }
        }

        #region MOVIE TAB

        #region Global data

        private ObservableCollection<Movie> listOfMovies = new ObservableCollection<Movie> ();
        CollectionView _movieView;

        #endregion

        #region Add / Edit / Remove

        Thread THAddRemoveMovie;

        //ADD
        private void newMovieMenu_Click (object sender, RoutedEventArgs e) {
            string link = SearchTitleReturnLink ();
            if (String.IsNullOrWhiteSpace (link)) {
                return;
            }
            MovieForm newMovieForm = new MovieForm (link, WishOrRegular.regular);
            newMovieForm.Owner = this;
            Movie newMovie = newMovieForm.currMovie;
            bool contains = listOfMovies.Contains (newMovie, new MovieComparerByNameYear ());
            newMovieForm.movieExists = contains;
            newMovieForm.ShowDialog ();

            if (newMovieForm.accepted) {
                busyIncidator.IsBusy = true;
                allThreads.Add (THAddRemoveMovie);
                busyIncidator.BusyContent = "Unosim u bazu podataka...";
                foreach (HDD tempHDD in newMovieForm.insertedHDDs)
                    hddList.Add (tempHDD);
                string temp = "";
                ThreadStart starter = delegate { AddNewMovie (newMovie, false, ref temp); };
                THAddRemoveMovie = new Thread (starter);
                THAddRemoveMovie.IsBackground = true;
                THAddRemoveMovie.Start ();
                //AddNewMovie (newMovie);
                //movieNumberTextBlock.Text = _movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();//_movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();
            }
            else {
                return;
            }
        }
        void AddNewMovie (Movie movie, bool automatic, ref string notFetched) {
            Movie cloneMovie = cloner.CloneMovie (movie);
            try {
               
                cloneMovie.Size += 1000000; //slike i prijevod cca 1MB

                SetBusyStateAndContent (movie.Name + "\r\nUnosim HDD", true);               
                SetHDDPointerOrInsert (cloneMovie.Hdd);

                SetBusyStateAndContent (movie.Name + "\r\nUnosim jezike", true);
                SetLanguagePointersOrInsert (cloneMovie.SubtitleLanguageList);

                SetBusyStateAndContent (movie.Name + "\r\nUnosim audio", true);
                SetAudioPointersOrInsert (cloneMovie.AudioList);

                SetBusyStateAndContent (movie.Name + "\r\nUnosim glumce", true);
                SetPersonPointersOrInsert (cloneMovie.Actors);

                SetBusyStateAndContent (movie.Name + "\r\nUnosim direktora", true);
                SetPersonPointersOrInsert (cloneMovie.Directors);
                SetGenrePointers (cloneMovie.Genres);
                SetBusyStateAndContent (movie.Name + "\r\nUnosim film", true);

                DatabaseManager.InsertNewMovie (cloneMovie);
                DatabaseManagerRemote.InsertNewMovie(cloneMovie);
                //DatabaseManagerMySql.InsertNewMovie (cloneMovie);
                movie = cloneMovie;
                movie.Hdd.FreeSpaceDecrease (movie.Size);
                FinderInCollection.FindInHDDCollection (hddList, movie.Hdd.ID).FreeSpaceDecrease (movie.Size);
                DatabaseManager.UpdateHDD (movie.Hdd);
                SetBusyStateAndContent (cloneMovie.Name + "\r\nUploadam poster", true);
                try {
                    HelpFunctions.DeletePhotoFTP (cloneMovie.Name + ".jpg", GlobalCategory.movie);
                }
                catch {
                }
                HelpFunctions.UploadPhotoFTP (HardcodedStrings.moviePosterFolder + cloneMovie.Name + ".jpg", GlobalCategory.movie, cloneMovie.Name + ".jpg");

                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    listOfMovies.Add (movie);
                    string cancelDeleteImage = HardcodedStrings.moviePosterFolder + movie.Name + ".jpg";
                    foreach (string path in imagesToDelete) 
                        if (path == cancelDeleteImage) {
                            cancelDeleteImage = path;
                            break;
                        }                    
                    try {
                        imagesToDelete.Remove (cancelDeleteImage);
                    }
                    catch {
                    }
                    this.movieListBox.Focus ();
                    _movieView.MoveCurrentTo (movie);
                    movieListBox.UpdateLayout ();
                    movieListBox.ScrollIntoView (movie);
                }));

            }
            catch (Exception e) {
                if (automatic == false)
                    MessageBox.Show (e.Message);
                else
                    notFetched += "\r\n" + e.Message;
                SetBusyStateAndContent ("Brišem film", true);
                DatabaseManager.DeleteMovie (cloneMovie);
                try {
                    //DatabaseManagerMySql.DeleteMovie (cloneMovie);
                }
                catch {
                }
                if (MessageBox.Show ("Probati ponovno?", "", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes) {
                    AddNewMovie (cloneMovie, automatic, ref notFetched);
                }
                SetBusyStateAndContent ("", false); this.Dispatcher.Invoke (new Action (() => {
                    this.movieListBox.Focus ();
                }));
            }

        }

        //EDIT
        private void editMovieMenu_Click (object sender, RoutedEventArgs e) {
            Movie movieBackUp = (Movie) _movieView.CurrentItem;
            if (movieBackUp == null)
                return;
            MovieForm editMovieForm = new MovieForm (movieBackUp);
            editMovieForm.Owner = this;
            int viewIndex = _movieView.CurrentPosition;
            int listIndex = listOfMovies.IndexOf (movieBackUp);

            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.moviePosterFolder + 
                    movieBackUp.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            listOfMovies.RemoveAt (listIndex);
            editMovieForm.ShowDialog ();

            if (editMovieForm.accepted) {
                movieBackUp.Hdd = FinderInCollection.FindInHDDCollection (hddList, movieBackUp.Hdd.ID);
                movieBackUp.Hdd.FreeSpace += movieBackUp.Size;
                DatabaseManager.UpdateHDD (movieBackUp.Hdd);
                busyIncidator.IsBusy = true;
                allThreads.Add (THAddRemoveMovie);
                busyIncidator.BusyContent = "Updateam online bazu podataka...";
                foreach (HDD tempHDD in editMovieForm.insertedHDDs)
                    hddList.Add (tempHDD);
                Movie editedMovie = editMovieForm.currMovie;
                editedMovie.Hdd = FinderInCollection.FindInHDDCollection (hddList, editedMovie.Hdd.ID);
                editedMovie.Hdd.FreeSpace -= editedMovie.Size;
                if (movieBackUp.Size != editedMovie.Size)
                    editedMovie.AddTime = DateTime.Now;
                DatabaseManager.UpdateHDD (editedMovie.Hdd);
                ThreadStart starter = delegate { 
                    UpdateMovie (editedMovie); 
                };
                THAddRemoveMovie = new Thread (starter);
                THAddRemoveMovie.IsBackground = true;
                THAddRemoveMovie.Start ();
                listOfMovies.Insert (listIndex, editedMovie);
            }
            else {
                listOfMovies.Insert (listIndex, movieBackUp);
            }
            _movieView.MoveCurrentToPosition (viewIndex);
        }
        void UpdateMovie (Movie movie) {
            try {
                SetBusyStateAndContent (movie.Name + "Updateam sporedne podatke", true);
                SetHDDPointerOrInsert (movie.Hdd);
                SetLanguagePointersOrInsert (movie.SubtitleLanguageList);
                SetPersonPointersOrInsert (movie.Actors);
                SetPersonPointersOrInsert (movie.Directors);
                SetGenrePointers (movie.Genres);
                SetAudioPointersOrInsert (movie.AudioList);

                SetBusyStateAndContent (movie.Name + "\r\nUpdateam film", true);
               
                DatabaseManager.UpdateMovie (movie);
                DatabaseManagerRemote.UpdateMovie(movie);
                //DatabaseManagerMySql.UpdateMovie (movie);
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    this.movieListBox.Focus ();
                    movieListBox.UpdateLayout ();
                    movieListBox.ScrollIntoView (movie);
                    _movieView.MoveCurrentTo (movie);
                }));
               
                SetBusyStateAndContent (movie.Name + "\r\nUploadam poster", true);
                try {
                    HelpFunctions.DeletePhotoFTP (movie.Name + ".jpg", GlobalCategory.movie);
                }
                catch {
                }
                HelpFunctions.UploadPhotoFTP (HardcodedStrings.moviePosterFolder + movie.Name + ".jpg", GlobalCategory.movie, movie.Name + ".jpg");
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
                SetBusyStateAndContent ("", false);
                this.Dispatcher.Invoke (new Action (() => {
                    this.movieListBox.Focus ();
                }));
            }
            SetBusyStateAndContent ("", false); 
            this.Dispatcher.Invoke (new Action (() => {
                this.movieListBox.Focus ();
            }));
        }

        //DELETE
        private void deleteMovieMenu_Click (object sender, RoutedEventArgs e) {
            Movie selectedMovie = (Movie) _movieView.CurrentItem;
            if (selectedMovie == null)
                return;
            string confirmMessage = "Brisanje filma: " + selectedMovie.Name.ToUpper () + "\r\nJeste li sigurni?";
            if (Xceed.Wpf.Toolkit.MessageBox.Show (confirmMessage, "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                busyIncidator.IsBusy = true;
                busyIncidator.BusyContent = "Brišem film";
                allThreads.Add (THAddRemoveMovie);
                ThreadStart starter = delegate { DeleteMovie (selectedMovie); };
                THAddRemoveMovie = new Thread (starter);
                THAddRemoveMovie.IsBackground = true;
                THAddRemoveMovie.Start ();

                int viewIndex = _movieView.CurrentPosition;
                int listIndex = listOfMovies.IndexOf (selectedMovie);
                _movieView.MoveCurrentToPrevious ();
                _movieView.MoveCurrentToNext ();
                listOfMovies.RemoveAt (listIndex);
                imagesToDelete.Add (HardcodedStrings.moviePosterFolder + selectedMovie.Name + ".jpg");
                //movieNumberTextBlock.Text = _movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();// listOfMovies.Count ().ToString ();   //_movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();
            }
        }
        private void DeleteMovie (Movie movie) {
            try {
                DatabaseManager.DeleteMovie (movie);
                DatabaseManagerRemote.DeleteMovie(movie);
                //DatabaseManagerMySql.DeleteMovie (movie);
                movie.Hdd = FinderInCollection.FindInHDDCollection (hddList, movie.Hdd.ID);
                movie.Hdd.FreeSpace += movie.Size;
                DatabaseManager.UpdateHDD (movie.Hdd);
                if (File.Exists (HardcodedStrings.moviePosterFolder + movie.Name + ".jpg"))
                    HelpFunctions.DeletePhotoFTP (movie.Name + ".jpg", GlobalCategory.movie);
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    this.movieListBox.Focus ();
                }));
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
                SetBusyStateAndContent ("", false);
                this.Dispatcher.Invoke (new Action (() => {
                    this.movieListBox.Focus ();
                }));
            }
        }

        //AUTOMATIC INSERT
        FolderPickerLib.FolderPickerDialog movieFolderPicker;
        private void automaticInsert_Click (object sender, System.Windows.RoutedEventArgs e) {
            SelectFromCollection selectHddForm = new SelectFromCollection (hddList, "Odaberi HDD");
            selectHddForm.Owner = this;
            selectHddForm.ShowDialog ();
            HDD selectedHDD = (HDD) selectHddForm.selectedObject;
            if (selectedHDD == null)
                return;
            movieFolderPicker = new FolderPickerLib.FolderPickerDialog ();
            if (movieFolderPicker.ShowDialog () == true) {
                DirectoryInfo dirInfo = new DirectoryInfo (movieFolderPicker.SelectedPath);
                ThreadStart starter = delegate { AutomaticInsert (dirInfo, selectedHDD); };
                StreamWriter summaryWriter = new StreamWriter (dirInfo.FullName + "\\Summary.txt");
                summaryWriter.Close ();
                THAddRemoveMovie = new Thread (starter);
                THAddRemoveMovie.IsBackground = true;
                THAddRemoveMovie.Start ();                
            }           
        }
        void AutomaticInsert (DirectoryInfo dirInfo, HDD selectedHDD) {
            foreach (DirectoryInfo directory in dirInfo.GetDirectories()) {
                StreamWriter summaryWriter;               
                StreamReader summaryReader;
                summaryReader = new StreamReader (dirInfo.FullName + "\\Summary.txt");
                string backupString = summaryReader.ReadToEnd ();
                summaryReader.Close ();
                summaryWriter = new StreamWriter (dirInfo.FullName + "\\Summary.txt");
                summaryWriter.Write (backupString);
                string movieName = directory.Name;
                #region Remove chars inside brackets
                int start = movieName.IndexOf ("[");
                int end = movieName.IndexOf ("]");
                while (start > -1) {
                    movieName = movieName.Remove (start, end - start + 1);
                    start = movieName.IndexOf ("[");
                    end = movieName.IndexOf ("]");
                }               
                #endregion
                movieName = movieName.Trim ();
                SetBusyStateAndContent (movieName, true);
                FileInfo[] files = directory.GetFiles ();
                string videoPath = "";
                foreach (FileInfo file in files)
                    if (HelpFunctions.IsVideoFile (file)) {
                        videoPath = file.FullName;
                        break;
                    }
                if (videoPath == "") {
                    summaryWriter.Write (movieName + " uopce nije unesen\r\n\r\n");
                    continue;
                }
                summaryWriter.Close();
                Movie tempMovie = new Movie ();
                string allNotFetched = "";
                this.Dispatcher.Invoke (new Action (() => {
                    SearchForm searchImdb = new SearchForm (movieName);
                    searchImdb.Owner = this;
                    searchImdb.ShowDialog ();
                    string link = searchImdb.selectedLink;
                    MovieForm form = new MovieForm (link, WishOrRegular.regular, videoPath, selectedHDD, movieName);
                    form.Owner = this;
                    form.ShowDialog ();
                    tempMovie = form.currMovie;
                    allNotFetched = form.notFetched;
                }));

                summaryReader = new StreamReader (dirInfo.FullName + "\\Summary.txt");
                backupString = summaryReader.ReadToEnd ();
                summaryReader.Close ();
                summaryWriter = new StreamWriter (dirInfo.FullName + "\\Summary.txt");
                summaryWriter.Write (backupString);
                summaryWriter.Write (movieName + " -> " + tempMovie.OrigName);
                               
                AddNewMovie (tempMovie, true, ref allNotFetched);
                if (allNotFetched.Length != 0) {
                    summaryWriter.Write (allNotFetched + "\r\n\r\n");
                }
                else
                    summaryWriter.Write ("\r\n\r\n");
                summaryWriter.Close ();
                this.Dispatcher.Invoke (new Action (() => {
                    _movieView.MoveCurrentToLast ();
                }));
            }
            SetBusyStateAndContent ("", false);
            System.Diagnostics.Process.Start (dirInfo.FullName + "\\Summary.txt");
        }

        #endregion

        private void ChangeMovieListTemplate (object sender, System.Windows.Input.MouseButtonEventArgs e) {
            busyIncidator.IsBusy = true;
            busyIncidator.BusyContent = "Postavljanje teme";
            TextBlock selected = sender as TextBlock;

            ThreadStart starter = delegate {
                DateTime now = DateTime.Now;
                while ((DateTime.Now - now).Milliseconds < 300) ;
                string buttonName = "";
                this.Dispatcher.Invoke (new Action (() => {
                    buttonName = selected.Name.ToLower();
                }));

                if (buttonName.Contains ("Simple")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        try {
                            movieListBox.ItemTemplate = null;
                            var panelTemplate = (ItemsPanelTemplate) TryFindResource ("movieListSimplePanel");
                            movieListBox.ItemsPanel = panelTemplate;
                        }
                        catch {
                            MessageBox.Show ("Nije uspjelo mijenjanje teme");
                            this.Dispatcher.Invoke (new Action (() => {
                                movieListBox.UpdateLayout ();
                                movieListBox.ScrollIntoView (_movieView.GetItemAt (_movieView.CurrentPosition + 4)); //+4 da bas ne bude prvi dolje
                                busyIncidator.IsBusy = false;
                            }));
                        }
                    }));
                }
                else if (buttonName.Contains ("advanced")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        try {
                            var listTemplate = (DataTemplate) TryFindResource ("movieListAdvancedTemplate");
                            movieListBox.ItemTemplate = listTemplate;
                            var panelTemplate = (ItemsPanelTemplate) TryFindResource ("movieListSimplePanel");
                            movieListBox.ItemsPanel = panelTemplate;
                        }
                        catch {
                            MessageBox.Show ("Nije uspjelo mijenjanje teme");
                            this.Dispatcher.Invoke (new Action (() => {
                                movieListBox.UpdateLayout ();
                                movieListBox.ScrollIntoView (_movieView.GetItemAt (_movieView.CurrentPosition + 4)); //+4 da bas ne bude prvi dolje
                                busyIncidator.IsBusy = false;
                            }));
                        }
                    }));
                }
                else if (buttonName.Contains ("poster")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        try {
                            var listTemplate = (DataTemplate) TryFindResource ("movieListPosterTemplate");
                            movieListBox.ItemTemplate = listTemplate;
                            var panelTemplate = (ItemsPanelTemplate) TryFindResource ("movieListPosterPanelTemplate");
                            movieListBox.ItemsPanel = panelTemplate;
                        }
                        catch {
                            MessageBox.Show ("Nije uspjelo mijenjanje teme");
                            this.Dispatcher.Invoke (new Action (() => {
                                movieListBox.UpdateLayout ();
                                movieListBox.ScrollIntoView (_movieView.GetItemAt (_movieView.CurrentPosition + 4)); //+4 da bas ne bude prvi dolje
                                busyIncidator.IsBusy = false;
                            }));
                        }
                    }));
                }
                this.Dispatcher.Invoke (new Action (() => {
                    movieListBox.UpdateLayout ();
                    try {
                        movieListBox.ScrollIntoView (_movieView.GetItemAt (_movieView.CurrentPosition + 4)); //+4 da bas ne bude prvi dolje
                    }
                    catch {
                    }
                    busyIncidator.IsBusy = false;

                }));
            };
            Thread SetThemeThread = new Thread (starter);
            SetThemeThread.IsBackground = true;
            SetThemeThread.Start ();
        }

        private void goToIMDbButton_Click (object sender, RoutedEventArgs e) {
            //movieListBox.Focus ();
            //_movieView.MoveCurrentToNext ();
            Movie selectedMovie = (Movie) _movieView.CurrentItem;
            if (selectedMovie == null)
                return;
            Clipboard.SetText(selectedMovie.InternetLink);
            //System.lau
            //System.Diagnostics.Process.Start((new System.Diagnostics.ProcessStartInfo("explorer.exe",
                    //selectedMovie.InternetLink)));
            //System.Diagnostics.Process.Start (selectedMovie.InternetLink);
            //for (int i = 0 ; i < movieListBox.Items.Count ; i++) {
            //    ListBoxItem item = (ListBoxItem) movieListBox.ItemContainerGenerator.ContainerFromIndex (i);
            //    if (item == null)
            //        continue;
            //    if (item.Name == selectedMovie.Name) {
            //        item.Focus ();
            //        item.IsSelected = true;
            //    }
            //    else {
            //        item.IsSelected = false;
            //    }
            //}
            
            //ListBoxItem item = (ListBoxItem) movieListBox.ItemContainerGenerator.ContainerFromIndex (movieListBox.SelectedIndex);
            //if (item != null) {
            //    item.Focus ();
            //    item.IsSelected = true;
            //}
        }
        private void goToTrailerButton_Click (object sender, RoutedEventArgs e) {
            Movie selectedMovie = (Movie) _movieView.CurrentItem;
            if (selectedMovie == null)
                return;
            Clipboard.SetText(selectedMovie.TrailerLink);
            //System.Diagnostics.Process.Start (selectedMovie.TrailerLink);
            movieListBox.Focus ();
        }
        private void isViewedImage_MouseLeftButtonUp (object sender, MouseButtonEventArgs e) {
            Movie changedMovie = ((Movie) movieListBox.SelectedItem);
            int selectedIndex = _movieView.CurrentPosition;
            if (changedMovie == null)
                return;
            changedMovie.IsViewed = !changedMovie.IsViewed;
            DatabaseManager.UpdateMovieChangeOnlyIsViewed (changedMovie);
            _movieView.Filter = FilterMovies ;
            if (selectedIndex >= _movieView.Count)
                _movieView.MoveCurrentToPosition (selectedIndex - 1);
            else
                _movieView.MoveCurrentToPosition (selectedIndex);
            movieListBox.UpdateLayout ();
            ListBoxItem item = (ListBoxItem) movieListBox.ItemContainerGenerator.ContainerFromIndex (_movieView.CurrentPosition);
            //if (item != null)
                item.Focus ();
        }
        private void movieListBox_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            Movie selectedMovie = (Movie) _movieView.CurrentItem;
            if (selectedMovie == null)
                return;
            SetRatingCanvas (movieRatingCanvas, selectedMovie.Rating);
        }

        #region Sort / Filter

        List<string> showingMoviesFromHDDs = new List<string> ();
        private void movieSearchGridClose_Click (object sender, System.Windows.RoutedEventArgs e) {
            playCloseSound (null, null);
            _movieView.Filter = FilterMovies;
            _movieView.MoveCurrentToPosition (0);
        }
        private bool FilterMovies (object movie) {
            Movie tempMovie = movie as Movie;

            //if (tempMovie.Name == "Ocean Oasis") {
            //    int bla;
            //}                

            #region Naziv filma

            if (string.IsNullOrEmpty (movieQuickSearch.Text.Trim ()) == false &&
                tempMovie.Name.ToLower ().Contains (movieQuickSearch.Text.ToLower ().Trim ()) == false &&
                tempMovie.OrigName.ToLower ().Contains (movieQuickSearch.Text.ToLower ().Trim ()) == false)
                return false;

            //if (string.IsNullOrEmpty (movieQuickSearch.Text.ToLower().Trim()) &&
            //    tempMovie.Name.ToLower().Contains (movieQuickSearch.Text.ToLower().Trim()) == false &&
            //    tempMovie.OrigName.ToLower().Contains (movieQuickSearch.Text.ToLower().Trim()) == false)
            //    return false;

            //if (tempMovie.Name.ToLower ().Contains (searchMovieName.Text.ToLower ().Trim()) == false && 
            //    tempMovie.Name.ToLower().Contains (movieQuickSearch.Text.ToLower().Trim()) == false &&
            //    tempMovie.OrigName.ToLower ().Contains (searchMovieName.Text.ToLower ().Trim()) == false &&
            //    tempMovie.OrigName.ToLower ().Contains (movieQuickSearch.Text.ToLower().Trim()) == false &&
            //    searchMovieName.Text != "" && movieQuickSearch.Text != "")
            //   return false;

            #endregion

            #region Slideri

            //Godina
            if ((double) tempMovie.Year < yearSliderMovie.LowerValue || (double) tempMovie.Year > yearSliderMovie.UpperValue)
                return false;

            //Ocjena
            if ((double) tempMovie.Rating < ratingSliderMovie.LowerValue || (double) tempMovie.Rating > ratingSliderMovie.UpperValue)
                return false;

            //Veličina
            if ((double) tempMovie.Size < sizeSliderMovie.LowerValue || (double) tempMovie.Size > sizeSliderMovie.UpperValue)
                return false;

            //Trajanje
            if (tempMovie.Runtime < runtimeSliderMovie.LowerValue || tempMovie.Runtime > runtimeSliderMovie.UpperValue)
                return false;

            //Budget
            if (tempMovie.Budget < budgetSliderMovie.LowerValue || tempMovie.Budget > budgetSliderMovie.UpperValue)
                return false;

            //Zarada
            if (tempMovie.Earnings < earningsSliderMovie.LowerValue || tempMovie.Earnings > earningsSliderMovie.UpperValue)
                return false;

            //Bitrate
            //if (tempMovie.Bitrate < bitrateSliderMovie.LowerValue || tempMovie.Bitrate > bitrateSliderMovie.UpperValue)
            //    return false;

            #endregion

            #region Glumci / Redatelji

            //Glumci
            if (searchMovieCast.Text.Length > 0) {
                bool foundActor = false;
                foreach (Person actor in tempMovie.Actors)
                    if (actor.Name.ToLower ().Contains (searchMovieCast.Text.ToLower ()))
                        foundActor = true;
                if (foundActor == false)
                    return false;
            }

            //Redatelji
            if (searchMovieDirector.Text.Length > 0) {
                bool foundDirector = false;
                foreach (Person director in tempMovie.Directors)
                    if (director.Name.ToLower ().Contains (searchMovieDirector.Text.ToLower ()))
                        foundDirector = true;
                if (foundDirector == false)
                    return false;
            }

            #endregion

            #region Rezolucija
           
            Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter ();
            string resolution = resolutionConverter.Convert (tempMovie.Width, typeof (string), null, null).ToString ();
            if (resolution == "nije HD" && rezSDMovie.IsChecked == false)
                return false;
            if (resolution == "720p" && rez720pMovie.IsChecked == false)
                return false;
            if (resolution == "1080p" && rez1080pMovie.IsChecked == false)
                return false;

            #endregion
            
            #region Prijevod i HDD

            //Prijevod
            Language cro = new Language ();
            cro.Name = "Croatian";
            Language eng = new Language();
            eng.Name = "English";

            if (subCroMovie.IsChecked == true && tempMovie.SubtitleLanguageList.Contains (cro, new LanguageComparerByName ()) == false)
                return false;
            if (subEngMovie.IsChecked == true && tempMovie.SubtitleLanguageList.Contains (eng, new LanguageComparerByName ()) == false)
                return false;
            if (subNoCroMovie.IsChecked == true && tempMovie.SubtitleLanguageList.Contains (cro, new LanguageComparerByName ()) == true)
                return false;

            //HDD
            if (showingMoviesFromHDDs.Contains (tempMovie.Hdd.Name) == false)
                return false;


            #endregion

            #region Žanrovi

            bool foundOneGenre = false;
            foreach (Genre tempGenre in tempMovie.Genres) {
                if (tempGenre.Name == "Akcija" && actionCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Avantura" && adventureCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Animacija" && animationCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Biografija" && biographyCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Komedija" && comedyCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Krimi" && crimeCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Dokumentarni" && documentaryCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Drama" && dramaCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Obiteljski" && familyCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Fantastika" && fantasyCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Crno-bijeli" && filmNoirCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Povijesni" && historyCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Horor" && horrorCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Muzički" && musicCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Pjevan" && musicalCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Misterija" && misteryCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Romantika" && romanceCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Znanstvena fantastika" && sciFiCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Sportski" && sportCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Triler" && thrillerCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Ratni" && warCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Vestern" && westernCheckBox.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
            }

            if (foundOneGenre == false)
                return false;

            #endregion

            #region Pogledano / nepogledano

            if (tempMovie.IsViewed == false && movieSearchShowNotViewed.IsChecked == false)
                return false;
            if (tempMovie.IsViewed == true && movieSearchShowViewed.IsChecked == false)
                return false;
            
            #endregion

            //Ako je sve proslo
            return true;
        }
        private bool QuickFilterMovies (object movie) {
            Movie currMovie = movie as Movie;
            if (movieQuickSearch.Text.Length <= 0)
                return true;
            if (movieQuickSearch.Text.Length > 0 && currMovie.Name.ToLower ().Contains (movieQuickSearch.Text.ToLower ()))
                return true;
            return false;
        }

        bool movieGenreGridExpanded = false;
        private void openCloseMovieGenreGrid_Click (object sender, System.Windows.RoutedEventArgs e) {
            if (movieGenreGridExpanded) {
                var stb = (Storyboard) TryFindResource ("movieGenreGridCollapse");
                //showingSelectHddSize = false;
                if (stb != null) {
                    stb.Begin ();
                }
                movieGenreGridExpanded = false;
            }
            else {
                var stb = (Storyboard) TryFindResource ("movieGenreGridExpand");
                //showingSelectHddSize = false;
                if (stb != null) {
                    stb.Begin ();
                }
                movieGenreGridExpanded = true;
            }
        }
        private void movieGenreAll_Click (object sender, System.Windows.RoutedEventArgs e) {
            SetMovieGenreCheckboxes (true);
        }
        private void movieGenreNone_Click (object sender, System.Windows.RoutedEventArgs e) {
            SetMovieGenreCheckboxes (false);
        }
        private void SetMovieGenreCheckboxes (bool value) {
            actionCheckBox.IsChecked = value;
            adventureCheckBox.IsChecked = value;
            animationCheckBox.IsChecked = value;
            biographyCheckBox.IsChecked = value;
            comedyCheckBox.IsChecked = value;
            crimeCheckBox.IsChecked = value;
            documentaryCheckBox.IsChecked = value;
            dramaCheckBox.IsChecked = value;
            familyCheckBox.IsChecked = value;
            fantasyCheckBox.IsChecked = value;
            filmNoirCheckBox.IsChecked = value;
            historyCheckBox.IsChecked = value;
            horrorCheckBox.IsChecked = value;
            musicalCheckBox.IsChecked = value;
            musicCheckBox.IsChecked = value;
            misteryCheckBox.IsChecked = value;
            sciFiCheckBox.IsChecked = value;
            romanceCheckBox.IsChecked = value;
            sportCheckBox.IsChecked = value;
            thrillerCheckBox.IsChecked = value;
            warCheckBox.IsChecked = value;
            westernCheckBox.IsChecked = value;
        }

        private void movieSearchHddChecked (object sender, System.Windows.RoutedEventArgs e) {
            CheckBox checkedHDD = sender as CheckBox;
            if (checkedHDD.IsChecked == true) {
                showingMoviesFromHDDs.Add (checkedHDD.Content.ToString());
            }
            else {
                List<string> listToRemove = new List<string> ();
                foreach (string tempHDD in showingMoviesFromHDDs)
                    if (tempHDD == checkedHDD.Content.ToString ())
                        listToRemove.Add (tempHDD);
                foreach (string toRemove in listToRemove)
                    showingMoviesFromHDDs.Remove (toRemove);
            }
        }

        private void ChangeSortingDirection (object sender, System.Windows.RoutedEventArgs e) {
            try {
                SortDescription tempSort = new SortDescription ();
                tempSort.PropertyName = _movieView.SortDescriptions[0].PropertyName;
                string senderName = ((System.Windows.Controls.Primitives.ToggleButton) sender).Name;
                if (senderName.ToLower ().Contains ("ascending"))
                    tempSort.Direction = ListSortDirection.Ascending;
                else
                    tempSort.Direction = ListSortDirection.Descending;
                _movieView.SortDescriptions[0] = tempSort;
            }
            catch {
                return;
            }
        }
        private void SortMovies (object sender, System.Windows.Input.MouseButtonEventArgs e) {
            SortDescription movieSort = new SortDescription ();
            TextBlock clickedTB = sender as TextBlock;
            if (movieSortDescending.IsChecked == true)
                movieSort.Direction = ListSortDirection.Descending;
            else
                movieSort.Direction = ListSortDirection.Ascending;

            if (clickedTB.Name.Contains ("Year")) {
                movieSort.PropertyName = "Year";
            }
            else if (clickedTB.Name.Contains ("Rating")) {
                movieSort.PropertyName = "Rating";
            }
            else if (clickedTB.Name.Contains ("Runtime")) {
                movieSort.PropertyName = "Runtime";
            }
            else if (clickedTB.Name.Contains ("Size")) {
                movieSort.PropertyName = "Size";
            }
            else if (clickedTB.Name.Contains ("Name")) {
                movieSort.PropertyName = "Name";
            }
            else if (clickedTB.Name.Contains ("Bitrate")) {
                movieSort.PropertyName = "Bitrate";
            }
            else if (clickedTB.Name.Contains ("Budget")) {
                movieSort.PropertyName = "Budget";
            }
            else if (clickedTB.Name.Contains ("Earnings")) {
                movieSort.PropertyName = "Earnings";
            }
            else if (clickedTB.Name.Contains ("AddDate")) {
                movieSort.PropertyName = "AddTime";
            }

            //cuvaj samo 3 sortdescriptionsa
            for (int i = 2; i < _movieView.SortDescriptions.Count; i++)
                _movieView.SortDescriptions.RemoveAt (i);
            //postavi novi sortdescription na pocetak liste
            try {
                _movieView.SortDescriptions.Add (movieSort);
                _movieView.SortDescriptions[2] = _movieView.SortDescriptions[1];
                _movieView.SortDescriptions[1] = _movieView.SortDescriptions[0];
            }
            catch {
                try {
                    _movieView.SortDescriptions[1] = _movieView.SortDescriptions[0];
                }
                catch {
                }
            }
            for (int i = 0 ; i < _movieView.SortDescriptions.Count ; i++) {
                SortDescription existing = _movieView.SortDescriptions.ElementAt (i);
                if (existing.PropertyName == movieSort.PropertyName)
                    _movieView.SortDescriptions.Remove (existing);
            }           
            _movieView.SortDescriptions[0] = movieSort;
            movieSortDescriptions.Items.Clear ();
            foreach (SortDescription tempsortDesc in _movieView.SortDescriptions)
                movieSortDescriptions.Items.Add (tempsortDesc.PropertyName.ToString () + " (" + tempsortDesc.Direction.ToString ().ToUpper().Substring(0, 3) + ")");
            
        }
        private void movieQuickSearch_TextChanged (object sender, TextChangedEventArgs e) {
            _movieView.Filter = QuickFilterMovies;
            //_movieView.Filter += FilterMovies;
        }

        #endregion

        #endregion

        #region SERIE TAB

        #region Global data

        private ObservableCollection<Serie> listOfSeries = new ObservableCollection<Serie> ();
        private ObservableCollection<SerieSeason> listOfSerSeasons = new ObservableCollection<SerieSeason> ();
        private ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode> ();

        CollectionView _serieView;
        CollectionView _serieSeasonView;
        CollectionView _serieEpisodeView;

        SerieCategory serieCategoryShowing = SerieCategory.serie;
        List<string> showingSeriesFromHDDs = new List<string> ();

        #endregion

        #region Add / Remove / Edit

        Thread THAddRemoveSerie;

        //***  ADD  ***//
        private void newSerieMenu_Click (object sender, RoutedEventArgs e) {
            string link = SearchTitleReturnLink ();
            if (String.IsNullOrWhiteSpace (link))
                return;
            Serie newSerie = new Serie ();
            newSerie.InternetLink = link;
            SerieForm newSerieForm = new SerieForm (newSerie, SerieCategory.serie, AddOrEdit.add);
            newSerieForm.Owner = this;
            newSerieForm.ShowDialog ();

            if (newSerieForm.accepted) {
                foreach (HDD tempHDD in newSerieForm.insertedHDDs)
                    hddList.Add (tempHDD);
                allThreads.Add (THAddRemoveSerie);
                busyIncidator.IsBusy = true;
                /*statusBarTextBlock.Text = "Unošenje...";
                statusBarProgressBar.Value = 0;
                statusBarProgressBar.Maximum = 0;
                statusBarProgressBar.Visibility = System.Windows.Visibility.Visible;
                foreach (SerieSeason tempSeason in newSerie.Seasons) {
                    statusBarProgressBar.Maximum += tempSeason.Episodes.Count + 1;
                }*/
                ThreadStart starter = delegate { AddSerie (newSerie); };
                THAddRemoveSerie = new Thread (starter);
                THAddRemoveSerie.IsBackground = true;
                THAddRemoveSerie.Start ();
            }
        }
        void AddSerie (Serie insertSerie) {
            try {
                SetBusyStateAndContent ("Unosim glumce/direktore/žanrove serije", true);
                
                SetPersonPointersOrInsert (insertSerie.Actors);
                SetPersonPointersOrInsert (insertSerie.Directors);
                SetGenrePointers (insertSerie.Genres);

                this.Dispatcher.Invoke (new Action (() => {
                    listOfSeries.Add (insertSerie);
                    busyIncidator.BusyContent = "Unosim seriju";
                }));

                DatabaseManager.InsertSerie (insertSerie);
                DatabaseManagerRemote.InsertSerie(insertSerie);
                //DatabaseManagerMySql.InsertSerie (insertSerie);

                SetBusyStateAndContent ("Uploadam poster", true);
               
                try {
                    HelpFunctions.DeletePhotoFTP (insertSerie.Name + ".jpg", GlobalCategory.serie);
                }
                catch {
                }
                HelpFunctions.UploadPhotoFTP (HardcodedStrings.seriePosterFolder + insertSerie.Name + ".jpg", GlobalCategory.serie, insertSerie.Name + ".jpg");

                foreach (SerieSeason tempSeason in insertSerie.Seasons) {
                    AddSerieSeason (tempSeason);
                }
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    FillSerieTreeView ();
                    statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
                    statusBarTextBlock.Text = "";
                }));
            }                
            catch (Exception e) {
                MessageBox.Show (e.Message + "\r\nError kod ubacivanja serije");
                this.Dispatcher.Invoke (new Action (() => {
                    try {
                        DatabaseManager.DeleteSerie (insertSerie);
                        DatabaseManagerMySql.DeleteSerie (insertSerie);
                    }
                    catch {
                        MessageBox.Show (e.Message + "\r\nNije uspjelo brisanje serije");
                    }
                    busyIncidator.IsBusy = false;
                }));
            } 
            this.Dispatcher.Invoke (new Action (() => {
                busyIncidator.IsBusy = false;
                FillSerieTreeView ();
            }));
        }
        private void newSerieSeasonMenu_Click (object sender, RoutedEventArgs e) {
            Serie selectedSerie = (Serie) _serieView.CurrentItem;
            Serie clonedSerie = cloner.CloneSerieWithSeasons (selectedSerie);
            SerieForm addSerieSeasonForm = new SerieForm (clonedSerie, SerieCategory.season, AddOrEdit.add);
            addSerieSeasonForm.Owner = this;
            Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + selectedSerie.Name + ".jpg")));
            addSerieSeasonForm.ShowDialog ();
            if (addSerieSeasonForm.accepted) {
                foreach (HDD tempHDD in addSerieSeasonForm.insertedHDDs)
                    hddList.Add (tempHDD);
                
                ThreadStart starter = delegate {
                    foreach (SerieSeason tempSeason in addSerieSeasonForm.currSerie.Seasons) {
                        this.Dispatcher.Invoke (new Action (() => {
                            tempSeason.ParentSerie = selectedSerie;
                            selectedSerie.Seasons.Add (tempSeason);
                            busyIncidator.IsBusy = true;
                            allThreads.Add (THAddRemoveSerie);
                        }));
                        AddSerieSeason (tempSeason);
                    }
                    this.Dispatcher.Invoke (new Action (() => {
                        busyIncidator.IsBusy = false;
                        FillSerieTreeView ();
                    }));
                };
                THAddRemoveSerie = new Thread (starter);
                THAddRemoveSerie.IsBackground = true;
                THAddRemoveSerie.Start ();

            }
        }
        void AddSerieSeason (SerieSeason insertSeason) {
            try {
                SetBusyStateAndContent (string.Format ("{0}\r\n{1} / {2}", insertSeason.ParentSerie.Name, insertSeason.Name, insertSeason.ParentSerie.Seasons.Count), true);
                this.Dispatcher.Invoke (new Action (() => {
                    listOfSerSeasons.Add (insertSeason);
                }));

                DatabaseManager.InsertSerieSeason (insertSeason);
                DatabaseManagerRemote.InsertSerieSeason(insertSeason);
                //DatabaseManagerMySql.InsertSerieSeason (insertSeason);
                foreach (SerieEpisode tempEpisode in insertSeason.Episodes) {
                    AddSerieEpisode (tempEpisode);
                }                
            }
            catch (Exception e) {
                MessageBox.Show (e.Message + "\r\nError kod ubacivanja: " + insertSeason.Name );
                this.Dispatcher.Invoke (new Action (() => {
                    try {
                        DatabaseManager.DeleteSerieSeason (insertSeason);
                        //DatabaseManagerMySql.DeleteSerieSeason (insertSeason);
                        insertSeason.ParentSerie.Seasons.Remove (insertSeason);
                        if (insertSeason.ParentSerie.Seasons.Count == 0) {
                            DatabaseManager.DeleteSerie (insertSeason.ParentSerie);
                            //DatabaseManagerMySql.DeleteSerie (insertSeason.ParentSerie);
                        }
                    }
                    catch {
                        MessageBox.Show (e.Message + "\r\nNije uspjelo brisanje sezone");
                    }
                }));
            }            
        }
        private void newSerieEpisodeMenu_Click (object sender, RoutedEventArgs e) {
            Serie selectedSerie = (Serie) _serieView.CurrentItem;
            Serie cloneSerie = cloner.CloneSerieOnlyInfo (selectedSerie);
            SerieSeason selectedSeason = (SerieSeason) _serieSeasonView.CurrentItem;
            if (selectedSeason == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi sezonu!", "Nije odabrana sezona", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SerieSeason cloneSeason = cloner.CloneSeasonWithEpisodes (selectedSeason);
            cloneSerie.Seasons.Add (cloneSeason);
            SerieForm addSerieEpForm = new SerieForm (cloneSerie, SerieCategory.episode, AddOrEdit.add);
            addSerieEpForm.Owner = this;
            Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + selectedSerie.Name + ".jpg")));
            addSerieEpForm.ShowDialog ();
            if (addSerieEpForm.accepted) {
                foreach (HDD tempHDD in addSerieEpForm.insertedHDDs)
                    hddList.Add (tempHDD);
                foreach (SerieEpisode tempEpisode in addSerieEpForm.currSerie.Seasons.ElementAt (0).Episodes) {
                    tempEpisode.ParentSeason = selectedSeason;
                    selectedSeason.Episodes.Add (tempEpisode);
                    AddSerieEpisode (tempEpisode);
                }
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    FillSerieTreeView ();
                }));
            }
        }
        void AddSerieEpisode (SerieEpisode insertEpisode) {
            try {
                SetBusyStateAndContent ("Unosim prijevode i audio za epizode", true);
               
                insertEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, insertEpisode.Hdd.ID);
                insertEpisode.Hdd.FreeSpace -= insertEpisode.Size;
                DatabaseManager.UpdateHDD (insertEpisode.Hdd);

                SetHDDPointerOrInsert (insertEpisode.Hdd);
                SetLanguagePointersOrInsert (insertEpisode.SubtitleLanguageList);
                SetAudioPointersOrInsert (insertEpisode.AudioList);

                this.Dispatcher.Invoke (new Action (() => {
                    listOfSerEpisodes.Add (insertEpisode);
                    busyIncidator.IsBusy = true;
                    busyIncidator.BusyContent = string.Format ("{0}\r\n{1} / {2}\r\n{3} / {4}",
                        insertEpisode.ParentSeason.ParentSerie.Name, insertEpisode.ParentSeason, insertEpisode.ParentSeason.ParentSerie.Seasons.Count, insertEpisode.Name, insertEpisode.ParentSeason.Episodes.Count);                    
                }));
                DatabaseManager.InsertSerieEpisode (insertEpisode);
                DatabaseManagerRemote.InsertSerieEpisode(insertEpisode);
                //DatabaseManagerMySql.InsertSerieEpisode (insertEpisode);
            }
            catch (Exception e) {
                MessageBox.Show (e.Message + "\r\nError kod ubacivanja: " + insertEpisode.ParentSeason.Name + " -> " + insertEpisode.Name);
                this.Dispatcher.Invoke (new Action (() => {
                    try {
                        DatabaseManager.DeleteSerieEpisode (insertEpisode);
                        //DatabaseManagerMySql.DeleteSerieEpisode (insertEpisode);
                        insertEpisode.ParentSeason.Episodes.Remove (insertEpisode);
                        if (insertEpisode.ParentSeason.Episodes.Count == 0) {
                            DatabaseManager.DeleteSerieSeason (insertEpisode.ParentSeason);
                            //DatabaseManagerMySql.DeleteSerieSeason (insertEpisode.ParentSeason);
                            insertEpisode.ParentSeason.ParentSerie.Seasons.Remove (insertEpisode.ParentSeason);
                            if (insertEpisode.ParentSeason.ParentSerie.Seasons.Count == 0) {
                                DatabaseManager.DeleteSerie (insertEpisode.ParentSeason.ParentSerie);
                                //DatabaseManagerMySql.DeleteSerie (insertEpisode.ParentSeason.ParentSerie);
                            }
                        }
                    }
                    catch {
                        MessageBox.Show (e.Message + "\r\nNije uspjelo brisanje epizode");
                    }
                }));
            }            
        }

        //***  EDIT  ***//
        private void edit_click (object sender, RoutedEventArgs e) {
            if (serieCategoryShowing == SerieCategory.serie)
                editSerieMenu_Click (sender, e);
            else if (serieCategoryShowing == SerieCategory.season)
                editSerieSeasonMenu_Click (sender, e);
            else if (serieCategoryShowing == SerieCategory.episode)
                editSerieEpisodeMenu_Click (sender, e);
            else
                MessageBox.Show ("Ništa odabrano");
        }
        private void editSerieMenu_Click (object sender, RoutedEventArgs e) {
            Serie selectedSerie = (Serie) _serieView.CurrentItem;
            if (selectedSerie == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi seriju!", "Nema odabira", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            Serie cloneSerie = cloner.CloneSerieWithSeasons (selectedSerie);
            SerieForm editSerieForm = new SerieForm (cloneSerie, SerieCategory.serie, AddOrEdit.edit);
            editSerieForm.Owner = this;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + selectedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            int viewIndex = _serieView.CurrentPosition;
            int listIndex = listOfSeries.IndexOf (selectedSerie);
            listOfSeries.RemoveAt (listIndex);

            editSerieForm.ShowDialog ();
            if (editSerieForm.accepted) {
                //ubaci u hdd listu sve hddove dodane u serieForm formi
                foreach (HDD tempHDD in editSerieForm.insertedHDDs)
                    hddList.Add (tempHDD);
                ThreadStart starter = delegate { 
                    UpdateSerie (cloneSerie, editSerieForm.changedVideo); 
                };
                THAddRemoveSerie = new Thread (starter);
                THAddRemoveSerie.IsBackground = true;
                THAddRemoveSerie.Start ();
                
                listOfSeries.Insert (listIndex, cloneSerie);
            }
            else {
                listOfSeries.Insert (listIndex, selectedSerie);
            }
            _serieView.MoveCurrentToPosition (viewIndex);
            FillSerieTreeView ();
        }
        void UpdateSerie (Serie serie, bool changedVideo) {
            try {
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = true;
                    busyIncidator.BusyContent = serie.Name + "\r\nUpdateam seriju";
                }));
                foreach (SerieSeason tempSeason in serie.Seasons) {
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                        SetHDDPointerOrInsert (tempEpisode.Hdd);
                        SetLanguagePointersOrInsert (tempEpisode.SubtitleLanguageList);
                        SetAudioPointersOrInsert (tempEpisode.AudioList);
                        SetPersonPointersOrInsert (serie.Actors);
                        SetPersonPointersOrInsert (serie.Directors);
                        SetGenrePointers (serie.Genres);
                        if (changedVideo)
                            tempEpisode.Version++;
                    }
                }
                DatabaseManager.UpdateSerie (serie);
                DatabaseManagerRemote.UpdateSerie(serie);

                if (changedVideo) {
                    //DatabaseManagerMySql.UpdateSerie (serie);
                }
                else {
                    //DatabaseManagerMySql.UpdateOnlySerie(serie);
                }
                

                try {
                    HelpFunctions.DeletePhotoFTP (serie.Name + ".jpg", GlobalCategory.serie);
                }
                catch {
                }
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.BusyContent = serie.Name + "\r\nUploadam poster";
                }));
                HelpFunctions.UploadPhotoFTP (HardcodedStrings.seriePosterFolder + serie.Name + ".jpg", GlobalCategory.serie, serie.Name + ".jpg"); 
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                }));
            }
            catch (Exception e) {
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                }));
                MessageBox.Show (e.Message);
            }
        }
        private void editSerieSeasonMenu_Click (object sender, RoutedEventArgs e) {
            Serie selectedSerie = (Serie) _serieView.CurrentItem;
            if (selectedSerie == null)
                return;
            Serie cloneSerie = cloner.CloneSerieOnlyInfo (selectedSerie);
            SerieSeason selectedSeason = (SerieSeason) _serieSeasonView.CurrentItem;
            if (selectedSeason == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi sezonu serije!", "Nema odabira", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SerieSeason cloneSeason = cloner.CloneSeasonWithEpisodes (selectedSeason);
            cloneSerie.Seasons.Add (cloneSeason);
            SerieForm editSerieSeasonForm = new SerieForm (cloneSerie, SerieCategory.season, AddOrEdit.edit);
            editSerieSeasonForm.Owner = this;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + selectedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();
            }
            editSerieSeasonForm.ShowDialog ();
            if (editSerieSeasonForm.accepted) {
                foreach (HDD tempHDD in editSerieSeasonForm.insertedHDDs)
                    hddList.Add (tempHDD);
                selectedSerie.Seasons.Remove (selectedSeason);
                cloneSeason.ParentSerie = selectedSerie;
                selectedSerie.Seasons.Add (cloneSeason);
                ThreadStart starter = delegate {
                    UpdateSerieSeason (cloneSeason);
                    SetBusyStateAndContent ("", false);
                };
                THAddRemoveSerie = new Thread (starter);
                THAddRemoveSerie.IsBackground = true;
                THAddRemoveSerie.Start ();
            }
        }
        void UpdateSerieSeason (SerieSeason season) {
            try {
                foreach (SerieEpisode tempEpisode in season.Episodes) {
                    SetHDDPointerOrInsert (tempEpisode.Hdd);
                    SetLanguagePointersOrInsert (tempEpisode.SubtitleLanguageList);
                    SetAudioPointersOrInsert (tempEpisode.AudioList);
                }
                SetBusyStateAndContent ("Updateam sezonu...", true);
                DatabaseManager.UpdateSerieSeason (season);
                DatabaseManagerRemote.UpdateSerieSeason(season);
                //DatabaseManagerMySql.UpdateSerieSeason (season);
            }
            catch (Exception e) {
                SetBusyStateAndContent ("", false);
                MessageBox.Show (e.Message);
            }
        }
        private void editSerieEpisodeMenu_Click (object sender, RoutedEventArgs e) {
            Serie selectedSerie = (Serie) _serieView.CurrentItem;
            if (selectedSerie == null)
                return;
            Serie cloneSerie = cloner.CloneSerieOnlyInfo (selectedSerie);
            SerieSeason selectedSeason = (SerieSeason) _serieSeasonView.CurrentItem;
            if (selectedSeason == null)
                return;
            SerieSeason cloneSeason = cloner.CloneSeasonOnlyInfo (selectedSeason);
            SerieEpisode selectedEpisode = (SerieEpisode) _serieEpisodeView.CurrentItem;
            if (selectedEpisode == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi epizodu sezone serije!", "Nema odabira", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            SerieEpisode cloneEpisode = cloner.CloneEpisode (selectedEpisode);
            cloneSeason.Episodes.Add (cloneEpisode);
            cloneSerie.Seasons.Add (cloneSeason);
            SerieForm addSerieEpForm = new SerieForm (cloneSerie, SerieCategory.episode, AddOrEdit.edit);
            addSerieEpForm.Owner = this;
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.seriePosterFolder + 
                    selectedSerie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            addSerieEpForm.ShowDialog ();
            if (addSerieEpForm.accepted) {
                SerieEpisode editedEpisode = addSerieEpForm.currSerie.Seasons.ElementAt (0).Episodes.ElementAt (0); //currSerie sadrzi samo jednu sezonu koja sadrzi samo jednu epizodu
                foreach (HDD tempHDD in addSerieEpForm.insertedHDDs)
                    hddList.Add (tempHDD);
                selectedSeason.Episodes.Remove (selectedEpisode);
                editedEpisode.ParentSeason = selectedSeason;
                selectedSeason.Episodes.Add (editedEpisode);
                SetBusyStateAndContent ("Updateam epizodu", true);
                ThreadStart starter = delegate {
                    UpdateSerieEpisode (editedEpisode);
                    SetBusyStateAndContent ("", false);
                };
                Thread updateSerieEpisodeThread = new Thread (starter);
                updateSerieEpisodeThread.IsBackground = true;
                updateSerieEpisodeThread.Start ();
            }
        }
        void UpdateSerieEpisode (SerieEpisode episode) {
            try {
                SetHDDPointerOrInsert (episode.Hdd);
                SetLanguagePointersOrInsert (episode.SubtitleLanguageList);
                SetAudioPointersOrInsert (episode.AudioList);
                SetBusyStateAndContent ("Updateam epizodu...", true);
                DatabaseManager.UpdateSerieEpisode(episode);
                DatabaseManagerRemote.UpdateSerieEpisode(episode);
                //DatabaseManagerMySql.UpdateSerieEpisode (episode);
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
            }
        }

        //***  DELETE  ***
        private void delete_Click (object sender, RoutedEventArgs e) {
            if (serieCategoryShowing == SerieCategory.serie)
                deleteSerieMenu_Click (sender, e);
            else if (serieCategoryShowing == SerieCategory.season)
                deleteSerieSeasonMenu_Click (sender, e);
            else if (serieCategoryShowing == SerieCategory.episode)
                deleteSerieEpisodeMenu_Click (sender, e);
            else
                MessageBox.Show ("Ništa odabrano");
        }
        private void deleteSerieMenu_Click (object sender, RoutedEventArgs e) {
            try {
                Serie selectedSerie = (Serie) _serieView.CurrentItem;
                if (selectedSerie == null)
                    return;
                string confirmMessage = "Brisanje serije: " + selectedSerie.Name.ToUpper () + "\r\nJeste li sigurni?";
                if (Xceed.Wpf.Toolkit.MessageBox.Show (confirmMessage, "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    foreach (SerieSeason tempSeason in selectedSerie.Seasons)
                        foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                            tempEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, tempEpisode.Hdd.ID);
                            tempEpisode.Hdd.FreeSpace += tempEpisode.Size;
                            DatabaseManager.UpdateHDD (tempEpisode.Hdd);
                        }

                    ThreadStart starter = delegate {
                        SetBusyStateAndContent ("Brišem", true);
                        DatabaseManager.DeleteSerie (selectedSerie);
                        DatabaseManagerRemote.DeleteSerie(selectedSerie);
                        //DatabaseManagerMySql.DeleteSerie (selectedSerie);
                        SetBusyStateAndContent ("", false);
                    };
                    THAddRemoveSerie = new Thread (starter);
                    THAddRemoveSerie.IsBackground = true;
                    THAddRemoveSerie.Start ();                                       
                                        
                    int viewIndeX = _serieView.CurrentPosition;
                    int listIndex = listOfSeries.IndexOf (selectedSerie);
                    _serieView.MoveCurrentToPrevious ();
                    _serieView.MoveCurrentToNext ();
                    listOfSeries.RemoveAt (listIndex);
                    imagesToDelete.Add (HardcodedStrings.moviePosterFolder + selectedSerie.Name + ".jpg");
                    HelpFunctions.DeletePhotoFTP (selectedSerie.Name + ".jpg", GlobalCategory.serie);
                }
            }
            catch (Exception exc) {
                MessageBox.Show (exc.Message);
            }
        }
        private void deleteSerieSeasonMenu_Click (object sender, RoutedEventArgs e) {
            try {
                Serie selectedSerie = (Serie) _serieView.CurrentItem;
                if (selectedSerie == null)
                    return;
                SerieSeason selectedSeason = (SerieSeason) _serieSeasonView.CurrentItem;
                if (selectedSeason == null)
                    return;
                string confirmMessage = string.Format ("Brisanje: {0}: {1}\r\nJeste li sigurni?", selectedSeason.ParentSerie.Name, selectedSeason.Name.ToUpper ());
                if (Xceed.Wpf.Toolkit.MessageBox.Show (confirmMessage, "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    foreach (SerieEpisode tempEpisode in selectedSeason.Episodes) {
                        tempEpisode.Hdd = FinderInCollection.FindInHDDCollection (hddList, tempEpisode.Hdd.ID);
                        tempEpisode.Hdd.FreeSpace += tempEpisode.Size;
                        DatabaseManager.UpdateHDD (tempEpisode.Hdd);
                    }
                    DatabaseManager.DeleteSerieSeason (selectedSeason);
                    DatabaseManagerRemote.DeleteSerieSeason(selectedSeason);
                    //DatabaseManagerMySql.DeleteSerieSeason (selectedSeason);
                    int viewIndeX = _serieSeasonView.CurrentPosition;
                    int listIndex = listOfSerSeasons.IndexOf (selectedSeason);
                    _serieSeasonView.MoveCurrentToPrevious ();
                    _serieSeasonView.MoveCurrentToNext ();
                    listOfSerSeasons.RemoveAt (listIndex);
                    selectedSerie.Seasons.Remove (selectedSeason);
                }
            }
            catch (Exception exc) {
                MessageBox.Show (exc.Message);
            }
        }
        private void deleteSerieEpisodeMenu_Click (object sender, RoutedEventArgs e) {
            try {
                Serie selectedSerie = (Serie) _serieView.CurrentItem;
                if (selectedSerie == null)
                    return;
                SerieSeason selectedSeason = (SerieSeason) _serieSeasonView.CurrentItem;
                if (selectedSeason == null)
                    return;
                SerieEpisode selectedEpisode = (SerieEpisode) _serieEpisodeView.CurrentItem;
                if (selectedEpisode == null)
                    return;
                string confirmMessage = string.Format ("Brisanje: {0}: {1}, {2}\r\nJeste li sigurni?", selectedSeason.ParentSerie.Name, selectedSeason.Name, selectedEpisode.Name.ToUpper ());
                if (Xceed.Wpf.Toolkit.MessageBox.Show (confirmMessage, "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    DatabaseManager.DeleteSerieEpisode (selectedEpisode);
                    DatabaseManagerRemote.DeleteSerieEpisode(selectedEpisode);
                    //DatabaseManagerMySql.DeleteSerieEpisode (selectedEpisode);
                    int viewIndex = _serieEpisodeView.CurrentPosition;
                    int listIndex = listOfSerEpisodes.IndexOf (selectedEpisode);
                    _serieEpisodeView.MoveCurrentToPrevious ();
                    _serieEpisodeView.MoveCurrentToNext ();
                    listOfSerEpisodes.RemoveAt (listIndex);
                    selectedSeason.Episodes.Remove (selectedEpisode);
                    selectedEpisode.Hdd.FreeSpace += selectedEpisode.Size;
                    DatabaseManager.UpdateHDD (selectedEpisode.Hdd);
                }
            }
            catch (Exception exc) {
                MessageBox.Show (exc.Message);
            }
        }

        #endregion

        private void SetSerieTabConstantBindings () {
            directorTBSerie.DataContext = _serieView;
            castTBSerie.DataContext = _serieView;
            serieGenresTB.DataContext = _serieView;
            serieRatingTB.DataContext = _serieView;
            seriePoster.DataContext = _serieView;
            this.serieNameTB.DataContext = _serieView;
            //this.episodeAirDateTB.DataContext = _serieEpisodeView;
            //this.episodeNameTB.DataContext = _serieEpisodeView;
        }
        private void FillSerieTreeViewFirstTime () {
            foreach (Serie tempSerie in listOfSeries) {
                TreeViewItem serie = new TreeViewItem ();
                this.Dispatcher.Invoke (new Action (() => {
                    serie = new TreeViewItem () { Header = tempSerie.OrigName, DataContext = tempSerie };
                }));
                if (serieTreeView.Items.Count == 0)
                    serie.IsSelected = true;
                serie.Foreground = System.Windows.Media.Brushes.Black;
                serie.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                serie.FontSize = 16;
                foreach (SerieSeason tempSeason in tempSerie.Seasons) {
                    TreeViewItem season = new TreeViewItem ();
                    this.Dispatcher.Invoke (new Action (() => {
                        season = new TreeViewItem () { Header = tempSeason.Name, DataContext = tempSeason };
                    }));
                    season.Foreground = System.Windows.Media.Brushes.Black;
                    season.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                    season.FontSize = 14;
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                        TreeViewItem episode = new TreeViewItem ();
                        this.Dispatcher.Invoke (new Action (() => {
                            episode = new TreeViewItem () { Header = tempEpisode.Name, DataContext = tempEpisode };
                        }));
                        episode.Foreground = System.Windows.Media.Brushes.Black;
                        episode.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                        episode.FontSize = 13;
                        season.Items.Add (episode);
                    }
                    serie.Items.Add (season);
                }
                this.Dispatcher.Invoke (new Action (() => {
                    serieTreeView.Items.Add (serie);
                    if (serieTreeView.Items.Count == 1)
                        serie.IsSelected = true;
                }));
            }
            serieNumberTextBlock.Content = listOfSeries.Count;
        }
        private void FillSerieTreeView () {
            serieTreeView.Items.Clear ();
            SetSerieSliders ();
            foreach (Serie tempSerie in listOfSeries) {
                if (IsSerieAccepted (tempSerie) == false)
                    continue;
                TreeViewItem serie = new TreeViewItem () { Header = tempSerie.OrigName, DataContext = tempSerie };
                serie.Foreground = System.Windows.Media.Brushes.Black;
                serie.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                serie.FontSize = 16;
                foreach (SerieSeason tempSeason in tempSerie.Seasons) {
                    if (IsSeasonAccepted (tempSeason) == false)
                        continue;
                    TreeViewItem season = new TreeViewItem () { Header = tempSeason.Name, DataContext = tempSeason };
                    season.Foreground = System.Windows.Media.Brushes.Black;
                    season.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                    season.FontSize = 14;
                    foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                        if (IsEpisodeAccepted (tempEpisode) == false)
                            continue;
                        TreeViewItem episode = new TreeViewItem () { Header = tempEpisode.Name, DataContext = tempEpisode };
                        episode.Foreground = System.Windows.Media.Brushes.Black;
                        episode.FontFamily = new System.Windows.Media.FontFamily ("Estrangelo Edessa");
                        episode.FontSize = 13;
                        season.Items.Add (episode);
                    }
                    serie.Items.Add (season);
                }
                serieTreeView.Items.Add (serie);
                if (serieTreeView.Items.Count == 1)
                    serie.IsSelected = true;
            }
            serieNumberTextBlock.Content = serieTreeView.Items.Count.ToString ();
        }
        private void SetSerieSliders () {
            int maxSerieYear = 0;
            int minSerieYear = 9999;
            long maxSerieSize = 0;
            long minSerieSize = 999999999999999999;
            foreach (Serie tempSerie in listOfSeries) {
                if (tempSerie.Seasons.Count == 0)
                    continue;
                if (tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year > maxSerieYear)
                    maxSerieYear = tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
                if (tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year < minSerieYear)
                    minSerieYear = tempSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year;
                long size = FetchSerieShowingSize (tempSerie);
                if (size > maxSerieSize)
                    maxSerieSize = size;
                if (size < minSerieSize)
                    minSerieSize = size;
            }

            this.Dispatcher.Invoke (new Action (() => {
                yearRangeSliderSerie.Minimum = (double) minSerieYear;
                yearRangeSliderSerie.Maximum = (double) maxSerieYear;
                yearRangeSliderSerie.UpperValue = yearRangeSliderSerie.Maximum;
                yearRangeSliderSerie.LowerValue = yearRangeSliderSerie.Minimum;

                sizeRangeSliderSerie.Minimum = (double) minSerieSize;
                sizeRangeSliderSerie.Maximum = (double) maxSerieSize;
                sizeRangeSliderSerie.UpperValue = sizeRangeSliderSerie.Maximum;
                sizeRangeSliderSerie.LowerValue = sizeRangeSliderSerie.Minimum;
            }));

        }

        #region Index changed - set info

        private void serieTreeView_SelectedItemChanged (object sender, RoutedPropertyChangedEventArgs<object> e) {
            TreeViewItem selectedItem = serieTreeView.SelectedItem as TreeViewItem;
            if (selectedItem == null)
                return;

            try {
                Serie selectedSerie = (Serie) selectedItem.DataContext;
                serieCategoryShowing = SerieCategory.serie;
                _serieView.MoveCurrentTo (selectedSerie);
                _serieSeasonView = (CollectionView) CollectionViewSource.GetDefaultView (selectedSerie.Seasons);
                SetRatingCanvas (serieRatingCanvas, selectedSerie.Rating);
                SetSerieInfo (selectedSerie);
            }
            catch {
                try {
                    SerieSeason selectedSeason = (SerieSeason) selectedItem.DataContext;
                    serieCategoryShowing = SerieCategory.season;
                    _serieView.MoveCurrentTo (selectedSeason.ParentSerie);
                    _serieSeasonView.MoveCurrentTo (selectedSeason);
                    _serieEpisodeView = (CollectionView) CollectionViewSource.GetDefaultView (selectedSeason.Episodes);
                    SetRatingCanvas (serieRatingCanvas, selectedSeason.ParentSerie.Rating);
                    SetSerieSeasonInfo (selectedSeason);
                }
                catch {
                    try {
                        SerieEpisode selectedEpisode = (SerieEpisode) selectedItem.DataContext;
                        serieCategoryShowing = SerieCategory.episode;
                        _serieView.MoveCurrentTo (selectedEpisode.ParentSeason.ParentSerie);
                        _serieSeasonView.MoveCurrentTo (selectedEpisode.ParentSeason);
                        _serieEpisodeView.MoveCurrentTo (selectedEpisode);
                        SetRatingCanvas (serieRatingCanvas, selectedEpisode.ParentSeason.ParentSerie.Rating);
                        SetSerieEpisodeInfo (selectedEpisode);
                    }
                    catch {
                        serieCategoryShowing = SerieCategory.none;
                    }
                }
            }
            try {
                //RefreshSerieSubtitleTextBlock (selectedItem);
            }
            catch {
            }
        }
        private void SetSerieInfo (Serie currentSerie) {
            long totalSize = 0;
            long totalRuntime = 0;
            Dictionary<string, int> resolutionDic = new Dictionary<string, int> ();
            Dictionary<string, int> hddDic = new Dictionary<string, int> ();
            Dictionary<string, int> aspectRatioDic = new Dictionary<string, int> ();
            Dictionary<Audio, int> audioDic = new Dictionary<Audio, int> ();
            double avgBitrate;
            long totalBitrate = 0;
            int countEpisodes = 0;
            bool viewed = true;
            DateTime minDate = new DateTime (9999, 1, 1);
            DateTime maxDate = new DateTime (1, 1, 1);
            foreach (SerieSeason tempSeason in currentSerie.Seasons) {
                FetchSerieEpisodesInfo (tempSeason, ref totalSize, ref totalRuntime, ref resolutionDic, ref hddDic,
                    ref aspectRatioDic, ref audioDic, ref totalBitrate, ref countEpisodes, ref maxDate, ref minDate, ref viewed);
                if (viewed == false) 
                    currentSerie.IsViewed = false;                
            }
            avgBitrate = totalBitrate / countEpisodes;

            serieYearTB.Text = currentSerie.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year.ToString ();
            SetSizeRuntimeBitrateViewedYearSpanTBs (totalSize, totalRuntime, avgBitrate, minDate, maxDate, viewed);

            SetTBsFromDictionaries (hddDic, " HDDa", ref serieHddNumTB);
            SetTBsFromDictionaries (resolutionDic, " različite", ref serieResolutionTB);
            SetTBsFromDictionaries (aspectRatioDic, " AR-a", ref serieAspectRatioTB);
            SetAudioTBFromDictionary (audioDic);
            summaryTBSerie.Text = System.Web.HttpUtility.HtmlDecode (currentSerie.Summary);
            serieNameTB.Text = currentSerie.OrigName;

            RefreshSerieSubtitleTextBlock (currentSerie);
            var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
            if (IsSerieViewed (currentSerie))
                hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
            else
                hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
            serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());
            SetRatingCanvas (serieRatingCanvas, currentSerie.Rating);

        }
        private void SetSerieSeasonInfo (SerieSeason currentSeason) {
            Serie currentSerie = currentSeason.ParentSerie;
            long totalSize = 0;
            long totalRuntime = 0;
            Dictionary<string, int> resolutionDic = new Dictionary<string, int> ();
            Dictionary<string, int> hddDic = new Dictionary<string, int> ();
            Dictionary<string, int> aspectRatioDic = new Dictionary<string, int> ();
            Dictionary<Audio, int> audioDic = new Dictionary<Audio, int> ();
            double avgBitrate;
            long totalBitrate = 0;
            int countEpisodes = 0;
            bool viewed = true;
            DateTime minDate = new DateTime (9999, 1, 1);
            DateTime maxDate = new DateTime (1, 1, 1);
            FetchSerieEpisodesInfo (currentSeason, ref totalSize, ref totalRuntime, ref resolutionDic,
                ref hddDic, ref aspectRatioDic, ref audioDic, ref totalBitrate, ref countEpisodes, ref maxDate, ref minDate, ref viewed);
            if (viewed == false) {
                //((SerieSeason) _serieSeasonView.CurrentItem).IsViewed = false;
            }
            avgBitrate = totalBitrate / countEpisodes;

            SetSizeRuntimeBitrateViewedYearSpanTBs (totalSize, totalRuntime, avgBitrate, minDate, maxDate, viewed);

            SetTBsFromDictionaries (hddDic, " HDDa", ref serieHddNumTB);
            SetTBsFromDictionaries (resolutionDic, " različite", ref serieResolutionTB);
            SetTBsFromDictionaries (aspectRatioDic, " AR-a", ref serieAspectRatioTB);
            SetAudioTBFromDictionary (audioDic);
            summaryTBSerie.Text = currentSeason.ParentSerie.Summary;

            var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
            if (IsSeasonViewed (currentSeason))
                hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
            else
                hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
            serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());
            serieNameTB.Text = currentSeason.ParentSerie.Name + ": " + currentSeason.Name;
            RefreshSerieSubtitleTextBlock (currentSeason);
        }
        private void SetSerieEpisodeInfo (SerieEpisode currentEp) {
            Converters.LanguageToStringConverter subConverter = new Converters.LanguageToStringConverter ();
            Converters.AudioListToStringConverter audioConverter = new Converters.AudioListToStringConverter ();
            this.SerieAudioTB.Text = audioConverter.Convert (currentEp.AudioList, typeof (string), null, null).ToString ();
            this.serieSubtitleLanguageTB.Text = subConverter.Convert (currentEp.SubtitleLanguageList, typeof (string), 50, null).ToString ();

            SetSizeRuntimeBitrateViewedYearSpanTBs (currentEp.Size, currentEp.Runtime, currentEp.Bitrate, new DateTime (1, 1, 1), new DateTime (1, 1, 1), currentEp.IsViewed);
            Converters.ResolutionToStringConverter rezConverter = new Converters.ResolutionToStringConverter ();
            serieResolutionTB.Text = rezConverter.Convert (currentEp.Width, typeof (string), null, null).ToString ();
            serieAspectRatioTB.Text = currentEp.AspectRatio;
            serieHddNumTB.Text = currentEp.Hdd.ToString ();
            summaryTBSerie.Text = System.Web.HttpUtility.HtmlDecode (currentEp.Summary);
            serieYearTB.Text = currentEp.AirDate.ToShortDateString ();
            serieNameTB.Text = currentEp.OrigName;
            var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
            if (currentEp.IsViewed)
                hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
            else
                hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
            serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());

        }

        private void FetchSerieEpisodesInfo (SerieSeason season, ref long totalSize, ref long totalRuntime, ref Dictionary<string, int> resolutionDic, ref Dictionary<string, int> hddDic,
            ref Dictionary<string, int> aspectRatioDic, ref Dictionary<Audio, int> audioDic, ref long totalBitrate, ref int countEpisodes, ref DateTime maxDate, ref DateTime minDate, ref bool viewed) {

            foreach (SerieEpisode tempEpisode in season.Episodes) {
                if (tempEpisode.AirDate > maxDate)
                    maxDate = tempEpisode.AirDate;
                if (tempEpisode.AirDate < minDate)
                    minDate = tempEpisode.AirDate;
                countEpisodes++;
                totalSize += tempEpisode.Size;
                totalRuntime += tempEpisode.Runtime;
                totalBitrate += tempEpisode.Bitrate;
                if (tempEpisode.IsViewed == false)
                    viewed = false;
                Converters.ResolutionToStringConverter converter = new Converters.ResolutionToStringConverter ();
                string resolution = converter.Convert (tempEpisode.Width, typeof (string), null, null).ToString ();

                FillStringIntDictionary (ref resolutionDic, resolution);
                FillStringIntDictionary (ref hddDic, tempEpisode.Hdd.ToString ());
                FillStringIntDictionary (ref aspectRatioDic, tempEpisode.AspectRatio);

                foreach (Audio tempAudio in tempEpisode.AudioList) {
                    if (audioDic.ContainsKey (tempAudio))
                        audioDic[tempAudio]++;
                    else
                        audioDic.Add (tempAudio, 1);
                }
            }
        }

        #endregion

        private void RefreshSerieSubtitleTextBlock (object obj) {
            Converters.SerieLanguagesConverterToString converter = new Converters.SerieLanguagesConverterToString ();
            object parameter = "Subtitle";
            this.serieSubtitleLanguageTB.Text = converter.Convert (obj, typeof (string), parameter, null).ToString ();
            //MessageBox.Show (converter.Convert (obj, typeof (string), parameter, null).ToString ());
        }

        private long FetchSerieShowingSize (Serie tempSerie) {
            long size = 0;
            foreach (SerieSeason tempSeason in tempSerie.Seasons)
                this.Dispatcher.Invoke (new Action (() => {
                    if (IsSeasonAccepted (tempSeason))
                        size += FetchSeasonSize (tempSeason);
                }));
            return size;
        }
        private long FetchSeasonSize (SerieSeason tempSeason) {
            long size = 0;
            foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                if (IsEpisodeAccepted (tempEpisode))
                    size += tempEpisode.Size;
            return size;
        }

        private void serieGoToIMDbButton_Click (object sender, RoutedEventArgs e) {
            try {
                if (serieCategoryShowing == SerieCategory.serie)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as Serie).InternetLink);
                else if (serieCategoryShowing == SerieCategory.season)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieSeason).InternetLink);
                else if (serieCategoryShowing == SerieCategory.episode)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieEpisode).InternetLink);
                else
                    Xceed.Wpf.Toolkit.MessageBox.Show ("Ništa nije selektirano");
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Greška!");
            }
        }
        private void serieGoToTrailerButton_Click (object sender, RoutedEventArgs e) {
            try {
                if (serieCategoryShowing == SerieCategory.serie)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as Serie).TrailerLink);
                else if (serieCategoryShowing == SerieCategory.season)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieSeason).TrailerLink);
                else if (serieCategoryShowing == SerieCategory.episode)
                    System.Diagnostics.Process.Start (((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieEpisode).TrailerLink);
            }
            catch {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Greška!");
            }
        }       

        private void SetSizeRuntimeBitrateViewedYearSpanTBs (long totalSize, long totalRuntime, double avgBitrate, DateTime minDate, DateTime maxDate, bool viewed) {
            Converters.SizeConverter sizeConverter = new Converters.SizeConverter ();
            serieSizeTB.Text = sizeConverter.Convert (totalSize, typeof (string), null, null).ToString ();

            Converters.RuntimeConverter runtimeConverter = new Converters.RuntimeConverter ();
            serieRuntimeTB.Text = runtimeConverter.Convert (totalRuntime, typeof (string), null, null).ToString ();

            Converters.BitrateConverter bitrateConverter = new Converters.BitrateConverter ();
            serieBitrateTB.Text = "Avg: " + bitrateConverter.Convert (avgBitrate, typeof (string), null, null).ToString ();

            if (serieCategoryShowing == SerieCategory.serie || serieCategoryShowing == SerieCategory.season)
                serieYearTB.Text = minDate.Year.ToString () + " - " + maxDate.Year.ToString ();

            if (viewed) {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly ();
                Stream myStream = myAssembly.GetManifestResourceStream ("Video_katalog.Resources.greenCheck50p.jpg");
                Stream s = this.GetType ().Assembly.GetManifestResourceStream ("Video_katalog.greenCheck50p.jpg");
                BitmapImage image = new BitmapImage ();
                image.StreamSource = myStream;
                serieIsViewedImage.Source = image;
            }
            else {
                System.Reflection.Assembly myAssembly = System.Reflection.Assembly.GetExecutingAssembly ();
                string[] names = myAssembly.GetManifestResourceNames ();
                Stream myStream = myAssembly.GetManifestResourceStream ("Video_katalog.greenCheck50p.jpg");
                Stream s = this.GetType ().Assembly.GetManifestResourceStream ("Video_katalog.redCross50p.jpg");
                BitmapImage image = new BitmapImage ();
                image.StreamSource = myStream;
                //serieIsViewedImage.Source = new ImageSource ("Resources/greenCheck50p.png");
            }
        }
        private void SetAudioTBFromDictionary (Dictionary<Audio, int> audioDic) {
            if (audioDic.Count == 1) {
                this.SerieAudioTB.Text = string.Format ("{0} [{1}]", audioDic.ElementAt (0).Key, audioDic.ElementAt (0).Value);
                this.SerieAudioTB.ToolTip = null;
            }
            else {
                string tooltip = "";
                foreach (KeyValuePair<Audio, int> pair in audioDic) {
                    tooltip += string.Format ("{0}: {1}\r\n", pair.Value.ToString("000"), pair.Key);
                }
                tooltip = tooltip.Substring (0, tooltip.Length - 2);
                this.SerieAudioTB.Text = audioDic.Count () + " razlicita";
                this.SerieAudioTB.ToolTip = tooltip;
            }
        }
        private void SetTBsFromDictionaries (Dictionary<string, int> dict, string extension, ref TextBlock textBlock) {
            if (dict.Count == 1) {
                textBlock.Text = dict.ElementAt (0).Key;
                textBlock.ToolTip = null;
            }
            else {
                textBlock.Text = dict.Count + extension;
                string tooltip = "";
                foreach (KeyValuePair<string, int> pair in dict) {
                    tooltip += string.Format ("{0}: {1}\r\n", pair.Value.ToString("000"), pair.Key);
                }
                tooltip = tooltip.Substring (0, tooltip.Length - 2);
                textBlock.ToolTip = tooltip;
            }
        }
        private void FillStringIntDictionary (ref Dictionary<string, int> dict, string str) {
            if (dict.ContainsKey (str))
                dict[str]++;
            else
                dict.Add (str, 1);
        }

        Thread THSetEpisodesWatched;
        private void serieIsViewedImage_MouseLeftButtonUp (object sender, MouseButtonEventArgs e) {
            TreeViewItem item = serieTreeView.SelectedItem as TreeViewItem;
            if (serieCategoryShowing == SerieCategory.serie) {
                Serie tempSerie = item.DataContext as Serie;

                //Set image
                var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
                if (IsSerieViewed (tempSerie))
                    hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
                else
                    hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
                serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());

                busyIncidator.IsBusy = true;
                busyIncidator.BusyContent = "Updateam";

                allThreads.Add (THSetEpisodesWatched);
                ThreadStart starter = delegate {
                    SetSerieEpisodesViewedStateAccordingly (tempSerie);
                };
                THSetEpisodesWatched = new Thread (starter);
                THSetEpisodesWatched.IsBackground = true;
                THSetEpisodesWatched.Start ();
            }
            else if (serieCategoryShowing == SerieCategory.season) {
                SerieSeason tempSeason = item.DataContext as SerieSeason;

                //Set image
                var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
                if (IsSeasonViewed (tempSeason))
                    hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
                else
                    hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
                serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());

                busyIncidator.BusyContent = "Updateam";

                allThreads.Add (THSetEpisodesWatched);
                ThreadStart starter = delegate { SetSeasonEpisodesViewedStateAccordingly (tempSeason); };
                THSetEpisodesWatched = new Thread (starter);
                THSetEpisodesWatched.IsBackground = true;
                THSetEpisodesWatched.Start ();
            }
            else if (serieCategoryShowing == SerieCategory.episode) {
                SerieEpisode tempEpisode = item.DataContext as SerieEpisode;
                //Set image
                var hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap ();
                if (tempEpisode.IsViewed)
                    hbitmap = VideoKatalog.View.Properties.Resources.redCross50p.GetHbitmap ();
                else
                    hbitmap = VideoKatalog.View.Properties.Resources.greenCheck50p.GetHbitmap ();
                serieIsViewedImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ());

                tempEpisode.IsViewed = !tempEpisode.IsViewed;
                DatabaseManager.UpdateSerieEpisodeChangeOnlyIsViewed (tempEpisode);
            }
        }
        bool IsSerieViewed (Serie tempSerie) {
            foreach (SerieSeason tempSeason in tempSerie.Seasons)
                if (IsSeasonViewed (tempSeason) == false)
                    return false;
            return true;
        }
        bool IsSeasonViewed (SerieSeason tempSeason) {
            foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                if (tempEpisode.IsViewed == false) {
                    return false;
                }
            return true;
        }
        void SetSeasonEpisodesViewedStateAccordingly (SerieSeason tempSeason) {
            bool Viewed = IsSeasonViewed (tempSeason);
            foreach (SerieEpisode tempEpisode in tempSeason.Episodes) {
                if (Viewed)
                    tempEpisode.IsViewed = false;
                else
                    tempEpisode.IsViewed = true;
                DatabaseManager.UpdateSerieEpisodeChangeOnlyIsViewed (tempEpisode);
            }
            this.Dispatcher.Invoke (new Action (() => {
                busyIncidator.IsBusy = false;
            }));
        }
        void SetSerieEpisodesViewedStateAccordingly (Serie tempSerie) {
            bool viewed = IsSerieViewed (tempSerie);
            foreach (SerieSeason tempseason in tempSerie.Seasons)
                foreach (SerieEpisode tempEpisode in tempseason.Episodes) {
                    if (viewed)
                        tempEpisode.IsViewed = false;
                    else
                        tempEpisode.IsViewed = true;
                    DatabaseManager.UpdateSerieEpisodeChangeOnlyIsViewed (tempEpisode);
                }
            this.Dispatcher.Invoke (new Action (() => {
                busyIncidator.IsBusy = false;
            }));
        }

        #region Sort / Filter

        //Filter
        private void serieSearchGridClose_Click (object sender, System.Windows.RoutedEventArgs e) {
            busyIncidator.IsBusy = true;
            busyIncidator.BusyContent = "Filtriranje";
            playCloseSound (null, null);

            ThreadStart starter = delegate {
                SetSerieSliders ();
                for (long i = 0 ; i < 100000000 ; i++) ;
                this.Dispatcher.Invoke (new Action (() => {
                    FillSerieTreeView ();
                    serieTreeView.Focus ();
                    busyIncidator.IsBusy = false;
                }));

            };
            Thread filterSeriesThread = new Thread (starter);
            filterSeriesThread.IsBackground = true;
            filterSeriesThread.Start ();

        }
        private bool IsSerieAccepted (Serie serieToCheck) {

            #region Naziv

            if (serieToCheck.Name.ToLower ().Contains (searchSerieName.Text.ToLower ()) == false && serieToCheck.OrigName.ToLower ().Contains (searchSerieName.Text.ToLower ()) == false)
                return false;

            #endregion

            #region Žanr

            bool foundOneGenre = false;
            foreach (Genre tempGenre in serieToCheck.Genres) {
                if (tempGenre.Name == "Akcija" && actionCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Avantura" && adventureCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Animacija" && animationCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Biografija" && biographyCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Komedija" && comedyCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Krimi" && crimeCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Dokumentarni" && documentaryCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Drama" && dramaCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Obiteljski" && familyCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Fantastika" && fantasyCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Crno-bijeli" && filmNoirCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Povijesni" && historyCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Horor" && horrorCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Muzički" && musicCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Pjevan" && musicalCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Misterija" && misteryCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Romantika" && romanceCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Znanstvena fantastika" && sciFiCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Sportski" && sportCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Triler" && thrillerCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Ratni" && warCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
                if (tempGenre.Name == "Vestern" && westernCheckBox2.IsChecked == true) {
                    foundOneGenre = true;
                    break;
                }
            }
            if (foundOneGenre == false)
                return false;

            #endregion

            #region Glumci / Redatelji

            //Glumci
            if (searchSerieCast.Text.Length > 0) {
                bool foundActor = false;
                foreach (Person actor in serieToCheck.Actors)
                    if (actor.Name.ToLower ().Contains (searchSerieCast.Text.ToLower ()))
                        foundActor = true;
                if (foundActor == false)
                    return false;
            }

            //Redatelji
            if (searchSerieDirector.Text.Length > 0) {
                bool foundDirector = false;
                foreach (Person director in serieToCheck.Directors)
                    if (director.Name.ToLower ().Contains (searchSerieDirector.Text.ToLower ()))
                        foundDirector = true;
                if (foundDirector == false)
                    return false;
            }

            #endregion

            #region Ocjena & Godina & Veličina

            if ((double) serieToCheck.Rating > ratingRangeSliderSerie.UpperValue || (double) serieToCheck.Rating < ratingRangeSliderSerie.LowerValue)
                return false;
            if ((double) serieToCheck.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year > yearRangeSliderSerie.UpperValue ||
                (double) serieToCheck.Seasons.ElementAt (0).Episodes.ElementAt (0).AirDate.Year < yearRangeSliderSerie.LowerValue)
                return false;
            long totalSize = FetchSerieShowingSize (serieToCheck);
            if ((double) totalSize > sizeRangeSliderSerie.UpperValue || (double) totalSize < sizeRangeSliderSerie.LowerValue)
                return false;

            #endregion

            #region Rezolucija & prijevod & Pogledano

            Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter ();
            int countAcceptedSeasons = 0;
            foreach (SerieSeason tempSeason in serieToCheck.Seasons) {
                if (IsSeasonAccepted (tempSeason)) {
                    countAcceptedSeasons++;
                    break;
                }
            }
            if (countAcceptedSeasons == 0)
                return false;

            #endregion

            return true;
        }
        private bool IsSeasonAccepted (SerieSeason seasonToCheck) {
            foreach (SerieEpisode tempEpisode in seasonToCheck.Episodes)
                if (IsEpisodeAccepted (tempEpisode))
                    return true;
            return false;
        }
        private bool IsEpisodeAccepted (SerieEpisode episodeToCheck) {

            #region Rezolucija

            Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter ();
            string resolution = resolutionConverter.Convert (episodeToCheck.Width, typeof (string), null, null).ToString ();
            if (resolution == "SD" && serieSearchRezSD.IsChecked == false)
                return false;
            if (resolution == "720p" && serieSearchRez720p.IsChecked == false)
                return false;
            if (resolution == "1080p" && serieSearchRez1080p.IsChecked == false)
                return false;

            #endregion

            #region Prijevod

            Language cro = new Language ();
            cro.Name = "Croatian";
            Language eng = new Language ();
            eng.Name = "English";

            if (serieSearchSubCro.IsChecked == true && episodeToCheck.SubtitleLanguageList.Contains (cro, new LanguageComparerByName ()) == false)
                return false;
            if (serieSearchSubEng.IsChecked == true && episodeToCheck.SubtitleLanguageList.Contains (eng, new LanguageComparerByName ()) == false)
                return false;
            if (serieSearchSubNoCro.IsChecked == true && episodeToCheck.SubtitleLanguageList.Contains (cro, new LanguageComparerByName ()) == true)
                return false;

            #endregion

            #region Pogledano

            if (episodeToCheck.IsViewed == false && serieSearchShowNotViewed.IsChecked == false)
                return false;
            if (episodeToCheck.IsViewed == true && serieSearchShowViewed.IsChecked == false)
                return false;

            #endregion

            #region HDD

            if (showingSeriesFromHDDs.Contains (episodeToCheck.Hdd.Name) == false)
                return false;

            #endregion

            return true;
        }

        //Sort
        private void SortSeries (object sender, MouseButtonEventArgs e) {
            busyIncidator.IsBusy = true;
            busyIncidator.BusyContent = "Sortiranje serija";
            TextBlock clickedTB = sender as TextBlock;
            ThreadStart starter = delegate {
                string TBName = "";
                this.Dispatcher.Invoke (new Action (() => {
                    TBName = clickedTB.Name;
                }));
                DateTime now = DateTime.Now;
                while ((DateTime.Now - now).Milliseconds < 300) ;
                if (TBName.ToLower ().Contains ("name")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        SortSeries ("name");
                    }));
                }
                else if (TBName.ToLower ().Contains ("rating")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        SortSeries ("rating");
                    }));
                }
                else if (TBName.ToLower ().Contains ("year")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        SortSeries ("year");
                    }));
                }
                else if (TBName.ToLower ().Contains ("size")) {
                    this.Dispatcher.Invoke (new Action (() => {
                        SortSeries ("size");
                    }));
                }
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                }));
            };
            Thread sortSeriesThread = new Thread (starter);
            sortSeriesThread.IsBackground = true;
            sortSeriesThread.Start ();
        }
        private void SortSeries (string byProperty) {
            ObservableCollection<Serie> sortedList = new ObservableCollection<Serie> ();
            foreach (Serie tempSerie in listOfSeries)
                sortedList.Add (tempSerie);
            for (int i = 0 ; i < sortedList.Count ; i++) {
                for (int j = i + 1 ; j < sortedList.Count ; j++) {
                    if (byProperty == "name") {
                        for (int k = 0 ; k < sortedList[i].Name.Length ; k++) {
                            if (sortedList[i].Name[k] > sortedList[j].Name[k]) {
                                ReplaceIndexes (sortedList, i, j);
                                break;
                            }
                            else
                                break;
                        }
                    }
                    else if (byProperty == "year") {
                        if (sortedList[i].Seasons[0].Episodes[0].AirDate.Year > sortedList[j].Seasons[0].Episodes[0].AirDate.Year)
                            ReplaceIndexes (sortedList, i, j);
                    }
                    else if (byProperty == "size") {
                        if (FetchSerieShowingSize (sortedList[i]) > FetchSerieShowingSize (sortedList[j]))
                            ReplaceIndexes (sortedList, i, j);
                    }
                    else if (byProperty == "rating") {
                        if (sortedList[i].Rating > sortedList[j].Rating)
                            ReplaceIndexes (sortedList, i, j);
                    }
                }
            }
            listOfSeries.Clear ();
            foreach (Serie tempSerie in sortedList)
                listOfSeries.Add (tempSerie);
            if (serieSortDescending.IsChecked == true)
                ChangeSerieSortingDirections (null, null);
            _serieView = (CollectionView) CollectionViewSource.GetDefaultView (listOfSeries);
            FillSerieTreeView ();
        }
        private void ReplaceIndexes (ObservableCollection<Serie> list, int i, int j) {
            Serie tempSerie = list.ElementAt (i);
            list[i] = list.ElementAt (j);
            list[j] = tempSerie;
        }
        private void ChangeSerieSortingDirections (object sender, RoutedEventArgs e) {
            //listOfSeries =  listOfSeries.Reverse () as ObservableCollection<Serie>;
            ObservableCollection<Serie> sortedList = new ObservableCollection<Serie> ();
            foreach (Serie tempSerie in listOfSeries)
                sortedList.Add (tempSerie);
            listOfSeries.Clear ();
            for (int i = sortedList.Count - 1 ; i >= 0 ; i--)
                listOfSeries.Add (sortedList[i]);

            if (sender == null)
                return;
            _serieView = (CollectionView) CollectionViewSource.GetDefaultView (listOfSeries);
            try {
                FillSerieTreeView ();
            }
            catch {
            }
        }

        #region Genre

        bool serieGenreGridExpanded = false;
        private void openCloseSerieGenreGrid_Click (object sender, System.Windows.RoutedEventArgs e) {
            if (serieGenreGridExpanded) {
                var stb = (Storyboard) TryFindResource ("serieGenreGridCollapse");
                //showingSelectHddSize = false;
                if (stb != null) {
                    stb.Begin ();
                }
                serieGenreGridExpanded = false;
            }
            else {
                var stb = (Storyboard) TryFindResource ("serieGenreGridExpand");
                //showingSelectHddSize = false;
                if (stb != null) {
                    stb.Begin ();
                }
                serieGenreGridExpanded = true;
            }
        }
        private void serieGenreAll_Click (object sender, System.Windows.RoutedEventArgs e) {
            // TODO: Add event handler implementation here.
        }
        private void serieGenreNone_Click (object sender, System.Windows.RoutedEventArgs e) {
            // TODO: Add event handler implementation here.
        }

        #endregion

        private void serieSearchHddChecked (object sender, System.Windows.RoutedEventArgs e) {
            CheckBox checkedHDD = sender as CheckBox;
            if (checkedHDD.IsChecked == true) {
                showingSeriesFromHDDs.Add (checkedHDD.Content.ToString ());
            }
            else {
                List<string> listToRemove = new List<string> ();
                foreach (string tempHDD in showingSeriesFromHDDs)
                    if (tempHDD == checkedHDD.Content.ToString ())
                        listToRemove.Add (tempHDD);
                foreach (string toRemove in listToRemove)
                    showingSeriesFromHDDs.Remove (toRemove);
            }
        }

        #endregion

        #endregion

        #region HOME-VIDEO TAB

        #region Global data
        
        //private ObservableCollection<Person> homeVideoPersonList = new ObservableCollection<Person> ();
        private ObservableCollection<HomeVideo> listOfHomeVideos = new ObservableCollection<HomeVideo> ();
        CollectionView _homeVideoView;

        #endregion

        #region Add / Edi / Remove

        private void newHomeVideoMenu_Click (object sender, RoutedEventArgs e) {
            HomeVideoForm newHV = new HomeVideoForm (WishOrRegular.regular, personList, cameraList, homeVideoCatList);
            newHV.Owner = this;
            newHV.ShowDialog ();
            if (newHV.accepted) {
                foreach (HDD tempHDD in newHV.insertedHDDs)
                    hddList.Add (tempHDD);
                AddHomeVideo (newHV.currHomeVideo);
            }
        }
        private void AddHomeVideo (HomeVideo insertHomeVideo) {
            try {
                SetHDDPointerOrInsert (insertHomeVideo.Hdd);
                SetAudioPointersOrInsert (insertHomeVideo.AudioList);
                SetPersonPointersOrInsert (insertHomeVideo.Camermans);
                SetPersonPointersOrInsert (insertHomeVideo.PersonsInVideo);

                DatabaseManager.InsertHomeVideo (insertHomeVideo);
                listOfHomeVideos.Add (insertHomeVideo);
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
            }
        }
        private void editHomeVideoMenu_Click (object sender, RoutedEventArgs e) {
            HomeVideo selected = (HomeVideo) _homeVideoView.CurrentItem;
            if (selected == null)
                return;
            HomeVideoForm editHV = new HomeVideoForm (selected, personList, cameraList, homeVideoCatList);
            editHV.Owner = this;
            int viewIndex = _homeVideoView.CurrentPosition;
            int listIndex = listOfHomeVideos.IndexOf (selected);
            try {
                Clipboard.SetImage (new BitmapImage (new Uri (HardcodedStrings.homeVideoPosterFolder + selected.Name + ".jpg")));
            }
            catch {
                Clipboard.Clear ();//new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            }
            listOfHomeVideos.RemoveAt (listIndex);

            editHV.ShowDialog ();

            if (editHV.accepted) {
                foreach (HDD tempHDD in editHV.insertedHDDs)
                    hddList.Add (tempHDD);
                HomeVideo editedHV = editHV.currHomeVideo;
                UpdateHomeVideo (editedHV);
                listOfHomeVideos.Insert (listIndex, editedHV);
            }
            else {
                listOfHomeVideos.Insert (listIndex, selected);
            }
            _homeVideoView.MoveCurrentToPosition (viewIndex);
        }
        private void UpdateHomeVideo (HomeVideo homeVideo) {
            try {
                SetHDDPointerOrInsert (homeVideo.Hdd);
                SetAudioPointersOrInsert (homeVideo.AudioList);
                SetPersonPointersOrInsert (homeVideo.Camermans);
                SetPersonPointersOrInsert (homeVideo.PersonsInVideo);

                DatabaseManager.UpdateHomeVideo (homeVideo);
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
            }
        }
        private void deleteHomeVideoMenu_Click_1 (object sender, RoutedEventArgs e) {
            try {
                HomeVideo selected = (HomeVideo) _homeVideoView.CurrentItem;
                if (selected == null)
                    return;
                string confirmMessage = "Brisanje kućnog videa: " + selected.Name.ToUpper () + "\r\nJeste li sigurni?";
                if (Xceed.Wpf.Toolkit.MessageBox.Show (confirmMessage, "Potvrda brisanja", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes) {
                    DatabaseManager.DeleteHomeVideo (selected);
                    int viewIndex = _homeVideoView.CurrentPosition;
                    int listIndex = listOfHomeVideos.IndexOf (selected);
                    _homeVideoView.MoveCurrentToPrevious ();
                    _homeVideoView.MoveCurrentToNext ();
                    listOfHomeVideos.RemoveAt (listIndex);
                    imagesToDelete.Add (HardcodedStrings.homeVideoPosterFolder + selected.Name + ".jpg");
                    //movieNumberTextBlock.Text = _movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();// listOfMovies.Count ().ToString ();   //_movieView.SourceCollection.Cast<Movie> ().Count ().ToString ();
                }
            }
            catch (Exception exc) {
                MessageBox.Show (exc.Message);
            }
        }

        #endregion

        #region Sort / Filter

        #endregion

        #endregion

        #region Set pointers

        void SetHDDPointerOrInsert (HDD hdd) {
            bool found = false;
            foreach (HDD tempHDD in hddList) {
                if (tempHDD.ID == hdd.ID) {
                    hdd = tempHDD;
                    found = true;
                    break;
                }
            }
            if (found) {
                return;
            }
            else {
                DatabaseManager.InsertHDD (hdd);
                //DatabaseManagerMySql.InsertHDD (hdd);
                this.Dispatcher.Invoke (new Action (() => {
                    hddList.Add (hdd);
                }));
            }
        }
        void SetLanguagePointersOrInsert (ObservableCollection<Language> languages) {
            //prodji kroz sve jezike filma, te provjeri da li postoje u listi jezika (ako postoje, onda postoje i u bazi)            
            for (int i = 0 ; i < languages.Count ; i++) {
                bool found = false;

                //za svaki jezik filma, ako vec postoji, postavi pokazivac na postojeci jezik (da se ne stvaraju dodatni isti jezici)
                foreach (Language tempLang in this.languageList) {
                    if (tempLang.Name == languages[i].Name) {
                        found = true;
                        this.Dispatcher.Invoke (new Action (() => {
                            languages[i] = tempLang;
                        }));
                        break;
                    }
                }

                //ako jezik ne postoji, unesi ga u bazu
                if (found == false) {
                    DatabaseManager.InsertLanguage (languages[i]);
                    //DatabaseManagerMySql.InsertLanguage (languages[i]);
                    this.Dispatcher.Invoke (new Action (() => {
                        languageList.Add (languages[i]);
                    }));
                }
            }
        }
        void SetPersonPointersOrInsert (ObservableCollection<Person> persons) {
            //prodji kroz sve osobe u filmu, te provjeri da li postoje u listi osoba (ako postoje, onda postoje i u bazi)
            for (int i = 0 ; i < persons.Count ; i++) {
                bool found = false;

                //za svaku osobu, ako vec postoji, postavi pokazivac na postojecu osobu (da se ne stvaraju dodatne iste osobe u memoriji)
                foreach (Person tempPerson in personList) {
                    if (tempPerson.Name == persons[i].Name) {
                        found = true;
                        this.Dispatcher.Invoke (new Action (() => {
                            persons[i] = tempPerson;
                        }));
                    }
                }

                //ako osoba ne postoji, unesi ju u bazu podataka i dohvati ID osobe
                if (found == false) {
                    DatabaseManager.InsertPerson (persons[i]);
                    //DatabaseManagerMySql.InsertPerson (persons[i]);
                    this.Dispatcher.Invoke (new Action (() => {
                        personList.Add (persons[i]);
                    }));
                }
            }
        }
        void SetGenrePointers (ObservableCollection<Genre> genres) {
            //prodji kroz sve zanrove filmova i postavi pokazivac za svaki na vec postojeci zanr u glavnoj listi zanrova
            for (int i = 0 ; i < genres.Count ; i++) {
                foreach (Genre tempGenre in genreList) {
                    if (tempGenre.Name == genres[i].Name) {
                        this.Dispatcher.Invoke (new Action (() => {
                            genres[i] = tempGenre;
                        }));
                    }
                }
            }
        }
        void SetAudioPointersOrInsert (ObservableCollection<Audio> audios) {
            //prodji kroz sve audie filma, te provjeri da li postoje u audio listi (ako postoje, onda postoje i u bazi)
            for (int i = 0 ; i < audios.Count ; i++) {
                bool foundAudio = false;

                //za svaki audio, ako vec postoji, postavi pokazivac na postojeci audio
                foreach (Audio tempAudio in audioList) {
                    if (tempAudio.Format == audios.ElementAt (i).Format &&
                        tempAudio.Channels == audios.ElementAt (i).Channels &&
                        tempAudio.Language.Name == audios.ElementAt (i).Language.Name) {
                        foundAudio = true;
                        this.Dispatcher.Invoke (new Action (() => {
                            audios[i] = tempAudio;
                            audios[i].ID = tempAudio.ID;
                        }));
                        break;
                    }
                }

                if (foundAudio == false) {
                    //ako audio ne postoji, provjeri da li postoji jezik audia, ako postoji postavi pointer na postojeci, ako ne postoji ubaci u bazu
                    bool foundLanguage = false;
                    foreach (Language tempLang in languageList) {
                        if (tempLang.Name == audios.ElementAt (i).Language.Name) {
                            foundLanguage = true;
                            this.Dispatcher.Invoke (new Action (() => {
                                audios.ElementAt (i).Language = tempLang;
                            }));
                        }
                    }
                    if (foundLanguage == false) {
                        DatabaseManager.InsertLanguage (audios.ElementAt (i).Language);
                        DatabaseManagerMySql.InsertLanguage (audios.ElementAt (i).Language);
                        this.Dispatcher.Invoke (new Action (() => {
                            languageList.Add (audios.ElementAt (i).Language);
                        }));
                    }

                    //ako Audio ne postoji, unesi ga u bazu podataka (i u globalnu listu) i dohvati ID tog audia
                    if (foundAudio == false) {
                        DatabaseManager.InsertAudio (audios.ElementAt (i));
                        //DatabaseManagerMySql.InsertAudio (audios.ElementAt (i));
                        this.Dispatcher.Invoke (new Action (() => {
                            audioList.Add (audios.ElementAt (i));
                        }));
                    }
                }
            }
        }

        #endregion

        void SetBusyStateAndContent (string content, bool state) {
            this.Dispatcher.Invoke (new Action (() => {
                busyIncidator.BusyContent = content;
                busyIncidator.IsBusy = state;
            }));
        }
        void SetStatusBarText (string content) {
            this.Dispatcher.Invoke (new Action (() => {
                statusBarTextBlock.Text = content;
            }));
        }
        
        private void Window_Closed (object sender, EventArgs e) {
            foreach (string imagePath in imagesToDelete) {
                try {
                    File.Delete (imagePath);
                }
                catch {
                }
            }
            foreach (Thread tempThread in allThreads) 
                try {
                    tempThread.Abort ();
                }
                catch {
                }
        }

        private void refreshCast_Click (object sender, System.Windows.RoutedEventArgs e) {
            //
            //for (int i = 0 ; i < movieListBox.Items.Count ; i++) {
            //    Movie editMovie = movieListBox.Items[i] as Movie;

            //    MovieForm editMovieForm = new MovieForm (editMovie, "cast");
            //    editMovieForm.Owner = this;
            //    int viewIndex = _movieView.CurrentPosition;
            //    int listIndex = listOfMovies.IndexOf (editMovie);

            //    try {
            //        Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () +
            //            @"/Images/Movie/" + editMovie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
            //    }
            //    catch {
            //        Clipboard.Clear ();//new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
            //    }
            //    listOfMovies.RemoveAt (listIndex);
            //    editMovieForm.ShowDialog ();
            //    editMovie = editMovieForm.currMovie;
            //    listOfMovies.Insert (i, editMovie);

            ThreadStart starter = delegate {
                StartRefresh ();
            };
            THAddRemoveMovie = new Thread (starter);
            THAddRemoveMovie.IsBackground = true;
            THAddRemoveMovie.Start ();

            //if (count++ > 2)
            //   break;
            //}           
        }
        private void StartRefresh () {
            //Movie bla = movieListBox.Items[648] as Movie;
            //MessageBox.Show (bla.Name);
            return;
            for (int i = 721 ; i < movieListBox.Items.Count ; i++) {
                Movie editMovie = movieListBox.Items[i] as Movie;

                //this.Dispatcher.Invoke (new Action (() => {
                //    MovieForm editMovieForm = new MovieForm (editMovie, "justUpdate");
                //    editMovieForm.Owner = this;
                //    int viewIndex = _movieView.CurrentPosition;
                //    int listIndex = listOfMovies.IndexOf (editMovie);

                //    try {
                //        Clipboard.SetImage (new BitmapImage (new Uri (Directory.GetCurrentDirectory () +
                //            @"/Images/Movie/" + editMovie.Name.Replace ("\\", "").Replace ("/", "").Replace (":", "-").Replace ("?", "").Replace ("<", "").Replace (">", "").Replace ("|", "") + ".jpg")));
                //    }
                //    catch {
                //        Clipboard.Clear ();//new BitmapImage (new Uri (Directory.GetCurrentDirectory () + @"/Images/blank.jpg")));
                //    }
                //    listOfMovies.RemoveAt (listIndex);
                //    editMovieForm.ShowDialog ();
                //    editMovie = editMovieForm.currMovie;
                //    listOfMovies.Insert (i, editMovie);
                //}));                
                
                UpdateMovie (editMovie);  
            }           

        }
        private void syncLocalAndOnlineDB_Click (object sender, System.Windows.RoutedEventArgs e) {
            ThreadLoadData = new Thread (new ThreadStart (SyncLocalOnlineDatabase));
            ThreadLoadData.IsBackground = true;
            ThreadLoadData.Start ();
        }
        private void SyncLocalOnlineDatabase () {            
            SetBusyStateAndContent ("Sinkroniziram online i lokalnu bazu", true);

            try {               

                #region Persons

                //SetStatusBarText(string.Format("OSOBE: online: {0} / local: {1}", DatabaseManagerRemote.CountPersons(), personList.Count));
                //ObservableCollection<Person> onlinePersonList = DatabaseManagerRemote.FetchPersonList();
                //SetBusyStateAndContent("Brišem nepostojeće osobe", true);
                //foreach (Person tempPerson in onlinePersonList) {
                //    if (personList.Contains(tempPerson, new PersonEqualityComparerByID()) == false)
                //        DatabaseManagerRemote.DeletePerson(tempPerson);
                //}
                //SetBusyStateAndContent("Ubacujem osobe koje fale", true);
                //foreach (Person tempPerson in personList) {
                //    if (onlinePersonList.Contains(tempPerson, new PersonEqualityComparerByID()) == false)
                //        DatabaseManagerRemote.InsertPerson(tempPerson);
                //}

                #endregion

                #region Languages

                //SetStatusBarText(string.Format("JEZICI: online: {0} / local: {1}", DatabaseManagerRemote.CountLanguages(), languageList.Count));
                //ObservableCollection<Language> onlineLangList = DatabaseManagerRemote.FetchLanguageList();
                //SetBusyStateAndContent("Brišem nepostojeće jezike", true);
                //foreach (Language onlineLang in onlineLangList) {
                //    if (languageList.Contains(onlineLang, new LanguageComparerByID()) == false)
                //        DatabaseManagerRemote.DeleteLanguage(onlineLang);
                //}
                //SetBusyStateAndContent ("Ubacujem jezike koji fale", true);
                //foreach (Language localLang in languageList) {
                //    if (onlineLangList.Contains (localLang, new LanguageComparerByID ()) == false)
                //        DatabaseManagerRemote.InsertLanguage(localLang);
                //}

                #endregion                   

                #region HDDs

                //SetStatusBarText(string.Format("HDDovi: online: {0} / local: {1}", DatabaseManagerRemote.CountHDDs(), hddList.Count));
                //ObservableCollection<HDD> onlineHDDList = DatabaseManagerRemote.FetchHDDList();
                //SetBusyStateAndContent("Brišem nepostojeće jezike", true);
                //foreach (HDD onlineHDD in onlineHDDList) {
                //    if (hddList.Contains(onlineHDD, new HDDComparerByID()) == false)
                //        DatabaseManagerRemote.DeleteHDD(onlineHDD);
                //}
                //SetBusyStateAndContent("Ubacujem HDDove koji fale", true);
                //foreach (HDD localHDD in hddList) {
                //    if (onlineHDDList.Contains(localHDD, new HDDComparerByID()) == false)
                //        DatabaseManagerRemote.InsertHDD(localHDD);
                //}

                #endregion

                #region Series

                SetStatusBarText(string.Format("SERIJE: online: {0} / local: {1}", DatabaseManagerRemote.FetchSerieCount(), listOfSeries.Count));
                int counter = 0;
                ObservableCollection<Serie> onlineSeries = DatabaseManagerRemote.FetchSerieList(genreList, personList, ref counter);
                SetBusyStateAndContent("Brišem nepostojeće serije", true);
                foreach (Serie onlineSerie in onlineSeries) {
                    if (listOfSeries.Contains(onlineSerie, new SerieComparerByID()) == false)
                        DatabaseManagerRemote.DeleteSerie(onlineSerie);
                }
                SetBusyStateAndContent ("Ubacujem serije koje fale", true);
                foreach (Serie localSerie in listOfSeries) {
                    if (onlineSeries.Contains (localSerie, new SerieComparerByID ()) == false) {
                        DatabaseManagerRemote.InsertSerie(localSerie);
                        //foreach (SerieSeason tempSeason in localSerie.Seasons) {
                        //    DatabaseManagerRemote.InsertSerieSeason(tempSeason);
                        //    foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                        //        DatabaseManagerRemote.InsertSerieEpisode(tempEpisode);
                        //}
                    }
                }

                #endregion

                #region Seasons

                SetStatusBarText(string.Format("SEZONE: online: {0} / local: {1}", DatabaseManagerRemote.FetchSeasonCount(), listOfSerSeasons.Count)); 
                counter = 0;
                ObservableCollection<SerieSeason> onlineSeasons = DatabaseManagerRemote.FetchSerieSeasonList(listOfSeries, ref counter);

                SetBusyStateAndContent("Brišem nepostojeće sezone", true);
                foreach (SerieSeason onlineSeason in onlineSeasons) {
                    if (listOfSerSeasons.Contains(onlineSeason, new SeasonComparerByID()) == false)
                        DatabaseManagerRemote.DeleteSerieSeason(onlineSeason);
                }

                SetBusyStateAndContent ("Ubacujem sezone koje fale", true);                
                foreach (SerieSeason localSeason in listOfSerSeasons) {
                    if (onlineSeasons.Contains (localSeason, new SeasonComparerByID ()) == false) {
                        DatabaseManagerRemote.InsertSerieSeason(localSeason);
                        //foreach (SerieEpisode tempEpisode in localSeason.Episodes)
                        //    DatabaseManagerRemote.InsertSerieEpisode(tempEpisode);
                    }
                }

                #endregion

                #region Episodes

                int countOnlineEpisodes = DatabaseManagerRemote.FetchEpisodeCount();
                ObservableCollection<SerieEpisode> onlineEpisodes = DatabaseManagerRemote.FetchSerieEpisodeList(listOfSerSeasons, languageList, hddList, ref countOnlineEpisodes);
                SetStatusBarText(string.Format("EPIZODE: online: {0} / local: {1}", countOnlineEpisodes, listOfSerEpisodes.Count));
                
                SetBusyStateAndContent("Brišem nepostojeće epizode", true);
                foreach (SerieEpisode onlineEpisode in onlineEpisodes) {
                    if (listOfSerEpisodes.Contains(onlineEpisode, new EpisodeComparerByID()) == false)
                        DatabaseManagerRemote.DeleteSerieEpisode(onlineEpisode);
                }

                SetBusyStateAndContent ("Ubacujem epizode koje fale", true);
                //return;
                //MessageBox.Show (onlineEpisodes.Count.ToString ());
                foreach (SerieEpisode localEpisode in listOfSerEpisodes) {
                    if (onlineEpisodes.Contains (localEpisode, new EpisodeComparerByID ()) == false)
                        DatabaseManagerRemote.InsertSerieEpisode(localEpisode);
                }

                #endregion
                
                #region Movies

                SetStatusBarText(string.Format("FILMOVI: online: {0} / local: {1}", DatabaseManagerRemote.FetchMovieCountList(), listOfMovies.Count));
                ObservableCollection<Movie> onlineMovies = DatabaseManagerRemote.FetchMovieList(genreList, personList, languageList, hddList, ref counter);

                SetBusyStateAndContent("Brišem nepostojeće filmove", true);
                foreach (Movie onlineMovie in onlineMovies)
                    if (listOfMovies.Contains(onlineMovie, new MovieComparerByID()) == false)
                        DatabaseManagerRemote.DeleteMovie(onlineMovie);         

                SetBusyStateAndContent("Ubacujem filmove koji fale", true);
                counter = 0;
                foreach (Movie localMovie in listOfMovies)
                    if (onlineMovies.Contains(localMovie, new MovieComparerByID()) == false)
                        DatabaseManagerRemote.InsertNewMovie(localMovie);

                      

                #endregion
            }
            catch (Exception e) {
                MessageBox.Show (e.Message);
            }

            SetBusyStateAndContent ("", false);
            SetStatusBarText ("");
            ThreadLoadData.Abort ();
        }
        private void uploadNewVersion_Click (object sender, System.Windows.RoutedEventArgs e) {
            try {
                foreach (Movie m in listOfMovies)
                {
                    HelpFunctions.UploadPhotoFTP(HardcodedStrings.moviePosterFolder + m.Name + ".jpg", GlobalCategory.movie, m.Name + ".jpg");
                }

                foreach (Serie s in listOfSeries)
                {
                    HelpFunctions.UploadPhotoFTP(HardcodedStrings.seriePosterFolder + s.Name + ".jpg", GlobalCategory.serie, s.Name + ".jpg");
                }
                MessageBox.Show("success");
                return;
                StreamWriter personOutput = new StreamWriter ("C:\\person.sql.txt");
                StreamWriter serieOutput = new StreamWriter ("C:\\serie.sql.txt");
                StreamWriter movieOutput = new StreamWriter ("C:\\movie.sql.txt");
                StreamWriter misc = new StreamWriter ("C:\\misc.sql.txt");

                #region Person

                PersonType newType = new PersonType (1);
                newType.Name = "za kućni video";
                ExportToTxtCommands.InsertPersonType (newType, personOutput);
                newType = new PersonType (2);
                newType.Name = "za filmove/serije";
                ExportToTxtCommands.InsertPersonType (newType, personOutput);
                foreach (Person tempPerson in personList)
                    ExportToTxtCommands.InsertPerson (tempPerson, personOutput);

                #endregion

                #region Misc

                foreach (Language templang in languageList)
                    ExportToTxtCommands.InsertLanguage (templang, misc);
                foreach (HDD tempHDD in hddList)
                    ExportToTxtCommands.InsertHDD (tempHDD, misc);
                foreach (Audio tempAudio in audioList)
                    ExportToTxtCommands.InsertAudio (tempAudio, misc);
                foreach (Genre tempGenre in genreList)
                    ExportToTxtCommands.InsertGenres (tempGenre, misc);

                #endregion

                foreach (Movie tempMovie in listOfMovies)
                    ExportToTxtCommands.InsertNewMovie (tempMovie, movieOutput);
                foreach (Serie tempSerie in listOfSeries) {
                    ExportToTxtCommands.InsertSerie (tempSerie, serieOutput);
                    foreach (SerieSeason tempSeason in tempSerie.Seasons) {
                        ExportToTxtCommands.InsertSerieSeason (tempSeason, serieOutput);
                        foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
                            ExportToTxtCommands.InsertSerieEpisode (tempEpisode, serieOutput);
                    }
                }

                personOutput.Close ();
                serieOutput.Close ();
                movieOutput.Close ();
                misc.Close ();
            }
            catch (Exception ex) {
                MessageBox.Show (ex.Message);
            }


            return;
            decimal version = DatabaseManager.ThisVersionNumber ();
            if (version == 0)
                version = 1.0m;
            version += 0.001m; 
            busyIncidator.IsBusy = true;
            busyIncidator.BusyContent = "Uploadanje nove verzije (" + version + ")";
            ThreadStart starter = delegate {
                DatabaseManager.InsertnewVersion (version, "");
                //DatabaseManagerMySql.InsertnewVersion (version, "");
                try {
                    HelpFunctions.DeleteCurrentVersionFTP ();
                }
                catch {
                }
                try {
                    HelpFunctions.UploadNewVersionFTP ();
                }
                catch (Exception ex) {
                    MessageBox.Show (ex.Message);
                    try {
                        HelpFunctions.DeleteCurrentVersionFTP ();
                        //DatabaseManagerMySql.DeleteAllVersions ();
                    }
                    catch {
                    }
                }
                this.Dispatcher.Invoke (new Action (() => {
                    busyIncidator.IsBusy = false;
                    statusBarTextBlock.Text =  string.Format ("Uspješno obavljen update s {0} na {1}", version-0.001m, version);
                }));
            };
            
            Thread uploadNewAppThread = new Thread (starter);
            uploadNewAppThread.IsBackground = true;
            uploadNewAppThread.Start ();
        }

        private void showAllSize_Click (object sender, System.Windows.RoutedEventArgs e) {
            
            long movieSize = 0;
            long serieSize = 0;
            foreach (Movie tempMovie in listOfMovies)
                movieSize += tempMovie.Size;
            foreach (SerieEpisode tempEpisode in listOfSerEpisodes)
                serieSize += tempEpisode.Size;

            double movieSizeF;
            double serieSizeF;
            movieSizeF = movieSize / 1024.0;
            movieSizeF = movieSizeF / 1024;
            movieSizeF = movieSizeF / 1024;
            movieSizeF = movieSizeF / 1024;
            serieSizeF = serieSize / 1024.0;
            serieSizeF = serieSizeF / 1024;
            serieSizeF = serieSizeF / 1024;
            serieSizeF = serieSizeF / 1024;
            MessageBox.Show (string.Format ("UKUPNO:  {0} TB\r\n\r\nFilmovi:     {1} TB\r\nSerije:         {2} TB", 
                (movieSizeF + serieSizeF).ToString (".0"), movieSizeF.ToString (".0"), serieSizeF.ToString (".0")));
        }

        private void UpdateMovieSummaries() {
            bool skip = true;
            foreach (Movie tempMovie in listOfMovies) {
                if (tempMovie.Name.Contains("Tom and Jerry")) {
                    skip = false;
                    continue;
                }
                if (skip)
                    continue;
                IMDb imdbPage = new IMDb();
                Movie temp = imdbPage.GetMovieInfo(tempMovie.InternetLink);
                if (!String.IsNullOrEmpty(temp.Summary) && tempMovie.Summary != temp.Summary) {
                    tempMovie.Summary = temp.Summary;
                    UpdateMovie(tempMovie);
                }
            }
        }

        private void UpdateSerieSummaries() {
            bool skip = false;
            foreach (Serie tempSerie in listOfSeries) {
                if (tempSerie.Name.Contains("Tom and Jerry")) {
                    skip = false;
                    continue;
                }
                if (skip)
                    continue;
                IMDb imdbPage = new IMDb();
                Serie temp = imdbPage.GetSerieInfo(tempSerie.InternetLink);
                if (!String.IsNullOrEmpty(temp.Summary) && tempSerie.Summary.Length > temp.Summary.Length) {
                    tempSerie.Summary = temp.Summary;
                    UpdateSerie(tempSerie, false);
                }
            }
        }

       
    }
}
