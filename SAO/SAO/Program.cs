using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAO
{
    class Program
    {
        static void Main(string[] args)
        {
			// TESTING PARSER
			ProblemInstance pi = new ProblemInstance();
			InputParser.FillRoadsAndCrossroads(pi, "test.txt");
            InputParser.FillRoutes(pi, "routes.xml");
            /*
			foreach (Road r in pi.Roads)
			{
				Console.Write("ROAD " + r.Id + ": " + r.Length);
				if (r.First != null)
				{
					Console.Write("; first: " + r.First.X + ", " + r.First.Y);
				}
				if (r.Second != null)
				{
					Console.Write("; second: " + r.Second.X + ", " + r.Second.Y);
				}
				Console.Write( "; lanes: " + r.IncreasingLaneCount + ", " + r.DecreasingLaneCount);
				Console.Write( "; orientation: " + r.Orientation);
				Console.WriteLine();
			}
			foreach (Crossroad c in pi.Crossroads)
			{
				Console.Write("CROSSROAD " + c.Id + ": " + c.X + ", " + c.Y);
				if (c.North != null)
				{
					Console.Write("; north: " + c.North.Id); 
				}
				if (c.South != null)
				{
					Console.Write("; south: " + c.South.Id); 
				}
				if (c.West != null)
				{
					Console.Write("; west: " + c.West.Id); 
				}
				if (c.East != null)
				{
					Console.Write("; east: " + c.East.Id); 
				}
				Console.WriteLine();
			}*/
            /*ProblemController controller = new ProblemController(pi);
            Dictionary<int,TrafficLights> lightsConfig = new Dictionary<int, TrafficLights>();
            foreach (var crossroad in pi.Crossroads)
            {
				TrafficLights lights = new TrafficLights(10,30,6);
                lightsConfig.Add(crossroad.Id,lights);
            }
            controller.SetTrafficLightsConfiguration(lightsConfig);
            controller.Start(10000);
            var result = controller.ComputeResult();
            Console.WriteLine("Average route time in seconds: "+result);
            Console.WriteLine(String.Format("Total number of cars: {0}",controller.ArchivedCarData.Count));
			Dictionary<Route, int> carCount;
            var dict = controller.ComputeEachRoute(out carCount);
            foreach (var route in dict.Keys)
            {
                Console.WriteLine("Average route time in seconds: " + dict[route] + " for Route number "+ route.Id + 
				                  " (" + carCount[route] + " cars)");
            }
            Console.ReadKey(); */
			var algorithm = new RandomStartGeneticAlgorithm(pi, 20, 50, 10000);
			algorithm.Run();
			var result = algorithm.GetResult();
			Console.WriteLine("END, result: " + result);
        }
    }
}
