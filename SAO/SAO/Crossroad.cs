using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class Crossroad
    {
        //
        public Road North { get; set; }
        public Road South { get; set; }
        public Road West { get; set; }
        public Road East { get; set; }
    }
}
