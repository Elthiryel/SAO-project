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
				Console.Out.WriteLine("ROAD " + r.Id + ": " + r.Length + "; first: " + r.First.X + ", " + r.First.Y + 
					"; second: " + r.Second.X + ", " + r.Second.Y + "; lanes: " + r.IncreasingLaneCount + ", " + 
				    r.DecreasingLaneCount);
			}
			foreach (Crossroad c in pi.Crossroads)
			{
				Console.Out.WriteLine("CROSSROAD " + c.Id + ": " + c.X + ", " + c.Y);
			}

        }
    }
}
