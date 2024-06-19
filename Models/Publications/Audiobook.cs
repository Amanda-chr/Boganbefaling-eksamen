using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    internal class Audiobook : Publication
    {
        public string Narrator { get; set; }
        public double LengthInMinutes { get; set; }
    }
}
