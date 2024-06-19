using Boganbefaling_eksamen.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using CsvHelper;

namespace Boganbefaling_eksamen
{
    public class MainViewModel : INotifyPropertyChanged
    {

        // Private fields
        private ImportJson _import;
        private List<Publication> _publications;
        private List<string> _possibleGenres;
        private List<string> _selectedGenres;
        private List<Publication> _recommendedPublications;

        private DispatcherTimer _saveTimer;
        private DispatcherTimer _timer;
        private RandomSearchThread _randomSearchThread;
        private SearchStatistics _searchStatistics;

        private DateTime _currentDateTime;

        // Properties
        public DateTime CurrentDateTime //gets and sets the time, updates the changes.
        {
            get { return _currentDateTime; }
            set
            {
                if (_currentDateTime != value)
                {
                    _currentDateTime = value;
                    OnPropertyChanged(nameof(CurrentDateTime));
                }
            }
        }

        public string SearchCounts //if SearchStatistics is not null, returns the total numbers of searches to a string
        {
            get { return _searchStatistics != null ? _searchStatistics.TotalSearches.ToString() : "0"; }
        }

        public List<Publication> Publications //get and set _publications. Updates _publications with new values
        {
            get { return _publications; }
            set
            {
                _publications = value;
                OnPropertyChanged(nameof(Publications));
            }
        }

        public List<string> PossibleGenres
        {
            get { return _possibleGenres; }
            set
            {
                _possibleGenres = value;
                OnPropertyChanged(nameof(PossibleGenres));
            }
        }

        public List<string> SelectedGenres
        {
            get { return _selectedGenres; }
            set
            {
                _selectedGenres = value;
                OnPropertyChanged(nameof(SelectedGenres));
            }
        }

        public List<Publication> RecommendedPublications
        {
            get { return _recommendedPublications; }
            set
            {
                _recommendedPublications = value;
                OnPropertyChanged(nameof(RecommendedPublications));
            }
        }

        public int TotalSearches //returns the TotalSearches from _searchStatistics
        {
            get { return _searchStatistics.TotalSearches; }
        }

        // ICommands - handles user actions. 
        public ICommand StartDateTimeCommand => new Command(_ => StartDateTime());
        public ICommand StopDateTimeCommand => new Command(_ => StopDateTime());
        public ICommand StartRandomSearchesCommand => new Command(_ => StartRandomSearches());
        public ICommand StopRandomSearchesCommand => new Command(_ => StopRandomSearches());
        public ICommand SaveSearchHistoryCommand => new Command(_ => SaveSearchHistory());
        public ICommand RecommendCommand => new Command(_ => RecommendBooks());

        // Constructor
        public MainViewModel()
        {
            //JSON is imported, loaded in Publications, genres are saved in PossibleGenres. SelectedGenres contains user input
            _import = new ImportJson(@"C:\Users\Amand\source\repos\Boganbefaling eksamen\ImportExport\PublikationData.json");
            Publications = _import.LoadPublicationsFromJson();
            PossibleGenres = _import.GetGenresToWPF();
            SelectedGenres = new List<string>();

            // Initialize and start the timer for updating the current time
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Initialize search statistics
            _searchStatistics = new SearchStatistics(PossibleGenres);
            InitializeSearchCount();
            _searchStatistics.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(SearchStatistics.TotalSearches))
                {
                    OnPropertyChanged(nameof(TotalSearches));
                }
            };

            // Initialize and start the random search thread
            _randomSearchThread = new RandomSearchThread(_searchStatistics);
            _randomSearchThread.Start();

            // Initialize and start the save timer for periodic saving of search history
            _saveTimer = new DispatcherTimer();
            _saveTimer.Interval = TimeSpan.FromMinutes(1);
            _saveTimer.Tick += SaveTimer_Tick;
            _saveTimer.Start();
        }

        // Methods
        //Generates a random search based on what time it is now
        private void InitializeSearchCount() 
        {
            int currentHour = DateTime.Now.Hour;
            int realisticSearchCount = new Random().Next(10, 50) * currentHour;
            for (int i = 0; i < realisticSearchCount; i++)
            {
                _searchStatistics.AddSearch(new List<string> { "" });
            }
        }

        //Method that updates current time
        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now;
            OnPropertyChanged(nameof(CurrentDateTime));
        }

        //Method that saves to the CSV file
        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            SaveSearchHistory();
        }

        //creates the CSV file and defines where to store it
        private void SaveSearchHistory()
        {
            string folderPath = @"C:\Users\Amand\source\repos\Boganbefaling eksamen\ImportExport\SearchHistory.csv";
            Directory.CreateDirectory(folderPath);

            string fileName = $"SearchHistory{DateTime.Now:yyyyMMdd}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            var searchHistoryData = _searchStatistics.GetSearchHistoryData();
            CsvHelper.WriteSearchHistoryToCsv(filePath, searchHistoryData);
        }

        //Method that recommends books based on the selceted genres. The publications with the most matching genres are shown first
        private void RecommendBooks()
        {
            if (SelectedGenres.Any())
            {
                RecommendedPublications = Publications
                    .Select(p =>
                    {
                        double matchCount = p.Genres.Count(g => SelectedGenres.Contains(g, StringComparer.OrdinalIgnoreCase));
                        p.MatchPercentage = matchCount / SelectedGenres.Count;
                        return p;
                    })
                    .Where(p => p.MatchPercentage > 0)
                    .OrderByDescending(p => p.MatchPercentage)
                    .ToList();

                _searchStatistics.AddSearch(SelectedGenres);
            }
            else
            {
                RecommendedPublications = new List<Publication>();
            }
        }

        //starts and stops the tthreads
        private void StartDateTime()
        {
            _timer.Start();
        }

        private void StopDateTime()
        {
            _timer.Stop();
        }

        private void StartRandomSearches()
        {
            _randomSearchThread.Start();
        }

        private void StopRandomSearches()
        {
            _randomSearchThread.Stop();
        }

        // Event and method for property change updates
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Private class for ICommand implementation
        private class Command : ICommand
        {
            private readonly Action<object> _execute;

            public Command(Action<object> execute)
            {
                _execute = execute;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                _execute(parameter);
            }

            public event EventHandler CanExecuteChanged
            {
                add { }
                remove { }
            }
        }
    }
}
