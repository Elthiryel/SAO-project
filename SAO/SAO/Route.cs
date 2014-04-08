using System;
using System.Collections.Generic;

namespace SAO
{
	public class Route
	{
		public List<Road> Roads { get; set; }
        public int Priority { get; set; }

		// distance will be computed
		public int Distance { get; set; }
	}
}

