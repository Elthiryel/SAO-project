using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class Road
    {
		public int Id { get; set; }

        //get later from Crossroads
        public int Y { get; set; }
        public int X { get; set; }
        public int Length { get; set; }

        //lesser coordinates 
        public Crossroad First { get; set; }

        public int IncreasingLaneCount { get; set; }
        public int DecreasingLaneCount { get; set; }

        //bigger coordinates
        public Crossroad Second { get; set; }

        public LinkedList<CarDistance>[] IncreasingLanes { get; set; }
        public LinkedList<CarDistance>[] DecreasingLanes { get; set; }

        public void AddCarToLinkedListWithParameterDistance()
        {
            
        }
    }
}
