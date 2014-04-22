using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAO
{
	public class RandomStartGeneticAlgorithm : IAlgorithm
	{
		private int phenotypeCount;
		private int iterationCount;
		private int secondCount;
		private ProblemInstance problemInstance;
		private Random random;

		private List<Dictionary<int, TrafficLights>> phenotypes;

		private Dictionary<int, TrafficLights> bestConfiguration;
		private double bestVelocity;

		public RandomStartGeneticAlgorithm(ProblemInstance problemInstance, int phenotypeCount, int iterationCount, 
		                                   int secondCount)
		{
			this.problemInstance = problemInstance;
			this.phenotypeCount = phenotypeCount;
			this.iterationCount = iterationCount;
			this.secondCount = secondCount;
			random = new Random();
		}

		public void Run()
		{
			bestVelocity = 0;
			GenerateInitialPhenotypes();
			for (var i = 0; i < iterationCount; ++i)
			{
				PerformIteration();
			}
		}

		public Dictionary<int, TrafficLights> GetResult()
		{
			return bestConfiguration;
		}

		private Dictionary<int, TrafficLights> GenerateRandomPhenotype()
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in problemInstance.Crossroads)
			{
				int northSouthDuration = random.Next(15, 121); // 15 - 120
				int westEastDuration = random.Next(15, 121); // 15 - 120
				int timeShift = random.Next(northSouthDuration + westEastDuration);
				var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
				phenotype[crossroad.Id] = trafficLights;
			}
			return phenotype;
		}

		private void GenerateInitialPhenotypes()
		{
			phenotypes = new List<Dictionary<int, TrafficLights>>(phenotypeCount);
			for (var i = 0; i < phenotypeCount; ++i)
			{
				phenotypes.Add(GenerateRandomPhenotype());
			}
		}

		private Dictionary<int, TrafficLights> Crossover(Dictionary<int, TrafficLights> first, 
		                                                Dictionary<int, TrafficLights> second)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in problemInstance.Crossroads)
			{
				int northSouthDuration = (first[crossroad.Id].NorthSouthDuration + second[crossroad.Id].NorthSouthDuration) / 2;
				int westEastDuration = (first[crossroad.Id].WestEastDuration + second[crossroad.Id].WestEastDuration) / 2;
				int timeShift = (first[crossroad.Id].TimeShift + second[crossroad.Id].TimeShift) / 2;
				var trafficLights = new TrafficLights(northSouthDuration, westEastDuration, timeShift);
				phenotype[crossroad.Id] = trafficLights;
			}
			return phenotype;
		}

		private Dictionary<int, TrafficLights> Mutation(Dictionary<int, TrafficLights> oldPhenotype)
		{
			var phenotype = new Dictionary<int, TrafficLights>();
			foreach (var crossroad in problemInstance.Crossroads)
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
			return phenotype;
		}

		private int KeepInBounds(int value, int lower, int upper)
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

		private void PerformIteration()
		{
			var simulationResult = new List<double>(phenotypeCount);
			for (var i = 0; i < phenotypeCount; ++i)
			{

				foreach (var p in phenotypes[i].Values)
				{
					Console.WriteLine(p.NorthSouthDuration + ", " + p.WestEastDuration + ", " + p.TimeShift);
				}

				var controller = new ProblemController(problemInstance);
				controller.SetTrafficLightsConfiguration(phenotypes[i]);
				controller.Start(secondCount);

				Console.WriteLine(controller.ComputeResult());

				var averageVelocity = ComputeAverageVelocity(controller);
				simulationResult.Add(averageVelocity);
			}
			SetBest(simulationResult);
			PerformEvolution(simulationResult);
		}

		private double ComputeAverageVelocity(ProblemController controller)
		{
			double sum = 0;
			int cars = 0;
			Dictionary<Route, int> carCount;
			var velocityForRoutes = controller.ComputeEachRoute(out carCount);
			foreach (var route in velocityForRoutes.Keys)
			{
				sum += velocityForRoutes[route] * carCount[route];
				cars += carCount[route];
			}
			return sum / cars;
		}

		private void SetBest(List<double> simulationResult)
		{
			for (var i = 0; i < simulationResult.Count; ++i)
			{
				if (simulationResult[i] > bestVelocity)
				{
					bestVelocity = simulationResult[i];
					bestConfiguration = phenotypes[i];
				}
			}
		}

		private void PerformEvolution(List<double> simulationResult)
		{
			int half = phenotypeCount / 2;
			double[] bestVelocities = new double[half];
			Dictionary<int, TrafficLights>[] bestConfigurations = new Dictionary<int, TrafficLights>[half];
			Array.Clear(bestVelocities, 0, half);
			Array.Clear(bestConfigurations, 0, half);
			for (var i = 0; i < phenotypeCount; ++i)
			{
				int putPosition = -1;
				for (var j = 0; j < half; ++j)
				{
					if (simulationResult[i] > bestVelocities[j])
					{
						putPosition = j;
						break;
					}
				}
				if (putPosition > -1)
				{
					for (var j = half - 1; j > putPosition; --j)
					{
						bestVelocities[j] = bestVelocities[j - 1];
						bestConfigurations[j] = bestConfigurations[j - 1];
					}
					bestVelocities[putPosition] = simulationResult[i];
					bestConfigurations[putPosition] = phenotypes[i];
				}
			}
			var newPhenotypes = new List<Dictionary<int, TrafficLights>>(phenotypeCount);
			for (var i = 0; i < half; ++i)
			{
				newPhenotypes.Add(Mutation(bestConfigurations[i]));
			}
			for (var i = half; i < phenotypeCount; ++i)
			{
				var first = random.Next(phenotypeCount);
				var second = random.Next(phenotypeCount);
				newPhenotypes.Add(Crossover(bestConfigurations[first], bestConfigurations[second]));
			}
			phenotypes = newPhenotypes;
		}
	}
}

