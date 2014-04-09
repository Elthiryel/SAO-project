using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;

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
						int lanes = 1;
						int iterY = c.Y + 1;
						while (true)
						{
							if (iterY >= ySize)
							{
								var road = new Road((iterY - c.Y - 1) * tileLength, crossroad, null, lanes, Orientation.NorthSouth);
								crossroad.South = road;
								roadsList.Add(road);
								break;
							}
							char tile = tiles[iterY, c.X];
							if (tile == '-')
							{
								++iterY;
							}
							else if (tile == '=')
							{
								++iterY;
								lanes = 2;
							}
							else if (tile == 'X')
							{
								var endCrossroad = crossroadsDict[new Coordinates(iterY, c.X)];
								var road = new Road((iterY - c.Y) * tileLength, crossroad, endCrossroad, lanes, Orientation.NorthSouth);
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
						int lanes = 1;
						int iterX = c.X + 1;
						while (true)
						{
							if (iterX >= xSize)
							{
								var road = new Road((iterX - c.X - 1) * tileLength, crossroad, null, lanes, Orientation.EastWest);
								crossroad.East = road;
								roadsList.Add(road);
								break;
							}
							char tile = tiles[c.Y, iterX];
							if (tile == '-')
							{
								++iterX;
							}
							else if (tile == '=')
							{
								++iterX;
								lanes = 2;
							}
							else if (tile == 'X')
							{
								var endCrossroad = crossroadsDict[new Coordinates(c.Y, iterX)];
								var road = new Road((iterX - c.X) * tileLength, crossroad, endCrossroad, lanes, Orientation.EastWest);
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

				for (int i = 0; i < ySize; ++i)
				{
					int lanes = 1;
					int iterX = 0;
					while (true) {
						char tile = tiles[i, iterX];
						if (tile == '-')
						{
							++iterX;
						}
						else if (tile == '=')
						{
							++iterX;
							lanes = 2;
						}
						else if (tile == 'X')
						{
							if (iterX == 0)
							{
								break;
							}
							var endCrossroad = crossroadsDict[new Coordinates(i, iterX)];
							var road = new Road(iterX * tileLength, null, endCrossroad, lanes, Orientation.EastWest);
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

				for (int i = 0; i < xSize; ++i)
				{
					int lanes = 1;
					int iterY = 0;
					while (true)
					{
						char tile = tiles[iterY, i];
						if (tile == '-')
						{
							++iterY;
						}
						else if (tile == '=')
						{
							++iterY;
							lanes = 2;
						}
						else if (tile == 'X')
						{
							if (iterY == 0)
							{
								break;
							}
							var endCrossroad = crossroadsDict[new Coordinates(iterY, i)];
							var road = new Road(iterY * tileLength, null, endCrossroad, lanes, Orientation.NorthSouth);
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

				problemInstance.Crossroads = new List<Crossroad>(crossroadsDict.Values);
				problemInstance.Roads = roadsList;
			}

			catch (Exception e)
			{
				Console.Out.Write(e.StackTrace);
			}
		}

	    public static void FillRoutes(ProblemInstance problemInstance, String inputFileName)
	    {
            var serializer = new XmlSerializer(typeof(RoutesList));

            using (var reader = new StreamReader(inputFileName))
            {
                var routesSerialized = (RoutesList)serializer.Deserialize(reader);
                foreach (var route in routesSerialized.Routes)
                {
                    List<Road> roads = new List<Road>();
                    var currentDirection = Direction.NotSet;
                    Crossroad currentCrossroad = null;
                    foreach (var crossroads in route.Crossroads)
                    {
                        if (crossroads.Direction != Direction.NotSet)
                        {
                            currentDirection = crossroads.Direction;
                        }
                        else
                        {
                            var nextCrossroad = problemInstance.Crossroads[crossroads.Crossroad - 1];
                            if (currentCrossroad != null)
                            {    
                                roads.AddRange(from road in currentCrossroad.Roads
                                    from _road in nextCrossroad.Roads
                                    where road.Id == _road.Id
                                    select road);
                            }
                            else
                            {
                                AddRoad(roads, currentDirection, nextCrossroad);
                                currentDirection = Direction.NotSet;
                            }
                            currentCrossroad = problemInstance.Crossroads[crossroads.Crossroad-1];
                        }
                    }
                    if (currentDirection != Direction.NotSet)
                    {
                        AddRoad(roads, currentDirection, currentCrossroad);
                    }
                    problemInstance.Routes.Add(new Route(roads, route.Rate));
                }
            }
            
	    }

        public static void AddRoad(List<Road> list, Direction direction, Crossroad nextCrossroad)
        {
            switch (direction)
            {
                case Direction.North:
                    list.Add(nextCrossroad.North);
                    break;
                case Direction.East:
                    list.Add(nextCrossroad.East);
                    break;
                case Direction.West:
                    list.Add(nextCrossroad.West);
                    break;
                case Direction.South:
                    list.Add(nextCrossroad.South);
                    break;
                default:
                    Console.WriteLine("Could not determine Direction");
                    break;
            }
        }
	}


    [Serializable()]
    [XmlRoot("RoutesList")]
    public class RoutesList
    {
        [XmlArray("Routes")]
        [XmlArrayItem("RouteData", typeof(RouteData))]
        public List<RouteData> Routes { get; set; }
    }

    [Serializable()]
    public class RouteData
    {
        [XmlArray("Crossroads")]
        [XmlArrayItem("CrossroadData", typeof(CrossroadData))]
        public List<CrossroadData> Crossroads { get; set; }
        public int Rate { get; set; }
    }

    [Serializable()]
    public class CrossroadData
    {
        public int Crossroad { get; set; }
        public Direction Direction = Direction.NotSet;
    }

    public enum Direction
    {
        South,
        East,
        West,
        North,
        NotSet
    }
}

