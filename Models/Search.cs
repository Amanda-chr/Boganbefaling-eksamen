using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public class Search
    {
        //the class that represents individual searches and is used in the search history tracking
        public DateTime Timestamp { get; set; }
        public List<string> SelectedGenres { get; set; }

        public Search(DateTime timestamp, List<string> selectedGenres) 
        { 
            Timestamp = timestamp;
            SelectedGenres = selectedGenres;
        }
    }
}
