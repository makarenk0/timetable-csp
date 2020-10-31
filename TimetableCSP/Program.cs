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
            CSPMachine gen = new CSPMachine("../../../middle_size_data_backtracking_test.json");
            Console.WriteLine(gen.Timetable.SolveByBacktracking());

        }
    }
}
