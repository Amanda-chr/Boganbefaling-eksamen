using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Boganbefaling_eksamen.Models
{
    public class RandomSearchThread
    {
        private readonly SearchStatistics _searchStatistics;
        private readonly Random _random;
        private bool _running;
        private Thread _thread;

        public RandomSearchThread(SearchStatistics searchStatistics)
        {
            _searchStatistics = searchStatistics;
            _random = new Random();
        }

        public void Start()
        {
            _running = true;
            _thread = new Thread(Run);
            _thread.Start();
        }

        public void Stop()
        {
            _running = false;
            _thread.Join();

        }

        private void Run()
        {
            while (_running)
            {
                int numberOfSearches = _random.Next(1, 6);
                for (int i = 0; i < numberOfSearches; i++)
                {
                    GenerateRandomSearch();
                }
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }

        private void GenerateRandomSearch()
        {
            int genreCount = _random.Next(1, 4);
            List<string> availableGenres = _searchStatistics.MuligeGenrer;
            List<string> selectedGenres = availableGenres.OrderBy(x => _random.Next()).Take(genreCount).ToList();

            _searchStatistics.AddSearch(selectedGenres);
        }
    }
}
