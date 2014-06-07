using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAO
{
	public class AlgorithmUtils
	{
		public static Dictionary<int, TrafficLights> Crossover(List<Crossroad> crossroads,
		                                                        Dictionary<int, TrafficLights> first, 
		                                                        Dictionary<int, TrafficLights> second)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				int northSouthDuration = (first[crossroad.Id].NorthSouthDuration + second[crossroad.Id].NorthSouthDuration) / 2;
				int westEastDuration = (first[crossroad.Id].WestEastDuration + second[crossroad.Id].WestEastDuration) / 2;
				int timeShift = (first[crossroad.Id].TimeShift + second[crossroad.Id].TimeShift) / 2;
				var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
				phenotype[crossroad.Id] = trafficLights;
			}
			return phenotype;
		}

		public static Dictionary<int, TrafficLights> CrossoverForSignificant(List<Crossroad> crossroads, 
		                                                                     HashSet<Crossroad> significantCrossroads,
		                                                                     Dictionary<int, TrafficLights> first, 
		                                                                     Dictionary<int, TrafficLights> second)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				if (significantCrossroads.Contains(crossroad))
				{
					int northSouthDuration = (first[crossroad.Id].NorthSouthDuration + second[crossroad.Id].NorthSouthDuration) / 2;
					int westEastDuration = (first[crossroad.Id].WestEastDuration + second[crossroad.Id].WestEastDuration) / 2;
					int timeShift = (first[crossroad.Id].TimeShift + second[crossroad.Id].TimeShift) / 2;
					var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
				else
				{
					int timeShift = (first[crossroad.Id].TimeShift + second[crossroad.Id].TimeShift) / 2;
					var trafficLights = new TrafficLights(60, 60, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
			}
			return phenotype;
		}

		public static Dictionary<int, TrafficLights> Mutation(List<Crossroad> crossroads, 
		                                                      Dictionary<int, TrafficLights> oldPhenotype,
		                                                      int randomizationLevel,
		                                                      Random random)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				int northSouthDuration = oldPhenotype[crossroad.Id].NorthSouthDuration + 
					random.Next(-randomizationLevel, randomizationLevel + 1);
				int westEastDuration = oldPhenotype[crossroad.Id].WestEastDuration + 
					random.Next(-randomizationLevel, randomizationLevel + 1);
				int timeShift = oldPhenotype[crossroad.Id].TimeShift + 
					random.Next(-randomizationLevel, randomizationLevel + 1);
				northSouthDuration = KeepInBounds(northSouthDuration, 15, 120);
				westEastDuration = KeepInBounds(westEastDuration, 15, 120);
				timeShift = KeepInBounds(timeShift, 0, northSouthDuration + westEastDuration - 1);
				var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
				phenotype[crossroad.Id] = trafficLights;
			}
			return phenotype;
		}

		public static Dictionary<int, TrafficLights> MutationForSignificant(List<Crossroad> crossroads, 
		                                                                    HashSet<Crossroad> significantCrossroads,
		                                                      				Dictionary<int, TrafficLights> oldPhenotype,
		                                                      				Random random)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				if (significantCrossroads.Contains(crossroad))
				{
					int northSouthDuration = oldPhenotype[crossroad.Id].NorthSouthDuration + random.Next(-10, 11);
					int westEastDuration = oldPhenotype[crossroad.Id].WestEastDuration + random.Next(-10, 11);
					int timeShift = oldPhenotype[crossroad.Id].TimeShift + random.Next(-10, 11);
					northSouthDuration = KeepInBounds(northSouthDuration, 15, 120);
					westEastDuration = KeepInBounds(westEastDuration, 15, 120);
					timeShift = KeepInBounds(timeShift, 0, northSouthDuration + westEastDuration - 1);
					var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
				else
				{
					int timeShift = oldPhenotype[crossroad.Id].TimeShift + random.Next(-10, 11);
					timeShift = KeepInBounds(timeShift, 0, 119);
					var trafficLights = new TrafficLights(60, 60, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
			}
			return phenotype;
		}

		public static Dictionary<int, TrafficLights> GenerateRandomPhenotype(List<Crossroad> crossroads, Random random)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				int northSouthDuration = random.Next(15, 121); // 15 - 120
				int westEastDuration = random.Next(15, 121); // 15 - 120
				int timeShift = random.Next(northSouthDuration + westEastDuration);
				var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
				phenotype[crossroad.Id] = trafficLights;
			}
			return phenotype;
		}

		public static Dictionary<int, TrafficLights> GenerateRandomPhenotypeForSignificant(List<Crossroad> crossroads,
		                                                                                   HashSet<Crossroad> significantCrossroads,
		                                                                                   Random random)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in crossroads)
			{
				if (significantCrossroads.Contains(crossroad))
				{
					int northSouthDuration = random.Next(15, 121); // 15 - 120
					int westEastDuration = random.Next(15, 121); // 15 - 120
					int timeShift = random.Next(northSouthDuration + westEastDuration);
					var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
				else
				{
					int timeShift = random.Next(120);
					var trafficLights = new TrafficLights(60, 60, timeShift);
					phenotype[crossroad.Id] = trafficLights;
				}
			}
			return phenotype;
		}

		public static double ComputeAverageVelocity(ProblemController controller)
		{
			double sum = 0;
			int cars = 0;
			Dictionary<Route, int> carCount;
			var timeForRoutes = controller.ComputeEachRoute(out carCount);
			foreach (var route in timeForRoutes.Keys)
			{
				sum += carCount[route] * route.Distance / timeForRoutes[route];
				cars += carCount[route];
			}
			return sum / cars;
		}

		public static int KeepInBounds(int value, int lower, int upper)
		{
			if (value < lower)
			{
				return lower;
			}
			if (value > upper)
			{
				return upper;
			}
			return value;
		}
	}
}

