using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public class CarHire
    {

        private int _carHireRef; //PrimaryKey for Databse, Required for database interaction
        private DateTime _startDate;
        private DateTime _endDate;
        private String _driver;

        public int CarHireRef
        {
            get
            {
                return _carHireRef;
            }
            set
            {
                _carHireRef = value;
            }
        }

        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                if (value == DateTime.MinValue || value == null)
                {
                    throw new ArgumentException("Please set a valid departure date!"); //WIP
                }

                _startDate = value;
            }
        }

        public DateTime EndDate
        {
            get
            {
                return _endDate;
            }
            set
            {
                _endDate = value;
            }
           
        }

        public String Driver
        {
            get
            {
                return _driver;
            }
            set
            {
                _driver = value;
            }
        }

    }
}
