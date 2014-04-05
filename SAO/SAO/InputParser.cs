using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SAO
{
	class Coordinates
	{
		public Coordinates(int y, int x)
		{
			X = x;
			Y = y;
		}

		public int X { get; set; }
		public int Y { get; set; }

		public override bool Equals(Object obj)
		{
			Coordinates coords = obj as Coordinates;
			if (coords == null)
			{
				return false;
			}
			return (coords.X == X) && (coords.Y == Y);
		}

		public override int GetHashCode()
		{
			return (X << 8) + Y;
		}
	}

	public class InputParser
	{
		public static void FillRoadsAndCrossroads(ProblemInstance problemInstance, String inputFileName)
		{
			try
			{
				var reader = File.OpenText(inputFileName);

				var ySize = Convert.ToInt32(reader.ReadLine());
				var xSize = Convert.ToInt32(reader.ReadLine());
				var tileLength = Convert.ToInt32(reader.ReadLine());

				char[,] tiles = new char[ySize, xSize];
				var crossroadsDict = new Dictionary<Coordinates, Crossroad>();
				var roadsList = new List<Road>();

				for (var i = 0; i < ySize; ++i)
				{
					var line = reader.ReadLine();
					for (var j = 0; j < xSize; ++j)
					{
						var tile = line[j];
						tiles[i, j] = tile;
						if (tile == 'X')
						{
							crossroadsDict.Add(new Coordinates(i, j), new Crossroad(i * tileLength, j * tileLength));
						}
					}
				}

				var coordinates = crossroadsDict.Keys;

				foreach (Coordinates c in coordinates)
				{
					Crossroad crossroad = crossroadsDict[c];

					if (c.Y < ySize - 1)
					{
						int iterY = c.Y + 1;
						while (true)
						{
							char tile = tiles[iterY, c.X];
							if (tile == '|')
							{
								++iterY;
							}
							else if (tile == 'X')
							{
								var endCrossroad = crossroadsDict[new Coordinates(iterY, c.X)];
								var road = new Road((iterY - c.Y) * tileLength, crossroad, endCrossroad);
								crossroad.South = road;
								endCrossroad.North = road;
								roadsList.Add(road);
								break;
							}
							else
							{
								break;
							}
						}
					}

					if (c.X < xSize - 1)
					{
						int iterX = c.X + 1;
						while (true)
						{
							char tile = tiles[c.Y, iterX];
							if (tile == '-')
							{
								++iterX;
							}
							else if (tile == 'X')
							{
								var endCrossroad = crossroadsDict[new Coordinates(c.Y, iterX)];
								var road = new Road((iterX - c.X) * tileLength, crossroad, endCrossroad);
								crossroad.East = road;
								endCrossroad.West = road;
								roadsList.Add(road);
								break;
							}
							else
							{
								break;
							}
						}
					}
				}

				problemInstance.Crossroads = new List<Crossroad>(crossroadsDict.Values);
				problemInstance.Roads = roadsList;
			}

			catch (Exception e)
			{
				Console.Out.Write(e.StackTrace);
			}
		}
	}
}

