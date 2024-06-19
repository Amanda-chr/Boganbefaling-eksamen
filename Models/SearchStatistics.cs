using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Boganbefaling_eksamen
{
    public class SearchStatistics : INotifyPropertyChanged
    {
        //Class that organizes the statistics of my searches and manages the search history
        private Dictionary<string, int> _genreCount;
        private int _totalSearches;
        private Dictionary<DateTime, SearchHistoryEntry> _searchHistory;

        public List<string> PossibleGenres { get; private set; }
        public int TotalSearches
        {
            get { return _totalSearches; }
            private set
            {
                _totalSearches = value;
                OnPropertyChanged(nameof(TotalSearches));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        //intitalizes _genreCount and _searchHistory
        public SearchStatistics(List<string> possibleGenres)
        {
            _genreCount = new Dictionary<string, int>();
            _searchHistory = new Dictionary<DateTime, SearchHistoryEntry>();
            PossibleGenres = possibleGenres;
            TotalSearches = 0;
        }

        //Method that adds to the TotalSearches
        public void AddSearch(List<string> genres)
        {
            TotalSearches++;
            foreach (var genre in genres)
            {
                if (_genreCount.ContainsKey(genre))
                {
                    _genreCount[genre]++;
                }
                else
                {
                    _genreCount[genre] = 1;
                }
            }
            OnPropertyChanged(nameof(SearchCounts));

            DateTime now = DateTime.Now;
            UpdateSummary(now.Date, genres);
        }

        //Gets the search count for specific genres
        public int GetSearchCount(string genre)
        {
            if (_genreCount.ContainsKey(genre))
            {
                return _genreCount[genre];
            }
            else
            {
                return 0;
            }
        }

        public Dictionary<string, int> GetAllSearchCounts()
        {
            return _genreCount;
        }

        public string SearchCounts
        {
            get
            {
                return string.Join(", ", _genreCount.Select(kv => $"{kv.Key}: {kv.Value}"));
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //Adding a formatted line with the search and timestamp to the CSV file
        public void LogSearch(DateTime timestamp, List<string> genres, string filePath)
        {
            string csvLine = $"{timestamp:yyyy-MM-ddTHH:mm:ss},{string.Join(",", genres)}";
            File.AppendAllLines(filePath, new[] { csvLine });
        }

        public void AddSearch(DateTime timestamp, List<string> genres, string logFilePath)
        {
            LogSearch(timestamp, genres, logFilePath);
            UpdateSummary(timestamp.Date, genres);
        }

        //Updates _searchHistory
        public void UpdateSummary(DateTime date, List<string> genres)
        {
            if (!_searchHistory.ContainsKey(date))
            {
                _searchHistory[date] = new SearchHistoryEntry
                {
                    Date = date,
                    TotalSearches = 0,
                    GenreCounts = new Dictionary<string, int>()
                };
            }

            _searchHistory[date].TotalSearches++;

            foreach (var genre in genres)
            {
                if (_searchHistory[date].GenreCounts.ContainsKey(genre))
                {
                    _searchHistory[date].GenreCounts[genre]++;
                }
                else
                {
                    _searchHistory[date].GenreCounts[genre] = 1;
                }
            }
        }

        public List<SearchHistoryEntry> GetSearchHistoryData()
        {
            return _searchHistory.Values.ToList();
        }

        //Writes to the CSV file
        public void SaveSearchHistoryToCSV(string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("Date, TotalSearches, GenreCounts");

                foreach (var entry in _searchHistory.Values)
                {
                    string date = entry.Date.ToString("yyyy-MM-dd");
                    string totalSearches = entry.TotalSearches.ToString();
                    string genreCounts = string.Join(",", entry.GenreCounts.Select(kv => $"{kv.Key}:{kv.Value}"));

                    writer.WriteLine($"{date}, {totalSearches}, {genreCounts}");
                }
            }
        }

        public class SearchHistoryEntry
        {
            public DateTime Date { get; set; }
            public int TotalSearches { get; set; }
            public Dictionary<string, int> GenreCounts { get; set; }
        }
    }
}
