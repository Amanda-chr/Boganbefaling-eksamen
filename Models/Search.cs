using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    public class Search
    {
        public DateTime Timestamp { get; set; }
        public List<string> ValgteGenrer { get; set; }

        public Search(DateTime timestamp, List<string> valgteGenrer) 
        { 
            Timestamp = timestamp;
            ValgteGenrer = valgteGenrer;
        }
    }
}
