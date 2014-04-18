using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class TrafficLights
    {
        public int NorthSouthDuration { get; set; }
        public int WestEastDuration { get; set; }
        public int TimeShift { get; set; }

        public TrafficLights(int northSouthDuration, int westEastDuration, int timeShift, Orientation startingOrientation = Orientation.NorthSouth)
        {
            NorthSouthDuration = northSouthDuration;
            WestEastDuration = westEastDuration;
            TimeShift = timeShift;
            StartingLightingState = startingOrientation;
        }

        public int CycleDuration
        {
            get { return NorthSouthDuration + WestEastDuration; }
        }

        public Orientation StartingLightingState { get; set; }

    }
}
