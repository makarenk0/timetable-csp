using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Linq;
using TimetableCSP;

namespace TimetableGeneticGeneration
{
    class Timetable
    {
        private Dictionary<String, WorkingWeek> _timetableRandom;

        private Dictionary<Variable, DomainSet> _variables;

        JsonElement root;
        

        public Timetable(String dataFilename)
        {
            _timetableRandom = new Dictionary<String, WorkingWeek>();
            _variables = new Dictionary<Variable, DomainSet>();

            GenerateeTimetable(dataFilename);
            DomainSet.LoadStaticLimitations(root);
            FillVariables();

        }

        public Timetable(Timetable timetable)
        {
            _timetableRandom = new Dictionary<string, WorkingWeek>(timetable._timetableRandom);
        }

        public int AmountOfWorkingDays()
        {
            return _timetableRandom.ElementAt(0).Value._week.Count;
        }

        public int AmountOfSpecialties()
        {
            return _timetableRandom.Count;
        }

        public List<String> Specialties()
        {
            return _timetableRandom.Keys.ToList();
        }

        public override String ToString()
        {
            String toString = "";
            foreach(var spec in _timetableRandom)
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


        private void GenerateeTimetable(String dataFilename)
        {
            if (File.Exists(dataFilename))
            {
                string text = File.ReadAllText(dataFilename);
                using JsonDocument doc = JsonDocument.Parse(text);
                root = doc.RootElement.Clone();
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
                _timetableRandom.Add(specialty, new WorkingWeek(specialty, root));
            }
        }



        public void FillVariables()  //required for checking amount of specific lectures/practices
        {
            for (int i = 0; i < _timetableRandom.Count; i++)
            {
                WorkingWeek specialtyWeek = _timetableRandom.ElementAt(i).Value;
                for (int j = 0; j < specialtyWeek._week.Count; j++)
                {
                    WorkingDay day = specialtyWeek._week.ElementAt(j).Value;
                    for (int k = 0; k < day._day.Count; k++)
                    {
                        if (!day._day.ElementAt(k).Value.IsFree)
                        {  
                            Variable var = new Variable(day._day.ElementAt(k).Value);
                            _variables.Add(var, new DomainSet(root, var.Subject, var.LessonType));
                        }
                    }
                }
            }
        }

        public bool SolveByBacktracking()
        {
            int counter = 0, end = _variables.Count, prevStatus = 0;
            bool couldSolve = true;
            while(counter != end)
            {
                var pair = _variables.ElementAt(counter);
                if (pair.Value.Value.Empty)
                {
                    pair.Value.InitValue();
                }
                else
                {
                    if(pair.Value.TriedWholeDomain()) 
                    {
                        if(counter == 0)  // in case of cant create timetable without conflicts
                        {
                            couldSolve = false;
                            break;
                        }
                        pair.Value.Value.Empty = true;
                        --counter;
                        continue;
                    }
                    pair.Value.NextValue(prevStatus);
                    prevStatus = 0;
                }
                while (true)
                {
                    int checkResult = CheckIfFeets(pair.Value.Value, pair.Key.Group, _variables.Where(x => x.Key != pair.Key).ToDictionary(p => p.Key, p => p.Value));
                    if (checkResult == 0)
                    {
                        ++counter;
                        break;
                    }
                    if (pair.Value.TriedWholeDomain())
                    {
                        
                        pair.Value.Value.Empty = true;
                        --counter;
                        prevStatus = checkResult;
                        break;
                    }
                    pair.Value.NextValue(checkResult);
                }
            }
            Console.WriteLine(StringFromVars());
            return couldSolve;
        }

        private int CheckIfFeets(Value value, String group, Dictionary<Variable, DomainSet> other)   //return: 0 - no conflicts, 1 - teacher conflict, 2 - audience conflict  
        {
            other = other.Where(x => (x.Value.Value.DayValue == value.DayValue) &&
                                     (x.Value.Value.TimeValue == value.TimeValue) &&
                                     (!x.Value.Value.Empty)).ToDictionary(p => p.Key, p => p.Value);

            if((other.Where(x => (x.Value.Value.TeacherValue == value.TeacherValue)).ToDictionary(p => p.Key, p => p.Value)).Count != 0)
            {
                return 1;
            }
            else if((other.Where(x => (x.Value.Value.AudienceValue == value.AudienceValue)).ToDictionary(p => p.Key, p => p.Value)).Count != 0)
            {
                return 2;
            }
            else if ((other.Where(x => (x.Key.Group == group) || (x.Key.Group.Contains(group)) || (group.Contains(x.Key.Group))).ToDictionary(p => p.Key, p => p.Value)).Count != 0)
            {
                return 3;
            }
            return 0;
        }


        private String StringFromVars()
        {
            String res = "";
            foreach(var v in _variables)
            {
                res = String.Concat(res, v.Key, " : ", v.Value, " \n");
            }
            return res;
        }

    }
}
