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
using System.IO;
using System.Net;
using System.Web;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace Video_katalog {
    /// <summary>
    /// Interaction logic for SearchTitleImdb.xaml
    /// </summary>
    public partial class SearchForm: Window {

        #region GLOBAL DATA

        public string selectedLink = null;
        public ICollectionView _searchResultsView;
        Thread ThreadDownloadData;
        DispatcherTimer TimerCheckThread = new DispatcherTimer ();
        string searchString;
        string defaultSearchString;
        DispatcherTimer continueTimer = new DispatcherTimer ();
        DispatcherTimer searchTimer = new DispatcherTimer ();
        DispatcherTimer refreshTimer = new DispatcherTimer ();
        System.Diagnostics.Stopwatch stopwatch;

        #endregion

        #region ON START

        public SearchForm () {
            InitializeComponent ();
        }
        public SearchForm (string searchString) {
            InitializeComponent ();
            this.searchString = searchString;
            this.searchTextBox.Text = searchString;
            //searchButton_Click (null, null);

            continueTimer.Interval = new TimeSpan (0, 0, 0, 1);
            continueTimer.Tick += (PressConfirm);

            searchTimer.Interval = new TimeSpan (0, 0, 0, 0, 100);
            searchTimer.Tick += (pressSearchButton);
            searchTimer.Start ();
            stopwatch = new System.Diagnostics.Stopwatch ();
            this.timerValueTB.DataContext = stopwatch;
            refreshTimer.Interval = new TimeSpan (0, 0, 0, 0, 100);
            refreshTimer.Tick += (refreshTimerTB);
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            //defaultSearchString = this.searchTextBox.Text;
            searchTextBox.Focus ();            
        }

        #endregion

        private void searchTextBox_GotFocus (object sender, RoutedEventArgs e) {
            if (searchTextBox.Text == defaultSearchString)
                searchTextBox.Text = "";
        }
        private void searchTextBox_LostFocus (object sender, RoutedEventArgs e) {
            if (searchTextBox.Text == "")
                searchTextBox.Text = defaultSearchString;
        }
        private void searchTextBox_TextChanged (object sender, TextChangedEventArgs e) {
            searchString = this.searchTextBox.Text;
        }
        private void ImageAsImdbLink_MouseUp (object sender, MouseButtonEventArgs e) {
            System.Diagnostics.Process.Start (((SearchResult) _searchResultsView.CurrentItem).Link);
        }

        void pressSearchButton (object o, EventArgs e) {
            searchTimer.Stop ();
            searchButton_Click (null, null);
        }
        private void searchButton_Click (object sender, RoutedEventArgs e) {
            try {
                TimerCheckThread.Stop ();
                ThreadDownloadData.Suspend ();                
            }
            catch {
            }
            ThreadDownloadData = new Thread (new ThreadStart (DownloadSearchResult));
            TimerCheckThread.Tick += new EventHandler (CheckIsThreadAlive);
            TimerCheckThread.Interval = new TimeSpan (0, 0, 0, 0, 100);
            busyIndicator.BusyContent = "Tražim...\r\n";
            searchString = searchTextBox.Text;

            busyIndicator.IsBusy = true;
            TimerCheckThread.Start ();
            ThreadDownloadData.Start ();
            resultListBox.Focus ();
        }
        public void DownloadSearchResult () {
            int maxSearchResult = 15;
            if (searchString.Length == 0 || searchString == defaultSearchString) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Upiši naziv za pretragu");
                return;
            }
            IMDb tempIMDb = new IMDb ();
            _searchResultsView = CollectionViewSource.GetDefaultView (tempIMDb.GetSearchResults ("http://www.imdb.com/find?s=tt&q=" + searchString, maxSearchResult));           
        }
        private void CheckIsThreadAlive (object sender, EventArgs e) {
            if (ThreadDownloadData.IsAlive == false) {
                TimerCheckThread.Stop ();
                busyIndicator.IsBusy = false;
                ShowResults ();
                resultListBox.Focus ();
                continueTimer.Start ();
                try {
                    stopwatch.Start ();
                    refreshTimer.Start ();
                    pauseTimer.Visibility = System.Windows.Visibility.Visible;
                }
                catch {
                }
            }
        }
        private void ShowResults () {
            this.DataContext = _searchResultsView;
            resultListBox.SelectedIndex = 0;
            if (resultListBox.Items.Count == 1) {
                resultListBox.SelectedIndex = 0;
                this.selectedLink = ((SearchResult) _searchResultsView.CurrentItem).Link;
                this.Close ();
            }            
            //this.searchButton.IsDefault = false;
            //this.confirmSelectionButton.IsDefault = true;
            resultListBox.Focus ();
        }

        void PressConfirm (object sender, EventArgs e) {
            continueTimer.Stop ();
            confirmSelectionButton_Click (null, null);
        }
        private void confirmSelectionButton_Click (object sender, RoutedEventArgs e) {
            if (resultListBox.SelectedIndex < 0) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberite jedan od rezultata");
                return;
            }
            this.selectedLink = ((SearchResult) _searchResultsView.CurrentItem).Link;
            this.Close ();
        }

        private void pauseTimer_Click (object sender, RoutedEventArgs e) {
            if (continueTimer.IsEnabled) {
                continueTimer.Stop ();
                stopwatch.Reset ();
                pauseTimer.Content = "Start";
            }
            else {
                continueTimer.Start ();
                stopwatch.Start ();
                pauseTimer.Content = "Stop";
            }
        }
        private void refreshTimerTB (object sender, EventArgs e) {
            this.timerValueTB.Text = stopwatch.Elapsed.Seconds.ToString () + "." + stopwatch.Elapsed.Milliseconds.ToString ().Substring (0, 1) + " / 1.0";
        }
    }
}