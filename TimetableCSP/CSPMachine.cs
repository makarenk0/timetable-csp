using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimetableGeneticGeneration
{
    class CSPMachine
    {
        private List<Timetable> _generation;
        private int _generationNum = 0;

        private int _startPopulation;

        private const int percentageOfMutations = 45;

        private float currentFitness = 0;
        private float previousFitness;

        private String _dataFilename;


        public CSPMachine(String dataFilename, int startPopulation = 4)
        {
            _startPopulation = startPopulation;
            _dataFilename = dataFilename;
            LoadStaticLimitations(dataFilename);
            _generation = new List<Timetable>();
            for (int i = 0; i< _startPopulation; i++)
            {
                Timetable start = new Timetable(dataFilename);
                _generation.Add(start);
            }
            
           

        }

        private void LoadStaticLimitations(String dataFilename)  //static limitations load here
        {
            Utilities.LoadLectureAudiences(dataFilename);
            Utilities.LoadRequiredLessonsSet(dataFilename);
        }

        //private void ComputeParametrs()
        //{
        //    float inversedCoeffsSum = 0;
        //    for (int i = 0; i < _generation.Count; i++)
        //    {
        //        float devi = _generation[i].ComputeDeviation();
        //        inversedCoeffsSum += devi == 0 ? 0 : 1 / devi;
        //    }

        //    previousFitness = currentFitness;
        //    int j = 1;
        //    currentFitness = 0;
        //    for (int i = 0; i < _generation.Count; i++, j++)
        //    {
        //        _generation[i].Likelihood = _generation[i].Deviation == 0 ? 100 : ((1 / _generation[i].Deviation) / inversedCoeffsSum) * 100;
        //        currentFitness += _generation[i].Deviation;
        //    }
        //    currentFitness /= j;
        //}


        //private bool CheckForRightAnswers()
        //{
        //    for (int i = 0; i < _generation.Count; i++)
        //    {
        //        if(_generation[i].Likelihood == 100)
        //        {
        //            Console.WriteLine("Found answer:");
        //            Console.WriteLine(_generation[i]);
        //            return true;
        //        }
        //    }
        //    return false;

        //}

      

        public override string ToString()
        {
            String result = String.Concat("Generation number = ", _generationNum, "\n");
            int i = 1;
            foreach (var chromo in _generation)
            {
                result += String.Concat("Timetable ", i, " , Likelihood: ", chromo.Likelihood, " % , Fitness: ", chromo.Deviation, chromo.Deviation == 0 ? " ANSWER FOUND!!!" : "", "\n");
                ++i;
            }
            result += String.Concat("Average fitness: ", currentFitness, "\n");
            return result;
        }
    }
}
