using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    internal class Tegneserie : Publikation
    {
        public int AntalSider { get; set; }
        public string Illustrator { get; set; }
        public bool Farve { get; set; }
        public bool Sort_Hvid { get; set; }

    }
}
