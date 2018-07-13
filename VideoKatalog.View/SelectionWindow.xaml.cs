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
using System.Data.SqlServerCe;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.IO;
using MySql.Data.MySqlClient;
//using ControlLib;

namespace Video_katalog
{
	/// <summary>
	/// Interaction logic for SelectionWindow.xaml
	/// </summary>
	public partial class SelectionWindow : Window
	{

		bool updateData = true;
		bool loadData = true;
		bool downloadImageOnUpdate = true;

		#region GLOBAL DATA

		SqlCeConnection connection = new SqlCeConnection(ComposeConnectionString.ComposeCEString());
		Cloner cloner = new Cloner();
		bool allreadyFilledData = false;
		bool notSaved = false;

		private ObservableCollection<Person> personList = new ObservableCollection<Person>();
		private ObservableCollection<Language> languageList = new ObservableCollection<Language>();
		private ObservableCollection<HDD> hddList = new ObservableCollection<HDD>();
		private ObservableCollection<Genre> genreList = new ObservableCollection<Genre>();
		private ObservableCollection<Audio> audioList = new ObservableCollection<Audio>();
		private ObservableCollection<Selection> selectionList = new ObservableCollection<Selection>();

		private List<Button> potentialDefaultButtons = new List<Button>();

		//List<string> imagesToDelete = new List<string> ();

		Thread ThreadLoadData;
		Thread ThreadCheckUpdates;
		DispatcherTimer TimerCheckLoadingThread = new DispatcherTimer();
		DispatcherTimer TimerCheckUpdatingThread = new DispatcherTimer();

		HDD customerHDD = new HDD("new", DateTime.Now, 0, 1490000000000);
		long hddSize = 1500000000000;
		long selectedSize = 0;

		int progressBarCounter = 0;
		DispatcherTimer TimerRefreshLoadProgressBar = new DispatcherTimer();

		#endregion

		#region ON START

		public SelectionWindow()
		{
			InitializeComponent();
			this.Title = string.Format("Video Katalog (v{0})", DatabaseManager.ThisVersionNumber());
		}
		public SelectionWindow(ObservableCollection<Movie> listOfMovies, ObservableCollection<Serie> listOfSeries, ObservableCollection<Person> personList, ObservableCollection<Language> languageList,
								ObservableCollection<Genre> genreList, ObservableCollection<HDD> hddList, ObservableCollection<Audio> audioList)
		{

			InitializeComponent();
			this.Title = string.Format("Video Katalog (v{0})", DatabaseManager.ThisVersionNumber());
			this.listOfMovies = listOfMovies;
			this.listOfSeries = listOfSeries;
			this.personList = personList;
			this.languageList = languageList;
			this.hddList = hddList;
			this.genreList = genreList;
			this.audioList = audioList;
			allreadyFilledData = true;
			SetMovieSlides();
			SetSerieSliders();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SetConstantHotkeys();
			tabMovies.Focus();
			if (allreadyFilledData)
			{
				SetMovieSlides();
				//SetSerieSliders ();
				//FillSerieTreeView ();
				SetViewsAndDataContent();
				FillSerieTreeViewFirstTime();
				statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
				statusBarTextBlock.Text = "";
				return;
			}
			TimerRefreshLoadProgressBar.Interval = new TimeSpan(0, 0, 0, 0, 100);
			TimerRefreshLoadProgressBar.Tick += new EventHandler(RefreshStatusBarProgressBar);
			TimerRefreshLoadProgressBar.Start();

			busyIndicator.IsBusy = true;
			busyIndicator.BusyContent = "Učitavanje podataka...\r\n";
			ThreadLoadData = new Thread(new ThreadStart(FillInitialData));
			TimerCheckLoadingThread.Tick += new EventHandler(CheckIsThreadAlive);
			TimerCheckLoadingThread.Interval = new TimeSpan(0, 0, 0, 0, 100);
			ThreadLoadData.IsBackground = true;
			if (loadData)
			{
				ThreadLoadData.Start();
				TimerCheckLoadingThread.Start();
			}
			else
				busyIndicator.IsBusy = false;

			potentialDefaultButtons.Add(movieSearchGridClose);
			potentialDefaultButtons.Add(closeMovieSortGrid);
			potentialDefaultButtons.Add(closeMovieViewTheme);
			//potentialDefaultButtons.Add (addRemoveMovie2Default);
			//potentialDefaultButtons.Add (addRemoveSerie2Default);
			//potentialDefaultButtons.Add (serieSearchGridClose);
			//potentialDefaultButtons.Add (closeSerieSortGrid);
		}
		private void SetConstantHotkeys()
		{
			#region Save
			RoutedCommand command = new RoutedCommand();
			InputBinding ib = new InputBinding(command, new KeyGesture(Key.S, ModifierKeys.Control));
			this.InputBindings.Add(ib);
			// Bind handler
			CommandBinding cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(HandlerThatSavesEverthing);
			this.CommandBindings.Add(cb);
			#endregion

			#region Open
			command = new RoutedCommand();
			ib = new InputBinding(command, new KeyGesture(Key.O, ModifierKeys.Control));
			this.InputBindings.Add(ib);
			// Bind handler
			cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(OpenLoadSelected);
			this.CommandBindings.Add(cb);
			#endregion

			#region Switch Tab
			command = new RoutedCommand();
			ib = new InputBinding(command, new KeyGesture(Key.Tab, ModifierKeys.Control));
			this.InputBindings.Add(ib);
			// Bind handler
			cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(SwitchTab);
			this.CommandBindings.Add(cb);
			#endregion
		}
		private void HandlerThatSavesEverthing(object obSender, ExecutedRoutedEventArgs e)
		{
			saveSelected_Click(null, null);
		}
		private void OpenLoadSelected(object obSender, ExecutedRoutedEventArgs e)
		{
			loadSelected_Click(null, null);
		}
		private void OpenSerieSearch(object obSender, ExecutedRoutedEventArgs e)
		{
			var stb = (Storyboard)TryFindResource("serieSearchGridShow");
			showingSelectHddSize = false;
			if (stb != null)
			{
				stb.Begin();
				System.Media.SoundPlayer player = new System.Media.SoundPlayer(VideoKatalog.View.Properties.Resources.close5);
				player.Play();
			}
			//playOpenSound (serieDockSearch, null);
			//searchSerieName.Focus ();
		}
		private void SwitchTab(object obSender, ExecutedRoutedEventArgs e)
		{
			if (serieTabFocused)
			{
				tabMovies.Focus();
				movieListBox.Focus();
			}
			else
			{
				//tabSeries.Focus ();
				//serieTreeView.Focus ();
			}
		}

		private void RefreshStatusBarProgressBar(object sender, EventArgs e)
		{
			statusBarProgressBar.Value = progressBarCounter;
		}

		private void FillInitialData()
		{
			try
			{
				connection.Open();
				/*this.Dispatcher.Invoke (new Action (() => {
                    statusBarTextBlock.Text = "Osobe";
                    statusBarProgressBar.Maximum = DatabaseManager.FetchPersonCount ();
                    statusBarProgressBar.Value = 0;
                    statusBarProgressBar.Visibility = System.Windows.Visibility.Visible;
                }));
                progressBarCounter = 0;*/
				personList = DatabaseManager.FetchPersonList(ref progressBarCounter);
				hddList = DatabaseManager.FetchHDDList();
				genreList = DatabaseManager.FetchGenreList();
				languageList = DatabaseManager.FetchLanguageList();
				audioList = DatabaseManager.FetchAudioList(languageList);

				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarTextBlock.Text = "Filmovi";
					statusBarProgressBar.Maximum = DatabaseManager.FetchMovieCountList();
					statusBarProgressBar.Value = 0;
				}));
				progressBarCounter = 0;
				listOfMovies = DatabaseManager.FetchMovieList(genreList, audioList, personList, languageList, hddList, ref progressBarCounter);

				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarTextBlock.Text = "Serije";
					statusBarProgressBar.Maximum = DatabaseManager.FetchSerieCount() + DatabaseManager.FetchSeasonCount() + DatabaseManager.FetchEpisodeCount() + 50;
					statusBarProgressBar.Value = 0;
				}));
				progressBarCounter = 0;
				listOfSeries = DatabaseManager.FetchSerieList(genreList, personList, ref progressBarCounter);
				listOfSerSeasons = DatabaseManager.FetchSerieSeasonList(listOfSeries, ref progressBarCounter);
				listOfSerEpisodes = DatabaseManager.FetchSerieEpisodeList(listOfSerSeasons, languageList, audioList, hddList, ref progressBarCounter);

				selectionList = DatabaseManager.FetchSelectionList(listOfMovies, listOfSerEpisodes);
				connection.Close();
			}
			catch (Exception newExc)
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("ERROR: " + newExc.Message);
				}));
			}
		}
		private void DeleteEmptySeasons()
		{
			List<SerieSeason> seasonsToRemove = new List<SerieSeason>();
			foreach (SerieSeason tempseason in listOfSerSeasons)
				if (tempseason.Episodes.Count == 0)
				{
					DatabaseManager.DeleteSerieSeason(tempseason);
					tempseason.ParentSerie.Seasons.Remove(tempseason);
					seasonsToRemove.Add(tempseason);
				}
			foreach (SerieSeason tempSeason in seasonsToRemove)
				listOfSerSeasons.Remove(tempSeason);
		}
		private void DeleteEmptySeries()
		{
			List<Serie> seriesToRemove = new List<Serie>();
			foreach (Serie tempserie in listOfSeries)
			{
				if (tempserie.Seasons.Count == 0)
				{
					DatabaseManager.DeleteSerie(tempserie);
					seriesToRemove.Add(tempserie);
				}
			}
			foreach (Serie tempSerie in seriesToRemove)
				listOfSeries.Remove(tempSerie);
		}
		private void CheckIsThreadAlive(object sender, EventArgs e)
		{
			if (ThreadLoadData.IsAlive == false)
			{
				TimerCheckLoadingThread.Stop();
				TimerRefreshLoadProgressBar.Stop();
				statusBarProgressBar.Value = 0;
				statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
				statusBarTextBlock.Text = "";

				//DeleteEmptySeasons ();
				//DeleteEmptySeries ();

				busyIndicator.IsBusy = false;
				SetMovieSlides();
				SetViewsAndDataContent();

				FillSerieTreeViewFirstTime();
				ThreadStart starter = delegate
				{
					CheckForNewMovies();
					//CheckForAppUpdate ();
					DownloadMissingPosters();
				};
				ThreadCheckUpdates = new Thread(starter);
				if (updateData)
					ThreadCheckUpdates.Start();
			}
		}
		private void SetViewsAndDataContent()
		{
			foreach (Movie tempMovie in listOfMovies)
			{
				moviesShowingList.Add(tempMovie);
			}
			_movieView = CollectionViewSource.GetDefaultView(moviesShowingList) as CollectionView;
			SortDescription sortByName = new SortDescription("Name", ListSortDirection.Ascending);
			_movieView.SortDescriptions.Add(sortByName);

			movieSubCro.Visibility = System.Windows.Visibility.Hidden;
			movieSubEng.Visibility = System.Windows.Visibility.Hidden;
			movieRez1080p.Visibility = System.Windows.Visibility.Hidden;
			movieRez720p.Visibility = System.Windows.Visibility.Hidden;
			movieRezSD.Visibility = System.Windows.Visibility.Hidden;

			_movieView.MoveCurrentToPosition(-1);
			_movieView.Filter += new Predicate<object>(FilterMovies);
			//SortSeries ("name");
			_serieView = (CollectionView)CollectionViewSource.GetDefaultView(listOfSeries);
			_serieView.MoveCurrentToPosition(0);

			this.tabMovies.DataContext = _movieView;
			SetSerieTabConstantBindings();

			movieNumberTextBlock.DataContext = _movieView;
		}

		private void CheckForNewMovies()
		{
			try
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarTextBlock.Text = "Provjera za novim filmovima i serijama...";
					updatingBusyIndicator.IsBusy = true;
					statusBarProgressBar.Maximum = 100;
					statusBarProgressBar.Value = 0;
				}));

				#region fetch online lists

				//MySqlConnection sqlConnection = new MySqlConnection (ComposeConnectionString.ComposeMySqlRemoteCS ());
				//sqlConnection.Open ();
				ObservableCollection<Language> onlineLanguageList = DatabaseManagerMySql.FetchLanguageList();
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<Audio> onlineAudioList = DatabaseManagerMySql.FetchAudioList(onlineLanguageList);
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<HDD> onlineHDDList = DatabaseManagerMySql.FetchHDDList(); ;
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<Person> onlinePersonList = DatabaseManagerMySql.FetchPersonList();
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<Movie> onlineMovieList = DatabaseManagerMySql.FetchMovieList(this.genreList, onlineAudioList, onlinePersonList, onlineLanguageList, onlineHDDList);
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<Serie> onlineSerieList = DatabaseManagerMySql.FetchSerieList(this.genreList, onlinePersonList);
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<SerieSeason> onlineSeasonList = DatabaseManagerMySql.FetchSerieSeasonList(onlineSerieList);
				if (closing)
					ThreadCheckUpdates.Abort();
				int countOnlineEpisodes = DatabaseManagerMySql.CountEpisodes();
				if (closing)
					ThreadCheckUpdates.Abort();
				ObservableCollection<SerieEpisode> onlineEpisodeList = DatabaseManagerMySql.FetchSerieEpisodeList(onlineSeasonList, onlineLanguageList, onlineAudioList, onlineHDDList, countOnlineEpisodes);
				if (closing)
					ThreadCheckUpdates.Abort();

				#endregion

				#region check local data / insert new data

				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarTextBlock.Text = "Skidanje novih podataka";
					statusBarProgressBar.Value = 0;
					statusBarProgressBar.Visibility = System.Windows.Visibility.Visible;
					updatingBusyIndicator.IsBusy = false;
				}));

				#region Insert new Language

				List<Language> newLanguages = new List<Language>();
				int countNewLanguages = 0;
				int countDeletedLangs = 0;

				foreach (Language onlineLang in onlineLanguageList)
				{
					if (languageList.Contains(onlineLang, new LanguageComparerByID()) == false)
					{
						countNewLanguages++;
						newLanguages.Add(onlineLang);
					}
				}
				foreach (Language localLang in languageList)
				{
					if (onlineLanguageList.Contains(localLang, new LanguageComparerByID()) == false)
					{
						countDeletedLangs++;
						DatabaseManager.DeleteLanguage(localLang);
					}
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					if (countNewLanguages > 0)
					{
						statusBarProgressBar.Maximum = countNewLanguages;
						statusBarProgressBar.Value = 0;
						statusBarTextBlock.Text = "Novi jezici";
					}
				}));
				foreach (Language tempLang in newLanguages)
				{
					DatabaseManager.InsertLanguageForSelection(tempLang);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
					}));
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert new audios

				List<Audio> NewAudios = new List<Audio>();
				int countNewAudios = 0;
				int countDeletedAudios = 0;

				foreach (Audio tempAudio in onlineAudioList)
				{
					if (audioList.Contains(tempAudio, new AudioComparerByID()) == false)
					{
						countNewAudios++;
						NewAudios.Add(tempAudio);
					}
				}
				foreach (Audio localAudio in audioList)
					if (onlineAudioList.Contains(localAudio, new AudioComparerByID()) == false)
					{
						countDeletedAudios++;
						DatabaseManager.DeleteAudio(localAudio);
					}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (countNewAudios > 0)
					{
						statusBarProgressBar.Maximum = countNewAudios;
						statusBarProgressBar.Value = 0;
						statusBarTextBlock.Text = "Novi audioi";
					}
				}));

				foreach (Audio tempAudio in NewAudios)
				{
					DatabaseManager.InsertAudioForSelection(tempAudio);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
					}));
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert new HDDs

				List<HDD> newHDDs = new List<HDD>();
				int countNewHDDs = 0;
				int countDeletedHDDs = 0;

				foreach (HDD tempHDD in onlineHDDList)
				{
					if (hddList.Contains(tempHDD, new HDDComparerByID()) == false)
					{
						countNewHDDs++;
						newHDDs.Add(tempHDD);
					}
				}
				foreach (HDD localHDD in hddList)
				{
					if (onlineHDDList.Contains(localHDD, new HDDComparerByID()) == false)
					{
						countDeletedHDDs++;
						DatabaseManager.DeleteHDD(localHDD);
					}
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarProgressBar.Maximum = countNewHDDs;
					statusBarProgressBar.Value = 0;
				}));
				foreach (HDD tempHDD in newHDDs)
				{
					DatabaseManager.InsertHDDForSelection(tempHDD);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
					}));
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert new persons

				List<Person> newPersons = new List<Person>();
				int countNewPersons = 0;
				int countDeletedPersons = 0;

				foreach (Person tempPerson in onlinePersonList)
				{
					if (personList.Contains(tempPerson, new PersonEqualityComparerByID()) == false)
					{
						countNewPersons++;
						newPersons.Add(tempPerson);
					}
				}
				foreach (Person localPerson in personList)
					if (onlinePersonList.Contains(localPerson, new PersonEqualityComparerByID()) == false)
					{
						countDeletedPersons++;
						DatabaseManager.DeletePerson(localPerson);
					}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (countNewPersons > 0)
					{
						statusBarProgressBar.Maximum = countNewPersons;
						statusBarProgressBar.Value = 0;
						statusBarTextBlock.Text = "Nove osobe";
					}
				}));
				foreach (Person tempPerson in newPersons)
				{
					DatabaseManager.InsertPersonForSelection(tempPerson);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Nove osobe ({0}, {1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));

				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert/Update Movies

				int newMovieCount = 0;
				int updatedMoviesCount = 0;
				int countDeletedMovies = 0;
				List<Movie> newMovies = new List<Movie>();
				List<Movie> updatedMovies = new List<Movie>();

				foreach (Movie onlineMovie in onlineMovieList)
				{
					Movie tempMovie = FinderInCollection.FindInMovieCollection(this.listOfMovies, onlineMovie.ID);
					if (tempMovie == null)
					{
						newMovieCount++;
						newMovies.Add(onlineMovie);
					}
					else if (onlineMovie.Version > tempMovie.Version)
					{
						updatedMoviesCount++;
						updatedMovies.Add(onlineMovie);
					}
				}
				foreach (Movie localMovie in listOfMovies)
				{
					if (closing)
						ThreadCheckUpdates.Abort();
					if (onlineMovieList.Contains(localMovie, new MovieComparerByID()) == false)
					{
						countDeletedMovies++;
						DatabaseManager.DeleteMovie(localMovie);
					}
				}

				//NEW MOVIES
				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarProgressBar.Maximum = newMovieCount;
					statusBarProgressBar.Value = 0;
				}));
				foreach (Movie newMovie in newMovies)
				{
					DatabaseManager.InsertNewMovieForSelection(newMovie);
					if (downloadImageOnUpdate)
						HelpFunctions.DownloadPhotoFTP(newMovie.Name, GlobalCategory.movie);
					this.Dispatcher.Invoke(new Action(() =>
					{
						if (newMovieCount > 0)
						{
							statusBarProgressBar.Value++;
							statusBarTextBlock.Text = string.Format("Novi filmovi ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
						}
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				//UPDATE MOVIES
				this.Dispatcher.Invoke(new Action(() =>
				{
					if (updatedMoviesCount > 0)
					{
						statusBarTextBlock.Text = "Ažuriranje filmova";
						statusBarProgressBar.Maximum = updatedMoviesCount;
						statusBarProgressBar.Value = 0;
					}
				}));
				foreach (Movie updatedMovie in updatedMovies)
				{
					DatabaseManager.UpdateMovie(updatedMovie);
					try
					{
						//File.Delete ("Images\\Movie\\" + updatedMovie.Name + ".jpg");
						//HelpFunctions.DownloadPhotoFTP (updatedMovie.Name, GlobalCategory.movie);
					}
					catch
					{
					}
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Ažuriranje filmova ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert new series

				int newSerieCount = 0;
				int countDeletedSeries = 0;
				List<Serie> newSeries = new List<Serie>();

				foreach (Serie tempSerie in onlineSerieList)
				{
					if (listOfSeries.Contains(tempSerie, new SerieComparerByID()) == false)
					{
						newSerieCount++;
						newSeries.Add(tempSerie);
					}
				}
				foreach (Serie localSerie in listOfSeries)
					if (onlineSerieList.Contains(localSerie, new SerieComparerByID()) == false)
					{
						countDeletedSeries++;
						DatabaseManager.DeleteSerie(localSerie);
					}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (newSerieCount > 0)
					{
						statusBarTextBlock.Text = "Nove serije";
						statusBarProgressBar.Maximum = newSerieCount;
						statusBarProgressBar.Value = 0;
					}
				}));
				foreach (Serie tempSerie in newSeries)
				{
					DatabaseManager.InsertSerieForSelection(tempSerie);
					if (downloadImageOnUpdate)
						HelpFunctions.DownloadPhotoFTP(tempSerie.Name, GlobalCategory.serie);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Nove serije ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert new seasons

				int newSerieSeasonCount = 0;
				int countDeletedSeasons = 0;
				List<SerieSeason> newSeasons = new List<SerieSeason>();

				foreach (SerieSeason tempSeason in onlineSeasonList)
				{
					if (listOfSerSeasons.Contains(tempSeason, new SeasonComparerByID()) == false)
					{
						newSerieSeasonCount++;
						newSeasons.Add(tempSeason);
					}
				}
				foreach (SerieSeason localSeason in listOfSerSeasons)
					if (onlineSeasonList.Contains(localSeason, new SeasonComparerByID()) == false)
					{
						countDeletedSeasons++;
						DatabaseManager.DeleteSerieSeason(localSeason);
					}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (newSerieSeasonCount > 0)
					{
						statusBarTextBlock.Text = "Nove sezone";
						statusBarProgressBar.Maximum = newSerieSeasonCount;
						statusBarProgressBar.Value = 0;
					}
				}));
				foreach (SerieSeason tempSeason in newSeasons)
				{
					DatabaseManager.InsertSerieSeasonSelection(tempSeason);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Nove sezone ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();

				#region Insert/update episodes

				int newSerieEpisodeCount = 0;
				int updatedEpisodesCount = 0;
				int countDeletedEpisodes = 0;
				List<SerieEpisode> newEpisodes = new List<SerieEpisode>();
				List<SerieEpisode> updatedEpisodes = new List<SerieEpisode>();

				foreach (SerieEpisode onlineEpisode in onlineEpisodeList)
				{
					SerieEpisode tempEpisode = FinderInCollection.FindInSerieEpisodeCollection(this.listOfSerEpisodes, onlineEpisode.ID);
					if (tempEpisode == null)
					{
						newSerieEpisodeCount++;
						newEpisodes.Add(onlineEpisode);
					}
					else if (onlineEpisode.Version > tempEpisode.Version)
					{
						updatedEpisodes.Add(onlineEpisode);
						updatedEpisodesCount++;
					}
				}
				foreach (SerieEpisode localEpisode in listOfSerEpisodes)
					if (onlineEpisodeList.Contains(localEpisode, new EpisodeComparerByID()) == false)
					{
						countDeletedEpisodes++;
						DatabaseManager.DeleteSerieEpisode(localEpisode);
					}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (newSerieEpisodeCount > 0)
					{
						statusBarTextBlock.Text = "Nove episode";
						statusBarProgressBar.Maximum = newSerieEpisodeCount;
						statusBarProgressBar.Value = 0;
					}
				}));
				foreach (SerieEpisode onlineEpisode in newEpisodes)
				{
					DatabaseManager.InsertSerieEpisodeForSelection(onlineEpisode);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Nove episode ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				this.Dispatcher.Invoke(new Action(() =>
				{
					if (updatedEpisodesCount > 0)
					{
						statusBarTextBlock.Text = "Ažurirane episode";
						statusBarProgressBar.Maximum = updatedEpisodesCount;
						statusBarProgressBar.Value = 0;
					}
				}));
				foreach (SerieEpisode onlineEpisode in updatedEpisodes)
				{
					DatabaseManager.UpdateSerieEpisode(onlineEpisode);
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarProgressBar.Value++;
						statusBarTextBlock.Text = string.Format("Ažurirane episode ({0}/{1})", statusBarProgressBar.Value, statusBarProgressBar.Maximum);
					}));
					if (closing)
					{
						ThreadCheckUpdates.Abort();
					}
				}

				#endregion

				if (closing)
					ThreadCheckUpdates.Abort();


				#region Show Changes

				if (newMovieCount == 0 && newSerieCount == 0 && newSerieSeasonCount == 0 && newSerieEpisodeCount == 0 && updatedMoviesCount == 0 && updatedEpisodesCount == 0 &&
					countDeletedAudios == 0 && countDeletedEpisodes == 0 && countDeletedHDDs == 0 && countDeletedLangs == 0 && countDeletedMovies == 0 && countDeletedPersons == 0 &&
					countDeletedSeasons == 0 && countDeletedSeries == 0)
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarTextBlock.Text = "Nema novih filmova/serija";
						statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
						updatingBusyIndicator.IsBusy = false;
					}));
					return;
				}

				this.Dispatcher.Invoke(new Action(() =>
				{
					Xceed.Wpf.Toolkit.MessageBox.Show(string.Format("Unesene promjene u katalog:\r\n\r\nNovi filmovi: {0}\r\nNove serije: {1}\r\nNove sezone: {2}\r\nNove episode: {3}\r\n\r\n" +
						"Ažurirani filmovi: {4}\r\nAžurirane epizode: {5}\r\n\r\n" +
						"Obrisani filmovi : {6}\r\nObrisane serije: {7}\r\nObrisane sezone: {8}\r\nObrisane episode: {9}\r\n" +
						"\r\nPotrebno restatati aplikaciju",
						newMovieCount, newSerieCount, newSerieSeasonCount, newSerieEpisodeCount,
						updatedMoviesCount, updatedEpisodesCount,
						countDeletedMovies, countDeletedSeries, countDeletedSeasons, countDeletedEpisodes),
						"Katalog osvježen", MessageBoxButton.OK, MessageBoxImage.Information);
				}));

				#endregion

				#endregion

				this.Dispatcher.Invoke(new Action(() =>
				{
					busyIndicator.IsBusy = false;
					statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
					updatingBusyIndicator.IsBusy = false;
					statusBarTextBlock.Text = "";
				}));
			}
			catch (Exception e)
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					statusBarTextBlock.Text = "Dohvat novih podataka nije uspio (" + e.Message + ")";
					statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
					updatingBusyIndicator.IsBusy = false;
					DateTime now = DateTime.Now;
					busyIndicator.IsBusy = false; while ((DateTime.Now - now).Milliseconds > 7000) ;
				}));
			}
		}
		private void CheckForAppUpdate()
		{
			decimal currentVersion = DatabaseManager.ThisVersionNumber();
			decimal newestVersion = DatabaseManagerMySql.NewestVersionNumber();
			if (newestVersion > currentVersion)
			{
				try
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarTextBlock.Text = string.Format("Skidanje nove verzije programa (v{0}, 3,5 MB)", newestVersion.ToString().Replace(",", "."));
						updatingBusyIndicator.IsBusy = true;
					}));
					HelpFunctions.DownloadNewVersionFTP();
					DatabaseManager.InsertnewVersion(newestVersion, "");
					if (closing)
						ThreadCheckUpdates.Abort();
					MessageBox.Show("Update uspješno napravljen. Potrebno restartati aplikaciju");
					this.Dispatcher.Invoke(new Action(() =>
					{
						updatingBusyIndicator.IsBusy = false;
					}));
				}
				catch (Exception ex)
				{
					if (closing)
						ThreadCheckUpdates.Abort();
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarTextBlock.Text = "Skidanje nove verzije programa nije uspjelo.\r\n" + ex.Message;
						updatingBusyIndicator.IsBusy = false;
					}));
					DateTime now = DateTime.Now;
					while ((DateTime.Now - now).Milliseconds > 10000) ;
					this.Dispatcher.Invoke(new Action(() =>
					{
						statusBarTextBlock.Text = "";
					}));
				}
			}
		}
		private void DownloadMissingPosters()
		{
			foreach (Movie tempMovie in listOfMovies)
			{
				if (closing)
					ThreadCheckUpdates.Abort();
				if (File.Exists(HardcodedStrings.moviePosterFolder + tempMovie.Name + ".jpg") == false)
				{
					try
					{
						HelpFunctions.DownloadPhotoFTP(tempMovie.Name, GlobalCategory.movie);
					}
					catch
					{
					}
				}
			}
			foreach (Serie tempSerie in listOfSeries)
			{
				if (closing)
					ThreadCheckUpdates.Abort();
				if (File.Exists(HardcodedStrings.seriePosterFolder + tempSerie.Name + ".jpg") == false)
				{
					try
					{
						HelpFunctions.DownloadPhotoFTP(tempSerie.Name, GlobalCategory.serie);
					}
					catch
					{
					}
				}
			}
			try
			{
				HelpFunctions.DownloadPhotoFTP("", GlobalCategory.homeVideo);   //za default.png, ne mogu poslati null pa saljem homevideo, glavno da nije movie ili serie
			}
			catch
			{
			}
			ThreadCheckUpdates.Abort();
		}

		#endregion

		#region Animation & sound

		private void AnimateSelectHddSizeUp()
		{
			var stb = (Storyboard)TryFindResource("Select HDDsize - close");
			showingSelectHddSize = false;
			if (stb != null)
			{
				stb.Begin();
				System.Media.SoundPlayer player = new System.Media.SoundPlayer(VideoKatalog.View.Properties.Resources.close5);
				player.Play();
			}
		}
		private void AnimateSelectHddSizeDown()
		{
			var stb = (Storyboard)TryFindResource("Select HDD size - down");
			showingSelectHddSize = true;
			if (stb != null)
			{
				stb.Begin();
				System.Media.SoundPlayer player = new System.Media.SoundPlayer(VideoKatalog.View.Properties.Resources.open2);
				player.Play();
			}
		}

		private void playOpenSound(object sender, MouseButtonEventArgs e)
		{
			System.Media.SoundPlayer player = new System.Media.SoundPlayer(VideoKatalog.View.Properties.Resources.open2);
			player.Play();
			ChangeDefaultButton(sender);
		}
		private void playCloseSound(object sender, RoutedEventArgs e)
		{
			System.Media.SoundPlayer player = new System.Media.SoundPlayer(VideoKatalog.View.Properties.Resources.close5);
			player.Play();
			ChangeDefaultButton(sender);
		}

		#endregion

		#region HDD

		bool showingSelectHddSize = false;
		private void selectSizeBTN_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			if (showingSelectHddSize)
			{
				AnimateSelectHddSizeUp();
			}
			else
			{
				AnimateSelectHddSizeDown();
			}
		}
		private void changeHddSize(object sender, MouseButtonEventArgs e)
		{
			TextBlock clickedTB = (TextBlock)sender;
			try
			{
				if (clickedTB.Name.Contains("250GB"))
				{
					customerHDD.ChangeCapacity(249000000000);
					hddSizeLabel.Text = "Veličina HDDa:   250 GB";
				}
				else if (clickedTB.Name.Contains("500GB"))
				{
					customerHDD.ChangeCapacity(499000000000);
					hddSizeLabel.Text = "Veličina HDDa:   500 GB";
				}
				else if (clickedTB.Name.Contains("640GB"))
				{
					customerHDD.ChangeCapacity(639000000000);
					hddSizeLabel.Text = "Veličina HDDa:   640 GB";
				}
				else if (clickedTB.Name.Contains("1TB"))
				{
					customerHDD.ChangeCapacity(999000000000);
					hddSizeLabel.Text = "Veličina HDDa:   1 TB";
				}
				else if (clickedTB.Name.Contains("1_5TB"))
				{
					customerHDD.ChangeCapacity(1490000000000);
					hddSizeLabel.Text = "Veličina HDDa:   1,5 TB";
				}
				else if (clickedTB.Name.Contains("2TB"))
				{
					customerHDD.ChangeCapacity(1990000000000);
					hddSizeLabel.Text = "Veličina HDDa:   2 TB";
				}
				else if (clickedTB.Name.Contains("3TB"))
				{
					customerHDD.ChangeCapacity(2990000000000);
					hddSizeLabel.Text = "Veličina HDDa:   3 TB";
				}
				filledProgressBar.Maximum = customerHDD.Capacity;
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
				PreviousHDDSizeAnimation();
				return;
			}

			if (clickedTB.Name.Contains("250GB"))
			{
				if (selectedSize > 249000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 249000000000;
				hddSizeLabel.Text = "Veličina HDDa:   250 GB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("500GB"))
			{
				if (selectedSize > 499000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 499000000000;
				hddSizeLabel.Text = "Veličina HDDa:   500 GB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("640GB"))
			{
				if (selectedSize > 639000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 639000000000;
				hddSizeLabel.Text = "Veličina HDDa:   640 GB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("1TB"))
			{
				if (selectedSize > 999000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 999000000000;
				hddSizeLabel.Text = "Veličina HDDa:   1 TB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("1_5TB"))
			{
				if (selectedSize > 1490000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 1490000000000;
				hddSizeLabel.Text = "Veličina HDDa:   1,5 TB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("2TB"))
			{
				if (selectedSize > 1990000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 1990000000000;
				hddSizeLabel.Text = "Veličina HDDa:   2 TB";
				filledProgressBar.Maximum = hddSize;
			}
			else if (clickedTB.Name.Contains("3TB"))
			{
				if (selectedSize > 2990000000000)
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Premala veličina HDDa s obzirom na odabrano. Potrebno smanjiti odabir", "Nedovoljno mjesta", MessageBoxButton.OK, MessageBoxImage.Error);
					PreviousHDDSizeAnimation();
					return;
				}
				hddSize = 2990000000000;
				hddSizeLabel.Text = "Veličina HDDa:   3 TB";
				filledProgressBar.Maximum = hddSize;
			}
			RefreshStatistics();
		}
		private string PreviousHDDSizeAnimation()
		{
			if (hddSize == 0)
			{
			}
			else if (hddSize == 0)
			{
			}
			if (hddSize == 0)
			{
			}
			if (hddSize == 0)
			{
			}
			return null;
		}

		#endregion

		public void RefreshStatistics()
		{
			Converters.SizeConverter sizeConv = new Converters.SizeConverter();
			remainingSizeLabel.Text = sizeConv.Convert(hddSize - selectedSize, typeof(string), null, null).ToString();
			selectedSizeLabel.Text = sizeConv.Convert(selectedSize, typeof(string), null, null).ToString();
			filledProgressBar.Value = selectedSize;
		}
		private void SetRatingCanvas(Canvas canvas, decimal rating)
		{
			float ratingFloat = float.Parse(rating.ToString());
			System.Drawing.Bitmap ratingImageBitmap = VideoKatalog.View.Properties.Resources.ratingStarsSmall;
			MemoryStream ms = new MemoryStream();
			ratingImageBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
			BitmapImage ratingStars = new BitmapImage();
			ratingStars.BeginInit();
			ratingStars.StreamSource = new MemoryStream(ms.ToArray());
			ratingStars.EndInit();
			ImageBrush myBrush = new ImageBrush();
			myBrush.ImageSource = ratingStars;
			myBrush.TileMode = TileMode.Tile;
			myBrush.Stretch = Stretch.None;
			int fullWidth = (int)this.ratingMask.Width;
			int width = (int)(ratingFloat * fullWidth / 10);
			myBrush.Viewport = new Rect(0, 0, 10 / ratingFloat * 0.2021, 1);

			System.Windows.Shapes.Rectangle newRect = new System.Windows.Shapes.Rectangle();
			newRect.Height = 20;
			newRect.Width = width;	 //(ratingFloat * this.ratingMask.Width / 10)
			newRect.Stroke = null;
			newRect.Fill = myBrush;

			canvas.Children.Clear();
			canvas.Children.Add(newRect);
		}

		#region Set Default Buttons

		bool serieTabFocused = false;
		private void tabMovies_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			serieTabFocused = false;

			SetMovieHotkeys();
		}
		private void tabSeries_GotFocus(object sender, System.Windows.RoutedEventArgs e)
		{
			RoutedCommand command = new RoutedCommand();
			InputBinding ib = new InputBinding(command, new KeyGesture(Key.F, ModifierKeys.Control));
			this.InputBindings.Add(ib);
			// Bind handler
			CommandBinding cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(OpenSerieSearch);
			this.CommandBindings.Add(cb);


			if (serieTabFocused)
				return;
			try
			{
				serieTabFocused = true;
			}
			catch
			{
			}
		}

		private void movieListBox_GotFocus(object sender, RoutedEventArgs e)
		{
			try
			{
				//addRemoveMovie2Default.IsDefault = true;
				//addRemoveSerie2Default.IsDefault = false;
			}
			catch
			{
			}
		}
		private void serieTreeView_GotFocus(object sender, RoutedEventArgs e)
		{
			try
			{
				//addRemoveMovie2Default.IsDefault = false;
				//addRemoveSerie2Default.IsDefault = true;
			}
			catch
			{
			}
		}
		private void ChangeDefaultButton(object sender)
		{
			foreach (Button btn in potentialDefaultButtons)
			{
				btn.IsDefault = false;
				try
				{
					Image pressedImage = sender as Image;
					if (pressedImage.Name == "movieDockSearch" && btn == movieSearchGridClose)
						btn.IsDefault = true;
					else if (pressedImage.Name == "movieDockSort" && btn == closeMovieSortGrid)
						btn.IsDefault = true;
					else if (pressedImage.Name == "movieDockView" && btn == closeMovieViewTheme)
						btn.IsDefault = true;
					//else if (pressedImage.Name == "serieDockSearch" && btn == serieSearchGridClose) {
					//    btn.IsDefault = true;
					//    btn.Focus ();
					//}
					//else if (pressedImage.Name == "serieDockSort" && btn == closeSerieSortGrid) {
					//    btn.IsDefault = true;
					//    btn.Focus ();
					//}

					//return;
					continue;
				}
				catch
				{
				}
				try
				{
					Button pressedBtn = sender as Button;
					//if (pressedBtn.Name == "movieSearchGridClose" || pressedBtn.Name == "closeMovieSortGrid" || pressedBtn.Name == "closeMovieViewTheme") {
					//    if (btn == addRemoveMovie2Default) {
					//        btn.IsDefault = true;
					//        //movieListBox.Focus ();
					//    }
					//}
					//else if (pressedBtn.Name == "serieSearchGridClose" || pressedBtn.Name == "closeSerieSortGrid") {
					//    if (btn == addRemoveSerie2Default) {
					//        //serieTreeView.Focus ();
					//        btn.IsDefault = true;
					//    }
					//}
					continue;
				}
				catch
				{
				}
			}
		}

		#endregion

		#region MOVIE

		#region Global Data

		private ObservableCollection<Movie> listOfMovies = new ObservableCollection<Movie>();
		private ObservableCollection<Movie> moviesShowingList = new ObservableCollection<Movie>();
		CollectionView _movieView;

		//bool showAllMovies = false;
		Movie focusedSelectedMovie;
		Movie focusedNotSelectedMovie;
		bool showOnlySelectedMovies = false;
		bool showOnlyNotSelectedMovies = true;

		#endregion

		private void addRemoveMovie_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Movie selectedMovie = (Movie)_movieView.CurrentItem;
				int currentPosition = _movieView.CurrentPosition;
				if (selectedMovie == null)
					return;

				notSaved = true;
				//micanje filma iz liste odabranog
				if (selectedMovie.Selected)
				{
					selectedMovie.Selected = false;
					selectedSize -= selectedMovie.Size;
					customerHDD.FreeSpaceIncrease(selectedMovie.Size);
				}

				//dodavanje filma u listu odabranog
				else
				{
					if (selectedMovie.Size > (hddSize - selectedSize))
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("Nedovoljno mjesta na HDDu", "Preveliko", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					selectedMovie.Selected = true;
					selectedSize += selectedMovie.Size;
					customerHDD.FreeSpaceDecrease(selectedMovie.Size);
				}
				moviesShowingList.Remove(selectedMovie);
				RefreshStatistics();

				if (_movieView.Count < currentPosition)
				{
					_movieView.MoveCurrentToPrevious();
				}
				else;
				ListBoxItem item = (ListBoxItem)movieListBox.ItemContainerGenerator.ContainerFromIndex(movieListBox.SelectedIndex);
				if (item != null)
					item.Focus();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
		#region Change Add Remove ButtonStyle

		private void addRemoveMovie_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (showOnlyNotSelectedMovies)
			{
				var style = (Style)TryFindResource("addBtnNormal");
				addRemoveMovie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnNormal");
				addRemoveMovie.Style = style;
			}
		}
		private void addRemoveMovie_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (showOnlyNotSelectedMovies)
			{
				var style = (Style)TryFindResource("addBtnClick");
				addRemoveMovie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnClick");
				addRemoveMovie.Style = style;
			}
		}
		private void addRemoveMovie_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			addRemoveMovie_Click(addRemoveMovie, null);
			if (showOnlyNotSelectedMovies)
			{
				var style = (Style)TryFindResource("addBtnNormal");
				addRemoveMovie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnNormal");
				addRemoveMovie.Style = style;
			}
		}

		#endregion

		private void SetMovieSlides()
		{
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
			foreach (Movie tempMovie in listOfMovies)
			{
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

			movieYearSlider.Minimum = (double)minMovieYear;
			movieYearSlider.LowerValue = movieYearSlider.Minimum;
			movieYearSlider.Maximum = (double)maxMovieYear;
			movieYearSlider.UpperValue = movieYearSlider.Maximum;

			movieSizeSlider.Minimum = (double)minMovieSize;
			movieSizeSlider.LowerValue = movieSizeSlider.Minimum;
			movieSizeSlider.Maximum = (double)maxMovieSize;
			movieSizeSlider.UpperValue = movieSizeSlider.Maximum;

			movieRuntimeSlider.Minimum = (double)minMovieRuntime;
			movieRuntimeSlider.LowerValue = movieRuntimeSlider.Minimum;
			movieRuntimeSlider.Maximum = (double)maxMovieRuntime;
			movieRuntimeSlider.UpperValue = movieRuntimeSlider.Maximum;

			movieBudgetSlider.Minimum = (double)minBudget;
			movieBudgetSlider.LowerValue = movieBudgetSlider.Minimum;
			movieBudgetSlider.Maximum = (double)maxBudget;
			movieBudgetSlider.UpperValue = movieBudgetSlider.Maximum;

			movieEarningsSlider.Minimum = (double)minEarnigns;
			movieEarningsSlider.LowerValue = movieEarningsSlider.Minimum;
			movieEarningsSlider.Maximum = (double)maxEarnings;
			movieEarningsSlider.UpperValue = movieEarningsSlider.Maximum;
		}

		#region Hotkeys

		private void SetMovieHotkeys()
		{
			//ctrl + f -> SEARCH
			RoutedCommand command = new RoutedCommand();
			InputBinding ib = new InputBinding(command, new KeyGesture(Key.F, ModifierKeys.Control));
			this.InputBindings.Add(ib);
			// Bind handler
			CommandBinding cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(OpenMovieSearch);
			this.CommandBindings.Add(cb);


			//shift + tab -> switch selected / not selected
			command = new RoutedCommand();
			ib = new InputBinding(command, new KeyGesture(Key.Tab, ModifierKeys.Shift));
			this.InputBindings.Add(ib);
			// Bind handler
			cb = new CommandBinding(command);
			cb.Executed += new ExecutedRoutedEventHandler(SwitchMovieSelectedNotSelected);
			this.CommandBindings.Add(cb);
		}
		private void OpenMovieSearch(object obSender, ExecutedRoutedEventArgs e)
		{
			var stb = (Storyboard)TryFindResource("movieSearchGridShow");
			showingSelectHddSize = false;
			if (stb != null)
			{
				stb.Begin();
				//System.Media.SoundPlayer player = new System.Media.SoundPlayer (VideoKatalog.View.Properties.Resources.close5);
				//player.Play ();
			}
			playOpenSound(movieDockSearch, null);
			searchMovieName.Focus();
		}
		private void SwitchMovieSelectedNotSelected(object obSender, ExecutedRoutedEventArgs e)
		{
			if (showOnlyNotSelectedMovies)
			{
				movieViewSelectedChosen_MouseLeftButtonUp(null, null);
				var stb = (Storyboard)TryFindResource("movieViewSelectedChosenSelect");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
					//System.Media.SoundPlayer player = new System.Media.SoundPlayer (VideoKatalog.View.Properties.Resources.close5);
					//player.Play ();
				}
			}
			else
			{
				var stb = (Storyboard)TryFindResource("movieViewSelectedNotChosenSelect");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
				}
				movieViewSelectedNotChosen_MouseLeftButtonUp(null, null);
			}
		}

		#endregion

		#region Movie Genres

		bool movieGenreGridExpanded = false;
		private void openCloseMovieGenreGrid_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (movieGenreGridExpanded)
			{
				var stb = (Storyboard)TryFindResource("movieGenreCollapse");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
				}
				movieGenreGridExpanded = false;
			}
			else
			{
				var stb = (Storyboard)TryFindResource("movieGenreExpand");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
				}
				movieGenreGridExpanded = true;
			}
		}
		private void movieGenreAll_Click(object sender, RoutedEventArgs e)
		{
			SetMovieGenreCheckboxes(true);
		}
		private void movieGenreNone_Click(object sender, RoutedEventArgs e)
		{
			SetMovieGenreCheckboxes(false);
		}
		private void SetMovieGenreCheckboxes(bool value)
		{
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

		#endregion

		private void movieListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Movie selectedMovie = (Movie)_movieView.CurrentItem;// movieListBox.SelectedItem;
			if (_movieView.Count == 0)
			{
				movieSubCro.Visibility = System.Windows.Visibility.Hidden;
				movieSubEng.Visibility = System.Windows.Visibility.Hidden;
				movieRez1080p.Visibility = System.Windows.Visibility.Hidden;
				movieRez720p.Visibility = System.Windows.Visibility.Hidden;
				movieRezSD.Visibility = System.Windows.Visibility.Hidden;
				SetRatingCanvas(movieRatingCanvas, 0);
			}
			else
			{
				movieSubCro.Visibility = System.Windows.Visibility.Visible;
				movieSubEng.Visibility = System.Windows.Visibility.Visible;
				movieRez1080p.Visibility = System.Windows.Visibility.Visible;
				movieRez720p.Visibility = System.Windows.Visibility.Visible;
				movieRezSD.Visibility = System.Windows.Visibility.Visible;
			}
			if (selectedMovie == null)
				return;

			SetRatingCanvas(movieRatingCanvas, selectedMovie.Rating);
			if (showOnlySelectedMovies)
				focusedSelectedMovie = selectedMovie;
			else
				focusedNotSelectedMovie = selectedMovie;
		}
		private void movieGoToImdb_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(((Movie)_movieView.CurrentItem).InternetLink);
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška!");
			}
		}
		private void movieGoToYoutube_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				System.Diagnostics.Process.Start(((Movie)_movieView.CurrentItem).TrailerLink);
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška!");
			}
		}

		private void ChangeMovieListTemplate(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			busyIndicator.IsBusy = true;
			busyIndicator.BusyContent = "Postavljanje teme";
			TextBlock selected = sender as TextBlock;

			ThreadStart starter = delegate
			{
				DateTime now = DateTime.Now;
				while ((DateTime.Now - now).Milliseconds < 300) ;
				string buttonName = "";
				this.Dispatcher.Invoke(new Action(() =>
				{
					buttonName = selected.Name;
				}));

				if (buttonName.Contains("Simple"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						try
						{
							var stb = (DataTemplate)TryFindResource("movieListTemplate");
							var panelTemplate = (ItemsPanelTemplate)TryFindResource("movieListSimplePanel");
							movieListBox.ItemTemplate = stb;
							movieListBox.ItemsPanel = panelTemplate;
						}
						catch
						{
							MessageBox.Show("Nije uspjelo mijenjanje teme");
							this.Dispatcher.Invoke(new Action(() =>
							{
								movieListBox.UpdateLayout();
								movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));	 //+4 da bas ne bude prvi dolje
								busyIndicator.IsBusy = false;
							}));
						}
					}));
				}
				else if (buttonName.Contains("Adv"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						try
						{
							var stb = (DataTemplate)TryFindResource("movieListTemplate2");
							var panelTemplate = (ItemsPanelTemplate)TryFindResource("movieListSimplePanel");
							movieListBox.ItemTemplate = stb;
							movieListBox.ItemsPanel = panelTemplate;
						}
						catch
						{
							MessageBox.Show("Nije uspjelo mijenjanje teme");
							this.Dispatcher.Invoke(new Action(() =>
							{
								movieListBox.UpdateLayout();
								movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));	 //+4 da bas ne bude prvi dolje
								busyIndicator.IsBusy = false;
							}));
						}
					}));
				}
				else if (buttonName.Contains("Poster"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						try
						{
							var stb = (DataTemplate)TryFindResource("movieListTemplatePoster");
							var panelTemplate = (ItemsPanelTemplate)TryFindResource("movielistboxPanelTemplate");
							movieListBox.ItemTemplate = stb;
							movieListBox.ItemsPanel = panelTemplate;
						}
						catch
						{
							MessageBox.Show("Nije uspjelo mijenjanje teme");
							this.Dispatcher.Invoke(new Action(() =>
							{
								movieListBox.UpdateLayout();
								movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));	 //+4 da bas ne bude prvi dolje
								busyIndicator.IsBusy = false;
							}));
						}
					}));
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					movieListBox.UpdateLayout();
					try
					{
						movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));	 //+4 da bas ne bude prvi dolje
					}
					catch
					{
					}
					busyIndicator.IsBusy = false;

				}));
			};
			Thread SetThemeThread = new Thread(starter);
			SetThemeThread.IsBackground = true;
			SetThemeThread.Start();
		}

		#region filters & sorting

		private void movieSearchGridClose_Click(object sender, RoutedEventArgs e)
		{
			Movie selectedMovie = _movieView.CurrentItem as Movie;
			playCloseSound(null, null);
			_movieView.Filter = FilterMovies;
			try
			{
				_movieView.MoveCurrentTo(selectedMovie);
				movieListBox.UpdateLayout();
				ListBoxItem item = (ListBoxItem)movieListBox.ItemContainerGenerator.ContainerFromIndex(movieListBox.SelectedIndex);
				if (item != null)
					item.Focus();
			}
			catch
			{
				_movieView.MoveCurrentToPosition(0);
				movieListBox.UpdateLayout();
				ListBoxItem item = (ListBoxItem)movieListBox.ItemContainerGenerator.ContainerFromIndex(movieListBox.SelectedIndex);
				if (item != null)
					item.Focus();
				movieListBox.ScrollIntoView(movieListBox.Items[0]);
			}
			movieListBox.Focus();
			//foreach (Movie tempMovie in listOfMovies)
			//    if (movieListBox.Items.Contains (tempMovie) == false)
			//        MessageBox.Show (tempMovie.Name);
		}
		private bool FilterMovies(object movie)
		{
			Movie tempMovie = movie as Movie;
			if (tempMovie == null)
				return false;
			if (tempMovie.Name == "Einstein")
			{
				int bla = 0;
			}
			//CheckWhoIsntOnList ();

			//NAZIV
			if (tempMovie.OrigName.ToLower().Contains(searchMovieName.Text.ToLower()) == false && tempMovie.Name.ToLower().Contains(searchMovieName.Text.ToLower()) == false)
				return false;

			#region Slideri

			//Godina
			if ((double)tempMovie.Year < movieYearSlider.LowerValue || (double)tempMovie.Year > movieYearSlider.UpperValue)
				return false;

			//Ocjena
			if ((double)tempMovie.Rating < movieRatingSlider.LowerValue || (double)tempMovie.Rating > movieRatingSlider.UpperValue)
				return false;

			//Veličina
			if ((double)tempMovie.Size < movieSizeSlider.LowerValue || (double)tempMovie.Size > movieSizeSlider.UpperValue)
				return false;

			//Trajanje
			if (tempMovie.Runtime < movieRuntimeSlider.LowerValue || tempMovie.Runtime > movieRuntimeSlider.UpperValue)
				return false;

			//Budget
			if (tempMovie.Budget < movieBudgetSlider.LowerValue || tempMovie.Runtime > movieBudgetSlider.UpperValue)
				return false;

			//Zarada
			if (tempMovie.Earnings < movieEarningsSlider.LowerValue || tempMovie.Runtime > movieEarningsSlider.UpperValue)
				return false;

			#endregion

			#region Glumci / Redatelji

			//Glumci
			if (searchMovieCast.Text.Length > 0)
			{
				bool foundActor = false;
				foreach (Person actor in tempMovie.Actors)
					if (actor.Name.ToLower().Contains(searchMovieCast.Text.ToLower()))
						foundActor = true;
				if (foundActor == false)
					return false;
			}

			//Redatelji
			if (searchMovieDirector.Text.Length > 0)
			{
				bool foundDirector = false;
				foreach (Person director in tempMovie.Directors)
					if (director.Name.ToLower().Contains(searchMovieDirector.Text.ToLower()))
						foundDirector = true;
				if (foundDirector == false)
					return false;
			}

			#endregion

			#region Rezolucija

			//Rezolucija
			Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter();
			string resolution = resolutionConverter.Convert(tempMovie.Width, typeof(string), null, null).ToString();
			if (resolution == "SD" && rezSD.IsChecked == false)
				return false;
			if (resolution == "720p" && rez720p.IsChecked == false)
				return false;
			if (resolution == "1080p" && rez1080p.IsChecked == false)
				return false;

			#endregion

			#region Prijevod

			//Prijevod
			Language cro = new Language();
			cro.Name = "Croatian";
			Language eng = new Language();
			eng.Name = "English";
			if (subCro.IsChecked == true && tempMovie.SubtitleLanguageList.Contains(cro, new LanguageComparerByName()) == false)
				return false;
			if (subEng.IsChecked == true && tempMovie.SubtitleLanguageList.Contains(eng, new LanguageComparerByName()) == false)
				return false;

			#endregion

			#region Žanrovi

			bool foundOneGenre = false;
			foreach (Genre tempGenre in tempMovie.Genres)
			{
				if (tempGenre.Name == "Akcija" && actionCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Avantura" && adventureCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Animacija" && animationCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Biografija" && biographyCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Komedija" && comedyCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Krimi" && crimeCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Dokumentarni" && documentaryCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Drama" && dramaCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Obiteljski" && familyCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Fantastika" && fantasyCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Crno-bijeli" && filmNoirCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Povijesni" && historyCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Horor" && horrorCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Muzički" && musicCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Pjevan" && musicalCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Misterija" && misteryCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Romantika" && romanceCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Znanstvena fantastika" && sciFiCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Sportski" && sportCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Triler" && thrillerCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Ratni" && warCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Vestern" && westernCheckBox.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
			}

			if (foundOneGenre == false)
				return false;

			#endregion

			#region Odabrano / Neodabrano

			if (tempMovie.Selected && (showOnlyNotSelectedMovies))
			{
				return false;
			}
			else if (tempMovie.Selected == false && showOnlySelectedMovies)
			{
				return false;
			}

			#endregion

			if (tempMovie.Show == false)
				return false;

			//Ako je sve proslo
			return true;
		}
		private void CheckWhoIsntOnList()
		{
			foreach (Movie tempMovie in listOfMovies)
			{
				bool found = false;
				foreach (Movie tempMovie2 in movieListBox.Items)
				{
					if (tempMovie.Name == tempMovie2.Name)
					{
						found = true;
						break;
					}
				}
				if (found == false)
					MessageBox.Show(tempMovie.Name);
			}
		}

		private void SortMovies(object sender, MouseButtonEventArgs e)
		{
			SortDescription movieSort = new SortDescription();
			TextBlock clickedTB = sender as TextBlock;
			if (movieSortDescending.IsChecked == true)
				movieSort.Direction = ListSortDirection.Descending;
			else
				movieSort.Direction = ListSortDirection.Ascending;

			if (clickedTB.Name.Contains("Year"))
			{
				movieSort.PropertyName = "Year";
			}
			else if (clickedTB.Name.Contains("Rating"))
			{
				movieSort.PropertyName = "Rating";
			}
			else if (clickedTB.Name.Contains("Runtime"))
			{
				movieSort.PropertyName = "Runtime";
			}
			else if (clickedTB.Name.Contains("Size"))
			{
				movieSort.PropertyName = "Size";
			}
			else if (clickedTB.Name.Contains("Name"))
			{
				movieSort.PropertyName = "Name";
			}
			else if (clickedTB.Name.Contains("AddDate"))
			{
				movieSort.PropertyName = "AddTime";
			}


			//cuvaj samo 3 sortdescriptionsa
			for (int i = 2; i < _movieView.SortDescriptions.Count; i++)
				_movieView.SortDescriptions.RemoveAt(i);


			//pomakni vaznost sortova za svaki za jedan manje
			try
			{
				_movieView.SortDescriptions.Add(movieSort);
				_movieView.SortDescriptions[2] = _movieView.SortDescriptions[1];
				_movieView.SortDescriptions[1] = _movieView.SortDescriptions[0];
			}
			catch
			{
				try
				{
					_movieView.SortDescriptions[1] = _movieView.SortDescriptions[0];
				}
				catch
				{
				}
			}

			//makni sort ako postoji vec
			for (int i = 1; i < _movieView.SortDescriptions.Count; i++)
			{
				SortDescription existing = _movieView.SortDescriptions.ElementAt(i);
				if (existing.PropertyName == movieSort.PropertyName)
					_movieView.SortDescriptions.Remove(existing);
			}

			_movieView.SortDescriptions[0] = movieSort;
		}
		private void ChangeSortingDirection(object sender, RoutedEventArgs e)
		{
			try
			{
				SortDescription tempSort = new SortDescription();
				tempSort.PropertyName = _movieView.SortDescriptions[0].PropertyName;
				string senderName = ((System.Windows.Controls.Primitives.ToggleButton)sender).Name;
				if (senderName.ToLower().Contains("ascending"))
					tempSort.Direction = ListSortDirection.Ascending;
				else
					tempSort.Direction = ListSortDirection.Descending;
				_movieView.SortDescriptions[0] = tempSort;
			}
			catch
			{
				return;
			}
		}

		#endregion

		#region view selected / not selected

		private void movieViewSelectedNotChosen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			movieListBox.Background = Brushes.White;
			var style = (Style)TryFindResource("addBtnNormal");
			addRemoveMovie.Style = style;

			showOnlyNotSelectedMovies = true;
			showOnlySelectedMovies = false;

			moviesShowingList.Clear();
			movieListBox.UpdateLayout();
			foreach (Movie tempMovie in listOfMovies)
				if (tempMovie.Selected == false)
					moviesShowingList.Add(tempMovie);
			_movieView.Refresh();
			movieListBox.Focus();

			if (focusedNotSelectedMovie == null)
				return;

			_movieView.MoveCurrentTo(focusedNotSelectedMovie);

			movieListBox.UpdateLayout();
			try
			{
				movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));//+4 da bas ne bude prvi dolje
				ListBoxItem item = (ListBoxItem)movieListBox.ItemContainerGenerator.ContainerFromItem(_movieView.CurrentItem);
				item.Focus();
			}
			catch
			{
			}
		}
		private void movieViewSelectedChosen_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			BrushConverter bc = new BrushConverter();
			movieListBox.Background = (Brush)bc.ConvertFrom("#FFE9FFFF");
			var style = (Style)TryFindResource("removeBtnNormal");
			addRemoveMovie.Style = style;

			showOnlyNotSelectedMovies = false;
			showOnlySelectedMovies = true;

			moviesShowingList.Clear();
			movieListBox.UpdateLayout();
			foreach (Movie tempMovie in listOfMovies)
				if (tempMovie.Selected)
					moviesShowingList.Add(tempMovie);
			_movieView.Refresh();
			movieListBox.Focus();

			if (focusedSelectedMovie == null)
				return;

			_movieView.MoveCurrentTo(focusedSelectedMovie);

			movieListBox.UpdateLayout();
			try
			{
				movieListBox.ScrollIntoView(_movieView.GetItemAt(_movieView.CurrentPosition + 4));	 //+4 da bas ne bude prvi dolje
				ListBoxItem item = (ListBoxItem)movieListBox.ItemContainerGenerator.ContainerFromItem(_movieView.CurrentItem);
				item.Focus();
			}
			catch
			{
			}
		}

		#endregion

		#endregion

		#region SERIE

		#region Global Data

		private ObservableCollection<Serie> listOfSeries = new ObservableCollection<Serie>();
		private ObservableCollection<SerieSeason> listOfSerSeasons = new ObservableCollection<SerieSeason>();
		private ObservableCollection<SerieEpisode> listOfSerEpisodes = new ObservableCollection<SerieEpisode>();

		CollectionView _serieView;
		CollectionView _serieSeasonView;
		CollectionView _serieEpisodeView;

		SerieCategory serieSelectedCategory = SerieCategory.none;

		bool showAllSeries = false;
		bool showOnlySelectedSeries = false;
		bool showOnlyNotSelectedSeries = true;

		#endregion

		#region Change Add Remove Button Style

		private void serieAddRemove_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			if (showOnlyNotSelectedSeries)
			{
				var style = (Style)TryFindResource("addBtnNormal");
				addRemoveSerie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnNormal");
				addRemoveSerie.Style = style;
			}
		}
		private void serieAddRemove_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			if (showOnlyNotSelectedSeries)
			{
				var style = (Style)TryFindResource("addBtnClick");
				addRemoveSerie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnClick");
				addRemoveSerie.Style = style;
			}

		}
		private void serieAddRemove_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			serieAddRemove_Click(null, null);
			if (showOnlyNotSelectedSeries)
			{
				var style = (Style)TryFindResource("addBtnNormal");
				addRemoveSerie.Style = style;
			}
			else
			{
				var style = (Style)TryFindResource("removeBtnNormal");
				addRemoveSerie.Style = style;
			}
		}

		#endregion

		private void SetSerieTabConstantBindings()
		{
			directorTBSerie.DataContext = _serieView;
			castTBSerie.DataContext = _serieView;
			genreLabelSerie.DataContext = _serieView;
			ratingLabelSerie.DataContext = _serieView;
			seriePoster.DataContext = _serieView;
		}
		private void FillSerieTreeViewFirstTime()
		{
			foreach (Serie tempSerie in listOfSeries)
			{
				TreeViewItem serie = new TreeViewItem();
				this.Dispatcher.Invoke(new Action(() =>
				{
					serie = new TreeViewItem() { Header = tempSerie.OrigName, DataContext = tempSerie };
				}));
				if (serieTreeView.Items.Count == 0)
					serie.IsSelected = true;
				serie.Foreground = System.Windows.Media.Brushes.Black;
				serie.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
				serie.FontSize = 16;
				foreach (SerieSeason tempSeason in tempSerie.Seasons)
				{
					TreeViewItem season = new TreeViewItem();
					this.Dispatcher.Invoke(new Action(() =>
					{
						season = new TreeViewItem() { Header = tempSeason.Name, DataContext = tempSeason };
					}));
					season.Foreground = System.Windows.Media.Brushes.Black;
					season.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
					season.FontSize = 14;
					foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
					{
						TreeViewItem episode = new TreeViewItem();
						this.Dispatcher.Invoke(new Action(() =>
						{
							episode = new TreeViewItem() { Header = tempEpisode.Name, DataContext = tempEpisode };
						}));
						episode.Foreground = System.Windows.Media.Brushes.Black;
						episode.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
						episode.FontSize = 13;
						season.Items.Add(episode);
					}
					serie.Items.Add(season);
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					serieTreeView.Items.Add(serie);
					if (serieTreeView.Items.Count == 1)
						serie.IsSelected = true;
				}));
			}
			this.Dispatcher.Invoke(new Action(() =>
			{
				numberTBSerie.Text = serieTreeView.Items.Count.ToString();
				serieTreeView.Focus();
				if (serieTreeView.Items.Count == 0)
					ResetSerieInfo();
			}));
		}
		private void FillSerieTreeView()
		{
			bool doesContinue = false;
			this.Dispatcher.Invoke(new Action(() =>
			{
				serieTreeView.Items.Clear();
			}));
			SetSerieSliders();
			foreach (Serie tempSerie in listOfSeries)
			{
				doesContinue = false;
				this.Dispatcher.Invoke(new Action(() =>
				{
					if (IsSerieAccepted(tempSerie) == false)
						doesContinue = true;
				}));
				if (doesContinue)
					continue;
				TreeViewItem serie = new TreeViewItem();
				this.Dispatcher.Invoke(new Action(() =>
				{
					serie = new TreeViewItem() { Header = tempSerie.OrigName, DataContext = tempSerie };
				}));
				if (serieTreeView.Items.Count == 0)
					serie.IsSelected = true;
				serie.Foreground = System.Windows.Media.Brushes.Black;
				serie.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
				serie.FontSize = 16;
				foreach (SerieSeason tempSeason in tempSerie.Seasons)
				{
					doesContinue = false;
					this.Dispatcher.Invoke(new Action(() =>
					{
						if (IsSeasonAccepted(tempSeason) == false)
							doesContinue = true;
					}));
					if (doesContinue)
						continue;

					TreeViewItem season = new TreeViewItem();
					this.Dispatcher.Invoke(new Action(() =>
					{
						season = new TreeViewItem() { Header = tempSeason.Name, DataContext = tempSeason };
					}));
					season.Foreground = System.Windows.Media.Brushes.Black;
					season.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
					season.FontSize = 14;
					foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
					{
						doesContinue = false;
						this.Dispatcher.Invoke(new Action(() =>
						{
							if (IsEpisodeAccepted(tempEpisode) == false)
								doesContinue = true;
						}));
						if (doesContinue)
							continue;
						TreeViewItem episode = new TreeViewItem();
						this.Dispatcher.Invoke(new Action(() =>
						{
							episode = new TreeViewItem() { Header = tempEpisode.Name, DataContext = tempEpisode };
						}));
						episode.Foreground = System.Windows.Media.Brushes.Black;
						episode.FontFamily = new System.Windows.Media.FontFamily("Estrangelo Edessa");
						episode.FontSize = 13;
						season.Items.Add(episode);
					}
					serie.Items.Add(season);
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					serieTreeView.Items.Add(serie);
					if (serieTreeView.Items.Count == 1)
						serie.IsSelected = true;
				}));

			}
			this.Dispatcher.Invoke(new Action(() =>
			{
				numberTBSerie.Text = serieTreeView.Items.Count.ToString();
				serieTreeView.Focus();
				if (serieTreeView.Items.Count == 0)
					ResetSerieInfo();
			}));
		}
		private void SetSerieSliders()
		{
			int maxSerieYear = 0;
			int minSerieYear = 9999;
			long maxSerieSize = 0;
			long minSerieSize = 999999999999999999;
			foreach (Serie tempSerie in listOfSeries)
			{
				if (tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year > maxSerieYear)
					maxSerieYear = tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year;
				if (tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year < minSerieYear)
					minSerieYear = tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year;
				long size = FetchSerieShowingSize(tempSerie);
				if (size > maxSerieSize)
					maxSerieSize = size;
				if (size < minSerieSize)
					minSerieSize = size;

				this.Dispatcher.Invoke(new Action(() =>
				{
					yearRangeSliderSerie.Minimum = (double)minSerieYear;
					yearRangeSliderSerie.Maximum = (double)maxSerieYear;
					yearRangeSliderSerie.UpperValue = yearRangeSliderSerie.Maximum;
					yearRangeSliderSerie.LowerValue = yearRangeSliderSerie.Minimum;

					sizeRangeSliderSerie.Minimum = (double)minSerieSize;
					sizeRangeSliderSerie.Maximum = (double)maxSerieSize;
					sizeRangeSliderSerie.UpperValue = sizeRangeSliderSerie.Maximum;
					sizeRangeSliderSerie.LowerValue = sizeRangeSliderSerie.Minimum;
				}));
			}
		}

		#region Select / Deselect

		private void serieAddRemove1_MouseLeftButtonDown(object sender, System.Windows.RoutedEventArgs e)
		{
			serieAddRemove_Click(null, null);
		}
		private void serieAddRemove_Click(object sender, RoutedEventArgs e)
		{
			TreeViewItem selectedItem = serieTreeView.SelectedItem as TreeViewItem;

			#region If Selected Serie

			if (serieSelectedCategory == SerieCategory.serie)
			{
				Serie selectedSerie = (Serie)selectedItem.DataContext;
				if (showOnlyNotSelectedSeries)
				{
					if (FetchSerieShowingSize(selectedSerie) > (hddSize - selectedSize))
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("Nedovoljno mjesta na HDDu.", "Preveliko", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					//ako se prikazuje ono sto nije odabrano, onda sve sto se prikazuje postavi da je odabrano
					SetAllSerieEpisodesSelectedState(selectedSerie, true);
					RemoveTreeViewItem(selectedItem, SerieCategory.serie);
					RefreshStatistics();
					//return;
				}
				else if (showOnlySelectedSeries)
				{
					//ako se prikazuje samo ono sto je odabrano, onda sve sto se prikazuje postavi da nije odabrano
					SetAllSerieEpisodesSelectedState(selectedSerie, false);
					RemoveTreeViewItem(selectedItem, SerieCategory.serie);
					RefreshStatistics();
					//return;
				}
				//ako se prikazuje sve, onda provjeri da li je u onome sto je selektirano sve odabrano ili postoji nesto sto nije odabrano
				int countNotSelectedEpisodes = 0;
				long notSelectedSize = 0;
				foreach (SerieSeason tempSeason in selectedSerie.Seasons)
					foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
						if (tempEpisode.Selected == false && IsEpisodeAccepted(tempEpisode))
						{
							notSelectedSize += tempEpisode.Size;
							countNotSelectedEpisodes++;
						}

				if (notSelectedSize > (hddSize - selectedSize))
				{
					Xceed.Wpf.Toolkit.MessageBox.Show("Nedovoljno mjesta na HDDu.", "Preveliko", MessageBoxButton.OK, MessageBoxImage.Warning);
					return;
				}

				if (countNotSelectedEpisodes == 0)
				{
					//ako je sve u onome sto je selektirano odabrano, postavi sve u onome sto je selektirano na odabrano = false;
					SetAllSerieEpisodesSelectedState(selectedSerie, false);
				}
				else
				{
					//ako u onome sto je selektirano postoji nesto sto nije odabrano, postavi sve u onome sto je selektirano na odabrano = true;
					SetAllSerieEpisodesSelectedState(selectedSerie, true);
				}
				RefreshStatistics();
			}

			#endregion

			#region If Selected Season

			else if (serieSelectedCategory == SerieCategory.season)
			{
				SerieSeason selectedSeason = (SerieSeason)selectedItem.DataContext;
				if (showOnlyNotSelectedSeries)
				{
					if (FetchSeasonShowingSize(selectedSeason) > (hddSize - selectedSize))
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("Nedovoljno mjesta na HDDu.", "Preveliko", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					SetAllSeasonEpisodesSelectedState(selectedSeason, true);
					RemoveTreeViewItem(selectedItem, SerieCategory.season);
					//FillSerieTreeView ();
					//return;
				}
				else if (showOnlySelectedSeries)
				{
					SetAllSeasonEpisodesSelectedState(selectedSeason, false);
					RemoveTreeViewItem(selectedItem, SerieCategory.season);
					//return;
				}
				int countNotSelectedEpisodes = 0;
				foreach (SerieEpisode tempEpisode in selectedSeason.Episodes)
					if (tempEpisode.Selected == false && IsEpisodeAccepted(tempEpisode))
						countNotSelectedEpisodes++;
				if (countNotSelectedEpisodes == 0)
					SetAllSeasonEpisodesSelectedState(selectedSeason, false);
				else
					SetAllSeasonEpisodesSelectedState(selectedSeason, true);
			}

			#endregion

			#region If Selected Episode

			else if (serieSelectedCategory == SerieCategory.episode)
			{
				SerieEpisode selectedEpisode = (SerieEpisode)selectedItem.DataContext;
				if (selectedEpisode.Selected)
				{
					selectedEpisode.Selected = false;
					selectedSize += selectedEpisode.Size;
				}
				else
				{
					if (selectedEpisode.Size > (hddSize - selectedSize))
					{
						Xceed.Wpf.Toolkit.MessageBox.Show("Nedovoljno mjesta na HDDu.", "Preveliko", MessageBoxButton.OK, MessageBoxImage.Warning);
						return;
					}
					selectedSize += selectedEpisode.Size;
					selectedEpisode.Selected = true;
				}
				if (showAllSeries == false)
					RemoveTreeViewItem(selectedItem, SerieCategory.episode);
			}

			#endregion

			#region If Selected None

			else
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Odaberi seriju");
				return;
			}

			#endregion


			notSaved = true;
			selectedItem.IsSelected = false;
			selectedItem.IsSelected = true;
			RefreshStatistics();
			numberTBSerie.Text = serieTreeView.Items.Count.ToString();
			serieTreeView.Focus();
			//FillSerieTreeView ();
		}
		private void SetAllSerieEpisodesSelectedState(Serie tempSerie, bool selectedStatus)
		{
			foreach (SerieSeason tempSeason in tempSerie.Seasons)
				if (IsSeasonAccepted(tempSeason))
					SetAllSeasonEpisodesSelectedState(tempSeason, selectedStatus);
		}
		private void SetAllSeasonEpisodesSelectedState(SerieSeason tempSeason, bool selectedStatus)
		{
			foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
				if (IsEpisodeAccepted(tempEpisode))
				{
					if (selectedStatus == false && tempEpisode.Selected == true)
						selectedSize -= tempEpisode.Size;
					else if (selectedStatus == true && tempEpisode.Selected == false)
						selectedSize += tempEpisode.Size;
					tempEpisode.Selected = selectedStatus;
				}
		}
		private void RemoveTreeViewItem(TreeViewItem item, SerieCategory category)
		{
			if (category == SerieCategory.serie)
			{
				int counter = 0;
				foreach (TreeViewItem tempItem in serieTreeView.Items)
				{
					if (tempItem == item)
						break;
					counter++;
				}
				serieTreeView.Items.Remove(item);
				if (serieTreeView.Items.Count == 0)
				{
					ResetSerieInfo();
					return;
				}

				//selektiraj prvu sljedecu seriju ili prvu prije ako nema sljedece
				try
				{
					TreeViewItem nextSerie = serieTreeView.Items[counter] as TreeViewItem;
					nextSerie.IsSelected = true;
				}
				catch
				{
					TreeViewItem previousSerie = serieTreeView.Items[counter - 1] as TreeViewItem;
					previousSerie.IsSelected = true;
				}
			}
			else if (category == SerieCategory.season)
			{
				TreeViewItem parentSerie = item.Parent as TreeViewItem;
				int counter = 0;
				foreach (TreeViewItem tempItem in parentSerie.Items)
				{
					if (tempItem == item)
						break;
					counter++;
				}

				parentSerie.Items.Remove(item);
				Serie tempSerie = parentSerie.DataContext as Serie;
				if (FetchSerieShowingSize(tempSerie) == 0)
					RemoveTreeViewItem(parentSerie, SerieCategory.serie);

				else
				{
					//selektiraj prvu sljedecu sezonu ili prvu prije ako nema sljedece
					try
					{
						TreeViewItem nextSeason = parentSerie.Items[counter] as TreeViewItem;
						nextSeason.IsSelected = true;
					}
					catch
					{
						TreeViewItem previousSeason = parentSerie.Items[counter - 1] as TreeViewItem;
						previousSeason.IsSelected = true;
					}
				}
			}
			else if (category == SerieCategory.episode)
			{
				TreeViewItem parentSeason = item.Parent as TreeViewItem;
				TreeViewItem parentSerie = parentSeason.Parent as TreeViewItem;

				//Zapamti index epizode za kasnije selektirati sljedecu
				int counter = 0;
				foreach (TreeViewItem tempItem in parentSeason.Items)
				{
					if (tempItem == item)
						break;
					counter++;
				}
				parentSeason.Items.Remove(item); //makni epizodu

				SerieSeason tempSeason = parentSeason.DataContext as SerieSeason;
				if (FetchSeasonShowingSize(tempSeason) == 0)
				{
					RemoveTreeViewItem(parentSeason, SerieCategory.season);	//ako sezona epizode nema vise sezona, makni sezonu
				}
				else
				{
					//u protivnom, selektiraj slijedecu / prijasnju epizodu
					try
					{
						TreeViewItem nextEpisode = parentSeason.Items[counter] as TreeViewItem;
						nextEpisode.IsSelected = true;
					}
					catch
					{
						TreeViewItem previousEpisode = parentSeason.Items[counter - 1] as TreeViewItem;
						previousEpisode.IsSelected = true;
					}
					return;
				}
			}

			numberTBSerie.Text = serieTreeView.Items.Count.ToString();
			RefreshStatistics();
			if (serieTreeView.Items.Count == 0)
			{
				_serieView.MoveCurrentToPosition(-1);
				serieNameTB.Text = "";
				SetRatingCanvas(ratingCanvasSerie, 0);
				summaryTBSerie.Text = "";
				sizeTBSerie.Text = "";
				runtimeTBSerie.Text = "";
				subCroSerie.Opacity = 0.06;
				subEngSerie.Opacity = 0.06;
				serieSubCroCount.Opacity = 0.06;
				serieSubEngCount.Opacity = 0.06;
				rezSDSerie.Opacity = 0.06;
				serieRezSDCount.Opacity = 0.06;
				rez720pSerie.Opacity = 0.06;
				serieRez720pCount.Opacity = 0.06;
				rez1080pSerie.Opacity = 0.06;
				serieRez1080pCount.Opacity = 0.06;
			}
		}
		private void ResetSerieInfo()
		{
			SetRatingCanvas(ratingCanvasSerie, 0);
			_serieView.MoveCurrentToPosition(-1);
			serieNameTB.Text = "";
			runtimeTBSerie.Text = "";
			sizeTBSerie.Text = "";
			summaryTBSerie.Text = "";
			numberTBSerie.Text = "0";

			ResetSerieResolutionTextBoxes();
			ResetSerieSubtitleTextBoxes();
		}

		private void serieViewSelectedNotChosen_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			serieTreeView.Background = Brushes.White;
			showAllSeries = false;
			showOnlyNotSelectedSeries = true;
			showOnlySelectedSeries = false;
			FillSerieTreeView();
			var style = (Style)TryFindResource("addBtnNormal");
			addRemoveSerie.Style = style;
		}
		private void serieViewSelectedChosen_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			BrushConverter bc = new BrushConverter();
			serieTreeView.Background = (Brush)bc.ConvertFrom("#FFE9FFFF");
			showAllSeries = false;
			showOnlyNotSelectedSeries = false;
			showOnlySelectedSeries = true;
			FillSerieTreeView();
			var style = (Style)TryFindResource("removeBtnNormal");
			addRemoveSerie.Style = style;
		}

		#endregion

		#region Index changed - set info

		private void serieTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			TreeViewItem selectedItem = serieTreeView.SelectedItem as TreeViewItem;
			if (selectedItem == null)
				return;

			#region Set info

			genreLabelSerie.Visibility = System.Windows.Visibility.Visible;
			try
			{
				Serie selectedSerie = (Serie)selectedItem.DataContext;
				serieSelectedCategory = SerieCategory.serie;
				_serieView.MoveCurrentTo(selectedSerie);
				_serieSeasonView = (CollectionView)CollectionViewSource.GetDefaultView(selectedSerie.Seasons);
				SetRatingCanvas(ratingCanvasSerie, selectedSerie.Rating);
				SetSerieInfo(selectedSerie);
			}
			catch
			{
				try
				{
					SerieSeason selectedSeason = (SerieSeason)selectedItem.DataContext;
					serieSelectedCategory = SerieCategory.season;
					_serieView.MoveCurrentTo(selectedSeason.ParentSerie);
					_serieSeasonView.MoveCurrentTo(selectedSeason);
					_serieEpisodeView = (CollectionView)CollectionViewSource.GetDefaultView(selectedSeason.Episodes);
					SetRatingCanvas(ratingCanvasSerie, selectedSeason.ParentSerie.Rating);
					SetSerieSeasonInfo(selectedSeason);
				}
				catch
				{
					try
					{
						SerieEpisode selectedEpisode = (SerieEpisode)selectedItem.DataContext;
						serieSelectedCategory = SerieCategory.episode;
						_serieView.MoveCurrentTo(selectedEpisode.ParentSeason.ParentSerie);
						_serieSeasonView.MoveCurrentTo(selectedEpisode.ParentSeason);
						_serieEpisodeView.MoveCurrentTo(selectedEpisode);
						SetRatingCanvas(ratingCanvasSerie, selectedEpisode.ParentSeason.ParentSerie.Rating);
						SetSerieEpisodeInfo(selectedEpisode);
					}
					catch
					{
					}
				}
			}
			try
			{
				RefreshSerieSubtitleTextBlock();
			}
			catch
			{
			}

			#endregion

			#region Set Button image

			var hbitmap = VideoKatalog.View.Properties.Resources.RemoveFromHDD.GetHbitmap();
			if (showOnlyNotSelectedSeries)
			{
				var style = (Style)TryFindResource("addBtnNormal");
				addRemoveSerie.Style = style;
			}
			else if (showOnlySelectedSeries)
			{
				var style = (Style)TryFindResource("removeBtnNormal");
				addRemoveSerie.Style = style;
			}
			else
			{
				if (serieSelectedCategory == SerieCategory.serie)
				{
					Serie selectedSerie = (Serie)selectedItem.DataContext;
					int countNotSelectedEpisodes = 0;
					foreach (SerieSeason tempSeason in selectedSerie.Seasons)
						foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
							if (tempEpisode.Selected == false && IsEpisodeAccepted(tempEpisode))
								countNotSelectedEpisodes++;
					if (countNotSelectedEpisodes == 0)
						hbitmap = VideoKatalog.View.Properties.Resources.RemoveFromHDD.GetHbitmap();
					else
						hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap();
				}
				else if (serieSelectedCategory == SerieCategory.season)
				{
					SerieSeason selectedSeason = (SerieSeason)selectedItem.DataContext;
					int countNotSelectedEpisodes = 0;
					foreach (SerieEpisode tempEpisode in selectedSeason.Episodes)
						if (tempEpisode.Selected == false && IsEpisodeAccepted(tempEpisode))
							countNotSelectedEpisodes++;
					if (countNotSelectedEpisodes == 0)
						hbitmap = VideoKatalog.View.Properties.Resources.RemoveFromHDD.GetHbitmap();
					else
						hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap();
				}
				else if (serieSelectedCategory == SerieCategory.episode)
				{
					SerieEpisode selectedEpisode = (SerieEpisode)selectedItem.DataContext;
					if (selectedEpisode.Selected == false)
						hbitmap = VideoKatalog.View.Properties.Resources.SelectForHDD.GetHbitmap();
					else
						hbitmap = VideoKatalog.View.Properties.Resources.RemoveFromHDD.GetHbitmap();
				}
				else
				{
					//addRemoveSerieBTNImage.Source = null;
					return;
				}
			}
			//addRemoveSerieBTNImage.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap (hbitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions ()); ;

			#endregion
			serieTreeView.Focus();
		}
		private void SetSerieInfo(Serie currentSerie)
		{
			long totalSize = 0;
			long totalRuntime = 0;
			Dictionary<string, int> resolutionDic = new Dictionary<string, int>();
			Dictionary<string, int> hddDic = new Dictionary<string, int>();
			int countEpisodes = 0;
			DateTime minDate = new DateTime(9999, 1, 1);
			DateTime maxDate = new DateTime(1, 1, 1);
			foreach (SerieSeason tempSeason in currentSerie.Seasons)
			{
				FetchSerieEpisodesInfo(tempSeason, ref totalSize, ref totalRuntime, ref resolutionDic, ref hddDic,
					ref countEpisodes, ref maxDate, ref minDate);
			}

			summaryTBSerie.Text = System.Web.HttpUtility.HtmlDecode(currentSerie.Summary);

			SetSerieNameAndYear(currentSerie);
			SetSerieResolutionTextBoxes(resolutionDic, countEpisodes);
			SetSizeAndRuntimeTextBox(totalRuntime, totalSize);
			episodeAirDateLabel.Text = "";
		}
		private void SetSerieSeasonInfo(SerieSeason currentSeason)
		{
			long totalSize = 0;
			long totalRuntime = 0;
			Dictionary<string, int> resolutionDic = new Dictionary<string, int>();
			Dictionary<string, int> hddDic = new Dictionary<string, int>();
			int countEpisodes = 0;
			DateTime minDate = new DateTime(9999, 1, 1);
			DateTime maxDate = new DateTime(1, 1, 1);
			FetchSerieEpisodesInfo(currentSeason, ref totalSize, ref totalRuntime, ref resolutionDic,
				ref hddDic, ref countEpisodes, ref maxDate, ref minDate);

			summaryTBSerie.Text = System.Web.HttpUtility.HtmlDecode(currentSeason.ParentSerie.Summary);
			SetSeasonNameYear(currentSeason);
			SetSerieResolutionTextBoxes(resolutionDic, countEpisodes);
			SetSizeAndRuntimeTextBox(totalRuntime, totalSize);
			episodeAirDateLabel.Text = "";
		}
		private void SetSerieEpisodeInfo(SerieEpisode currentEp)
		{
			ResetSerieResolutionTextBoxes();
			Converters.ResolutionToStringConverter rezConverter = new Converters.ResolutionToStringConverter();
			string resolution = rezConverter.Convert(currentEp.Width, typeof(string), null, null).ToString();
			if (resolution == "SD")
			{
				rezSDSerie.Opacity = 1;
			}
			else if (resolution == "720p")
			{
				rez720pSerie.Opacity = 1;
			}
			else if (resolution == "1080p")
			{
				rez1080pSerie.Opacity = 1;
			}
			SetSizeAndRuntimeTextBox(currentEp.Runtime, currentEp.Size);
			serieNameTB.Text = currentEp.OrigName;
			summaryTBSerie.Text = System.Web.HttpUtility.HtmlDecode(currentEp.Summary);
			if (currentEp.AirDate.Year < 1900)
				episodeAirDateLabel.Text = "Datum prikazivanja: N/A";
			else
				episodeAirDateLabel.Text = "Datum prikazivanja: " + currentEp.AirDate.ToShortDateString();
			genreLabelSerie.Visibility = System.Windows.Visibility.Hidden;
		}

		#endregion

		private void SetSerieNameAndYear(Serie currentSerie)
		{
			int minYear = DateTime.Now.Year + 1;
			for (int i = 0; i < currentSerie.Seasons.Count; i++)
			{
				for (int j = 0; j < currentSerie.Seasons.ElementAt(i).Episodes.Count; j++)
				{
					int episodeYear = currentSerie.Seasons.ElementAt(i).Episodes.ElementAt(j).AirDate.Year;
					if (episodeYear < minYear && episodeYear > 1900)
					{
						minYear = episodeYear;
					}
				}
			}
			if (minYear == DateTime.Now.Year + 1)
				serieNameTB.Text = currentSerie.OrigName;
			else
				serieNameTB.Text = currentSerie.OrigName + "  (" + minYear + ")";
		}
		private void SetSeasonNameYear(SerieSeason currentSeason)
		{
			int minYear = DateTime.Now.Year + 1;
			foreach (SerieEpisode tempEpisode in currentSeason.Episodes)
				if (tempEpisode.AirDate.Year < minYear && tempEpisode.AirDate.Year > 1900)
					minYear = tempEpisode.AirDate.Year;
			if (minYear == DateTime.Now.Year + 1)
				serieNameTB.Text = currentSeason.Name;
			else
				serieNameTB.Text = currentSeason.Name + "  (" + minYear + ")";
		}
		private void RefreshSerieSubtitleTextBlock()
		{
			object obj = ((TreeViewItem)serieTreeView.SelectedItem).DataContext;
			Dictionary<string, int> subtitleDic = new Dictionary<string, int>();
			int episodeCount = 0;
			try
			{
				Serie selectedSerie = obj as Serie;
				foreach (SerieSeason tempSeason in selectedSerie.Seasons)
				{
					if (IsSeasonAccepted(tempSeason) == false)
						continue;
					foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
					{
						if (IsEpisodeAccepted(tempEpisode) == false)
							continue;
						episodeCount++;
						foreach (Language tempSub in tempEpisode.SubtitleLanguageList)
						{
							FillStringIntDictionary(ref subtitleDic, tempSub.Name);
						}
					}
				}
				serieSubEngCount.Visibility = System.Windows.Visibility.Visible;
				serieSubCroCount.Visibility = System.Windows.Visibility.Visible;
			}
			catch
			{
			}
			try
			{
				SerieSeason selectedSeason = obj as SerieSeason;
				foreach (SerieEpisode tempEpisode in selectedSeason.Episodes)
				{
					episodeCount++;
					foreach (Language tempSub in tempEpisode.SubtitleLanguageList)
					{
						FillStringIntDictionary(ref subtitleDic, tempSub.Name);
					}
				}
				serieSubEngCount.Visibility = System.Windows.Visibility.Visible;
				serieSubCroCount.Visibility = System.Windows.Visibility.Visible;
			}
			catch
			{
			}
			try
			{
				SerieEpisode selectedEpisode = obj as SerieEpisode;
				foreach (Language tempSub in selectedEpisode.SubtitleLanguageList)
				{
					FillStringIntDictionary(ref subtitleDic, tempSub.Name);
				}
				serieSubEngCount.Visibility = System.Windows.Visibility.Hidden;
				serieSubCroCount.Visibility = System.Windows.Visibility.Hidden;
			}
			catch
			{
			}

			ResetSerieSubtitleTextBoxes();
			if (subtitleDic.ContainsKey("Croatian") == false)
			{
				subCroSerie.Opacity = 1;
				serieSubCroCount.Opacity = 1;
				subCroSerie.Foreground = new SolidColorBrush(Colors.DarkRed);
				serieSubCroCount.Foreground = new SolidColorBrush(Colors.DarkRed);
				serieSubCroCount.Text = string.Format("(0/{0})", episodeCount);
			}
			else
			{
				subCroSerie.Foreground = new SolidColorBrush(Colors.Black);
				serieSubCroCount.Foreground = new SolidColorBrush(Colors.Black);
			}

			foreach (KeyValuePair<string, int> tempSub in subtitleDic)
			{
				if (tempSub.Key == "Croatian")
				{
					subCroSerie.Opacity = 1;
					serieSubCroCount.Opacity = 1;
					serieSubCroCount.Text = string.Format("({0}/{1})", tempSub.Value, episodeCount);
				}
				else if (tempSub.Key == "English")
				{
					subEngSerie.Opacity = 1;
					serieSubEngCount.Opacity = 1;
					serieSubEngCount.Text = string.Format("({0}/{1})", tempSub.Value, episodeCount);
				}
			}
		}
		private void ResetSerieResolutionTextBoxes()
		{
			rezSDSerie.Opacity = 0.06;
			serieRezSDCount.Opacity = 0.06;
			serieRezSDCount.Text = "(0/0)";

			rez720pSerie.Opacity = 0.06;
			serieRez720pCount.Opacity = 0.06;
			serieRez720pCount.Text = "(0/0)";

			rez1080pSerie.Opacity = 0.06;
			serieRez1080pCount.Opacity = 0.06;
			serieRez1080pCount.Text = "(0/0)";
		}
		private void ResetSerieSubtitleTextBoxes()
		{
			subCroSerie.Opacity = 0.06;
			serieSubCroCount.Opacity = 0.06;
			serieSubCroCount.Text = "(0/0)";

			subEngSerie.Opacity = 0.06;
			serieSubEngCount.Opacity = 0.06;
			serieSubEngCount.Text = "(0/0)";
		}
		private void SetSerieResolutionTextBoxes(Dictionary<string, int> resolutionDic, int countEpisodes)
		{
			ResetSerieResolutionTextBoxes();

			foreach (KeyValuePair<string, int> pair in resolutionDic)
			{
				if (pair.Key == "SD")
				{
					serieRezSDCount.Text = string.Format("({0}/{1})", pair.Value, countEpisodes);
					rezSDSerie.Opacity = 1;
					serieRezSDCount.Opacity = 1;
				}
				else if (pair.Key == "720p")
				{
					serieRez720pCount.Text = string.Format("({0}/{1})", pair.Value, countEpisodes);
					rez720pSerie.Opacity = 1;
					serieRez720pCount.Opacity = 1;
				}
				else
				{
					serieRez1080pCount.Text = string.Format("({0}/{1})", pair.Value, countEpisodes);
					rez1080pSerie.Opacity = 1;
					serieRez1080pCount.Opacity = 1;
				}
			}
		}
		private void SetSizeAndRuntimeTextBox(long runtime, long size)
		{
			Converters.SizeConverter sizeConv = new Converters.SizeConverter();
			sizeTBSerie.Text = sizeConv.Convert(size, typeof(string), null, null).ToString();

			Converters.RuntimeConverter runtimeConv = new Converters.RuntimeConverter();
			runtimeTBSerie.Text = runtimeConv.Convert(runtime, typeof(string), null, null).ToString();
		}

		#region Genres

		bool serieGenreGridExpanded = false;
		private void openCloseSerieGenreGrid_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (serieGenreGridExpanded)
			{
				var stb = (Storyboard)TryFindResource("serieGenreCollapse");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
				}
				serieGenreGridExpanded = false;
			}
			else
			{
				var stb = (Storyboard)TryFindResource("serieGenreExpand");
				showingSelectHddSize = false;
				if (stb != null)
				{
					stb.Begin();
				}
				serieGenreGridExpanded = true;
			}
		}
		private void serieGenreAll_Click(object sender, RoutedEventArgs e)
		{
			SetSerieGenreCheckboxes(true);
		}
		private void serieGenreNone_Click(object sender, RoutedEventArgs e)
		{
			SetSerieGenreCheckboxes(false);
		}
		private void SetSerieGenreCheckboxes(bool value)
		{
			actionCheckBox1.IsChecked = value;
			adventureCheckBox1.IsChecked = value;
			animationCheckBox1.IsChecked = value;
			biographyCheckBox1.IsChecked = value;
			comedyCheckBox1.IsChecked = value;
			crimeCheckBox1.IsChecked = value;
			documentaryCheckBox1.IsChecked = value;
			dramaCheckBox1.IsChecked = value;
			familyCheckBox1.IsChecked = value;
			fantasyCheckBox1.IsChecked = value;
			filmNoirCheckBox1.IsChecked = value;
			historyCheckBox1.IsChecked = value;
			horrorCheckBox1.IsChecked = value;
			musicalCheckBox1.IsChecked = value;
			musicCheckBox1.IsChecked = value;
			misteryCheckBox1.IsChecked = value;
			sciFiCheckBox1.IsChecked = value;
			romanceCheckBox1.IsChecked = value;
			sportCheckBox1.IsChecked = value;
			thrillerCheckBox1.IsChecked = value;
			warCheckBox1.IsChecked = value;
			westernCheckBox1.IsChecked = value;
		}

		#endregion

		//Help functions
		private void FetchSerieEpisodesInfo(SerieSeason season, ref long totalSize, ref long totalRuntime, ref Dictionary<string, int> resolutionDic,
			ref Dictionary<string, int> hddDic, ref int countEpisodes, ref DateTime maxDate, ref DateTime minDate)
		{

			if (IsSeasonAccepted(season) == false)
				return;
			foreach (SerieEpisode tempEpisode in season.Episodes)
			{
				if (IsEpisodeAccepted(tempEpisode) == false)
					continue;
				if (tempEpisode.AirDate > maxDate)
					maxDate = tempEpisode.AirDate;
				if (tempEpisode.AirDate < minDate)
					minDate = tempEpisode.AirDate;
				countEpisodes++;
				totalSize += tempEpisode.Size;
				totalRuntime += tempEpisode.Runtime;
				Converters.ResolutionToStringConverter converter = new Converters.ResolutionToStringConverter();
				string resolution = converter.Convert(tempEpisode.Width, typeof(string), null, null).ToString();

				FillStringIntDictionary(ref resolutionDic, resolution);
				FillStringIntDictionary(ref hddDic, tempEpisode.Hdd.ToString());
			}
		}
		private long FetchSerieShowingSize(Serie tempSerie)
		{
			long size = 0;
			foreach (SerieSeason tempSeason in tempSerie.Seasons)
				this.Dispatcher.Invoke(new Action(() =>
				{
					if (IsSeasonAccepted(tempSeason))
						size += FetchSeasonShowingSize(tempSeason);
				}));
			return size;
		}
		private long FetchSerieFullSize(Serie tempSerie)
		{
			long size = 0;
			foreach (SerieSeason tempSeason in tempSerie.Seasons)
				size += FetchSeasonFullSize(tempSeason);
			return size;
		}
		private long FetchSerieSelectedSize(Serie tempSerie)
		{
			long size = 0;
			foreach (SerieSeason tempSeason in tempSerie.Seasons)
				size += FetchSeasonSelectedSize(tempSeason);
			return size;
		}
		private long FetchSeasonShowingSize(SerieSeason tempSeason)
		{
			long size = 0;
			foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
				if (IsEpisodeAccepted(tempEpisode))
					size += tempEpisode.Size;
			return size;
		}
		private long FetchSeasonFullSize(SerieSeason tempSeason)
		{
			long size = 0;
			foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
				size += tempEpisode.Size;
			return size;
		}
		private long FetchSeasonSelectedSize(SerieSeason tempSeason)
		{
			long size = 0;
			foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
				if (tempEpisode.Selected)
					size += tempEpisode.Size;
			return size;
		}
		private void FillStringIntDictionary(ref Dictionary<string, int> dict, string str)
		{
			if (dict.ContainsKey(str))
				dict[str]++;
			else
				dict.Add(str, 1);
		}

		#region Filter / Sort

		//Filter
		private bool IsSerieAccepted(Serie serieToCheck)
		{

			#region Naziv

			if (serieToCheck.Name.ToLower().Contains(searchSerieName.Text.ToLower()) == false && serieToCheck.OrigName.ToLower().Contains(searchSerieName.Text.ToLower()) == false)
				return false;

			#endregion

			#region Žanr

			bool foundOneGenre = false;
			foreach (Genre tempGenre in serieToCheck.Genres)
			{
				if (tempGenre.Name == "Akcija" && actionCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Avantura" && adventureCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Animacija" && animationCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Biografija" && biographyCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Komedija" && comedyCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Krimi" && crimeCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Dokumentarni" && documentaryCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Drama" && dramaCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Obiteljski" && familyCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Fantastika" && fantasyCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Crno-bijeli" && filmNoirCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Povijesni" && historyCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Horor" && horrorCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Muzički" && musicCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Pjevan" && musicalCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Misterija" && misteryCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Romantika" && romanceCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Znanstvena fantastika" && sciFiCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Sportski" && sportCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Triler" && thrillerCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Ratni" && warCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
				if (tempGenre.Name == "Vestern" && westernCheckBox1.IsChecked == true)
				{
					foundOneGenre = true;
					break;
				}
			}
			if (foundOneGenre == false)
				return false;

			#endregion

			#region Glumci / Redatelji

			//Glumci
			if (searchSerieCast.Text.Length > 0)
			{
				bool foundActor = false;
				foreach (Person actor in serieToCheck.Actors)
					if (actor.Name.ToLower().Contains(searchSerieCast.Text.ToLower()))
						foundActor = true;
				if (foundActor == false)
					return false;
			}

			//Redatelji
			if (searchSerieDirector.Text.Length > 0)
			{
				bool foundDirector = false;
				foreach (Person director in serieToCheck.Directors)
					if (director.Name.ToLower().Contains(searchSerieDirector.Text.ToLower()))
						foundDirector = true;
				if (foundDirector == false)
					return false;
			}

			#endregion

			#region Ocjena & Godina & Veličina

			if ((double)serieToCheck.Rating > ratingRangeSliderSerie.UpperValue || (double)serieToCheck.Rating < ratingRangeSliderSerie.LowerValue)
				return false;
			if ((double)serieToCheck.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year > yearRangeSliderSerie.UpperValue ||
				(double)serieToCheck.Seasons.ElementAt(0).Episodes.ElementAt(0).AirDate.Year < yearRangeSliderSerie.LowerValue)
				return false;
			long totalSize = FetchSerieShowingSize(serieToCheck);
			if ((double)totalSize > sizeRangeSliderSerie.UpperValue || (double)totalSize < sizeRangeSliderSerie.LowerValue)
				return false;

			#endregion

			#region Rezolucija & prijevod & odabrano

			Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter();
			int countAcceptedSeasons = 0;
			foreach (SerieSeason tempSeason in serieToCheck.Seasons)
			{
				if (IsSeasonAccepted(tempSeason))
				{
					countAcceptedSeasons++;
					break;
				}
			}
			if (countAcceptedSeasons == 0)
				return false;

			#endregion

			return true;
		}
		private bool IsSeasonAccepted(SerieSeason seasonToCheck)
		{
			bool toReturn = false;
			foreach (SerieEpisode tempEpisode in seasonToCheck.Episodes)
			{
				this.Dispatcher.Invoke(new Action(() =>
				{
					if (IsEpisodeAccepted(tempEpisode))
						toReturn = true;
				}));
			}
			return toReturn;
		}
		private bool IsEpisodeAccepted(SerieEpisode episodeToCheck)
		{

			#region Rezolucija

			Converters.ResolutionToStringConverter resolutionConverter = new Converters.ResolutionToStringConverter();
			string resolution = resolutionConverter.Convert(episodeToCheck.Width, typeof(string), null, null).ToString();
			if (resolution == "SD" && serieSearchRezSD.IsChecked == false)
				return false;
			if (resolution == "720p" && serieSearchRez720p.IsChecked == false)
				return false;
			if (resolution == "1080p" && serieSearchRez1080p.IsChecked == false)
				return false;

			#endregion

			#region Prijevod

			Language cro = new Language();
			cro.Name = "Croatian";
			Language eng = new Language();
			eng.Name = "English";

			if (serieSearchSubCro.IsChecked == true && episodeToCheck.SubtitleLanguageList.Contains(cro, new LanguageComparerByName()) == false)
				return false;
			if (serieSearchSubEng.IsChecked == true && episodeToCheck.SubtitleLanguageList.Contains(eng, new LanguageComparerByName()) == false)
				return false;

			#endregion

			#region Odabrano

			if (episodeToCheck.Selected && (showOnlyNotSelectedSeries))
			{
				return false;
			}
			else if (episodeToCheck.Selected == false && showOnlySelectedSeries)
			{
				return false;
			}

			#endregion

			if (episodeToCheck.Show == false)
				return false;

			return true;
		}
		private void serieSearchGridClose_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			busyIndicator.IsBusy = true;
			busyIndicator.BusyContent = "Filtriranje";
			playCloseSound(null, null);

			ThreadStart starter = delegate
			{
				SetSerieSliders();
				for (long i = 0; i < 100000000; i++) ;
				this.Dispatcher.Invoke(new Action(() =>
				{
					FillSerieTreeView();
					serieTreeView.Focus();
					busyIndicator.IsBusy = false;
				}));

			};
			Thread filterSeriesThread = new Thread(starter);
			filterSeriesThread.IsBackground = true;
			filterSeriesThread.Start();
		}

		//Sort
		private void SortSeries(object sender, MouseButtonEventArgs e)
		{
			busyIndicator.IsBusy = true;
			busyIndicator.BusyContent = "Sortiranje serija";
			TextBlock clickedTB = sender as TextBlock;
			ThreadStart starter = delegate
			{
				string TBName = "";
				this.Dispatcher.Invoke(new Action(() =>
				{
					TBName = clickedTB.Name;
				}));
				DateTime now = DateTime.Now;
				while ((DateTime.Now - now).Milliseconds < 300) ;
				if (TBName.ToLower().Contains("name"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						SortSeries("name");
					}));
				}
				else if (TBName.ToLower().Contains("rating"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						SortSeries("rating");
					}));
				}
				else if (TBName.ToLower().Contains("year"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						SortSeries("year");
					}));
				}
				else if (TBName.ToLower().Contains("size"))
				{
					this.Dispatcher.Invoke(new Action(() =>
					{
						SortSeries("size");
					}));
				}
				this.Dispatcher.Invoke(new Action(() =>
				{
					busyIndicator.IsBusy = false;
				}));
			};
			Thread sortSeriesThread = new Thread(starter);
			sortSeriesThread.IsBackground = true;
			sortSeriesThread.Start();
		}
		private void SortSeries(string byProperty)
		{
			ObservableCollection<Serie> sortedList = new ObservableCollection<Serie>();
			foreach (Serie tempSerie in listOfSeries)
				sortedList.Add(tempSerie);
			for (int i = 0; i < sortedList.Count; i++)
			{
				for (int j = i + 1; j < sortedList.Count; j++)
				{
					if (byProperty == "name")
					{
						for (int k = 0; k < sortedList[i].Name.Length; k++)
						{
							if (sortedList[i].Name[k] > sortedList[j].Name[k])
							{
								ReplaceIndexes(sortedList, i, j);
								break;
							}
							else
								break;
						}
					}
					else if (byProperty == "year")
					{
						if (sortedList[i].Seasons[0].Episodes[0].AirDate.Year > sortedList[j].Seasons[0].Episodes[0].AirDate.Year)
							ReplaceIndexes(sortedList, i, j);
					}
					else if (byProperty == "size")
					{
						if (FetchSerieShowingSize(sortedList[i]) > FetchSerieShowingSize(sortedList[j]))
							ReplaceIndexes(sortedList, i, j);
					}
					else if (byProperty == "rating")
					{
						if (sortedList[i].Rating > sortedList[j].Rating)
							ReplaceIndexes(sortedList, i, j);
					}
				}
			}
			listOfSeries.Clear();
			foreach (Serie tempSerie in sortedList)
				listOfSeries.Add(tempSerie);
			if (serieSortDescending.IsChecked == true)
				ChangeSerieSortingDirections(null, null);
			_serieView = (CollectionView)CollectionViewSource.GetDefaultView(listOfSeries);
			FillSerieTreeView();
		}
		private void ReplaceIndexes(ObservableCollection<Serie> list, int i, int j)
		{
			Serie tempSerie = list.ElementAt(i);
			list[i] = list.ElementAt(j);
			list[j] = tempSerie;
		}

		private void ChangeSerieSortingDirections(object sender, RoutedEventArgs e)
		{
			//listOfSeries =  listOfSeries.Reverse () as ObservableCollection<Serie>;
			ObservableCollection<Serie> sortedList = new ObservableCollection<Serie>();
			foreach (Serie tempSerie in listOfSeries)
				sortedList.Add(tempSerie);
			listOfSeries.Clear();
			for (int i = sortedList.Count - 1; i >= 0; i--)
				listOfSeries.Add(sortedList[i]);

			if (sender == null)
				return;
			_serieView = (CollectionView)CollectionViewSource.GetDefaultView(listOfSeries);
			try
			{
				FillSerieTreeView();
			}
			catch
			{
			}
		}

		#endregion

		//IMDb / Trailer
		private void serieGoToImdb_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				if (serieSelectedCategory == SerieCategory.serie)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as Serie).InternetLink);
				else if (serieSelectedCategory == SerieCategory.season)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieSeason).InternetLink);
				else if (serieSelectedCategory == SerieCategory.episode)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieEpisode).InternetLink);
				else
					Xceed.Wpf.Toolkit.MessageBox.Show("Ništa nije selektirano");
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška!");
			}
		}
		private void serieGoToYoutube_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				if (serieSelectedCategory == SerieCategory.serie)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as Serie).TrailerLink);
				else if (serieSelectedCategory == SerieCategory.season)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieSeason).TrailerLink);
				else if (serieSelectedCategory == SerieCategory.episode)
					System.Diagnostics.Process.Start(((serieTreeView.SelectedItem as TreeViewItem).DataContext as SerieEpisode).TrailerLink);
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška!");
			}
		}


		#endregion

		#region Load / Save Selection

		private void loadSelected_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
			openFileDlg.DefaultExt = ".txt"; // Default file extension
			openFileDlg.Filter = "Text documents |*.txt";   // Filter files by extension
			busyIndicator.IsBusy = true;
			busyIndicator.BusyContent = "Učitavanje odabira";

			Nullable<bool> result = openFileDlg.ShowDialog();
			try
			{
				if (result == true)
				{

					#region Reset selection status

					selectedSize = 0;
					foreach (Movie tempMovie in listOfMovies)
						tempMovie.Selected = false;

					foreach (SerieEpisode tempEpisode in listOfSerEpisodes)
						tempEpisode.Selected = false;

					#endregion

					string filename = openFileDlg.FileName;
					string readedLine;
					StreamReader selectedTxt = new StreamReader(filename);
					readedLine = selectedTxt.ReadLine();

					//najnovija verzija odabira
					if (readedLine.Contains("!S"))
					{
						loadModernSelected(readedLine);
						selectedTxt.Close();
						RefreshStatistics();
						busyIndicator.IsBusy = false;
						return;
					}

					//najobičniji popis samo filmova bez kategorija
					if (readedLine.Contains("Veličina HDDa:") == false)
					{

						//postavi hdd size na max
						hddSize = 2990000000000;
						hddSizeLabel.Text = "Veličina HDDa:   3 TB";
						filledProgressBar.Maximum = hddSize;

						while (string.IsNullOrEmpty(readedLine) == false)
						{
							foreach (Movie tempMovie in listOfMovies)
								if (readedLine.ToLower() == tempMovie.Name.ToLower())
								{
									selectedSize += tempMovie.Size;
									tempMovie.Selected = true;
									break;
								}
							readedLine = selectedTxt.ReadLine();
						}
						selectedTxt.Close();
						RefreshStatistics();
						statusBarTextBlock.Text = "Uspješno učitana datoteka";
						busyIndicator.IsBusy = false;
						return;
					}

					#region Set HDD Size

					readedLine = readedLine.Replace("Veličina HDDa: ", "").Trim().Replace("\r\n", "");
					if (readedLine.Contains("GB"))
					{
						readedLine = readedLine.Replace("GB", "").Trim();
						if (readedLine == "250")
						{
							hddSize = 249000000000;
							hddSizeLabel.Text = "Veličina HDDa:   250 GB";
							filledProgressBar.Maximum = hddSize;
						}
						else if (readedLine == "500")
						{
							hddSize = 499000000000;
							hddSizeLabel.Text = "Veličina HDDa:   500 GB";
							filledProgressBar.Maximum = hddSize;
						}
						else if (readedLine == "640")
						{
							hddSize = 639000000000;
							hddSizeLabel.Text = "Veličina HDDa:   640 GB";
							filledProgressBar.Maximum = hddSize;
						}
					}
					else if (readedLine.Contains("TB"))
					{
						readedLine = readedLine.Replace("TB", "").Trim();
						if (readedLine == "1")
						{
							hddSize = 990000000000;
							hddSizeLabel.Text = "Veličina HDDa:   1 TB";
							filledProgressBar.Maximum = hddSize;
						}
						else if (readedLine == "1,5")
						{
							hddSize = 1490000000000;
							hddSizeLabel.Text = "Veličina HDDa:   1,5 TB";
							filledProgressBar.Maximum = hddSize;
						}
						else if (readedLine == "2")
						{
							hddSize = 1990000000000;
							hddSizeLabel.Text = "Veličina HDDa:   2 TB";
							filledProgressBar.Maximum = hddSize;
						}
						else if (readedLine == "3")
						{
							hddSize = 2990000000000;
							hddSizeLabel.Text = "Veličina HDDa:   3 TB";
							filledProgressBar.Maximum = hddSize;
						}
					}
					else
						throw new Exception();

					#endregion

					#region učitaj filmove

					readedLine = selectedTxt.ReadLine();
					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("FILMOVI"))
						readedLine = selectedTxt.ReadLine();

					while (readedLine != "" && readedLine.Contains("SERIJE") == false)
					{
						foreach (Movie tempMovie in listOfMovies)
							if (readedLine.Trim().ToLower() == tempMovie.Name.ToLower() && tempMovie.Selected == false)
							{
								selectedSize += tempMovie.Size;
								tempMovie.Selected = true;
								break;
							}
						readedLine = selectedTxt.ReadLine();
					}

					#endregion

					#region učitaj serije

					//load
					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("SERIJE"))
					{
						readedLine = selectedTxt.ReadLine();
						if (readedLine == null)
							break;
					}

					while (readedLine != null)
					{
						if (readedLine == "")
						{
							readedLine = selectedTxt.ReadLine();
							continue;
						}
						string serieName = readedLine;
						Serie currSerie = new Serie();
						foreach (Serie tempSerie in listOfSeries)
							if (tempSerie.Name == serieName)
							{
								currSerie = tempSerie;
								break;
							}
						//Sezone
						readedLine = selectedTxt.ReadLine();
						if (readedLine.Contains("["))
						{
							while (readedLine.Contains("["))
							{
								string seasonName = readedLine.Replace("[", "").Replace("]", "");
								SerieSeason currSeason = new SerieSeason();
								foreach (SerieSeason tempSeason in currSerie.Seasons)
								{
									if (tempSeason.Name == seasonName)
									{
										currSeason = tempSeason;
										break;
									}
								}
								readedLine = selectedTxt.ReadLine();

								//Episode
								if (readedLine != null)
								{
									if (readedLine.Contains("..Ep"))
									{
										while (readedLine.Contains("..Ep"))
										{
											string episodeName = readedLine.Replace("..", "");
											foreach (SerieEpisode tempEpisode in currSeason.Episodes)
												if (tempEpisode.Name == episodeName && tempEpisode.Selected == false)
												{
													selectedSize += tempEpisode.Size;
													tempEpisode.Selected = true;
													break;
												}
											readedLine = selectedTxt.ReadLine();
										}
									}
									else
									{
										foreach (SerieEpisode tempEpisode in currSeason.Episodes)
										{
											selectedSize += tempEpisode.Size;
											tempEpisode.Selected = true;
										}
										if (readedLine == null)
											break;
										//readedLine = selectedTxt.ReadLine ();
									}
								}
								else
								{
									foreach (SerieEpisode tempEpisode in currSeason.Episodes)
									{
										tempEpisode.Selected = true;
										selectedSize += tempEpisode.Size;
									}
									if (readedLine == null)
										break;
									//readedLine = selectedTxt.ReadLine ();
								}

							}
						}
						else
							foreach (SerieSeason tempSeason in currSerie.Seasons)
								foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
								{
									tempEpisode.Selected = true;
									selectedSize += tempEpisode.Size;
								}
					}

					#endregion

					selectedTxt.Close();
					FillSerieTreeView();
					if (showOnlySelectedMovies)
						movieViewSelectedChosen_MouseLeftButtonUp(null, null);
					else
						movieViewSelectedNotChosen_MouseLeftButtonUp(null, null);
					RefreshStatistics();
					statusBarTextBlock.Text = "Uspješno učitana datoteka";
				}
				busyIndicator.IsBusy = false;
			}
			catch (Exception ex)
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Neispravan format datoteke s odabirom" + ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
				busyIndicator.IsBusy = false;
			}
		}
		private void loadModernSelected(string selected)
		{
			int movieStartsIndex = selected.IndexOf("F") + 1;
			int movieEndIndex = selected.IndexOf("!S");
			if (movieStartsIndex < movieEndIndex)
			{
				string movies = selected.Substring(movieStartsIndex, movieEndIndex - movieStartsIndex);
				foreach (string movieID in movies.Split('%'))
				{
					if (!string.IsNullOrEmpty(movieID.Trim()))
					{
						foreach (Movie tempMovie in listOfMovies)
						{
							if (int.Parse(movieID) == tempMovie.ID)
							{
								selectedSize += tempMovie.Size;
								tempMovie.Selected = true;
								break;
							}
						}
					}
				}
			}

			int serieStartsIndex = selected.IndexOf("!S") + 2;
			int serieSEndIndex = Math.Max(selected.IndexOf("\r\n"), selected.Length);
			if (serieStartsIndex < selected.Length)
			{
				string serieSelectedString = selected.Substring(serieStartsIndex, serieSEndIndex - serieStartsIndex);
				char[] serieDelimiter = new char[] { '#' };
				foreach (string serieString in serieSelectedString.Split(serieDelimiter, StringSplitOptions.RemoveEmptyEntries))
				{
					Serie tempSerie = new Serie();
					tempSerie.ID = int.Parse(serieString.Substring(0, serieString.IndexOf('$')));
					tempSerie.Seasons = new ObservableCollection<SerieSeason>();
					string serieRestString = serieString.Substring(serieString.IndexOf('$') + 1);

					foreach (Serie wholeSerie in listOfSeries)
					{
						if (tempSerie.ID == wholeSerie.ID)
						{
							char[] seasonDelimiter = new char[] { '$' };
							foreach (string seasonString in serieRestString.Split(seasonDelimiter, StringSplitOptions.RemoveEmptyEntries))
							{
								SerieSeason tempSeason = new SerieSeason();
								tempSeason.ID = int.Parse(seasonString.Substring(0, seasonString.IndexOf('&')));
								tempSeason.Episodes = new ObservableCollection<SerieEpisode>();
								string seasonRestString = seasonString.Substring(seasonString.IndexOf('&') + 1);

								foreach (SerieSeason wholeSeason in wholeSerie.Seasons)
								{
									if (wholeSeason.ID == tempSeason.ID)
									{
										char[] episodeDelimiter = new char[] { '&' };
										foreach (string episodeString in seasonRestString.Split(episodeDelimiter, StringSplitOptions.RemoveEmptyEntries))
										{
											SerieEpisode tempEpisode = new SerieEpisode();
											tempEpisode.ID = int.Parse(episodeString);
											tempSeason.Episodes.Add(tempEpisode);

											foreach (SerieEpisode wholeEpisode in wholeSeason.Episodes)
											{
												if (wholeEpisode.ID == tempEpisode.ID)
												{
													wholeEpisode.Selected = true;
													selectedSize += wholeEpisode.Size;
													break;
												}
											}
										}
										break;
									}
								}
							}
							break;
						}
					}
				}
			}
		}
		private void loadSelectedDatabase_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				selectedSize = 0;
				SelectFromCollection selectSelectionForm = new SelectFromCollection(selectionList, "Odaberi jedan od spremljenih odabira");
				selectSelectionForm.Owner = this;
				busyIndicator.IsBusy = true;
				busyIndicator.BusyContent = "Učitavanje odabira";
				selectSelectionForm.ShowDialog();
				Selection loadSelection = selectSelectionForm.selectedObject as Selection;
				if (loadSelection != null)
				{
					#region Set HDD Size

					hddSize = loadSelection.Size;
					filledProgressBar.Maximum = hddSize;
					if (hddSize == 249000000000)
						hddSizeLabel.Text = "Veličina HDDa:   250 GB";
					else if (hddSize == 499000000000)
						hddSizeLabel.Text = "Veličina HDDa:   500 GB";
					else if (hddSize == 639000000000)
						hddSizeLabel.Text = "Veličina HDDa:   640 GB";
					else if (hddSize == 990000000000)
						hddSizeLabel.Text = "Veličina HDDa:   1 TB";
					else if (hddSize == 1490000000000)
						hddSizeLabel.Text = "Veličina HDDa:   1,5 TB";
					else if (hddSize == 1990000000000)
						hddSizeLabel.Text = "Veličina HDDa:   2 TB";
					else
						hddSizeLabel.Text = "Veličina HDDa:   3 TB";

					#endregion

					foreach (Movie tempMovie in listOfMovies)
						tempMovie.Selected = false;
					foreach (SerieEpisode tempEpisode in listOfSerEpisodes)
						tempEpisode.Selected = false;
					foreach (Movie TempMovie in loadSelection.SelectedMovies)
					{
						FinderInCollection.FindInMovieCollection(listOfMovies, TempMovie.ID).Selected = true;
						selectedSize += FinderInCollection.FindInMovieCollection(listOfMovies, TempMovie.ID).Size;
					}
					foreach (SerieEpisode tempEpisode in loadSelection.selectedEpisodes)
					{
						FinderInCollection.FindInSerieEpisodeCollection(listOfSerEpisodes, tempEpisode.ID).Selected = true;
						selectedSize += FinderInCollection.FindInSerieEpisodeCollection(listOfSerEpisodes, tempEpisode.ID).Size;
					}

					FillSerieTreeView();
					_movieView.Refresh();
					RefreshStatistics();
					statusBarTextBlock.Text = "Uspješno učitan odabir";
				}
				busyIndicator.IsBusy = false;
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Neuspješno učitavanje", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
				busyIndicator.IsBusy = false;
			}
		}
		private void loadSelectedToRemove_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
				openFileDlg.DefaultExt = ".txt"; // Default file extension
				openFileDlg.Filter = "Text documents |*.txt";   // Filter files by extension

				busyIndicator.IsBusy = true;
				busyIndicator.BusyContent = "Učitavanje odabira";
				Nullable<bool> result = openFileDlg.ShowDialog();
				if (result == true)
				{
					string filename = openFileDlg.FileName;
					string readedLine;
					StreamReader selectedTxt = new StreamReader(filename);
					readedLine = selectedTxt.ReadLine();

					//najobičniji popis samo filmova bez kategorija
					if (readedLine.Contains("Veličina HDDa:") == false)
					{
						while (readedLine != null)
						{
							foreach (Movie tempMovie in listOfMovies)
								if (readedLine == tempMovie.Name)
								{
									tempMovie.Show = false;
									break;
								}
							readedLine = selectedTxt.ReadLine();
						}
						selectedTxt.Close();
						return;
					}

					readedLine = readedLine.Replace("Veličina HDDa: ", "");

					#region učitaj filmove

					int countExMovies = 0;
					readedLine = selectedTxt.ReadLine();
					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("FILMOVI"))
						readedLine = selectedTxt.ReadLine();

					while (readedLine != "")
					{
						foreach (Movie tempMovie in listOfMovies)
							if (readedLine == tempMovie.Name)
							{
								tempMovie.Show = false;
								countExMovies++;
								break;
							}
						readedLine = selectedTxt.ReadLine();
					}

					#endregion

					#region učitaj serije

					int countExSeries = 0;
					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("SERIJE"))
					{
						readedLine = selectedTxt.ReadLine();
						if (readedLine == null)
							break;
					}

					while (readedLine != null)
					{
						if (readedLine == "")
						{
							readedLine = selectedTxt.ReadLine();
							continue;
						}
						string serieName = readedLine;
						Serie currSerie = new Serie();
						foreach (Serie tempSerie in listOfSeries)
							if (tempSerie.Name == serieName)
							{
								currSerie = tempSerie;
								countExSeries++;
								break;
							}
						//Sezone
						readedLine = selectedTxt.ReadLine();
						if (readedLine.Contains("["))
						{
							while (readedLine.Contains("["))
							{
								string seasonName = readedLine.Replace("[", "").Replace("]", "");
								SerieSeason currSeason = new SerieSeason();
								foreach (SerieSeason tempSeason in currSerie.Seasons)
								{
									if (tempSeason.Name == seasonName)
									{
										currSeason = tempSeason;
										break;
									}
								}
								readedLine = selectedTxt.ReadLine();

								//Episode
								if (readedLine.Contains("..Ep"))
								{
									while (readedLine.Contains("..Ep"))
									{
										string episodeName = readedLine.Replace("..", "");
										foreach (SerieEpisode tempEpisode in currSeason.Episodes)
											if (tempEpisode.Name == episodeName)
											{
												tempEpisode.Show = false;
												break;
											}
										readedLine = selectedTxt.ReadLine();
									}
								}
								else
								{
									foreach (SerieEpisode tempEpisode in currSeason.Episodes)
										tempEpisode.Show = false;
									//readedLine = selectedTxt.ReadLine ();
								}
							}
						}
						else
							foreach (SerieSeason tempSeason in currSerie.Seasons)
								foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
									tempEpisode.Show = false;
					}

					#endregion

					selectedTxt.Close();
					FillSerieTreeView();
					_movieView.Refresh();
					statusBarTextBlock.Text = string.Format("Uspješno učitan odabir. Maknuto {0} filmova i {0} serija.", countExMovies, countExSeries);
				}
				busyIndicator.IsBusy = false;
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Neispravan format datoteke s odabirom", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
				busyIndicator.IsBusy = false;
			}
		}

		private void saveSelected_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
				saveFileDlg.FileName = "Popis odabranog";   // Default file name
				saveFileDlg.DefaultExt = ".txt"; // Default file extension
				saveFileDlg.Filter = "Text document|*.txt";	// Filter files by extension

				Nullable<bool> result = saveFileDlg.ShowDialog();
				if (result == true)
				{
					notSaved = false;
					string filename = saveFileDlg.FileName;
					StreamWriter selectedTxt = new StreamWriter(filename);
					selectedTxt.WriteLine(hddSizeLabel.Text);
					selectedTxt.WriteLine("\r\n--------------");
					int counter = 0;

					//Filmovi
					foreach (Movie tempMovie in listOfMovies)
						if (tempMovie.Selected)
							counter++;
					selectedTxt.WriteLine(string.Format("FILMOVI ({0}):\r\n--------------", counter));
					foreach (Movie tempMovie in listOfMovies)
						if (tempMovie.Selected)
							selectedTxt.WriteLine(tempMovie.Name);
					selectedTxt.WriteLine("\r\n--------------");

					//Serije
					counter = 0;
					foreach (Serie tempSerie in listOfSeries)
						if (FetchSerieSelectedSize(tempSerie) > 0)
							counter++;
					selectedTxt.WriteLine(string.Format("SERIJE ({0}):\r\n--------------", counter));
					foreach (Serie tempSerie in listOfSeries)
					{
						if (FetchSerieSelectedSize(tempSerie) > 0)
						{
							selectedTxt.WriteLine(tempSerie.Name);

							if (FetchSerieSelectedSize(tempSerie) != FetchSerieFullSize(tempSerie))
								foreach (SerieSeason tempSeason in tempSerie.Seasons)
									if (FetchSeasonSelectedSize(tempSeason) > 0)
									{
										selectedTxt.WriteLine("[" + tempSeason.Name + "]");

										if (FetchSeasonSelectedSize(tempSeason) != FetchSeasonFullSize(tempSeason))
											foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
												if (tempEpisode.Selected)
													selectedTxt.WriteLine(".." + tempEpisode.Name);
									}
							selectedTxt.WriteLine();
						}
					}

					selectedTxt.Close();
					statusBarTextBlock.Text = "Uspješno spremljen odabir (" + DateTime.Now.ToShortTimeString() + ")";
				}
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška prilikom sprenmanja", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void saveSelectedDatabase_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				InputDialog nameDialog = new InputDialog("Unesi naziv odabira");
				nameDialog.Owner = this;
				nameDialog.ShowDialog();

				if (nameDialog.accepted)
				{
					connection.Open();
					notSaved = false;
					Selection newSelection = new Selection();
					newSelection.Name = nameDialog.inputString;
					newSelection.DateOfLastChange = DateTime.Now;
					newSelection.Size = hddSize;
					foreach (Movie tempMovie in listOfMovies)
						if (tempMovie.Selected)
							newSelection.SelectedMovies.Add(tempMovie);
					foreach (SerieEpisode tempEpisode in listOfSerEpisodes)
						if (tempEpisode.Selected)
							newSelection.selectedEpisodes.Add(tempEpisode);
					progressBarCounter = 0;
					statusBarProgressBar.Visibility = System.Windows.Visibility.Visible;
					statusBarProgressBar.Maximum = newSelection.SelectedMovies.Count + newSelection.selectedEpisodes.Count;
					TimerRefreshLoadProgressBar.Start();
					busyIndicator.IsBusy = true;
					busyIndicator.BusyContent = "Spremanje odabira, može potrajati (max 2-3 min)";
					ThreadStart starter = delegate
					{
						DatabaseManager.InsertSelection(newSelection, ref progressBarCounter);
						this.Dispatcher.Invoke(new Action(() =>
						{
							statusBarTextBlock.Text = "Uspješno spremljen odabir (" + DateTime.Now.ToShortTimeString() + ")";
							connection.Close();
							TimerRefreshLoadProgressBar.Stop();
							busyIndicator.IsBusy = false;
							statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
						}));
					};
					Thread insertSelectionThread = new Thread(starter);
					insertSelectionThread.IsBackground = true;
					insertSelectionThread.Start();
				}
			}
			catch (Exception ex)
			{
				busyIndicator.IsBusy = false;
				MessageBox.Show(ex.Message);
				connection.Close();
				statusBarProgressBar.Visibility = System.Windows.Visibility.Hidden;
			}
		}
		private void saveSelectedHtml_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
				saveFileDlg.FileName = "Popis odabranog";   // Default file name
				saveFileDlg.DefaultExt = ".html";   // Default file extension
				saveFileDlg.Filter = "HTML document|*.html"; // Filter files by extension

				Nullable<bool> result = saveFileDlg.ShowDialog();
				if (result == true)
				{
					busyIndicator.IsBusy = true;
					busyIndicator.BusyContent = "Kreiranje html datoteke";

					ThreadStart starter = delegate
					{
						string filename = saveFileDlg.FileName;
						StreamWriter selectedTxt = new StreamWriter(filename);
						selectedTxt.WriteLine("<!DOCTYPE html>");
						selectedTxt.WriteLine("<html>");
						selectedTxt.WriteLine("<body background=\"http://www.pixeden.com/media/k2/galleries/220/002-wood-melamine-subttle-pattern-background-pat.jpg\" style=\"font-family:verdana;\">\n");

						//Filmovi
						foreach (Movie tempMovie in _movieView)
						{
							if (tempMovie.Selected)
							{
								selectedTxt.WriteLine("<table border=\"0\">");
								selectedTxt.WriteLine("\t<tr height=\"40\">");
								selectedTxt.WriteLine("\t\t<td rowspan=\"6\" width=\"210\">");
								selectedTxt.WriteLine("\t\t\t<img src=\"" + HelpFunctions.GetPosterImageUrl(tempMovie.InternetLink, tempMovie.OrigName, "movies") +
									"\" width=\"200\" height=\"287\" alt=\"Fali slika\">");
								selectedTxt.WriteLine("\t\t</td>");
								selectedTxt.WriteLine("\t\t<td><font size=\"5px\"><a href=\"" + tempMovie.InternetLink +
									"\" target=\"blank\">" + tempMovie.Name + "</font></td>\n\n\t</tr>\n");
								selectedTxt.WriteLine("\t<tr height=\"40\">");
								selectedTxt.WriteLine("\t\t<td align=\"left\"><font size=\"3px\">" + tempMovie.Year + "</font></td>\n\t</tr>");
								selectedTxt.WriteLine("\t<tr height=\"30\">\n\t\t<td align=\"left\"></td>\n\t</tr>\n");
								selectedTxt.WriteLine("\t<tr height=\"50\">");
								Converters.GenresToStringConverter genreConvert = new Converters.GenresToStringConverter();
								selectedTxt.WriteLine("\t\t<td><i><font size=\"3px\">" + genreConvert.Convert(tempMovie.Genres, null, null, null) + "</font></i></td>\n\t</tr>\n");
								selectedTxt.WriteLine("\t<tr height=\"50\">");
								selectedTxt.WriteLine("\t\t<td><img src=\"https://cdn1.iconfinder.com/data/icons/16x16-free-toolbar-icons/16/25.png\" width=\"19\" heigth=\"19\">  <font size=\"5px\">" +
									tempMovie.Rating + "</font></td>\n\t</tr>\n");
								selectedTxt.WriteLine("\t<tr height=\"50\">");
								Converters.RuntimeConverter runtimeConvert = new Converters.RuntimeConverter();
								selectedTxt.WriteLine("\t\t<td><img src=\"https://cdn2.iconfinder.com/data/icons/flat-ui-icons-24-px/24/time-24-16.png\" width=\"15\" heigth=\"15\">  <font size=\"4px\">" +
									runtimeConvert.Convert(tempMovie.Runtime, null, null, null) + "</font></td>\n\t</tr>");
								selectedTxt.WriteLine("</table>\n");
								selectedTxt.WriteLine("---------------------------------------------------------------------------------------------<br>---------------------------------------------------------------------------------------------<br>");
							}
						}
						selectedTxt.Close();

						this.Dispatcher.Invoke(new Action(() =>
						{
							busyIndicator.IsBusy = false;
							statusBarTextBlock.Text = "Uspješno spremljen odabir (" + DateTime.Now.ToShortTimeString() + ")";
						}));

					};
					Thread filterSeriesThread = new Thread(starter);
					filterSeriesThread.IsBackground = true;
					filterSeriesThread.Start();

				}
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Greška prilikom sprenmanja", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void loadAndMergeSelected_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Microsoft.Win32.OpenFileDialog openFileDlg = new Microsoft.Win32.OpenFileDialog();
			openFileDlg.DefaultExt = ".txt"; // Default file extension
			openFileDlg.Filter = "Text documents |*.txt";   // Filter files by extension

			Nullable<bool> result = openFileDlg.ShowDialog();
			try
			{
				if (result == true)
				{
					notSaved = false;
					string filename = openFileDlg.FileName;
					string readedLine;
					StreamReader selectedTxtRead = new StreamReader(filename);
					readedLine = selectedTxtRead.ReadLine();

					//najobičniji popis samo filmova bez kategorija
					if (readedLine.Contains("Veličina HDDa:") == false)
					{
						while (readedLine != null)
						{
							foreach (Movie tempMovie in listOfMovies)
								if (readedLine == tempMovie.Name)
								{
									tempMovie.Selected = true;
									break;
								}
							readedLine = selectedTxtRead.ReadLine();
						}
						selectedTxtRead.Close();
						return;
					}

					readedLine = readedLine.Replace("Veličina HDDa: ", "");

					#region učitaj filmove

					readedLine = selectedTxtRead.ReadLine();
					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("FILMOVI"))
						readedLine = selectedTxtRead.ReadLine();

					while (readedLine != "")
					{
						foreach (Movie tempMovie in listOfMovies)
							if (readedLine == tempMovie.Name)
							{
								tempMovie.Selected = true;
								break;
							}
						readedLine = selectedTxtRead.ReadLine();
					}

					#endregion

					#region učitaj serije

					while (readedLine == "" || readedLine.Contains("--") || readedLine.Contains("SERIJE"))
					{
						readedLine = selectedTxtRead.ReadLine();
						if (readedLine == null)
							break;
					}

					while (readedLine != null)
					{
						if (readedLine == "")
						{
							readedLine = selectedTxtRead.ReadLine();
							continue;
						}
						string serieName = readedLine;
						Serie currSerie = new Serie();
						foreach (Serie tempSerie in listOfSeries)
							if (tempSerie.Name == serieName)
							{
								currSerie = tempSerie;
								break;
							}
						//Sezone
						readedLine = selectedTxtRead.ReadLine();
						if (readedLine.Contains("["))
						{
							while (readedLine.Contains("["))
							{
								string seasonName = readedLine.Replace("[", "").Replace("]", "");
								SerieSeason currSeason = new SerieSeason();
								foreach (SerieSeason tempSeason in currSerie.Seasons)
								{
									if (tempSeason.Name == seasonName)
									{
										currSeason = tempSeason;
										break;
									}
								}
								readedLine = selectedTxtRead.ReadLine();

								//Episode
								if (readedLine.Contains("..Ep"))
								{
									while (readedLine.Contains("..Ep"))
									{
										string episodeName = readedLine.Replace("..", "");
										foreach (SerieEpisode tempEpisode in currSeason.Episodes)
											if (tempEpisode.Name == episodeName)
											{
												tempEpisode.Selected = true;
												break;
											}
										readedLine = selectedTxtRead.ReadLine();
									}
								}
								else
								{
									foreach (SerieEpisode tempEpisode in currSeason.Episodes)
										tempEpisode.Selected = true;
									//readedLine = selectedTxt.ReadLine ();
								}
							}
						}
						else
							foreach (SerieSeason tempSeason in currSerie.Seasons)
								foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
									tempEpisode.Selected = true;
					}

					#endregion

					selectedTxtRead.Close();

					StreamWriter selectedTxtWrite = new StreamWriter(filename);
					int counter = 0;

					#region Spremi filmove

					foreach (Movie tempMovie in listOfMovies)
						if (tempMovie.Selected)
							counter++;
					selectedTxtWrite.WriteLine(string.Format("FILMOVI ({0}):\r\n--------------", counter));
					foreach (Movie tempMovie in listOfMovies)
						if (tempMovie.Selected)
							selectedTxtWrite.WriteLine(tempMovie.Name);
					selectedTxtWrite.WriteLine("\r\n--------------");

					#endregion

					#region Spremi serije

					counter = 0;
					foreach (Serie tempSerie in listOfSeries)
						if (FetchSerieSelectedSize(tempSerie) > 0)
							counter++;
					selectedTxtWrite.WriteLine(string.Format("SERIJE ({0}):\r\n--------------", counter));
					foreach (Serie tempSerie in listOfSeries)
					{
						if (FetchSerieSelectedSize(tempSerie) > 0)
						{
							selectedTxtWrite.WriteLine(tempSerie.Name);

							if (FetchSerieSelectedSize(tempSerie) != FetchSerieFullSize(tempSerie))
								foreach (SerieSeason tempSeason in tempSerie.Seasons)
									if (FetchSeasonSelectedSize(tempSeason) > 0)
									{
										selectedTxtWrite.WriteLine("[" + tempSeason.Name + "]");

										if (FetchSeasonSelectedSize(tempSeason) != FetchSeasonFullSize(tempSeason))
											foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
												if (tempEpisode.Selected)
													selectedTxtWrite.WriteLine(".." + tempEpisode.Name);
									}
							selectedTxtWrite.WriteLine();
						}
					}

					#endregion

					selectedTxtWrite.Close();

					_movieView.Refresh();
					FillSerieTreeView();
					FillSerieTreeView();
					_movieView.Refresh();
					statusBarTextBlock.Text = "Uspješno učitana datoteka";
				}
			}
			catch
			{
				Xceed.Wpf.Toolkit.MessageBox.Show("Neispravan format datoteke s odabirom", "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void saveSelectedTxtHdd_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			try
			{
				Microsoft.Win32.SaveFileDialog saveFileDlg = new Microsoft.Win32.SaveFileDialog();
				saveFileDlg.FileName = "Raspored na HDDove"; // Default file name
				saveFileDlg.DefaultExt = ".txt"; // Default file extension
				saveFileDlg.Filter = "Text document|*.txt";	// Filter files by extension

				Nullable<bool> result = saveFileDlg.ShowDialog();
				if (result == true)
				{
					notSaved = false;
					string filename = saveFileDlg.FileName;
					StreamWriter selectedTxt = new StreamWriter(filename);
					Converters.SizeConverter sizeConv = new Converters.SizeConverter();
					foreach (HDD tempHDD in hddList)
					{
						long size = 0;
						int count = 0;

						//brojanje veličine filmova
						foreach (Movie tempMovie in listOfMovies)
						{
							if (tempMovie.Hdd.ID == tempHDD.ID && tempMovie.Selected)
							{
								count++;
								size += tempMovie.Size;
							}
						}

						//brojanje veličine epizoda
						foreach (Serie tempSerie in listOfSeries)
						{
							foreach (SerieSeason tempSeason in tempSerie.Seasons)
								foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
								{
									if (tempEpisode.Hdd.ID == tempHDD.ID && tempEpisode.Selected)
										size += tempEpisode.Size;
								}
						}

						if (size == 0)
							continue;

						double transferInMinutes = (size / (20971520)) / 60; //20 MBps transfer -> u minute

						string sizeString = sizeConv.Convert(size, typeof(string), null, null).ToString();
						selectedTxt.WriteLine(string.Format("-------------------\r\n{0} [{1} ; {2} min @ 20 MBps]:\r\n-------------------", tempHDD.Name, sizeString, transferInMinutes.ToString("0.")));

						//Upisivanje filmova
						if (count > 0)
						{
							selectedTxt.WriteLine("FILMOVI (" + count.ToString() + "):\r\n");
							foreach (Movie tempMovie in listOfMovies)
							{
								if (tempMovie.Hdd.ID == tempHDD.ID && tempMovie.Selected)
									selectedTxt.WriteLine(tempMovie.Name);
							}
							selectedTxt.WriteLine("\r\n");
						}

						//upisivanje serija
						int counter = 0;
						foreach (Serie tempSerie in listOfSeries)
							if (FetchSerieSelectedSize(tempSerie) > 0 && tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).Hdd.ID == tempHDD.ID)
								counter++;
						if (counter == 0)
						{
							selectedTxt.WriteLine("\r\n\r\n\r\n");
							continue;
						}
						selectedTxt.WriteLine(string.Format("SERIJE ({0}):\r\n", counter));
						foreach (Serie tempSerie in listOfSeries)
						{
							if (FetchSerieSelectedSize(tempSerie) > 0 && tempSerie.Seasons.ElementAt(0).Episodes.ElementAt(0).Hdd.ID == tempHDD.ID)
							{
								selectedTxt.WriteLine(tempSerie.Name);

								if (FetchSerieSelectedSize(tempSerie) != FetchSerieFullSize(tempSerie))
									foreach (SerieSeason tempSeason in tempSerie.Seasons)
										if (FetchSeasonSelectedSize(tempSeason) > 0)
										{
											selectedTxt.WriteLine("[" + tempSeason.Name + "]");

											if (FetchSeasonSelectedSize(tempSeason) != FetchSeasonFullSize(tempSeason))
												foreach (SerieEpisode tempEpisode in tempSeason.Episodes)
													if (tempEpisode.Selected)
														selectedTxt.WriteLine(".." + tempEpisode.Name);
										}
								selectedTxt.WriteLine();
							}
						}
						selectedTxt.WriteLine("\r\n\r\n\r\n");
					}
					selectedTxt.Close();
					statusBarTextBlock.Text = "Uspješno spremljen raspored na HDDove (" + DateTime.Now.ToShortTimeString() + ")";
				}
			}
			catch (Exception ex)
			{
				Xceed.Wpf.Toolkit.MessageBox.Show(ex.Message, "Greška", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#endregion

		private bool closing = false;
		private void Window_Closing(object sender, CancelEventArgs e)
		{
			closing = true;
			//return;
			if (notSaved)
			{
				if ((Xceed.Wpf.Toolkit.MessageBox.Show("Izmjene u odabiru nisu spremljene. Spremiti odabir?", "Odabir nije spremljen", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.OK))
				{
					saveSelected_Click(null, null);
				}
			}
		}
	}
}
