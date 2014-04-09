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
            //InputParser.FillRoutes(pi,"routes.xml");
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
			}

        }
    }
}
