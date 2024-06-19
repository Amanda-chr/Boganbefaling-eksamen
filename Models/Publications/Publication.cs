using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public class Publication
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public List<string> Genres { get; set; }
        public int PublicationYear { get; set; }
        public string Publisher { get; set; }
        public double MatchPercentage { get; set; }
    }
}
