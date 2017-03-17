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
   
    public partial class HomeVideoForm: Window {

        #region GLOBAL DATA

        public HomeVideo currHomeVideo = new HomeVideo ();
        ObservableCollection<Category> categoryList = new ObservableCollection<Category> ();
        ObservableCollection<Camera> cameraList = new ObservableCollection<Camera> ();
        ObservableCollection<HDD> hddList = new ObservableCollection<HDD> ();
        ObservableCollection<Person> personList = new ObservableCollection<Person> ();
        public bool accepted = false;
        Cloner cloner = new Cloner();
        ////FinderInCollection finder = new FinderInCollection ();
        Settings settings = new Settings ();
        public List<HDD> insertedHDDs = new List<HDD> ();
        public List<Person> insertedPersons = new List<Person> ();
        public List<Camera> insertedCameras = new List<Camera> ();
        public List<Category> insertedCategories = new List<Category> ();
        bool isNew;
        WishOrRegular type;

        #endregion

        #region ON LOAD

        public HomeVideoForm (WishOrRegular type, ObservableCollection<Person> personList, ObservableCollection<Camera> cameraList, ObservableCollection<Category> categoryList) {
            InitializeComponent ();
            this.personList = personList;
            this.cameraList = cameraList;
            this.categoryList = categoryList;
            this.currHomeVideo.AddTime = DateTime.Now;
            isNew = true;
            this.type = type;
            if (this.type == WishOrRegular.regular) {
            }
            else {
                this.mainWindow.Width = 832;
                this.movieTechInfoGrid.Visibility = System.Windows.Visibility.Hidden;
                this.movieTechInfoGrid.IsEnabled = false;
            }
        }
        public HomeVideoForm (HomeVideo toEditHomeVideo, ObservableCollection<Person> personList, ObservableCollection<Camera> cameraList, ObservableCollection<Category> categoryList) {
            InitializeComponent ();
            this.personList = personList;
            this.cameraList = cameraList;
            this.categoryList = categoryList;
            this.currHomeVideo = cloner.CloneHomeVideo (toEditHomeVideo);            
            this.filmDatePicker.SelectedDate = currHomeVideo.FilmDate;
            isNew = false;
            this.type = WishOrRegular.regular;
        }
        public HomeVideoForm (WishHomeVideo homeVideoToEdit, ObservableCollection<Person> personList, ObservableCollection<Camera> cameraList, ObservableCollection<Category> categoryList) {
            InitializeComponent ();
            this.personList = personList;
            this.cameraList = cameraList;
            this.categoryList = categoryList;
            this.type = WishOrRegular.wish;
            this.currHomeVideo = HomeVideoFromWish (homeVideoToEdit);
            this.filmDatePicker.SelectedDate = currHomeVideo.FilmDate;
            isNew = false;

            this.mainWindow.Width = 832;
            this.movieTechInfoGrid.Visibility = System.Windows.Visibility.Hidden;
            this.movieTechInfoGrid.IsEnabled = false;
        }
        private void Window_Loaded (object sender, RoutedEventArgs e) {
            this.DataContext = currHomeVideo;            
            hddList = DatabaseManager.FetchHDDList ();
            settings = DatabaseManager.FetchSettings ();
            this.hddComboBox.ItemsSource = hddList;            
            this.cameraComboBox.ItemsSource = cameraList;            
            this.categoryComboBox.ItemsSource = categoryList;     
       
            if (isNew == false)
                try {
                    this.posterImage.Source = Clipboard.GetImage ();
                    this.currHomeVideo.Hdd = FinderInCollection.FindInHDDCollection (hddList, currHomeVideo.Hdd.ID);
                    this.currHomeVideo.VideoCategory = FinderInCollection.FindInCategoryCollection (categoryList, currHomeVideo.VideoCategory.ID);
                    this.currHomeVideo.FilmingCamera = FinderInCollection.FindInCameraCollection (cameraList, currHomeVideo.FilmingCamera.ID);
                }
                catch {
                }

        }

        #endregion

        private void openCameraManager_Click (object sender, RoutedEventArgs e) {
            CamerasManagerForm camerasForm = new CamerasManagerForm (cameraList);
            camerasForm.Owner = this;
            camerasForm.ShowDialog ();
        }
        private void openCategoryManager_Click (object sender, RoutedEventArgs e) {
            CategoryForm selectCategoryForm = new CategoryForm (categoryList);
            selectCategoryForm.Owner = this;
            selectCategoryForm.ShowDialog ();
        }

        private void addCamerman_Click (object sender, RoutedEventArgs e) {
            PersonsManagerForm selectCamermanForm = new PersonsManagerForm (personList);
            selectCamermanForm.Owner = this;
            selectCamermanForm.ShowDialog ();
            if (selectCamermanForm.accepted) {
                foreach (Person selectedPerson in selectCamermanForm.personsListBox.SelectedItems) {
                    if (currHomeVideo.Camermans.Contains (selectedPerson, new PersonEqualityComparerByName ()) == false)
                        currHomeVideo.Camermans.Add (selectedPerson);
                }
            }
        }
        private void removeCamerman_Click (object sender, RoutedEventArgs e) {
            Person selectedPerson = currHomeVideo.Camermans.ElementAt (camermanListBox.SelectedIndex);
            if (selectedPerson == null)
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi kamermana", "Nema odabira", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                currHomeVideo.Camermans.Remove (selectedPerson);
        }

        private void addPerson_Click (object sender, RoutedEventArgs e) {
            PersonsManagerForm selectPersonForm = new PersonsManagerForm (personList);
            selectPersonForm.Owner = this;
            selectPersonForm.ShowDialog ();
            if (selectPersonForm.accepted) {
                foreach (Person selectedPerson in selectPersonForm.personsListBox.SelectedItems) {
                    if (currHomeVideo.PersonsInVideo.Contains (selectedPerson, new PersonEqualityComparerByName()) == false)
                        currHomeVideo.PersonsInVideo.Add (selectedPerson);
                }
            }
        }
        private void removePerson_Click (object sender, RoutedEventArgs e) {
            Person selectedPerson = currHomeVideo.PersonsInVideo.ElementAt (castListBox.SelectedIndex);
            if (selectedPerson == null)
                Xceed.Wpf.Toolkit.MessageBox.Show ("Odaberi kamermana", "Nema odabira", MessageBoxButton.OK, MessageBoxImage.Warning);
            else
                currHomeVideo.PersonsInVideo.Remove (selectedPerson);
        }

        private void filmDatePicker_SelectedDateChanged (object sender, SelectionChangedEventArgs e) {
            this.currHomeVideo.FilmDate = (DateTime) filmDatePicker.SelectedDate;
        }

        #region POSTER

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

        #endregion

        #region TECH INFO

        private void loadFileButton_Click (object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog browseFile = new Microsoft.Win32.OpenFileDialog ();
            browseFile.Filter = "Podržani video formati |*.avi;*.mkv;*.vob;*.vob;*.mpg;*.mpeg;*.mp4;*.mov;*.3gp;*.wmv;*.mpgv;*.mpv;*.m1v;*.m2v;*.asf;*.rmvb|Svi formati |*.*";
            Nullable<bool> result = browseFile.ShowDialog ();
            if (result == true) {
                currHomeVideo.AudioList.Clear ();
                currHomeVideo.SubtitleLanguageList.Clear ();
                string videoPath = browseFile.FileName;
                currHomeVideo.GetTechInfo (videoPath);
                movieTechInfoGrid.DataContext = null;
                movieTechInfoGrid.DataContext = currHomeVideo;
            }
        }
        private void addNewHdd_Click (object sender, RoutedEventArgs e) {
            HDDForm newHDDForm = new HDDForm ();
            newHDDForm.Owner = this;
            newHDDForm.ShowDialog ();
            if (newHDDForm.accepted) {
                DatabaseManager.InsertHDD (newHDDForm.currHDD);
                DatabaseManagerMySql.InsertHDD (newHDDForm.currHDD);
                hddList.Add (newHDDForm.currHDD);
                insertedHDDs.Add (newHDDForm.currHDD);
            }
        }
        private void editAudioLng_Click (object sender, RoutedEventArgs e) {
            try {
                Audio selectedAudio = currHomeVideo.AudioList.ElementAt (audioLanguagesListBox.SelectedIndex);
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
                audioLanguagesListBox.DataContext = currHomeVideo;
            }
        }      

        #endregion        

        private void acceptButton_Click (object sender, RoutedEventArgs e) {
            if (Validate ()) {
                this.accepted = true;
                DatabaseManager.UpdateSettings (settings);
                SavePoster ();
                this.currHomeVideo.AddTime = DateTime.Now;
                this.Close ();
            }
        }
        private void cancelButton_Click (object sender, RoutedEventArgs e) {
            this.accepted = false;
            this.Close ();
        }

        #region HELP FUNCTIONS

        private bool Validate () {
            if (currHomeVideo.Name.Length < 3) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Duljina naziva mora biti barem 3 znaka");
                return false;
            }
            if (this.cameraComboBox.SelectedIndex < 0) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabrana kamera");
                return false;
            }
            if (this.categoryComboBox.SelectedIndex < 0) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabrana kategorija");
                return false;
            }
            if (this.filmDatePicker.SelectedDate == null) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran datum");
                return false;
            }
            if (this.currHomeVideo.Size == 0 && this.type == WishOrRegular.regular) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije učitana video datoteka");
                return false;
            }
            if (this.currHomeVideo.Hdd == null && this.type == WishOrRegular.regular) {
                Xceed.Wpf.Toolkit.MessageBox.Show ("Nije odabran HDD");
                return false;
            }
            return true;
        }
        private void SavePoster () {
            if (posterImage.Source == null)
                return;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder ();
            encoder.Frames.Add (BitmapFrame.Create ((BitmapSource) posterImage.Source));
            encoder.QualityLevel = (int) imageQualityLevelSlider.Value;
            try {
                File.Delete (HardcodedStrings.homeVideoPosterFolder + this.currHomeVideo.Name + ".jpg");
            }
            catch {
            }
            try {
                try {
                    Directory.CreateDirectory (HardcodedStrings.homeVideoPosterFolder);
                }
                catch {
                }
                FileStream newStream = new FileStream (HardcodedStrings.homeVideoPosterFolder + this.currHomeVideo.Name + ".jpg", FileMode.Create);
                encoder.Save (newStream);
                newStream.Close ();
            }
            catch {
            }
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
    }
}
