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

		protected int randomizationLevel;

		protected List<Dictionary<int, TrafficLights>> phenotypes;

		protected Dictionary<int, TrafficLights> bestConfiguration;
		protected double bestVelocity;

		public RandomStartGeneticAlgorithm(ProblemInstance problemInstance, int phenotypeCount, int iterationCount, 
		                                   int secondCount)
		{
			this.randomizationLevel = 10;
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
				Console.WriteLine("iteration " + i + ": " + bestVelocity + " | rL: " + randomizationLevel);
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
				phenotypes.Add(AlgorithmUtils.GenerateRandomPhenotype(problemInstance.Crossroads, random));
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

		protected virtual void SetBest(List<double> simulationResult)
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
				newPhenotypes.Add(AlgorithmUtils.Mutation(problemInstance.Crossroads, bestConfigurations[i], 
				                                          randomizationLevel, random));
			}
			for (var i = half; i < phenotypeCount; ++i)
			{
				var first = random.Next(phenotypeCount);
				var second = random.Next(phenotypeCount);
				newPhenotypes.Add(AlgorithmUtils.Crossover(problemInstance.Crossroads, phenotypes[first], phenotypes[second]));
			}
			phenotypes = newPhenotypes;
		}
	}
}

