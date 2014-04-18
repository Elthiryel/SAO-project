using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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

		public Road(int length, Crossroad firstCrossroad, Crossroad secondCrossroad, int laneCount, Orientation orientation)
		{
			Id = GetUniqueId();
			Length = length;
			First = firstCrossroad;
			Second = secondCrossroad;
			IncreasingLaneCount = laneCount;
			DecreasingLaneCount = laneCount;
			Orientation = orientation;
            IncreasingLanes = new LinkedList<CarDistance>[laneCount];
            DecreasingLanes = new LinkedList<CarDistance>[laneCount];
		    for (int i = 0; i < laneCount; i++)
		    {
                IncreasingLanes[i] = new LinkedList<CarDistance>();
                DecreasingLanes[i] = new LinkedList<CarDistance>();
		    }         
		}

		public int Id { get; set; }

		public Orientation Orientation { get; set; }

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

        public bool AddCarToIncreasingLane(Car car,int distanceFromCrossroad,int delay = 0)
        {
            var targetLaneIndex = GetEmptiestIncreasingLane();
            if (SpaceOnEnterIncreasingLane(targetLaneIndex))
            {
                car.RoadProgress ++;
                IncreasingLanes[targetLaneIndex].AddLast(new CarDistance(car, distanceFromCrossroad, this, delay));
            }
            else
            {
                targetLaneIndex = SpaceOnEnterIncreasingLane();
                if (targetLaneIndex != -1)
                {
                    car.RoadProgress++;
                    IncreasingLanes[targetLaneIndex].AddLast(new CarDistance(car, distanceFromCrossroad, this, delay));
                }
                    else
                {
                    return false;
                }
            }
            return true;
        }

        public bool AddCarToDecreasingLane(Car car, int distanceFromCrossroad,int delay = 0)
        {
            var targetLaneIndex = GetEmptiestDecreasingLane();
            if (SpaceOnEnterDecreasingLane(targetLaneIndex))
            {
                car.RoadProgress++;
                DecreasingLanes[targetLaneIndex].AddLast(new CarDistance(car, distanceFromCrossroad, this, delay));
            }
                
            else
            {
                targetLaneIndex = SpaceOnEnterDecreasingLane();
                if (targetLaneIndex != -1)
                {
                    car.RoadProgress++;
                    DecreasingLanes[targetLaneIndex].AddLast(new CarDistance(car, distanceFromCrossroad, this, delay));
                }
                    
                else
                {
                    return false;
                }
            }
            return true;
        }

        public int GetEmptiestIncreasingLane()
        {
            var min = IncreasingLanes[0].Count;
            var laneIndex = 0;
            for (int i = 0; i < IncreasingLaneCount; i++)
            {
                if (IncreasingLanes[i].Count < min)
                {
                    min = IncreasingLanes[i].Count;
                    laneIndex = i;
                }
            }
            return laneIndex;
        }

        public int GetEmptiestDecreasingLane()
        {
            var min = DecreasingLanes[0].Count;
            var laneIndex = 0;
            for (int i = 0; i < DecreasingLaneCount; i++)
            {
                if (DecreasingLanes[i].Count < min)
                {
                    min = DecreasingLanes[i].Count;
                    laneIndex = i;
                }
            }
            return laneIndex;
        }

        public int SpaceOnEnterDecreasingLane()
        {
            for (int i = 0; i < DecreasingLaneCount; i++)
            {
                if(DecreasingLanes[i].Last.Value.Distance + 2*Car.Length > this.Length)
                    continue;
                return i;
                
            }
            return -1;
        }

        public bool SpaceOnEnterDecreasingLane(int laneIndex)
        {
            if (DecreasingLanes[laneIndex].Count == 0)
                return true;
            if (DecreasingLanes[laneIndex].Last.Value.Distance + 2*Car.Length > this.Length)
                return false;
            return true;
        }

        public bool SpaceOnEnterIncreasingLane(int laneIndex)
        {
            if (IncreasingLanes[laneIndex].Count == 0)
                return true;
            if (IncreasingLanes[laneIndex].Last.Value.Distance + 2 * Car.Length > this.Length)
                return false;
            return true;
        }

        public int SpaceOnEnterIncreasingLane()
        {
            for (int i = 0; i < IncreasingLaneCount; i++)
            {
                if (IncreasingLanes[i].Last.Value.Distance + 2 * Car.Length > this.Length)
                    continue;
                return i;

            }
            return -1;
        }
    }

	public enum Orientation {
		NorthSouth,
		EastWest
	}
}
