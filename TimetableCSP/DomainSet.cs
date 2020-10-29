using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using TimetableGeneticGeneration;
using static TimetableGeneticGeneration.Utilities;

namespace TimetableCSP
{
    class DomainSet
    {
        

        private List<String> _days;
        private List<String> _time;
        private List<int> _audiences;
        private List<String> _teachers;

        private static Dictionary<String, String> _subjectLecturer;
        private static List<int> _lectureAudiences;

        public static void LoadStaticLimitations(JsonElement element)
        {
            _subjectLecturer = Utilities.GetAsObjectJSON(element, "Subject_lecturer", "Subject");
            _lectureAudiences = new List<int>(Utilities.GetAsObjectJSON<int[]>(element, "AudienceForLectures"));
        }

        public List<String> Days
        {
            get { return _days; }
            set { _days = value; }
        }

        public List<String> Time
        {
            get { return _time; }
            set { _time = value; }
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

        public DomainSet(JsonElement element, String subject, LessonType type)
        {
            _days = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "WorkingDays"));
            _time = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "Lessons_time"));
            _audiences = new List<int>(Utilities.GetAsObjectJSON<int[]>(element, "Audience"));
            _teachers = new List<String>(Utilities.GetAsObjectJSON<string[]>(element, "Teacher"));

            CutOffStaticLimitations(subject, type);
        }

        public DomainSet()
        {
            _days = new List<String>();
            _time = new List<String>();
            _audiences = new List<int>();
            _teachers = new List<String>();
        }

        private void CutOffStaticLimitations(String subject, LessonType type)
        {
            if(type == LessonType.Lecture)
            {
                _audiences = _audiences.Where(x => _lectureAudiences.Contains(x)).ToList();
                _teachers = new List<String>() { _subjectLecturer[subject] };
            }
            
        }
    }
}
