using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;

namespace TimetableGeneticGeneration
{
    class Chromosome
    {
        private Dictionary<String, WorkingWeek> _timetable;

        private float _deviation = 0;  //deviation from normal timetable (if deviation is 0 that's a correct timetable)
        private float _likelihood = 0;

        public float Deviation
        {
            get { return _deviation; }
        }

        public float Likelihood
        {
            get { return _likelihood; }
            set { _likelihood = value; }
        }

        JsonElement root;
        

        public Chromosome(String dataFilename)
        {
            _timetable = new Dictionary<string, WorkingWeek>();
            root = new JsonElement();
            GenerateeChromosome(dataFilename);
        }

        public Chromosome(Chromosome chr)
        {
            _timetable = new Dictionary<string, WorkingWeek>(chr._timetable);
        }

        public Chromosome()
        {
            _timetable = new Dictionary<string, WorkingWeek>();
        }

        public int AmountOfWorkingDays()
        {
            return _timetable.ElementAt(0).Value._week.Count;
        }

        public int AmountOfSpecialties()
        {
            return _timetable.Count;
        }

        public List<String> Specialties()
        {
            return _timetable.Keys.ToList();
        }

        public override String ToString()
        {
            String toString = "";
            foreach(var spec in _timetable)
            {
                toString += String.Concat(spec.Key, " :\n");
                foreach (var day in spec.Value._week)
                {
                    toString += String.Concat(" ", day.Key, " :\n");
                    foreach (var hours in day.Value._day)
                    {
                        toString += String.Concat("  ", hours.Key, " : ", hours.Value.ToString(), "\n");
                    }
                }
            }
            return toString;
        }


        private void GenerateeChromosome(String dataFilename)
        {
            if (File.Exists(dataFilename))
            {
                string text = File.ReadAllText(dataFilename);
                using JsonDocument doc = JsonDocument.Parse(text);
                root = doc.RootElement;
                FillForSpecialties();
            }
            else
            {
                throw new FileNotFoundException(String.Concat(dataFilename, " doesn't exist!"));
            }
        }


        private void FillForSpecialties()
        {
            string[] arr = Utilities.GetAsObjectJSON<string[]>(root, "Specialty");
            foreach(var specialty in arr)
            {
                _timetable.Add(specialty, new WorkingWeek(specialty, root));
            }
        }

        public float ComputeDeviation()
        {
            
            for(int i = 0; i< _timetable.Count()-1; i++)
            {
                for(int j = i + 1; j< _timetable.Count(); j++)
                {
                    var weekSpec1 = _timetable.ElementAt(i).Value._week;  //weeks
                    var weekSpec2 = _timetable.ElementAt(j).Value._week;
                    for (int k = 0; k < weekSpec1.Count; k++)
                    {
                        var daySpec1 = weekSpec1.ElementAt(k).Value; //days
                        var daySpec2 = weekSpec2.ElementAt(k).Value;
                        for (int m = 0; m < daySpec1._day.Count; m++)  
                        {
                            var hourSpec1 = daySpec1._day.ElementAt(m).Value; //hours
                            var hourSpec2 = daySpec2._day.ElementAt(m).Value;
                            
                            CheckBetweenBoth(hourSpec1, hourSpec2);   //check conflicts between different specialties
                            CheckInCurrent(hourSpec1);  //check conflicts inside one specialty
                        }
                    } 
                }
            }
            return _deviation;
        }

        private void CheckBetweenBoth(Lesson hourSpec1, Lesson hourSpec2)
        {
            if (!hourSpec1.IsFree && !hourSpec2.IsFree)
            {
                if (hourSpec1.Teacher == hourSpec2.Teacher)
                {
                    ++_deviation;
                }
                if (hourSpec1.Audience == hourSpec2.Audience)
                {
                    ++_deviation;
                } 
            }

        }
        private void CheckInCurrent(Lesson hourSpec1)
        {
            if (!hourSpec1.IsFree)
            {
                if (hourSpec1.LessonType == Utilities.LessonType.Lecture && !Utilities._lectureAudiences.Contains(hourSpec1.Audience))
                {
                    ++_deviation;
                }
            }
        }


        public Chromosome[] doubleDaysCrossover(Chromosome secondParent, int crossoverLine, String specialty)
        {
       
            Chromosome first = new Chromosome(this);
            Chromosome second = new Chromosome(secondParent);

            for (int i = 0; _timetable.ElementAt(i).Value != _timetable[specialty]; i++)
            {
                second._timetable[second._timetable.ElementAt(i).Key] = _timetable.ElementAt(i).Value;

                first._timetable[_timetable.ElementAt(i).Key] = secondParent._timetable.ElementAt(i).Value;
            }

          

            WorkingWeek weekFirst = new WorkingWeek(_timetable[specialty]);
            WorkingWeek weekSecond = new WorkingWeek(secondParent._timetable[specialty]);
            for (int j = 0; j < crossoverLine; j++)
            {
                weekFirst._week[weekFirst._week.ElementAt(j).Key] = secondParent._timetable[specialty]._week.ElementAt(j).Value;
                weekSecond._week[weekSecond._week.ElementAt(j).Key] = _timetable[specialty]._week.ElementAt(j).Value;
            }
            first._timetable[specialty] = weekFirst;
            second._timetable[specialty] = weekSecond;

            return new Chromosome[] { first, second };

        }

        
    }
}
