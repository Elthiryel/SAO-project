using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAO
{
	public class Intensity
	{
		public Intensity()
		{
			NorthSouth = 0;
			EastWest = 0;
		}

		public int NorthSouth { get; set; }
		public int EastWest { get; set; }

	    public override bool Equals(object obj)
	    {
	        var intensity = obj as Intensity;
			if (intensity == null)
			{
				return false;
			}
	        return ((intensity.NorthSouth == NorthSouth) && (intensity.EastWest == EastWest));
	    }

		public override int GetHashCode()
		{
			return NorthSouth ^ EastWest;
		}
	}

	public class CrossroadDataComparer : IComparer<KeyValuePair<Crossroad, Intensity>>
	{
		public int Compare(KeyValuePair<Crossroad, Intensity> x, KeyValuePair<Crossroad, Intensity> y)
		{
			var xValue = Math.Abs(x.Value.EastWest - x.Value.NorthSouth);
			var yValue = Math.Abs(y.Value.EastWest - y.Value.NorthSouth);
			return xValue - yValue;
		}
	}

	public class RandomStartHierarchicGeneticAlgorithm : IAlgorithm
	{
		private int phenotypeCount;
		private int iterationCount;
		private int secondCount;
		private ProblemInstance problemInstance;
		private Random random;

		private List<Dictionary<int, TrafficLights>> phenotypes;

		private Dictionary<int, TrafficLights> bestConfiguration;
		private double bestVelocity;

		private HashSet<Crossroad> significantCrossroads;

		public RandomStartHierarchicGeneticAlgorithm(ProblemInstance problemInstance, int phenotypeCount, int iterationCount, 
		                                   			 int secondCount)
		{
			this.problemInstance = problemInstance;
			this.phenotypeCount = phenotypeCount;
			this.iterationCount = iterationCount;
			this.secondCount = secondCount;
			random = new Random();
		}

		private HashSet<Crossroad> GetSignificantCrossroads()
		{
			Dictionary<Crossroad, Intensity> crossroadHierarchy = new Dictionary<Crossroad, Intensity>();
			foreach (var route in problemInstance.Routes)
			{
				for (var i = 0; i < route.Roads.Count - 1; ++i)
				{
					var currentRoad = route.Roads[i];
					var nextRoad = route.Roads[i + 1];
					Crossroad currentCrossroad;
					if ((nextRoad.First == currentRoad.First) || (nextRoad.Second == currentRoad.First))
					{
						currentCrossroad = currentRoad.First;
					}
					else
					{
						currentCrossroad = currentRoad.Second;
					}
					if (crossroadHierarchy.ContainsKey(currentCrossroad))
					{
						if (currentRoad.Orientation == Orientation.NorthSouth)
						{
							crossroadHierarchy[currentCrossroad].NorthSouth += route.Priority;
						}
						else
						{
							crossroadHierarchy[currentCrossroad].EastWest += route.Priority;
						}
					}
					else
					{
						crossroadHierarchy[currentCrossroad] = new Intensity();
						if (currentRoad.Orientation == Orientation.NorthSouth)
						{
							crossroadHierarchy[currentCrossroad].NorthSouth = route.Priority;
						}
						else
						{
							crossroadHierarchy[currentCrossroad].EastWest = route.Priority;
						}
					}
				}
			}
			var crossroadDataList = new List<KeyValuePair<Crossroad, Intensity>>(problemInstance.Crossroads.Count);
			foreach (var crossroadData in crossroadHierarchy)
			{
				crossroadDataList.Add(crossroadData);
			}
			crossroadDataList.Sort(new CrossroadDataComparer());
			int half = problemInstance.Crossroads.Count / 2;
			if (crossroadDataList.Count > half)
			{
				crossroadDataList.RemoveRange(half, crossroadDataList.Count - half);
			}
			var crossroadSet = new HashSet<Crossroad>();
			foreach (var crossroadData in crossroadDataList)
			{
				crossroadSet.Add(crossroadData.Key);
			}
			return crossroadSet;
		}

		public void Run()
		{
			bestVelocity = 0;
			significantCrossroads = GetSignificantCrossroads();
			GenerateInitialPhenotypes();
			for (var i = 0; i < iterationCount; ++i)
			{
				PerformIteration();
				Console.WriteLine("iteration " + i + ": " + bestVelocity);
			}
		}

		public Dictionary<int, TrafficLights> GetResult()
		{
			return bestConfiguration;
		}

		private void GenerateInitialPhenotypes()
		{
			phenotypes = new List<Dictionary<int, TrafficLights>>(phenotypeCount);
			for (var i = 0; i < phenotypeCount; ++i)
			{
				phenotypes.Add(AlgorithmUtils.GenerateRandomPhenotypeForSignificant(problemInstance.Crossroads, 
				                                                                    significantCrossroads,
				                                                                    random));
			}
		}

		private void PerformIteration()
		{
			var simulationResult = new List<double>(phenotypeCount);
			//Console.WriteLine("--- --- ---");
			for (var i = 0; i < phenotypeCount; ++i)
			{
				var controller = new ProblemController(problemInstance);
				controller.SetTrafficLightsConfiguration(phenotypes[i]);
				controller.Start(secondCount);
				var averageVelocity = AlgorithmUtils.ComputeAverageVelocity(controller);
				//Console.Write(averageVelocity + ", ");
				simulationResult.Add(averageVelocity);
			}
			//Console.WriteLine("--- --- ---");
			SetBest(simulationResult);
			PerformEvolution(simulationResult);
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
				newPhenotypes.Add(AlgorithmUtils.MutationForSignificant(problemInstance.Crossroads, significantCrossroads, 
				                                                        bestConfigurations[i], random));
			}
			for (var i = half; i < phenotypeCount; ++i)
			{
				var first = random.Next(phenotypeCount);
				var second = random.Next(phenotypeCount);
				newPhenotypes.Add(AlgorithmUtils.CrossoverForSignificant(problemInstance.Crossroads, significantCrossroads, 
				                                                         phenotypes[first], phenotypes[second]));
			}
			phenotypes = newPhenotypes;
		}
	}
}

