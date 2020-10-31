using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TimetableGeneticGeneration;
using static TimetableGeneticGeneration.Timetable;
using static TimetableGeneticGeneration.Utilities;

namespace TimetableCSP
{
    class DomainSet
    {
        //actual value
        private Value _value;


        //domains
        private List<String> _days;
        private List<String> _times;
        private List<int> _audiences;
        private List<String> _teachers;

        //static data and limitations
        private static Dictionary<String, String> _subjectLecturer;
        private static List<int> _lectureAudiences;
        private static DomainSet _fullDomain;

        private List<Value> _possibleValues;


        public static void LoadStaticLimitations(JsonElement element)
        {
            _subjectLecturer = Utilities.GetAsObjectJSON(element, "Subject_lecturer", "Subject");
            _lectureAudiences = new List<int>(Utilities.GetAsObjectJSON<int[]>(element, "AudienceForLectures"));
            _fullDomain = new DomainSet(element);
        }

        public int PossibleDomainVariationsNumber()
        {
            return _days.Count * _times.Count * _audiences.Count * _teachers.Count;
        }

        public bool OutOfDomain()
        {
            return _days.Count == 0 && _times.Count == 0 && _audiences.Count == 0 && _teachers.Count == 0;
        }

        public bool TriedWholeDomain(ValuePickingHeuristic heuristic, FilteringHeuristic filtering)  //TO DO: reduce code
        {
            if(filtering != FilteringHeuristic.NONE)
            {
                if (_possibleValues.Count == 0)
                {
                    FillPossibleValues();
                    return true;
                }
                return false;
            }

            switch (heuristic)
            {
                case ValuePickingHeuristic.LCV:
                    if(_possibleValues.Count == 0)
                    {
                        FillPossibleValues();
                        return true;
                    }
                    return false;
                default:
                    bool lastDayTime = _value.DayValue == _days.Last() && _value.TimeValue == _times.Last();
                    return (lastDayTime && _value.AudienceValue == _audiences.Last() && _value.TeacherValue == _teachers.Last());
            }
           
        }

        public void InitValue()
        {
            _value.DayValue = _days.First();
            _value.TimeValue = _times.First();
            _value.AudienceValue = _audiences.First();
            _value.TeacherValue = _teachers.First();
            _value.Empty = false;
        }

        public bool CanConstruct(Value val)
        {
            return _days.Contains(val.DayValue) && _times.Contains(val.TimeValue) && _teachers.Contains(val.TeacherValue) && _audiences.Contains(val.AudienceValue);
        }

        public void NextValue(ValuePickingHeuristic heuristic, FilteringHeuristic filtering, Dictionary<Variable, DomainSet> emptyVars)   
        {
            switch (heuristic)
            {
                case ValuePickingHeuristic.LCV:
                    LCVPicking(emptyVars);
                    break;
                default:
                    StepByStepPicking(filtering);
                    break;
            }

            switch (filtering)
            {
                case FilteringHeuristic.ForwardChecking:
                    foreach(var k in new List<Variable>(emptyVars.Keys))
                    {
                        emptyVars[k]._possibleValues = emptyVars[k]._possibleValues.Where(x => (!x.Equals(_value))).ToList();
                    }
                    break;
                default:
                    break;
            }
           
        }

        private void LCVPicking(Dictionary<Variable, DomainSet> emptyVars)  //TO DO optimize and check for bugs
        {
            if(_possibleValues.Count == 1)
            {
                _value = new Value(_possibleValues.ElementAt(0));
                _value.Empty = false;
                _possibleValues.Remove(_value);
            }
            else
            {
                int intersectCount = -1;
                foreach (var p in _possibleValues)
                {
                    int newIntersectCount = 0;
                    foreach (var k in emptyVars)
                    {
                        newIntersectCount += k.Value.PossibleValues.Count(el => el.Equals(p));
                    }

                    if (intersectCount == -1)
                    {
                        _value = new Value(p);
                        intersectCount = newIntersectCount;
                    }
                    else if (newIntersectCount < intersectCount)
                    {
                        _value = new Value(p);
                        _value.Empty = false;
                        intersectCount = newIntersectCount;
                    }
                    else if (newIntersectCount == 0)
                    {
                        _value = new Value(p);
                    }
                }
                _possibleValues.Remove(_value);
            }
            
        }

        private void StepByStepPicking(FilteringHeuristic filtering) //choosing next value (tries all possible variations of parametrs, if doesnt fit - backtracking will solve the issue)
        {
            if(filtering == FilteringHeuristic.NONE)
            {
                if (_value.TeacherValue != _teachers.Last())
                {
                    _value.TeacherValue = _teachers.ElementAt(_teachers.IndexOf(_value.TeacherValue) + 1);
                }
                else if (_value.AudienceValue != _audiences.Last())
                {
                    _value.TeacherValue = _teachers.First();   // resetting previous data element
                    _value.AudienceValue = _audiences.ElementAt(_audiences.IndexOf(_value.AudienceValue) + 1);
                }
                else if (_value.TimeValue != _times.Last())
                {
                    _value.AudienceValue = _audiences.First();  // resetting previous data element
                    _value.TimeValue = _times.ElementAt(_times.IndexOf(_value.TimeValue) + 1);
                }
                else if (_value.DayValue != _days.Last())
                {
                    _value.TimeValue = _times.First();   // resetting previous data element
                    _value.DayValue = _days.ElementAt(_days.IndexOf(_value.DayValue) + 1);
                }
            }
            else
            {
                _value = new Value(_possibleValues.First());
                _possibleValues.RemoveAt(0);
            }
            
        }

        public void SetDomainDefault(String subject, LessonType type)   //set default domain (all possible values)
        {
            _days = new List<String>(_fullDomain._days);
            _times = new List<String>(_fullDomain._times);
            _audiences = new List<int>(_fullDomain._audiences);
            _teachers = new List<String>(_fullDomain._teachers);
            CutOffStaticLimitations(subject, type);
        }


        public Value Value
        {
            get { return _value; }
            set { _value = value; }
        }
        

        public List<String> Days
        {
            get { return _days; }
            set { _days = value; }
        }

        public List<String> Times
        {
            get { return _times; }
            set { _times = value; }
        }

        public List<int> Audiences
        {
            get { return _audiences; }
            set { _audiences = value; }
        }

        public List<String> Teachers
        {
            get { return _teachers; }
            set { _teachers = value; }
        }

        public List<Value> PossibleValues
        {
            get { return _possibleValues; }
            set { _possibleValues = value; }
        }

        public DomainSet(JsonElement element)
        {
            _days = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "WorkingDays"));
            _times = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "Lessons_time"));
            _audiences = new List<int>(Utilities.GetAsObjectJSON<int[]>(element, "Audience"));
            _teachers = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "Teacher"));

            _value = new Value();
        }

        public DomainSet(JsonElement element, String subject, LessonType type, ValuePickingHeuristic heuristic, FilteringHeuristic filtering) : this(element)
        {
            CutOffStaticLimitations(subject, type);

            if(heuristic != ValuePickingHeuristic.NONE || filtering != FilteringHeuristic.NONE)
            {
                FillPossibleValues();
            }
        }

        private void FillPossibleValues()
        {
            _possibleValues = new List<Value>();
            InitValue();
            _possibleValues.Add(new Value(Value));
            while (!TriedWholeDomain(ValuePickingHeuristic.NONE, FilteringHeuristic.NONE))
            {
                StepByStepPicking(FilteringHeuristic.NONE);
                _possibleValues.Add(new Value(Value));
            }
            _value.Empty = true;
        }

        private void CutOffStaticLimitations(String subject, LessonType type)
        {
            if(type == LessonType.Lecture)
            {
                _audiences = _audiences.Where(x => _lectureAudiences.Contains(x)).ToList();
                _teachers = new List<String>() { _subjectLecturer[subject] };
            }
            
        }

        public override string ToString()
        {
            return String.Concat(_value.DayValue, " , ", _value.TimeValue, " , ", _value.AudienceValue, " , ", _value.TeacherValue);
        }
    }
}
