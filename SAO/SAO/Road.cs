using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class Road
    {
		private static int lastId = -1;

		private static int GetUniqueId()
		{
			++lastId;
			return lastId;
		}

		public Road(int length, Crossroad firstCrossroad, Crossroad secondCrossroad)
		{
			Id = GetUniqueId();
			Length = length;
			First = firstCrossroad;
			Second = secondCrossroad; // TODO what about lanes?
		}

		public int Id { get; set; }

        //get later from Crossroads
        public int Y { get; set; } // what is x or y? road is NOT a point :P
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
