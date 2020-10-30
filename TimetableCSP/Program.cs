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
            CSPMachine gen = new CSPMachine("../../../data.json");
            gen.Timetable.SolveByBacktracking();

        }
    }
}
