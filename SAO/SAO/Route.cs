using System;
using System.Collections.Generic;
using System.Linq;

namespace SAO
{
	public class Route
	{
	    public Route(List<Road> list, int priority, int id)
	    {
	        this.Roads = list;
	        this.Priority = priority;
	        this.Id = id;
	    }
		public List<Road> Roads { get; set; }
        public int Id { get; set; }
        public int Priority { get; set; }

		// distance will be computed
	    public int Distance
	    {
	        get { return Roads.Sum(x => x.Length); }
	    }

	    public override bool Equals(object obj)
	    {
	        var route = obj as Route;
	        if (this.Roads.Count != route.Roads.Count)
	            return false;
	        for (int i = 0; i < this.Roads.Count; i++)
	        {
	            if (route.Roads[i].Id != this.Roads[i].Id)
	                return false;
	        }
	        return true;
	    }

		public override int GetHashCode()
		{
			var toReturn = Roads.Count;
			foreach (var road in Roads)
			{
				toReturn += road.Id;
			}
			return toReturn;
		}
	}
}

