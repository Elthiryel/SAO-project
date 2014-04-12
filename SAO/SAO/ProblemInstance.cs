using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class ProblemInstance
    {
        public List<Road> Roads { get; set; }
        public List<Crossroad> Crossroads { get; set; }
        public List<Route> Routes { get; set; }

        public static ProblemInstance GenerateInstance(string path)
        {
            return null;
        }

        public static int CarSpeed = 1;
    }
}
