using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SAO
{
	public class RandomStartAdaptiveGeneticAlgorithm : RandomStartGeneticAlgorithm
	{
		private int stuck;

		public RandomStartAdaptiveGeneticAlgorithm(ProblemInstance problemInstance, int phenotypeCount, int iterationCount, 
		                                           int secondCount) : 
			base(problemInstance, phenotypeCount, iterationCount, secondCount)
		{
			this.stuck = 0;
		}

		protected override void SetBest(List<double> simulationResult)
		{
			var oldBestVelocity = bestVelocity;
			for (var i = 0; i < simulationResult.Count; ++i)
			{
				if (simulationResult[i] > bestVelocity)
				{
					bestVelocity = simulationResult[i];
					bestConfiguration = phenotypes[i];
				}
			}
			if (oldBestVelocity == bestVelocity)
			{
				if (randomizationLevel < 40)
				{
					if (stuck > 1)
					{
						randomizationLevel += 5;
					}
					else
					{
						++stuck;
					}
				}
			}
			else
			{
				randomizationLevel = 10;
				stuck = 0;
			}
		}
	}
}

