using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boganbefaling_eksamen
{
    internal class Comic : Publication
    {
        public int NumOfPages { get; set; }
        public string Illustrator { get; set; }
        public bool Color { get; set; }
        public bool Black_White { get; set; }

    }
}
