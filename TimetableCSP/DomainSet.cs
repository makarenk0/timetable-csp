using System;
using System.Collections.Generic;
using System.Text;

namespace TimetableCSP
{
    class DomainSet
    {

        private List<String> _days;
        private List<String> _time;
        private List<String> _audiences;
        private List<String> _teachers;

        public DomainSet(List<String> days, List<String> time, List<String> audiences, List<String> teachers)
        {
            _days = new List<String>(days);
            _time = new List<String>(time);
            _audiences = new List<String>(audiences);
            _teachers = new List<String>(teachers);
        }
    }
}
