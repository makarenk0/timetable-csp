using System;
using System.Collections.Generic;
using System.Linq;
using TimetableCSP;

namespace TimetableGeneticGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            CSPMachine gen = new CSPMachine("../../../large_data.json");
            Console.WriteLine(gen.Timetable.SolveByBacktracking());

        }
    }
}
