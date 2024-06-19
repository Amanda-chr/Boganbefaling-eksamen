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
        private ImporterJSON _importer;
        private List<Publikation> _publikationer;
        private List<string> _muligeGenrer;
        private List<string> _valgteGenrer;
        private List<Publikation> _anbefaledePublikationer;

        private DispatcherTimer _saveTimer;
        private DispatcherTimer _timer;
        private RandomSearchThread _randomSearchThread;
        private SearchStatistics _searchStatistics;

        private DateTime _currentDateTime;

        // Properties
        public DateTime CurrentDateTime
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

        public string SearchCounts
        {
            get { return _searchStatistics != null ? _searchStatistics.TotalSearches.ToString() : "0"; }
        }

        public List<Publikation> Publikationer
        {
            get { return _publikationer; }
            set
            {
                _publikationer = value;
                OnPropertyChanged(nameof(Publikationer));
            }
        }

        public List<string> MuligeGenrer
        {
            get { return _muligeGenrer; }
            set
            {
                _muligeGenrer = value;
                OnPropertyChanged(nameof(MuligeGenrer));
            }
        }

        public List<string> ValgteGenrer
        {
            get { return _valgteGenrer; }
            set
            {
                _valgteGenrer = value;
                OnPropertyChanged(nameof(ValgteGenrer));
            }
        }

        public List<Publikation> AnbefaledePublikationer
        {
            get { return _anbefaledePublikationer; }
            set
            {
                _anbefaledePublikationer = value;
                OnPropertyChanged(nameof(AnbefaledePublikationer));
            }
        }

        public int TotalSearches
        {
            get { return _searchStatistics.TotalSearches; }
        }

        // Commands
        public ICommand StartDateTimeCommand => new Command(_ => StartDateTime());
        public ICommand StopDateTimeCommand => new Command(_ => StopDateTime());
        public ICommand StartRandomSearchesCommand => new Command(_ => StartRandomSearches());
        public ICommand StopRandomSearchesCommand => new Command(_ => StopRandomSearches());
        public ICommand SaveSearchHistoryCommand => new Command(_ => SaveSearchHistory());
        public ICommand AnbefalCommand => new Command(_ => AnbefalBoeger());

        // Constructor
        public MainViewModel()
        {
            _importer = new ImporterJSON(@"C:\Users\Amand\Desktop\Eksamen_VP\PublikationData.json");
            Publikationer = _importer.ImporterPublikationer();
            MuligeGenrer = _importer.HentGenrertilWPF();
            ValgteGenrer = new List<string>();

            // Initialize and start the timer for updating the current time
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            // Initialize search statistics
            _searchStatistics = new SearchStatistics(MuligeGenrer);
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
        private void InitializeSearchCount()
        {
            int currentHour = DateTime.Now.Hour;
            int realisticSearchCount = new Random().Next(10, 50) * currentHour;
            for (int i = 0; i < realisticSearchCount; i++)
            {
                _searchStatistics.AddSearch(new List<string> { "" });
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            CurrentDateTime = DateTime.Now;
            OnPropertyChanged(nameof(CurrentDateTime));
        }

        private void SaveTimer_Tick(object sender, EventArgs e)
        {
            SaveSearchHistory();
        }

        private void SaveSearchHistory()
        {
            string folderPath = @"C:\Users\Amand\Desktop\Eksamen_VP\SearchHistory.csv";
            Directory.CreateDirectory(folderPath);

            string fileName = $"SearchHistory{DateTime.Now:yyyyMMdd}.csv";
            string filePath = Path.Combine(folderPath, fileName);

            var searchHistoryData = _searchStatistics.GetSearchHistoryData();
            CsvHelper.WriteSearchHistoryToCsv(filePath, searchHistoryData);
        }

        private void AnbefalBoeger()
        {
            if (ValgteGenrer.Any())
            {
                AnbefaledePublikationer = Publikationer
                    .Select(p =>
                    {
                        double matchCount = p.Genrer.Count(g => ValgteGenrer.Contains(g, StringComparer.OrdinalIgnoreCase));
                        p.MatchPercentage = matchCount / ValgteGenrer.Count;
                        return p;
                    })
                    .Where(p => p.MatchPercentage > 0)
                    .OrderByDescending(p => p.MatchPercentage)
                    .ToList();

                _searchStatistics.AddSearch(ValgteGenrer);
            }
            else
            {
                AnbefaledePublikationer = new List<Publikation>();
            }
        }

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

        // Event and method for property change notification
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Private class for command implementation
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
