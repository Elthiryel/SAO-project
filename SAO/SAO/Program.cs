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
			InputParser.FillRoadsAndCrossroads(pi, "/home/elthiryel/Dokumenty/Mono/SAO-project/SAO/test.txt");
			foreach (Road r in pi.Roads)
			{
				Console.Out.Write("ROAD " + r.Id + ": " + r.Length);
				if (r.First != null)
				{
					Console.Out.Write("; first: " + r.First.X + ", " + r.First.Y);
				}
				if (r.Second != null)
				{
					Console.Out.Write("; second: " + r.Second.X + ", " + r.Second.Y);
				}
				Console.Out.Write( "; lanes: " + r.IncreasingLaneCount + ", " + r.DecreasingLaneCount);
				Console.Out.Write( "; orientation: " + r.Orientation);
				Console.Out.WriteLine();
			}
			foreach (Crossroad c in pi.Crossroads)
			{
				Console.Out.WriteLine("CROSSROAD " + c.Id + ": " + c.X + ", " + c.Y);
			}

        }
    }
}
