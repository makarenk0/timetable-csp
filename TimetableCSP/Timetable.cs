using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TimetableCSP;
using static TimetableGeneticGeneration.Utilities;

namespace TimetableGeneticGeneration
{
    class Timetable
    {

        private Dictionary<Variable, DomainSet> _variables;
        private List<int> _sequenceNumbers;
        private int _backSteps = 0;
        private int _allSteps = 0;

        JsonElement root;

        private const bool RANDOM_VARS_ORDER = false;
        const VarPickingHeuristic _pickingVarHeuristic = VarPickingHeuristic.LDH;

        List<Variable> _staticVarSequence = Utilities.Hard2Sequence();

        public enum VarPickingHeuristic 
        {
            NONE,
            MRV,   // Minimum remaining values
            LDH,    // Largest degree heuristic
        };


        public Timetable(String dataFilename)
        {
            _variables = new Dictionary<Variable, DomainSet>();
            _sequenceNumbers = new List<int>();

            GenerateeTimetable(dataFilename);
            DomainSet.LoadStaticLimitations(root);
            FillVariables();

        }

        public override String ToString()
        {
            //String toString = "";
            //foreach(var spec in _timetableRandom)
            //{
            //    toString += String.Concat(spec.Key, " :\n");
            //    foreach (var day in spec.Value._week)
            //    {
            //        toString += String.Concat(" ", day.Key, " :\n");
            //        foreach (var hours in day.Value._day)
            //        {
            //            toString += String.Concat("  ", hours.Key, " : ", hours.Value.ToString(), "\n");
            //        }
            //    }
            //}
            return "";
        }


        private void GenerateeTimetable(String dataFilename)
        {
            if (File.Exists(dataFilename))
            {
                string text = File.ReadAllText(dataFilename);
                using JsonDocument doc = JsonDocument.Parse(text);
                root = doc.RootElement.Clone();
            }
            else
            {
                throw new FileNotFoundException(String.Concat(dataFilename, " doesn't exist!"));
            }
        }



        public void FillVariables()  //required for checking amount of specific lectures/practices
        {
            List<Variable> allLessons = _staticVarSequence;
            if(_staticVarSequence == null)
            {
                allLessons = new List<Variable>();
                string[] specialties = Utilities.GetAsObjectJSON<string[]>(root, "Specialty");
                for (int i = 0; i < specialties.Length; i++)
                {
                    string[] groups = Utilities.GetAsObjectJSON<string[]>(root, String.Concat(specialties[i], "_", "groups"));
                    for (int j = 0; j < groups.Length + 1; j++)
                    {
                        String group = (j == groups.Length) ? String.Join(", ", groups) : groups[j];
                        LessonType type = (j == groups.Length) ? LessonType.Lecture : LessonType.Practice;
                        string[] subjects = Utilities.GetAsObjectJSON<string[]>(root, String.Concat(specialties[i], "_", "subjects"));
                        for (int k = 0; k < subjects.Length; k++)
                        {
                            allLessons.Add(new Variable(type, subjects[k], group));
                        }
                    }
                }
            }
            

            int n = allLessons.Count;
            for (int i = 0; i < n; i++)
            {
                int number = 0;
                if (RANDOM_VARS_ORDER)
                {
                    number = Utilities.ChooseRandomly(0, allLessons.Count);
                }
                _variables.Add(allLessons[number], new DomainSet(root, allLessons[number].Subject, allLessons[number].LessonType));
                allLessons.RemoveAt(number);
            }


        }

        public bool SolveByBacktracking()
        {
            int counter = 0, end = _variables.Count;
            bool couldSolve = true;
            while (counter != end)
            {
                ++_allSteps;
                var pair = _variables.ElementAt(HeuristicVariablePick(counter));
                if (pair.Value.Value.Empty)
                {
                    pair.Value.InitValue();
                }
                else
                {
                    if (pair.Value.TriedWholeDomain())
                    {
                        if (counter == 0)  // in case of cant create timetable without conflicts
                        {
                            couldSolve = false;
                            break;
                        }
                        pair.Value.Value.Empty = true;
                        --counter;
                        ++_backSteps;
                        continue;
                    }
                    pair.Value.NextValue();

                }
                while (true)
                {
                    if (CheckIfFeets(pair.Value.Value, pair.Key.Group, _variables.Where(x => x.Key != pair.Key).ToDictionary(p => p.Key, p => p.Value)))
                    {
                        ++counter;
                        break;
                    }
                    if (pair.Value.TriedWholeDomain())
                    {
                        pair.Value.Value.Empty = true;
                        --counter;
                        ++_backSteps;
                        break;
                    }
                    pair.Value.NextValue();
                }
            }
            Console.WriteLine(StringFromVars());
            return couldSolve;
        }

        private bool CheckIfFeets(Value value, String group, Dictionary<Variable, DomainSet> other)   //return: 0 - no conflicts, 1 - teacher conflict, 2 - audience conflict  
        {
            other = other.Where(x => (x.Value.Value.DayValue == value.DayValue) &&
                                     (x.Value.Value.TimeValue == value.TimeValue) &&
                                     (!x.Value.Value.Empty)).ToDictionary(p => p.Key, p => p.Value);

            if (((other.Where(x => (x.Value.Value.TeacherValue == value.TeacherValue)).ToDictionary(p => p.Key, p => p.Value)).Count != 0) ||
                ((other.Where(x => (x.Value.Value.AudienceValue == value.AudienceValue)).ToDictionary(p => p.Key, p => p.Value)).Count != 0) ||
                ((other.Where(x => (x.Key.Group == group) || (x.Key.Group.Contains(group)) ||
                (group.Contains(x.Key.Group))).ToDictionary(p => p.Key, p => p.Value)).Count != 0))
            {
                return false;
            }
            return true;
        }

        private int CountConflictsNumber(DomainSet set, Dictionary<Variable, DomainSet> other)
        {
            int conflictsN = 0;
            other = other.Where(x => !x.Value.Value.Empty).ToDictionary(p => p.Key, p => p.Value);
            foreach(var p in other)
            {
                if (set.CanConstruct(p.Value.Value))
                {
                    ++conflictsN;
                }
            }
            return conflictsN;
        }


        private int HeuristicVariablePick(int counter)
        {
            switch (_pickingVarHeuristic)
            {    
                case VarPickingHeuristic.MRV:
                    return MRVHeuristic(counter);
                case VarPickingHeuristic.LDH:
                    return LDHeuristic(counter);
                default:
                    return counter;

            }
              
        }

        private int MRVHeuristic(int counter)
        {
            Dictionary<Variable, DomainSet> _emptyVars = _variables.Where(x => x.Value.Value.Empty).ToDictionary(p => p.Key, p => p.Value);
            if (counter >= _sequenceNumbers.Count)
            {
                KeyValuePair<Variable, DomainSet> minPossiblePair = _emptyVars.First();
                int possibleValues = minPossiblePair.Value.PossibleDomainVariationsNumber() - CountConflictsNumber(minPossiblePair.Value, _variables);

                foreach (var p in _emptyVars)
                {
                    int newPossibleValues = p.Value.PossibleDomainVariationsNumber() - CountConflictsNumber(p.Value, _variables);
                    if (newPossibleValues < possibleValues)
                    {
                        possibleValues = newPossibleValues;
                        minPossiblePair = p;
                    }
                }
                _sequenceNumbers.Add(_variables.Keys.ToList().IndexOf(minPossiblePair.Key));
            }
            return _sequenceNumbers.ElementAt(counter);
        }

        private int LDHeuristic(int counter)
        {
            Dictionary<Variable, DomainSet> _emptyVars = _variables.Where(x => x.Value.Value.Empty).ToDictionary(p => p.Key, p => p.Value);
            if (counter >= _sequenceNumbers.Count)
            {
                KeyValuePair<Variable, DomainSet> minPossiblePair = _emptyVars.First();
                foreach (var p in _emptyVars)
                {
                    if (p.Key.LessonType == LessonType.Lecture || _emptyVars.Keys.ToList().IndexOf(p.Key) == _emptyVars.Count - 1)
                    {
                        minPossiblePair = p;
                    }
                }
                _sequenceNumbers.Add(_variables.Keys.ToList().IndexOf(minPossiblePair.Key));
            }
            return _sequenceNumbers.ElementAt(counter);
        }

        private String StringFromVars()
        {
            String res = "";
            foreach (var v in _variables)
            {
                res = String.Concat(res, v.Key, " : ", v.Value, " \n");
            }
            res = String.Concat(res, "Steps number: ", _allSteps, "\nBack steps done: ", _backSteps, "\n");
            return res;
        }

    }
}
