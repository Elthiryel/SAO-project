using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class CarDistance
    {
        public Car Car { get; set; }
        public int Distance { get; set; }
        //Delay matters only when Distance = 0 and turn is left/right; Delay differs for left and right
        public int Delay { get; set; }
        public bool Stopped { get; set; }
        public Road Road { get; private set; }

        public CarDistance(Car car, int distance,Road road, int delay = 0 )
        {
            this.Car = car;
            this.Distance = distance;
            this.Road = road;
            this.Delay = delay;
        }
    }
}
