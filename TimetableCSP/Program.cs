using System;
using System.Collections.Generic;
using System.Linq;

namespace TimetableGeneticGeneration
{
    class Program
    {
        static void Main(string[] args)
        {
            CSPMachine gen = new CSPMachine("../../../data.json", 15);
            gen.FindAnswer();
            
        }
    }
}
