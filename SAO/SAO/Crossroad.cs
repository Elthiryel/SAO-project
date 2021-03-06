﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    public class Crossroad
    {
		private static int lastId = -1;

		private static int GetUniqueId()
		{
			++lastId;
			return lastId;
		}

		public Crossroad(int y, int x)
		{
			Id = GetUniqueId();
			X = x;
			Y = y;
			North = null;
			South = null;
			West = null;
			East = null;
		}

		public int Id { get; set; }

        public int X { get; set; }
        public int Y { get; set; }

        //increase Y
        public Road North { get; set; }
        //decrease Y
        public Road South { get; set; }
        //decrease X
        public Road West { get; set; }
        //increase X
        public Road East { get; set; }

		public TrafficLights Lights { get; set; }

        public Orientation LightsState { get; set; }

        public List<Road> Roads
        {
            get
            {
                var list =  new List<Road>(){North,South,West,East};
                list.RemoveAll(item => item == null);
                return list;
            }
        }

        public void SwitchLightState()
        {
            if (LightsState == Orientation.NorthSouth)
                LightsState = Orientation.EastWest;
            else
                LightsState = Orientation.NorthSouth;
        }
    }

    public enum TrafficLightsState
    {
        NorthSouth,
        WestEast
    }
}
