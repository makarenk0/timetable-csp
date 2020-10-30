using System;
using System.Collections.Generic;
using System.Text;
using TimetableGeneticGeneration;
using static TimetableGeneticGeneration.Utilities;

namespace TimetableCSP
{
    class Variable
    {
        private LessonType _type;
        private String _subject;
        private String _group;

        public LessonType LessonType
        {
            get { return _type; }
        }

        public String Subject
        {
            get { return _subject; }
        }

        public String Group
        {
            get { return _group; }
        }

        public Variable(Lesson les)
        {
           
            _type = les.LessonType;
            _subject = les.Subject;
            _group = les.Group;     
        }

        public override string ToString()
        {
            return String.Concat(_type == LessonType.Lecture ? "Lecture" : "Practice", " , ", _subject, " , ", _group);
        }
    }
}
