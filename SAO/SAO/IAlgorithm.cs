using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAO
{
	public interface IAlgorithm
	{
		void Run();
		Dictionary<int, TrafficLights> GetResult();
	}
}

