using System;
using System.Collections.Generic;

namespace SAO
{
	public class Route
	{
	    public Route(List<Road> list, int priority)
	    {
	        this.Roads = list;
	        this.Priority = priority;
	    }
		public List<Road> Roads { get; set; }
        public int Priority { get; set; }

		// distance will be computed
		public int Distance { get; set; }
	}
}

