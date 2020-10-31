using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableCSP
{
    class Value
    {
        private String _dayValue;
        private String _timeValue;
        private int _audienceValue;
        private String _teacherValue;

        private bool _empty = true;

        public bool Empty
        {
            get { return _empty; }
            set { _empty = value; }
        }

        public Value()
        {

        }

        public Value(Value value)
        {
            DayValue = value.DayValue;
            TimeValue = value.TimeValue;
            AudienceValue = value.AudienceValue;
            TeacherValue = value.TeacherValue;
            Empty = false;
        }

        public String DayValue
        {
            get { return _dayValue; }
            set { _dayValue = value; }
        }

        public String TimeValue
        {
            get { return _timeValue; }
            set { _timeValue = value; }
        }

        public int AudienceValue
        {
            get { return _audienceValue; }
            set { _audienceValue = value; }
        }

        public String TeacherValue
        {
            get { return _teacherValue; }
            set { _teacherValue = value; }
        }

        public override bool Equals(object obj)
        {
            var item = obj as Value;

            if (item == null)
            {
                return false;
            }
            bool sameDateTime = (_dayValue == item.DayValue) && (_timeValue == item.TimeValue);
            return (sameDateTime && (_audienceValue == item.AudienceValue)) && (sameDateTime && (_teacherValue == item.TeacherValue));
        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }
    }
}
