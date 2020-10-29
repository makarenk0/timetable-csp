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

        private Dictionary<Lesson, DomainSet> _variables;

        JsonElement root;
        

        public Timetable(String dataFilename)
        {
            root = new JsonElement();
            _timetableRandom = new Dictionary<String, WorkingWeek>();
            _variables = new Dictionary<Lesson, DomainSet>();

            GenerateeTimetable(dataFilename);
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
                _timetableRandom.Add(specialty, new WorkingWeek(specialty, root));
            }
        }

        //public float ComputeDeviation()
        //{

        //    for(int i = 0; i< _timetable.Count()-1; i++)
        //    {
        //        for(int j = i + 1; j< _timetable.Count(); j++)
        //        {
        //            var weekSpec1 = _timetable.ElementAt(i).Value._week;  //weeks
        //            var weekSpec2 = _timetable.ElementAt(j).Value._week;
        //            for (int k = 0; k < weekSpec1.Count; k++)
        //            {
        //                var daySpec1 = weekSpec1.ElementAt(k).Value; //days
        //                var daySpec2 = weekSpec2.ElementAt(k).Value;
        //                for (int m = 0; m < daySpec1._day.Count; m++)  
        //                {
        //                    var hourSpec1 = daySpec1._day.ElementAt(m).Value; //hours
        //                    var hourSpec2 = daySpec2._day.ElementAt(m).Value;

        //                    CheckBetweenSpecialties(hourSpec1, hourSpec2);   //check conflicts between different specialties
        //                    AudiencesTypeFitness(hourSpec1);
        //                }
        //            } 
        //        }
        //    }

        //    RequiredLessonsFitness();
        //    return _deviation;
        //}

        //private void CheckBetweenSpecialties(Lesson hourSpec1, Lesson hourSpec2)
        //{
        //    if (!hourSpec1.IsFree && !hourSpec2.IsFree)
        //    {
        //        if (hourSpec1.Teacher == hourSpec2.Teacher)
        //        {
        //            ++_deviation;
        //        }
        //        if (hourSpec1.Audience == hourSpec2.Audience)
        //        {
        //            ++_deviation;
        //        } 
        //    }

        //}

        //private void AudiencesTypeFitness(Lesson hourSpec1)
        //{
        //    if (!hourSpec1.IsFree)
        //    {
        //        if (hourSpec1.LessonType == Utilities.LessonType.Lecture && !Utilities._lectureAudiences.Contains(hourSpec1.Audience))
        //        {
        //            ++_deviation;
        //        }
        //    }
        //}


        //private void RequiredLessonsFitness()
        //{
        //    Dictionary<String, List<Lesson>> currentLessonsSet = GetAllLessonsSet();
        //    Dictionary<String, List<Lesson>> requiredLessonsSet = new Dictionary<string, List<Lesson>>(Utilities._requiredLessonsSet);

        //    int lackLessons = 0;
        //    int overLessons = 0;
        //    for (int i = 0; i< currentLessonsSet.Count; i++)
        //    {
        //        String specialty = currentLessonsSet.ElementAt(i).Key;
        //        for(int j = 0; j< requiredLessonsSet[specialty].Count; j++)
        //        {
        //            if(!currentLessonsSet[specialty].Contains(requiredLessonsSet[specialty].ElementAt(j)))
        //            {
        //                ++lackLessons;
        //            }
        //        }
        //        int rawOverLessons = currentLessonsSet[specialty].Count - requiredLessonsSet[specialty].Count;
        //        overLessons += rawOverLessons >= 0 ? rawOverLessons : 0;
        //    }

        //    _deviation += lackLessons + overLessons;
        //}


        //public Timetable[] doubleDaysCrossover(Timetable secondParent, int crossoverLine, String specialty)
        //{

        //    Timetable first = new Timetable(this);
        //    Timetable second = new Timetable(secondParent);

        //    for (int i = 0; _timetable.ElementAt(i).Value != _timetable[specialty]; i++)
        //    {
        //        second._timetable[second._timetable.ElementAt(i).Key] = _timetable.ElementAt(i).Value;

        //        first._timetable[_timetable.ElementAt(i).Key] = secondParent._timetable.ElementAt(i).Value;
        //    }



        //    WorkingWeek weekFirst = new WorkingWeek(_timetable[specialty]);
        //    WorkingWeek weekSecond = new WorkingWeek(secondParent._timetable[specialty]);
        //    for (int j = 0; j < crossoverLine; j++)
        //    {
        //        weekFirst._week[weekFirst._week.ElementAt(j).Key] = secondParent._timetable[specialty]._week.ElementAt(j).Value;
        //        weekSecond._week[weekSecond._week.ElementAt(j).Key] = _timetable[specialty]._week.ElementAt(j).Value;
        //    }
        //    first._timetable[specialty] = weekFirst;
        //    second._timetable[specialty] = weekSecond;

        //    return new Timetable[] { first, second };

        //}



        public void FillVariables()  //required for checking amount of specific lectures/practices
        {
            //Dictionary<String, List<Lesson>> lessonsSet = new Dictionary<string, List<Lesson>>();
            for (int i = 0; i < _timetableRandom.Count; i++)
            {
                //lessonsSet.Add(_timetableRandom.ElementAt(i).Key, new List<Lesson>());
                WorkingWeek specialtyWeek = _timetableRandom.ElementAt(i).Value;
                for (int j = 0; j < specialtyWeek._week.Count; j++)
                {
                    WorkingDay day = specialtyWeek._week.ElementAt(j).Value;
                    for (int k = 0; k < day._day.Count; k++)
                    {
                        if (!day._day.ElementAt(k).Value.IsFree)
                        {
                            //lessonsSet[_timetableRandom.ElementAt(i).Key].Add(new Lesson(day._day.ElementAt(k).Value));
                            Lesson lesson = new Lesson(day._day.ElementAt(k).Value);
                            _variables.Add(lesson, new DomainSet(root, lesson.Subject, lesson.LessonType));
                        }
                    }
                }
            }
        }


    }
}
