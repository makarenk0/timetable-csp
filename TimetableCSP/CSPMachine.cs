using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimetableGeneticGeneration
{
    class CSPMachine
    {
        private Timetable _timetable;
        private String _dataFilename;


        public CSPMachine(String dataFilename)
        {
            _dataFilename = dataFilename;
            _timetable = new Timetable(dataFilename);
           
        }

        public Timetable Timetable
        {
            get { return _timetable; }
        }

    }
}
