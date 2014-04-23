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
        public static int LeftTurnDelay = 2;
        public static int RightTurnDelay = 1;
        //chance to generate a car with route rate 100
        public static double ChanceToGenerate = 4.0;
        public static int Seed = 16431;

        public void CleanInstance()
        {
            foreach (var road in Roads)
            {
                for (int i = 0; i < road.IncreasingLaneCount; i++)
                {
                    road.IncreasingLanes[i].Clear();
                }

                for (int j = 0; j < road.DecreasingLaneCount; j++)
                {
                    road.DecreasingLanes[j].Clear();
                }
            }
        }
    }
}
